using MultiLanguage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Demo
{
    /// <summary>
    /// DynamicWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DynamicWindow : Window
    {
        public DynamicWindow()
        {
            InitializeComponent();
        }

        #region field
        private LanguageManager _Language => LanguageManager.Instance;
        #endregion

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //_Language.CollectText(this, 1);
            //_Language.SaveTranslateData(1);
            _Language.InitDynamicForm(this, true);
        }
    }
}
