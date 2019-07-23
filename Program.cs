using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.IO;

namespace PromoEmail
{

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Select Options:\n");
            Console.WriteLine("1 Send Resume / CV");
            Console.WriteLine("2 Send Reply\n");
            string choise = Console.ReadLine();
            int option = 0;
            while (!int.TryParse(Convert.ToString(choise), out option) || (option < 1 || option > 2))
            {
                Console.WriteLine("\nPlease select appropriate option: ");
                choise = Console.ReadLine();
            }

            Console.Write("\nPlease enter user name: ");
            string username = Console.ReadLine();
            username = !string.IsNullOrEmpty(username) ? username : "Dear User";

            Console.Write("Please enter the email address: ");
            string toEmail = Console.ReadLine();
            while (!new EmailAddressAttribute().IsValid(toEmail))
            {
                Console.WriteLine("Please enter correct email address: ");
                toEmail = Convert.ToString(Console.ReadLine());
            }

            Console.Write("Please enter the email subject: ");
            string subject = Console.ReadLine();
            subject = !string.IsNullOrEmpty(subject) ? subject : "Thanks for contacting !";

            if (option == 1)
                SendPromoEmail(username, toEmail, subject);
            else if (option == 2)
            {
                Console.Write("Please enter the email message: ");
                string message = Console.ReadLine();
                message = !string.IsNullOrEmpty(message) ? message : "Thanks for contacting !";

                SendReplyEmail(username, toEmail, subject, message);
            }
        }

        static void SendPromoEmail(string username, string toEmail, string subject)
        {
            string fromEmail = System.Configuration.ConfigurationManager.AppSettings["Email"];
            string password = System.Configuration.ConfigurationManager.AppSettings["Password"];
            string smtpClient = System.Configuration.ConfigurationManager.AppSettings["SMTPClient"];
            int port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Port"]);

            string file = File.ReadAllText(Directory.GetCurrentDirectory() + "\\Resume.htm");
            string body = ReplaceWords(file, username);
            MailMessage mail = new MailMessage(fromEmail, toEmail, subject, body);
            using (SmtpClient smtpServer = new SmtpClient(smtpClient, port))
            {
                try
                {
                    mail.IsBodyHtml = true;
                    smtpServer.Credentials = new System.Net.NetworkCredential(fromEmail, password);
                    smtpServer.EnableSsl = true;
                    smtpServer.Send(mail);
                    Console.WriteLine("Email sent successfully !");
                    Thread.Sleep(2000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Email failure: " + ex.Message);
                    Console.Read();
                }
            }
        }

        static void SendReplyEmail(string username, string toEmail, string subject, string message)
        {
            string fromEmail = System.Configuration.ConfigurationManager.AppSettings["Email"];
            string password = System.Configuration.ConfigurationManager.AppSettings["Password"];
            string smtpClient = System.Configuration.ConfigurationManager.AppSettings["SMTPClient"];
            int port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Port"]);

            string file = File.ReadAllText(Directory.GetCurrentDirectory() + "\\Reply.htm");
            string body = ReplaceWords(file, username, message);
            MailMessage mail = new MailMessage(fromEmail, toEmail, subject, body);
            using (SmtpClient smtpServer = new SmtpClient(smtpClient, port))
            {
                try
                {
                    mail.IsBodyHtml = true;
                    smtpServer.Credentials = new System.Net.NetworkCredential(fromEmail, password);
                    smtpServer.EnableSsl = true;
                    smtpServer.Send(mail);
                    Console.WriteLine("Email sent successfully !");
                    Thread.Sleep(2000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Email failure: " + ex.Message);
                    Console.Read();
                }
            }
        }


        static string ReplaceWords(string fileContent, string username)
        {
            return fileContent.Replace("[Username]", username);
        }

        static string ReplaceWords(string fileContent, string username, string message)
        {
            return fileContent.Replace("[Username]", username).Replace("[Message]", message);
        }
    }
}
