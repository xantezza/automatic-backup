using System;
using System.Collections.Generic;
using System.Text;

namespace Client
{
    internal static class FileProcessor
    {
        public static byte[] FormatFileToSend(string filePath, byte[] fileData, ClientNetworkProcessor.SendInfo sendInfo)
        {
            var dataToSend = new List<byte>();

            if (sendInfo == ClientNetworkProcessor.SendInfo.Chunk)
            {
                foreach (var byteOfFile in fileData)
                {
                    dataToSend.Add(byteOfFile);
                }
            }
            if (sendInfo == ClientNetworkProcessor.SendInfo.FullFile)
            {
                var encodedFilePath = Encoding.Unicode.GetBytes(filePath);
                var offsetForEncodedFilePathBytes = 10;
                var encodedFilePathPreparedBytes = new byte[offsetForEncodedFilePathBytes];
                var encodedFilePathBytesLengthInfo = Encoding.ASCII.GetBytes(encodedFilePath.Length.ToString());

                for (var i = 0; i < encodedFilePathBytesLengthInfo.Length; i++)
                {
                    encodedFilePathPreparedBytes[i] = encodedFilePathBytesLengthInfo[i];
                }
                for (var i = 0; i < offsetForEncodedFilePathBytes; i++)
                {
                    dataToSend.Add(encodedFilePathPreparedBytes[i]);
                }
                for (var i = 0; i < encodedFilePath.Length; i++)
                {
                    dataToSend.Add(encodedFilePath[i]);
                }
                for (var i = 0; i < fileData.Length; i++)
                {
                    dataToSend.Add(fileData[i]);
                }
            }
            dataToSend.Add((byte)sendInfo);

            return dataToSend.ToArray();
        }
    }
}