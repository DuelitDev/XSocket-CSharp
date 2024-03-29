﻿using XSocket.Core.Net;
using XSocket.Protocol.Protocol;
using XSocket.Util;

namespace XSocket.Core.Handle;

/// <summary>
/// Provides client connections for network services.
/// </summary>
public interface IHandle
{
    /// <summary>
    /// Gets a value indicating whether
    /// the Socket for a Handle has been closed.
    /// </summary>
    /// <returns>bool</returns>
    public bool Closed { get; }
    
    /// <summary>
    /// Gets the local endpoint.
    /// </summary>
    /// <returns>AddressInfo</returns>
    public AddressInfo LocalAddress { get; }
    
    /// <summary>
    /// Gets the remote endpoint.
    /// </summary>
    /// <returns>AddressInfo</returns>
    public AddressInfo RemoteAddress { get; }
    
    /// <summary>
    /// Gets the address family of the Socket.
    /// </summary>
    /// <returns>AddressFamily</returns>
    public AddressFamily AddressFamily { get; }
    
    /// <summary>
    /// Gets the protocol type of the Handle.
    /// </summary>
    /// <returns>ProtocolType</returns>
    public ProtocolType ProtocolType { get; }
    
    /// <summary>
    /// Create a new Handle with the address info.
    /// </summary>
    /// <param name="address">AddressInfo</param>
    /// <returns>IHandle</returns>
    public Task<IHandle> Create(AddressInfo address);

    /// <summary>
    /// Closes the Socket connection.
    /// </summary>
    public Task Close();

    /// <summary>
    /// Generates a packet to be transmitted.
    /// </summary>
    /// <param name="data">Data to send</param>
    /// <param name="opcode">Operation code</param>
    /// <returns>Packet generator</returns>
    public IEnumerable<byte[]> Pack(byte[] data, OPCode opcode = OPCode.Data);
    
    /// <summary>
    /// Read the header of the received packet and get the data.
    /// </summary>
    /// <param name="packets">Received packet</param>
    /// <returns>See docstring</returns>
    public IEnumerable<int> Unpack(List<List<byte>> packets);
    
    /// <summary>
    /// Sends data to a connected Socket.
    /// </summary>
    /// <param name="data">Data to send</param>
    /// <param name="opcode">Operation Code</param>
    /// <returns></returns>
    public Task Send(byte[] data, OPCode opcode);
    
    /// <summary>
    /// Receives data from a bound Socket.
    /// </summary>
    /// <returns>Received data</returns>
    public Task<byte[]> Receive();
}