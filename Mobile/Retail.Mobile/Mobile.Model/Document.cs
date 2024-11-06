using System;
using DevExpress.Xpo;
using System.Collections.Generic;
using System.Text;
using Retail.Mobile_Model;
using DevExpress.Data.Filtering;

namespace Retail.Mobile_Model
{


    public enum DOC_STATUS : byte
    {
        OPEN = 0,
        CLOSED = 1,
        EXPORTED = 2
    }

    public class Document
    {
        private DocHead _header;
        public DocHead Header
        {
            get
            {
                return _header;
            }
            set
            {
                _header = value;
            }
        }

        private DocLine _currentLine;
        public DocLine CurrentLine
        {
            get
            {
                return _currentLine;
            }
            set
            {
                _currentLine = value;
            }
        }

        public Document Load()//DOC_TYPES doc_type)
        {
            _header = XpoDefault.Session.FindObject<DocHead>(null);//CriteriaOperator.Parse(string.Format("[DocType] = {0}")));//, (int)doc_type)));
            if (_header == null)
            {
                _header = new DocHead(XpoDefault.Session);
                _header.HeadOid = Guid.NewGuid().ToString();
                //_header.DocType = doc_type;
                _header.Save();
            }
            return this;
        }
    }
}
