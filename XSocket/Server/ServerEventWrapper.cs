using XSocket.Events;

namespace XSocket.Server;

/// <summary>
/// Represents the method that will handle an events.
/// </summary>
public class ServerEventWrapper
{
    public event EventHandler<OnOpenEventArgs>? OnOpen;
    public event EventHandler<OnCloseEventArgs>? OnClose;
    public event EventHandler<OnAcceptEventArgs>? OnAccept;
    public event EventHandler<OnErrorEventArgs>? OnError;

    public async Task Open(Server sender, OnOpenEventArgs e)
    {
        var handler = OnOpen;
        if (handler == null) return;
        await Task.Run(() => handler(sender, e));
    }

    public async Task Close(Server sender, OnCloseEventArgs e)
    {
        var handler = OnClose;
        if (handler == null) return;
        await Task.Run(() => handler(sender, e));
    }

    public async Task Accept(Server sender, OnAcceptEventArgs e)
    {
        var handler = OnAccept;
        if (handler == null) return;
        await Task.Run(() => handler(sender, e));
    }

    public async Task Error(Server sender, OnErrorEventArgs e)
    {
        var handler = OnError;
        if (handler == null) return;
        await Task.Run(() => handler(sender, e));
    }
}