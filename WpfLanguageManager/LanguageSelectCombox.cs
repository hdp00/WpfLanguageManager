//切换语言的下拉框
//by hdp 2025.04.16
using System.Windows;
using System.Windows.Controls;

namespace MultiLanguage
{
    internal class LanguageSelectCombox
    {
        public LanguageSelectCombox(LanguageManager language, Window main, ComboBox comboBox)
        {
            InitLanguageSelectComboBox(language, main, comboBox);
        }

        private void InitLanguageSelectComboBox(LanguageManager language, Window main, ComboBox comboBox)
        {
            ComboBox c = comboBox;

            c.ItemsSource = language.TranslateTypes;
            c.DisplayMemberPath = "Text";
            c.SelectedValuePath = "Value";
            c.SelectedIndex = 0;
            c.SelectionChanged += (sender, e) =>
            {
                if (language.IsChangingLanguage)
                    return;

                language.CurrentLanguage = comboBox.SelectedItem.ToString();
                language.ChangeLanguage(main);
            };
        }
    }
}