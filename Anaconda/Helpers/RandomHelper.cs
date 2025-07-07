using System.Security.Cryptography;

namespace Anaconda.Helpers
{
    public static class RandomHelper
    {
        //private static readonly char[] _chars =
        //    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();

        public static string GenerateRandomString(char[] chars, int length)
        {
            byte[] data = new byte[length];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(data);

            var result = new char[length];
            for (int i = 0; i < result.Length; i++)
                result[i] = chars[data[i] % chars.Length];

            return new string(result);
        }
    }
}
