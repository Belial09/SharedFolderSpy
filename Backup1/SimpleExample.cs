using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Aga.Controls.Tree;

namespace SampleApp
{
	public partial class SimpleExample : UserControl
	{
		private class ToolTipProvider : IToolTipProvider
		{
			public string GetToolTip(TreeNodeAdv node)
			{
				return "You can Drag&Drop nodes to move them";
			}
		}

		private TreeModel _model;

		public SimpleExample()
		{
			InitializeComponent();
			_nodeTextBox.ToolTipProvider = new ToolTipProvider();
			_model = new TreeModel();
			_tree.Model = _model;
			ChangeButtons();
		}

		private void ClearClick(object sender, EventArgs e)
		{
			_model.Nodes.Clear();
		}

		private void AddRootClick(object sender, EventArgs e)
		{
			Node node = new Node("root" + _model.Nodes.Count.ToString());
			_model.Nodes.Add(node);
			_tree.SelectedNode = _tree.FindNode(_model.GetPath(node));
		}

		private void AddChildClick(object sender, EventArgs e)
		{
			if (_tree.SelectedNode != null)
			{
				Node parent = _tree.SelectedNode.Tag as Node;
				Node node = new Node("child" + parent.Nodes.Count.ToString());
				parent.Nodes.Add(node);
				_tree.SelectedNode.IsExpanded = true;
			}
		}

		private void DeleteClick(object sender, EventArgs e)
		{
			if (_tree.SelectedNode != null)
				(_tree.SelectedNode.Tag as Node).Parent = null;
		}

		private void _tree_SelectionChanged(object sender, EventArgs e)
		{
			ChangeButtons();
		}

		private void ChangeButtons()
		{
			_addChild.Enabled = _deleteNode.Enabled = (_tree.SelectedNode != null);
		}

		private void _tree_ItemDrag(object sender, ItemDragEventArgs e)
		{
			TreeNodeAdv[] nodes  = new TreeNodeAdv[_tree.SelectedNodes.Count];
			_tree.SelectedNodes.CopyTo(nodes, 0);
			DoDragDrop(nodes, DragDropEffects.Move);
		}

		private void _tree_DragOver(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(typeof(TreeNodeAdv[])) && _tree.DropPosition.Node != null)
				e.Effect = e.AllowedEffect;
			else
				e.Effect = DragDropEffects.None;
		}

		private void _tree_DragDrop(object sender, DragEventArgs e)
		{
			TreeNodeAdv[] nodes = (TreeNodeAdv[])e.Data.GetData(typeof(TreeNodeAdv[]));
			Node dropNode = _tree.DropPosition.Node.Tag as Node;
			if (_tree.DropPosition.Position == NodePosition.Inside)
			{
				foreach (TreeNodeAdv n in nodes)
				{
					(n.Tag as Node).Parent = dropNode;
				}
				_tree.DropPosition.Node.IsExpanded = true;
			}
			else
			{
				Node parent = dropNode.Parent;
				Node nextItem = dropNode;
				if (_tree.DropPosition.Position == NodePosition.After)
					nextItem = dropNode.NextNode;

				foreach(TreeNodeAdv node in nodes)
					(node.Tag as Node).Parent = null;

				int index = -1;
				index = parent.Nodes.IndexOf(nextItem);
				foreach (TreeNodeAdv node in nodes)
				{
					Node item = node.Tag as Node;
					if (index == -1)
						parent.Nodes.Add(item);
					else
					{
						parent.Nodes.Insert(index, item);
						index++;
					}
				}
			}
		}
	}
}
