//多语言管理类
//by hdp 2025.04.18
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

namespace MultiLanguage
{
    public partial class LanguageManager
    {
        #region property
        private static readonly LanguageManager _instance = new LanguageManager();
        //实例
        public static LanguageManager Instance => _instance;

        #region language index
        //当前语言索引
        private int _currentLanguageIndex = 0;
        //当前语言
        public string CurrentLanguage
        {
            get => TranslateConfig.Types[_currentLanguageIndex].Value;
            set
            {
                int index = Array.FindIndex(TranslateConfig.Types, x => x.Value == value);
                if (index >= 0)
                    _currentLanguageIndex = index;
            }
        }
        internal TranslateTypeInfo[] TranslateTypes => TranslateConfig.Types;
        #endregion

        //标记是否正在切换语言
        private bool _isChangingLanguage = false;
        public bool IsChangingLanguage => _isChangingLanguage;
        //管理不需要翻译的控件
        public ExcludeManager Exclude = new ExcludeManager();
        #endregion

        #region field
        //翻译数据
        private readonly TranslateData _translateData = new TranslateData();
        //翻译配置
        private TranslateConfigInfo TranslateConfig => _translateData.Config;
        //翻译字典
        private Dictionary<string, TranslateDataInfo> TranslateDict => _translateData.Data;
        //<hash, texts> 初始的文本数据
        private readonly Dictionary<int, string[]> _sourceDict = new Dictionary<int, string[]>();
        #endregion

        #region public function
        //初始化
        public void Init(string configFileName)
        {
            _translateData.ConfigFileName = configFileName;
            _translateData.Load();
        }
        //保存翻译数据 -1:保存所有；其他值:保存指定文件
        public void SaveTranslateData(int level) => _translateData.Save(level);
        //翻译文本
        public string TranslateText(string text)
        {
            if (!string.IsNullOrEmpty(text) && TranslateDict.TryGetValue(text, out TranslateDataInfo data))
            {
                string[] texts = data.Texts;
                if (texts?.Length > _currentLanguageIndex && !string.IsNullOrWhiteSpace(texts[_currentLanguageIndex]))
                    return texts[_currentLanguageIndex];
            }

            return text;
        }
        //初始化语言切换控件
        public void InitLanguageSelectComboBox(Window main, ComboBox comboBox)
        {
            new LanguageSelectCombox(this, main, comboBox);
        }
        #endregion

        #region private function

        private IEnumerable<T> FindLogicalChildren<T>(DependencyObject parent, bool IsOperatingMainForm = false) where T : DependencyObject
        {
            if (parent == null) yield break;
            if (!CurrentExclude.IsValid(parent)) yield break;
            if (IsOperatingMainForm && IsDynamicForm(parent as FrameworkElement)) yield break;

            foreach (var child in LogicalTreeHelper.GetChildren(parent))
            {
                if (child is T t)
                {
                    yield return t;
                }
                foreach (var descendant in FindLogicalChildren<T>(child as DependencyObject, IsOperatingMainForm))
                {
                    yield return descendant;
                }
            }
        }
        private bool IsPropertyBound(DependencyObject obj, DependencyProperty property)
        {
            return BindingOperations.IsDataBound(obj, property);
        }
        #endregion

