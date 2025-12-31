using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace Syncdesign.Client;

/// <summary>
/// 对称加密
/// </summary>
public static class SymmetricCrypto
{
    private const int NonceSize = 12; // GCM 标准
    private const int TagSize = 16;   // 128-bit

    public static byte[] Encrypt(byte[] key, byte[] plaintext)
    {
        byte[] nonce = new byte[NonceSize];
        new SecureRandom().NextBytes(nonce);

        var cipher = new GcmBlockCipher(new Org.BouncyCastle.Crypto.Engines.AesEngine());
        var parameters = new AeadParameters(
            new KeyParameter(key),
            TagSize * 8,
            nonce,
            null
        );

        cipher.Init(true, parameters);

        byte[] output = new byte[cipher.GetOutputSize(plaintext.Length)];
        int len = cipher.ProcessBytes(plaintext, 0, plaintext.Length, output, 0);
        cipher.DoFinal(output, len);

        return Combine(nonce, output); // nonce + ciphertext + tag
    }

    public static byte[] Decrypt(byte[] key, byte[] encrypted)
    {
        byte[] nonce = new byte[NonceSize];
        byte[] cipherText = new byte[encrypted.Length - NonceSize];

        Buffer.BlockCopy(encrypted, 0, nonce, 0, NonceSize);
        Buffer.BlockCopy(encrypted, NonceSize, cipherText, 0, cipherText.Length);

        var cipher = new GcmBlockCipher(new Org.BouncyCastle.Crypto.Engines.AesEngine());
        var parameters = new AeadParameters(
            new KeyParameter(key),
            TagSize * 8,
            nonce,
            null
        );

        cipher.Init(false, parameters);

        byte[] output = new byte[cipher.GetOutputSize(cipherText.Length)];
        int len = cipher.ProcessBytes(cipherText, 0, cipherText.Length, output, 0);
        cipher.DoFinal(output, len);

        return output;
    }

    private static byte[] Combine(byte[] a, byte[] b)
    {
        var result = new byte[a.Length + b.Length];
        Buffer.BlockCopy(a, 0, result, 0, a.Length);
        Buffer.BlockCopy(b, 0, result, a.Length, b.Length);
        return result;
    }
}