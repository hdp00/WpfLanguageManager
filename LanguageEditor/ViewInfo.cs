//显示数据
//by hdp 2025.04.24
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.Mvvm.Native;
using MultiLanguage;

namespace LanguageEditor
{
    internal class ViewInfo
    {
        public ViewInfo(string configFileName)
        {
            _ConfigFileName = configFileName;
            Load();
        }

        #region property
        public List<string> ColumnNames { get; set; } = new List<string>();
        public HashSet<int> Levels { get; set; } = new HashSet<int>();
        public ObservableCollection<RowInfo> Rows { get; set; } = new ObservableCollection<RowInfo>();
        #endregion

        #region field
        private readonly string _ConfigFileName;
        #endregion

        #region load & save
        public void Load()
        {
            try
            {
                TranslateData data = new TranslateData() { ConfigFileName = _ConfigFileName };
                data.Load();
                Data2View(data);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        public void Save()
        {
            try
            {
                View2Data().Save();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void Data2View(TranslateData data)
        {
            ColumnNames.Clear();
            Levels.Clear();
            Rows.Clear();

            data.Config.Types.ForEach(type => ColumnNames.Add(type.Text));
            data.Config.Files.Keys.ForEach(level => Levels.Add(level));
            data.Data.Keys.ForEach(source => 
            {
                Rows.Add(new RowInfo()
                {
                    Source = source,
                    Level = data.Data[source].Level,
                    Translations = data.Data[source].Texts
                });
            });
        }
        private TranslateData View2Data()
        {
            TranslateData data = new TranslateData() { ConfigFileName = _ConfigFileName };
            Dictionary<string, TranslateDataInfo> dict = data.Data;

            foreach (RowInfo row in Rows)
            {
                dict[row.Source] = new TranslateDataInfo(row.Level, row.Translations);
            }

            return data;
        }
        #endregion
    }

    //行数据
    public class  RowInfo
    {
        public string Source { get; set; } = "";
        public int Level { get; set; }
        public string[]? Translations { get; set; }
    }
}



