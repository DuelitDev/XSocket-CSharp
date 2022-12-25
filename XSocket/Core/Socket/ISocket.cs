/*
 * XSocket (version: 0.0.2a)
 * 
 * Copyright 2022. DuelitDev all rights reserved.
 * 
 * This Library is distributed under the LGPL-2.1 License.
 */

using XSocket.Core.Net;

namespace XSocket.Core.Socket;

/// <summary>
/// Implements sockets interface.
/// </summary>
public interface ISocket
{
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