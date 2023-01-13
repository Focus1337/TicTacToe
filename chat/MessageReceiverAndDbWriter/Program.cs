using System.Text;
using System.Text.Json;
using MessageReceiverAndDbWriter;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

for (;;)
{
    IConnection? connection = null;
    try
    {
        var factory = new ConnectionFactory { HostName = "rabbitmq" };
        connection = factory.CreateConnection();
        var channel = connection.CreateModel();
        channel.QueueDeclare(queue: "messages", durable: true, exclusive: false, autoDelete: false, arguments: null);

        Console.WriteLine("Connection opened");

        var context = new AppDbContext();


        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (ch, ea) =>
        {
            var content = Encoding.UTF8.GetString(ea.Body.ToArray());
            var message = JsonSerializer.Deserialize<Message>(content);
            if (message is null)
            {
                Console.WriteLine($"Broken message: '{content}'");
                return;
            }

            Console.WriteLine($"New message: {message.UserName} says '{message.Text}'");
            context.Messages.Add(message);
            context.SaveChanges();
            channel.BasicAck(ea.DeliveryTag, false);
        };
        for (;;)
        {
            channel.BasicConsume("messages", false, consumer);
        }
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
    }
    finally
    {
        Console.WriteLine("Closing connection");
        connection?.Close();
        await Task.Delay(100);
    }
}