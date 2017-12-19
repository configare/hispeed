using System;
using System.Reflection;
using System.Windows.Forms;

namespace Telerik.WinControls.Themes.Design
{
    /// <summary>
    /// Utility class for Design - Time VisualStudio.NET project management.
    /// </summary>
    internal class ProjectManagement
    {
        private DteServices _Services = null;

        #region Constructors
        private ProjectManagement(IServiceProvider provider)
        {
            this._Services = new DteServices(provider);
        }
        #endregion

        #region Static Methods
        internal static ProjectManagement GetProjectManagementInstance(IServiceProvider provider)
        {
            if (provider != null)
            {
                return new ProjectManagement(provider);
            }

            return null;
        }

        internal static string GetProjectFolder(IServiceProvider provider)
        {
            DteServices Services = new DteServices(provider);
            return Services.GetActiveProjectFullPath();
        }

        #endregion

        #region Instance Methods
        #endregion

        #region Properties
        public DteServices Services
        {
            get
            {
                return this._Services;
            }
        }
        #endregion

    }

    internal class DteServices
    {
        public Type EnvDteType = null;
        public object EnvDteInstance = null;
        public IServiceProvider ServiceProvider = null;

        public object EnvDteActiveDocument = null;
        public object EnvDteActiveProjectItem = null;
        public object EnvDteContainingProject = null;
        public object EnvDteContainingProjectProperties = null;
        public object EnvDteContainingProjectPath = null;
        public string EnvDteContainingProjectPathStr = null;
        private static readonly Guid DesignTimeEnvironmentCLSID = new Guid(0x04a72314, 0x32e9, 0x48e2, 0x9b, 0x87, 0xa6, 0x36, 0x03, 0x45, 0x4f, 0x3e);
        // Members invocation condition flags
        private const BindingFlags propFlag = BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty;
        private const BindingFlags mthdFlag = BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod;
        
        public DteServices(IServiceProvider provider)
        {
            ServiceProvider = provider;
            EnvDteType = DteServices.GetEnvDteType();
            EnvDteInstance = this.GetEnvDteInstance();
            EnvDteActiveDocument = this.GetActiveDocument();
            EnvDteActiveProjectItem = this.GetActiveProjectItem();
            EnvDteContainingProject = this.GetActiveContainingProject();
            EnvDteContainingProjectProperties = this.GetActiveProjectProperties();
            EnvDteContainingProjectPath = this.GetActivePathAddIn();
            EnvDteContainingProjectPathStr = this.GetActiveProjectFullPath();
        }

        #region Public Instance methods
        public object GetEnvDteInstance()
        {
            object envDte = ServiceProvider.GetService(this.EnvDteType);
            return envDte;
        }
        public object GetActiveDocument()
        {
            object activeDoc = null;
            if (this.EnvDteInstance != null)
            {
                activeDoc = this.EnvDteType.InvokeMember("ActiveDocument",
                    propFlag, null, this.EnvDteInstance, new Object[] { });
            }
            return activeDoc;
        }

        public string GetActiveDocumentPath()
        {
            string res = null;
            object doc = this.GetActiveDocument();
            if (doc != null)
            {
                res = (string)GetPropertyValue("Path", doc);
            }

            return res;
        }

        public object GetActiveProjectItem()
        {
            object prjItem = null;

            Type projectItemType = EnvDteInstance.GetType().Assembly.GetType("EnvDTE.ProjectItem");

            if (projectItemType == null)
            {
                //fix for VS 2008!!!
                if (EnvDteActiveDocument != null)
                {
                    prjItem =
                        EnvDteActiveDocument.GetType().InvokeMember("ProjectItem", propFlag, null,
                                                                    EnvDteActiveDocument, new Object[] {});
                }
            }
            else
            {
                prjItem = ServiceProvider.GetService(projectItemType);
            }

            if (prjItem == null)
            {
               // throw new InvalidOperationException("Project item is null.");
            }
            //prjItem = EnvDteActiveDocument.GetType().InvokeMember("ProjectItem",
            //    propFlag, null, EnvDteActiveDocument, new Object[] { });
            return prjItem;
        }

        public object GetActiveContainingProject()
        {
            object ContainingProject = null;
            if (EnvDteActiveProjectItem != null)
            {
                ContainingProject = EnvDteActiveProjectItem.GetType().InvokeMember("ContainingProject",
                    propFlag, null, EnvDteActiveProjectItem, new Object[] { });
            }            
            return ContainingProject;
        }
        public object GetActiveProjectProperties()
        {
            object ProjectProperties = null;
            if (EnvDteContainingProject != null)
            {
                ProjectProperties = EnvDteContainingProject.GetType().InvokeMember("Properties",
                    propFlag, null, EnvDteContainingProject, new Object[] { });
            }
            return ProjectProperties;
        }
        public object GetActivePathAddIn()
        {
            object ProjectPathAddIn = null;
            if (EnvDteContainingProjectProperties != null)
            {
                ProjectPathAddIn = EnvDteContainingProjectProperties.GetType().InvokeMember("Item",
                    mthdFlag, null, EnvDteContainingProjectProperties, new Object[] { "FullPath" });
            }
            return ProjectPathAddIn;
        }
        public object GetProjectPropertyByName(string propertyName)
        {
            object ProjectProperty = null;
            if (EnvDteContainingProjectProperties != null)
            {
                ProjectProperty = EnvDteContainingProjectProperties.GetType().InvokeMember("Item",
                    mthdFlag, null, EnvDteContainingProjectProperties, new Object[] { propertyName });
            }
            return ProjectProperty;
        }

