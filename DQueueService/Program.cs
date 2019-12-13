using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace DQueueService
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("start");
            //ActitvmqService activemq = new ActitvmqService();
            //activemq.SendMessActitvMQ();

            //RabbitMQService rabbitMQ = new RabbitMQService();
            //rabbitMQ.SendMessActitvMQ();

            //var factory = new ConnectionFactory();
            //factory.HostName = "106.15.180.98";
            //factory.UserName = "zxsj";
            //factory.Password = "zxsj";

            //using (var connection = factory.CreateConnection())
            //{
            //    using (var channel = connection.CreateModel())
            //    {
            //        ////申明交换机 
            //        //this._channel.ExchangeDeclare(this.ExchangeName, this.RType.ToString().ToLower(), true, false, null);
            //        ////申明队列
            //        ////string queue(队列名称), 
            //        ////bool durable(是否持久化), bool exclusive(是否排外（专有的）), bool autoDelete(是否自动删除)
            //        //this._channel.QueueDeclare(this.QueueName, true, false, false, null);
            //        ////绑定交换机
            //        //this._channel.QueueBind(this.QueueName, this.ExchangeName, this.RoutingKey, null);

            //        channel.QueueDeclare("QueueNameTest", true, false, false, null);

            //        var consumer = new EventingBasicConsumer(channel);
            //        channel.BasicConsume("QueueNameTest", false, consumer);
            //        consumer.Received += (model, ea) =>
            //        {
            //            var body = ea.Body;
            //            var message = Encoding.UTF8.GetString(body);
            //            Console.WriteLine("已接收： {0}", message);
            //        };
            //        Console.ReadLine();
            //    }
            //}


            //create client instance  MqttClient client =  new  MqttClient(  "   127.0.0.1   "   );   //   register to message received  client.MqttMsgPublishReceived +=  client_MqttMsgPublishReceived;   string  clientId =  Guid.NewGuid().ToString();
            MqttClient client = new MqttClient("10.16.0.78");
            //register to message received  
            
            client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
            var clientId = "dragon";
            client.Connect(clientId, "ylzyx", "ylzyx", false, 60);

            client.Subscribe(new string[] { "topic.ylz" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
            
            Console.WriteLine("end");
            Console.ReadLine();

        }

        static void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            string msg = Encoding.Default.GetString(e.Message);
            
            Console.WriteLine(msg);
        }


    }

  
}
