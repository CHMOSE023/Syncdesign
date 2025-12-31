using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Syncdesign.Client.Utils;
using System.Security.Cryptography;

namespace Syncdesign.Client.Identity;

public static class SyncdesignDeviceId
{
    /// <summary>
    /// 根据私钥生成设备ID
    /// </summary> 
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
            return string.Join("-", Enumerable.Range(0, base32.Length / 7).Select(i => base32.Substring(i * 7, 7)));
        }
        catch (Exception ex)
        {
            throw new CryptographicException("Failed to derive device ID from private key", ex);
        }
    }
}
