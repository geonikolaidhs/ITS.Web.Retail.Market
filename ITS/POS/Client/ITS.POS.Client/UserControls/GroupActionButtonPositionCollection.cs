using ITS.POS.Client.Helpers;
using System.Collections.Generic;
using System.ComponentModel;

namespace ITS.POS.Client.UserControls
{
    [TypeConverterAttribute(typeof(ExpandableObjectConverter))]
    public class GroupActionButtonPositionCollection : List<GroupActionButtonPosition>
    {

    }
}
