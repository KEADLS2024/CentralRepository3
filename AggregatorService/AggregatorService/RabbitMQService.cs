using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace AggregatorService
{
    public interface IRabbitMQService
    {
        void StartConsuming();
    }

    public class RabbitMQService : IRabbitMQService
    {
        private readonly IConfiguration _configuration;

        public RabbitMQService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void StartConsuming()
        {
            var factory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQSettings:HostName"],
                UserName = _configuration["RabbitMQSettings:UserName"],
                Password = _configuration["RabbitMQSettings:Password"]
            };

            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.QueueDeclare(queue: "aggregator_queue",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(" [x] Received {0}", message);
                // Implement your aggregation logic here
            };
            channel.BasicConsume(queue: "aggregator_queue",
                autoAck: true,
                consumer: consumer);
        }
    }
}