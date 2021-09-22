using Newtonsoft.Json;
using System;
using System.IO;
using System.Timers;

namespace Client
{
    public enum SendInfo
    {
        FullFile,
        Chunk
    }

    internal class Program
    {
        private Timer _timer;
        public static Config Config;

        private static void Main(string[] args)
        {
            new Program();
        }

        public Program()
        {
            Config = TryLoadConfig("Config.json");
            SetTimer(Config.TimeStampForAutoCheckInSeconds);
            Console.ReadKey();
            _timer.Stop();
            _timer.Dispose();
        }

        private void SetTimer(double time)
        {
            _timer = new Timer(time * 1000);
            _timer.Elapsed += OnTimedEvent;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            SynchronyzeFiles();
        }

        private void SynchronyzeFiles()
        {
            PathsProcessor.GetPathsToFiles(out var pathsToFilesToSend, out var pathsToFilesToDelete);
            ClientNetworkProcessor.TrySendData(pathsToFilesToSend, pathsToFilesToDelete, new LocalFakeServer());
        }

        private Config TryLoadConfig(string path)
        {
            if (File.Exists(path))
            {
                var jsonData = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<Config>(jsonData);
            }
            else
            {
                File.Create(path);
                Console.WriteLine("Config.json has been created, configure it and restart application.");
                Console.ReadKey();
                throw new Exception();
            }
        }
    }
}