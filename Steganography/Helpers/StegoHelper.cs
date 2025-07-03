//using System.Drawing;

//namespace Steganography.Helpers
//{
//    public class StegoHelper
//    {
//        public static Bitmap EmbedText(string text, Bitmap bmp)
//        {
//            int charIndex = 0;
//            int charValue = 0;
//            int pixelElementIndex = 0;
//            bool isFinished = false;

//            for (int i = 0; i < bmp.Height && !isFinished; i++)
//            {
//                for (int j = 0; j < bmp.Width; j++)
//                {
//                    Color pixel = bmp.GetPixel(j, i);
//                    byte R = pixel.R, G = pixel.G, B = pixel.B;

//                    for (int n = 0; n < 3; n++)
//                    {
//                        if (pixelElementIndex / 8 >= text.Length)
//                        {
//                            isFinished = true;
//                            break;
//                        }

//                        charValue = text[pixelElementIndex / 8];
//                        int bit = charValue >> 7 - pixelElementIndex % 8 & 1;

//                        if (n == 0) R = (byte)(R & 0xFE | bit);
//                        else if (n == 1) G = (byte)(G & 0xFE | bit);
//                        else B = (byte)(B & 0xFE | bit);

//                        pixelElementIndex++;
//                    }

//                    bmp.SetPixel(j, i, Color.FromArgb(R, G, B));
//                }
//            }

//            return bmp;
//        }
//        public static string ExtractText(Bitmap bmp)
//        {
//            int colorUnitIndex = 0;
//            int charValue = 0;
//            string extractedText = "";

//            for (int i = 0; i < bmp.Height; i++)
//            {
//                for (int j = 0; j < bmp.Width; j++)
//                {
//                    Color pixel = bmp.GetPixel(j, i);
//                    for (int n = 0; n < 3; n++)
//                    {
//                        int value = n switch
//                        {
//                            0 => pixel.R,
//                            1 => pixel.G,
//                            2 => pixel.B,
//                            _ => 0
//                        };

//                        charValue = charValue << 1 | value & 1;
//                        colorUnitIndex++;

//                        if (colorUnitIndex % 8 == 0)
//                        {
//                            char c = (char)charValue;
//                            if (c == ' ') return extractedText;
//                            extractedText += c;
//                            charValue = 0;
//                        }
//                    }
//                }
//            }

//            return extractedText;
//        }
//    }
//}
