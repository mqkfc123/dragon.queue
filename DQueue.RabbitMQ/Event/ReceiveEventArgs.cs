using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DQueue.RabbitMQ.Event
{
    public class ReceiveEventArgs : EventArgs
    {
        public object MessageObj { get; set; }
        public object AssistObj { get; set; }
        public ReceiveEventArgs(object messageObj, object assistObj)
        {
            this.MessageObj = messageObj;
            this.AssistObj = assistObj;
        }
    }
}
