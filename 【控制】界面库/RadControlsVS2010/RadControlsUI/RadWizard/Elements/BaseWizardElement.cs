using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Base class for RadWizard elements.
    /// </summary>
    public class BaseWizardElement : LightVisualElement
    {
        #region Dependency properties

        public static RadProperty IsWelcomePageProperty = RadProperty.Register(
           "IsWelcomePage", typeof(bool), typeof(WizardCommandArea), new RadElementPropertyMetadata(
               false, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty IsCompletionPageProperty = RadProperty.Register(
           "IsCompletionPage", typeof(bool), typeof(WizardCommandArea), new RadElementPropertyMetadata(
               false, ElementPropertyOptions.AffectsDisplay));

        #endregion

        #region Fields

        private RadWizardElement owner;

        #endregion

        #region Initialization

        static BaseWizardElement()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new BaseWizardElementStateManagerFactory(), typeof(BaseWizardElement));
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating that the element currently refers to a WizardWelcomePage.
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Gets or sets a value indicating that the element currently refers to a WizardWelcomePage.")]
        public virtual bool IsWelcomePage
        {
            get
            {
                return (bool)this.GetValue(IsWelcomePageProperty);
            }
            set
            {
                this.SetValue(IsWelcomePageProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating that the element currently refers to a WizardCompletionPage.
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Gets or sets a value indicating that the element currently refers to a WizardCompletionPage.")]
        public virtual bool IsCompletionPage
        {
            get
            {
                return (bool)this.GetValue(IsCompletionPageProperty);
            }
            set
            {
                this.SetValue(IsCompletionPageProperty, value);
            }
        }

        
        /// <summary>
        /// Gets the owner RadWizardElement of the element.
        /// </summary>
        public RadWizardElement Owner
        {
            get { return this.owner; }
            internal set { this.owner = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates the current state of the element.
        /// </summary>
        /// <param name="page">The WizardPage the element currently refers to.</param>
        public virtual void UpdateInfo(WizardPage page)
        {
            if (page == null)
            {
                this.IsWelcomePage = false;
                this.IsCompletionPage = false;
            }

            this.IsWelcomePage = page is WizardWelcomePage;
            this.IsCompletionPage = page is WizardCompletionPage;
        }

        #endregion
    }
}