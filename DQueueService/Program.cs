using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DQueueService
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("start");
            ActitvmqService activemq = new ActitvmqService();
            activemq.SendMessActitvMQ();
            Console.ReadLine();
        }
    }
}
