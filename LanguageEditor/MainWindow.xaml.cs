using DevExpress.Data.Browsing;
using DevExpress.Mvvm;
using DevExpress.Xpf.Core.ConditionalFormatting;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Grid;
using LanguageEditor;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LanguageEditor
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Init();
        }

        #region property
        private bool IsModified { get; set; }
        #endregion

        #region field
        private ViewInfo _View;
        #endregion

        #region event

        #endregion

        #region init
        private void Init()
        {
            InitData();
            InitCommand();
            InitGrid();
        }
        private void InitData()
        {
            _View = new ViewInfo("LanguageData/Config.json");
            DataContext = _View;
        }
        #endregion

        #region command
        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand NewItemCommand { get; set; }
        public DelegateCommand DeleteItemCommand { get; set; }

        private void InitCommand()
        {
            SaveCommand = new DelegateCommand(() =>
            {
                _View.Save();
                IsModified = false;
            },
            () =>
            {
                return IsModified;
            });

            NewItemCommand = new DelegateCommand(() =>
            {
                Title = "newItem";
            });

            DeleteItemCommand = new DelegateCommand(() =>
            {
                Title = "deleteItem";
            });
        }
        #endregion

        #region gridcontrol
        private void InitGrid()
        {
            InitColumn();
        }
        private void InitColumn()
        {
            grid.Columns.Add(new GridColumn() { Header = "源", FieldName = "Source.Text" });
            InitFormat(grid.Columns[0], "Source.IsModified == true");
            for (int i = 0; i < _View?.ColumnNames.Count; i++)
            {
                string name = _View.ColumnNames[i];
                grid.Columns.Add(
                    new GridColumn()
                    {
                        Header = name,
                        Binding = new System.Windows.Data.Binding($"Translations[{i}].Text") { Mode = BindingMode.TwoWay }
                    }
                );
                InitFormat(grid.Columns[i + 1], $"Translations[{i}].IsModified == true");
            }

            grid.Columns[0].AllowEditing = DevExpress.Utils.DefaultBoolean.False;
        }
        private void InitFormat(GridColumn column, string expression)
        {
            FormatCondition format = new FormatCondition()
            {
                FieldName = column.FieldName,
                Expression = expression,
                ApplyToRow = false,
            };

            format.Format = new Format()
            {
                Background = System.Windows.Media.Brushes.Yellow
            };

            tableView.FormatConditions.Add(format);
        }

        private void tableView_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            if (e.Row is RowInfo r)
            {
                int index = e.Column.VisibleIndex;

                if (index == 0)
                    r.Source.IsModified = true;
                else
                    r.Translations[index - 1].IsModified = true;
            }
        }
        private void tableView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is TableView table)
            {
                //var view = sender as TableView;
                //var hitInfo = view?.CalcHitInfo(e.OriginalSource as DependencyObject);

                //if (hitInfo != null && hitInfo.RowHandle >= 0 && hitInfo.Column != null)
                //{
                //    // 获取单元格的值
                //    var row = grid.GetRow(hitInfo.RowHandle);
                //    System.Windows.Forms.MessageBox.Show($"{(row as Person).Name}");
                //}            
            }
        }
        #endregion
    }
}