#region

using System;
using System.Drawing;
using Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.EventArgs;

#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.Interfaces
{
    /// <summary>
    ///     Interface that every drawable ribbon element must implement
    /// </summary>
    public interface IRibbonElement
    {
        /// <summary>
        ///     Gets the bounds of the item
        /// </summary>
        Rectangle Bounds { get; }

        /// <summary>
        ///     Gets the Ribbon owner of this item.
        /// </summary>
        Ribbon Owner { get; }

        /// <summary>
        ///     Gets the size in pixels needed for the element in the specified mode
        /// </summary>
        /// <param name="sender">Object that sends the measure message</param>
        /// <param name="e">Event data</param>
        Size MeasureSize(object sender, RibbonElementMeasureSizeEventArgs e);

        /// <summary>
        ///     Called on every element when its time to draw itself
        /// </summary>
        /// <param name="g">Device to draw</param>
        /// <param name="sender">Object that is invoking the paint element</param>
        /// <param name="e">Paint event data</param>
        void OnPaint(Object sender, RibbonElementPaintEventArgs e);

        /// <summary>
        ///     Called to make the element aware of its actual bounds on the control
        /// </summary>
        void SetBounds(Rectangle bounds);
    }
}