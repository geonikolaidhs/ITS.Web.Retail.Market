using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Common.OposReportSchema
{
    public class ReportLine
    {

        public ReportLine() { }

        public ReportLine(ReportCell cell)
        {
            this.Cells.Add(cell);
        }

        public ReportLine(ReportCell cell1, ReportCell cell2)
        {
            this.Cells.Add(cell1);
            this.Cells.Add(cell2);
        }
        public ReportLine(ReportCell cell1, ReportCell cell2, ReportCell cell3)
        {
            this.Cells.Add(cell1);
            this.Cells.Add(cell2);
            this.Cells.Add(cell3);
        }

        public ReportLine(List<ReportCell> cells)
        {
            this.Cells = cells;
        }

        public ReportLine(string content)
        {
            ReportCell cell = new ReportCell(content);
            Cells.Add(cell);
        }

        public ReportLine(string content, eAlignment alignment)
        {
            ReportCell cell = new ReportCell(content, alignment);
            Cells.Add(cell);
        }


        public List<ReportCell> Cells { get; set; } = new List<ReportCell>();

        public uint MaxCharacters { get; set; } = 42;

        public string BuildReportLine()
        {
            if (this.Cells.Count == 0)
            {
                return "";
            }

            string finalLine = "";
            int cellLength = (int)this.MaxCharacters / this.Cells.Count;
            int remainingLineLength = (int)this.MaxCharacters;
            int remainingCells = this.Cells.Count;

            foreach (ReportCell cell in this.Cells)
            {
                string cellString = cell.Content;

                int totalCellSpace = cell.MaxCharacters == 0 ? remainingLineLength / remainingCells : (int)cell.MaxCharacters;
                totalCellSpace = totalCellSpace > remainingLineLength ? remainingLineLength : totalCellSpace;
                cellString = new string(cellString.Take(cellString.Length > totalCellSpace ? totalCellSpace : cellString.Length).ToArray());



                int pad = totalCellSpace - cellString.Length;

                switch (cell.CellAlignment)
                {
                    case eAlignment.CENTER:
                        cellString = PadCenter(cellString, totalCellSpace, ' ');
                        break;
                    case eAlignment.RIGHT:
                        cellString = PadToLeft(cellString, pad, ' ');
                        break;
                    case eAlignment.LEFT:
                        cellString = PadToRight(cellString, pad, ' ');
                        break;
                }
                remainingLineLength = remainingLineLength - totalCellSpace;
                remainingCells--;
                finalLine += cellString;

            }
            return finalLine;
        }


        private string PadCenter(string s, int width, char c)
        {
            if (s == null || width <= s.Length) return s;
            int padchars = (width - s.Length) / 2;

            s = PadToLeft(s, padchars, c);
            s = PadToRight(s, padchars, c);
            return s;
        }



        private string PadToLeft(string s, int pad, char c)
        {
            for (int i = 0; i < pad; i++)
                s = c + s;
            return s;
        }

        private string PadToRight(string s, int pad, char c)
        {
            for (int i = 0; i < pad; i++)
                s = s + c;
            return s;
        }
    }
}
