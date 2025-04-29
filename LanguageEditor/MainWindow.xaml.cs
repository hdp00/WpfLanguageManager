using DevExpress.Data.Browsing;
using DevExpress.Mvvm;
using DevExpress.Xpf.Core.ConditionalFormatting;
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

        #region field
        private ViewInfo? _View;
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

        private void InitCommand()
        {
            SaveCommand = new DelegateCommand(() =>
            {
                Title = "save";
            },
            () =>
            {
                return false;
            });
            NewItemCommand = new DelegateCommand(() =>
            {
                Title = "newItem";
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
            grid.Columns.Add(new GridColumn() { Header = "源", FieldName = "Source" });
            for (int i = 0; i < _View?.ColumnNames.Count; i++)
            {
                string name = _View.ColumnNames[i];
                grid.Columns.Add(
                    new GridColumn()
                    {
                        Header = name,
                        Binding = new System.Windows.Data.Binding($"Translations[{i}]") { Mode = BindingMode.TwoWay }
                    }
                );
            }

            GridColumn c = new GridColumn()
            {
                Header = "ValueA",
            };
            c.Binding = new System.Windows.Data.Binding("ValueA.Text") { Mode = BindingMode.TwoWay };
            grid.Columns.Add(c);

            // 创建条件格式化
            var formatCondition = new FormatCondition()
            {
                FieldName = c.FieldName, //绑定到IsChanged属性
                Expression = "ValueA.IsChanged==True", //条件表达式
                ApplyToRow = false, //仅应用到单元格
            };

            // 设置格式化样式
            formatCondition.Format = new Format()
            {
                Background = System.Windows.Media.Brushes.Yellow
            };

            // 添加到 TableView 的 FormatConditions
            tableView.FormatConditions.Add(formatCondition);

            grid.Columns.Add(new GridColumn()
            {
                Header = "ValueB",
                Binding = new System.Windows.Data.Binding("ValueB.Text") { Mode = BindingMode.TwoWay }
            });
            grid.Columns.Add(new GridColumn()
            {
                Header = "ValueC",
                Binding = new System.Windows.Data.Binding("ValueC.Text") { Mode = BindingMode.TwoWay }
            });
            grid.Columns.Add(new GridColumn()
            {
                Header = "ValueD",
                Binding = new System.Windows.Data.Binding("ValueD.Text") { Mode = BindingMode.TwoWay }
            });
        }

        private void tableView_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            if (e.Column.VisibleIndex == 1)
            {
                if (e.Row is Person p)
                {
                    p.ValueA.IsChanged = true;
                }
            }
        }
        private void tableView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // 获取当前单元格信息
            var view = sender as TableView;
            var hitInfo = view?.CalcHitInfo(e.OriginalSource as DependencyObject);

            if (hitInfo != null && hitInfo.RowHandle >= 0 && hitInfo.Column != null)
            {
                // 获取单元格的值
                var row = grid.GetRow(hitInfo.RowHandle);
                System.Windows.Forms.MessageBox.Show($"{(row as Person).Name}");
            }
        }
        #endregion
    }
}