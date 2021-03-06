﻿using DQueue.RabbitMQ;
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
                    var mq = new DRabbitMQ();
                    mq.QueueIP = "106.15.180.98";//ConfigurationManager.AppSettings["QueueUrl"]; ;
                    mq.VirtualHost = "15672";
                    mq.QueueName = queueName;
                    mq.ExchangeName = "SurevyExchangeName";
                    mq.AutoAck = false;
                    mq.UserName = "zxsj";
                    mq.Password = "zxsj";
                    mq.RType = DRabbitMQ.TypeName.Direct;

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
                CreateInstance(queueName).PublishMQMessage(dicString);
            }
            catch (Exception ex)
            {
            }
        }

    }
}
