using Microsoft.Extensions.DependencyInjection;

namespace MyLocation;

public partial class AppShell : Shell
{
    public AppShell(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        MainPageContent.ContentTemplate = new DataTemplate(() => serviceProvider.GetRequiredService<MainPage>());
    }
}
