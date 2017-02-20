using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace SimplCommerce.Module.Core.Services
{
    
    // This class is used by the application to send Email and SMS
    // when you turn on two-factor authentication in ASP.NET Identity.
    // For more details see this link http://go.microsoft.com/fwlink/?LinkID=532713
    public class AuthMessageSender : IEmailSender, ISmsSender
    {
        public AuthMessageSender(IOptions<AuthMessageSenderOptions> optionsAccessor)
        {
            Options = optionsAccessor.Value;
        }

        public AuthMessageSenderOptions Options { get; } // set only via Secret Manager

        public Task SendEmailAsync(string email, string subject, string message)
        {
            // use gmail's smtp services to send mail - requires a gmail account

            using (var client = new SmtpClient())
            {
                // https://github.com/jstedfast/MailKit/issues/307
                client.ServerCertificateValidationCallback= (sender, certificate, chain, sslPolicyErrors) => { return true; };

                client.Connect("smtp.gmail.com", 465, SecureSocketOptions.SslOnConnect);

                /*
                TODO: Creds here via Options are only valid for development, per UserSecrets in
                Startup.Startup
                */
                client.Authenticate(Options.SendMailUser, Options.SendMailPassword);

                var mail = new MimeMessage();

                // looks like From is required, but address arg gets replaced by authenticated account address, by Send
                mail.From.Add(new MailboxAddress("SimplCommerce", "noreply@esimplcommerce.com"));
                mail.To.Add(new MailboxAddress(email));
                mail.Subject = subject;
                mail.Body = new TextPart("plain") { Text = message };
                client.Send(mail);
                client.Disconnect(true);
            }

            return Task.FromResult(0);
        }

        public Task SendSmsAsync(string number, string message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }
}
