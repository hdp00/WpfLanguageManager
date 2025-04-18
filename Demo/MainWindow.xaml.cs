using MultiLanguage;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

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

            list = new List<Person>()
            {
                new Person("a", 0),
                new Person("b", 1),
                new Person("c", 2),
            };

            //list = new List<string>()
            //{
            //    "选项A",
            //    "选项B",
            //    "c",
            //};

            //ComboBoxItem
            //TreeViewItem
            //comboBox1.ItemsSource = list;
            //comboBox1.DisplayMemberPath = "Name";
            //comboBox1.DisplayMemberPath = "Id";
            //ListBox
            //ComboBox
            //Canvas
            //TreeView
            //TreeViewItem
            //ComboBoxItem
            //MenuItem
            //TreeView
        }

        #region field
        private LanguageManager _language = LanguageManager.Instance;

        public List<Person> list { get; set; }
        #endregion

        #region event
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            comboBox.Items.Add("工具栏A");
            comboBox1.Items.Add(new Person("aaa", 111));
            comboBox1.Items.Add(new Person("bbb", 222));
            comboBox1.SelectedIndex = 0;

            DependencyObject c = comboBox;
            //bool b = BindingOperations.IsDataBound(c, ComboBox.ItemsSourceProperty);
            //var aaa = c.ItemsSource;
            //var a0 = FindLogicalChildren<FrameworkElement>(listBox1).ToList();
            //var a1 = FindLogicalChildren(listBox1).ToList();

            InitLanguage();
        }
        private IEnumerable<T> FindVisualChildren<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) yield break;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T t)
                {
                    yield return t;
                }
                foreach (var childOfChild in FindVisualChildren<T>(child))
                {
                    yield return childOfChild;
                }
            }
        }
        private IEnumerable<T> FindLogicalChildren<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) yield break;

            foreach(var child in LogicalTreeHelper.GetChildren(parent))
            {
                if (child is T t)
                {
                    yield return t;
                }
                foreach (var descendant in FindLogicalChildren<T>(child as DependencyObject))
                {
                    yield return descendant;
                }
            }
        }
        private IEnumerable<object> FindLogicalChildren(DependencyObject parent)
        {
            if (parent == null) yield break;

            foreach (var child in LogicalTreeHelper.GetChildren(parent))
            {
                yield return child;
                foreach (var childOfChild in FindLogicalChildren(child as DependencyObject))
                {
                    yield return childOfChild;
                }
            }
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
            var a0 = FindVisualChildren<DependencyObject>(comboBox1).ToList();
        }

        private void button_Click_1(object sender, RoutedEventArgs e)
        {
            //var a0 = FindLogicalChildren<FrameworkElement>(listBox1).ToList();
            //var a1 = FindLogicalChildren(listBox1).ToList();
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
