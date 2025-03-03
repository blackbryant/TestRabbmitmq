
/// <summary>
/// Direct exchange routing example
/// This class demonstrates a simple RabbitMQ producer that sends persistent messages to a queue.
/// </summary>
/// 
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

public class Producer04Routing
{

    private const string QUEUE_NAME= "directqueue";
    private const string QUEUE_NAME_EVEN = "directqueue_even";
    private const string QUEUE_NAME_ODD = "directqueue_odd";
    private const string EXCHANGE_ROUTING = "directexchange";

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    /// <param name="args">Command-line arguments.</param>
    public static void Main(string[] args)
    {
        // Configuration for RabbitMQ connection
        string rabbitMqHost = RabbitConfig.HOST;
        int rabbitMqPort = RabbitConfig.PORT;
        string rabbitMqUser = RabbitConfig.MQ_USER;
        string rabbitMqPass = RabbitConfig.MQ_PASS;

        // Create a connection factory with the specified settings
        var factory = new ConnectionFactory()
        {
            HostName = rabbitMqHost,
            Port = rabbitMqPort,
            UserName = rabbitMqUser,
            Password = rabbitMqPass
        };

        // Establish a connection and create a channel
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        // Declare a queue with persistence enabled
        channel.QueueDeclare(queue: QUEUE_NAME_EVEN,
                             durable: true,  // Enable persistence
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);

        channel.QueueDeclare(queue: QUEUE_NAME_ODD,
                             durable: true,  // Enable persistence
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);


         // Declare a direct exchange
        channel.ExchangeDeclare(exchange: EXCHANGE_ROUTING, type: ExchangeType.Direct);
        
         // Bind the queue to the exchange with routing keys
        channel.QueueBind(queue: QUEUE_NAME_EVEN, exchange: EXCHANGE_ROUTING, routingKey: "even");
        channel.QueueBind(queue: QUEUE_NAME_ODD , exchange: EXCHANGE_ROUTING, routingKey: "odd");

        
        // Create properties for the message and set it to be persistent
        var properties = channel.CreateBasicProperties();
        properties.Persistent = true; // Set message to be persistent

        // Send 20 messages to the queue
        for (int i = 0; i < 20; i++)
        {
            Message message = new Message($" Message-{i}");


            string key  = "";
            
            if(i % 2 == 0)
            {
                key = "even";
                
            }
            else
            {
                key = "odd";
                
            }

            message.RoutingKey = key;
            // Publish the message to the queue
            var body = Encoding.UTF8.GetBytes(message.ToJson());
            channel.BasicPublish(
                                 exchange: EXCHANGE_ROUTING,
                                 routingKey: key,
                                 basicProperties: properties,
                                 body: body);

            // Output the sent message to the console
            Console.WriteLine($" [x] Sent: {message.ToJson()}");
        }

        // Inform the user that messages have been sent and wait for a key press to exit
        Console.WriteLine("Messages sent. Press any key to exit.");
        Console.ReadKey();
    }


    public class Message
    {
        public string Uuid { get; set; }
        public string? Text { get; set; }

        public string? RoutingKey { get; set; }
        

        public Message()
        {
            Uuid = Guid.NewGuid().ToString();
        }

        public Message(string text)
        {
            Text = text;
            Uuid = Guid.NewGuid().ToString();
        }

        public override string ToString()
        {
            return $"[x] Sent: {Text} - {Uuid} - {RoutingKey}";
        }

        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }

    }

}