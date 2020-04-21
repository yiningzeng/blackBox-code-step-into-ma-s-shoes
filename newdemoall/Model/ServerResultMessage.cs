using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceServer
{
    public class ServerResultMessage
    {
        public CommandType CommandType { get; set; }
        public ResultCode ResultCode { get; set; }
        public Object Data { get; set; }
        public string messageId { get; set; }
    }

    public enum ResultCode
    {
        /// <summary>
        /// 任意值
        /// </summary>
        None = 0,
        /// <summary>
        /// 成功
        /// </summary>
        Success = 1,
        /// <summary>
        /// 失败
        /// </summary>
        fail = 2,
    }

    public enum CommandType
    {
        /// <summary>
        /// 任意值
        /// </summary>
        None = 0,
        /// <summary>
        /// 打开设备
        /// </summary>
        Pressure = 1,
        /// <summary>
        /// 打开设备
        /// </summary>
        Open = 2
    }
}
