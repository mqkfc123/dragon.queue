using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Apache.NMS.ActiveMQ.Commands;
using DQueue.Event;

namespace DQueue
{
    public class Activemq : IDisposable, IMessageQueue
    {
        public enum DataType
        {
            Text,
            Byte
        }
        public enum ActiveMQType
        {
            Topic,
            Queue
        }
        private ConnectionFactory m_ConnectionFactory = null;
        private IConnection m_Connection = null;
        private ISession m_Session = null;
        private IDestination m_Destination = null;
        private IMessageConsumer m_MessageConsumer = null;
        public event ReceiveEventHandler onReceive;
        public string QueueIP { get; set; }

        public string QueueName { get; set; }
        public Activemq.DataType DType { get; set; }
        public Activemq.ActiveMQType AMQType { get; set; }
        public AcknowledgementMode AMQAckMode { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsReceOver { get; set; }
        public int SleepInterval { get; set; }

        public Activemq()
        {
            this.QueueIP = "tcp://localhost:61616/";
            this.QueueName = "liuyl_queue";
            this.DType = Activemq.DataType.Text;
            this.AMQType = Activemq.ActiveMQType.Queue;
            this.AMQAckMode = 0;
            this.UserName = "";
            this.Password = "";
            this.IsReceOver = false;
            this.SleepInterval = 50;
        }
        private void Consumer_Listener(IMessage message)
        {
            if (this.onReceive != null)
            {
                string messageObj;
                if (this.DType == Activemq.DataType.Text)
                {
                    ITextMessage textMessage = (ITextMessage)message;
                    messageObj = textMessage.Text;
                }
                else
                {
                    IBytesMessage bytesMessage = (IBytesMessage)message;
                    messageObj = Encoding.UTF8.GetString(bytesMessage.Content);
                }
                this.onReceive(this, new ReceiveEventArgs(messageObj, message));
            }
        }
        public void SendMQMessage(string msgText)
        {
            IMessageProducer messageProducer = this.m_Session.CreateProducer(this.m_Destination);
            ITextMessage textMessage = messageProducer.CreateTextMessage();
            textMessage.Text = msgText;

            messageProducer.Send(textMessage, MsgDeliveryMode.NonPersistent, MsgPriority.Normal, TimeSpan.MinValue);
        }
        public void ReceiveMQMessage()
        {
            try
            {
                while (!this.IsReceOver)
                {
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
            throw new NotImplementedException();
        }
        public void Init()
        {
            this.m_ConnectionFactory = new ConnectionFactory(this.QueueIP);
            if (!string.IsNullOrEmpty(this.UserName))
            {
                this.m_Connection = this.m_ConnectionFactory.CreateConnection(this.UserName, this.Password);
            }
            else
            {
                this.m_Connection = this.m_ConnectionFactory.CreateConnection();
            }
            this.m_Connection.Start();
            this.m_Session = this.m_Connection.CreateSession(this.AMQAckMode);
            if (this.AMQType == Activemq.ActiveMQType.Topic)
            {
                this.m_Destination = new ActiveMQTopic(this.QueueName);
            }
            else
            {
                this.m_Destination = new ActiveMQQueue(this.QueueName);
            }
        }
        public void AddListening()
        {
            this.m_MessageConsumer = this.m_Session.CreateConsumer(this.m_Destination);
            this.m_MessageConsumer.Listener += new MessageListener(this.Consumer_Listener);
        }
        public void Dispose()
        {
            if (null != this.m_Session)
            {
                this.m_Session.Close();
            }
            if (null != this.m_Connection)
            {
                this.m_Connection.Stop();
                this.m_Connection.Close();
            }
            this.m_ConnectionFactory = null;
        }
    }
}
