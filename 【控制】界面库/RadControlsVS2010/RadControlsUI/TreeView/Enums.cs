
namespace Telerik.WinControls.UI
{
    public enum DropPosition : short
    {
        None = 0,
        BeforeNode = 1,
        AfterNode = 2,
        AsChildNode = 3
    }

    public enum CheckType
    {
        None,
        CheckBox,
        RadioButton
    }

    public enum ToggleMode
    {
        None,
        DoubleClick,
        SingleClick,
    }

    public enum ExpandMode
    {
        Multiple,
        Single,
    }

    /// <summary>
    /// Defines the expanding animation style of nodes in a 
    ///     <see cref="RadTreeView">RadTreeView Class</see>.
    /// </summary>
    public enum ExpandAnimation
    {
        /// <summary>
        /// Indicates animation style changing the opacity of the expanding nodes.
        /// </summary>
        Opacity,
        /// <summary>
        /// Indicates no animation.
        /// </summary>
        None,
    }

    /// <summary>
    /// Specifies the type of option list formed by child nodes.
    /// </summary>
    public enum ChildListType
    {
        /// <summary>
        /// All children have a check box.
        /// </summary>
        CheckList,
        /// <summary>
        /// All children have a radio button.
        /// </summary>
        OptionList,
        /// <summary>
        /// Every child can specify whether it has a check box or a radio button.
        /// </summary>
        Custom
    }

    /// <summary>
    ///     Defines the style of the lines between the nodes in a
    ///     <see cref="RadTreeView">RadTreeView Class</see>.
    /// </summary>
    public enum TreeLineStyle
    {
        /// <summary>Specifies a solid line.</summary>
        Solid = 0,
        /// <summary>Specifies a line consisting of dashes.</summary>
        Dash = 1,
        /// <summary>Specifies a line consisting of dots.</summary>
        Dot = 2,
        /// <summary>Specifies a line consisting of a repeating pattern of dash-dot.</summary>
        DashDot = 3,
        /// <summary>Specifies a line consisting of a repeating pattern of dash-dot-dot.</summary>
        DashDotDot = 4
    }
}
