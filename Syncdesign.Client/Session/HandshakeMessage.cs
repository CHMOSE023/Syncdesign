namespace Syncdesign.Client.Session;
/// <summary>
/// 握手消息数据结构
/// </summary>
public sealed class HandshakeMessage
{
    public string DeviceId { get; set; } = default!;
    public byte[] X25519PublicKey { get; set; } = default!;
    public byte[] Signature { get; set; } = default!;
}