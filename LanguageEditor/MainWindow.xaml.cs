//翻译文本编辑器
//by hdp 2025.04.20
using DevExpress.ClipboardSource.SpreadsheetML;
using DevExpress.Xpf.Grid;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LanguageEditor
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<Employee> Employees { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            // 初始化示例数据
            Employees = new ObservableCollection<Employee>
            {
                new Employee { Id = 1, Name = "张三", Position = "开发工程师", Salary = 12000 },
                new Employee { Id = 2, Name = "李四", Position = "测试工程师", Salary = 10000 },
                new Employee { Id = 3, Name = "王五\r\nccc\r\nddd", Position = "产品经理", Salary = 15000 }
            };

            // 设置数据上下文
            DataContext = this;
        }

        #region event
        private void button_Click(object sender, RoutedEventArgs e)
        {
            Employees.Add(new Employee { Id = Employees.Count + 1, Name = "新员工dgjfdlkgjrdofihuoeridlkjgdflkhjgflkhjoiertjgdfoibjer90igjktrlhfgijsdoihgdlfkjhgofijgdsilkg", Position = "新职位", Salary = 8000 });
        }

        private void button_Delete_Click(object sender, RoutedEventArgs e)
        {
            Employees.RemoveAt(0);
        }

        private void tableView_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
        }
        #endregion


    }

    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public decimal Salary { get; set; }
    }
}
