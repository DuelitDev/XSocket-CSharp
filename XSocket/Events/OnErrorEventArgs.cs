namespace XSocket.Events;

/// <summary>
/// Contains state information and event data associated
/// with error occurred event.
/// </summary>
public class OnErrorEventArgs : EventArgs
{
    /// <summary>
    /// Returns occurred error object.
    /// </summary>
    /// <returns>Exception</returns>
    public System.Exception Exception { get; init; }

    public OnErrorEventArgs(System.Exception exception)
    {
        Exception = exception;
    }
}
