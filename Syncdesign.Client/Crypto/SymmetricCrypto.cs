using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace Syncdesign.Client.Crypto;

/// <summary>
/// AES-GCM 对称加密（BouncyCastle）
/// </summary>
public static class SymmetricCrypto
{
    private const int NonceSize = 12; // GCM 标准 nonce
    private const int TagSize = 16;   // 128-bit

    /// <summary>
    /// AES-GCM 加密
    /// </summary>
    public static byte[] Encrypt(byte[] key, byte[] plaintext)
    {
        byte[] nonce = new byte[NonceSize];
        new SecureRandom().NextBytes(nonce);

        var cipher = new GcmBlockCipher(new Org.BouncyCastle.Crypto.Engines.AesEngine());
        var parameters = new AeadParameters(new KeyParameter(key), TagSize * 8, nonce, null);
        cipher.Init(true, parameters);

        byte[] output = new byte[cipher.GetOutputSize(plaintext.Length)];
        int len = cipher.ProcessBytes(plaintext, 0, plaintext.Length, output, 0);
        cipher.DoFinal(output, len);

        return Combine(nonce, output); // 返回 nonce + 密文 + tag
    }

    /// <summary>
    /// AES-GCM 解密
    /// </summary>
    public static byte[] Decrypt(byte[] key, byte[] encrypted)
    {
        if (encrypted.Length < NonceSize + TagSize)
            throw new ArgumentException("Encrypted data too short");

        byte[] nonce = new byte[NonceSize];
        byte[] cipherText = new byte[encrypted.Length - NonceSize];

        Buffer.BlockCopy(encrypted, 0, nonce, 0, NonceSize);
        Buffer.BlockCopy(encrypted, NonceSize, cipherText, 0, cipherText.Length);

        var cipher = new GcmBlockCipher(new Org.BouncyCastle.Crypto.Engines.AesEngine());
        var parameters = new AeadParameters(new KeyParameter(key), TagSize * 8, nonce, null);
        cipher.Init(false, parameters);

        byte[] output = new byte[cipher.GetOutputSize(cipherText.Length)];
        int len = cipher.ProcessBytes(cipherText, 0, cipherText.Length, output, 0);
        cipher.DoFinal(output, len);

        return output;
    }

    /// <summary>
    /// 拼接数组
    /// </summary>
    private static byte[] Combine(byte[] a, byte[] b)
    {
        byte[] result = new byte[a.Length + b.Length];
        Buffer.BlockCopy(a, 0, result, 0, a.Length);
        Buffer.BlockCopy(b, 0, result, a.Length, b.Length);
        return result;
    }
}