        public object GetProjectPropertyValue(string propertyName)
        {
            object propertyInstance = this.GetProjectPropertyByName(propertyName);
            object PropertyValue = null;
            if (propertyInstance != null)
            {
                PropertyValue = propertyInstance.GetType().InvokeMember("Value",
                    propFlag, null, propertyInstance, new Object[] { });
            }
            return PropertyValue;
        }
        public object GetProjectPropertyValue(object propertyInstance)
        {
            object PropertyValue = null;
            if (propertyInstance != null)
            {
                PropertyValue = propertyInstance.GetType().InvokeMember("Value",
                    propFlag, null, propertyInstance, new Object[] { });
            }
            return PropertyValue;
        }
        public string GetActiveProjectFullPath()
        {
            object FullPath = null;
            if (EnvDteContainingProjectPath != null)
            {
                FullPath = EnvDteContainingProjectPath.GetType().InvokeMember("Value",
                    propFlag, null, EnvDteContainingProjectPath, new Object[] { });
            }
            return FullPath as string;
        }

        public string GetActiveProjectFullName()
        {
            object FullName = null;
            if (EnvDteContainingProject != null)
            {
                FullName = EnvDteContainingProject.GetType().InvokeMember("FullName",
                    propFlag, null, EnvDteContainingProject, new Object[] { });
            }
            return FullName as string;
        }

        public object GetProjectConfigurationPropertyValue(string propertyName)
        {
            object resValue = null;
            if (EnvDteContainingProject != null)
            {
                object confManager = GetPropertyValue("ConfigurationManager", EnvDteContainingProject);

                if (confManager == null)
                    return null;

                object conf = GetPropertyValue("ActiveConfiguration", confManager);

                if (conf == null)
                    return null;

                object confProperties = GetPropertyValue("Properties", conf);

                if (confProperties == null)
                    return null;

                object confProperty = InvokeMethod("Item", confProperties, propertyName);

                if (confProperty == null)
                    return null;

                resValue = GetPropertyValue("Value", confProperty);
            }

            return resValue;
        }

        private static object GetPropertyValue(string memberName, object targetObject, params object[] invokationParams)
        {
            try
            {
                return targetObject.GetType().InvokeMember(memberName,
                                                           propFlag, null,
                                                           targetObject,
                                                           invokationParams);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting value of property property " + memberName + " from object " + targetObject.ToString() + ": " + ex.ToString());
            }

            return null;
        }

        private static object InvokeMethod(string memberName, object targetObject, params object[] invokationParams)
        {
            try
            {
                return targetObject.GetType().InvokeMember(memberName,
                                                           mthdFlag, null,
                                                           targetObject,
                                                           invokationParams);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error invoking method " + memberName + " from object " + targetObject.ToString() + ": " + ex.ToString());
            }

            return null;
        }

        #endregion

        #region Public Static methods
        public static Type GetEnvDteType()
        {
            Guid envDteGuid = DesignTimeEnvironmentCLSID; // new Guid( "04a72314-32e9-48e2-9b87-a63603454f3e");
            Type envType = Type.GetTypeFromCLSID(envDteGuid, false);
            return envType;
        }

        public static object GetEnvDteInstance(IServiceProvider provider)
        {
            object envDte = provider.GetService(DteServices.GetEnvDteType());
            return envDte;
        }

        public static object GetActiveDocument(object envdteInstance)
        {
            object activeDoc = null;
            if (envdteInstance != null)
            {
                activeDoc = DteServices.GetEnvDteType().InvokeMember("ActiveDocument",
                    propFlag, null, envdteInstance, new Object[] { });
            }
            return activeDoc;
        }

        public static object GetActiveProjectItem(object activeDoc)
        {
            object prjItem = null;
            if (activeDoc != null)
            {
                prjItem = activeDoc.GetType().InvokeMember("ProjectItem",
                    propFlag, null, activeDoc, new Object[] { });
            }
            return prjItem;
        }

        public static object GetActiveContainingProject(object projectItem)
        {
            object ContainingProject = null;
            if (projectItem != null)
            {
                ContainingProject = projectItem.GetType().InvokeMember("ContainingProject",
                    propFlag, null, projectItem, new Object[] { });
            }
            return ContainingProject;
        }

        public static object GetActiveProjectProperties(object containingProject)
        {
            object ProjectProperties = null;
            if (containingProject != null)
            {
                ProjectProperties = containingProject.GetType().InvokeMember("Properties",
                    propFlag, null, containingProject, new Object[] { });
            }
            return ProjectProperties;
        }

        public static object GetActivePathAddIn(object containingProjectProperties)
        {
            object ProjectPathAddIn = null;
            if (containingProjectProperties != null)
            {
                ProjectPathAddIn = containingProjectProperties.GetType().InvokeMember("Item",
                    mthdFlag, null, containingProjectProperties, new Object[] { "FullPath" });
            }
            return ProjectPathAddIn;
        }

        public static string GetActiveProjectFullPath(object addinItem)
        {
            object FullPath = null;
            if (addinItem != null)
            {
                FullPath = addinItem.GetType().InvokeMember("Value",
                    propFlag, null, addinItem, new Object[] { });
            }
            return FullPath as string;
        }

        public static string GetActiveProjectFullName(object containingProject)
        {
            object FullName = null;
            if (containingProject != null)
            {
                FullName = containingProject.GetType().InvokeMember("FullName",
                    propFlag, null, containingProject, new Object[] { });
            }
            return FullName as string;
        }
        #endregion
    }
}
