namespace XSocket.Exception;

/// <summary>
/// The exception that is thrown when
/// an invalid parameter is passed to a method.
/// </summary>
public class InvalidParameterException : System.Exception
{
    public InvalidParameterException(string message) : base(message, null) { }
}
