using XSocket.Core.Handle;
using XSocket.Core.Net;
using XSocket.Exception;
using XSocket.Protocol.Inet.Xtcp.Socket;
using XSocket.Protocol.Protocol;
using XSocket.Util;

namespace XSocket.Protocol.Inet.Xtcp.Handle;

/// <summary>
/// Provides client connections for TCP network services.
/// </summary>
public class XTCPHandle : IHandle
{
    private readonly XTCPSocket _socket;

    public XTCPHandle(XTCPSocket socket)
    {
        _socket = socket;
    }
    
    /// <summary>
    /// Gets a value indicating whether
    /// the Socket for a Handle has been closed.
    /// </summary>
    /// <returns>bool</returns>
    public bool Closed { get; private set; }

    /// <summary>
    /// Gets the local ip endpoint.
    /// </summary>
    /// <returns>IPAddressInfo</returns>
    public AddressInfo LocalAddress => _socket.LocalAddress;

    /// <summary>
    /// Gets the remote ip endpoint.
    /// </summary>
    /// <returns>IPAddressInfo</returns>
    public AddressInfo RemoteAddress => _socket.RemoteAddress;

    /// <summary>
    /// Gets the address family of the Socket.
    /// </summary>
    /// <returns>AddressFamily</returns>
    public AddressFamily AddressFamily => LocalAddress.AddressFamily;

    /// <summary>
    /// Gets the protocol type of the Handle.
    /// </summary>
    /// <returns>ProtocolType</returns>
    public ProtocolType ProtocolType => ProtocolType.Xtcp;
    
    /// <summary>
    /// Closes the Socket connection.
    /// </summary>
    public async Task Close()
    {
        await _close(false);
    }
    
    /// <summary>
    /// Sends a connection close signal to the peer and closes the socket.
    /// </summary>
    /// <param name="closeSocket">Whether to close the socket</param>
    private async Task _close(bool closeSocket)
    {
        if (closeSocket)
        {
            _socket.Close();
            return;
        }
        await Send(Array.Empty<byte>(), OPCode.ConnectionClose);
        Closed = true;
    }

    /// <summary>
    /// Generates a packet to be transmitted.
    /// </summary>
    /// <param name="data">Data to send</param>
    /// <param name="opcode">Data type</param>
    /// <returns>Packet generator</returns>
    public IEnumerable<byte[]> Pack(byte[] data, OPCode opcode)
    {
        const byte fin = 128;
        switch (data.Length)
        {
            case <= 125:
            {
                yield return 
                    new[] { (byte)(fin | (byte)opcode), (byte)data.Length }
                    .Concat(data).ToArray();
                break;
            }
            case <= 65535:
            {
                var len = BitConverter.IsLittleEndian ? (byte[])
                    BitConverter.GetBytes(data.Length).Reverse() : 
                    BitConverter.GetBytes(data.Length);
                yield return new[] { (byte)(fin | (byte)opcode), (byte)126 }
                        .Concat(len[^2..]).Concat(data).ToArray();
                break;
            }
            default:
            {
                for (var index = 0; index < data.Length; index += 65535)
                {
                    var end = 65535 > data.Length - index ? 
                        data.Length : index + 65535;
                    var segment = data[index..end];
                    var len = BitConverter.IsLittleEndian ? (byte[])
                        BitConverter.GetBytes(segment.Length).Reverse() : 
                        BitConverter.GetBytes(segment.Length);
                    yield return new[]
                    {
                        (byte)(index == 0 ? opcode : OPCode.Continuation),
                        (byte)126
                    }.Concat(len[^2..]).Concat(segment).ToArray();
                }
                yield return new[] { (byte)(fin | (byte)opcode), (byte)0 };
                break;
            }
        }
    }

    public IEnumerable<int> Unpack(List<List<byte>> packets)
    {
        for (var _ = 0; _ < packets.Count; _++)
        {
            yield return 2;
            var fin = packets[^1][0] >> 7;
            var rsv = ((127 & packets[^1][0]) >> 4) + (packets[^1][1] >> 7);
            var opcode = 15 & packets[^1][0];
            var size = 127 & packets[^1][1];
            var extend = size == 126;
            if (rsv != 0) throw new BrokenPipeException();
            packets[^1].Clear();
            if (extend)
            {
                yield return 2;
                size = BitConverter.ToInt16(packets[^1].ToArray());
            }
            else yield return 0;
            yield return size;
            yield return opcode;
            if (fin == 1) break;
        }
    }

    /// <summary>
    /// Sends data to a connected Socket.
    /// </summary>
    /// <param name="data">Data to send</param>
    /// <param name="opcode">Operation Code</param>
    public async Task Send(byte[] data, OPCode opcode = OPCode.Data)
    {
        foreach (var packet in Pack(data, opcode))
            await _socket.Send(packet);
    }

    /// <summary>
    /// Receives data from a bound Socket.
    /// </summary>
    /// <returns>Received data</returns>
    public async Task<byte[]> Receive()
    {
        OPCode? opcode = null;
        var temp = new List<List<byte>>();
        var counter = 0;
        foreach (var packet in Unpack(temp))
        {
            counter++;
            if (counter % 4 != 0)
            {
                temp[^1] = (await _socket.Receive(packet, true)).ToList();
                continue;
            }
            opcode ??= (OPCode)packet;
            if (opcode == OPCode.ConnectionClose || Closed)
            {
                if (Closed && opcode == OPCode.ConnectionClose)
                {
                    await _close(true);
                    throw new ConnectionAbortedException();
                }
                await _close(false);
            }
            else if (opcode == OPCode.Continuation) 
                throw new BrokenPipeException();
            temp.Add(new List<byte>());
        }
        return temp.SelectMany(x => x).ToArray();
    }
}