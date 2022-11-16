using MauiSaxonHESchematronValidator.ViewModels;
using Microsoft.Extensions.Logging;
using net.sf.saxon.s9api;
using System.Reflection;

namespace MauiSaxonHESchematronValidator;

public static class MauiProgram
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
			});


        // force loading of updated xmlresolver
        ikvm.runtime.Startup.addBootClassPathAssembly(Assembly.Load("org.xmlresolver.xmlresolver"));
        ikvm.runtime.Startup.addBootClassPathAssembly(Assembly.Load("org.xmlresolver.xmlresolver_data"));

        builder.Services.AddSingleton<Processor>(new Processor(false));
        builder.Services.AddSingleton<HttpClient>();
        builder.Services.AddSingleton<SaxonSchematronValidatorViewModel>();
        builder.Services.AddSingleton<MainPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
