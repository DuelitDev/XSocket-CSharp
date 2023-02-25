using System.Net;
using XSocket.Core.Net;
using XSocket.Exception;

namespace XSocket.Protocol.Inet.Net;

/// <summary>
/// Represents a network endpoint as an IP address and a port number.
/// </summary>
public class IPAddressInfo : AddressInfo
{
    public IPAddressInfo(string address, ushort port)
    {
        if (!IPAddress.TryParse(address, out _)) 
            throw new InvalidParameterException("Address is invalid.");
        Address = address;
        Port = port;
    }

    public IPAddressInfo(IPEndPoint ipe)
    {
        Address = ipe.Address.ToString();
        Port = (ushort)ipe.Port;
    }

    public static implicit operator IPEndPoint(IPAddressInfo ipAddressInfo)
    {
        return new IPEndPoint(
            IPAddress.Parse(ipAddressInfo.Address), ipAddressInfo.Port);
    }

    public static explicit operator IPAddressInfo(IPEndPoint ipe) => new(ipe);

    /// <summary>
    /// Specifies the maximum value that can be assigned to the Port property.
    /// This field is read-only.
    /// </summary>
    /// <returns>65535</returns>
    public readonly ushort MaxPort = 65535;
    
    /// <summary>
    /// Specifies the minimum value that can be assigned to the Port property.
    /// This field is read-only.
    /// </summary>
    /// <returns>0</returns>
    public readonly ushort MinPort = 0;

    /// <summary>
    /// Gets the IP address of the AddressInfo.
    /// </summary>
    /// <returns>String of ip address</returns>
    public string Address { get; }
    
    /// <summary>
    /// Gets the port number of the AddressInfo.
    /// </summary>
    /// <returns>Port number</returns>
    public ushort Port { get; }
    
    /// <summary>
    /// Gets the address family.
    /// </summary>
    /// <returns>AddressFamily</returns>
    public override AddressFamily AddressFamily => 
        (AddressFamily)IPAddress.Parse(Address).AddressFamily;

    /// <summary>
    /// Returns a hash code for the current object.
    /// </summary>
    /// <returns>Hash code</returns>
    public override int GetHashCode()
    {
        return (Address, Port).GetHashCode();
    }
}