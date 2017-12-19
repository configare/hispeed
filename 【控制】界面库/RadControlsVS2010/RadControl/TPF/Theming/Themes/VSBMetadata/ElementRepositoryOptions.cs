using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Primitives;

namespace Telerik.WinControls.Themes
{
    public static class RepositoryItemTypes
    {
        public const string Text = "Text";
        public const string Border = "Border";
        public const string Gradient = "Gradient";
        public const string Image = "Image";
        public const string Arrow = "Arrow";
        public const string Shape = "Shape";
        public const string Layout = "Layout";
        public const string ImageShape = "ImageShape";
    }

    public class ElementRepositoryOptions
    {
        public const char RepositoryTypeDelimiter = '|';
        private string repositoryType;
        private static ElementRepositoryOptions none = new ElementRepositoryOptions(string.Empty);

		public ElementRepositoryOptions(string supportedRepositoryType)
        {
            this.repositoryType = supportedRepositoryType;
        }        

        public static ElementRepositoryOptions None
		{
			get
			{                
				return none;
			}
		}

        public string RepositoryType
        {
            get
            {
                return repositoryType;
            }
        }

        /// <summary>
        /// Returns a boolean value determining whether a
        /// repository option is supported.
        /// </summary>
        /// <param name="repositoryType">The type of repository which is check for support of an option.</param>
        /// <param name="repositoryOption">The repository option.</param>
        /// <returns>True if option supported, otherwise false.</returns>
        public static bool SupportsRepositoryOption(string repositoryType, string repositoryOption)
        {
            return repositoryType.Contains(repositoryOption);
        }

        /// <summary>
        /// Returns a boolean value determining whether
        /// the provided repository type contains one of the
        /// repository options defined in the <see cref="RepositoryItemTypes"/>
        /// enumeration.
        /// </summary>
        /// <param name="repositoryType">The type to check for standard repository option.</param>
        public static bool ContainsStandardRepositoryOption(string repositoryType)
        {
            string[] options = repositoryType.Split(ElementRepositoryOptions.RepositoryTypeDelimiter);

            foreach (string option in options)
            {
                if (option == RepositoryItemTypes.Arrow)
                    return true;
                if (option == RepositoryItemTypes.Border)
                    return true;
                if (option == RepositoryItemTypes.Text)
                    return true;
                if (option == RepositoryItemTypes.Image)
                    return true;
                if (option == RepositoryItemTypes.Gradient)
                    return true;
                if (option == RepositoryItemTypes.Layout)
                    return true;
                if (option == RepositoryItemTypes.Shape)
                    return true;
                if (option == RepositoryItemTypes.ImageShape)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Gets an integer representing the count of repository options
        /// a given repository type supports.
        /// </summary>
        /// <param name="repositoryType">The repository type to check.</param>
        ///<returns>A string array containing the options</returns>
        public static string[] GetRepositoryOptions(string repositoryType)
        {
            return repositoryType.Split(new char[] { ElementRepositoryOptions.RepositoryTypeDelimiter });
        }

        private static string GetCommonRepositoryOptions()
        {
            string options = RepositoryItemTypes.ImageShape;
            options += RepositoryTypeDelimiter + RepositoryItemTypes.Layout;
            options += RepositoryTypeDelimiter + RepositoryItemTypes.Shape;

            return options;
        }

        public static ElementRepositoryOptions GetDefaultRepositoryOpions(RadElement element)
        {
            string options = GetCommonRepositoryOptions();

            if (element is IFillElement)
            {
                options += RepositoryItemTypes.Gradient;
            }

            if (element is IBorderElement)
            {
                options += RepositoryTypeDelimiter + RepositoryItemTypes.Border;
            }

            if (element is ITextPrimitive || 
                (element is VisualElement && !(element is IBorderElement || element is IFillElement || element is ArrowPrimitive)))
            {
                options += RepositoryTypeDelimiter + RepositoryItemTypes.Text;
            }

            if (element is IImageElement)
            {
                options += RepositoryTypeDelimiter + RepositoryItemTypes.Image;
            }

            if (element is ArrowPrimitive)
            {
                options += RepositoryTypeDelimiter + RepositoryItemTypes.Arrow;
            }

            return new ElementRepositoryOptions(options);
        }
    }
}
