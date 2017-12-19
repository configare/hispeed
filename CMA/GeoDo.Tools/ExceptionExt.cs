using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.Tools
{
    public class ExceptionExt:Exception
    {
        private int _level;

        /// <summary>
        /// 1、消息
        /// 2、警告
        /// 3、错误
        /// </summary>
        public int MessageNumber
        {
            get { return _level; }
            set { _level = value; }
        }

        public ExceptionExt(int messageNumber, string message)
            : base(message)
        {
            _level = messageNumber;
        }

        public ExceptionExt(int messageNumber, string message, Exception innerException)
            : base(message, innerException)
        {
            _level = messageNumber;
        }
    }
}
