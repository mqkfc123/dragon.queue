using RabbitMQ.Client;
using RabbitMQ.Client.Content;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DQueue
{
    public class Rabbitmq3
    {
        public string ExchangeName { get; set; }
        public string TypeName { get; set; }
        public string RoutingKey { get; set; }
        public sbyte SendData(string aQueueIp, string aDataString, string aQueueName = "")
        {
            IPHostEntry hostEntry = Dns.GetHostEntry(aQueueIp);
            ConnectionFactory connectionFactory = new ConnectionFactory();
            connectionFactory.HostName = hostEntry.HostName;
            connectionFactory.VirtualHost = "/";
            connectionFactory.UserName = "admin";
            connectionFactory.Password = "admin";
            ConnectionFactory connectionFactory2 = connectionFactory;
            using (IConnection connection = connectionFactory2.CreateConnection())
            {
                using (IModel model = connection.CreateModel())
                {
                    model.ExchangeDeclare(this.ExchangeName, this.TypeName, true);
                    model.QueueDeclare(aQueueName, true, false, false, null);
                    model.QueueBind(aQueueName, this.ExchangeName, this.RoutingKey, null);
                    MapMessageBuilder mapMessageBuilder = new MapMessageBuilder(model);
                    IBasicProperties basicProperties = model.CreateBasicProperties();
                    basicProperties.DeliveryMode = 2;
                    model.BasicPublish(this.ExchangeName, this.RoutingKey, basicProperties, System.Text.Encoding.UTF8.GetBytes(aDataString));
                }
            }
            return 0;
        }
        public string GetData(string aQueueIp, string aQueueName = "")
        {
            IPHostEntry hostEntry = Dns.GetHostEntry(aQueueIp);
            ConnectionFactory connectionFactory = new ConnectionFactory();
            connectionFactory.HostName = hostEntry.HostName;
            connectionFactory.VirtualHost = "/";
            connectionFactory.UserName = "admin";
            connectionFactory.Password = "admin";
            ConnectionFactory connectionFactory2 = connectionFactory;
            string text = string.Empty;
            string result;
            using (IConnection connection = connectionFactory2.CreateConnection())
            {
                using (IModel model = connection.CreateModel())
                {
                    BasicGetResult basicGetResult = model.BasicGet(aQueueName, false);
                    if (basicGetResult != null)
                    {
                        try
                        {
                            bool redelivered = basicGetResult.Redelivered;
                            text = System.Text.Encoding.UTF8.GetString(basicGetResult.Body);
                            model.BasicAck(basicGetResult.DeliveryTag, false);
                        }
                        catch
                        {
                        }
                    }
                    result = text;
                }
            }
            return result;
        }
        public void SendDataByPublish(string aQueueIp, string aDataString, string aQueueName = "")
        {
            IPHostEntry hostEntry = Dns.GetHostEntry(aQueueIp);
            ConnectionFactory connectionFactory = new ConnectionFactory();
            connectionFactory.HostName = hostEntry.HostName;
            connectionFactory.VirtualHost = "/";
            connectionFactory.UserName = "admin";
            connectionFactory.Password = "admin";
            ConnectionFactory connectionFactory2 = connectionFactory;
            using (IConnection connection = connectionFactory2.CreateConnection())
            {
                using (IModel model = connection.CreateModel())
                {
                    model.ExchangeDeclare(this.ExchangeName, this.TypeName, true);
                    model.QueueDeclare(aQueueName, true, false, false, null);
                    model.QueueBind(aQueueName, this.ExchangeName, this.RoutingKey, null);
                    MapMessageBuilder mapMessageBuilder = new MapMessageBuilder(model);
                    IBasicProperties basicProperties = model.CreateBasicProperties();
                    basicProperties.DeliveryMode = 2;
                    model.BasicPublish(this.ExchangeName, this.RoutingKey, basicProperties, System.Text.Encoding.UTF8.GetBytes(aDataString));
                }
            }
        }
        public string GetDataBySubscribe(string aQueueIp, string aQueueName = "")
        {
            IPHostEntry hostEntry = Dns.GetHostEntry(aQueueIp);
            ConnectionFactory connectionFactory = new ConnectionFactory();
            connectionFactory.HostName = hostEntry.HostName;
            connectionFactory.VirtualHost = "/";
            connectionFactory.UserName = "admin";
            connectionFactory.Password = "admin";
            ConnectionFactory connectionFactory2 = connectionFactory;
            string text = string.Empty;
            string result;
            using (IConnection connection = connectionFactory2.CreateConnection())
            {
                using (IModel model = connection.CreateModel())
                {
                    QueueingBasicConsumer queueingBasicConsumer = new QueueingBasicConsumer(model);
                    model.BasicConsume(aQueueName, false, queueingBasicConsumer);
                    BasicDeliverEventArgs basicDeliverEventArgs = queueingBasicConsumer.Queue.Dequeue();
                    if (basicDeliverEventArgs != null)
                    {
                        try
                        {
                            byte[] body = basicDeliverEventArgs.Body;
                            text = System.Text.Encoding.UTF8.GetString(body);
                            model.BasicAck(basicDeliverEventArgs.DeliveryTag, false);
                        }
                        catch
                        {
                        }
                    }
                    result = text;
                }
            }
            return result;
        }
        public int GetNum(string queueIp, string queueName = "")
        {
            IPHostEntry hostEntry = Dns.GetHostEntry(queueIp);
            ConnectionFactory connectionFactory = new ConnectionFactory();
            connectionFactory.HostName = hostEntry.HostName;
            connectionFactory.VirtualHost = "/";
            connectionFactory.UserName = "admin";
            connectionFactory.Password = "admin";
            ConnectionFactory connectionFactory2 = connectionFactory;
            string empty = string.Empty;
            int result;
            using (IConnection connection = connectionFactory2.CreateConnection())
            {
                using (IModel model = connection.CreateModel())
                {
                    try
                    {
                        BasicGetResult basicGetResult = model.BasicGet(queueName, false);
                        if (basicGetResult != null)
                        {
                            uint messageCount = basicGetResult.MessageCount;
                            result = (int)(messageCount + 1u);
                        }
                        else
                        {
                            result = -2;
                        }
                    }
                    catch (Exception var_7_8F)
                    {
                        result = -1;
                    }
                }
            }
            return result;
        }
    }
}
