using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syncdesign.Client;
/// <summary>
/// 握手消息
/// </summary>
public sealed class HandshakeMessage
{
    public string DeviceId { get; set; } = default!;
    public byte[] X25519PublicKey { get; set; } = default!;
    public byte[] Signature { get; set; } = default!;
}