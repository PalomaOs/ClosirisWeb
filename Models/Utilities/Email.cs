using System;
using System.Configuration;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace closirissystem
{
    public class Email
    {
        private static readonly IConfiguration _configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();
        public void SendEmail(string subject, string text, string address)
        {
            try
            {
                var smtpSettings = _configuration.GetSection("SmtpSettings");

                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;

                string smtpUsername = smtpSettings["SmtpUser"];
                string smtpPassword = smtpSettings["SmtpPassword"];

                smtp.Credentials = new NetworkCredential(smtpUsername, smtpPassword);

                MailMessage mailMessage = new MailMessage();
                mailMessage.IsBodyHtml = true;
                mailMessage.Priority = MailPriority.Normal;
                mailMessage.From = new MailAddress(smtpUsername);
                mailMessage.Subject = subject;
                mailMessage.Body = text;
                mailMessage.To.Add(new MailAddress(address));

                smtp.Send(mailMessage);
            }
            catch (SmtpException ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }

        public string Format(String token)
        {
            String content = "<html>\n"
                + "<head>\n"
                + "    <title>Sistema</title>\n"
                + "</head>\n"
                + "<body>\n"
                + "    <div style=\"text-align: center;\">\n"
                + "       <img src=\"https://i.ibb.co/VYpBZq0/Closiris-Header.png\" alt=\"Closiris-Header\" border=\"0\">\n"
                + "    </div>\n"
                + "    <h1><center>" + "Cambio de contraseña" + "</h1>\n"
                + "    <p><center>" + "Su token" + "</p>\n"
                + "    <p><center>" + token + "</p>\n"
                + "    <div style=\"text-align: center;\">\n"
                + "        <img src=\"https://i.ibb.co/VmzcxR9/Closiris-Footpage.png\" alt=\"Closiris-Footpage\" border=\"0\">\n"
                + "    </div>\n"
                + "    <p><center>" + "Este correo es únicamente informativo" + "</p>\n"
                + "</body>\n"
                + "</html>";
            return content;
        }

        public string GenerateToken()
        {
            const string CHARACTERS = "0123456789";
            StringBuilder token = new StringBuilder();

            using (var randomGenerator = RandomNumberGenerator.Create())
            {
                byte[] data = new byte[6];
                randomGenerator.GetBytes(data);

                foreach (byte b in data)
                {
                    int index = b % CHARACTERS.Length;
                    token.Append(CHARACTERS[index]);
                }
            }
            return token.ToString();
        }
    }
}