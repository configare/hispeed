using System;
using System.Collections.Generic;
using System.Text;

namespace CodeCell.Bricks.AppFramework
{
    public interface IMenu:IItem
    {
        /// <summary>
        /// IItem Ö»ÄÜÊÇICommand
        /// </summary>
        IItem[] Items { get;}
    }
}
