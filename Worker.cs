using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FileWatcher
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        
        int timeDelayInMilliseconds { get; set; }
        int failCountBeforeEmail { get; set; }
        string emailTitle { get; set; }
        string emailBody { get; set; }
        List<string> emailsToNotify { get; set; }
        List<string> filesToCheck { get; set; }

        string smtpClientUsername { get; set; }
        string smtpClientPassword { get; set; }
        string smtpClientUrl { get; set; }
        int smtpClientPort { get; set; }
        public SmtpClient SmtpServer { get; set; }


        bool allValuesAreValid = false;
        bool bigException = false;

        public Worker(ILogger<Worker> logger, IOptions<FileWatcherOptions> fileWatcherOptions, IOptions<SmtpEmailClientOptions> emailOptions )
        {
            _logger = logger;

            timeDelayInMilliseconds = fileWatcherOptions.Value.timeDelayInMilliseconds;
            emailTitle = fileWatcherOptions.Value.emailTitle;
            emailBody = fileWatcherOptions.Value.emailBody;
            emailsToNotify = fileWatcherOptions.Value.emailsToNotify;
            filesToCheck = fileWatcherOptions.Value.filesToCheck;

            smtpClientUsername = emailOptions.Value.smtpClientEmail;
            smtpClientPassword = emailOptions.Value.smtpClientPassword;
            smtpClientUrl = emailOptions.Value.smtpClientUrl;
            smtpClientPort = emailOptions.Value.smtpClientPort;

            //TODO better required value checking and logging
            if (timeDelayInMilliseconds != 0 && emailTitle != null
                && emailBody != null && emailsToNotify.Count != 0 
                && filesToCheck.Count != 0 && smtpClientPassword != null 
                && smtpClientUsername != null && smtpClientUrl != null
                && smtpClientPort != 0)
            {
                this.allValuesAreValid = true;
                SmtpServer = new SmtpClient(smtpClientUrl);
                SmtpServer.Port = smtpClientPort;
                SmtpServer.Credentials = new System.Net.NetworkCredential(smtpClientUsername, smtpClientPassword);
                SmtpServer.EnableSsl = true;
            }
            else
            {
                _logger.LogError("There's something wrong with your configuration settings.");
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!allValuesAreValid) await this.StopAsync(stoppingToken);
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("FileWatcher Worker running at: {time}", DateTimeOffset.Now);

                if (bigException)
                {
                    await this.StopAsync(stoppingToken);
                }
                string emailFailText = "";
                foreach(var filePath in filesToCheck)
                {
                    FileStream testWriteFile = null;

                    if (!File.Exists(filePath) && !Directory.Exists(filePath))
                    {
                        emailFailText += $"File/Directory does not exist at path {filePath}\n";
                        continue;
                    }
                    if (Directory.Exists(filePath)) continue;

                    try
                    {
                        testWriteFile = File.OpenWrite(filePath);
                    }
                    catch (Exception e)
                    {
                        emailFailText += $"File(hopefully) is not writeable at path {filePath}";
                    }
                    finally
                    {
                        if (testWriteFile != null)
                        {
                            testWriteFile.Close();
                        }
                    }
                }

                if (emailFailText != "") SendFailEmail(emailFailText);
                await Task.Delay(timeDelayInMilliseconds, stoppingToken);
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            SmtpServer.Dispose();
            _logger.LogInformation("Stopping the FileWatcher service...");
            return base.StopAsync(cancellationToken);
        }

        protected void SendFailEmail(string bodyAppendage = "")
        {
            try
            {
                MailMessage mail = new MailMessage();

                mail.From = new MailAddress(smtpClientUsername);
                emailsToNotify.ForEach(emailToNotify => mail.To.Add(emailToNotify));
                mail.Subject = emailTitle;
                mail.Body = emailBody + "\n" + bodyAppendage;

                SmtpServer.Send(mail);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending email {ex.Message}");
                bigException = true;
            }
        }
    }
}
