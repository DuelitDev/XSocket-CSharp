using XSocket.Core.Net;

namespace XSocket.Core.Socket;

/// <summary>
/// Implements sockets interface.
/// </summary>
public interface ISocket
{
    /// <summary>
    /// Gets a value indicating whether
    /// the Socket for a Socket has been closed.
    /// </summary>
    /// <returns>bool</returns>
    public bool Closed { get; }
    
    /// <summary>
    /// Get a low-level socket.
    /// </summary>
    /// <returns>Low-level socket</returns>
    public object GetRawSocket { get; }
    
    /// <summary>
    /// Gets the local address info.
    /// </summary>
    /// <returns>AddressInfo</returns>
    public AddressInfo LocalAddress { get; }
    
    /// <summary>
    /// Gets the remote address info.
    /// </summary>
    /// <returns>AddressInfo</returns>
    public AddressInfo RemoteAddress { get; }
    
    /// <summary>
    /// Create a new Socket with the address info.
    /// </summary>
    /// <param name="address">AddressInfo</param>
    /// <returns>ISocket</returns>
    public Task<ISocket> Create(AddressInfo address);
        
    /// <summary>
    /// Close the socket.
    /// </summary>
    public void Close();
    
    /// <summary>
    /// Sends data to a connected Socket.
    /// </summary>
    /// <param name="data">Data to send</param>
    public Task Send(byte[] data);
    
    /// <summary>
    /// Receives data from a bound Socket.
    /// </summary>
    /// <param name="length">The number of bytes to receive</param>
    /// <param name="exactly">Weather to read exactly</param>
    /// <returns>Received data</returns>
    public Task<byte[]> Receive(int length, bool exactly = false);
}