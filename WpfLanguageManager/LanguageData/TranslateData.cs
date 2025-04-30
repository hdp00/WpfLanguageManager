//翻译数据定义
//主要的修改是为翻译文件加了个索引，可以将翻译文件分割为多个
//by hdp 2024.12.25
using MultiLanguage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MultiLanguage
{
    public class TranslateData
    {
        #region property
        //配置文件名
        public string ConfigFileName { get; set; }
        //翻译配置
        public TranslateConfigInfo Config { get; set; }
        //翻译数据
        public Dictionary<string, TranslateDataInfo> Data { get; } = new Dictionary<string, TranslateDataInfo>();
        #endregion

        #region private function
        private Dictionary<int, Dictionary<string, string[]>> SortAndGroup()
        {
            Dictionary<string, TranslateDataInfo> sortedDict = new Dictionary<string, TranslateDataInfo>();
            List<string> keys = new List<string>(Data.Keys);
            keys.Sort();

            Dictionary<int, Dictionary<string, string[]>> total = new Dictionary<int, Dictionary<string, string[]>>();
            keys.ForEach(key =>
            {
                TranslateDataInfo data = Data[key];
                int level = data.Level;
                if (!total.ContainsKey(level))
                    total[level] = new Dictionary<string, string[]>();
                Dictionary<string, string[]> single = total[level];

                single[key] = data.Texts;
            });

            return total;
        }
        #endregion

        #region load & save
        public void Load()
        {
            string path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            string configName = Path.Combine(path, ConfigFileName); 
            
            try
            {
                string text = File.ReadAllText(configName);
                Config = JsonConvert.DeserializeObject<TranslateConfigInfo>(text);

                foreach (int level in Config.Files.Keys)
                { 
                    string fileName = Path.Combine(path, Config.Files[level]);
                    string fileText = File.ReadAllText(fileName);
                    Dictionary<string, string[]> translate = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(fileText);
                    foreach(string source in translate.Keys)
                    {
                        Data[source] = new TranslateDataInfo(level, translate[source]);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        //-1 保存全部；其他值 保存指定层级
        public void Save(int level = -1)
        {
            string path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            try
            {
                Dictionary<int, Dictionary<string, string[]>> saveData = SortAndGroup();
                foreach (int key in saveData.Keys)
                {
                    if (level != -1 && key != level)
                        continue;

                    string fullName = Path.Combine(path, Config.Files[key]);
                    string text = JsonConvert.SerializeObject(saveData[key], Formatting.Indented);
                    File.WriteAllText(fullName, text);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        #endregion
    }

    #region define
    //语言类型
    public class TranslateTypeInfo
    {
        public string Value { get; set; }
        public string Text { get; set; }

        public override string ToString() => Value;
    }
    //翻译配置
    public class TranslateConfigInfo
    {
        //语言类型
        public TranslateTypeInfo[] Types { get; set; }
        //翻译数据文件<层级, 文件名>
        public Dictionary<int, string> Files { get; set; }
    }
    //翻译数据
    public class TranslateDataInfo
    {
        public TranslateDataInfo(int level, string[] texts)
        { 
            Level = level;
            Texts = texts;
        }
        
        //层级
        public int Level { get; set; }
        //翻译文本
        public string[] Texts { get; set; }
    }
    #endregion
}