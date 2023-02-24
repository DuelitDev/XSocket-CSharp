using System.Net.Sockets;

namespace XSocket.Exception;

public class ConnectionResetException : SocketException
{
    public ConnectionResetException() : base(10054) { }
}