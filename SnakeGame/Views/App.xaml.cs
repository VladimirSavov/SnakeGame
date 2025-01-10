namespace SnakeGame;

#if WINDOWS
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Windows.Graphics;
#endif

public partial class App : Application
{
    public App(IServiceProvider services)
    {
        InitializeComponent();

        MainPage = new NavigationPage(services.GetRequiredService<LoginPage>());
    }
}
