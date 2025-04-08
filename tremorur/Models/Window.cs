using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
namespace tremorur.Models
{

    public class WatchWindow : Window
    {
        public WatchWindow(Page page) : base(page)
        {
            this.MaximumWidth = 800;
            this.MaximumHeight = 800;
            this.MinimumHeight = 800;
            this.MinimumWidth = 800;
        }
    }
}
