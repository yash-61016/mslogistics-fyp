using System;
using Microsoft.Extensions.Logging;
using MSLogistics.Application.Logging.Loggers;

namespace MSLogistics.Application.Logging.Providers
{
	public class ErrorFileLoggerProvider : ILoggerProvider
    {
        private readonly string _filePath;
        private readonly long _fileSizeLimitBytes;

        public ErrorFileLoggerProvider(string filePath, long fileSizeLimitBytes)
        {
            _filePath = filePath;
            _fileSizeLimitBytes = fileSizeLimitBytes;

            // Check if the file exists and create it if it doesn't
            if (!File.Exists(_filePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_filePath));
                File.Create(_filePath).Dispose();
            }
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new ErrorFileLogger(_filePath, _fileSizeLimitBytes);
        }

        public void Dispose() { }
    }
}