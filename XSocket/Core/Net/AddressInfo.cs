/*
 * XSocket (version: 0.0.2a)
 * 
 * Copyright 2022. DuelitDev all rights reserved.
 * 
 * This Library is distributed under the LGPL-2.1 License.
 */

namespace XSocket.Core.Net;

/// <summary>
/// Identifies a network address.
/// </summary>
public abstract class AddressInfo
{
    /// <summary>
    /// Gets the address family.
    /// </summary>
    /// <returns>AddressFamily</returns>
    public AddressFamily AddressFamily { get; }
    
    /// <summary>
    /// Returns a hash code for the current object.
    /// </summary>
    /// <returns>Hash code</returns>
    public abstract override int GetHashCode();
}