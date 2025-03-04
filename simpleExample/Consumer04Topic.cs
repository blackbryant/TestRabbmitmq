using System;
using RabbitMQ.Client;  
using RabbitMQ.Client.Events;
using System.Text;


public class Consumer04Topic
{

    private const string QUEUE_NAME= "topic_queue";
    private const string EXCHANGE_ROUTING = "topic_exchange";



    public static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.Error.WriteLine("Usage: {0} [binding_key...]",
                                    Environment.GetCommandLineArgs()[0]);
            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
            Environment.ExitCode = 1;
            return;
        }

        Console.WriteLine($" [x]  Topic Consumer ");

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

        var queue = channel.QueueDeclare( 
                                durable: true,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);

        string queueName = queue.QueueName;
        Console.WriteLine($" [x] Queue Name: {queueName}");

        channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
        
        channel.ExchangeDeclare(exchange: EXCHANGE_ROUTING, type: ExchangeType.Topic);

        foreach (var bindingKey in args)
        {
            Console.WriteLine($" [x] bindingKey: {bindingKey}");
            channel.QueueBind(queue: queueName, exchange: EXCHANGE_ROUTING, routingKey: bindingKey);
        }

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received  += (model, ea) =>
        {
            HandleMessage(channel, ea);
        };
        

        Console.WriteLine($" [x] 執行: BasicConsume");
        string reuslt =  channel.BasicConsume(
                                queue: queueName,
                                autoAck: false,
                                consumer: consumer
                            );

        Console.ReadLine();

    }

    // 回呼函式
    private static void HandleMessage(IModel channel, BasicDeliverEventArgs ea)
    {
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        
        Thread.Sleep(1000);
        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        Console.WriteLine($" [x] Received: '{ea.RoutingKey}':{message}");
        
    }
}






