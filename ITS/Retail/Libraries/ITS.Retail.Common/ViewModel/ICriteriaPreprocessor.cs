using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITS.Retail.Common.ViewModel
{
    public interface ICriteriaPreprocessor { }

    public interface ICriteriaPreprocessor<T> : ICriteriaPreprocessor
    {
        T[] Preprocess(T input, CriteriaFieldAttribute attribute);
    }
}