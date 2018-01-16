using DQueue.Event;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.MessagePatterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DQueue
{
    public class Rabbitmq : IDisposable, IMessageQueue
    {
        public enum TypeName
        {
            Fanout,
            Direct,
            Topic,
            Headers
        }

        private ConnectionFactory _connectionFactory = null; 
        private IConnection _connection = null;
        private IModel _channel = null;
        //事件触发
        private EventingBasicConsumer _eventCnsumer = null;
        public event ReceiveEventHandler onReceive;

        public string ExchangeName { get; set; }
        public TypeName RType { get; set; }
        public string RoutingKey { get; set; }
        public string VirtualHost { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string QueueIP { get; set; }
        public string QueueName { get; set; }
        public bool AutoAck { get; set; }
        public bool IsReceOver { get; set; }
        public int SleepInterval { get; set; }

        public Rabbitmq()
        {
            ExchangeName = "RabittMQ_Exchange";  //交换机
            RoutingKey = "RabittMQ";
            RType = TypeName.Topic;
            VirtualHost = "/";
            UserName = "quset";
            Password = "quset";
            QueueIP = "192.168.1.120";
            QueueName = "xxx";
            AutoAck = false;
            IsReceOver = false;
            SleepInterval = 50;
        }

        /// <summary>
        /// 监听
        /// </summary>  p 
        public void AddListening()
        {
            if (onReceive != null)
            {
                string empty = string.Empty;
                //BasicDeliverEventArgs messageObj = this.m_Consumer.Queue.Dequeue(); 
                _eventCnsumer.Received += (sender, e) =>
                { 
                    var msg = Encoding.UTF8.GetString(e.Body);
                    this.onReceive(this, new ReceiveEventArgs(msg, this._channel));
                    if (!this.AutoAck)
                    {
                        this._channel.BasicAck(e.DeliveryTag, true);
                    } 
                };
            }
        }

        /// <summary>
        /// 订阅队列
        /// </summary>
        public void SubscribeQueue()
        {
            //EventingBasicConsumer
            this._eventCnsumer = new EventingBasicConsumer(this._channel);
            //指定消费队列
            this._channel.BasicConsume(this.QueueName, this.AutoAck, this._eventCnsumer);
        }

        [Obsolete("测试")]
        public void SendMQMessage(string msgText)
        {
            IBasicProperties basicProperties = this._channel.CreateBasicProperties();
            basicProperties.DeliveryMode = 2;
            this._channel.BasicPublish(this.ExchangeName, this.RoutingKey, basicProperties, System.Text.Encoding.UTF8.GetBytes(msgText));
        }

        /// <summary>
        /// 接受消息
        /// </summary>
        public void ReceiveMQMessage()
        {
            try
            { 
                while (!this.IsReceOver)
                {
                    //this.AddListening();
                    System.Threading.Thread.Sleep(this.SleepInterval);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int GetCurrentCount()
        {
            int result;
            try
            {
                BasicGetResult basicGetResult = this._channel.BasicGet(this.QueueName, false);
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
            catch (Exception ex)
            {
                result = -1;
            }
            return result;
        }

        /// <summary>
        /// 初始化队列
        /// </summary>
        public void Init()
        {
            //IPHostEntry hostEntry = Dns.GetHostEntry(this.QueueIP);
            _connectionFactory = new ConnectionFactory();
            _connectionFactory.HostName = this.QueueIP;
            //_connectionFactory.Port = Convert.ToInt32(this.VirtualHost);
            //m_ConnectionFactory.VirtualHost = this.VirtualHost; 
            _connectionFactory.UserName = this.UserName;
            _connectionFactory.Password = this.Password; 
            this._connection = this._connectionFactory.CreateConnection();
            this._channel = this._connection.CreateModel();
            //申明交换机 
            this._channel.ExchangeDeclare(this.ExchangeName, this.RType.ToString().ToLower(), true, false, null);
            //申明队列
            this._channel.QueueDeclare(this.QueueName, true, false, false, null);
            //绑定交换机
            this._channel.QueueBind(this.QueueName, this.ExchangeName, this.RoutingKey, null);
        }

        public void Dispose()
        {
            if (null != this._channel)
            {
                this._channel.Close();
            }
            if (null != this._connection)
            {
                this._connection.Close();
            }
            this._connectionFactory = null;
        }


    }
}
