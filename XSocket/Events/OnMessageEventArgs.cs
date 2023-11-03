namespace XSocket.Events;

/// <summary>
/// Contains state information and event data associated
/// with message received event.
/// </summary>
public class OnMessageEventArgs : EventArgs
{
    private readonly List<byte[]> _data;
    
    /// <summary>
    /// Returns received message.
    /// </summary>
    /// <returns>Data of bytes</returns>
    public byte[] Data => _data[0];

    public OnMessageEventArgs(List<byte[]> data)
    {
        _data = data;
    }
}
