namespace XSocket.Exception;

public class InvalidOperationException : System.Exception
{
    public InvalidOperationException(string message) : base(message, null) { }
}