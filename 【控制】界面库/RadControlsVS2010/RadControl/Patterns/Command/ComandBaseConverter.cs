using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Telerik.WinControls.Elements;
using System.Reflection;
using System.ComponentModel.Design;
using System.Collections;
using System.Runtime.InteropServices;
using System.Globalization;

namespace Telerik.WinControls.Commands
{
	public class CommandBaseConverter : TypeConverter
	{
		protected Type type;
		protected static string none = "(none)";
		protected List<CommandBase> commands;
		protected List<IComponent> commandSources;

		#region Constructors

		public CommandBaseConverter(System.Type type)
			: base()
		{
			this.type = type;
		}

		#endregion

		#region TypeConverter overrides

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if ((sourceType == typeof(string)) && (context != null))
			{
				return true;
			}
			return base.CanConvertFrom(context, sourceType);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if ((destinationType == typeof(string)) && (context != null))
			{
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}

		protected virtual bool IsValueAllowed(ITypeDescriptorContext context, object value)
		{
			return true;
		}

		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return true;
		}

		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			return TypeDescriptor.GetProperties(value, attributes);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (!(value is string))
			{
				return base.ConvertFrom(context, culture, value);
			}
			string text1 = ((string)value).Trim();
			if (!string.Equals(text1, CommandBaseConverter.none) && (context != null))
			{
				IReferenceService service1 = (IReferenceService)context.GetService(typeof(IReferenceService));
				if (service1 != null)
				{
					object obj1 = service1.GetReference(text1);
					if (obj1 != null)
					{
						return obj1;
					}
				}
				IContainer container1 = context.Container;
				if (container1 != null)
				{
					object obj2 = container1.Components[text1];
					if (obj2 != null)
					{
						return obj2;
					}
				}
			}
			return null;
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == null)
			{
				throw new ArgumentNullException("Destination Type is not defined.");
			}
			if (destinationType != typeof(string))
			{
				return base.ConvertTo(context, culture, value, destinationType);
			}
			if (value == null)
			{
				return CommandBaseConverter.none;
			}
			if (context != null)
			{
				IReferenceService service1 = (IReferenceService)context.GetService(typeof(IReferenceService));
				if (service1 != null)
				{
					string text1 = service1.GetName(value);
					if (text1 != null)
					{
						return text1;
					}
				}
			}
			if (!Marshal.IsComObject(value) && (value is IComponent))
			{
				IComponent component1 = (IComponent)value;
				ISite site1 = component1.Site;
				if (site1 != null)
				{
					string text2 = site1.Name;
					if (text2 != null)
					{
						return text2;
					}
				}
			}
			return string.Empty;
		}

		#endregion

		public virtual TypeConverter.StandardValuesCollection GetStandardValuesCore(ITypeDescriptorContext context, TypeConverter.StandardValuesCollection collection)
		{
			return new TypeConverter.StandardValuesCollection(null);
		}

		protected virtual TypeConverter.StandardValuesCollection GetComponentsReferences(ITypeDescriptorContext context)
		{
			return GetReferences(context, typeof(IComponent));
		}

		internal static TypeConverter.StandardValuesCollection GetReferences(IServiceProvider context, Type type)
		{
			if (context != null)
			{
				ArrayList list1 = new ArrayList();
				//list1.Add(null);
				IReferenceService service1 = (IReferenceService)context.GetService(typeof(IReferenceService));
				if (service1 != null)
				{
					object[] objArray2 = service1.GetReferences(type);
					int num1 = objArray2.Length;
					for (int num2 = 0; num2 < num1; num2++)
					{
						IComponent tempComponent = objArray2[num2] as IComponent;
						if (tempComponent != null && tempComponent.Site != null && tempComponent.Site.DesignMode)
						{
							list1.Add(objArray2[num2]);
						}
					}
					object[] objArray1 = list1.ToArray();
					return new TypeConverter.StandardValuesCollection(objArray1);
				}
			}
			return null;
		}

		internal static /*protected virtual*/ TypeConverter.StandardValuesCollection GetReferences(ITypeDescriptorContext context, Type type)
		{
			object[] objArray1 = null;
			if (context != null)
			{
				ArrayList list1 = new ArrayList();
				list1.Add(null);
				IReferenceService service1 = (IReferenceService)context.GetService(typeof(IReferenceService));
				if (service1 != null)
				{
					object[] objArray2 = service1.GetReferences(type);
					int num1 = objArray2.Length;
					for (int num2 = 0; num2 < num1; num2++)
					{
						//if (this.IsValueAllowed(context, objArray2[num2]))
						{
							list1.Add(objArray2[num2]);
						}
					}
				}
				else
				{
					IContainer container1 = context.Container;
					if (container1 != null)
					{
						foreach (IComponent component1 in container1.Components)
						{
							if (((component1 != null) && type.IsInstanceOfType(component1))) //&& this.IsValueAllowed(context, component1)
							{
								list1.Add(component1);
							}
						}
					}
				}
				objArray1 = list1.ToArray();
			}
			return new TypeConverter.StandardValuesCollection(objArray1);
		}

		#region Commands and Sources Static Methods
		//scanning for RadItems
		//RadControl --> RootElement --> Children --> RootElement --> RadItemCollection Items

		public static List<IComponent> DiscoverCommandsSources(ICommand command, TypeConverter.StandardValuesCollection collection)
		{
			List<IComponent> commandSources = DiscoverCommandsSources(collection);
			List<IComponent> filteredCommandSources = new List<IComponent>(1);
			for (int i = 0; i < commandSources.Count; i++)
			{
				if ((commandSources[i].GetType()).IsAssignableFrom(command.ContextType))
				{
					filteredCommandSources.Add(commandSources[i]);
				}
			}
			if (filteredCommandSources.Count > 0)
			{
				return filteredCommandSources;
			}
			return null;
		}

		public static List<IComponent> DiscoverCommandsSources(TypeConverter.StandardValuesCollection collection)
		{
			List<IComponent> tempList = new List<IComponent>(1);
			List<IComponent> returnList = null; // new List<IComponent>(1);

			for (int i = 0; i < collection.Count; i++)
			{
				IComponent component = collection[i] as IComponent;
				if (component != null)
				{
					tempList.Add(component);
				}
			}
			returnList = DiscoverCommandsSources(tempList);
			if (returnList != null && returnList.Count > 0)
			{
				return returnList;
			}
			return null;
		}

        public static List<IComponent> DiscoverCommandsContexts(TypeConverter.StandardValuesCollection collection)
        {
            List<IComponent> tempList = new List<IComponent>(1);
            List<IComponent> returnList = new List<IComponent>(1);

            for (int i = 0; i < collection.Count; i++)
            {
                IComponent component = collection[i] as IComponent;
                if (component != null)
                {
                    tempList.Add(component);
                }
            }
            returnList = DiscoverCommandsContexts(tempList);
            if (returnList != null && returnList.Count > 0)
            {
                return returnList;
            }
            return null;
        }

        public static List<IComponent> DiscoverCommandsContexts(List<IComponent> list)
        {
            List<IComponent> returnList = new List<IComponent>(1);
            List<IComponent> tempList = null;

            for (int i = 0; i < list.Count; i++)
            {
                tempList = DiscoverCommandsContexts(list[i]);
                if (tempList != null && tempList.Count > 0)
                {
                    TransferListUniquePart<IComponent>(tempList, returnList);
                }
            }
            if (returnList.Count > 0)
            {
                return returnList;
            }
            return null;
        }

        public static List<IComponent> DiscoverCommandsContexts(IComponent source)
        {
            if (source == null)
            {
                return null;
            }
            List<IComponent> returnList = new List<IComponent>(1);
            List<IComponent> tempList = null;

            if (typeof(RadControl).IsAssignableFrom(source.GetType()) ||
                typeof(RadItem).IsAssignableFrom(source.GetType()))
            {
                TransferListUniquePart<IComponent>(source, returnList);
            }
            //testing instance implementation of Telerik speciffic interfaces
            if (source is RadControl)
            {
                RootRadElement rootElement = (source as RadControl).RootElement;
                if (rootElement != null &&
                    (rootElement.Children.Count > 0))
                {
                    foreach (RadElement element in rootElement.Children) //RadElement 
                    {
                        IComponent componentElement = element as IComponent;
                        if (componentElement != null && element is IItemsOwner)
                        {
                            tempList = DiscoverCommandsContexts(componentElement);
                            if (returnList != null)
                            {
                                TransferListUniquePart<IComponent>(tempList, returnList);
                            }
                        }
                    }
                }
            }
            if (returnList.Count > 0)
            {
                return returnList;
            }
            return null;
        }

		public static List<IComponent> DiscoverCommandsSources(List<IComponent> list)
		{
			List<IComponent> returnList = new List<IComponent>(1);
			List<IComponent> tempList = null;

			for (int i = 0; i < list.Count; i++)
			{
				tempList = DiscoverCommandsSources(list[i]);
				if (tempList != null && tempList.Count > 0)
				{
					TransferListUniquePart<IComponent>(tempList, returnList);
				}
			}
			if (returnList.Count > 0)
			{
				return returnList;
			}
			return null;
		}

		public static List<IComponent> DiscoverCommandsSources(IComponent source)
		{
			if (source == null)
			{
				return null;
			}
			List<IComponent> returnList = new List<IComponent>(1);
			List<IComponent> tempList = null;

			if (DiscoverCommands(source) != null)
			{
				TransferListUniquePart<IComponent>(source, returnList);
			}
			//testing instance implementation of Telerik speciffic interfaces
			if (source is RadControl)
			{
				RootRadElement rootElement = (source as RadControl).RootElement;
				if (rootElement != null &&
					(rootElement.Children.Count > 0))
				{
                    for (int i = 0; i < rootElement.Children.Count; i++)
                    {
                        IComponent element = rootElement.Children[i] as IComponent;
                        if (element != null && element is IItemsOwner)
                        {
                            tempList = DiscoverCommandsSources(element);
                            if (returnList != null)
                            {
                                TransferListUniquePart<IComponent>(tempList, returnList);
                            }
                        }
                    }
				}
			}
			if (returnList.Count > 0)
			{
				return returnList;
			}
			return null;
		}

		protected static void TransferListUniquePart<T>(List<T> sourceList, List<T> destinationList)
		{
			for (int i = 0; i < sourceList.Count; i++)
			{
				TransferListUniquePart<T>(sourceList[i], destinationList);
			}
		}

		protected static void TransferListUniquePart<T>(T sourceItem, List<T> destinationList)
		{
			if (!destinationList.Contains(sourceItem))
			{
				destinationList.Add(sourceItem);
			}
		}

		public static List<CommandBase> DiscoverCommands(TypeConverter.StandardValuesCollection collection)
		{
			List<IComponent> commandSources = DiscoverCommandsSources(collection);
			List<CommandBase> commandsList = DiscoverCommands(commandSources);
			if (commandsList != null && commandsList.Count > 0)
			{
				return commandsList;
			}
			return null;
		}

		public static List<CommandBase> DiscoverCommands(List<IComponent> list)
		{
			List<CommandBase> returnList = new List<CommandBase>(1);
			List<CommandBase> tempList = null;

			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					tempList = DiscoverCommands(list[i]);
					if (tempList != null && tempList.Count > 0)
					{
						TransferListUniquePart<CommandBase>(tempList, returnList);
					}
				}
			}
			if (returnList.Count > 0)
			{
				return returnList;
			}
			return null;
		}

		public static List<CommandBase> DiscoverCommands(IComponent source)
		{
			if (source == null)
			{
				return null;
			}
			List<CommandBase> returnList = new List<CommandBase>();
			Type type = source.GetType();
			FieldInfo[] members = type.GetFields(BindingFlags.Public | BindingFlags.Static |
												 BindingFlags.GetField | BindingFlags.FlattenHierarchy);
			foreach (FieldInfo command in members)
			{
				CommandBase realCommand = (command.GetValue(source) as CommandBase);
				if (realCommand != null)
				{
					returnList.Add(realCommand);
				}
			}
			if (returnList.Count > 0)
			{
				return returnList;
			}
			return null;
		}

		#endregion
	}
}