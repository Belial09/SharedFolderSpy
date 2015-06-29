using System;
using System.Collections.Generic;
using System.Text;

namespace Aga.Controls.Tree
{
	public interface IToolTipProvider
	{
		string GetToolTip(TreeNodeAdv node);
	}
}
