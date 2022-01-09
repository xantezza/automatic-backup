using System;
using System.IO;
using System.Text;

namespace Client
{
    internal class LocalFakeServer
    {
        private string _cachedFilePath;

        public void ProcessFormatedData(byte[] incomeData)
        {
            var isTheArrayAChunk = incomeData[incomeData.Length - 1] == 1; //last byte represents data array is a chunk or not
            if (isTheArrayAChunk && _cachedFilePath != null)
            {
                var fileStream = new FileStream(_cachedFilePath, FileMode.Append, FileAccess.Write);

                fileStream.Write(incomeData, 0, incomeData.Length - 1);
                fileStream.Close();
            }
            else
            {
                var offset = 10;
                var filePathLengthInBytes = new byte[offset];

                for (var i = 0; i < offset; i++)
                {
                    filePathLengthInBytes[i] = incomeData[i];
                }

                var filePathLength = Convert.ToInt32(Encoding.ASCII.GetString(filePathLengthInBytes));
                var filePathBytes = new byte[filePathLength];

                for (var i = 0; i < filePathLength; i++)
                {
                    filePathBytes[i] = incomeData[i + offset];
                }
                var filePathString = Encoding.Unicode.GetString(filePathBytes);
                var newFilePath = filePathString.Replace(filePathString.Substring(0, 2), Program.Config.BackUpPath);

                _cachedFilePath = newFilePath;

                var fileName = Path.GetFileName(newFilePath);
                var directoryPath = newFilePath.Replace(fileName, "");

                Directory.CreateDirectory(directoryPath);

                Console.WriteLine($"Saving file {newFilePath}");
                var fileStream = new FileStream(newFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite, incomeData.Length - offset - filePathLength - 1);
                fileStream.Write(incomeData, offset + filePathLength, incomeData.Length - offset - filePathLength - 1);
                fileStream.Close();
            }
        }
    }
}