using System.Windows;

namespace VisualKeyboard.Examples
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainWindow app = new MainWindow();
            app.DataContext = new MainWindowViewModel();
            app.Show();
        }
    }
}
