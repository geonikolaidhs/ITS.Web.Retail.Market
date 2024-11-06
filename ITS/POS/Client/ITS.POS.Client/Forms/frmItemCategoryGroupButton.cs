using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ITS.POS.Client.Helpers;
using ITS.POS.Client.UserControls;
using ITS.POS.Client.Kernel;
using ITS.POS.Model.Master;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.POS.Model.Settings;

namespace ITS.POS.Client.Forms
{
    public partial class frmItemCategoryGroupButton : frmGroupButton
    {
        public frmItemCategoryGroupButton()
        {
            InitializeComponent();
        }

        public void InitializeItemCategories(string itemCategoryCode, ButtonProperties categoryProperties, ButtonProperties itemProperties, int numberOfRows, int numberOfCols, bool useItemCode, bool remainOpenAfterClick)
        {
            PrepareForm(numberOfRows, numberOfCols, remainOpenAfterClick);
            ControlsToShow.AddRange(GetChildItemCategories(itemCategoryCode, categoryProperties, itemProperties, useItemCode));
            ControlsToShow.AddRange(GetChildItems(itemCategoryCode, categoryProperties, itemProperties, useItemCode));
            ShowButtonPage();
        }

        private List<ucButton> GetChildItems(string itemCategoryCode, ButtonProperties categoryProperties, ButtonProperties itemProperties, bool useItemCode)
        {
            IConfigurationManager config = this.Kernel.GetModule<IConfigurationManager>();

            ISessionManager sessionManager = this.Kernel.GetModule<ISessionManager>();
            ItemCategory itemCategory = sessionManager.FindObject<ItemCategory>(new BinaryOperator("Code", itemCategoryCode));
            List<Item> items = GetItems(sessionManager, itemCategory.Oid);
            items = items.Where(it => it != null).ToList();

            Dictionary<Item, MeasurementUnit> itemsWithMeasurementUnit = items.ToDictionary(x => x,
                x => sessionManager.GetObjectByKey<MeasurementUnit>(sessionManager.GetObjectByKey<Barcode>(x.DefaultBarcode).MeasurementUnit(x.Owner)));


            List<ucButton> listToReturn = itemsWithMeasurementUnit.Select(x =>
            x.Value.SupportDecimal ?
            new ucAddWeightedItemButton()
            {
                Barcode = x.Key.Code,
                Name = Guid.NewGuid().ToString(),
                Dock = DockStyle.Fill,
                ButtonText = (useItemCode ? x.Key.Code + " - " : "") + x.Key.NameWithExtraInfo
            } as ucButton
            :
            new ucAddItemButton()
            {
                Barcode = x.Key.Code,
                Name = Guid.NewGuid().ToString(),
                Dock = DockStyle.Fill,
                ButtonText = (useItemCode ? x.Key.Code + " - " : "") + x.Key.NameWithExtraInfo
            } as ucButton).ToList();

            listToReturn.ForEach(x =>
            {
                itemProperties.Apply(x.Button);
                if (this.RemainOpenAfterClick == false)
                {
                    x.Button.Click += Ctrl_Click;
                }
            });
            return listToReturn;
        }

        private static List<Item> GetItems(ISessionManager sessionManager, Guid itemCategoryOid)
        {
            List<Item> resultItems = new List<Item>();
            XPQuery<Item> items = new XPQuery<Item>(sessionManager.GetSession<Item>());
            IQueryable<ItemAnalyticTree> itemAnalyticTrees = new XPQuery<ItemAnalyticTree>(sessionManager.GetSession<ItemAnalyticTree>()).
                Where(x => x.Node == itemCategoryOid).OrderBy(x => x.ShowOrder);//.ThenBy(x => sessionManager.GetObjectByKey<Item>(x.Object).Name);
            foreach (ItemAnalyticTree itemTree in itemAnalyticTrees)
            {
                int order = itemTree.ShowOrder;
                Item item = itemTree.Session.GetObjectByKey<Item>(itemTree.Object);
                if (item != null && !resultItems.Contains(item))
                {
                    resultItems.Add(item);
                }
            }
            return resultItems;
        }

        private List<ucItemCategoryGroupButton> GetChildItemCategories(string itemCategoryCode, ButtonProperties categoryProperties, ButtonProperties itemProperties, bool useItemCode)
        {
            ISessionManager sessionManager = this.Kernel.GetModule<ISessionManager>();
            ItemCategory itemCategory = sessionManager.FindObject<ItemCategory>(new BinaryOperator("Code", itemCategoryCode));

            List<ucItemCategoryGroupButton> listToReturn = itemCategory.ChildCategories.OrderBy(x => x.Description).Select(x => new ucItemCategoryGroupButton()
            {
                ItemCategoryCode = x.Code,
                NumberOfRows = this.NumberOfRows,
                NumberOfColumns = this.NumberOfCols,
                ItemCategoryButtonProperties = categoryProperties,
                DefaultButtonProperties = itemProperties,
                ButtonText = x.Description,
                Name = Guid.NewGuid().ToString(),
                Dock = DockStyle.Fill,
                RemainOpenAfterClick = this.RemainOpenAfterClick
            }).ToList();

            listToReturn.ForEach(x => categoryProperties.Apply(x.Button));
            return listToReturn;
        }
    }
}
