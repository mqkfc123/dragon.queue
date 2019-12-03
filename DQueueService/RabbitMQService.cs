
using DQueue.RabbitMQ;
using DQueue.RabbitMQ.Event;
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
                var thread = new ThreadStart(() =>
                {
                    using (var rabRead = new DRabbitMQ())
                    {
                        rabRead.QueueIP = "106.15.180.98";// ConfigurationManager.AppSettings["QueueUrl"];
                        rabRead.QueueName = "QueueNameTest";
                        rabRead.VirtualHost = "15672";
                        rabRead.ExchangeName = "ExchangeNameTest";
                        rabRead.UserName = "zxsj";
                        rabRead.Password = "zxsj";
                        rabRead.AutoAck = false;
                        rabRead.RType = DRabbitMQ.TypeName.Direct;

                        rabRead.Init();
                        rabRead.SubscribeQueue((eventArgs) =>
                        {
                            imq_onReceive(eventArgs);
                        });
                        //接收的消息 加一个循环判断， 不会退出using，等需要的时候在退出 
                        rabRead.ReceiveMQMessage(); 
                    }
                });
                thread.BeginInvoke(null, null);
            }
            catch (Exception ex)
            {
            }

        }

        private void imq_onReceive(ReceiveEventArgs e)
        {
            var mm = e.MessageObj;
            Console.WriteLine($"rabbitMQ:{mm}");
        }

    }
}
