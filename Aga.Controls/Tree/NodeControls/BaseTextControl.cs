using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.ComponentModel;

namespace Aga.Controls.Tree.NodeControls
{
	public abstract class BaseTextControl : EditableControl
	{
		private static Graphics _measureGraphics = Graphics.FromImage(new Bitmap(1, 1));
		private StringFormat _format;
		private Pen _focusPen;

		#region Properties

		private Font _font = null;
		public Font Font
		{
			get
			{
				if (_font == null)
					return Control.DefaultFont;
				else
					return _font;
			}
			set
			{
				if (value == Control.DefaultFont)
					_font = null;
				else
					_font = value;
			}
		}

		protected bool ShouldSerializeFont()
		{
			return (_font != null);
		}

		private HorizontalAlignment _textAlign = HorizontalAlignment.Left;
		[DefaultValue(HorizontalAlignment.Left)]
		public HorizontalAlignment TextAlign
		{
			get { return _textAlign; }
			set { _textAlign = value; }
		}

		private StringTrimming _trimming = StringTrimming.None;
		[DefaultValue(StringTrimming.None)]
		public StringTrimming Trimming
		{
			get { return _trimming; }
			set { _trimming = value; }
		}

		#endregion

		protected BaseTextControl()
		{
			_focusPen = new Pen(Color.Black);
			_focusPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;

			_format = new StringFormat(StringFormatFlags.NoWrap | StringFormatFlags.NoClip | StringFormatFlags.FitBlackBox);
			_format.LineAlignment = StringAlignment.Center;
		}

		public override Size MeasureSize(TreeNodeAdv node)
		{
			return GetLabelSize(node);
		}

		protected Size GetLabelSize(TreeNodeAdv node)
		{
			return GetLabelSize(GetLabel(node));
		}

		protected Size GetLabelSize(string label)
		{
			SizeF s = _measureGraphics.MeasureString(label, Font);
			if (!s.IsEmpty)
				return new Size((int)s.Width, (int)s.Height);
			else
				return new Size(10, Font.Height);
		}

		public override void Draw(TreeNodeAdv node, DrawContext context)
		{
			if (context.CurrentEditorOwner == this && node == Parent.CurrentNode)
				return;

			Rectangle clipRect = context.Bounds;
			Brush text = SystemBrushes.ControlText;

			string label = GetLabel(node);
			Size s = GetLabelSize(label);
			Rectangle focusRect = new Rectangle(clipRect.X, clipRect.Y, s.Width, clipRect.Height);

			if (context.DrawSelection == DrawSelectionMode.Active)
			{
				text = SystemBrushes.HighlightText;
				context.Graphics.FillRectangle(SystemBrushes.Highlight, focusRect);
			}
			else if (context.DrawSelection == DrawSelectionMode.Inactive)
			{
				text = SystemBrushes.ControlText;
				context.Graphics.FillRectangle(SystemBrushes.InactiveBorder, focusRect);
			}
			else if (context.DrawSelection == DrawSelectionMode.FullRowSelect)
			{
				text = SystemBrushes.HighlightText;
			}

			if (!context.Enabled)
				text = SystemBrushes.GrayText;

			if (context.DrawFocus)
			{
				focusRect.Width--;
				focusRect.Height--;
				context.Graphics.DrawRectangle(Pens.Gray, focusRect);
				context.Graphics.DrawRectangle(_focusPen, focusRect);
			}
			_format.Alignment = TextHelper.TranslateAligment(TextAlign);
			_format.Trimming = Trimming;
			context.Graphics.DrawString(label, context.Font, text, clipRect, _format);
		}

		protected virtual string GetLabel(TreeNodeAdv node)
		{
			if (node.Tag != null)
			{
				if (string.IsNullOrEmpty(DataPropertyName))
					return node.Tag.ToString();
				else
				{
					object obj = GetValue(node);
					if (obj != null)
						return obj.ToString();
				}
			}
			return string.Empty;
		}

		protected virtual void SetLabel(TreeNodeAdv node, string value)
		{
			SetValue(node, value);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				_focusPen.Dispose();
				_format.Dispose();
			}
		}

	}
}
