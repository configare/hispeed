using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.Data.Expressions
{
    public class ExpressionItem
    {
        private string name;
        private string value;
        private string description;
        private string syntax;
        private ExpressionItemType type;

        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        public string Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        public string Description
        {
            get { return this.description; }
            set { this.description = value; }
        }

        public string Syntax
        {
            get { return this.syntax; }
            set { this.syntax = value; }
        }

        public ExpressionItemType Type
        {
            get { return this.type; }
            set { this.type = value; }
        }

        public override string ToString()
        {
            string text = String.Format("ExpressionItem: {0}", this.syntax);
            return text;
        }

    }
}
