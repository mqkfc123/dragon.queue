using DQueue;
using DQueue.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DQueueTest
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Write("请输入: ");
                var mesStr = Console.ReadLine();
                if (!string.IsNullOrEmpty(mesStr))
                {
                    //ActitvmqClient.InsertQueue(mesStr, "liuyl_Queue");
                    RabbitMQClient.InsertQueue(mesStr, "liuyl_Queue");
                }
            }
            //RabbitMQ
        }
    }

    
}
