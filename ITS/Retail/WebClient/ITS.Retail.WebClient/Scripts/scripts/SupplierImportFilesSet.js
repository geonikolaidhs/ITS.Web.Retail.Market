var SupplierImportFilesSet = {
    SupplierImportFileRecordFieldGrid_OnBeginCallback: function (s, e) {
        e.customArgs.SupplierImportFileRecordHeaderOid = $('#SupplierImportFileRecordHeaderOid').val();
        e.customArgs.SupplierImportFileRecordFieldOid = s.deleteKeyValue;
        e.customArgs.CurrentEntityName = EntityName.GetValue();
    },
    SupplierImportMappingDetailGrid_OnBeginCallback: function (s, e) {
        e.customArgs.SupplierImportMappingHeaderOid = $('#SupplierImportMappingHeaderOid').val();
        e.customArgs.SupplierImportMappingDetailOid = s.deleteKeyValue;
    }
};