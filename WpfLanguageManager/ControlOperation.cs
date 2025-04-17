//控件操作类
//by hdp 2025.01.01
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

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
            if (!(value is Control))
                return;
            if (!Container.Exclude.IsValid(value))
                return;

            //tree || list
            Container.FillSourceDict(value.GetHashCode(), Container.GetControlText(value));

            List<string> strList = new List<string>();
            //Items中包含控件，也包含string
            if (value is ItemsControl items)
            {
                foreach (object item in items.Items)
                {
                    if (item is string s)
                    {
                        if (!string.IsNullOrWhiteSpace(s))
                            strList.Add(s);
                        else
                            strList.Add(null);
                    }
                    else
                    {
                        InitLanguageItem(item);
                        strList.Add(null);
                    }
                }

                //list
                if (strList.Exists(x => x != null))
                    Container.FillSourceDict(value.GetHashCode(), strList.ToArray());
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
            if (!(value is Control))
                return;
            if (!Container.Exclude.IsValid(value))
                return;

            //tree || list
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

                //list
                if (Container.GetSourceText(value.GetHashCode(), out string[] texts))
                {
                    int count = Math.Min(items.Items.Count, texts.Length);
                    for (int i = 0; i < count; i++)
                    {
                        string s = Container.TranslateText(texts[i]);
                        if (!string.IsNullOrWhiteSpace(s) && items.Items[i] is string)
                            items.Items[i] = s;
                    }
                }
            }
        }
        #endregion
    }
    #endregion
}
