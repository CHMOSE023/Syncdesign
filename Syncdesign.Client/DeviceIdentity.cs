using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace Syncdesign.Client;

/// <summary>
/// 设备身份
/// </summary>
public sealed class DeviceIdentity
{
    public string PrivateKeyPem { get; }
    public byte[] PublicKey { get; }
    public string DeviceId { get; }

    private DeviceIdentity(string pem, byte[] pub, string id)
    {
        PrivateKeyPem = pem;
        PublicKey = pub;
        DeviceId = id;
    }

    public static DeviceIdentity Create()
    {
        string pem = KeyGenerator.GeneratePrivateKeyPem();
        byte[] pub = ExtractPublicKey(pem);
        string id = SyncthingDeviceId.FromPrivateKeyPem(pem);

        return new DeviceIdentity(pem, pub, id);
    }

    private static byte[] ExtractPublicKey(string pem)
    {
        using var reader = new StringReader(pem);
        var pemReader = new Org.BouncyCastle.OpenSsl.PemReader(reader);
        var pki = (PrivateKeyInfo)pemReader.ReadObject();

        var priv = (Ed25519PrivateKeyParameters)
            PrivateKeyFactory.CreateKey(pki);

        return priv.GeneratePublicKey().GetEncoded();
    }
}
