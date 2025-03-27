using Microsoft.Maui.Controls.Shapes;

namespace tremorur.Views;
public class WatchPage : ContentPage
{
    public WatchPage(View content)
    {
        BackgroundColor = Transparent;

        Content = new Grid
        {
            Children =
            {
                new Border
                {

                    Padding = 0,
                    Margin = 20,
                    StrokeShape = new RoundRectangle {
                        CornerRadius = new CornerRadius(100),
                    },
                    BackgroundColor = Colors.White,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Content = content
                }
            }
        };
    }
}
