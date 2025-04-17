using MultiLanguage;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

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

            //List<Person> list = new List<Person>()
            //{
            //    new Person("a", 0),
            //    new Person("b", 1),
            //    new Person("c", 2),
            //};

            List<string> list = new List<string>()
            {
                "a",
                "b",
                "c",
            };

            //ComboBoxItem
            //TreeViewItem
            //comboBox1.ItemsSource = list;
            //comboBox1.DisplayMemberPath = "Name";
            //comboBox1.DisplayMemberPath = "Id";
        }

        #region field
        private LanguageManager _language = LanguageManager.Instance;
        #endregion

        #region event
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            comboBox.Items.Add("工具栏A");

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

        private void button_Click(object sender, RoutedEventArgs e)
        {


        }

    }

    public class Person
    { 
        public Person(string name, int id)
        {
            Name = name;
            Id = id;
        }   

        public string Name { get; set; }
        public int Id { get; set; }
    }
}
