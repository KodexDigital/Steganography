using System.Security.Cryptography;
using System.Text;

namespace Anaconda.Helpers
{
    public static class HashHelper
    {
        public static string ComputeHash(string input)
        {
            byte[] bytes = SHA512.HashData(Encoding.UTF8.GetBytes(input));
            var builder = new StringBuilder();
            foreach (var b in bytes)
                builder.Append(b.ToString("x2"));

            return builder.ToString();
        }
    }
}