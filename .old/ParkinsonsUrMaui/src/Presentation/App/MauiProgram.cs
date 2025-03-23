using CommunityToolkit.Maui;
using MauiCleanTodos.ApiClient;
using MauiCleanTodos.App.Controls;
using MauiCleanTodos.Infrastructure.Identity

namespace MauiCleanTodos.App;

public static partial class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				fonts.AddFont("Sora-VariableFont_wght.ttf", "Sora");
				fonts.AddFont("Viga-Regular.ttf", "Viga");
				fonts.AddFont("FluentSystemIcons-Regular.ttf", "FluentIcons");
			})
			.UseMauiCommunityToolkit()
			.UseAutodependencies();

		builder.Services.RegisterMauiClient(opt =>
		{
			opt.Authority = "https://localhost:44447";
			opt.BaseUrl = "https://localhost:44447";
			opt.ClientId = "mctmobileapp";
			opt.RedirectUri = "auth.com.goldie.mauicleantodos.app://callback";
			opt.Scope = "MauiCleanTodos.WebUIAPI openid profile offline_access";
		});

		builder.Services.AddInfrastructureServices(builder.Configuration);

		builder.Services.AddSingleton<IBottomSheet, BottomSheetControl>();

		return builder.Build();
	}
}