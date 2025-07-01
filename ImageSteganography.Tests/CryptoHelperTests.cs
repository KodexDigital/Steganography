//using Xunit;

//namespace ImageSteganography.Tests
//{ 
//    public class CryptoHelperTests
//    {
//        [Fact]
//        public void EncryptAndDecrypt_ReturnsOriginalMessage()
//        {
//            // Arrange
//            string original = "This is a secret message.";
//            string password = "strong-password";

//            // Act
//            string encrypted = CryptoHelper.Encrypt(original, password);
//            string decrypted = CryptoHelper.Decrypt(encrypted, password);

//            // Assert
//            Assert.Equal(original, decrypted);
//        }

//        [Fact]
//        public void Decrypt_WithWrongPassword_FailsOrReturnsWrongMessage()
//        {
//            string message = "Sensitive text";
//            string password = "correct";
//            string wrongPassword = "incorrect";

//            string encrypted = CryptoHelper.Encrypt(message, password);
//            Assert.ThrowsAny<Exception>(() => CryptoHelper.Decrypt(encrypted, wrongPassword));
//        }
//    }

//}