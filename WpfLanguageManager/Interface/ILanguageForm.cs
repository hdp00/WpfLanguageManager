//翻译窗体接口，在翻译时需要额外处理的窗体实现该接口
//by hdp 2025.03.23
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiLanguage
{
    public interface ILanguageForm
    {
        void ChangeLanguage();
    }
}
