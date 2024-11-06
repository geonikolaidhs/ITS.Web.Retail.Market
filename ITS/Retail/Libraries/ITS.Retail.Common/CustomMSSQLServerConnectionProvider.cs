using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.Data.Filtering;
using System.Collections.Specialized;

namespace ITS.Retail.Common
{
    public class CustomMSSQLServerConnectionProvider : MSSqlConnectionProvider
    {
        public CustomMSSQLServerConnectionProvider(IDbConnection connection, AutoCreateOption autoCreateOption) : base(connection, autoCreateOption)
        {
        }
//        public override string GetSqlCreateColumnFullAttributes(DBTable table, DBColumn column)
//        {
//            string result = GetSqlCreateColumnType(table, column);
//            if (column.IsKey)
//                result += " NOT NULL";
//            else
//                result += " NULL";
//            if (column.IsKey)
//            {
//                if (column.IsIdentity && (column.ColumnType == DBColumnType.Int32 || column.ColumnType == DBColumnType.Int64) && IsSingleColumnPKColumn(table, column))
//                    result += GetIsAzure() ? " IDENTITY" : " IDENTITY NOT FOR REPLICATION";
//                else if (column.ColumnType == DBColumnType.Guid && IsSingleColumnPKColumn(table, column) && !GetIsAzure())
//                    result += " ROWGUIDCOL NONCLUSTERED ";
//            }
//            return result;
//        }
//        bool is2000;
//        bool is2005;
//        bool is2008;
//        bool is2012;
//        bool? isAzure;
//        bool GetIsAzure()
//        {
//            if (!is2000)
//                return false;
//            if (!isAzure.HasValue)
//            {
//                using (IDbCommand c = CreateCommand(new Query("select SERVERPROPERTY('edition')")))
//                {
//                    isAzure = (string)c.ExecuteScalar() == "SQL Azure";
//                }
//            }
//            return isAzure.Value;
//        }
//        void GetPrimaryKey(DBTable table)
//        {
//            string schema = ComposeSafeSchemaName(table.Name);
//            Query query;
//            if (string.IsNullOrEmpty(schema))
//            {
//                string ClusteredValue = "";
//                if (table.Columns.Where(x => x.IsKey && x.ColumnType == DBColumnType.Guid).Count() > 1)
//                {
//                    ClusteredValue = "NONCLUSTERED";
//                }
//                query = new Query(
//                    !is2005 ? @"select c.COLUMN_NAME, COLUMNPROPERTY(OBJECT_ID(c.TABLE_SCHEMA + '.' + c.TABLE_NAME), c.COLUMN_NAME, 'IsIdentity')
//from INFORMATION_SCHEMA.KEY_COLUMN_USAGE c 
//join INFORMATION_SCHEMA.TABLE_CONSTRAINTS p on p.CONSTRAINT_NAME = c.CONSTRAINT_NAME 
//where c.TABLE_NAME = @p1 and p.CONSTRAINT_TYPE = 'PRIMARY KEY'" + ClusteredValue + ""
//                    :
//@"select c.name, COLUMNPROPERTY(t.object_id, c.name, 'IsIdentity') from sys.key_constraints p
// join sys.index_columns i on p.parent_object_id = i.object_id and p.unique_index_id = i.index_id
// join sys.columns c on i.column_id = c.column_id and p.parent_object_id = c.object_id
// join sys.tables t on p.parent_object_id = t.object_id
//where t.name = @p1 and p.type = 'PK'
//order by i.key_ordinal"
//                    , new QueryParameterCollection(new OperandValue(ComposeSafeTableName(table.Name))), new string[] { "@p1" });
//            }
//            else
//                query = new Query(
//                    !is2005 ? @"SELECT
//clmns.name,
//COLUMNPROPERTY(tbl.id, clmns.name, 'IsIdentity')
//FROM
//dbo.sysobjects AS tbl
//INNER JOIN sysusers AS stbl ON stbl.uid = tbl.uid
//INNER JOIN dbo.syscolumns AS clmns ON clmns.id=tbl.id
//LEFT OUTER JOIN dbo.sysindexes AS ik ON ik.id = clmns.id and 0 != ik.status & 0x0800
//LEFT OUTER JOIN dbo.sysindexkeys AS cik ON cik.indid = ik.indid and cik.colid = clmns.colid and cik.id = clmns.id
//WHERE
//(tbl.type='U')and(tbl.name=@p1 and stbl.name=@p2) and cik.colid is not null" :
//                    @"select c.name, COLUMNPROPERTY(t.object_id, c.name, 'IsIdentity') from sys.key_constraints p
// join sys.index_columns i on p.parent_object_id = i.object_id and p.unique_index_id = i.index_id
// join sys.columns c on i.column_id = c.column_id and p.parent_object_id = c.object_id
// join sys.tables t on p.parent_object_id = t.object_id
// join sys.schemas s on s.schema_id = p.schema_id
//where t.name = @p1 and p.type = 'PK' and s.name = @p2
//order by i.key_ordinal"
//                    , new QueryParameterCollection(new OperandValue(ComposeSafeTableName(table.Name)), new OperandValue(schema)), new string[] { "@p1", "@p2" });
//            SelectStatementResult data = SelectData(query);
//            if (data.Rows.Length > 0)
//            {
//                StringCollection cols = new StringCollection();
//                for (int i = 0; i < data.Rows.Length; i++)
//                    cols.Add((string)(data.Rows[i]).Values[0]);
//                table.PrimaryKey = new DBPrimaryKey(cols);
//                foreach (string columnName in cols)
//                {
//                    DBColumn column = table.GetColumn(columnName);
//                    if (column != null)
//                        column.IsKey = true;
//                }
//                if (cols.Count == 1 && ((int)(data.Rows[0]).Values[1]) == 1)
//                    table.GetColumn(cols[0]).IsIdentity = true;
//            }
//        }
    }
}
