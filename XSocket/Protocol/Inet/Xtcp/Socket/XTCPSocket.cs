using System.Net;
using System.Net.Sockets;
using XSocket.Core.Net;
using XSocket.Core.Socket;
using XSocket.Protocol.Inet.Net;

namespace XSocket.Protocol.Inet.Xtcp.Socket;

/// <summary>
/// Implements XTCP sockets interface.
/// </summary>
public class XTCPSocket : ISocket
{
    private readonly System.Net.Sockets.Socket _socket;

    public XTCPSocket(System.Net.Sockets.Socket socket)
    {
        _socket = socket;
    }

    /// <summary>
    /// Get a low-level socket.
    /// </summary>
    /// <returns>Low-level socket</returns>
    public object GetRawSocket => _socket;

    /// <summary>
    /// Gets the local address info.
    /// </summary>
    /// <returns>AddressInfo</returns>
    public AddressInfo LocalAddress => 
        new IPAddressInfo((IPEndPoint)_socket.LocalEndPoint!);

    /// <summary>
    /// Gets the remote address info.
    /// </summary>
    /// <returns>AddressInfo</returns>
    public AddressInfo RemoteAddress =>
        new IPAddressInfo((IPEndPoint)_socket.RemoteEndPoint!);
    
    /// <summary>
    /// Close the socket.
    /// </summary>
    public void Close()
    {
        _socket.Close();
    }
    
    /// <summary>
    /// Sends data to a connected Socket.
    /// </summary>
    /// <param name="data">Data to send</param>
    public async Task Send(byte[] data)
    {
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
        var buffer = Array.Empty<byte>();
        while(buffer.Length < length)
        {
            await Task.Factory.FromAsync(
                _socket.BeginReceive(
                    buffer, buffer.Length, length - buffer.Length, 
                    SocketFlags.None, null, _socket), 
                _socket.EndReceive);
            if (!exactly) break;
        }
        return buffer;
    }
}