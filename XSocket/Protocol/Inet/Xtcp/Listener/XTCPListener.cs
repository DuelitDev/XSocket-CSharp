using System.Collections;
using System.Net;
using XSocket.Core.Handle;
using XSocket.Core.Listener;
using XSocket.Core.Net;
using XSocket.Protocol.Inet.Net;
using XSocket.Protocol.Inet.Xtcp.Handle;
using XSocket.Protocol.Inet.Xtcp.Socket;
using XSocket.Protocol.Protocol;

namespace XSocket.Protocol.Inet.Xtcp.Listener;

public class XTCPListener : IListener
{
    private System.Net.Sockets.Socket? _socket;
    private readonly IPAddressInfo _address;
    /// <summary>
    /// Listens for connections from TCP network clients.
    /// </summary>
    public XTCPListener(IPAddressInfo address)
    {
        _address = address;
    }
    
    public XTCPListener(IPEndPoint ipe) : this(new IPAddressInfo(ipe)) { }

    public XTCPListener(Tuple<string, ushort> address) : 
        this(new IPAddressInfo(address.Item1, address.Item2)) { }
    
    public XTCPListener(string address, ushort port) : 
        this(new IPAddressInfo(address, port)) { }

    /// <summary>
    /// Gets a value indicating whether
    /// the Socket for a Listener is running.
    /// </summary>
    /// <returns>bool</returns>
    public bool Running { get; private set; }
    
    /// <summary>
    /// Gets a value indicating whether
    /// the Socket for a Listener has been closed.
    /// </summary>
    /// <returns>bool</returns>
    public bool Closed { get; private set; }
    
    /// <summary>
    /// Determines if there are pending connection requests.
    /// </summary>
    /// <returns>bool</returns>
    public bool Pending
    {
        get
        {
            if (!Running)
                throw new InvalidOperationException("Listener in not running.");
            var readList = new ArrayList { _socket };
            System.Net.Sockets.Socket.Select(readList, null, null, 0);
            return readList.Count > 0;
        }
    }

    /// <summary>
    /// Gets the local endpoint.
    /// </summary>
    /// <returns>IPAddressInfo</returns>
    public AddressInfo LocalAddress => _address;

    /// <summary>
    /// Gets the address family of the Socket.
    /// </summary>
    /// <returns>AddressFamily</returns>
    public AddressFamily AddressFamily => _address.AddressFamily;

    /// <summary>
    /// Gets the protocol type of the Socket.
    /// </summary>
    /// <returns>ProtocolType</returns>
    public ProtocolType ProtocolType => ProtocolType.Xtcp;
    
    /// <summary>
    /// Starts listening for incoming connection requests.
    /// </summary>
    public void Run()
    {
        _socket = new System.Net.Sockets.Socket(
            (System.Net.Sockets.AddressFamily)AddressFamily,
            System.Net.Sockets.SocketType.Stream, 
            System.Net.Sockets.ProtocolType.Tcp);
        _socket.LingerState = new System.Net.Sockets.LingerOption(true, 0);
        _socket.Blocking = true;
        _socket.Bind((IPEndPoint)_address);
        _socket.Listen();
        Running = true;
    }

    /// <summary>
    /// Closes the listener.
    /// </summary>
    public void Close()
    {
        if (!Running)
            throw new InvalidOperationException("Listener in not running.");
        _socket!.Close();
        Running = false;
        Closed = true;
    }

    /// <summary>
    /// Establishes a connection to a remote host.
    /// </summary>
    /// <returns>XTCPHandle</returns>
    public async Task<IHandle> Connect()
    {
        var socket = new System.Net.Sockets.Socket(
            (System.Net.Sockets.AddressFamily)AddressFamily,
            System.Net.Sockets.SocketType.Stream, 
            System.Net.Sockets.ProtocolType.Tcp);
        socket.LingerState = new System.Net.Sockets.LingerOption(true, 0);
        socket.Blocking = true;
        await socket.ConnectAsync((IPEndPoint)_address);
        return new XTCPHandle(new XTCPSocket(socket));
    }

    /// <summary>
    /// Creates a new Handle for a newly created connection.
    /// </summary>
    /// <returns>XTCPHandle</returns>
    public async Task<IHandle> Accept()
    {
        if (!Running)
            throw new InvalidOperationException("Listener in not running.");
        var socket = await _socket!.AcceptAsync();
        socket.LingerState = new System.Net.Sockets.LingerOption(true, 0);
        socket.Blocking = false;
        return new XTCPHandle(new XTCPSocket(socket));
    }
}