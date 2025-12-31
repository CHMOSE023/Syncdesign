using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Syncdesign.Client.Crypto;
using Syncdesign.Client.Identity;
using Syncdesign.Client.Session;
using System.Text;

namespace Syncdesign.Client.Client;


public class Client
{
    static void Main()
    {
        /** 
         * Ed25519  → 身份认证（签名）
         * X25519   → 密钥交换（前向安全）
         * SHA-256  → 指纹 / KDF 基础
         * AES-GCM  → 对称加密 + 完整性
         */

        Console.WriteLine("==== 1. 加载设备身份（Ed25519） ====");

        // 模拟 A / B 两台设备
        var deviceA = LoadDevice();
        var deviceB = LoadDevice();

        Console.WriteLine($"Device A ID: {deviceA.DeviceId}");
        Console.WriteLine($"Device B ID: {deviceB.DeviceId}");

        Console.WriteLine();
        Console.WriteLine("==== 2. A → B 发送带签名的 X25519 公钥 ====");

        // A 生成会话密钥
        var xGenA = new X25519KeyPairGenerator();
        xGenA.Init(new X25519KeyGenerationParameters(new SecureRandom()));
        var xA = xGenA.GenerateKeyPair();

        byte[] xAPublic = ((X25519PublicKeyParameters)xA.Public).GetEncoded();
        byte[] sigA = SignatureUtil.Sign(xAPublic, deviceA.PrivateKey);

        HandshakeMessage msgA = new()
        {
            DeviceId = deviceA.DeviceId,
            X25519PublicKey = xAPublic,
            Signature = sigA
        };

        Console.WriteLine("A 已发送握手消息");

        Console.WriteLine();
        Console.WriteLine("==== 3. B 验证 A，并回复自己的公钥 ====");

        bool verifyA = SignatureUtil.Verify(
            msgA.X25519PublicKey,
            msgA.Signature,
            deviceA.PublicKey
        );

        if (!verifyA)
            throw new Exception("A 身份验证失败");

        Console.WriteLine("B 验证 A 成功");

        // B 生成会话密钥
        var xGenB = new X25519KeyPairGenerator();
        xGenB.Init(new X25519KeyGenerationParameters(new SecureRandom()));
        var xB = xGenB.GenerateKeyPair();

        byte[] xBPublic = ((X25519PublicKeyParameters)xB.Public).GetEncoded();
        byte[] sigB = SignatureUtil.Sign(xBPublic, deviceB.PrivateKey);

        HandshakeMessage msgB = new()
        {
            DeviceId = deviceB.DeviceId,
            X25519PublicKey = xBPublic,
            Signature = sigB
        };

        Console.WriteLine("B 已回复握手消息");

        Console.WriteLine();
        Console.WriteLine("==== 4. A 验证 B，并计算共享密钥 ====");

        bool verifyB = SignatureUtil.Verify(
            msgB.X25519PublicKey,
            msgB.Signature,
            deviceB.PublicKey
        );

        if (!verifyB)
            throw new Exception("B 身份验证失败");

        Console.WriteLine("A 验证 B 成功");

        byte[] sharedA = KeyAgreement.GenerateSharedSecret(
            ((X25519PrivateKeyParameters)xA.Private).GetEncoded(),
            msgB.X25519PublicKey
        );

        byte[] sharedB = KeyAgreement.GenerateSharedSecret(
            ((X25519PrivateKeyParameters)xB.Private).GetEncoded(),
            msgA.X25519PublicKey
        );

        Console.WriteLine($"共享密钥一致: {AreEqual(sharedA, sharedB)}");

        Console.WriteLine();
        Console.WriteLine("==== 5. 使用共享密钥进行加密通信 ====");

        byte[] aesKey = new byte[32];
        Buffer.BlockCopy(sharedA, 0, aesKey, 0, 32);

        string text = "Hello Secure Syncdesign";
        byte[] encrypted = SymmetricCrypto.Encrypt(aesKey, Encoding.UTF8.GetBytes(text));
        byte[] decrypted = SymmetricCrypto.Decrypt(aesKey, encrypted);

        Console.WriteLine("原文: " + text);
        Console.WriteLine("解密: " + Encoding.UTF8.GetString(decrypted));

        Console.WriteLine();
        Console.WriteLine("==== 测试完成 ====");
        Console.ReadLine();
    }

