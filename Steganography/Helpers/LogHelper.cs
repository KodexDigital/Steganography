using System.Text;

namespace Steganography.Helpers
{
    public static class LogHelper
    {
        private static readonly string LogFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "log.txt");
        public static void Log(string action, string message, string status, string username, string? logId)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(LogFilePath)!);
            var logEntry = $"{DateTime.UtcNow:u} | {action} | {status} | User: {username} | {message}";
            File.AppendAllText(LogFilePath, logEntry + Environment.NewLine, Encoding.UTF8);
        }

        public static string[] GetLogs()
        {
            if (!File.Exists(LogFilePath)) return Array.Empty<string>();
            return File.ReadAllLines(LogFilePath);
        }
    }
}