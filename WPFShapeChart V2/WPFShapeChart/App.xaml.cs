using System.Windows;
using WPFShapeChart.ViewModels;

namespace WPFShapeChart
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);


            //so maybe the best way to do this would then be to have page browsing in a viewmodel, will be the mainmenuviewmodel then?



            var app = new MainWindow(); //use this simply as the container of our app, displaying only our main menu viewmodel

            var MainMenuView = new MainMenuViewModel();
            app.TestContentControl.Content = MainMenuView;
            


            app.Show();
        }



    }
}
