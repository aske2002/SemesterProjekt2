using Microsoft.Maui.LifecycleEvents;
using Tremorur;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureMauiHandlers(handlers =>
			{
#if WINDOWS
                handlers.AddHandler<Microsoft.UI.Xaml.Window, CustomWindowHandler>();
#endif
			})
			.ConfigureLifecycleEvents(events =>
			{
#if WINDOWS
                events.AddWindows(w =>
                {
                    w.OnWindowCreated(window =>
                    {
                        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
                        var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(
                            Microsoft.UI.WindowId.FromWindowHandle(hWnd));

                        if (appWindow.Presenter is Microsoft.UI.Windowing.OverlappedPresenter presenter)
                        {
                            presenter.IsResizable = false;
                            presenter.IsMaximizable = false;
                        }

                        appWindow.SetPresenter(Microsoft.UI.Windowing.AppWindowPresenterKind.CompactOverlay);
                    });
                });
#endif
			});

		return builder.Build();
	}
}
