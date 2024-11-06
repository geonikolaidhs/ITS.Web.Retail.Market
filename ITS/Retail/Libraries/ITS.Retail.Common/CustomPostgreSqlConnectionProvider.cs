using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using DevExpress.Data.Filtering;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
using System.Collections.Specialized;

namespace Instashop.Retail.Common
{
    public class CustomPostgreSqlConnectionProvider : PostgreSqlConnectionProvider
    {
        public CustomPostgreSqlConnectionProvider(IDbConnection connection, AutoCreateOption autoCreateOption)
            : base(connection, autoCreateOption)
        {
        }

        public override string FormatFunction(ProcessParameter processParameter, FunctionOperatorType operatorType, params object[] operands)
        {
            return base.FormatFunction(processParameter, operatorType, operands);
        }


        //protected override SelectStatementResult ProcessSelectData(SelectStatement selects)
        //{
        //    return base.ProcessSelectData(selects);
        //}

        //public override SelectedData SelectData(params SelectStatement[] selects)
        //{
        //    return base.SelectData(selects);
        //}

        protected override IDbCommand CreateCommand(Query query)
        {
            IDbCommand result = base.CreateCommand(query);
            return result;
        }

        public override string FormatSelect(string selectedPropertiesSql, string fromSql, string whereSql, string orderBySql, string groupBySql, string havingSql, int skipSelectedRecords, int topSelectedRecords)
        {
            string result = base.FormatSelect(selectedPropertiesSql, fromSql, whereSql, orderBySql, groupBySql, havingSql, skipSelectedRecords, topSelectedRecords);
            return result;
        }

        public override string FormatSelect(string selectedPropertiesSql, string fromSql, string whereSql, string orderBySql, string groupBySql, string havingSql, int topSelectedRecords)
        {
            string result = base.FormatSelect(selectedPropertiesSql, fromSql, whereSql, orderBySql, groupBySql, havingSql, topSelectedRecords);
            return result;
        }

        public override string FormatUnary(UnaryOperatorType operatorType, string operand)
        {
            string result = base.FormatUnary(operatorType, operand);
            return result;
        }

        public override string FormatFunction(FunctionOperatorType operatorType, params string[] operands)
        {
            if (operatorType == FunctionOperatorType.Contains && operands.Length == 2)
            {
                string internalResult = string.Format("(Strpos(lower({0}), lower({1})) > 0)", operands[0], operands[1]);
                return internalResult;
            }
            else if (operatorType == FunctionOperatorType.Custom && operands.Length == 3 && string.Compare(operands[0], "like", true) == 0)
            {
                string internalResult = string.Format("{0} ilike {1}", operands[1], operands[2]);
                return internalResult;
            }
            else if (operatorType == FunctionOperatorType.Contains || operatorType == FunctionOperatorType.Custom)
            {

            }
            string result = base.FormatFunction(operatorType, operands);
            return result;
        }


        public override string FormatBinary(BinaryOperatorType operatorType, string leftOperand, string rightOperand)
        {
            if (BinaryOperatorType.Like == operatorType)
            {
                return String.Format(CultureInfo.InvariantCulture, "{0} ilike {1}", leftOperand, rightOperand);
            }
            else
            {
                return base.FormatBinary(operatorType, leftOperand, rightOperand);
            }
        }

        public override void GetTableSchema(DBTable table, bool checkIndexes, bool checkForeignKeys)
        {
            base.GetTableSchema(table, false, checkForeignKeys);
            if (checkIndexes)
            {
                GetIndexes(table);
            }
        }

        void GetIndexes(DBTable table)
        {
            string schema = ComposeSafeSchemaName(table.Name);
            if (schema == string.Empty) schema = ObjectsOwner;
            string safeTableName = ComposeSafeTableName(table.Name);

            Query query = new Query(@"select indname, indkey_names,col.attnum, indisunique
                                      from (
                                        SELECT  
                                         ind.relname as indname,

                                               trim( both '""' from (unnest(ARRAY(
                                               SELECT pg_get_indexdef(i.indexrelid, k + 1, true)
                                               FROM generate_subscripts(i.indkey, 1) as k
                                               ORDER BY k
                                               )))) as indkey_names,

                                               unnest(ARRAY(
                                               SELECT  k+1
                                               FROM generate_subscripts(i.indkey, 1) as k
                                               ORDER BY k
                                               )) as k1

     
                                        , i.indisunique,ind.oid indoid, n.nspname, tbl.relname
                                        
                                        FROM   pg_index as i
                                        left JOIN   pg_class as ind
                                        ON     ind.oid = i.indexrelid
                                        left JOIN   pg_am as am
                                        ON     ind.relam = am.oid
                                        left join pg_class tbl on i.indrelid = tbl.oid
                                        left join pg_namespace n on tbl.relnamespace = n.oid

                                        ) ex
                                        left join pg_attribute col on indoid = col.attrelid and col.attnum = k1
                                        where relname = @p0 and nspname = @p1
                                        order by indname, col.attnum",
                new QueryParameterCollection(new OperandValue(safeTableName), new OperandValue(schema)), new string[] { "@p0", "@p1" });
            SelectStatementResult data = SelectData(query);
            DBIndex index = null;
            foreach (SelectStatementResultRow row in data.Rows)
            {
                string indexColumn = (string)row.Values[1];
                if (indexColumn.ToUpper().Contains("COALESCE"))
                {
                    indexColumn = indexColumn.Substring(indexColumn.IndexOf('"') + 1);
                    indexColumn = indexColumn.Substring(0, indexColumn.IndexOf('"'));
                }
                if (index == null || index.Name != (string)row.Values[0])
                {
                    StringCollection list = new StringCollection();
                    list.Add(indexColumn);
                    index = new DBIndex((string)row.Values[0], list, (bool)row.Values[3]);
                    table.Indexes.Add(index);
                }
                else
                {
                    index.Columns.Add(indexColumn);
                }
            }
        }

