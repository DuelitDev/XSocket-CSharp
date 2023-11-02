namespace XSocket.Events;

public class OnAcceptEventArgs : EventArgs
{
    public Client.Client Client { get; init; }
    public OnAcceptEventArgs(Client.Client client)
    {
        Client = client;
    }
}
