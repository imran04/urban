using System;
using System.Security.Cryptography;
using System.Text;

namespace app.Infra
{
    public static class PasswordHelper
    {

        
        public static (string, string) HashPassword(this string Password)
        {
            string SecurityStamp = Guid.NewGuid().ToString();
            return Password.HashPassword(SecurityStamp);
        }
        public static (string, string) HashPassword(this string Password, string SecurityStamp)
        {
            string hash;
            using (Rfc2898DeriveBytes h = new Rfc2898DeriveBytes(Password, Encoding.UTF8.GetBytes(SecurityStamp), 10))
            {
                hash = Convert.ToBase64String(h.GetBytes(20));
            }
            return (hash, SecurityStamp);
        }

        public static bool CheckValidPasswprd(this string Password, string SecurityStamp, string passwordHash)
        {
            var (hash, _) = Password.HashPassword(SecurityStamp);
            if (hash == passwordHash)
                return true;
            else return false;

        }
    }
}
