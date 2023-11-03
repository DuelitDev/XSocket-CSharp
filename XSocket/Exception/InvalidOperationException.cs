namespace XSocket.Exception;

/// <summary>
/// The exception that is thrown when a method call is invalid
/// for the object's current state.
/// </summary>
public class InvalidOperationException : System.Exception
{
    public InvalidOperationException(string message = "") : 
        base(message, null) { }
}