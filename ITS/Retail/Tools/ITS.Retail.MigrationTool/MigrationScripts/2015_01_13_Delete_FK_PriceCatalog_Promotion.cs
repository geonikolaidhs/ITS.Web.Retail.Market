﻿using FluentMigrator;
using ITS.Retail.MigrationTool.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.MigrationTool.MigrationScripts
{
    [RetailMigration(author: "sak", year: 2015, month: 01, day: 13, order: 1, version: "2.3.0.32")]
    public class Delete_FK_PriceCatalog_Promotion : Migration
    {
        private Dictionary<string, string> foreignKeys1 = new Dictionary<string, string>()
        {
            {"FK_PriceCatalog_Promotion","PriceCatalog"},
        };


        public override void Up()
        {
            foreach (KeyValuePair<string, string> pair in this.foreignKeys1)
            {
                if (Schema.Table(pair.Value).Constraint(pair.Key).Exists())
                {
                    Delete.ForeignKey(pair.Key).OnTable(pair.Value);
                }
            }
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}