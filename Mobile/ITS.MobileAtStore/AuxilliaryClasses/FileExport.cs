using System;

using System.Collections.Generic;
using System.Text;
using ITS.MobileAtStore.ObjectModel;

namespace ITS.MobileAtStore.AuxilliaryClasses
{    

    public class FileExport
    {
        public int Id { get; set; }
        public string FullPath { get; set; }
        public string Description { get; set; }
        //public List<DOC_TYPES> ApplicableDocTypes { get; private set; }
        /*protected FileExport()
        {
        }*/
        static public FileExport Database()
        {
            return new FileExport() { Id = -1, FullPath = "", Description = ""
                //,ApplicableDocTypes = new List<DOC_TYPES>() 
            };
        }

        public FileExport(/*WRMMobileAtStore.FileExportLocation fileExport*/)
        {
           /* this.Id = fileExport.Id;
            this.FullPath = fileExport.FullPath;
            char[] trimCharacters = new char[]{' ', '\r', '\n'};
            this.Description = fileExport.Descriprion.TrimStart(trimCharacters).TrimEnd(trimCharacters);*/
            //ApplicableDocTypes = new List<DOC_TYPES>();
            //foreach (var v in fileExport.ApplicableDocTypes)
            //{
            //    ApplicableDocTypes.Add((DOC_TYPES)((int)v));
            //}
        }

        public override string ToString()
        {
            return this.Description;
        }

        
    }
}
