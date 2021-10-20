using Microsoft.Extensions.Configuration;
using PlatformService.DataTransferObjects;
using RabbitMQ.Client;
using System;

namespace PlatformService.AsyncDataServices
{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly IConfiguration _configuration;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public MessageBusClient(IConfiguration configuration)
        {
            _configuration = configuration;
            var factory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQHost"],
                Port = int.Parse(_configuration["RabbitMQPort"])
            };

            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
                _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
                _connection.ConnectionShutdown += RabbitMq_ConnectionShutDown;
                
                Console.WriteLine("Connected To Message Bus");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could Not Connect to the Message Bus: {ex}");
            }
        }

        public void PublishNewPlatform(PlatformPublishedDto platformPublishedDto)
        {
            
        }

        private void RabbitMq_ConnectionShutDown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine("RabbitMQ Connection Shut Down");
        }
    }
}
