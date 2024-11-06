using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using DevExpress.Web;
using DevExpress.Web.ASPxSpreadsheet;

namespace ITS.Retail.WebClient.Helpers
{
    /// <summary>
    /// 
    /// </summary>
    public static class SpreadsheetUtils {
        public static void HideFileTab(ASPxSpreadsheet spreadsheet) {
            spreadsheet.CreateDefaultRibbonTabs(true);
            RemoveRibbonTab(spreadsheet, typeof(SRFileTab));
        }

        /// <summary>
        /// Creates the overview ribbon.
        /// </summary>
        /// <param name="spreadsheet">The spreadsheet.</param>
        public static void CreateOverviewRibbon(ASPxSpreadsheet spreadsheet) {
            spreadsheet.CreateDefaultRibbonTabs(true);
            RemoveRibbonTab(spreadsheet, typeof(SRFileTab));
            RemoveRibbonTab(spreadsheet, typeof(SRPageLayoutTab));
        }

        /// <summary>
        /// Removes the ribbon tab.
        /// </summary>
        /// <param name="spreadsheet">The spreadsheet.</param>
        /// <param name="tabTypeToRemove">The tab type to remove.</param>
        static void RemoveRibbonTab(ASPxSpreadsheet spreadsheet, Type tabTypeToRemove) {
            foreach(RibbonTab tab in spreadsheet.RibbonTabs) {                
                if(tab.GetType() == tabTypeToRemove) {
                    spreadsheet.RibbonTabs.Remove(tab);
                   break;
                }
            }
        }

        /// <summary>
        /// Hides all tabs except file and page layout.
        /// </summary>
        /// <param name="spreadsheet">The spreadsheet.</param>
        public static void HideAllTabsExceptFileAndPageLayout(ASPxSpreadsheet spreadsheet) {
            spreadsheet.CreateDefaultRibbonTabs(true);

            ShowRibbonItemsOnly(spreadsheet,  
                new Type[] { 
                    typeof(SRFilePrintCommand), 
                    typeof(SRPageSetupMarginsCommand), 
                    typeof(SRPageSetupOrientationCommand), 
                    typeof(SRPageSetupPaperKindCommand), 
                    typeof(SRPrintGridlinesCommand),
                    typeof(SRPrintHeadingsCommand)
                }
            );
        }

        public static void ShowOnlyPrintFile(ASPxSpreadsheet spreadsheet){
            spreadsheet.CreateDefaultRibbonTabs(true);
            HideRibbonItems(spreadsheet,
                new Type[]{
                    typeof(SRFileNewCommand),
                    typeof(SRFileOpenCommand),
                    typeof(SRFileSaveCommand),
                    typeof(SRFileSaveAsCommand)
                }
            );
        }

        /// <summary>
        /// Hides the ribbon items.
        /// </summary>
        /// <param name="spreadsheet">The spreadsheet.</param>
        /// <param name="itemTypes">The item types.</param>
        private static void HideRibbonItems(ASPxSpreadsheet spreadsheet, Type[] itemTypes) {
            ProcessRibbonItems(spreadsheet, itemTypes, false);
        }
        /// <summary>
        /// Shows the ribbon items only.
        /// </summary>
        /// <param name="spreadsheet">The spreadsheet.</param>
        /// <param name="itemTypes">The item types.</param>
        private static void ShowRibbonItemsOnly(ASPxSpreadsheet spreadsheet, Type[] itemTypes) {
            ProcessRibbonItems(spreadsheet, itemTypes, true);
        }
        /// <summary>
        /// Processes the ribbon items.
        /// </summary>
        /// <param name="spreadsheet">The spreadsheet.</param>
        /// <param name="targetTypes">The target types.</param>
        /// <param name="removeNotInList">if set to <c>true</c> [remove not in list].</param>
        private static void ProcessRibbonItems(ASPxSpreadsheet spreadsheet, Type[] targetTypes, bool removeNotInList) {
            var groups = new List<RibbonGroup>();
            var items = new List<RibbonItemBase>();
            
            List<RibbonTab> tabs = spreadsheet.RibbonTabs.ToList();
            foreach(RibbonTab tab in tabs) {                
                groups.AddRange(tab.Groups);
            }
            foreach(RibbonGroup group in groups) {
                items.AddRange(group.Items);
            }

            RemoveItemsByTypes(items, targetTypes, removeNotInList);
            RemoveEmptyGroups(groups);
            RemoveEmptyTabs(spreadsheet, tabs);
        }

        /// <summary>
        /// Removes the items by types.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="targetTypes">The target types.</param>
        /// <param name="removeNotInList">if set to <c>true</c> [remove not in list].</param>
        private static void RemoveItemsByTypes(List<RibbonItemBase> items, Type[] targetTypes, bool removeNotInList) {
            var targetItem = new List<RibbonItemBase>();
            var itemsNotInList = new List<RibbonItemBase>();
            GroupItemsByType(items, targetTypes, targetItem, itemsNotInList);

            if(removeNotInList)
                RemoveItems(itemsNotInList);
            else
                RemoveItems(targetItem);
                
        }

        /// <summary>
        /// Groups the type of the items by.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="targetTypes">The target types.</param>
        /// <param name="targetItem">The target item.</param>
        /// <param name="itemsNotInList">The items not in list.</param>
        private static void GroupItemsByType(List<RibbonItemBase> items, Type[] targetTypes, List<RibbonItemBase> targetItem, List<RibbonItemBase> itemsNotInList) {
            foreach(RibbonItemBase item in items) {
                if(targetTypes.Contains(item.GetType()))
                    targetItem.Add(item);
                else
                    itemsNotInList.Add(item);
            }
        }

        /// <summary>
        /// Removes the items.
        /// </summary>
        /// <param name="items">The items.</param>
        private static void RemoveItems(List<RibbonItemBase> items) {
            foreach(RibbonItemBase item in items)
                item.Group.Items.Remove(item);
        }

        /// <summary>
        /// Removes the empty groups.
        /// </summary>
        /// <param name="groups">The groups.</param>
        private static void RemoveEmptyGroups(List<RibbonGroup> groups) {
            foreach(RibbonGroup group in groups) {
                if(group.Items.IsEmpty)
                    group.Tab.Groups.Remove(group);
            }
        }

        /// <summary>
        /// Removes the empty tabs.
        /// </summary>
        /// <param name="spreadsheet">The spreadsheet.</param>
        /// <param name="tabs">The tabs.</param>
        private static void RemoveEmptyTabs(ASPxSpreadsheet spreadsheet, List<RibbonTab> tabs) {
            foreach(RibbonTab tab in tabs) {
                if(tab.Groups.IsEmpty)
                    spreadsheet.RibbonTabs.Remove(tab); 
            }
        }
    }
}
