using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Common.Communication
{
    public interface IMessage
    {
        String Serialize();
        void Deserialize(String input);

        string ErrorMessage { get; set; }
    }
}
