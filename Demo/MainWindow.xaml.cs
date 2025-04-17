using MultiLanguage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Demo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window, ILanguageForm
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        #region field
        private LanguageManager _language = LanguageManager.Instance;
        #endregion

        #region event
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitLanguage();
        }
        #endregion

        #region init
        private void InitLanguage()
        {
            InitExcludeControl();
            _language.InitLanguageSelectComboBox(this, cmb_Language);

            //_language.CollectText(this);
            //_language.SaveTranslateData();

            _language.InitLanguage(this);
            _language.ChangeLanguage(this);
        }
        private void InitExcludeControl()
        {
            _language.Exclude.ExcludeClass.TextBox = true;
            _language.Exclude.AddExcludeName("cmb_Language", "group_Exclude");
        }
        #endregion

        #region ILanguageForm
        public void OnChangeLanguage()
        {
            //lable_Additional.Text = _language.TranslateText("页面A");
        }
        #endregion
    }
}