        #region control text
        private string[] GetControlText(object control)
        {
            //所有控件都有text，tooltip
            //list & tree, 还需要添加列表项中的文本
            if (!(control is FrameworkElement framework))
                return null;

            //text, tooltip
            List<string> texts = new List<string> { null, null };

            if (!IsPropertyBound(framework, FrameworkElement.ToolTipProperty))
            {
                if (framework.ToolTip is string s)
                    texts[1] = s;
            }

            switch (control)
            {
                //list
                case Selector selector:
                    if (selector.ItemsSource == null)
                        texts.AddRange(GetItemText(selector));
                    break;
                //tree 
                case HeaderedItemsControl header:
                    if (header.ItemsSource == null && !IsPropertyBound(header, HeaderedItemsControl.HeaderProperty))
                    {
                        if (header.Header is string sHeader)
                            texts[0] = sHeader;

                        texts.AddRange(GetItemText(header));
                    }
                    break;
                case TextBox textBox:
                    if (!IsPropertyBound(textBox, TextBox.TextProperty))
                        texts[0] = textBox.Text;
                    break;
                case TextBlock textBlock:
                    if (!IsPropertyBound(textBlock, TextBlock.TextProperty))
                        texts[0] = textBlock.Text;
                    break;
                case Window window:
                    if (!IsPropertyBound(window, Window.TitleProperty))
                        texts[0] = window.Title;
                    break;
                case HeaderedContentControl headeredContent:
                    if (!IsPropertyBound(headeredContent, HeaderedContentControl.HeaderProperty))
                    {
                        if (headeredContent.Header is string s)
                        texts[0] = s;
                    }
                    break;
                case ContentControl content:
                    if (!IsPropertyBound(content, ContentControl.ContentProperty))
                    {
                        if (content.Content is string s)
                            texts[0] = s;
                    }
                    break;
                default:
                    break;
            }

            return texts.ToArray();
        }
        private void SetControlText(object control, string[] texts)
        {
            //所有控件都有text，tooltip
            //list & tree, 还需要添加列表项中的文本
            if (!(control is FrameworkElement framework))
                return;

            string sTooltip = TranslateText(texts[1]);
            if (!string.IsNullOrWhiteSpace(sTooltip))
            {
                if (!IsPropertyBound(framework, FrameworkElement.ToolTipProperty))
                {
                    if (framework.ToolTip is string)
                        framework.ToolTip = sTooltip;
                }
            }

            //itmes
            switch (control)
            {
                //list
                case Selector selector:
                    if (selector.ItemsSource == null)
                        SetItemText(selector, texts);
                    break;
                //tree 
                case HeaderedItemsControl header:
                    if (header.ItemsSource == null && !IsPropertyBound(header, HeaderedItemsControl.HeaderProperty))
                        SetItemText(header, texts);
                    break;
                default:
                    break;
            }

            string sText = TranslateText(texts[0]);
            if (!string.IsNullOrWhiteSpace(sText))
            {
                switch (control)
                {
                    case TextBox textBox:
                        if (!IsPropertyBound(textBox, TextBox.TextProperty))
                            textBox.Text = sText;
                        break;
                    case TextBlock textBlock:
                        if (!IsPropertyBound(textBlock, TextBlock.TextProperty))
                            textBlock.Text = sText;
                        break;
                    case HeaderedItemsControl header:
                        if (!IsPropertyBound(header, HeaderedItemsControl.HeaderProperty))
                        {
                            if (header.Header is string)
                                header.Header = sText;
                        }
                        break;
                    case Window window:
                        if (!IsPropertyBound(window, Window.TitleProperty))
                            window.Title = sText;
                        break;
                    case HeaderedContentControl headeredContent:
                        if (!IsPropertyBound(headeredContent, HeaderedContentControl.HeaderProperty))
                        {
                            if (headeredContent.Header is string)
                                headeredContent.Header = sText;
                        }
                        break;
                    case ContentControl content:
                        if (!IsPropertyBound(content, ContentControl.ContentProperty))
                        {
                            if (content.Content is string)
                                content.Content = sText;
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        private List<string> GetItemText(ItemsControl value)
        {
            List<string> texts = new List<string>();
            foreach (object item in value.Items)
            {
                if (item is string s)
                    texts.Add(s);
                else
                    texts.Add(null);
            }

            return texts;
        }
        private void SetItemText(ItemsControl value, string[] texts)
        {
            int count = Math.Min(value.Items.Count, texts.Length - 2);
            for (int i = 0; i < count; i++)
            {
                string s = TranslateText(texts[i + 2]);
                if (!string.IsNullOrWhiteSpace(s) && value.Items[i] is string)
                    value.Items[i] = s;
            }
        }
        #endregion

        #region collect text. 收集需要翻译的信息
        public void CollectText(FrameworkElement value, int level = 0)
        {
            CollectTextFunc(value, level);
        }
        private void CollectTextFunc(FrameworkElement value, int level)
        {
            FillTranslateDict(level, GetControlText(value));

            foreach (FrameworkElement item in FindLogicalChildren<FrameworkElement>(value))
            {
                FillTranslateDict(level, GetControlText(item));
            }
        }
        private void FillTranslateDict(int level, params string[] texts)
        {
            foreach (string text in texts)
            {
                if (string.IsNullOrWhiteSpace(text) || TranslateDict.ContainsKey(text))
                    continue;

                TranslateDict[text] = new TranslateDataInfo(level, null);
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
            FillSourceDict(value.GetHashCode(), GetControlText(value));

            foreach (FrameworkElement item in FindLogicalChildren<FrameworkElement>(value, !IsOperatingDynamicForm))
            {
                FillSourceDict(item.GetHashCode(), GetControlText(item));
            }
        }
        private void FillSourceDict(int hash, params string[] texts)
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
            ChangeLanguageControl(value);

            foreach (FrameworkElement item in FindLogicalChildren<FrameworkElement>(value, !IsOperatingDynamicForm))
            {
                ChangeLanguageFunc(item);
            }

            (value as ILanguageForm)?.OnChangeLanguage();
        }
        private void ChangeLanguageControl(FrameworkElement value)
        {
            if (GetSourceText(value.GetHashCode(), out string[] texts))
                SetControlText(value, texts);
        }
        private bool GetSourceText(int hash, out string[] texts)
        {
            return CurrentSourceDict.TryGetValue(hash, out texts);
        }
        #endregion

        #region dynamic form
        private Dictionary<int, string[]> _currentSourceDict;
        //用于切换主窗体和动态窗体的SourceDict
        internal Dictionary<int, string[]> CurrentSourceDict
        {
            get => _currentSourceDict ?? _sourceDict;
            set => _currentSourceDict = value;
        }
        //用于切换主窗体和动态窗体的Exclude
        private ExcludeManager _currentExclude;
        internal ExcludeManager CurrentExclude
        {
            get => _currentExclude ?? Exclude;
            set => _currentExclude = value;
        }
        //是否正在执行动态窗体
        private bool IsOperatingDynamicForm => _currentExclude != null;

        //动态窗体字典 [form_hash, DynamicFormManager]
        private readonly Dictionary<int, DynamicFormManager> _dynamicFormDict = new Dictionary<int, DynamicFormManager>();

        public DynamicFormManager InitDynamicForm(Window value)
        {
            DynamicFormManager m = new DynamicFormManager(this, value);
            _dynamicFormDict[value.GetHashCode()] = m;
            value.Closed += DynamicFormClosed;

            return m;
        }
        public DynamicFormManager InitDynamicFormLanguage(Window value)
        {
            DynamicFormManager m = InitDynamicForm(value);
            m.InitLanguage();
            m.ChangeLanguage();
            return m;
        }
        public DynamicFormManager InitDialog(Window value)
        {
            return new DynamicFormManager(this, value);
        }

        private void DynamicFormClosed(object sender, EventArgs e)
        {
            _dynamicFormDict.Remove(sender.GetHashCode());
        }
        private bool IsDynamicForm(FrameworkElement value)
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
