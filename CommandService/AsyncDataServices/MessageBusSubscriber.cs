using CommandService.EventProcessing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommandService.AsyncDataServices;

public class MessageBusSubscriber : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly IEventProcessor _eventProcessor;
    private IConnection _connection;
    private IModel _channel;
    private string _queueName;
    private bool _subscribed = false;

    public MessageBusSubscriber(IConfiguration configuration, IEventProcessor eventProcessor)
    {
        _configuration = configuration;
        _eventProcessor = eventProcessor;
        TryInternalizeRabbitMq();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (ModuleHandle, ea) =>
        {
            Log.Information("Event Recieved!");
            var body = ea.Body;
            var notificationMessage = Encoding.UTF8.GetString(body.ToArray());

            _eventProcessor.ProcessEvent(notificationMessage);
        };

        if(_subscribed)
            _channel.BasicConsume(
                queue: _queueName,
                autoAck: true,
                consumer: consumer);

        return Task.CompletedTask;
    }

    protected void TryInternalizeRabbitMq()
    {
        var factory = new ConnectionFactory()
        {
            HostName = _configuration["RabbitMQHost"],
            Port = int.Parse(_configuration["RabbitMQPort"])
        };
        try
        {
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare("trigger", ExchangeType.Fanout);
            _queueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(_queueName, exchange: "trigger", routingKey: "");
            _subscribed = true;
            Log.Information("Listening on Message Bus");
            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutDown;
        }
        catch(Exception ex)
        {
            _subscribed = false;
            Log.Error(ex, "Could not subscribe to Message Bus");
        }
    }

    private void RabbitMQ_ConnectionShutDown(object sender, ShutdownEventArgs e)
    {
        Log.Information("Connection Shutdown");        
    }

    public override void Dispose()
    {
        if (_channel.IsOpen)
        {
            _channel.Close();
            _connection.Close();
        }
        base.Dispose();
    }
}
