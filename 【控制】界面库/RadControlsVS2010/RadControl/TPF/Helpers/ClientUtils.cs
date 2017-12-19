using System;
using System.Collections;
using System.Reflection;
using System.Security;
using System.Threading;

namespace Telerik.WinControls
{   /// <exclude/>
    public static class ClientUtils
    {
        // Methods
        public static int GetBitCount(uint x)
        {
            int num1 = 0;
            while (x > 0)
            {
                x &= (x - 1);
                num1++;
            }
            return num1;
        }

        public static bool IsCriticalException(Exception ex)
        {
            if (((!(ex is NullReferenceException) 
                && !(ex is StackOverflowException)) &&
                (!(ex is OutOfMemoryException) && 
                !(ex is ThreadAbortException))) &&
                (/*!(ex is ExecutionEngineException) &&*/ //by fdc
                 !(ex is IndexOutOfRangeException)))
            {
                return (ex is AccessViolationException);
            }
            return true;
        }

        public static bool IsEnumValid(Enum enumValue, int value, int minValue, int maxValue)
        {
            return ((value >= minValue) && (value <= maxValue));
        }

        public static bool IsEnumValid(Enum enumValue, int value, int minValue, int maxValue, int maxNumberOfBitsOn)
        {
            bool flag1 = (value >= minValue) && (value <= maxValue);
            return (flag1 && (ClientUtils.GetBitCount((uint) value) <= maxNumberOfBitsOn));
        }

        public static bool IsEnumValid_Masked(Enum enumValue, int value, uint mask)
        {
            return ((value & mask) == value);
        }

        public static bool IsEnumValid_NotSequential(Enum enumValue, int value, params int[] enumValues)
        {
            for (int num1 = 0; num1 < enumValues.Length; num1++)
            {
                if (enumValues[num1] == value)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsSecurityOrCriticalException(Exception ex)
        {
            if (!(ex is SecurityException))
            {
                return ClientUtils.IsCriticalException(ex);
            }
            return true;
        }
    }
}