    private static (string DeviceId, Ed25519PrivateKeyParameters PrivateKey, Ed25519PublicKeyParameters PublicKey) LoadDevice()
    {
        // 1. 从本地加载或生成长期身份私钥
        string pem = KeyStorage.LoadOrCreatePrivateKey();

        // 2. 计算 Device ID（非法会直接抛异常）
        string deviceId = SyncdesignDeviceId.FromPrivateKeyPem(pem);

        // 3. 解析 Ed25519 私钥
        using var reader = new StringReader(pem);
        var pemReader = new Org.BouncyCastle.OpenSsl.PemReader(reader);
        object obj = pemReader.ReadObject();

        Ed25519PrivateKeyParameters privateKey = obj switch
        {
            AsymmetricKeyParameter akp when akp.IsPrivate =>
                (Ed25519PrivateKeyParameters)akp,

            AsymmetricCipherKeyPair kp =>
                (Ed25519PrivateKeyParameters)kp.Private,

            PrivateKeyInfo pki =>
                (Ed25519PrivateKeyParameters)PrivateKeyFactory.CreateKey(pki),

            _ => throw new InvalidOperationException("Unsupported private key format")
        };

        // 4. 派生公钥
        Ed25519PublicKeyParameters publicKey = privateKey.GeneratePublicKey();

        return (deviceId, privateKey, publicKey);
    }
    private void MITMBug()
    {
        Console.WriteLine("==== 1. 读取 / 生成设备身份 ====");

        // ✅ 只从 KeyStorage 入口拿私钥
        string pem = KeyStorage.LoadOrCreatePrivateKey();
        Console.WriteLine(pem);

        string deviceId = SyncdesignDeviceId.FromPrivateKeyPem(pem);
        Console.WriteLine("Device ID:");
        Console.WriteLine(deviceId);

        Console.WriteLine();
        Console.WriteLine("==== 2. 模拟两台设备（X25519 密钥交换） ====");

        /**
        //！ 1.A 与 B 交换各自的公钥
                   设备 A                           设备 B
          ------------------------------------------------
          生成 (a_priv, a_pub)
          发送 a_pub  ------------------->
                                             生成 (b_priv, b_pub)
                               <------------ 发送 b_pub
          sharedA = a_priv × b_pub
          sharedB = b_priv × a_pub
       */

        // 设备 A
        var rng = new SecureRandom();
        var genA = new X25519KeyPairGenerator();
        genA.Init(new X25519KeyGenerationParameters(rng));
        var kpA = genA.GenerateKeyPair();
        // byte[] messageFromA = ((X25519PublicKeyParameters)kpA.Public).GetEncoded();
        // byte[] aPublicKeyFromNetwork = messageFromB;

        // MITM 中间人替换公钥 攻击者拦截 A - 拦截替换 → B 的公钥 


        // 设备 B
        var genB = new X25519KeyPairGenerator();
        genB.Init(new X25519KeyGenerationParameters(rng));
        var kpB = genB.GenerateKeyPair();
        // byte[] messageFromB = ((X25519PublicKeyParameters)kpB.Public).GetEncoded();
        // byte[] aPublicKeyFromNetwork = messageFromA;
        // byte[] messageFromB = ((X25519PublicKeyParameters)kpB.Public).GetEncoded();

        byte[] sharedA = KeyAgreement.GenerateSharedSecret(
            ((X25519PrivateKeyParameters)kpA.Private).GetEncoded(),
            ((X25519PublicKeyParameters)kpB.Public).GetEncoded()
        );

        byte[] sharedB = KeyAgreement.GenerateSharedSecret(
            ((X25519PrivateKeyParameters)kpB.Private).GetEncoded(),
            ((X25519PublicKeyParameters)kpA.Public).GetEncoded()
        );

        Console.WriteLine("共享密钥是否一致: " + AreEqual(sharedA, sharedB));

        Console.WriteLine();
        Console.WriteLine("==== 3. AES-GCM 加密 / 解密测试 ====");

        // 测试用：取前 32 字节
        byte[] aesKey = new byte[32];
        Buffer.BlockCopy(sharedA, 0, aesKey, 0, 32);

        string message = "Hello Syncdesign Secure World";
        byte[] plaintext = Encoding.UTF8.GetBytes(message);

        byte[] encrypted = SymmetricCrypto.Encrypt(aesKey, plaintext);

        Console.WriteLine("密文: " + Encoding.UTF8.GetString(encrypted));

        byte[] decrypted = SymmetricCrypto.Decrypt(aesKey, encrypted);

        Console.WriteLine("原文: " + message);
        Console.WriteLine("解密: " + Encoding.UTF8.GetString(decrypted));

        Console.WriteLine();
        Console.WriteLine("==== 测试完成 ====");
        Console.ReadLine();
    }

    private static bool AreEqual(byte[] a, byte[] b)
    {
        if (a.Length != b.Length) return false;
        for (int i = 0; i < a.Length; i++)
            if (a[i] != b[i]) return false;
        return true;
    }

    /** 
     *            架构概览
     *  ┌─────────────────────────────┐
        │        应用层 (App Layer)   │
        │  - 消息处理逻辑             │
        │  - 文件 / 命令 / 数据解析  │
        └─────────────┬───────────────┘
                      │
                      ▼
        ┌─────────────────────────────┐
        │      会话层 (Session Layer) │
        │  SecureSessionManager       │
        │  - 管理 SecureSession      │
        │  - 会话超时 / 失效 /重建   │
        │  - 会话加密 / 解密          │
        └─────────────┬───────────────┘
                      │
                      ▼
        ┌─────────────────────────────┐
        │   握手层 (Handshake Layer)  │
        │  - Ed25519 身份验证         │
        │  - X25519 会话密钥交换      │
        │  - 消息签名/验证            │
        └─────────────┬───────────────┘
                      │
                      ▼
        ┌─────────────────────────────┐
        │    加密层 (Crypto Layer)    │
        │  SymmetricCrypto (AES-GCM)  │
        │  - 消息加解密               │
        │  - Nonce / IV 管理          │
        └─────────────┬───────────────┘
                      │
                      ▼
        ┌─────────────────────────────┐
        │      网络层 (Transport)     │
        │  - UDP Socket / Datagram    │
        │  - 消息打包/解包            │
        │  - 包序号 / ACK / 重传      │
        └─────────────────────────────┘

    [App Layer]  -----> Encrypt -----> [Session Manager] -----> [Handshake/AES-GCM] -----> [UDP Socket] -----> [Network]

    [Network] -----> Receive -----> [Handshake/AES-GCM] -----> Decrypt -----> [Session Manager] -----> [App Layer]

     */
}
