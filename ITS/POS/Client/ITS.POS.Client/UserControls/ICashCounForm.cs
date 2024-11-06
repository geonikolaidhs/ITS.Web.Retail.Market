using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.UserControls
{
    public interface ICashCounForm
    {
        void MoveUp();
        void MoveDown();
        void MoveNext();
        void MovePrevious();
        void DeleteCurrentLine();
        void Multiply();
    }
}
