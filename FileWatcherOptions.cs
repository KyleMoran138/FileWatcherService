using System.Collections.Generic;

namespace FileWatcher
{
    public class FileWatcherOptions
    {
        public int timeDelayInMilliseconds { get; set; }
        public int failCountBeforeEmail { get; set; }
        public string emailTitle { get; set; }
        public string emailBody { get; set; }
        public List<string> emailsToNotify { get; set; }
        public List<string> filesToCheck { get; set; }
    }
}