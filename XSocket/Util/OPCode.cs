namespace XSocket.Util;

/// <summary>
/// Specifies the data type.
/// </summary>
public enum OPCode
{
    Continuation = 0x0,
    Data = 0x2,
    ConnectionClose = 0x8
}