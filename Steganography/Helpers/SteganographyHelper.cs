using System.Drawing;
using System.Text;

namespace Steganography.Helpers
{
    public class SteganographyHelper
    {
        private const char Delimiter = '|'; // Used to indicate end of message

        public Bitmap EmbedMessage(Bitmap image, string message)
        {
            message += Delimiter; // Add delimiter to indicate end of message
            var binaryMessage = GetBinaryString(message);
            int messageIndex = 0;

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    if (messageIndex >= binaryMessage.Length)
                        return image;

                    Color pixel = image.GetPixel(x, y);
                    byte r = pixel.R, g = pixel.G, b = pixel.B;

                    // Replace LSB of Blue channel
                    b = (byte)((b & 0xFE) | (binaryMessage[messageIndex] == '1' ? 1 : 0));
                    messageIndex++;

                    image.SetPixel(x, y, Color.FromArgb(r, g, b));

                    if (messageIndex >= binaryMessage.Length)
                        return image;
                }
            }

            throw new Exception("Message too long to embed in image");
        }

        public string ExtractMessage(Bitmap image)
        {
            var binaryBuilder = new StringBuilder();
            var messageBuilder = new StringBuilder();

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color pixel = image.GetPixel(x, y);
                    int lsb = pixel.B & 1; // Get LSB of Blue
                    binaryBuilder.Append(lsb);

                    if (binaryBuilder.Length == 8)
                    {
                        char c = (char)Convert.ToByte(binaryBuilder.ToString(), 2);
                        if (c == Delimiter)
                            return messageBuilder.ToString();

                        messageBuilder.Append(c);
                        binaryBuilder.Clear();
                    }
                }
            }

            return messageBuilder.ToString(); // Fallback
        }

        private static string GetBinaryString(string message)
        {
            var sb = new StringBuilder();
            foreach (char c in message)
            {
                sb.Append(Convert.ToString(c, 2).PadLeft(8, '0'));
            }
            return sb.ToString();
        }

        //Check Message Capacity Before Embedding
        public bool CanEmbedMessage(Bitmap image, string message)
        {
            string binary = GetBinaryString(message + Delimiter);
            int maxBits = image.Width * image.Height; // 1 bit per pixel (Blue channel)
            return binary.Length <= maxBits;
        }
    }
}
