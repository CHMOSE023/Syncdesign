using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using System.Text;

namespace Syncdesign.Client.Identity;
/// <summary>
/// 生成 Ed25519 长期身份密钥
/// </summary>
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
