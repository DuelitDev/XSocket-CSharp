using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices.JavaScript;
using XSocket.Core.Net;
using XSocket.Core.Socket;
using XSocket.Exception;
using XSocket.Protocol.Inet.Net;

namespace XSocket.Protocol.Inet.Xtcp.Socket;

/// <summary>
/// Implements XTCP sockets interface.
/// </summary>
public class XTCPSocket : ISocket
{
    private readonly System.Net.Sockets.Socket _socket;
    private readonly IPAddressInfo _localAddress;
    private readonly IPAddressInfo _remoteAddress;

    public XTCPSocket(System.Net.Sockets.Socket socket)
    {
        _socket = socket;
        _localAddress = new IPAddressInfo((IPEndPoint)_socket.LocalEndPoint!);
        _remoteAddress = new IPAddressInfo((IPEndPoint)_socket.RemoteEndPoint!);
    }
    
    /// <summary>
    /// Gets a value indicating whether
    /// the Socket for a Socket has been closed.
    /// </summary>
    /// <returns>bool</returns>
    public bool Closed { get; private set; }

    /// <summary>
    /// Get a low-level socket.
    /// </summary>
    /// <returns>Low-level socket</returns>
    public object GetRawSocket {
        get
        {
            if (Closed) throw new SocketClosedException();
            return _socket;
        } 
    }

    /// <summary>
    /// Gets the local address info.
    /// </summary>
    /// <returns>AddressInfo</returns>
    public AddressInfo LocalAddress => _localAddress;

    /// <summary>
    /// Gets the remote address info.
    /// </summary>
    /// <returns>AddressInfo</returns>
    public AddressInfo RemoteAddress => _remoteAddress;
    
    /// <summary>
    /// Create a new XTCPSocket with the IP address info.
    /// </summary>
    /// <param name="address">IPAddressInfo</param>
    /// <returns>XTCPSocket</returns>
    public static async Task<XTCPSocket> Create(IPAddressInfo address) 
    {
        var socket = new System.Net.Sockets.Socket(
            (System.Net.Sockets.AddressFamily)address.AddressFamily,
            SocketType.Stream, ProtocolType.Tcp);
        socket.LingerState = new LingerOption(true, 0);
        socket.Blocking = true;
        await socket.ConnectAsync((IPEndPoint)address);
        return new XTCPSocket(socket);
    } async Task<ISocket> ISocket.Create(AddressInfo address) {
        return await Create((address as IPAddressInfo)!);
    }
    
    /// <summary>
    /// Close the socket.
    /// </summary>
    public void Close()
    {
        if (Closed) return;
        _socket.Close();
        Closed = true;
    }
    
    /// <summary>
    /// Sends data to a connected Socket.
    /// </summary>
    /// <param name="data">Data to send</param>
    public async Task Send(byte[] data)
    {
        if (Closed) throw new SocketClosedException();
        var sentLength = 0;
        while (data.Length > sentLength)
        {
            sentLength += await _socket.SendAsync(data[sentLength..]);
        }
    }
    
    /// <summary>
    /// Receives data from a bound Socket.
    /// </summary>
    /// <param name="length">The number of bytes to receive</param>
    /// <param name="exactly">Weather to read exactly</param>
    /// <returns>Received data</returns>
    public async Task<byte[]> Receive(int length, bool exactly = false)
    {
        if (Closed) throw new SocketClosedException();
        var received = 0;
        var buffer = new byte[length];
        while(received < length)
        {
            received += await Task.Factory.FromAsync(
                _socket.BeginReceive(
                    buffer, received, length - received, 
                    SocketFlags.None, null, _socket), 
                _socket.EndReceive);
            if (!exactly) break;
        }
        return buffer;
    }
}