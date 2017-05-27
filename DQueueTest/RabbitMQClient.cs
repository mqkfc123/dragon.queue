using DQueue;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DQueueTest
{
    public class RabbitMQClient
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
                    var mq = (Rabbitmq)MQFactory.CreateMessageQueue(MQFactory.MQType.RabbitMQ);
                    mq.QueueIP = "192.168.1.120";//ConfigurationManager.AppSettings["QueueUrl"]; ;
                    mq.VirtualHost = "15672";
                    mq.QueueName = queueName;
                    mq.ExchangeName = "ExchangeName";
                    mq.UserName = "zxsj";
                    mq.Password = "zxsj";

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
