﻿
/// <summary>
/// This class demonstrates a simple RabbitMQ producer that sends persistent messages to a queue.
/// </summary>
/// 
using System.Text;
using RabbitMQ.Client;

public class Producer02
{
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
        channel.QueueDeclare(queue: "tutorial",
                             durable: true,  // Enable persistence
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);

        // Create properties for the message and set it to be persistent
        var properties = channel.CreateBasicProperties();
        properties.Persistent = true; // Set message to be persistent

        // Send 20 messages to the queue
        for (int i = 0; i < 20; i++)
        {
            string msg = Guid.NewGuid().ToString();
            var body = Encoding.UTF8.GetBytes(msg);

            // Publish the message to the queue
            channel.BasicPublish(exchange: "",
                                 routingKey: "tutorial",
                                 basicProperties: properties,
                                 body: body);

            // Output the sent message to the console
            Console.WriteLine($" [x] Sent: {msg}");
        }

        // Inform the user that messages have been sent and wait for a key press to exit
        Console.WriteLine("Messages sent. Press any key to exit.");
        Console.ReadKey();
    }
}