//排除/包含需要翻译的控件
//by hdp 2025.01.02
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MultiLanguage
{
    public class ExcludeManager
    {
        #region property
        //排除类
        public ExcludeClass ExcludeClass { get; set; } = new ExcludeClass();
        #endregion

        #region field
        //包含名称
        private HashSet<string> _includeNameHash = new HashSet<string>();
        //排除名称
        private HashSet<string> _excludeNameHash = new HashSet<string>();
        #endregion

        #region public function
        public bool IsValid(object value)
        {
            if (!ExcludeClass.IsValid(value))
            {
                if (_includeNameHash.Contains(GetName(value)))
                    return true;

                return false;
            }
            if (_excludeNameHash.Contains(GetName(value)))
                return false;

            return true;
        }
        #endregion

        #region private function
        private string GetName(object value)
        {
            if (value is Control)
                return ((Control)value).Name;
            if (value is ToolStripItem)
                return ((ToolStripItem)value).Name;
            if (value is TreeNode)
                return ((TreeNode)value).Name;

            return null;
        }
        #endregion

        #region add include | exclude
        public void AddIncludeName(params string[] names)
        {
            foreach (string name in names)
            {
                if (!string.IsNullOrWhiteSpace(name))
                    _includeNameHash.Add(name);
            }
        }
        public void AddExcludeName(params string[] names)
        {
            foreach (string name in names)
            {
                if (!string.IsNullOrWhiteSpace(name))
                    _excludeNameHash.Add(name);
            }
        }
        #endregion
    }

    public class ExcludeClass
    {
        #region property
        public bool TextBox = false;
        public bool TreeView = false;
        #endregion

        #region public function
        public bool IsValid(object value)
        {
            if (TextBox && value is TextBox)
                return false;
            if (TreeView && value is TreeView)
                return false;

            return true;
        }
        #endregion
    }
}
