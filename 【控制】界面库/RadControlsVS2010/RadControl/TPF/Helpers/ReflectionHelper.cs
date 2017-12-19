using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Telerik.WinControls
{
    /// <summary>
    /// Encapsulates common functionality related with reflection-based operations such as Cloning, Field Copying, etc.
    /// </summary>
    public static class ReflectionHelper
    {
        /// <summary>
        /// Copies all the fields, which are not marked by the [NonSerialized] attribute and are not Delegate instances,
        /// from the source object to the target one. Reference type fields will be copied by reference rather than cloned.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="source"></param>
        public static void CopyFields<T>(T target, T source) where T : class
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            FieldInfo[] fields = target.GetType().GetFields(flags);

            Type nonSerializedAttributeType = typeof(NonSerializedAttribute);

            foreach (FieldInfo fi in fields)
            {
                //do not copy non-serialized fields
                object[] nonSerializedAttribute = fi.GetCustomAttributes(nonSerializedAttributeType, true);
                if (nonSerializedAttribute != null && nonSerializedAttribute.Length > 0)
                {
                    continue;
                }

                //do not copy delegates
                if(fi.FieldType.IsAssignableFrom(typeof(Delegate)))
                {
                    continue;
                }

                object value = fi.GetValue(source);
                fi.SetValue(target, value);
            }

            //send a notification if appropriate
            IRadCloneable cloneable = target as IRadCloneable;
            if(cloneable != null)
            {
                cloneable.OnFieldsCopied();
            }
        }

        /// <summary>
        /// Creates a new instance of type T and copies its fields from the provided source instance.
        /// Reference type fields will be copied by reference rather than cloned.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T Clone<T>(T source) where T : class, new ()
        {
            T cloned = new T();
            CopyFields<T>(cloned, source);

            //send a notification if appropriate
            IRadCloneable cloneable = cloned as IRadCloneable;
            if (cloneable != null)
            {
                cloneable.OnCloneComplete();
            }

            return cloned;
        }
    }

    /// <summary>
    /// An extended interface that supports some additional notifications sent by the ReflectionHelper.
    /// </summary>
    public interface IRadCloneable : ICloneable
    {
        /// <summary>
        /// The instance gets notified for a field copy process.
        /// </summary>
        void OnFieldsCopied();

        /// <summary>
        /// The instance gets notified for a clone complete process.
        /// </summary>
        void OnCloneComplete();
    }
}
