using System;
using System.IO;

namespace Client
{
    internal static class ClientNetworkProcessor
    {
        public static void TrySendData(string[] filesToSend, string[] filesToDelete, LocalFakeServer localFakeServer)
        {
            foreach (var filePath in filesToSend)
            {
                try
                {
                    SendData(filePath, localFakeServer);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    continue;
                }
            }
        }

        private static void SendData(string filePath, LocalFakeServer localFakeServer)
        {
            long chunkByteSize = 1000 * 1000 * 100; //100 mb

            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            byte[] formatedFileData = null;

            if (fileStream.Length > chunkByteSize)
            {
                var i = 0;

                for (i = 0; i < fileStream.Length / chunkByteSize; i++)
                {
                    fileStream.Position = i * chunkByteSize;

                    var fileData = new byte[chunkByteSize];

                    fileStream.Read(fileData, 0, (int)chunkByteSize);

                    SendInfo sendInfo;
                    if (i != 0)
                    {
                        sendInfo = SendInfo.Chunk;
                    }
                    else
                    {
                        sendInfo = SendInfo.FullFile;
                    }

                    formatedFileData = FileProcessor.FormatFileToSend(filePath, fileData, sendInfo);
                }
                if (fileStream.Length % chunkByteSize > 0)
                {
                    fileStream.Position = (i + 1) * chunkByteSize;

                    var fileData = new byte[fileStream.Length % chunkByteSize];

                    fileStream.Read(fileData, 0, fileData.Length);

                    formatedFileData = FileProcessor.FormatFileToSend(filePath, fileData, SendInfo.Chunk);
                }
            }
            else
            {
                var fileData = new byte[fileStream.Length % chunkByteSize];

                fileStream.Read(fileData, 0, fileData.Length);

                formatedFileData = FileProcessor.FormatFileToSend(filePath, fileData, SendInfo.FullFile);
            }

            localFakeServer.ProcessFormatedData(formatedFileData);
        }
    }
}