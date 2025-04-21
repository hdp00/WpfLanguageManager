using MultiLanguage;
using System.Windows;

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
            DataContext = this;
        }

        #region field
        private readonly LanguageManager _language = LanguageManager.Instance;
        #endregion

        #region event
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitLanguage();
        }
        private void button_NewForm_Click(object sender, RoutedEventArgs e)
        {
            new DynamicWindow().Show();
        }
        #endregion

        #region init
        private void InitLanguage()
        {
            _language.Init("LanguageData/Config.json");
            InitExcludeControl();
            _language.InitLanguageSelectComboBox(this, cmb_Language);

            //_language.CollectText(this, 0);
            //_language.SaveTranslateData(0);

            _language.InitLanguage(this);
            _language.ChangeLanguage(this);
        }
        private void InitExcludeControl()
        {
            _language.Exclude.ExcludeClass.TextBox = true;
            _language.Exclude.AddExcludeName("cmb_Language", "groupBox");
        }
        #endregion

        #region ILanguageForm
        public void OnChangeLanguage()
        {
            lable_Additional.Content = _language.TranslateText("页面A");
        }
        #endregion
    }
}
