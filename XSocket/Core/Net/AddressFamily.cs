/*
 * XSocket (version: 0.0.2a)
 * 
 * Copyright 2022. DuelitDev all rights reserved.
 * 
 * This Library is distributed under the LGPL-2.1 License.
 */

namespace XSocket.Core.Net;

/// <summary>
/// Specifies the addressing scheme
/// that a socket will use to resolve an address.
/// </summary>
public enum AddressFamily
{
    /// <summary>Unspecified address family.</summary>
    Unspecified = 0,
    /// <summary>Unix local to host address.</summary>
    Unix = 1,
    /// <summary>Address for IP version 4.</summary>
    InterNetwork = 2,
    /// <summary>Address for IP version 6.</summary>
    InterNetworkV6 = 23
}
