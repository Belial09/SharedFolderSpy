namespace SampleApp
{
	partial class SimpleExample
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
			this._addRoot = new System.Windows.Forms.Button();
			this._clear = new System.Windows.Forms.Button();
			this._addChild = new System.Windows.Forms.Button();
			this._deleteNode = new System.Windows.Forms.Button();
			this._tree = new Aga.Controls.Tree.TreeViewAdv();
			this._nodeCheckBox = new Aga.Controls.Tree.NodeControls.NodeCheckBox();
			this._nodeStateIcon = new Aga.Controls.Tree.NodeControls.NodeStateIcon();
			this._nodeTextBox = new Aga.Controls.Tree.NodeControls.NodeTextBox();
			this.SuspendLayout();
			// 
			// _addRoot
			// 
			this._addRoot.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._addRoot.Location = new System.Drawing.Point(362, 3);
			this._addRoot.Name = "_addRoot";
			this._addRoot.Size = new System.Drawing.Size(75, 23);
			this._addRoot.TabIndex = 1;
			this._addRoot.Text = "Add Root";
			this._addRoot.UseVisualStyleBackColor = true;
			this._addRoot.Click += new System.EventHandler(this.AddRootClick);
			// 
			// _clear
			// 
			this._clear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._clear.Location = new System.Drawing.Point(362, 90);
			this._clear.Name = "_clear";
			this._clear.Size = new System.Drawing.Size(75, 23);
			this._clear.TabIndex = 2;
			this._clear.Text = "Clear Tree";
			this._clear.UseVisualStyleBackColor = true;
			this._clear.Click += new System.EventHandler(this.ClearClick);
			// 
			// _addChild
			// 
			this._addChild.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._addChild.Location = new System.Drawing.Point(362, 32);
			this._addChild.Name = "_addChild";
			this._addChild.Size = new System.Drawing.Size(75, 23);
			this._addChild.TabIndex = 3;
			this._addChild.Text = "Add Child";
			this._addChild.UseVisualStyleBackColor = true;
			this._addChild.Click += new System.EventHandler(this.AddChildClick);
			// 
			// _deleteNode
			// 
			this._deleteNode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._deleteNode.Location = new System.Drawing.Point(362, 61);
			this._deleteNode.Name = "_deleteNode";
			this._deleteNode.Size = new System.Drawing.Size(75, 23);
			this._deleteNode.TabIndex = 5;
			this._deleteNode.Text = "Delete Node";
			this._deleteNode.UseVisualStyleBackColor = true;
			this._deleteNode.Click += new System.EventHandler(this.DeleteClick);
			// 
			// _tree
			// 
			this._tree.AllowDrop = true;
			this._tree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._tree.BackColor = System.Drawing.SystemColors.Window;
			this._tree.Cursor = System.Windows.Forms.Cursors.Default;
			this._tree.DragDropMarkColor = System.Drawing.Color.Black;
			this._tree.LineColor = System.Drawing.SystemColors.ControlDark;
			this._tree.Location = new System.Drawing.Point(0, 0);
			this._tree.Model = null;
			this._tree.Name = "_tree";
			this._tree.NodeControls.Add(this._nodeCheckBox);
			this._tree.NodeControls.Add(this._nodeStateIcon);
			this._tree.NodeControls.Add(this._nodeTextBox);
			this._tree.SelectedNode = null;
			this._tree.SelectionMode = Aga.Controls.Tree.TreeSelectionMode.MultiSameParent;
			this._tree.ShowNodeToolTips = true;
			this._tree.Size = new System.Drawing.Size(356, 324);
			this._tree.TabIndex = 0;
			this._tree.Text = "treeViewAdv1";
			this._tree.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this._tree_ItemDrag);
			this._tree.SelectionChanged += new System.EventHandler(this._tree_SelectionChanged);
			this._tree.DragDrop += new System.Windows.Forms.DragEventHandler(this._tree_DragDrop);
			this._tree.DragOver += new System.Windows.Forms.DragEventHandler(this._tree_DragOver);
			// 
			// _nodeCheckBox
			// 
			this._nodeCheckBox.DataPropertyName = "CheckState";
			this._nodeCheckBox.ThreeState = true;
			// 
			// _nodeTextBox
			// 
			this._nodeTextBox.DataPropertyName = "Text";
			this._nodeTextBox.EditEnabled = true;
			// 
			// SimpleExample
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._deleteNode);
			this.Controls.Add(this._addChild);
			this.Controls.Add(this._clear);
			this.Controls.Add(this._addRoot);
			this.Controls.Add(this._tree);
			this.Name = "SimpleExample";
			this.Size = new System.Drawing.Size(440, 324);
			this.ResumeLayout(false);

		}

		#endregion

		private Aga.Controls.Tree.TreeViewAdv _tree;
		private System.Windows.Forms.Button _addRoot;
		private System.Windows.Forms.Button _clear;
		private Aga.Controls.Tree.NodeControls.NodeCheckBox _nodeCheckBox;
		private System.Windows.Forms.Button _addChild;
		private System.Windows.Forms.Button _deleteNode;
		private Aga.Controls.Tree.NodeControls.NodeStateIcon _nodeStateIcon;
		private Aga.Controls.Tree.NodeControls.NodeTextBox _nodeTextBox;
	}
}
