namespace SampleApp
{
	partial class FolderBrowser
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			Aga.Controls.Tree.TreeColumn treeColumn1 = new Aga.Controls.Tree.TreeColumn();
			Aga.Controls.Tree.TreeColumn treeColumn2 = new Aga.Controls.Tree.TreeColumn();
			Aga.Controls.Tree.TreeColumn treeColumn3 = new Aga.Controls.Tree.TreeColumn();
			this._treeView = new Aga.Controls.Tree.TreeViewAdv();
			this._icon = new Aga.Controls.Tree.NodeControls.NodeStateIcon();
			this._name = new Aga.Controls.Tree.NodeControls.NodeTextBox();
			this._size = new Aga.Controls.Tree.NodeControls.NodeTextBox();
			this._date = new Aga.Controls.Tree.NodeControls.NodeTextBox();
			this.SuspendLayout();
			// 
			// _treeView
			// 
			this._treeView.BackColor = System.Drawing.SystemColors.Window;
			treeColumn1.Header = "Name";
			treeColumn1.Width = 250;
			treeColumn2.Header = "Size";
			treeColumn2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			treeColumn2.Width = 100;
			treeColumn3.Header = "Date";
			treeColumn3.Width = 120;
			this._treeView.Columns.Add(treeColumn1);
			this._treeView.Columns.Add(treeColumn2);
			this._treeView.Columns.Add(treeColumn3);
			this._treeView.Cursor = System.Windows.Forms.Cursors.Default;
			this._treeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this._treeView.DragDropMarkColor = System.Drawing.Color.Black;
			this._treeView.FullRowSelect = true;
			this._treeView.LineColor = System.Drawing.SystemColors.ControlDark;
			this._treeView.LoadOnDemand = true;
			this._treeView.Location = new System.Drawing.Point(0, 0);
			this._treeView.Model = null;
			this._treeView.Name = "_treeView";
			this._treeView.NodeControls.Add(this._icon);
			this._treeView.NodeControls.Add(this._name);
			this._treeView.NodeControls.Add(this._size);
			this._treeView.NodeControls.Add(this._date);
			this._treeView.SelectedNode = null;
			this._treeView.ShowNodeToolTips = true;
			this._treeView.Size = new System.Drawing.Size(533, 327);
			this._treeView.TabIndex = 0;
			this._treeView.Text = "treeViewAdv1";
			this._treeView.UseColumns = true;
			// 
			// _icon
			// 
			this._icon.DataPropertyName = "Icon";
			// 
			// _name
			// 
			this._name.DataPropertyName = "Name";
			this._name.EditEnabled = true;
			// 
			// _size
			// 
			this._size.Column = 1;
			this._size.DataPropertyName = "Size";
			this._size.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// _date
			// 
			this._date.Column = 2;
			this._date.DataPropertyName = "Date";
			// 
			// FolderBrowser
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._treeView);
			this.Name = "FolderBrowser";
			this.Size = new System.Drawing.Size(533, 327);
			this.ResumeLayout(false);

		}

		#endregion

		private Aga.Controls.Tree.TreeViewAdv _treeView;
		private Aga.Controls.Tree.NodeControls.NodeStateIcon _icon;
		private Aga.Controls.Tree.NodeControls.NodeTextBox _name;
		private Aga.Controls.Tree.NodeControls.NodeTextBox _size;
		private Aga.Controls.Tree.NodeControls.NodeTextBox _date;
	}
}
