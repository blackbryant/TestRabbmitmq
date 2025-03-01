
using System.Text;
using RabbitMQ.Client;


public class Producer01
{
    public static void Main(string[] args)
    {
        string rabbitMqHost = RabbitConfig.HOST;
        int rabbitMqPort = RabbitConfig.PORT; // 這裡應該是數字，不是字串
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
                                durable: false,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);

        for (int i = 0; i < 20; i++)
        {
            string msg = Guid.NewGuid().ToString();
            var body = System.Text.Encoding.UTF8.GetBytes(msg);
            byte[] messageBodyBytes = System.Text.Encoding.UTF8.GetBytes("Hello, world!");
            
            channel.BasicPublish(exchange: "",
                                routingKey: "tutorial",
                                basicProperties: null,
                                body: body);

                                

            Console.WriteLine($" [x] Sent: {msg}");
        }

        Console.WriteLine("Messages sent. Press any key to exit.");
        Console.ReadKey();
    }
}