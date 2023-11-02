using System.Text;
using XSocket.Core.Handle;
using XSocket.Core.Listener;
using XSocket.Core.Net;
using XSocket.Events;
using XSocket.Exception;
using XSocket.Protocol.Protocol;
using XSocket.Util;

namespace XSocket.Client;

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

    public bool Running { get; private set; }

    public bool Closed { get; private set; }

    public AddressInfo LocalAddress => _handle?.LocalAddress ?? 
                                       _listener!.LocalAddress;

    public AddressInfo RemoteAddress
    {
        get
        {
            if (!Running) throw new Exception.InvalidOperationException(
                "Client is not connected.");
            return _handle!.RemoteAddress;
        }
    }
    
    public AddressFamily AddressFamily => _handle?.AddressFamily ?? 
                                          _listener!.AddressFamily;
    
    public ProtocolType ProtocolType => _handle?.ProtocolType ?? 
                                        _listener!.ProtocolType;

    public ClientEventWrapper Event { get; } = new();

    public async Task Run()
    {
        if (Running || Closed) return;
        Running = true;
        _task = Task.Run(async () => { await Handler(); });
        
    }
    
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
            catch (ConnectionResetException) { break; }
            catch (ConnectionAbortedException) { break; }
            catch (System.Exception e)
            {
                await Event.Error(this, new OnErrorEventArgs(e));
            }
        }
        await Event.Close(this, new OnCloseEventArgs());
    }

    public async Task Send(IEnumerable<byte> data)
    {
        if (!Running || Closed) throw new ClientClosedException();
        await _handle!.Send(data.ToArray(), OPCode.Data);
    }

    public async Task SendString(string str, string encode = "UTF-8")
    {
        await Send(Encoding.GetEncoding(encode).GetBytes(str));
    }
}