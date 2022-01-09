<H1> Automatic backup and data storage on your own remote server or other disc</H1>

(At the moment, networking is under development.)

To use the application you need:

1. Download .NET CORE 3.1

https://dotnet.microsoft.com/en-us/download/dotnet/3.1

2. Download build

https://github.com/xantezza/automatic-backup/releases/latest


3. Unzip files to any folder.

4. It should contain the `Config.json` file.

5. Open the `Config.json` file and configure it as in the example by entering the required `values` and `paths`.

```
{
  "timeStampForAutoCheckInSeconds" : 300, 

  "backupPath" : "D:\\BackUp", 
  "cachePath" : "D:\\BackUp\\Cache.json",

  "pathesToData" : 
  [
      "C:\\Folder1\\Subfolder",
      "D:\\Folder2\\Subfolder"
  ]
}
```
6. Run the `Client.exe` from admin.
7. You can add app to autostart group => C:\Users\%USERNAME%\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup
