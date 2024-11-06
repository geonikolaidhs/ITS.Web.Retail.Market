using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace ITS.Retail.ReportsLibrary
{
    public class ExcelResult : ActionResult
    {

        private Stream excelStream;
        private string filename;

        public ExcelResult(byte[] excel,string filename)
        {
            excelStream = new MemoryStream(excel);
            this.filename = filename;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            HttpResponseBase response = context.HttpContext.Response;

            response.AddHeader("Content-Disposition", "attachment; filename=" + filename);
            response.ContentType = "application/vnd.ms-excel";
            
            byte[] buffer = new byte[4096];

            while (true)
            {
                int read = this.excelStream.Read(buffer, 0, buffer.Length);
                if (read == 0)
                {
                    break;
                }

                response.OutputStream.Write(buffer, 0, read);
            }

            response.End();

        }
    }
}