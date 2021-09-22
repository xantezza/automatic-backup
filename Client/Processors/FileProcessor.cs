using System.Collections.Generic;
using System.Text;

namespace Client
{
    internal static class FileProcessor
    {
        public static byte[] FormatFileToSend(string filePath, byte[] fileData, bool isTheArrayAChunk)
        {
            var dataToSend = new List<byte>();

            if (isTheArrayAChunk)
            {
                foreach (var byteOfFile in fileData)
                {
                    dataToSend.Add(byteOfFile);
                }
                dataToSend.Add(1);
            }
            else
            {
                var encodedFilePath = Encoding.Unicode.GetBytes(filePath);

                var offset = 10;

                var bytes = new byte[offset];

                var encodedFilePathBytesInfo = Encoding.ASCII.GetBytes(encodedFilePath.Length.ToString());

                for (var i = 0; i < encodedFilePathBytesInfo.Length; i++)
                {
                    bytes[i] = encodedFilePathBytesInfo[i];
                }

                for (var i = 0; i < offset; i++)
                {
                    dataToSend.Add(bytes[i]);
                }

                for (var i = 0; i < encodedFilePath.Length; i++)
                {
                    dataToSend.Add(encodedFilePath[i]);
                }

                for (var i = 0; i < fileData.Length; i++)
                {
                    dataToSend.Add(fileData[i]);
                }
                dataToSend.Add(0);
            }

            return dataToSend.ToArray();
        }
    }
}