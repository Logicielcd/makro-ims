using System;
using System.Net;
using System.Net.Mail;

// Sender's email address
string account = "logicielcd.com";
string fromEmail = "automail@logicielcd.com";
// Sender's email password
string password = "Z7teTTjjV8VaURqF";
// Recipient's email address
string toEmail = "chitisan@logicielcd.com";
// Email subject
string subject = "Test Email from C#";
// Email body
string body = "This is a test email sent from C#.";

// Configure SMTP client
var smtpClient = new SmtpClient("mail.smtp2go.com")
{
    Port = 2525,
    Credentials = new NetworkCredential(account, password),
    EnableSsl = true,
};

// Create the email message
var message = new MailMessage(fromEmail, toEmail, subject, body);

try
{
    // Send the email
    smtpClient.Send(message);
    Console.WriteLine("Email sent successfully.");
}
catch (Exception ex)
{
    Console.WriteLine($"Failed to send email. Error message: {ex.Message}");
}
