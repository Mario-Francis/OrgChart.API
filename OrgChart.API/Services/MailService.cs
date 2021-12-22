using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using OrgChart.API.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrgChart.API.Services
{
    public class MailService:IMailService
    {
        private readonly ILogger<MailService> logger;
        private readonly IOptionsSnapshot<AppSettings> appSettingsDelegate;

        public MailService(ILogger<MailService> logger,
            IOptionsSnapshot<AppSettings> appSettingsDelegate)
        {
            this.logger = logger;
            this.appSettingsDelegate = appSettingsDelegate;
        }

       
        // send mail
        public async Task<bool> SendMail(Mail mail)
        {
            try
            {
                // create email message
                var email = new MimeMessage();
                var _from = new MailboxAddress(appSettingsDelegate.Value.EmailSMTPConfig.FromName, appSettingsDelegate.Value.EmailSMTPConfig.Username);
                // from
                email.From.Add(_from);
                // to
                email.To.Add(MailboxAddress.Parse(mail.Email));
                //  subject
                email.Subject = mail.Subject;
                // bodyappse
                BodyBuilder bodyBuilder = new BodyBuilder();
                //bodyBuilder.HtmlBody = mail.Body;
                bodyBuilder.TextBody = mail.Body;

                if (mail.Attachments != null && mail.Attachments.Count() > 0)
                {
                    foreach (var file in mail.Attachments)
                    {
                        bodyBuilder.Attachments.Add(file.FileName, file.FileBuffer);
                    }
                }
                email.Body = bodyBuilder.ToMessageBody();

                // send email
                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(appSettingsDelegate.Value.EmailSMTPConfig.Host, appSettingsDelegate.Value.EmailSMTPConfig.Port, SecureSocketOptions.Auto);
                if (appSettingsDelegate.Value.EmailSMTPConfig.AuthenticateMail)
                {
                    await smtp.AuthenticateAsync(appSettingsDelegate.Value.EmailSMTPConfig.Username, appSettingsDelegate.Value.EmailSMTPConfig.Password);
                }
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error enccountered while sending mail");
                return false;
            }
        }

       
    }
}
