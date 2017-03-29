using DQueue.Event;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
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
        private ConnectionFactory m_ConnectionFactory = null;
        private IConnection m_Connection = null;
        private IModel m_Channel = null;
        private QueueingBasicConsumer m_Consumer = null;
        public event ReceiveEventHandler onReceive;
        public string ExchangeName { get; set; }
        public Rabbitmq.TypeName RType { get; set; }
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
            this.ExchangeName = "xxx.bpms.ehr";
            this.RoutingKey = "EHR.EmpMessageUpdate";
            this.RType = Rabbitmq.TypeName.Topic;
            this.VirtualHost = "/";
            this.UserName = "admin";
            this.Password = "admin";
            this.QueueIP = "192.168.217.128";
            this.QueueName = "xxx.bpms.ehr";
            this.AutoAck = false;
            this.IsReceOver = false;
            this.SleepInterval = 50;
        }
        private void Consumer_Listener()
        {
            if (this.onReceive != null)
            {
                string empty = string.Empty;
                BasicDeliverEventArgs messageObj = this.m_Consumer.Queue.Dequeue();
                this.onReceive(this, new ReceiveEventArgs(messageObj, this.m_Channel));
            }
        }
        public void UseSubscribe()
        {
            this.m_Consumer = new QueueingBasicConsumer(this.m_Channel);
            this.m_Channel.BasicConsume(this.QueueName, this.AutoAck, this.m_Consumer);
        }
        public void SendMQMessage(string msgText)
        {
            IBasicProperties basicProperties = this.m_Channel.CreateBasicProperties();
            basicProperties.DeliveryMode = 2;
            this.m_Channel.BasicPublish(this.ExchangeName, this.RoutingKey, basicProperties, System.Text.Encoding.UTF8.GetBytes(msgText));
        }
        public void ReceiveMQMessage()
        {
            try
            {
                while (!this.IsReceOver)
                {
                    this.Consumer_Listener();
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
                BasicGetResult basicGetResult = this.m_Channel.BasicGet(this.QueueName, false);
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
            catch (Exception var_2_34)
            {
                result = -1;
            }
            return result;
        }
        public void Init()
        {
            IPHostEntry hostEntry = Dns.GetHostEntry(this.QueueIP);
            ConnectionFactory connectionFactory = new ConnectionFactory();
            connectionFactory.HostName = hostEntry.HostName;
            connectionFactory.VirtualHost=this.VirtualHost ;
            connectionFactory.UserName=this.UserName;
            connectionFactory.Password=this.Password;
            this.m_ConnectionFactory = connectionFactory;
            this.m_Connection = this.m_ConnectionFactory.CreateConnection();
            this.m_Channel = this.m_Connection.CreateModel();
            this.m_Channel.ExchangeDeclare(this.ExchangeName, this.RType.ToString().ToLower(), true);
            this.m_Channel.QueueDeclare(this.QueueName, true, false, false, null);
            this.m_Channel.QueueBind(this.QueueName, this.ExchangeName, this.RoutingKey, null);
        }
        public void Dispose()
        {
            if (null != this.m_Channel)
            {
                this.m_Channel.Close();
            }
            if (null != this.m_Connection)
            {
                this.m_Connection.Close();
            }
            this.m_ConnectionFactory = null;
        }
    }
}
