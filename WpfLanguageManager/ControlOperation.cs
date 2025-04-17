//控件操作类
//by hdp 2025.01.01
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MultiLanguage
{
    #region basic

    public class ControlOperationManager
    {
        public ControlOperationManager(LanguageManager container)
        {
            _container = container;
        }

        #region property
        public ItemsOperation Items => new ItemsOperation(_container);
        #endregion

        #region field
        private readonly LanguageManager _container;
        #endregion

        #region public function
        public bool CollectText(FrameworkElement value)
        {
            if (value is ItemsControl c)
                Items.CollectText(c);
            else
                return false;
            return true;
        }
        public bool InitLanguage(FrameworkElement value)
        {
            if (value is ItemsControl c)
                Items.InitLanguage(c);
            else
                return false;
            return true;
        }
        public bool ChangeLanguage(FrameworkElement value)
        {
            if (value is ItemsControl c)
                Items.ChangeLanguage(c);
            else
                return false;
            return true;
        }
        #endregion
    }
    public class ControlOperation
    {
        public ControlOperation(LanguageManager container)
        {
            Container = container;
        }

        public readonly LanguageManager Container;
    }
    #endregion

    #region items
    public class ItemsOperation : ControlOperation
    {
        public ItemsOperation(LanguageManager container) : base(container) { }

        #region collect text
        public void CollectText(ItemsControl value)
        {
            CollectTextItem(value);
        }
        private void CollectTextItem(object value)
        {
            if (value is string s)
            {
                Container.FillTranslateDict(s);
            }
            else
            {
                if (!Container.Exclude.IsValid(value))
                    return;

                if (value is Selector selector)
                {
                    if (selector.ItemsSource == null)
                    {
                        foreach (object item in selector.Items)
                        {
                            if (item is string itemText)
                                Container.FillTranslateDict(itemText);
                        }
                    }
                }
                else
                    Container.FillTranslateDict(Container.GetControlText(value));

                if (value is ItemsControl items)
                {
                    foreach (object item in items.Items)
                    {
                        CollectTextItem(item);
                    }
                }
            }
        }
        #endregion

        #region init language
        public void InitLanguage(ItemsControl value)
        {
            InitLanguageItem(value);
        }
        private void InitLanguageItem(object value)
        {
            if (!(value is FrameworkElement))
                return;
            if (!Container.Exclude.IsValid(value))
                return;

            //list Items中包含控件，也包含string
            if (value is Selector selector)
            {
                if (selector.ItemsSource == null)
                {
                    List<string> strList = new List<string>();
                    foreach (object item in selector.Items)
                    {
                        if (item is string s && !string.IsNullOrWhiteSpace(s))
                            strList.Add(s);
                        else
                            strList.Add(null);
                    }

                    if (strList.Exists(x => x != null))
                        Container.FillSourceDict(value.GetHashCode(), strList.ToArray());
                }
            }
            else
                Container.FillSourceDict(value.GetHashCode(), Container.GetControlText(value));

            if (value is ItemsControl items)
            {
                foreach (object item in items.Items)
                {
                    InitLanguageItem(item);
                }
            }
        }
        #endregion

        #region change language
        public void ChangeLanguage(ItemsControl value)
        {
            ChangeLanguageItem(value);
        }
        private void ChangeLanguageItem(object value)
        {
            if (!(value is FrameworkElement))
                return;
            if (!Container.Exclude.IsValid(value))
                return;

            //list Items中包含控件，也包含string
            if (value is Selector selector)
            {
                if (selector.ItemsSource == null)
                {
                    if (Container.GetSourceText(value.GetHashCode(), out string[] texts))
                    {
                        int count = Math.Min(selector.Items.Count, texts.Length);
                        for (int i = 0; i < count; i++)
                        {
                            string s = Container.TranslateText(texts[i]);
                            if (!string.IsNullOrWhiteSpace(s) && selector.Items[i] is string)
                                selector.Items[i] = s;
                        }
                    }
                }
            }
            else
            {
                if (Container.GetSourceText(value.GetHashCode(), out string[] texts))
                    Container.SetControlText(value, texts);
            }

            if (value is ItemsControl items)
            {
                foreach (object item in items.Items)
                {
                    ChangeLanguageItem(item);
                }
            }
        }
        #endregion
    }
    #endregion
}
