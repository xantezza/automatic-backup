using System;
using System.IO;

namespace Client
{
    internal class ClientNetworkProcessor
    {
        private readonly Config _config;
        private LocalFakeServer _localFakeServer;

        public ClientNetworkProcessor(Config config, LocalFakeServer localFakeServer)
        {
            _config = config;
            _localFakeServer = localFakeServer;
        }

        public void TrySendData(string[] filesToSend, string[] filesToDelete)
        {
            {
                foreach (var filePath in filesToSend)
                {
                    try
                    {
                        SendData(filePath);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        continue;
                    }
                }
            }
        }

        private void SendData(string filePath)
        {
            var chunkByteSize = 1000 * 1000L; //1 mb

            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            if (fileStream.Length > chunkByteSize)
            {
                var i = 0;
                for (i = 0; i < fileStream.Length / chunkByteSize; i++)
                {
                    fileStream.Position = i * chunkByteSize;

                    var fileData = new byte[chunkByteSize];

                    fileStream.Read(fileData, 0, (int)chunkByteSize);

                    var isTheArrayAChunk = i != 0;

                    var formatedFileData = FileProcessor.FormatFileToSend(filePath, fileData, isTheArrayAChunk);

                    _localFakeServer.ProcessFormatedData(formatedFileData, _config);
                }
                if (fileStream.Length % chunkByteSize > 0)
                {
                    fileStream.Position = (i + 1) * chunkByteSize;

                    var fileData = new byte[fileStream.Length % chunkByteSize];

                    fileStream.Read(fileData, 0, fileData.Length);
                    var formatedFileData = FileProcessor.FormatFileToSend(filePath, fileData, true);

                    _localFakeServer.ProcessFormatedData(formatedFileData, _config);
                }
            }
            else
            {
                var fileData = new byte[fileStream.Length % chunkByteSize];

                fileStream.Read(fileData, 0, fileData.Length);
                var formatedFileData = FileProcessor.FormatFileToSend(filePath, fileData, false);

                _localFakeServer.ProcessFormatedData(formatedFileData, _config);
            }
        }
    }
}