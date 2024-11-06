using FluentMigrator;
using ITS.Retail.MigrationTool.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.MigrationTool.MigrationScripts
{
    [RetailMigration(author: "Kostopoulos", year: 2014, month: 1, day: 2, order: 2, version:"2.0.0.67")]
    public class DeleteStorePriceListDuplicates : Migration
    {
        public override void Up()
        {
            long dateTimeNowTicks = DateTime.Now.Ticks;
            string GroupsTable = "Groups_" + dateTimeNowTicks.ToString();
            string CurrentGroupTable = "CurrentGroup_" + dateTimeNowTicks.ToString();

            try
            {

                #region Βήμα 1ο Ενημέρωση CustomerStorePricelist και σβήσιμο από τον StorePriceList
                //πονάνε τα μυαλά μας...
                String sql_query = @"
DECLARE @current_group int = 0
DECLARE @groups int = 0
DECLARE @current_group_oid uniqueidentifier = CAST(CAST(0 AS BINARY) AS UNIQUEIDENTIFIER)

--step 1
	SELECT  a.Oid, a.Store,a.PriceList,DENSE_RANK() OVER (ORDER BY a.Store,a.PriceList) as grp
	into "+GroupsTable+
	@" FROM   StorePriceList As [a]
	INNER JOIN 
	(
		SELECT Store
			 , PriceList
		FROM   StorePriceList 
		GROUP
			BY Store
			, PriceList
		HAVING Count(*) >1
	) As [b]
    ON a.Store = b.Store AND a.PriceList = b.PriceList
 
--step 2
	set @groups = (select MAX(grp) from "+GroupsTable+@");

-- step 3
WHILE @current_group < @groups BEGIN
    SET @current_group = @current_group + 1;
    --step 3.1
		select *
			into "+CurrentGroupTable+
			" from "+GroupsTable+
            @" where grp = @current_group;
    --step 3.2
		set @current_group_oid = (select top 1 Oid from "+CurrentGroupTable+ @");
    --step 3.3
		update CustomerStorePriceList
			set StorePriceList = @current_group_oid
			where StorePriceList in (select Oid from " + CurrentGroupTable + @");
	--step 3.4
		delete from StorePriceList
			where Oid in (select Oid from " + CurrentGroupTable + @")
			and Oid != @current_group_oid;
    --step 3.5
		drop table " + CurrentGroupTable + @";
END 
";

                Execute.Sql(sql_query);
                #endregion

                #region Βήμα 2ο Καθαρισμός προσωρινών πινάκων
                Delete.Table(GroupsTable);
                #endregion
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }

    }
}
