using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DQueue
{
    public class MQFactory
    {
        public enum MQType
        {
            MSMQ,
            ActiveMQ,
            RabbitMQ
        }
        public static IMessageQueue CreateMessageQueue(MQType aMQType)
        {
            switch (aMQType)
            {
                //case MQType.ActiveMQ:
                //    {
                //        IMessageQueue result = new Activemq();
                //        return result;
                //    }
                case MQType.RabbitMQ:
                    {
                        IMessageQueue result = new Rabbitmq();
                        return result;
                    }
            }
            return null;
        }

    }
}
