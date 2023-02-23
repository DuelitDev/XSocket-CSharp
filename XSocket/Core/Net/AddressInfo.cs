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