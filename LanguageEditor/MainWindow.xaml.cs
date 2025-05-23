﻿using DevExpress.Data.Browsing;
using DevExpress.Export.Xl;
using DevExpress.Mvvm;
using DevExpress.Xpf.Core.ConditionalFormatting;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Grid.GroupRowLayout;
using LanguageEditor;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
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

        #region const
        private const string Text = "Language Editor";
        #endregion

        #region property
        private bool _IsModified;
        private bool IsModified
        {
            get => _IsModified;
            set
            {
                _IsModified = value;
                Title = _IsModified ? $"{Text} *" : Text;
                //SaveCommand.RaiseCanExecuteChanged();
            }
        }
        public ViewInfo View { get; set; }
        #endregion

        #region event
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (IsModified)
            {
                MessageBoxResult result = System.Windows.MessageBox.Show("是否保存修改？", "提示", MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Cancel)
                    e.Cancel = true;
                else if (result == MessageBoxResult.Yes)
                    SaveCommand.Execute(null);
            }
        }
        #endregion

        #region 直接在Xmal中为GridControl定义name会弹出DevExpress试用信息，在代码中就没问题
        public GridControl grid => dockPanel.Children[1] as GridControl;
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
            DataContext = this;
            View = new ViewInfo("LanguageData/Config.json");
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
                View.Save();
                IsModified = false;
            },
            () =>
            {
                return IsModified;
            });

            NewItemCommand = new DelegateCommand(() =>
            {
                RowInfo row = View.CreateRow();
                ItemWindow w = new();
                w.Init(View, row.Source.Text, row.Level);
                bool? result = w.ShowDialog();
                if (result == true)
                {
                    ChangeSource(row, w.Source, w.Level);
                }
                View.Rows.Add(row);
            });

            DeleteItemCommand = new DelegateCommand(() =>
            {
                if (grid.SelectedItem is RowInfo row
                    && System.Windows.MessageBox.Show("是否删除当前项？", "提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    View.DeleteRow(row.Source.Text);
                    IsModified = true;
                }
            });
        }
        #endregion

        #region gridcontrol
        private void InitGrid()
        {
            InitColumn();
            InitFormat();
        }
        private void InitColumn()
        {
            for (int i = 0; i < View.ColumnNames.Count; i++)
            {
                grid.Columns.Add(
                    new GridColumn()
                    {
                        Header = View.ColumnNames[i],
                        Binding = new System.Windows.Data.Binding($"Texts[{i}].Text") { Mode = BindingMode.TwoWay }
                    }
                );
            }

            grid.Columns.Add(new GridColumn() { Header = "文件", FieldName = "Level", GroupIndex = 0 });
            grid.Columns[0].AllowEditing = DevExpress.Utils.DefaultBoolean.False;
        }
        private void InitFormat()
        {
            //最后一列Level，不需要格式化
            int count = grid.Columns.Count - 1;
            for (int i = 0; i < count; i++)
            {
                GridColumn c = grid.Columns[i];

                FormatCondition condiation = new()
                {
                    FieldName = c.FieldName,
                    Expression = $"IsModified{i}",
                    ApplyToRow = false,
                    Format = new Format()
                    {
                        Background = System.Windows.Media.Brushes.Yellow
                    }
                };

                tableView.FormatConditions.Add(condiation);
            }
        }
        //设置修改状态
        private void tableView_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            if (e.Row is RowInfo r)
            {
                int index = e.Column.VisibleIndex;
                if (index < 0 || index >= r.Texts.Count)
                    return;

                r.Texts[index].IsModified = true;
                IsModified = true;
            }
        }
        //编辑源文本
        private void tableView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is TableView table)
            {
                var hitInfo = table.CalcHitInfo(e.OriginalSource as DependencyObject);

                if (hitInfo != null && hitInfo.RowHandle >= 0 && hitInfo.Column == grid.Columns[0])
                {
                    RowInfo row = grid.GetRow(hitInfo.RowHandle) as RowInfo;
                    ItemWindow w = new();
                    w.Init(View, row.Source.Text, row.Level);
                    bool? result = w.ShowDialog();
                    if (result == true)
                    {
                        ChangeSource(row, w.Source, w.Level);
                        grid.RefreshRow(hitInfo.RowHandle);
                    }
                }
            }
        }
        #endregion

        #region data
        private void ChangeSource(RowInfo row, string source, int level)
        {
            row.Source.Text = source;
            row.Source.IsModified = true;
            row.Level = level;
            IsModified = true;
        }
        #endregion


    }
}