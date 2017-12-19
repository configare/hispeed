using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Telerik.WinControls.Styles;

namespace Telerik.WinControls
{
    public sealed class ItemStateManagerFactoryRegistry
    {
        private ItemStateManagerFactoryRegistry()
        {
        }
        
        private static Type RadItemType = typeof(RadItem);
        private static Dictionary<Type, ItemStateManagerFactoryBase> stateManagersByElementThemeEffectiveType = new Dictionary<Type, ItemStateManagerFactoryBase>();

        public static void AddStateManagerFactory(ItemStateManagerFactoryBase stateManagerFactory, Type themeType)
        {
            stateManagersByElementThemeEffectiveType[themeType] = stateManagerFactory;
        }

        public static ItemStateManagerFactoryBase GetStateManagerFactory(Type themeType)
        {
            return GetStateManagerFactory(themeType, false);
        }

        public static ItemStateManagerFactoryBase GetStateManagerFactory(Type themeType, bool searchBaseTypes)
        {
            //base RadItem type may not have state manager
            if (themeType == RadItemType)
            {
                return null;
            }
            if (!RadItemType.IsAssignableFrom(themeType))
            {
                throw new ArgumentException("Only RadItem inheritors may have StateManager registered");
            }

            ItemStateManagerFactoryBase factory;
            stateManagersByElementThemeEffectiveType.TryGetValue(themeType, out factory);

            if (factory == null && searchBaseTypes)
            {
                Type breakType = typeof(RadItem);
                Type currentType = themeType.BaseType;
                while (currentType != breakType)
                {
                    stateManagersByElementThemeEffectiveType.TryGetValue(currentType, out factory);
                    if (factory != null)
                    {
                        break;
                    }

                    currentType = currentType.BaseType;
                }
            }

            return factory;
        }
    }
}
