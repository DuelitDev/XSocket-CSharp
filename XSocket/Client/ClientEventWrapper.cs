using XSocket.Events;

namespace XSocket.Client;

public class ClientEventWrapper
{
    public event EventHandler<OnOpenEventArgs>? OnOpen;
    public event EventHandler<OnCloseEventArgs>? OnClose;
    public event EventHandler<OnMessageEventArgs>? OnMessage;
    public event EventHandler<OnErrorEventArgs>? OnError;

    public async Task Open(Client sender, OnOpenEventArgs e)
    {
        var handler = OnOpen;
        if (handler == null) return;
        await Task.Run(() => handler(sender, e));
    }
    
    public async Task Close(Client sender, OnCloseEventArgs e)
    {
        var handler = OnClose;
        if (handler == null) return;
        await Task.Run(() => handler(sender, e));
    }
    
    public async Task Message(Client sender, OnMessageEventArgs e)
    {
        var handler = OnMessage;
        if (handler == null) return;
        await Task.Run(() => handler(sender, e));
    }
    
    public async Task Error(Client sender, OnErrorEventArgs e)
    {
        var handler = OnError;
        if (handler == null) return;
        await Task.Run(() => handler(sender, e));
    }
}