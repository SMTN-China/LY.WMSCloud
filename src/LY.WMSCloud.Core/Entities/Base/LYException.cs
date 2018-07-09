using Abp.Logging;
using Abp.UI;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace LY.WMSCloud
{
    /// <summary>
    /// 立宇的友好错误信息,用于打断工作单元的数据保存和返回客户端错误信息。
    /// </summary>
    public class LYException : UserFriendlyException
    {
        public LYException(object ex) : base(Newtonsoft.Json.JsonConvert.SerializeObject(ex)) { }
        public LYException() : base() { }
        public LYException(string message) : base(message) { }
        public LYException(SerializationInfo serializationInfo, StreamingContext context) : base(serializationInfo, context) { }
        public LYException(string message, LogSeverity severity) : base(message, severity) { }
        public LYException(int code, string message) : base(code, message) { }
        public LYException(string message, string details) : base(message, details) { }
        public LYException(string message, Exception innerException) : base(message, innerException) { }
        public LYException(int code, string message, string details) : base(code, message, details) { }
        public LYException(string message, string details, Exception innerException) : base(message, details, innerException) { }
    }
}
