using Org.BouncyCastle.Crypto.Parameters;
using Syncdesign.Client.Crypto;
using Syncdesign.Client.Session;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Syncdesign.Client.UDP;

/// <summary>
/// UDP 安全通信封装
/// </summary>
public class UdpSecurePeer
{
    private readonly UdpClient _udp;
    private readonly SecureSessionManager _sessionManager;
    private readonly IPEndPoint _remote;

    private uint _sendCounter = 0;

    public UdpSecurePeer(
        string localDeviceId,
        Ed25519PrivateKeyParameters localPrivate,
        IPEndPoint remote)
    {
        _udp = new UdpClient(0); // 自动分配端口
        _sessionManager = new SecureSessionManager(localDeviceId, localPrivate);
        _remote = remote;
    }


    /// <summary>
    /// 发送消息
    /// </summary>
    public void Send(string remoteDeviceId, Ed25519PublicKeyParameters remotePublicKey, string text)
    {
        var session = _sessionManager.GetOrCreateSession(remoteDeviceId, remotePublicKey);

        // 每条消息派生 IV
        byte[] iv = BitConverter.GetBytes(_sendCounter++);
        if (iv.Length < 12)
        {
            var tmp = new byte[12];
            Buffer.BlockCopy(iv, 0, tmp, 12 - iv.Length, iv.Length);
            iv = tmp;
        }

        byte[] plaintext = Encoding.UTF8.GetBytes(text);

        // 使用 SecureSession 的 AES key
        byte[] cipher = SymmetricCrypto.Encrypt(session.AesKey, plaintext);

        // UDP 打包
        byte[] packet = new byte[4 + iv.Length + cipher.Length];
        Buffer.BlockCopy(BitConverter.GetBytes(_sendCounter), 0, packet, 0, 4);
        Buffer.BlockCopy(iv, 0, packet, 4, iv.Length);
        Buffer.BlockCopy(cipher, 0, packet, 4 + iv.Length, cipher.Length);

        _udp.Send(packet, packet.Length, _remote);
    }


    /// <summary>
    /// 接收消息
    /// </summary>
    public async Task ReceiveLoop(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            var result = await _udp.ReceiveAsync();
            byte[] data = result.Buffer;

            if (data.Length < 16) continue; // 最小长度检查

            // 解析
            uint blockNumber = BitConverter.ToUInt32(data, 0);
            byte[] iv = data.Skip(4).Take(12).ToArray();
            byte[] cipher = data.Skip(16).ToArray();

            // 查找 session（假设 DeviceId 已知或者在包头传 DeviceId）
            var session = _sessionManager.GetAllSessions().FirstOrDefault();
            if (session == null) continue;

            // 使用 SecureSession 的 AES key
            byte[] plaintext = SymmetricCrypto.Decrypt(session.AesKey, cipher);
            string text = Encoding.UTF8.GetString(plaintext);

            Console.WriteLine($"Received block {blockNumber}: {text}");
        }
    }
}
