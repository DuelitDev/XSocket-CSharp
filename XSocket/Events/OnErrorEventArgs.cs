namespace XSocket.Events;

public class OnErrorEventArgs : EventArgs
{
    public System.Exception Exception { get; init; }

    public OnErrorEventArgs(System.Exception exception)
    {
        Exception = exception;
    }
}
