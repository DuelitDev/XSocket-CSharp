namespace XSocket.Exception;

public class InvalidParameterException : System.Exception
{
    public InvalidParameterException(string message) : base(message, null) { }
}
