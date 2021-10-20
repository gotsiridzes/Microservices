using Microsoft.Extensions.Configuration;
using PlatformService.DataTransferObjects;
using RabbitMQ.Client;
using System;
using System.Text;
using System.Text.Json;

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
            var messageObject = JsonSerializer.Serialize(platformPublishedDto);

            if (_connection.IsOpen)
            {
                Console.WriteLine("RabbitMQ connection is open, sending message ...");
                SendMessage(messageObject);
            }
            else
            {
                Console.WriteLine("RabbitMQ connection is closed, not sending");
            }
        }

        private void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish("trigger", "", null, body);
            Console.WriteLine($"We Have sent {message}");

        }

        public void Dispose()
        {
            Console.WriteLine("MessageBus Disposed");
            
            if (_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }
        }

        private void RabbitMq_ConnectionShutDown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine("RabbitMQ Connection Shut Down");
        }
    }
}
