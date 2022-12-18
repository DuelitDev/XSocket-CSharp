/*
 * XSocket (version: 0.0.2a)
 * 
 * Copyright 2022. DuelitDev all rights reserved.
 * 
 * This Library is distributed under the LGPL-2.1 License.
 */


using System.Collections;
using System.Collections.Generic;
using XSocket.Core.Net;
using XSocket.Protocol.Protocol;

namespace XSocket.Core.Handle;

public interface IHandle
{
    public abstract AddressInfo LocalAddress { get; }
    
    public abstract AddressInfo RemoteAddress { get; }
    
    public abstract AddressFamily AddressFamily { get; }
    
    public abstract ProtocolType ProtocolType { get; }
    
    public bool Closed { get; }

    public static abstract IEnumerable<uint> Pack();

    public static abstract IEnumerable<uint> Unpack();
}