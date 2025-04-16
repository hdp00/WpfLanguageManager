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
using System.Windows;
using System.Windows.Controls;
using System.Reflection;
using System.Windows.Media;

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
        public void InitLanguageSelectComboBox(Window main, ComboBox comboBox)
        { 
            new LanguageSelectCombox(this, main, comboBox);
        }

        public string GetControlText(object item)
        {
            switch(item)
            {
                case TextBox textBox:
                    {
                        if (textBox.Text is string s)
                            return s;
                    }
                    break;
                case TextBlock textBlock:
                    {
                        if (textBlock.Text is string s)
                            return s;
                    }
                    break;
                case HeaderedItemsControl header:
                    {
                        if (header.Header is string s)
                            return s;
                    }
                    break;
                case ContentControl content:
                    { 
                        if (content.Content is string s)
                            return s;
                    }
                    break;
                default:
                    break;
            }

            return null;
        }
        public void SetControlText(object item, string text)
        {
            switch (item)
            {
                case TextBox textBox:
                    {
                        if (textBox.Text is string s)
                            textBox.Text = s;
                    }
                    break;
                case TextBlock textBlock:
                    {
                        if (textBlock.Text is string s)
                            textBlock.Text = s;
                    }
                    break;
                case HeaderedItemsControl header:
                    {
                        if (header.Header is string s)
                            header.Header = s;             
                    }
                    break;
                case ContentControl content:
                    {
                        if (content.Content is string s)
                            content.Content = s;
                    }
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region private function
        private void Init()
        {
            LoadTranslateData();
        }
        private IEnumerable<T> FindVisualChildren<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) yield break;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T t)
                {
                    yield return t;
                }
            }
        }
        #endregion

        #region translate data. 翻译数据的读取/保存
        private string TranslateFileName => Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Translate.json");
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
        public void CollectText(FrameworkElement value)
        {
            CollectTextFunc(value);
        }
        private void CollectTextFunc(FrameworkElement value)
        {
            if (!CurrentExclude.IsValid(value))
                return;

            CollectTextControl(value);

            foreach (FrameworkElement item in FindVisualChildren<FrameworkElement>(value))
            {
                CollectTextFunc(item);
            }
        }
        private void CollectTextControl(FrameworkElement value)
        {
            if (!_oper.CollectText(value))
            {
                FillTranslateDict(GetControlText(value));
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
        public void InitLanguage(FrameworkElement value)
        {
            InitLanguageFunc(value);
        }

        internal void InitLanguageFunc(FrameworkElement value)
        {
            if (!CurrentExclude.IsValid(value))
                return;
            if (!IsOperatingDynamicForm && IsDynamicForm(value))
                return;

            InitLanguageControl(value);

            foreach (Control item in value.Controls)
            {
                InitLanguageFunc(item);
            }
        }
        private void InitLanguageControl(FrameworkElement value)
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
        public void ChangeLanguage(FrameworkElement value)
        {
            _isChangingLanguage = true;

            ChangeLanguageFunc(value);
            ChangeLanguageAllDynamicForm();

            _isChangingLanguage = false;
        }

        internal void ChangeLanguageFunc(FrameworkElement value)
        {
            if (!CurrentExclude.IsValid(value))
                return;
            //主窗体碰到动态窗体时不再翻译，避免重复
            if (!IsOperatingDynamicForm && IsDynamicForm(value))
                return;

            ChangeLanguageControl(value);

            foreach (Control item in value.Controls)
            {
                ChangeLanguageFunc(item);
            }

            (value as ILanguageForm)?.ChangeLanguage();
        }
        private void ChangeLanguageControl(FrameworkElement value)
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

        public DynamicFormManager InitDynamicForm(Window value)
        {
            DynamicFormManager m = new DynamicFormManager(this, value);
            _dynamicFormDict[value.GetHashCode()] = m;
            value.FormClosed += DynamicFormClosed;

            return m;
        }
        public DynamicFormManager InitDialog(Window value)
        {
            return new DynamicFormManager(this, value);
        }

        private void DynamicFormClosed(object sender, FormClosedEventArgs e)
        {
            _dynamicFormDict.Remove(sender.GetHashCode());
        }
        private bool IsDynamicForm(Control value)
        { 
            return (value is Window) && _dynamicFormDict.ContainsKey(value.GetHashCode());
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
