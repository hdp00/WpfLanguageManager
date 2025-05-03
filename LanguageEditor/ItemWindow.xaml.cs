using DevExpress.Mvvm.Native;
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

namespace LanguageEditor
{
    public partial class ItemWindow : Window
    {
        public ItemWindow()
        {
            InitializeComponent();
        }

        #region property
        public string Source
        { 
            get => txt_Source.Text;
            set => txt_Source.Text = value;
        }
        public int Level
        { 
            get => (int)combo_Level.SelectedItem;
            set
            { 
                combo_Level.SelectedItem = value;
                if (combo_Level.SelectedItem == null)
                    combo_Level.SelectedIndex = 0;
            }
        }
        public ViewInfo View { get; set; }
        #endregion

        #region field
        private string _SourceBackup;
        private int _LevelBackup;
        #endregion

        #region event
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txt_Source.Focus();
        }
        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            string s = Source;
            if (string.IsNullOrEmpty(s))
            {
                System.Windows.MessageBox.Show("不能输入空值", "提示");
                return;
            }

            if (_SourceBackup != s)
            {
                if (View.SourceHash.Contains(s))
                {
                    System.Windows.MessageBox.Show(this, "文本已存在", "提示");
                    return;
                }
                else
                    DialogResult = true;
            }
            else
            {
                if (_LevelBackup != Level)
                    DialogResult = true;
                else
                    DialogResult = false;
            }
        }
        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
        #endregion

        #region public function
        public void Init(ViewInfo view, string source, int level)
        {
            View = view;
            Source = source;

            combo_Level.Items.Clear();
            view.Levels.ForEach(level => combo_Level.Items.Add(level));
            Level = level;

            _SourceBackup = source;
            _LevelBackup = level;
        }

        #endregion


    }
}
