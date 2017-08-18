using EasyNetQ;
using System.Threading;

namespace AkkaStreamsSample.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("Publishing messages, press any key to terminate");

            using (var bus = RabbitHutch.CreateBus("host=localhost"))
            {
                var t = new Timer(state => 
                {
                    var message = state as Message;

                    System.Console.WriteLine("Publishing message #{0}", message.Id);

                    bus.Publish(message);

                    var nextId = int.Parse(message.Id) + 1;

                    message.Id = nextId.ToString();
                    message.Text = $"Message #{nextId}";
                }, 
                new Message { Id = "1", Text = "Message #1" }, 0, 5000);

                System.Console.ReadKey();
            }
        }
    }
}
