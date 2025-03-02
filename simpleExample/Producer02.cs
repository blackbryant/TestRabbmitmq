
using System.Text;
using RabbitMQ.Client;

//加入持久化

public class Producer02
{
    public static void Main(string[] args)
    {
        string rabbitMqHost = RabbitConfig.HOST;
        int rabbitMqPort = RabbitConfig.PORT;
        string rabbitMqUser = RabbitConfig.MQ_USER;
        string rabbitMqPass = RabbitConfig.MQ_PASS;

        var factory = new ConnectionFactory()
        {
            HostName = rabbitMqHost,
            Port = rabbitMqPort,
            UserName = rabbitMqUser,
            Password = rabbitMqPass
        };

        using var connection =  factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(queue: "tutorial",
                                durable: true,  //持久化
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);

        var properties = channel.CreateBasicProperties();
        properties.Persistent = true; // 設置訊息為持久化

        for (int i = 0; i < 20; i++)
        {
            string msg = Guid.NewGuid().ToString();
            var body = System.Text.Encoding.UTF8.GetBytes(msg);
            byte[] messageBodyBytes = System.Text.Encoding.UTF8.GetBytes("Hello, world!");
            
            channel.BasicPublish(exchange: "",
                                routingKey: "tutorial",
                                basicProperties: properties,
                                body: body);

                                

            Console.WriteLine($" [x] Sent: {msg}");
        }

        Console.WriteLine("Messages sent. Press any key to exit.");
        Console.ReadKey();
    }
}