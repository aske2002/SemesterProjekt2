
namespace MauiCleanTodos.App.Controls;

public interface IBottomSheet
{
    void ShowBottomSheet(object view, bool animated = true);
}

public class BottomSheetControl : IBottomSheet
{
    public void ShowBottomSheet(object view, bool animated = true)
    {
        if (view is not View)
            return;
    }
}

