namespace XSocket.Events;

/// <summary>
/// Contains state information and event data associated
/// with event of client accepted from server.
/// </summary>
public class OnAcceptEventArgs : EventArgs
{
    /// <summary>
    /// Contains state information and event data associated
    /// with event of client accepted from server.
    /// </summary>
    /// <returns>Client</returns>
    public Client.Client Client { get; init; }
    
    public OnAcceptEventArgs(Client.Client client)
    {
        Client = client;
    }
}
