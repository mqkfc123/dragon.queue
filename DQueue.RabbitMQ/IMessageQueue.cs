using DQueue.RabbitMQ.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DQueue.RabbitMQ
{
    public interface IMessageQueue
    {
        bool IsReceOver
        {
            get;
            set;
        }
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="msgText"></param>
        void PublishMQMessage(string msgText);
        /// <summary>
        /// 接收消息
        /// </summary>
        void ReceiveMQMessage();
        /// <summary>
        /// 查询消息条数
        /// </summary>
        /// <returns></returns>
        int GetCurrentCount();
        /// <summary>
        /// 初始化
        /// </summary>
        void Init();
    }
}
