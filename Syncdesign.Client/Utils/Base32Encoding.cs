using System.Text;

namespace Syncdesign.Client.Utils;
/// <summary>
/// Base32 编码
/// </summary>
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
