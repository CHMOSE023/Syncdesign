using Org.BouncyCastle.Crypto.Parameters;

namespace Syncdesign.Client.Crypto;
/// <summary>
/// Ed25519 签名与验证
/// </summary>
public static class SignatureUtil
{
    public static byte[] Sign(byte[] data, Ed25519PrivateKeyParameters privateKey)
    {
        var signer = new Org.BouncyCastle.Crypto.Signers.Ed25519Signer();
        signer.Init(true, privateKey);
        signer.BlockUpdate(data, 0, data.Length);
        return signer.GenerateSignature();
    }

    public static bool Verify(byte[] data, byte[] signature, Ed25519PublicKeyParameters publicKey)
    {
        var verifier = new Org.BouncyCastle.Crypto.Signers.Ed25519Signer();
        verifier.Init(false, publicKey);
        verifier.BlockUpdate(data, 0, data.Length);
        return verifier.VerifySignature(signature);
    }
}
