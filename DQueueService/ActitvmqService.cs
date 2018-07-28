//using DQueue;
//using DQueue.Event;
//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace DQueueService
//{

//    public class ActitvmqService
//    {
       
//        public void SendMessActitvMQ()
//        {
//            try
//            {
//                using (var rabRead = (Activemq)MQFactory.CreateMessageQueue(MQFactory.MQType.ActiveMQ))
//                {
//                    rabRead.QueueIP = "tcp://localhost:61616";// ConfigurationManager.AppSettings["QueueUrl"];
//                    rabRead.QueueName = "liuyl_Queue";
//                    rabRead.DType = Activemq.DataType.Text;
//                    rabRead.AMQType = Activemq.ActiveMQType.Queue;

//                    rabRead.UserName = "";
//                    rabRead.Password = "";
//                    rabRead.onReceive += imq_onReceive;

//                    rabRead.Init();
//                    rabRead.AddListening();
//                    rabRead.ReceiveMQMessage();
//                    //  结束时使用
//                    rabRead.IsReceOver = true;
//                }
//            }
//            catch (Exception ex)
//            {
//            }

//        }

//        private void imq_onReceive(object src, ReceiveEventArgs e)
//        {
//            var mm = e.MessageObj;
//            Console.WriteLine($"activeMQ:{mm}");

//        }

//    }
//}
