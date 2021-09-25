using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

namespace AlarmLibrary
{
    /// <summary>
    /// 提示信息类,当事件还未绑定时用安全队列将历史信息存起来
    /// </summary>
    public static class NotifyG
    {
        public static event Action<string> EventHandlerNotify;
        /// <summary>
        /// 轮询方式的数据队列
        /// </summary>
        private static ConcurrentQueue<String> DataQueue = new ConcurrentQueue<String>();

        public static void Add(string msg)
        {
            //开始处理
            if (EventHandlerNotify != null)
            {
                //如果有缓存数据则 则先处理完缓存数据
                while (DataQueue.Count() > 0)
                {
                    string result;
                    DataQueue.TryDequeue(out result);
                    EventHandlerNotify(result);
                    Thread.Sleep(1);
                }
                EventHandlerNotify(String.Format("{0} {1}", DateTime.Now.ToString("HH:mm:ss.ffff"), msg));
            }
            else
            {
                DataQueue.Enqueue(String.Format("{0} {1} ", DateTime.Now.ToString("HH:mm:ss.ffff"), msg));
            }
        }
    }
}
