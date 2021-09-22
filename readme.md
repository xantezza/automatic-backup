<H1> Automatic backup and data storage on your own remote server or other disc</H1>

(At the moment, networking is under development.)

To use the application you need:

1. Download repository.

2. Build the project using `cmd` & `dotnet build` or any another convenient way.

3. Run the `build`.

4. Check the folder in which this build is located, it should contain the `Config.json` file.

5. Open the `Config.json` file and configure it as in the example by entering the required `values` and `paths`.

```
{
  "timeStampForAutoCheckInSeconds" : 300, 

  "backupPath" : "I:\\BackUp", 
  "cachePath" : "I:\\BackUp\\Cache.json",

  "pathesToData" : 
  [
	  "C:\\Users\\"
  ]
}
```
6. Restart the `build`.