        protected override string GetSqlCreateColumnTypeForGuid(DBTable table, DBColumn column)
        {
            return "uuid";
        }

        public override void CreateIndex(DBTable table, DBIndex index)
        {
            if (index.IsUnique)
            {
                string indexName = this.GetIndexName(index, table);
                bool useBaseFunction = true;
                StringCollection customFields = new StringCollection();
                for (int i = 0; i < index.Columns.Count; i++)
                {
                    DBColumn column = table.Columns.FirstOrDefault(x => x.Name == index.Columns[i]);
                    if (column.IsKey == false)
                    {
                        if (string.Compare(index.Columns[i], "GCRecord", true) == 0)
                        {
                            customFields.Add("COALESCE(\"GCRecord\",-1)");
                            useBaseFunction = false;
                        }
                        else
                        {
                            switch (column.ColumnType)
                            {
                                case DBColumnType.String:
                                    useBaseFunction = false;
                                    customFields.Add("COALESCE(\"" + column.Name + "\",'##null##')");
                                    break;
                                case DBColumnType.Guid:
                                    useBaseFunction = false;
                                    customFields.Add("COALESCE(\"" + column.Name + "\",'" + Guid.Empty + "')");
                                    break;
                                default:
                                    customFields.Add(column.Name);
                                    break;
                            }
                        }
                    }
                    else
                    {
                        customFields.Add(column.Name);
                    }
                }
                if (!useBaseFunction)
                {
                    for (int i = 0; i < index.Columns.Count; i++)
                    {
                        if (customFields[i].Contains("COALESCE(") == false)
                        {
                            customFields[i] = '"' + customFields[i] + '"';
                        }
                    }
                    string createIndexString = String.Format(CultureInfo.InvariantCulture,
                                        CreateIndexTemplate,
                                        "unique",
                                        FormatConstraintSafe(indexName),
                                        FormatTableSafe(table),
                                        StringListHelper.DelimitedText(customFields, ","));
                    ExecuteSqlSchemaUpdate("Index", index.Name, table.Name, createIndexString);
                    return;
                }
            }
            base.CreateIndex(table, index);
        }

        public new static IDataStore CreateProviderFromString(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect)
        {
            IDbConnection connection = new Npgsql.NpgsqlConnection(connectionString);
            objectsToDisposeOnDisconnect = new IDisposable[] { connection };
            return CreateProviderFromConnection(connection, autoCreateOption);
        }

        public new static IDataStore CreateProviderFromConnection(IDbConnection connection, AutoCreateOption autoCreateOption)
        {
            if (connection is Npgsql.NpgsqlConnection)
            {
                return new CustomPostgreSqlConnectionProvider(connection, autoCreateOption);
            }
            else
            {
                return null;
            }
        }

        public new static string GetConnectionString(string server, string userid, string password, string database)
        {
            string pgIntConnString = PostgreSqlConnectionProvider.GetConnectionString(server, userid, password, database);
            string pgFinConnString = pgIntConnString.Replace(PostgreSqlConnectionProvider.XpoProviderTypeString, CustomPostgreSqlConnectionProvider.XpoProviderTypeString);
            return pgFinConnString;
        }

        public new static void Register()
        {

            try
            {
                DataStoreBase.RegisterDataStoreProvider(XpoProviderTypeString, new DataStoreCreationFromStringDelegate(CreateProviderFromString));
            }
            catch (ArgumentException e)
            {
                string errorMessage = e.GetFullMessage();
            }

        }

        public new const string XpoProviderTypeString = "CustomPostgreSql";

    }
}
