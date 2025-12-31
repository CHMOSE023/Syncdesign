namespace Syncdesign.Client;

public static class KeyStorage
{
    private static readonly string KeyPath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Syncdesign",
            "identity.key"
        );

    public static string LoadOrCreatePrivateKey()
    {
        if (File.Exists(KeyPath))
            return File.ReadAllText(KeyPath);

        string pem = KeyGenerator.GeneratePrivateKeyPem();
        SavePrivateKey(pem);
        return pem;
    }

    private static void SavePrivateKey(string pem)
    {
        string dir = Path.GetDirectoryName(KeyPath)!;
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        File.WriteAllText(KeyPath, pem);
    }
 
}
