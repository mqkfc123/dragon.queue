using DQueue;
using DQueue.Event;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DQueueTest
{
    public class ActitvmqClient
    {
        private static Dictionary<string, IMessageQueue> LstMqs = new Dictionary<string, IMessageQueue>();
        private static readonly object Obj = new object();
        /// <summary>
        /// 实例化队列对象
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns></returns>
        private static IMessageQueue CreateInstance(string queueName)
        {
            IMessageQueue iQueue = null;
            if (!LstMqs.ContainsKey(queueName))
            {
                lock (Obj)
                {
                    if (LstMqs.ContainsKey(queueName))
                    {
                        iQueue = LstMqs[queueName];
                        return iQueue;
                    }
                    var mq = (Activemq)MQFactory.CreateMessageQueue(MQFactory.MQType.ActiveMQ);
                    mq.QueueIP = "tcp://localhost:61616";//ConfigurationManager.AppSettings["QueueUrl"]; ;
                    mq.QueueName = queueName;
                    mq.DType = Activemq.DataType.Text;
                    mq.AMQType = Activemq.ActiveMQType.Queue;

                    mq.UserName = "";
                    mq.Password = "";

                    mq.Init();
                    iQueue = mq;
                    if (!LstMqs.ContainsKey(queueName))
                    {
                        LstMqs.Add(queueName, iQueue);
                    }
                }
            }
            iQueue = LstMqs[queueName];
            return iQueue;
        }


        /// <summary>
        /// 放入队列
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="queueName"></param>
        public static void InsertQueue(Dictionary<string, object> dic, string queueName)
        {
            InsertQueue(JsonConvert.SerializeObject(dic), queueName);
        }

        /// <summary>
        /// 放入队列
        /// </summary>
        /// <param name="dicString"></param>
        /// <param name="queueName"></param>
        public static void InsertQueue(string dicString, string queueName)
        {
            try
            {
                CreateInstance(queueName).SendMQMessage(dicString);
            }
            catch (Exception ex)
            {
            }
        }

    }

}
