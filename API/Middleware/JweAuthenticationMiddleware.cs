using System.Security.Claims;
using System.Security.Cryptography; // Needed for HMACSHA256, HashAlgorithmName
using System.Text;
using Jose;

namespace Hengeler.API.Middleware;

public class JweAuthenticationMiddleware
{
  private readonly RequestDelegate _next;
  private readonly string _secret;
  private readonly byte[] _derivedKey;

  private const int HkdfOutputKeyLength = 64;
  private const string DefaultCookieName = "authjs.session-token";

  public JweAuthenticationMiddleware(RequestDelegate next, IConfiguration configuration)
  {
    _next = next;
    _secret = configuration["NextAuth:Secret"] ?? throw new Exception("NextAuth:Secret is not configured");

    byte[] ikm = Encoding.UTF8.GetBytes(_secret);

    byte[] salt = Encoding.UTF8.GetBytes(DefaultCookieName);

    string infoString = $"Auth.js Generated Encryption Key ({DefaultCookieName})";
    byte[] info = Encoding.UTF8.GetBytes(infoString);

    _derivedKey = DeriveKeyWithHkdf(ikm, salt, info, HkdfOutputKeyLength);
  }

  private static byte[] DeriveKeyWithHkdf(byte[] ikm, byte[] salt, byte[] info, int keyLength)
  {
    byte[] prk;
    using (var hmac = new HMACSHA256(salt))
    {
      prk = hmac.ComputeHash(ikm);
    }

    byte[] okm = new byte[keyLength];
    byte[] t = [];

    using (var hmac = new HMACSHA256(prk))
    {
      int hashLen = hmac.HashSize / 8;
      int N = (int)Math.Ceiling((double)keyLength / hashLen);

      if (N > 255) throw new ArgumentException("Key length too large for HKDF-Expand");

      for (int i = 1; i <= N; i++)
      {
        byte[] input = new byte[t.Length + info.Length + 1];
        Buffer.BlockCopy(t, 0, input, 0, t.Length);
        Buffer.BlockCopy(info, 0, input, t.Length, info.Length);
        input[input.Length - 1] = (byte)i;

        t = hmac.ComputeHash(input);

        int bytesToCopy = Math.Min(hashLen, keyLength - (i - 1) * hashLen);
        Buffer.BlockCopy(t, 0, okm, (i - 1) * hashLen, bytesToCopy);
      }
    }
    return okm;
  }

  public async Task Invoke(HttpContext context)
  {
    var token = context.Request.Cookies["authjs.session-token"];

    if (!string.IsNullOrEmpty(token))
    {
      try
      {
        JweToken jweTokenObject = JWE.Decrypt(token, _derivedKey);
        var payload = jweTokenObject.Plaintext;

        var claims = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(payload);
        if (claims is null)
        {
          await _next(context);
          return;
        }

        if (claims.TryGetValue("exp", out var expObj) && long.TryParse(expObj.ToString(), out var expUnix))
        {
          var expDate = DateTimeOffset.FromUnixTimeSeconds(expUnix);
          if (expDate < DateTimeOffset.UtcNow)
          {

            await _next(context);
            return;
          }
        }

        var claimsList = new List<Claim>();
        foreach (var kv in claims)
        {
          if (kv.Value is string str)
            claimsList.Add(new Claim(kv.Key, str));
          else
            claimsList.Add(new Claim(kv.Key, kv.Value?.ToString() ?? ""));
        }

        var identity = new ClaimsIdentity(claimsList, "jwe");
        var principal = new ClaimsPrincipal(identity);

        context.User = principal;
      }
      catch (Exception ex)
      {

      }
    }

    await _next(context);
  }
}