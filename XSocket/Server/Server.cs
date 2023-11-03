using System.Runtime.Serialization;
using System.Text;
using XSocket.Core.Listener;
using XSocket.Core.Net;
using XSocket.Events;
using XSocket.Exception;
using XSocket.Protocol.Protocol;
using XSocket.Util;

namespace XSocket.Server;

/// <summary>
/// Listens for connections from network clients.
/// </summary>
public class Server
{
    private readonly IListener _listener;
    private readonly ObjectIDGenerator _idGenerator = new();
    private readonly Dictionary<long, Client.Client> _clients = new();
    private readonly AsyncLock _wrapperLock = new();
    private readonly AsyncLock _collectorLock = new();
    private Task? _task;

    public Server(IListener listener)
    {
        _listener = listener;
    }

    /// <summary>
    /// Gets a value indicating whether Server is running.
    /// </summary>
    /// <returns>bool</returns>
    public bool Running { get; private set; } = false;

    /// <summary>
    /// Gets a value indicating whether Server has been closed.
    /// </summary>
    /// <returns>bool</returns>
    public bool Closed { get; private set; } = false;

    /// <summary>
    /// Gets the local endpoint.
    /// </summary>
    /// <returns>AddressInfo</returns>
    public AddressInfo LocalAddress => _listener.LocalAddress;

    /// <summary>
    /// Gets the address family of the Socket.
    /// </summary>
    /// <returns>AddressFamily</returns>
    public AddressFamily AddressFamily => _listener.AddressFamily;

    /// <summary>
    /// Gets the protocol type of the Listener.
    /// </summary>
    /// <returns>ProtocolType</returns>
    public ProtocolType ProtocolType => _listener.ProtocolType;
    
    /// <summary>
    /// Represents the method that will handle an events.
    /// </summary>
    /// <returns>ServerEventWrapper</returns>
    public ServerEventWrapper Event { get; } = new();

    /// <summary>
    /// Starts listening for incoming connection requests.
    /// </summary>
    public async Task Run()
    {
        if (Running || Closed) return;
        Running = true;
        _listener.Run();
        _task = Task.Run(async () => { await Wrapper(); });
    }

    /// <summary>
    /// Close all client connections and server.
    /// </summary>
    public async Task Close()
    {
        if (!Running || Closed) return;
        Closed = true;
        await _task!;
        await Task.WhenAll(_clients.Select(client => client.Value.Close()));
        _listener.Close();
        Running = false;
    }

    private async Task Wrapper()
    {
        await Event.Open(this, new OnOpenEventArgs());
        while (!Closed)
        {
            try
            {
                var handle = await _listener.Accept();
                var client = new Client.Client(handle);
                client.Event.OnClose += Collector;
                using (await _wrapperLock.LockAsync())
                {
                    var cid = _idGenerator.GetId(client, out _);
                    _clients[cid] = client;
                }
                await client.Run();
                await Event.Accept(this, new OnAcceptEventArgs(client));
            }
            catch (System.Exception e)
            {
                await Event.Error(this, new OnErrorEventArgs(e));
            }
        }
        await Event.Close(this, new OnCloseEventArgs());
    }

    private async void Collector(object? sender, OnCloseEventArgs _)
    {
        using (await _collectorLock.LockAsync())
        {
            _clients.Remove(_idGenerator.GetId(sender!, out var _));
        }
    }
    
    /// <summary>
    /// Send data to all clients.
    /// </summary>
    /// <param name="data">Data to send</param>
    public async Task Broadcast(IEnumerable<byte> data)
    {
        if (!Running || Closed) throw new ServerClosedException();
        var tmp = data.ToArray();
        await Task.WhenAll(_clients.Values.Select(client => client.Send(tmp)));
    }

    /// <summary>
    /// Send string to all clients.
    /// </summary>
    /// <param name="str">String to send</param>
    /// <param name="encode">String encoding</param>
    public async Task BroadcastString(string str, string encode = "UTF-8")
    {
        await Broadcast(Encoding.GetEncoding(encode).GetBytes(str));
    }
}