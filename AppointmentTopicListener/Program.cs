using AppointmentTopicListener;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Model;
using Shared.Services;
using System.Text;

var config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false)
        .Build();

var serviceCollection = new ServiceCollection()
    .Configure<MongoDbSettings>(config.GetSection("UserDatabase"))
    .Configure<UserDatabaseSettings>(config.GetSection("UserDatabase"))
    .AddSingleton<IMessageHandler, MessageHandler>()
    .AddSingleton<IMongoDBService, MongoDBService>()
    .AddSingleton<IUserService, UserService>();

var serviceProvider = serviceCollection.BuildServiceProvider();
var messageHandler = serviceProvider.GetService<IMessageHandler>();

var factory = new ConnectionFactory();
//TODO: Get from settings/environment variable
factory.UserName = "";
factory.VirtualHost = "";
factory.Password = "";
factory.Port = 5672;
factory.Uri = new Uri("");

using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
    channel.QueueDeclare(queue: "Appointment",
                         durable: true,
                         exclusive: false,
                         autoDelete: false,
                         arguments: null);

    var consumer = new EventingBasicConsumer(channel);
    consumer.Received += async (model, eventArguments) =>
    {
        var body = eventArguments.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        await messageHandler.HandleMessage(message);
    };
    channel.BasicConsume(queue: "Appointment",
                         autoAck: true,
                         consumer: consumer);
    Console.ReadLine();
}