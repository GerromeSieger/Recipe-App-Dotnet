using RecipeApp.Data;
using RecipeApp.ServicesInterface;
using System.Net.Mail;

namespace RecipeApp.Services
{
    public class Mailer : IMailer
    {
        private readonly DataContext dataContext;

        private readonly IConfiguration _configuration;
        public Mailer(DataContext dataContext, IConfiguration configuration)
        {
            this.dataContext = dataContext;

            this._configuration = configuration;
        }

        public bool SendMail(string Email, string body, string Subject)
        {
            // Command-line argument must be the SMTP host.
            SmtpClient client = new SmtpClient(_configuration["EmailConfiguration:SmtpServer"], 587);

            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(_configuration["EmailConfiguration:Username"],
            _configuration["EmailConfiguration:Password"]);
            client.EnableSsl = true;
            // Specify the email sender.
            MailAddress from = new MailAddress(_configuration["EmailConfiguration:From"], _configuration["AppDisplayName"]);
            // Set destinations for the email message.
            MailAddress to = new MailAddress(Email);
            // Specify the message content.
            MailMessage message = new MailMessage(from, to);
            message.Subject = Subject;
            message.Body = body;
            var res = false;
            try
            {
                client.Send(message);
                res = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
            message.Dispose();
            return res;
        }
    }
}
