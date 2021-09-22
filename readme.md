<H1> Automatic backup and data storage on your own remote server or other disc</H1>

(At the moment, networking is under development.)

To use the application you need:

1. Unzip the [archive](https://github.com/xantezza/automatic-backup/releases/) completely.

2. Open Config.json file and configure it like in the example.

```
{
  "timeStampForAutoCheckInSeconds" : 300, 

  "backupPath" : "I:\\BackUp", 
  "cachePath" : "I:\\BackUp\\Cache.json",

  "pathesToData" : [
	"C:\\Users\\"
  ]
}
```

3. Start application.