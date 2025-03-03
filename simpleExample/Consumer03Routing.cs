using System;
using RabbitMQ.Client;  
using RabbitMQ.Client.Events;
using System.Text;


public class Consumer03Routing
{

    private const string QUEUE_NAME= "directqueue";
    private const string QUEUE_NAME_EVEN = "directqueue_even";
    private const string QUEUE_NAME_ODD = "directqueue_odd";
    private const string EXCHANGE_ROUTING = "directexchange";



    public static void Main(string[] args)
    {
        Console.WriteLine($" [x] Consumer ");

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

        channel.QueueDeclare(queue:QUEUE_NAME,
                                durable: true,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);

        channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
        
        channel.ExchangeDeclare(exchange: EXCHANGE_ROUTING, type: ExchangeType.Direct);

        channel.QueueBind(queue: QUEUE_NAME_EVEN, exchange: EXCHANGE_ROUTING, routingKey: "even");

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received  += (model, ea) =>
        {
            HandleMessage(channel, ea);
        };
        

        Console.WriteLine($" [x] 執行: BasicConsume");
        string reuslt =  channel.BasicConsume(
                                queue: QUEUE_NAME_EVEN,
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






