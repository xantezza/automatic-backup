using Newtonsoft.Json;
using System;
using System.IO;
using System.Timers;
using System.Diagnostics;

namespace Client
{
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
            if (Config == null || Config.TimeStampForAutoCheckInSeconds <= 5)
            {
                Console.WriteLine($"Config filled wrong. Refill");
                Process.Start(new ProcessStartInfo(new FileInfo("Config.json").FullName) { UseShellExecute = true});
                return;
            }
            SynchronyzeFiles();
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
            Console.WriteLine("Synchronization started.");
            PathsProcessor.GetPathsToFiles(out var pathsToFilesToSend, out var pathsToFilesToDelete);
            ClientNetworkProcessor.TrySendData(pathsToFilesToSend, pathsToFilesToDelete, new LocalFakeServer());
            Console.WriteLine("Synchronization finished. Waiting for timer.");
        }

        private Config TryLoadConfig(string path)
        {
            if (!File.Exists(path))
            { 
                File.Create(path).Close();
                Console.WriteLine("Config has been created. Fill config and Press any key. Make sure timestamp is more than 5 seconds");
                Process.Start(new ProcessStartInfo(new FileInfo("Config.json").FullName) { UseShellExecute = true });
                Console.ReadLine();
            }

            Console.WriteLine($"Config loaded from path {new FileInfo(path).FullName}");
            var jsonData = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<Config>(jsonData);
        }
    }
}