using System.Text;

namespace Lost
{
    public static class BytesExtensions
    {
        public static string ToHexString(this byte[] bytes)
        {
            string hashString;
            var builder = new StringBuilder();
            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            // Return the hexadecimal string.
            hashString = builder.ToString();
            return hashString;
        }
    }
}