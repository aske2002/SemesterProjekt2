using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
namespace tremorur.Models
{

    public class WatchWindow : Window
    {
        public WatchWindow(Page page) : base(page)
        {
            SizeChanged += KeepWindowSquare;
        }
        private void KeepWindowSquare(object? sender, EventArgs e)
        {
            Debug.WriteLine($"Window size changed: {Width}x{Height}");
            if (Width == 0 || Height == 0)
                return;

            // Keep the window square
            if (Width > Height)
            {
                Width = Height;
            }
            else
            {
                Height = Width;
            }
        }
    }
}
