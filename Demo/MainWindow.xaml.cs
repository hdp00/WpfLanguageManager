using MultiLanguage;
using System.Windows;
using System.Windows.Controls;

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
            //新建窗体的翻译
            new DynamicWindow().Show();
        }
        private void button_ChangeControl_Click(object sender, RoutedEventArgs e)
        {
            treeView.Items.Clear();
            treeView.Items.Add("节点2");
            treeView.Items.Add("节点1");
            _language.ChangeDynamicControlLanguage(treeView);
        }
        #endregion

        #region init
        private void InitLanguage()
        {
            //设置翻译数据文件
            _language.Init("LanguageData/Config.json");
            //默认翻译所有控件，在此添加不需要翻译的控件
            InitExcludeControl();
            //填充语言切换下拉框
            _language.InitLanguageSelectComboBox(this, cmb_Language);

            //收集翻译文本
            //_language.CollectText(this, 0);
            //_language.SaveTranslateData(0);

            //初始化控件数据
            _language.InitLanguage(this);
            //翻译控件
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
