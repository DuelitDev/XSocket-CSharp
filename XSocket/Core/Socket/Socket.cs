/*
 * XSocket (version: 0.0.2a)
 * 
 * Copyright 2022. DuelitDev all rights reserved.
 * 
 * This Library is distributed under the LGPL-2.1 License.
 */

using System.Threading.Tasks;
using XSocket.Core.Net;

namespace XSocket.Core.Socket;

/// <summary>
/// Implements sockets interface.
/// </summary>
public abstract class Socket
{
    /// <summary>
    /// Gets the local address info.
    /// </summary>
    /// <returns>AddressInfo</returns>
    public abstract AddressInfo LocalAddress { get; }
    
    /// <summary>
    /// Gets the remote address info.
    /// </summary>
    /// <returns>AddressInfo</returns>
    public abstract AddressInfo RemoteAddress { get; }
    
    /// <summary>
    /// Get a low-level socket.
    /// </summary>
    /// <returns>Low-level socket</returns>
    public abstract object GetRawSocket { get; }

    /// <summary>
    /// Sends data to a connected Socket.
    /// </summary>
    /// <param name="data">Data to send</param>
    public abstract Task Send(byte data);
    
    /// <summary>
    /// Receives data from a bound Socket.
    /// </summary>
    /// <param name="length">The number of bytes to receive</param>
    /// <param name="exactly">Weather to read exactly</param>
    /// <returns>Received data</returns>
    public abstract Task<byte> Receive(uint length, bool exactly = false);
    
    /// <summary>
    /// Close the socket.
    /// </summary>
    public abstract void Close();
}