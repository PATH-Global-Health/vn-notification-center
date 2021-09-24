﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Data.ViewModels;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Services.Core;

namespace Services.RabbitMQ
{
    public class Consumer : BackgroundService
    {
        private IConnection connection;
        private IModel channel;
        private readonly IServiceScopeFactory _scopeFactory;
        private EventingBasicConsumer consumer;
        private readonly IConfiguration _configuration;
        private readonly ILogger<Consumer> _logger;

        public Consumer(IServiceScopeFactory scopeFactory, IConfiguration configuration, ILogger<Consumer> logger)
        {
            _scopeFactory = scopeFactory;
            _configuration = configuration;
            _logger = logger;
            InitRabbitMQ();
        }

        private void InitRabbitMQ()
        {
            try
            {
                var factory = new ConnectionFactory();
                _configuration.Bind("RabbitMqConnection", factory);

                connection = factory.CreateConnection();
                channel = connection.CreateModel();

                channel.QueueDeclare(queue: "NewNotification", durable: false,
                  exclusive: false, autoDelete: false, arguments: null);
                channel.BasicQos(0, 1, false);
                consumer = new EventingBasicConsumer(channel);
                channel.BasicConsume(queue: "NewNotification",
                  autoAck: false, consumer: consumer);
                _logger.LogInformation("-RabbitMQ queue created: HomeQuarantineCreateAccount");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "RabbitMQ queue create fail.");
            }
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            consumer.Received += (model, ea) =>
            {
                string response = null;

                var body = ea.Body.ToArray();
                var props = ea.BasicProperties;
                var replyProps = channel.CreateBasicProperties();
                replyProps.CorrelationId = props.CorrelationId;

                try
                {
                    var message = Encoding.UTF8.GetString(body);
                    var result = SendNotification(message);
                    if (result.Succeed == false)
                    {
                        response = result.ErrorMessages;
                    }
                    else
                    {
                        response = result.Data + "";
                    }
                }
                catch (Exception e)
                {
                    response = e.Message;
                }
                finally
                {
                    var responseBytes = Encoding.UTF8.GetBytes(response);
                    channel.BasicPublish(exchange: "", routingKey: props.ReplyTo,
                      basicProperties: replyProps, body: responseBytes);
                    channel.BasicAck(deliveryTag: ea.DeliveryTag,
                      multiple: false);
                }

            };
            return Task.CompletedTask;
        }
        private ResultModel SendNotification(string message)
        {
            var messageDTO = JsonConvert.DeserializeObject<NotificationAddModel>(message);
            using var scope = _scopeFactory.CreateScope();
            INotificationService _notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
            return _notificationService.Add(messageDTO);
        }
    }
}
