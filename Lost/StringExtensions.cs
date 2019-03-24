using System.Security.Cryptography;
using System.Text;

namespace Lost
{
    public static class StringExtensions
    {
        public static string ToMd5Hash(this string prop)
        {
            string hashString;
            using (var md5 = MD5.Create())
            {
                var bytes = Encoding.Default.GetBytes(prop);
                var hash = md5.ComputeHash(bytes);
                hashString = hash.ToHexString();
            }
            return hashString;
        }
    }
}