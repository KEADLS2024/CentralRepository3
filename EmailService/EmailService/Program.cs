using System.Net.Mail;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EmailService;

class Program
{
    static void Main(string[] args)
    {
        var rabbitMqHelper = new RabbitMqHelper("centralrepository3-rabbitmq-1", "user", "password");
        CreateHostBuilder(args).Build().Run();

        rabbitMqHelper.Receive("emailQueue", SendConfirmationEmail);

        Console.WriteLine("Press [enter] to exit.");
        Console.ReadLine();

        

        rabbitMqHelper.Close();

        //var factory = new ConnectionFactory() { HostName = "centralrepository3-rabbitmq-1", UserName = "user", Password = "password" };
        //using var connection = factory.CreateConnection();
        //using var channel = connection.CreateModel();

        ////channel.QueueDeclare(queue: "emailQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);

        //var consumer = new EventingBasicConsumer(channel);
        //consumer.Received += (model, ea) =>
        //{
        //    var email = Encoding.UTF8.GetString(ea.Body.ToArray());
        //    Console.WriteLine($"Received email to send to: {email}");
        //    SendConfirmationEmail(email);
        //};

        //channel.BasicConsume(queue: "emailQueue", autoAck: true, consumer: consumer);

        //Console.WriteLine("Press [enter] to exit.");
        //Console.ReadLine();
    }

    static void SendConfirmationEmail(string email)
    {
        using var smtpClient = new SmtpClient("smtp-mail.outlook.com")
        {
            Credentials = new System.Net.NetworkCredential("usama-ahmad@outlook.com", "Talhahuzaifa1"),
            EnableSsl = true
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress("usama-ahmad@outlook.com"),
            Subject = "Payment Confirmation",
            Body = "Your payment was successful.",
            IsBodyHtml = true,
        };
        mailMessage.To.Add(email);

        try
        {
            smtpClient.Send(mailMessage);
            Console.WriteLine($"Confirmation email sent to {email}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to send email to {email}. Error: {ex.Message}");
        }


    }
    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<EmailSenderService>();
            });
}