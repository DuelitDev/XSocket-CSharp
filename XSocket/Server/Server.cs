using System.Runtime.Serialization;
using System.Text;
using XSocket.Core.Listener;
using XSocket.Core.Net;
using XSocket.Events;
using XSocket.Exception;
using XSocket.Protocol.Protocol;
using XSocket.Util;

namespace XSocket.Server;

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

    public bool Running { get; private set; } = false;

    public bool Closed { get; private set; } = false;

    public AddressInfo LocalAddress => _listener.LocalAddress;

    public AddressFamily AddressFamily => _listener.AddressFamily;

    public ProtocolType ProtocolType => _listener.ProtocolType;
    
    public ServerEventWrapper Event { get; } = new();

    public async Task Run()
    {
        if (Running || Closed) return;
        Running = true;
        _listener.Run();
        _task = Task.Run(async () => { await Wrapper(); });
    }

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
    
    public async Task Broadcast(IEnumerable<byte> data)
    {
        if (!Running || Closed) throw new ServerClosedException();
        var tmp = data.ToArray();
        await Task.WhenAll(_clients.Values.Select(client => client.Send(tmp)));
    }

    public async Task BroadcastString(string str, string encode = "UTF-8")
    {
        await Broadcast(Encoding.GetEncoding(encode).GetBytes(str));
    }
}