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

        if(File.Exists("log.csv"))
        {
            File.Delete("log.csv");
           
        }

        FileStream fs =  File.Create("log.csv");
        StreamWriter sw = new StreamWriter(fs);
        
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
     
        
        consumer.Received  += (model, ea) =>
        {
            HandleMessage(channel, ea, fs);
        };
        

        Console.WriteLine($" [x] 執行: BasicConsume");
        string reuslt =  channel.BasicConsume(queue: "tutorial",
                            autoAck: false,
                            consumer: consumer
                            );

        if(reuslt == null)
        {
            fs.Close();
            sw.Close();          
        }


        Console.ReadLine();

    }

    // 回呼函式
    private static void HandleMessage(IModel channel, BasicDeliverEventArgs ea, FileStream fs)
    {
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        
        Thread.Sleep(2000);
        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        Console.WriteLine($" [x] Received: {message}");
        string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss},{message} \n"; 
        fs.Write(Encoding.UTF8.GetBytes(logEntry), 0, Encoding.UTF8.GetBytes(logEntry).Length);
        fs.Flush();
    }
}






