using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universal.UI.Xaml.Controls
{
    public enum SwipeListViewItemState
    {
        /// <summary>
        /// The item is collapsed.
        /// </summary>
        Collapsed,

        /// <summary>
        /// The item is expanded.
        /// </summary>
        Expanded,

        /// <summary>
        /// The item is persisted to the left.
        /// </summary>
        PersistedLeft,

        /// <summary>
        /// The item is persisted to the right.
        /// </summary>
        PersistedRight
    }
}
