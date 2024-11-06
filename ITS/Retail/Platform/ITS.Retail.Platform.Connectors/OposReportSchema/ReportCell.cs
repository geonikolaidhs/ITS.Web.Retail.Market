using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Common.OposReportSchema
{
    public class ReportCell
    {
        public ReportCell() { }

        public ReportCell(string content)
        {
            this.Content = content;
            CellAlignment = eAlignment.LEFT;
        }

        public ReportCell(string content, eAlignment alignment)
        {
            this.Content = content;
            CellAlignment = alignment;
        }

        public eAlignment CellAlignment { get; set; }
        public string Content { get; set; }
        public uint MaxCharacters { get; set; } = 0;
    }
}
