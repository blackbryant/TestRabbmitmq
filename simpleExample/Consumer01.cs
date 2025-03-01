using System;
using RabbitMQ.Client;  
using RabbitMQ.Client.Events;
using System.Text;


public class Consumer01
{
    public static void Main(string[] args)
    {
        Console.WriteLine($" [x] Consumer ");

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

        channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
        var consumer = new EventingBasicConsumer(channel);
        channel.BasicConsume(queue: "tutorial",
                                autoAck: true,
                                consumer: consumer
                                
                                );

        
        consumer.Received += (model, ea) =>
        {  
             
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($" [x] Received: {message}");
        };

        
    }
}





