//翻译数据定义
//by hdp 2024.12.25
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiLanguage
{
    public class TranslateData
    {
        //语言类型
        public TranslateTypeData[] Types { get; set; }
        //翻译数据
        public Dictionary<string, string[]> Data { get; set; }

        public void Sort()
        {
            Dictionary<string, string[]> sortedDict = new Dictionary<string, string[]>();
            List<string> keys = new List<string>(Data.Keys);
            keys.Sort();
            keys.ForEach(key => sortedDict.Add(key, Data[key]));
            Data = sortedDict;
        }
    }

    //语言类型
    public class TranslateTypeData
    {
        public string Value { get; set; }
        public string Text { get; set; }

        public override string ToString() => Value;
    }


}

