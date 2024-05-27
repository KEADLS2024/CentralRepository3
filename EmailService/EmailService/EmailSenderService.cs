using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;



using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

using Microsoft.Extensions.Hosting;


namespace EmailService;



    public class EmailSenderService : BackgroundService
    {
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var rabbitMqHelper = new RabbitMqHelper("centralrepository3-rabbitmq-1", "user", "password");
        rabbitMqHelper.Receive("emailQueue", SendEmail);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }

        rabbitMqHelper.Close();


        //RabbitMqHelper.Receive("UserEmailQueue", emailAddress =>
        //{
        //    SendEmail(emailAddress);
        //});

        //while (!stoppingToken.IsCancellationRequested)
        //{
        //    await Task.Delay(1000, stoppingToken); // Holder servicen kørende
        //}
    }

    private void SendEmail(string toAddress)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Usama Ahmad", "usama-ahmad@outlook.com"));
            email.To.Add(new MailboxAddress("", toAddress));
            email.Subject = "Test Email from Your Service";
            email.Body = new TextPart("plain") { Text = "Hello, this is a test email from your service." };

            using var smtp = new SmtpClient();
            smtp.Connect("smtp-mail.outlook.com", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("usama-ahmad@outlook.com", "Talhahuzaifa1");
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }

    

