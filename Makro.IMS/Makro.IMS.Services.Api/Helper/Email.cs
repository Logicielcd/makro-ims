using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Configuration;
using System.Net.Mail;
using System.Net;

namespace Makro.IMS.Services.Api.Helper
{
    public static class Email
    {
        
        public static void SendEmail(string to, string body, string subject)
        {
            
            System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
            SmtpClient smtpServer = new SmtpClient("10.81.1.70");
            smtpServer.Port = 25;

            //SmtpServer.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
            // Encrypt the connection
            //SmtpServer.EnableSsl = true;

            // Callback to validate server certificate
            try
            {
                mail.From = new MailAddress("booking@cpaxtra.co.th");

                foreach (var mailTo in to.Split(';'))
                {
                    mail.To.Add(mailTo);
                }

                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;
                smtpServer.Send(mail);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                smtpServer.Dispose();
            }
        }

    }

}