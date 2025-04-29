using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.ConditionalFormatting;
using DevExpress.Xpf.Grid;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : ThemedWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Init();
        }

        #region init
        private void Init()
        {
            DataContext = this;
            InitCommand();
            InitData();
            InitGrid();
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
        public class Person
        {
            public Person(string name, int age)
            {
                Name = name;
                Age = age;
            }

            public string Name { get; set; }
            public int Age { get; set; }
            public Value ValueA { get; set; } = new Value();
            public Value ValueB { get; set; } = new Value();
            public Value ValueC { get; set; } = new Value();
            public Value ValueD { get; set; } = new Value();
        }
        public class Value
        {
            public string Text { get; set; } = "aaa";
            public bool IsChanged { get; set; }
        }

        public List<Person> Persons { get; set; } = new List<Person>();

        private void InitData()
        {
            Persons.Add(new Person("张三", 18));
            Persons.Add(new Person("李四", 17));
            Persons.Add(new Person("王五", 22));
            Persons.Add(new Person("赵六", 22));

            //Persons[0].ValueA.IsChanged = false;
        }
        private void InitGrid()
        {
            GridColumn c = new GridColumn()
            {
                Header = "ValueA",
            };
            c.Binding = new Binding("ValueA.Text") { Mode = BindingMode.TwoWay };
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
                Background = Brushes.Yellow
            };

            // 添加到 TableView 的 FormatConditions
            tableView.FormatConditions.Add(formatCondition);

            grid.Columns.Add(new GridColumn() {
                Header = "ValueB",
                Binding = new Binding("ValueB.Text") { Mode = BindingMode.TwoWay }
            });
            grid.Columns.Add(new GridColumn()
            {
                Header = "ValueC",
                Binding = new Binding("ValueC.Text") { Mode = BindingMode.TwoWay }
            });
            grid.Columns.Add(new GridColumn()
            {
                Header = "ValueD",
                Binding = new Binding("ValueD.Text") { Mode = BindingMode.TwoWay }
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
                MessageBox.Show($"{(row as Person).Name}");
            }
        }

        #endregion
    }
}
