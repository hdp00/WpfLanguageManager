//控件操作类
//by hdp 2025.01.01
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
        #endregion

        #region init language
        public void InitLanguage(ItemsControl value)
        {
            InitLanguageItem(value);
        }
        private void InitLanguageItem(object value)
        {
            if (!Container.Exclude.IsValid(value))
                return;

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
            ChangeLanguaeItem(value);
        }
        private void ChangeLanguaeItem(object value)
        {
            if (!Container.Exclude.IsValid(value))
                return;

            if (Container.GetSourceText(value.GetHashCode(), out string[] texts))
                Container.SetControlText(value, texts);

            if (value is ItemsControl items)
            {
                foreach (object item in items.Items)
                {
                    ChangeLanguaeItem(item);
                }
            }
        }
        #endregion
    }
    #endregion
}
