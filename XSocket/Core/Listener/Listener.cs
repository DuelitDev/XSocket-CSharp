/*
 * XSocket (version: 0.0.2a)
 * 
 * Copyright 2022. DuelitDev all rights reserved.
 * 
 * This Library is distributed under the LGPL-2.1 License.
 */

using XSocket.Core.Handle;
using XSocket.Core.Net;
using XSocket.Protocol.Protocol;

namespace XSocket.Core.Listener;

/// <summary>
/// Listens for connections from network clients.
/// </summary>
public interface IListener
{
    /// <summary>
    /// Gets a value indicating whether
    /// the Socket for a Listener is running.
    /// </summary>
    /// <returns>bool</returns>
    public bool Running { get; }
    
    /// <summary>
    /// Gets a value indicating whether
    /// the Socket for a Listener has been closed.
    /// </summary>
    /// <returns>bool</returns>
    public bool Closed { get; }
    
    /// <summary>
    /// Gets the local endpoint.
    /// </summary>
    /// <returns>AddressInfo</returns>
    public AddressInfo LocalAddress { get; }
    
    /// <summary>
    /// Gets the address family of the Socket.
    /// </summary>
    /// <returns>AddressFamily</returns>
    public AddressFamily AddressFamily { get; }
    
    /// <summary>
    /// Gets the protocol type of the Socket.
    /// </summary>
    /// <returns>ProtocolType</returns>
    public ProtocolType ProtocolType { get; }
    
    /// <summary>
    /// Starts listening for incoming connection requests.
    /// </summary>
    public void Run();
    
    /// <summary>
    /// Closes the listener.
    /// </summary>
    public void Close();
    
    /// <summary>
    /// Establishes a connection to a remote host.
    /// </summary>
    /// <returns>Handle</returns>
    public Task<IHandle> Connect();
    
    /// <summary>
    /// Creates a new Handle for a newly created connection.
    /// </summary>
    /// <returns>Handle</returns>
    public Task<IHandle> Accept();
}