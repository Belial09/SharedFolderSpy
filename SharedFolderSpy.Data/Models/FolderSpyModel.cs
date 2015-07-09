using System;
using System.Collections.Generic;
using Fesslersoft.SharedFolderSpy.Controls.AdvancedTreeview;
using SharedFolderSpy.Data.Entities;

namespace SharedFolderSpy.Data.Models
{
    public class FolderSpyModel : ITreeModel
    {
        private List<BaseItem> _itemsToRead;

        public System.Collections.IEnumerable GetChildren(TreePath treePath)
        {
            if (treePath.IsEmpty())
            {
                //get folders
            }
            else
            {
                List<BaseItem> items = new List<BaseItem>();
                BaseItem parent = treePath.LastNode as BaseItem;
                if (parent != null)
                {
                    var folder = parent.ItemPath;
                    //get all users

                    //foreach (string str in Directory.GetDirectories())
                    //    items.Add(new FolderItem(str, parent));

                    //_itemsToRead.AddRange(items);

                    //foreach (BaseItem item in items)
                    //    yield return item;
                    yield return null;
                }
            }
        }

        public bool IsLeaf(TreePath treePath)
        {
            return treePath.LastNode is UserItem;
        }

        public event EventHandler<TreeModelEventArgs> NodesChanged;
        public event EventHandler<TreeModelEventArgs> NodesInserted;
        public event EventHandler<TreeModelEventArgs> NodesRemoved;
        public event EventHandler<TreePathEventArgs> StructureChanged;


    }
}
