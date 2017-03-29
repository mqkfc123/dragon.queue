using DQueue.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DQueue
{
    public interface IMessageQueue
    {
        event ReceiveEventHandler onReceive;
        bool IsReceOver
        {
            get;
            set;
        }
        void SendMQMessage(string msgText);
        void ReceiveMQMessage();
        int GetCurrentCount();
        void Init();
    }
}
