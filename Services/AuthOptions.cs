using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Services
{
    public static class AuthOptions
    {
        public const string Issuer = "LisaProduction"; // издатель токена
        public const string Audience = "DocumentStorageServer"; // потребитель токена
        public const int Lifetime = 60 * 24; // время жизни токена в минутах - 1 день
        
        const string Key = "SuppaPuppaSecretKey";   // ключ для шифровки
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Key));
        }
    }
}