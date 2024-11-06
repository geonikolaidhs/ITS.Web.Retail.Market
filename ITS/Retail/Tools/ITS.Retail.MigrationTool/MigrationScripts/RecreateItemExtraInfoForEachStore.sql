--select * from "ItemExtraInfoOld";

--select "Oid" from "Store";

--drop table "ItemExtraInfoOld";

--Insert Fields into ItemExtraInfo from ItemExtraInfoOld
select "Oid", "CreatedOnTicks", "UpdatedOnTicks", "CreatedBy", "UpdatedBy", 
							   "CreatedByDevice", "UpdateByDevice", "RowDeleted", "IsActive", 
							   "IsSynchronized", "MLValues", "MasterObjOid", "ReferenceId", 
							   "ObjectSignature", "Description", "Ingredients", "PackedAt", 
							   "ExpiresAt", "Item", "OptimisticLockField", 
							   "GCRecord" from "ItemExtraInfo";
insert into "ItemExtraInfo" ("Oid", "CreatedOnTicks", "UpdatedOnTicks", "CreatedBy", "UpdatedBy", 
							   "CreatedByDevice", "UpdateByDevice", "RowDeleted", "IsActive", 
							   "IsSynchronized", "MLValues", "MasterObjOid", "ReferenceId", 
							   "ObjectSignature", "Description", "Ingredients", "PackedAt", 
							   "ExpiresAt", "Item", "OptimisticLockField", 
							   "GCRecord")
	    select "Oid", "CreatedOnTicks", "UpdatedOnTicks", "CreatedBy", "UpdatedBy", 
							   "CreatedByDevice", "UpdateByDevice", "RowDeleted", "IsActive", 
							   "IsSynchronized", "MLValues", "MasterObjOid", "ReferenceId", 
							   "ObjectSignature", "Description", "Ingredients", "PackedAt", 
							   "ExpiresAt", "Item", "OptimisticLockField", 
							   "GCRecord" from "ItemExtraInfoOld";
--Insert Stores into ItemExtraInfo
update "ItemExtraInfo" set "Store" = "StoreControllerSettings"."Store" from "StoreControllerSettings";

--Postgres Create CSV data file
COPY (select 'Insert into "ItemExtraInfo" ("Oid", "CreatedOnTicks", "UpdatedOnTicks", "CreatedBy", "UpdatedBy", 
					"CreatedByDevice", "UpdateByDevice", "RowDeleted", "IsActive", 
					"IsSynchronized", "MLValues", "MasterObjOid", "ReferenceId", 
					"ObjectSignature", "Description", "Ingredients", "PackedAt", 
					"ExpiresAt", "Item","Origin", "Lot", "Store" "OptimisticLockField", 
					"GCRecord" )
			values ('||"Oid"||',' ||"CreatedOnTicks"||','|| "UpdatedOnTicks"||','|| coalesce("CreatedBy",'00000000-0000-0000-0000-000000000000')||','||
			 coalesce("UpdatedBy",'00000000-0000-0000-0000-000000000000')||','||coalesce("CreatedByDevice",'00000000-0000-0000-0000-000000000000')||','||
			  coalesce("UpdateByDevice",'00000000-0000-0000-0000-000000000000')||','|| "RowDeleted"||','|| "IsActive"||','|| 
					"IsSynchronized"||','|| coalesce("MLValues",'')||','|| coalesce("MasterObjOid",'00000000-0000-0000-0000-000000000000')||','||
					 coalesce("ReferenceId",'')||','||coalesce("ObjectSignature",'')||','|| "Description"||','|| coalesce("Ingredients",'')||','||
					 -- "PackedAt"||','|| "ExpiresAt"||','||
					   "Item"||','||coalesce("Origin",'')||','||
					  coalesce("Lot",'')||','||"Store"||','|| "OptimisticLockField"||','|| 
					--coalesce("GCRecord",NULL)||
					');' from "ItemExtraInfo") TO 'C:/git/ItemExtraInfo.txt' with NULL as 'NULL';

--SQL Server Create CSV data file
:setvar username  sa
:setvar password 123456
:setvar server ATS-DEV\SQLSERVER14
:setvar sqlpath C:\Users\min\Desktop\myscript.sql
:setvar outputpath  C:\Users\min\Documents\Test.txt

!!sqlcmd -U $(username) -P $(password) -S $(server) -i $(sqlpath) -o $(outputpath)
