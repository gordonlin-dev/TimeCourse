using Newtonsoft.Json;
using RabbitMQ.Client;
using Shared.Model;
using System.Text;

namespace Appointments.API.Services
{
    public interface IRabbitMqPublisher
    {
        public void PublishMessage(UserAppointmentMessage message);
    }

    public class RabbitMqPublisher : IRabbitMqPublisher
    {
        private readonly ConnectionFactory _factory;
        private IConnection _connection;
        private IModel _channel;
        public RabbitMqPublisher()
        {
            _factory = new ConnectionFactory();
            //TODO: Get from settings/environment variable
            _factory.UserName = "";
            _factory.VirtualHost = "";
            _factory.Password = "";
            _factory.Port = 5672;
            _factory.Uri = new Uri("");
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();
        }
        public void PublishMessage(UserAppointmentMessage message)
        {
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            _channel.BasicPublish(exchange: "", routingKey: "Appointment", basicProperties: null, body: body);
        }
    }
}
