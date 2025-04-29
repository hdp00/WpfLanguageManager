using DevExpress.Data.Browsing;
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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
    }
}


//public MainWindow()
//{
//    InitializeComponent();
//    Init();
//}

//#region event
//#endregion

//#region field
//private ViewInfo _View;
//#endregion

//#region init
//private void Init()
//{
//    _View = new ViewInfo("LanguageData/Config.json");
//    DataContext = _View;

//    InitGrid();
//}
//private void InitGrid()
//{
//    InitColumns();
//}
//private void InitColumns()
//{
//    grid.Columns.Add(new GridColumn() { Header = "源", FieldName = "Source" });
//    for (int i = 0; i < _View.ColumnNames.Count; i++)
//    {
//        string name = _View.ColumnNames[i];
//        grid.Columns.Add(
//            new GridColumn()
//            {
//                Header = name,
//                Binding = new Binding($"Translations[{i}]") { Mode = BindingMode.TwoWay }
//            }
//        );
//    }
//}
//#endregion