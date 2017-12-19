using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace GeoDo.RSS.RasterTools
{
    /// <summary>
    /// 状态记录器(使用位操作)
    /// </summary>
    public class StatusRecorder : IDisposable
    {
        private byte[] _buffer;
        private const byte BIT_0 = 1;  //0000 0001
        private const byte BIT_1 = 2;  //0000 0010
        private const byte BIT_2 = 4;  //0000 0100
        private const byte BIT_3 = 8;  //0000 1000
        private const byte BIT_4 = 16; //0001 0000
        private const byte BIT_5 = 32; //0010 0000
        private const byte BIT_6 = 64; //0100 0000
        private const byte BIT_7 = 128;//1000 0000
        private byte[] BITS_OPERATORS = new byte[] { BIT_0, BIT_1, BIT_2, BIT_3, BIT_4, BIT_5, BIT_6, BIT_7 };

        public StatusRecorder(int count)
        {
            int memorySize = (int)Math.Ceiling(count / (float)Marshal.SizeOf(typeof(byte)));
            _buffer = new byte[memorySize];
        }

        public void Reset()
        {
            for (int i = 0; i < _buffer.Length; i++)
                _buffer[i] = 0;
        }

        public bool IsTrue(int index)
        {
            int idx = index >> 3;    //index / 8
            int offset = index % 8;  //8 == Marshal.SizeOf(typeof(byte))
            return (_buffer[idx] & BITS_OPERATORS[offset]) == BITS_OPERATORS[offset];
        }

        public void SetStatus(int index, bool isTrue)
        {
            int idx = index >> 3;  //index / 8
            int offset = index % 8;//8 == Marshal.SizeOf(typeof(byte))
            _buffer[idx] = (byte)(_buffer[idx] ^ BITS_OPERATORS[offset]);
        }

        public void Dispose()
        {
            _buffer = null;
            GC.Collect();
        }
    }
}
