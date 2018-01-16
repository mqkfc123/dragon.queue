using DQueue;
using DQueue.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DQueueService
{
    class RabbitMQService
    {

        public void SendMessActitvMQ()
        {
            try
            {
                using (var rabRead = (Rabbitmq)MQFactory.CreateMessageQueue(MQFactory.MQType.RabbitMQ))
                {
                    rabRead.QueueIP = "106.15.180.98";// ConfigurationManager.AppSettings["QueueUrl"];
                    rabRead.QueueName = "Surevy_Reward_Queue";
                    rabRead.VirtualHost = "15672"; 
                    rabRead.ExchangeName = "SurevyExchangeName";
                    rabRead.UserName = "zxsj";
                    rabRead.Password = "zxsj";
                    rabRead.AutoAck = false;
                    rabRead.RType = Rabbitmq.TypeName.Direct;
                    rabRead.onReceive += imq_onReceive;

                    rabRead.Init();
                    rabRead.SubscribeQueue();
                    rabRead.AddListening();
                    rabRead.ReceiveMQMessage();
                    //  结束时使用
                    rabRead.IsReceOver = true;

                }
            }
            catch (Exception ex)
            {
            }

        }

        private void imq_onReceive(object src, ReceiveEventArgs e)
        {
            var mm = e.MessageObj;

            Console.WriteLine($"rabbitMQ:{mm}");
            Thread.Sleep(2000);

        }
    }
}
