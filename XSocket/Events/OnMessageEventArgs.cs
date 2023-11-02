namespace XSocket.Events;

public class OnMessageEventArgs : EventArgs
{
    private readonly List<byte[]> _data;
    public byte[] Data => _data[0];

    public OnMessageEventArgs(List<byte[]> data)
    {
        _data = data;
    }
}
