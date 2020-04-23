using Amib.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace newdemoall.Tools
{
    class MySmartThreadPool
    {
        static SmartThreadPool Pool = new SmartThreadPool() { MaxThreads = 5 };
        static SmartThreadPool LimitPool = new SmartThreadPool() { MaxThreads = 1 };
        public static SmartThreadPool Instance()
        {
            return Pool;
        }
        public static SmartThreadPool InstanceSmall()
        {
            return LimitPool;
        }
    }
}
