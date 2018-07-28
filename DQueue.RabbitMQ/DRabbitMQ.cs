using DQueue.RabbitMQ.Event;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DQueue.RabbitMQ
{
    public class DRabbitMQ : IDisposable, IMessageQueue
    {
        /// <summary>
        /// RabbitMQ的四种交换机
        /// </summary>
        public enum TypeName
        {
            /// <summary>
            /// 多播 一条消息分发给多个消费者
            /// </summary>
            Fanout,
            /// <summary>
            /// 轮询
            /// </summary>
            Direct, 
            /// <summary>
            /// 正则条件匹配
            /// </summary>
            Topic,
            /// <summary>
            /// 头信息
            /// </summary>
            Headers
        }
        /// <summary>
        /// 连接工厂
        /// </summary>
        private ConnectionFactory _connectionFactory = null;
        private IConnection _connection = null;
        private IModel _channel = null;
        //事件触发
        private EventingBasicConsumer _eventCnsumer = null;

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

        public DRabbitMQ()
        {
            ExchangeName = "RabittMQ_Exchange";  //交换机
            RoutingKey = "RabittMQ";
            RType = TypeName.Direct;
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
        /// 订阅队列 后执行ReceiveMQMessage
        /// </summary>
        public void SubscribeQueue()
        {
            //订阅队列类EventingBasicConsumer
            this._eventCnsumer = new EventingBasicConsumer(this._channel);
            //指定消费队列
            this._channel.BasicConsume(this.QueueName, this.AutoAck, this._eventCnsumer);
        }

        /// <summary>
        /// 接收消息
        /// </summary>
        public void ReceiveMQMessage(Action<ReceiveEventArgs> action)
        {
            try
            {
                string empty = string.Empty;
                //获取订阅的时间，拉取数据
                _eventCnsumer.Received += (sender, e) =>
                {
                    var msg = Encoding.UTF8.GetString(e.Body);
                    action?.Invoke(new ReceiveEventArgs(msg, this._channel));
                    if (!this.AutoAck)
                    {
                        this._channel.BasicAck(e.DeliveryTag, true);
                    }
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="msgText"></param>
        public void PublishMQMessage(string msgText)
        {
            IBasicProperties basicProperties = this._channel.CreateBasicProperties();
            basicProperties.DeliveryMode = 2;
            this._channel.BasicPublish(this.ExchangeName, this.RoutingKey, basicProperties, System.Text.Encoding.UTF8.GetBytes(msgText));
        }

        /// <summary>
        /// 查询消息条数
        /// </summary>
        /// <returns></returns>
        public int GetCurrentCount()
        {
            int result;
            try
            {
                //主动拉取队列消息 this._channel.BasicGet
                BasicGetResult basicGetResult = this._channel.BasicGet(this.QueueName, false);
                if (basicGetResult != null)
                {
                    //var messageCount = Encoding.UTF8.GetString(basicGetResult.Body);
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
            _connectionFactory = new ConnectionFactory();
            //ip地址
            _connectionFactory.HostName = this.QueueIP;
            //端口
            _connectionFactory.Port = Convert.ToInt32(this.VirtualHost);
            _connectionFactory.UserName = this.UserName;
            _connectionFactory.Password = this.Password;
            //通过工厂创建连接
            this._connection = this._connectionFactory.CreateConnection();
            //创建channel（通道）
            this._channel = this._connection.CreateModel();
            //申明交换机 
            this._channel.ExchangeDeclare(this.ExchangeName, this.RType.ToString().ToLower(), true, false, null);
            //申明队列
            //string queue(队列名称), 
            //bool durable(是否持久化), bool exclusive(是否排外（专有的）), bool autoDelete(是否自动删除)
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
