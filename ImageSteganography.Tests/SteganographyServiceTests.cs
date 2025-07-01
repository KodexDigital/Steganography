//using Xunit;
//using System.Drawing;

//namespace ImageSteganography.Tests
//{
//    public class SteganographyServiceTests
//    {
//        private readonly SteganographyService _service = new();

//        [Fact]
//        public void CanEmbedMessage_ReturnsTrue_WhenFits()
//        {
//            var bmp = new Bitmap(100, 100); // 10,000 pixels
//            string message = "Short message";
//            bool result = _service.CanEmbedMessage(bmp, message);
//            Assert.True(result);
//        }

//        [Fact]
//        public void CanEmbedMessage_ReturnsFalse_WhenTooBig()
//        {
//            var bmp = new Bitmap(10, 10); // 100 pixels
//            string longMessage = new string('A', 500); // Way too big
//            bool result = _service.CanEmbedMessage(bmp, longMessage);
//            Assert.False(result);
//        }

//        [Fact]
//        public void EmbedAndExtractMessage_ReturnsOriginalMessage()
//        {
//            var bmp = new Bitmap(100, 100);
//            string message = "Stego message test!";
//            var encodedBmp = _service.EmbedMessage(bmp, message);
//            string extracted = _service.ExtractMessage(encodedBmp);
//            Assert.Equal(message, extracted);
//        }
//    }
//}
