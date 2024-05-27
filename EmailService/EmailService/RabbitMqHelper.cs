using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EmailService;

public class RabbitMqHelper
{
    private readonly string _hostName;
    private readonly string _userName;
    private readonly string _password;
    private IConnection _connection;
    private IModel _channel;

    public RabbitMqHelper(string hostName, string userName, string password)
    {
        _hostName = hostName;
        _userName = userName;
        _password = password;
        InitializeConnection();
    }

    private void InitializeConnection()
    {
        var factory = new ConnectionFactory()
        {
            HostName = _hostName,
            UserName = _userName,
            Password = _password
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    public void Receive(string queueName, Action<string> onMessageReceived)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            var email = Encoding.UTF8.GetString(ea.Body.ToArray());
            Console.WriteLine($"Received email to send to: {email}");
            onMessageReceived?.Invoke(email);
        };

        _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
    }

    public void Close()
    {
        _channel?.Close();
        _connection?.Close();
    }
}