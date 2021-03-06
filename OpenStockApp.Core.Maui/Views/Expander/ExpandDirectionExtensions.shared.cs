using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStockApp.Core.Maui.Views
{
	static class ExpandDirectionExtensions
	{
		public static bool IsVertical(this ExpandDirection orientation)
			=> orientation == ExpandDirection.Down
				|| orientation == ExpandDirection.Up;

		public static bool IsRegularOrder(this ExpandDirection orientation)
			=> orientation == ExpandDirection.Down
				|| orientation == ExpandDirection.Right;
	}
}
