using DevExpress.Xpf.Core;
using System.Configuration;
using System.Data;
using System.Windows;

namespace LanguageEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        public App()
        {
            // Set the application theme in the constructor
            ApplicationThemeHelper.ApplicationThemeName = "Office2019Black";
        }
    }

}
