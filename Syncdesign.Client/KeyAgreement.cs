using Org.BouncyCastle.Crypto.Agreement;
using Org.BouncyCastle.Crypto.Parameters;

namespace Syncdesign.Client;

/// <summary>
/// 密钥交换（X25519）
/// </summary>
public static class KeyAgreement
{
    public static byte[] GenerateSharedSecret(byte[] myPrivateKey, byte[] peerPublicKey)
    {
        var priv = new X25519PrivateKeyParameters(myPrivateKey, 0);
        var pub = new X25519PublicKeyParameters(peerPublicKey, 0);

        var agreement = new X25519Agreement();
        agreement.Init(priv);

        byte[] shared = new byte[agreement.AgreementSize];
        agreement.CalculateAgreement(pub, shared, 0);
       
        return shared;
    }
}