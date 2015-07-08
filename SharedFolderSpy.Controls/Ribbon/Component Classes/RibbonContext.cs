#region

using System.ComponentModel;
using System.Drawing;
using Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.Collections;

#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.Ribbon
{
    /// <summary>
    ///     Represents a context on the Ribbon
    /// </summary>
    /// <remarks>
    ///     Contexts are useful when some tabs are volatile, depending on some selection. A RibbonTabContext can be added
    ///     to the ribbon by calling Ribbon.Contexts.Add
    /// </remarks>
    [ToolboxItem(false)]
    public class RibbonContext
        : Component
    {
        private readonly RibbonTabCollection _tabs;
        private Color _glowColor;
        private Ribbon _owner;

        /// <summary>
        ///     Creates a new RibbonTabContext
        /// </summary>
        /// <param name="Ribbon">Ribbon that owns the context</param>
        public RibbonContext(Ribbon owner)
        {
            _tabs = new RibbonTabCollection(owner);
        }

        /// <summary>
        ///     Gets or sets the text of the Context
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        ///     Gets or sets the color of the glow that indicates a context
        /// </summary>
        public Color GlowColor
        {
            get { return _glowColor; }
            set { _glowColor = value; }
        }

        /// <summary>
        ///     Gets the Ribbon that owns this context
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Ribbon Owner
        {
            get { return _owner; }
        }

        public RibbonTabCollection Tabs
        {
            get { return _tabs; }
        }

        /// <summary>
        ///     Sets the value of the Owner Property
        /// </summary>
        internal void SetOwner(Ribbon owner)
        {
            _owner = owner;
            _tabs.SetOwner(owner);
        }
    }
}