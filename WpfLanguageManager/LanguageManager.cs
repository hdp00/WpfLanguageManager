//多语言管理类
//by hdp 2024.12.22
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Xml.Linq;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Security.Policy;
using Newtonsoft.Json.Linq;
using System.ComponentModel;

namespace MultiLanguage
{
    public partial class LanguageManager
    {
        public LanguageManager()
        {
            Init();
        }

        #region property
        private static LanguageManager _instance = new LanguageManager();
        //实例
        public static LanguageManager Instance => _instance;

        #region language index
        //当前语言索引
        private int _currentLanguageIndex = 0;
        //当前语言
        public string CurrentLanguage
        {
            get => _translateData.Types[_currentLanguageIndex].Value;
            set
            {
                int index = Array.FindIndex(_translateData.Types, x => x.Value == value);
                if (index >= 0)
                    _currentLanguageIndex = index;
            }
        }
        public TranslateTypeData[] TranslateTypes => _translateData.Types;
        #endregion

        //标记是否正在切换语言
        private bool _isChangingLanguage = false;
        public bool IsChangingLanguage => _isChangingLanguage;
        //管理不需要翻译的控件
        public ExcludeManager Exclude = new ExcludeManager();
        #endregion

        #region field
        //翻译数据
        private TranslateData _translateData;
        //翻译字典
        private Dictionary<string, string[]> TranslateDict => _translateData.Data;
        //<hash, texts> 初始的文本数据
        private Dictionary<int, string[]> _sourceDict = new Dictionary<int, string[]>();
        //控件操作
        private ControlOperationManager _oper => new ControlOperationManager(this);
        #endregion

        #region public function
        //翻译文本
        public string TranslateText(string text)
        {
            if (!string.IsNullOrEmpty(text) && TranslateDict.TryGetValue(text, out string[] texts))
            {
                if (texts != null && texts.Length > _currentLanguageIndex && !string.IsNullOrWhiteSpace(texts[_currentLanguageIndex]))
                    return texts[_currentLanguageIndex];
            }

            return text;
        }
        //初始化语言切换控件
        public void InitLanguageSelectComboBox(Form main, ComboBox comboBox)
        { 
            new LanguageSelectCombox(this, main, comboBox);
        }
        #endregion

        #region private function
        private void Init()
        {
            LoadTranslateData();
        }
        #endregion

        #region translate data. 翻译数据的读取/保存
        private string TranslateFileName => Path.Combine(Application.StartupPath, "Translate.json");
        private void LoadTranslateData()
        {
            try
            {
                string text = File.ReadAllText(TranslateFileName);
                _translateData = JsonConvert.DeserializeObject<TranslateData>(text);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        public void SaveTranslateData()
        {
            try
            {
                _translateData.Sort();
                string text = JsonConvert.SerializeObject(_translateData, Formatting.Indented);
                File.WriteAllText(TranslateFileName, text);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        #endregion

        #region collect text. 收集需要翻译的信息
        public void CollectText(Control value)
        {
            CollectTextFunc(value);
        }
        private void CollectTextFunc(Control value)
        {
            if (!CurrentExclude.IsValid(value))
                return;

            CollectTextControl(value);

            if (value is ToolStrip)
                return;
            foreach (Control item in value.Controls)
            {
                CollectTextFunc(item);
            }
        }
        private void CollectTextControl(Control value)
        {
            if (!_oper.CollectText(value))
            {
                FillTranslateDict(value.Text);
            }
        }

        internal void FillTranslateDict(params string[] texts)
        {
            foreach (string text in texts)
            {
                if (string.IsNullOrWhiteSpace(text) || TranslateDict.ContainsKey(text))
                    continue;

                TranslateDict[text] = null;
            }
        }
        #endregion

        #region  init language 获取所有控件的初始文本
        public void InitLanguage(Control value)
        {
            InitLanguageFunc(value);
        }

        internal void InitLanguageFunc(Control value)
        {
            if (!CurrentExclude.IsValid(value))
                return;
            if (!IsOperatingDynamicForm && IsDynamicForm(value))
                return;

            InitLanguageControl(value);

            if (value is ToolStrip)
                return;
            foreach (Control item in value.Controls)
            {
                InitLanguageFunc(item);
            }
        }
        private void InitLanguageControl(Control value)
        {
            if (!_oper.InitLanguage(value))
            {
                FillSourceDict(value.GetHashCode(), value.Text);
            }
        }
        internal void FillSourceDict(int hash, params string[] texts)
        {
            if (Array.Exists(texts, text => !string.IsNullOrWhiteSpace(text)))
            {
                CurrentSourceDict[hash] = texts;
            }
        }
        #endregion

        #region change language 切换语言
        public void ChangeLanguage(Control value)
        {
            _isChangingLanguage = true;

            ChangeLanguageFunc(value);
            ChangeLanguageAllDynamicForm();

            _isChangingLanguage = false;
        }

        internal void ChangeLanguageFunc(Control value)
        {
            if (!CurrentExclude.IsValid(value))
                return;
            //主窗体碰到动态窗体时不再翻译，避免重复
            if (!IsOperatingDynamicForm && IsDynamicForm(value))
                return;

            ChangeLanguageControl(value);

            if (value is ToolStrip)
                return;
            foreach (Control item in value.Controls)
            {
                ChangeLanguageFunc(item);
            }

            (value as ILanguageForm)?.ChangeLanguage();
        }
        private void ChangeLanguageControl(Control value)
        {
            if (!_oper.ChangeLanguage(value))
            {
                if (GetSourceText(value.GetHashCode(), out string[] texts))
                    value.Text = TranslateText(texts[0]);
            }
        }
        internal bool GetSourceText(int hash, out string[] texts)
        {
            return CurrentSourceDict.TryGetValue(hash, out texts);
        }
        #endregion

        #region dynamic form
        private Dictionary<int, string[]> _currentSourceDict;
        //用于切换主窗体和动态窗体的SourceDict
        internal Dictionary<int, string[]> CurrentSourceDict
        {
            get => _currentSourceDict == null ? _sourceDict : _currentSourceDict;
            set => _currentSourceDict = value;
        }
        //用于切换主窗体和动态窗体的Exclude
        private ExcludeManager _currentExclude;
        internal ExcludeManager CurrentExclude
        {
            get => _currentExclude == null ? Exclude : _currentExclude;
            set => _currentExclude = value;
        }
        //是否正在执行动态窗体
        private bool IsOperatingDynamicForm => _currentExclude != null;

        //动态窗体字典 [form_hash, DynamicFormManager]
        private Dictionary<int, DynamicFormManager> _dynamicFormDict = new Dictionary<int, DynamicFormManager>();

        public DynamicFormManager InitDynamicForm(Form value)
        {
            DynamicFormManager m = new DynamicFormManager(this, value);
            _dynamicFormDict[value.GetHashCode()] = m;
            value.FormClosed += DynamicFormClosed;

            return m;
        }
        public DynamicFormManager InitDialog(Form value)
        {
            return new DynamicFormManager(this, value);
        }

        private void DynamicFormClosed(object sender, FormClosedEventArgs e)
        {
            _dynamicFormDict.Remove(sender.GetHashCode());
        }
        private bool IsDynamicForm(Control value)
        { 
            return (value is Form) && _dynamicFormDict.ContainsKey(value.GetHashCode());
        }
        private void ChangeLanguageAllDynamicForm()
        {
            foreach (DynamicFormManager item in _dynamicFormDict.Values)
            {
                item.ChangeLanguage();
            }
        }
        #endregion
    }
}
