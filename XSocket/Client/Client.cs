using System.Net.Sockets;
using System.Text;
using XSocket.Core.Handle;
using XSocket.Core.Listener;
using XSocket.Core.Net;
using XSocket.Events;
using XSocket.Exception;
using XSocket.Util;
using AddressFamily = XSocket.Core.Net.AddressFamily;
using ProtocolType = XSocket.Protocol.Protocol.ProtocolType;

namespace XSocket.Client;

/// <summary>
/// Provides client connections for network services.
/// </summary>
public class Client
{
    private readonly IListener? _listener;
    private IHandle? _handle;
    private Task? _task;

    public Client(IListener listener)
    {
        _listener = listener;
    }

    public Client(IHandle handle)
    {
        _handle = handle;
    }
    
    /// <summary>
    /// Gets a value indicating whether Client is running.
    /// </summary>
    /// <returns>bool</returns>
    public bool Running { get; private set; }

    /// <summary>
    /// Gets a value indicating whether Client has been closed.
    /// </summary>
    /// <returns>bool</returns>
    public bool Closed { get; private set; }

    /// <summary>
    /// Gets the local endpoint.
    /// </summary>
    /// <returns>AddressInfo</returns>
    public AddressInfo LocalAddress => _handle?.LocalAddress ?? 
                                       _listener!.LocalAddress;

    /// <summary>
    /// Gets the remote ip endpoint.
    /// </summary>
    /// <returns>AddressInfo</returns>
    public AddressInfo RemoteAddress
    {
        get
        {
            if (!Running) throw new Exception.InvalidOperationException(
                "Client is not connected.");
            return _handle!.RemoteAddress;
        }
    }
    
    /// <summary>
    /// Gets the address family of the Socket.
    /// </summary>
    /// <returns>AddressFamily</returns>
    public AddressFamily AddressFamily => _handle?.AddressFamily ?? 
                                          _listener!.AddressFamily;
    
    /// <summary>
    /// Gets the protocol type of the Listener.
    /// </summary>
    /// <returns>ProtocolType</returns>
    public ProtocolType ProtocolType => _handle?.ProtocolType ?? 
                                        _listener!.ProtocolType;

    /// <summary>
    /// Represents the method that will handle an events.
    /// </summary>
    /// <returns>ClientEventWrapper</returns>
    public ClientEventWrapper Event { get; } = new();
    
    /// <summary>
    /// Establishes a connection to a remote host.
    /// </summary>
    public async Task Run()
    {
        if (Running || Closed) return;
        Running = true;
        _task = Task.Run(async () => { await Handler(); });
        
    }
    
    /// <summary>
    /// Close the connection.
    /// </summary>
    public async Task Close()
    {
        if (!Running || Closed) return;
        await _handle!.Close();
        await _task!;
        Closed = true;
        Running = false;
    }
    
    private async Task Handler()
    {
        if (_handle == null)
        {
            _handle = await _listener!.Connect();
            await Event.Open(this, new OnOpenEventArgs());
        }
        while (!Closed)
        {
            try
            {
                var data = new List<byte[]> { await _handle.Receive() };
                await Event.Message(this, new OnMessageEventArgs(data));
            }
            catch (OperationControl) { }
            catch (SocketException) { break; }
            catch (System.Exception e)
            {
                await Event.Error(this, new OnErrorEventArgs(e));
            }
        }
        await Event.Close(this, new OnCloseEventArgs());
    }

    /// <summary>
    /// Send data to server.
    /// </summary>
    /// <param name="data">Data to send</param>
    public async Task Send(IEnumerable<byte> data)
    {
        if (!Running || Closed) throw new ClientClosedException();
        await _handle!.Send(data.ToArray(), OPCode.Data);
    }

    /// <summary>
    /// Send string to server.
    /// </summary>
    /// <param name="str">String to send</param>
    /// <param name="encode">String encoding</param>
    public async Task SendString(string str, string encode = "UTF-8")
    {
        await Send(Encoding.GetEncoding(encode).GetBytes(str));
    }
}