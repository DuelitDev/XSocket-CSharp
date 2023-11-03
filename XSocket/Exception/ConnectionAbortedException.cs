using System.Net.Sockets;

namespace XSocket.Exception;

/// <summary>
/// The exception that is thrown when connection is aborted.
/// </summary>
public class ConnectionAbortedException : SocketException
{
    public ConnectionAbortedException() : base(10053) { }
}