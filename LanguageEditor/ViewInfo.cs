//显示数据
//by hdp 2025.04.24
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.Mvvm.Native;
using MultiLanguage;
using PropertyChanged;

namespace LanguageEditor
{
    public class ViewInfo
    {
        public ViewInfo(string configFileName)
        {
            _ConfigFileName = configFileName;
            Load();
        }

        #region property
        public List<string> ColumnNames { get; set; } = [];
        public HashSet<int> Levels { get; set; } = [];
        public ObservableCollection<RowInfo> Rows { get; set; } = [];
        //源文本字典，用于修改时校验
        public readonly HashSet<string> SourceHash = [];
        #endregion

        #region field
        //配置文件名
        private readonly string _ConfigFileName;
        #endregion

        #region public function
        public RowInfo CreateRow()
        {
            RowInfo row = new()
            {
                Source = new TextInfo(string.Empty),
                Translations = new TextInfo[ColumnNames.Count]
            };
            for (int i = 0; i < row.Translations.Length; i++)
                row.Translations[i] = new TextInfo(string.Empty);
            return row;
        }
        #endregion

        #region private function 
        private void SetModified(bool isModified)
        { 
            Rows.ForEach(row =>
            {
                row.Source.IsModified = isModified;
                for (int i = 0; i < row.Translations.Length; i++)
                    row.Translations[i].IsModified = isModified;
            });
        }
        #endregion

        #region load & save
        private void Load()
        {
            try
            {
                TranslateData data = new() { ConfigFileName = _ConfigFileName };
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
                SetModified(false);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void Data2View(TranslateData data)
        {
            Rows.Clear();
            SourceHash.Clear();

            ColumnNames = [.. data.Config.Types.Select(type => type.Text)];
            Levels = [.. data.Config.Files.Keys.Select(level => level)];

            data.Data.Keys.ForEach(source =>
            {
                TranslateDataInfo d = data.Data[source];
                RowInfo row = new()
                {
                    Source = new TextInfo(source),
                    Level = d.Level,
                };
                //避免null
                if (d.Texts == null)
                { 
                    row.Translations = new TextInfo[data.Config.Types.Length];
                    for (int i = 0; i < row.Translations.Length; i++)
                        row.Translations[i] = new TextInfo(string.Empty);
                }
                else
                    row.Translations = [.. d.Texts.Select(t => new TextInfo(t))];
                Rows.Add(row);

                SourceHash.Add(source);
            });
        }
        private TranslateData View2Data()
        {
            TranslateData data = new() { ConfigFileName = _ConfigFileName };
            data.LoadConfig();
            if (data.Config == null)
                return data;

            Dictionary<string, TranslateDataInfo> dict = data.Data;

            foreach (RowInfo row in Rows)
            {
                if (!string.IsNullOrEmpty(row.Source.Text))
                {
                    string[] texts = row.Translations?.Select(t => t.Text).ToArray();
                    dict[row.Source.Text] = new TranslateDataInfo(row.Level, texts);
                }
            }

            return data;
        }
        #endregion
    }

    //行数据
    [AddINotifyPropertyChangedInterface]
    public class RowInfo
    {
        public TextInfo Source { get; set; }
        public int Level { get; set; }
        public TextInfo[] Translations { get; set; }

        #region 表格的条件比较不支持数组，只能额外写属性来处理
        public bool IsModified0 => Source.IsModified;
        public bool IsModified1 => (Translations != null && Translations.Length > 0) && Translations[0].IsModified;
        public bool IsModified2 => (Translations != null && Translations.Length > 1) && Translations[1].IsModified;
        public bool IsModified3 => (Translations != null && Translations.Length > 2) && Translations[2].IsModified;
        public bool IsModified4 => (Translations != null && Translations.Length > 3) && Translations[3].IsModified;
        #endregion

    }
    //文本数据
    [AddINotifyPropertyChangedInterface]
    public class TextInfo
    {
        public TextInfo(string text) => Text = text;

        public string Text { get; set; }
        public bool IsModified { get; set; }
    }
}



