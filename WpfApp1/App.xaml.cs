using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DevExpress.Xpf.Core;

namespace WpfApp1
{
    /// <summary>  
    /// App.xaml 的交互逻辑  
    /// </summary>  
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            ApplicationThemeHelper.ApplicationThemeName = "Office2019Black";
        }
    }
}
