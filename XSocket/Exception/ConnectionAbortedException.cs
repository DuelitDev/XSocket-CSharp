using System.Net.Sockets;

namespace XSocket.Exception;

public class ConnectionAbortedException : SocketException
{
    public ConnectionAbortedException() : base(10053) { }
}