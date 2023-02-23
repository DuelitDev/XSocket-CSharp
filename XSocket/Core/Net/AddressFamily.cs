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
