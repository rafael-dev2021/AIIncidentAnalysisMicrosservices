using System.Security.Cryptography;

namespace XUnitTests;

public static class GenerateKey
{
    public static string GenerateHmac256Key()
    {
        using var hmac = new HMACSHA256();
        return Convert.ToBase64String(hmac.Key);
    }
}