﻿using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using RabbitMQ.Client;
using Stripe;
using Stripe.Checkout;
using PaymentService3.Models;
using System;

namespace PaymentService3.Controllers
{
    /// <summary>
    /// Controlleren er ansvarlig for håndtering af betalingsrelaterede operationer.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class PaymentController : Controller
    {
        private readonly StripeSettings _stripeConfig;

        /// <summary>
        /// Initialiserer en ny instans af <see cref="PaymentController"/>-klassen.
        /// </summary>
        /// <param name="configuration">Konfigurationen, der indeholder Stripe-indstillingerne.</param>
        public PaymentController(IConfiguration configuration)
        {
            _stripeConfig = configuration.GetSection("StripeSettings").Get<StripeSettings>();
            StripeConfiguration.ApiKey = _stripeConfig.SecretKey;
        }

        /// <summary>
        /// Opretter en ny betalingssession til behandling af betalinger.
        /// </summary>
        /// <param name="request">Anmodningen indeholder betalingsoplysninger.</param>
        /// <returns>En handling, der repræsenterer resultatet af operationen.</returns>
        [HttpPost]
        [Route("create-checkout-session")]
        public async Task<IActionResult> CreateCheckoutSession([FromBody] CreateCheckoutSessionRequest request)
        {
            try
            {
                int customerId = request.CustomerId;
                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string>
                    {
                        "card",
                    },
                    LineItems = new List<SessionLineItemOptions>
                    {
                        new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                Currency = "usd",
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = request.ProductName,
                                },
                                UnitAmount = request.ProductPrice,
                            },
                            Quantity = request.Quantity,
                        },
                    },
                    Mode = "payment",
                    SuccessUrl = "https://example.com/success",
                    CancelUrl = "https://example.com/cancel",
                };

                var service = new SessionService();
                var session = await service.CreateAsync(options);

                // Send customerId to RabbitMQ
                SendCustomerIdToQueue(customerId.ToString());

                return Ok(new { sessionId = session.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        /// <summary>
        /// Sender kunde-id til RabbitMQ-køen.
        /// </summary>
        /// <param name="customerId">Kunde-id, der skal sendes til køen.</param>
        private void SendCustomerIdToQueue(string customerId)
        {
            var factory = new ConnectionFactory() { HostName = "centralrepository3-rabbitmq-1", UserName = "user", Password = "password" }; // Opdater til din RabbitMQ-serveradresse

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "paymentQueueTest",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                var body = Encoding.UTF8.GetBytes(customerId);

                channel.BasicPublish(exchange: "",
                    routingKey: "paymentQueue",
                    basicProperties: null,
                    body: body);
                Console.WriteLine(" [x] Sent {0}", customerId);
            }
        }
    }
}
