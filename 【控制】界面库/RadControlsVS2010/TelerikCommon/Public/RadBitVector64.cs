using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
    public class RadBitVector64
    {
        #region Fields

        private ulong data;

        #endregion

        #region Constructor

        public RadBitVector64(RadBitVector64 source)
        {
            this.data = source.data;
        }

        public RadBitVector64(ulong data)
        {
            this.data = data;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Determines whether the bit, corresponding to the specified key is set
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool this[ulong key]
        {
            get
            {
                return (this.data & key) == key;
            }
            set
            {
                if (value)
                {
                    this.data |= key;
                }
                else
                {
                    this.data &= ~key;
                }
            }
        }

        /// <summary>
        /// Gets the UInt64 structure holding the separate bits of the vector.
        /// </summary>
        public ulong Data
        {
            get
            {
                return this.data;
            }
        }

        /// <summary>
        /// Clears all currently set bits in this vector.
        /// </summary>
        public void Reset()
        {
            this.data = 0;
        }

        #endregion

        #region Public Overrides

        public override bool Equals(object obj)
        {
            if (!(obj is RadBitVector64))
            {
                return false;
            }

            return (RadBitVector64)obj == this;
        }

        public override int GetHashCode()
        {
            return this.data.GetHashCode();
        }

        #endregion

        #region Operators

        public static bool operator ==(RadBitVector64 vector1, RadBitVector64 vector2)
        {
            return vector1.data == vector2.data;
        }

        public static bool operator !=(RadBitVector64 vector1, RadBitVector64 vector2)
        {
            return vector1.data != vector2.data;
        }

        #endregion
    }
}
