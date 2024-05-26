using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Models;

namespace UserService.Services
{
    public class RabbitMQListener : BackgroundService
    {
        private IModel _channel;
        private IConnection _connection;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly string _hostname = "localhost";
        private readonly string _paymentQueueName = "paymentQueue";
        private readonly string _emailQueueName = "emailQueue"; // New queue for emails

        public RabbitMQListener(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            InitializeRabbitMQ();
        }

        private void InitializeRabbitMQ()
        {
            var factory = new ConnectionFactory() { HostName = _hostname };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: _paymentQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueDeclare(queue: _emailQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null); // Declare the email queue
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                int customerId;
                if (int.TryParse(content, out customerId))
                {
                    await ProcessMessage(customerId);
                }
                else
                {
                    Console.WriteLine($"Failed to convert message to int: {content}");
                }

                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume(_paymentQueueName, false, consumer);
            return Task.CompletedTask;
        }

        private async Task ProcessMessage(int customerId)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<MyDbContext>();
                var _logger = scope.ServiceProvider.GetRequiredService<ILogger<RabbitMQListener>>();

                var customer = await _context.Customers
                    .FromSqlRaw("SELECT * FROM Customers WHERE CustomerId = {0}", customerId)
                    .SingleOrDefaultAsync();

                if (customer == null)
                {
                    _logger.LogWarning($"No customer found with ID: {customerId}");
                }
                else
                {
                    _logger.LogInformation($"Retrieved customer: {customer.Email}");

                    // Send email to the emailQueue
                    var emailBody = Encoding.UTF8.GetBytes(customer.Email);
                    _channel.BasicPublish(exchange: "",
                                          routingKey: _emailQueueName,
                                          basicProperties: null,
                                          body: emailBody);
                    _logger.LogInformation($"Email sent to queue for customer {customer.Email}");
                }
            }
        }

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            base.Dispose();
        }
    }
}
