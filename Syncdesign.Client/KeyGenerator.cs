using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using System.Security.Cryptography;
using System.Text;

namespace Syncdesign.Client;

public class KeyGenerator
{
    public static string GeneratePrivateKeyPem()
    {
        // 强随机
        var random = new SecureRandom();

        // Ed25519 密钥对
        var gen = new Ed25519KeyPairGenerator();
        gen.Init(new Ed25519KeyGenerationParameters(random));

        AsymmetricCipherKeyPair keyPair = gen.GenerateKeyPair();

        // PKCS#8
        var privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(keyPair.Private);
        byte[] pkcs8 = privateKeyInfo.GetEncoded();

        return ToPem("PRIVATE KEY", pkcs8);
    }

    private static string ToPem(string label, byte[] data)
    {
        var base64 = Convert.ToBase64String(data);
        var sb = new StringBuilder();

        sb.AppendLine($"-----BEGIN {label}-----");
        for (int i = 0; i < base64.Length; i += 64)
            sb.AppendLine(base64.Substring(i, Math.Min(64, base64.Length - i)));
        sb.AppendLine($"-----END {label}-----");

        return sb.ToString();
    }
}
public static class SyncthingDeviceId
{
    public static string FromPrivateKeyPem(string pem)
    {

        if (string.IsNullOrWhiteSpace(pem))
            throw new FormatException("Private key PEM is empty");

        Ed25519PrivateKeyParameters privateKey;

        try
        {
            using var reader = new StringReader(pem);
            var pemReader = new Org.BouncyCastle.OpenSsl.PemReader(reader);
            object obj = pemReader.ReadObject()
                ?? throw new FormatException("PEM content is invalid");

            privateKey = obj switch
            {
                AsymmetricKeyParameter akp when akp.IsPrivate =>
                    akp as Ed25519PrivateKeyParameters
                        ?? throw new CryptographicException("Not an Ed25519 private key"),

                AsymmetricCipherKeyPair kp =>
                    kp.Private as Ed25519PrivateKeyParameters
                        ?? throw new CryptographicException("Not an Ed25519 key pair"),

                PrivateKeyInfo pki =>
                    PrivateKeyFactory.CreateKey(pki) as Ed25519PrivateKeyParameters
                        ?? throw new CryptographicException("Unsupported private key algorithm"),

                _ => throw new FormatException(
                    $"Unsupported PEM object: {obj.GetType().FullName}")
            };
        }
        catch (IOException ex)
        {
            throw new FormatException("Failed to parse PEM format", ex);
        }

        try
        {
            // 派生公钥
            byte[] publicKey = privateKey.GeneratePublicKey().GetEncoded();

            // SHA-256
            using var sha = SHA256.Create();
            byte[] hash = sha.ComputeHash(publicKey);

            // Base32（RFC4648，无 padding）
            string base32 = Base32Encoding.ToBase32String(hash);

            // 每 7 位分组（Syncthing 风格）
            return string.Join("-",
                Enumerable.Range(0, base32.Length / 7)
                          .Select(i => base32.Substring(i * 7, 7)));
        }
        catch (Exception ex)
        {
            throw new CryptographicException("Failed to derive device ID from private key", ex);
        }
    }
}

public static class Base32Encoding
{
    private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

    public static string ToBase32String(byte[] data)
    {
        if (data == null || data.Length == 0)
            return string.Empty;

        int outputLength = (int)Math.Ceiling(data.Length / 5d) * 8;
        var result = new StringBuilder(outputLength);

        int buffer = data[0];
        int next = 1;
        int bitsLeft = 8;

        while (bitsLeft > 0 || next < data.Length)
        {
            if (bitsLeft < 5)
            {
                if (next < data.Length)
                {
                    buffer <<= 8;
                    buffer |= data[next++] & 0xff;
                    bitsLeft += 8;
                }
                else
                {
                    int pad = 5 - bitsLeft;
                    buffer <<= pad;
                    bitsLeft += pad;
                }
            }

            int index = (buffer >> (bitsLeft - 5)) & 0x1f;
            bitsLeft -= 5;
            result.Append(Alphabet[index]);
        }

        return result.ToString();
    }
}
