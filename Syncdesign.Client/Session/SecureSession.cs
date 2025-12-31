using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Syncdesign.Client.Crypto;
using System.Security.Cryptography;

namespace Syncdesign.Client.Session;

/// <summary>
/// 单会话逻辑（握手、AES 密钥）
/// </summary>
public sealed class SecureSession
{ 
    // ===== 身份（长期）=====
    private readonly string _localDeviceId;
    private readonly Ed25519PrivateKeyParameters _localIdentityPrivate;
    private readonly Ed25519PublicKeyParameters _localIdentityPublic;
   
    private Ed25519PublicKeyParameters? _remoteIdentityPublic;

    // ===== 会话（临时）=====
    private AsymmetricCipherKeyPair? _x25519KeyPair;
    private byte[]? _sharedSecret;
    public byte[] AesKey => _aesKey ?? throw new InvalidOperationException("Session not established");
    // ===== 对称加密 =====
    private byte[]? _aesKey;

    public SecureSessionState State { get; private set; }

    public SecureSession(string localDeviceId, Ed25519PrivateKeyParameters localIdentityPrivate)
    {
        _localDeviceId = localDeviceId;
        _localIdentityPrivate = localIdentityPrivate;
        _localIdentityPublic = localIdentityPrivate.GeneratePublicKey();

        State = SecureSessionState.Created;
    }


    /// <summary>
    /// 主动方：创建握手消息
    /// </summary>
    /// <returns></returns>
    public HandshakeMessage CreateHandshake()
    {
        EnsureState(SecureSessionState.Created);

        GenerateX25519KeyPair();

        byte[] publicKey =
            ((X25519PublicKeyParameters)_x25519KeyPair!.Public).GetEncoded();

        byte[] signature =
            SignatureUtil.Sign(publicKey, _localIdentityPrivate);

        State = SecureSessionState.HandshakeSent;

        return new HandshakeMessage
        {
            DeviceId = _localDeviceId,
            X25519PublicKey = publicKey,
            Signature = signature
        };
    }

    /// <summary>
    ///  处理对方握手（双方通用）
    /// </summary>
    public HandshakeMessage ProcessHandshake(
        HandshakeMessage remote,
        Ed25519PublicKeyParameters remoteIdentityPublic)
    {
        if (State == SecureSessionState.Closed)
            throw new InvalidOperationException("Session already closed");

        // 1. 验证签名（防 MITM）
        bool ok = SignatureUtil.Verify(
            remote.X25519PublicKey,
            remote.Signature,
            remoteIdentityPublic);

        if (!ok)
            throw new CryptographicException("Handshake signature invalid");

        _remoteIdentityPublic = remoteIdentityPublic;

        // 2. 如果本地还没生成会话密钥（被动方）
        if (_x25519KeyPair == null)
            GenerateX25519KeyPair();

        // 3. 计算共享密钥
        _sharedSecret = KeyAgreement.GenerateSharedSecret(
            ((X25519PrivateKeyParameters)_x25519KeyPair!.Private).GetEncoded(),
            remote.X25519PublicKey);

        // 4. 派生对称密钥
        DeriveAesKey();

        State = SecureSessionState.Established;

        // 5. 如果我是被动方，需要回一个握手消息
        byte[] publicKey =
            ((X25519PublicKeyParameters)_x25519KeyPair.Public).GetEncoded();

        byte[] signature =
            SignatureUtil.Sign(publicKey, _localIdentityPrivate);

        return new HandshakeMessage
        {
            DeviceId = _localDeviceId,
            X25519PublicKey = publicKey,
            Signature = signature
        };
    }


    /// <summary>
    /// 会话加密
    /// </summary>
    public byte[] Encrypt(byte[] plaintext)
    {
        EnsureState(SecureSessionState.Established);
        return SymmetricCrypto.Encrypt(_aesKey!, plaintext);
    }

    public byte[] Decrypt(byte[] ciphertext)
    {
        EnsureState(SecureSessionState.Established);
        return SymmetricCrypto.Decrypt(_aesKey!, ciphertext);
    }

    /// <summary>
    /// 内部工具
    /// </summary>
    private void GenerateX25519KeyPair()
    {
        var gen = new X25519KeyPairGenerator();
        gen.Init(new X25519KeyGenerationParameters(new SecureRandom()));
        _x25519KeyPair = gen.GenerateKeyPair();
    }

    private void DeriveAesKey()
    {
        // 最小实现：取前 32 字节
        _aesKey = new byte[32];
        Buffer.BlockCopy(_sharedSecret!, 0, _aesKey, 0, 32);
    }

    private void EnsureState(SecureSessionState expected)
    {
        if (State != expected)
            throw new InvalidOperationException(
                $"Invalid session state: {State}, expected: {expected}");
    }

    public void Close()
    {
        State = SecureSessionState.Closed;
        _sharedSecret = null;
        _aesKey = null;
        _x25519KeyPair = null;
    }
}

/**
 *  使用方式
    // A
    var sessionA = new SecureSession(deviceAId, deviceAPrivateKey);
    var msgA = sessionA.CreateHandshake();

    // → 通过网络发送 msgA →

    // B
    var sessionB = new SecureSession(deviceBId, deviceBPrivateKey);
    var msgB = sessionB.ProcessHandshake(msgA, deviceAPublicKey);

    // → 通过网络发送 msgB →

    // A
    sessionA.ProcessHandshake(msgB, deviceBPublicKey);

    // 之后就可以安全通信
    byte[] cipher = sessionA.Encrypt(data);
    byte[] plain = sessionB.Decrypt(cipher);

 * **/