# FileWatcherService
This watches listed files in the configuration and alerts administrators if the files are missing or do not have the correct read/write permissions

Here are the steps to set up this service:
1. Go on over to the releases page and download the newest binaries. 
2. Make a directory on your computer somewhere. I'd personally recommend `C:\Program Files (x86)\FileWatcherService`
3. Extract the binary into that directory
4. Go into the appsettings.json file and fill in the following settings 
  ```
  "fileWatcherOptions": {
    "timeDelayInMilliseconds": 60000, //  This is how frequently(in milliseconds) the service will scan the listed files default is one minute
    "emailTitle": "", //  The title/subject/topic of the email
    "emailBody": "", //  The body of the email that desplays before the listed file errors
    "emailsToNotify": [ //  A list of emails to send the errors 
      "<emailOne>",
      "<emailTwo>"
    ],
    "filesToCheck": [ //  A list of file paths to check (this will verify that directories exist too)
      "<filePath>",
      "<AnotherFilePath>"
    ]
  },
  "smtpEmailClientOptions": {
    "smtpClientUrl": "smtp.gmail.com", //  The address of your smtp email server
    "smtpClientPort": 587, //  The port for your smtp server. This is the default smtp port 
    "smtpClientEmail": "", //  The smtp user email address for authentication and for sent from address
    "smtpClientPassword": "" //  The smtp user password for authentication
  }
  ```
  5. Open up an administrator powershell in the install directory and run the following commands
    a. ```sc.exe create FileWatcherService binpath= C:\Program Files (x86)\FileWatcherService\FileWatcher.exe start= auto```(if you diddn't follow my install location recommendation you're on your own )
  6. Open up the windows services tool and find the service called ```FileWatcherService``` and start it
  7. Now you should be able to find logs in ```C:\temp\fileTrackingWorkerService\Log.txt```
  
  If anything is broken or you see I'm missing steps please contact me at KyleMoran.mail(AT)gmail.com
