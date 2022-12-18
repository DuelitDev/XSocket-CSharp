/*
 * XSocket (version: 0.0.2a)
 * 
 * Copyright 2022. DuelitDev all rights reserved.
 * 
 * This Library is distributed under the LGPL-2.1 License.
 */

using System.Threading.Tasks;
using XSocket.Core.Net;
using XSocket.Protocol.Protocol;

namespace XSocket.Core.Listener;

/// <summary>
/// Listens for connections from network clients.
/// </summary>
public abstract class Listener
{
    protected Socket.Socket? _socket = null;
    protected AddressInfo _address;
    protected bool _running = false;
    protected bool _closed = false;
    
    /// <summary>
    /// Gets a value indicating whether
    /// the Socket for a Listener is running.
    /// </summary>
    /// <returns>bool</returns>
    public bool Running => _running;
    
    /// <summary>
    /// Gets a value indicating whether
    /// the Socket for a Listener has been closed.
    /// </summary>
    /// <returns>bool</returns>
    public bool Closed => _closed;
    
    /// <summary>
    /// Gets the local endpoint.
    /// </summary>
    /// <returns>AddressInfo</returns>
    public abstract AddressInfo LocalAddress { get; }
    
    /// <summary>
    /// Gets the address family of the Socket.
    /// </summary>
    /// <returns>AddressFamily</returns>
    public abstract AddressFamily AddressFamily { get; }
    
    /// <summary>
    /// Gets the protocol type of the Socket.
    /// </summary>
    /// <returns>ProtocolType</returns>
    public abstract ProtocolType ProtocolType { get; }
    
    /// <summary>
    /// Starts listening for incoming connection requests.
    /// </summary>
    public abstract void Run();
    
    /// <summary>
    /// Establishes a connection to a remote host.
    /// </summary>
    /// <returns>Handle</returns>
    public abstract Task<Handle.Handle> Connect();
    
    /// <summary>
    /// Creates a new Handle for a newly created connection.
    /// </summary>
    /// <returns>Handle</returns>
    public abstract Task<Handle.Handle> Accept();
    
    /// <summary>
    /// Closes the listener.
    /// </summary>
    public abstract void Close();
}