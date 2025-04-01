using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace tremorur.Models
{

    public class WatchWindow : Window
    {
        public WatchWindow(Page page) : base(page)
        {
            this.SizeChanged += OnSizeChanged;
            this.MaximumWidth = 800;
            this.MaximumHeight = 800;
            this.MinimumHeight = 800;
            this.MinimumWidth = 800;
        }

        protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
        }
        private void OnSizeChanged(object? sender, EventArgs e)
        {
        }
    }
}
