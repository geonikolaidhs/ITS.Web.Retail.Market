package gr.net.its.retail;

import gr.net.its.common.AsyncTaskRunnable;
import gr.net.its.common.DialogResult;
import gr.net.its.common.GeneralAsyncTask;
import gr.net.its.common.InputFilterDecimalMinMax;
import gr.net.its.common.InputFilterIntegerMinMax;
import gr.net.its.common.PostExecuteRunnable;
import gr.net.its.common.PreExecuteRunnable;
import gr.net.its.common.Utilities;
import gr.net.its.common.bluetooth.BTScanState;
import gr.net.its.common.bluetooth.BluetoothConnector;
import gr.net.its.common.bluetooth.BluetoothScanningEvent;
import gr.net.its.common.bluetooth.BluetoothScanningEventListener;
import gr.net.its.retail.data.Address;
import gr.net.its.retail.data.ApplicationSettings;
import gr.net.its.retail.data.Barcode;
import gr.net.its.retail.data.BinaryOperator;
import gr.net.its.retail.data.Customer;
import gr.net.its.retail.data.DatabaseHelper;
import gr.net.its.retail.data.DatabaseHelper.ExportDatabaseFileTask;
import gr.net.its.retail.data.DatabaseHelper.ImportDatabaseFileTask;
import gr.net.its.retail.data.DocumentDetail;
import gr.net.its.retail.data.DocumentHeader;
import gr.net.its.retail.data.DocumentStatus;
import gr.net.its.retail.data.DocumentType;
import gr.net.its.retail.data.FilterType;
import gr.net.its.retail.data.InvalidItem;
import gr.net.its.retail.data.Item;
import gr.net.its.retail.data.ItemCategory;
import gr.net.its.retail.data.ItemPrice;
import gr.net.its.retail.data.MeasurementUnit;
import gr.net.its.retail.data.Offer;
import gr.net.its.retail.data.OfferDetail;
import gr.net.its.retail.data.OperatorType;
import gr.net.its.retail.data.PriceCatalog;
import gr.net.its.retail.data.PriceCatalogDetail;
import gr.net.its.retail.data.Store;
import gr.net.its.retail.data.TDocHead;
import gr.net.its.retail.data.User;
import gr.net.its.retail.data.UserStoreAccess;
import gr.net.its.retail.data.VATCategory;
import gr.net.its.retail.data.VATLevel;

import java.io.ByteArrayInputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.sql.SQLException;
import java.text.DecimalFormat;
import java.text.SimpleDateFormat;
import java.util.ArrayDeque;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.Collections;
import java.util.Comparator;
import java.util.Date;
import java.util.GregorianCalendar;
import java.util.HashMap;
import java.util.LinkedList;
import java.util.List;
import java.util.Queue;
import java.util.Set;
import java.util.UUID;

import javax.xml.transform.Source;
import javax.xml.transform.TransformerException;
import javax.xml.transform.stream.StreamSource;

import android.app.ActionBar;
import android.app.ActionBar.Tab;
import android.app.AlertDialog;
import android.app.FragmentTransaction;
import android.app.ProgressDialog;
import android.bluetooth.BluetoothAdapter;
import android.bluetooth.BluetoothDevice;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.IntentFilter;
import android.content.pm.ActivityInfo;
import android.content.pm.PackageInfo;
import android.content.pm.PackageManager.NameNotFoundException;
import android.content.res.Configuration;
import android.graphics.Color;
import android.graphics.drawable.Drawable;
import android.media.AudioManager;
import android.media.ToneGenerator;
import android.net.ConnectivityManager;
import android.os.Bundle;
import android.os.Environment;
import android.os.Handler;
import android.os.Looper;
import android.os.Message;
import android.support.v4.app.FragmentActivity;
import android.text.Editable;
import android.text.InputFilter;
import android.text.InputType;
import android.text.TextWatcher;
import android.util.Log;
import android.util.Xml;
import android.view.KeyEvent;
import android.view.Menu;
import android.view.MenuItem;
import android.view.MotionEvent;
import android.view.View;
import android.view.WindowManager;
import android.view.inputmethod.EditorInfo;
import android.view.inputmethod.InputMethodManager;
import android.webkit.WebView;
import android.widget.AdapterView;
import android.widget.AdapterView.OnItemClickListener;
import android.widget.AdapterView.OnItemSelectedListener;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.CheckBox;
import android.widget.CompoundButton.OnCheckedChangeListener;
import android.widget.DatePicker;
import android.widget.DatePicker.OnDateChangedListener;
import android.widget.CompoundButton;
import android.widget.EditText;
import android.widget.GridView;
import android.widget.ImageButton;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.ListView;
import android.widget.Spinner;
import android.widget.TabHost;
import android.widget.TableLayout;
import android.widget.TableRow;
import android.widget.TextView;
import android.widget.TextView.OnEditorActionListener;

import com.google.zxing.integration.android.IntentIntegrator;
import com.google.zxing.integration.android.IntentResult;
import com.j256.ormlite.dao.Dao;
import com.j256.ormlite.stmt.PreparedQuery;
import com.j256.ormlite.stmt.QueryBuilder;

import android.os.*;

public class MainActivity extends FragmentActivity implements ActionBar.TabListener
{
	// static final variables
	public static final int DEBUG = 1;
	public static String IMAGES_PATH = "";
	public static final String DEFAULT_XSL_PATH = Environment.getExternalStorageDirectory() + "/" + "transform.xsl";
	public static final long UPDATE_CHECKS_TIMESPAN = 86400000;// in
	// milliseconds

	// Gui Elements
	private TabHost mTabHost;
	private View tabLogin, tabSettings;
	private View tabMain, tabItemList;
	private View tabCustomers, tabCustomerInfo;
	private TableLayout filterTable;
	private View tabOrders, tabOrdersTotal, tabOrderByNewItems, tabItemSearchByName, tabItemInfo, tabAbout, tabOrderByOffers, tabCheckPrice;// ,previousTab;
	private Button btnLogin, btnExit, btnSendCurrentOrder, btnSaveSettings, btnRevertSettings, btnShowItems;
	private Button btnExitMenu, btnSettings, btnUpdate, btnUploadAll, btnExportDB, btnImportDB, btnSearchItemByName, btnAbout;
	public Button btnOrders;
	// private Button btnShowItemOffers;
	private ImageButton btnShowWarning, btnInfoShowWarning, btnFilterCategory, btnFilterOffers, btnFilterNewItems, btnInfoFilterCategory;
	private ImageView itemInfoImage;
	private EditText txtCode, txtCheckPriceCode, txtPrice, txtCheckPriceValue, txtQty, txtServerUrl, txtDBImportPath, txtDefaultQty, txtCheckPriceDiscount;
	private EditText txtDescription, txtCheckPriceDescription, txtItemNameSearch, txtAddress, txtPathToXSL;
	private EditText txtItemInfoCode, txtItemInfoBarcode, txtItemInfoDescription, txtItemInfoQty, txtItemInfoGrossTotal, txtItemInfoUnitPrice, txtItemInfoVat;
	private EditText txtItemInfoPackingQty, txtItemInfoMaxOrderQty, txtItemInfoMeasurementUnit, txtItemInfoTotalDiscount, txtPriceCatalog, txtItemInfoVatCategory;
	private Button btnShowNewitems, btnAddNewCustomer, btnInfoShowItems, btnCheckPrice;
	private DatePicker dtNewItemsFrom, dtFinalizeDate, dtFromFilter, dtToFilter;
	private ListView lstCustomers, lstItemCategories, lstItemInCategory, lstNewItems, lstItemsByNameResult, lstDocumentHeaders;
	private ListView docDetailsView, lstOffers, lstItemsInOffer, lstInfoItemCategories;
	private GridView lstInfoItemList;
	private DocumentDetailAdapter docDetailsListAdapter;
	private Context mContext;
	private Spinner cmbDocumentStatus, cmbNewItems, cmbBluetoothDevices, cmbStore, cmbAppSettingsDefaultStore, cmbAppSettingsDefaultPriceCatalog, cmbAppSettingsDefaultDocumentStatus;
	private Spinner cmbStatusFilter, cmbTypeFilter, cmbHasBeenCheckedFilter, cmbHasBeenExecutedFilter;
	private EditText txtCustomerSearchBox;
	private Button btnScan, btnCheckPriceScan, btnLoadHeaders;
	private LinearLayout importLinearLayout;
	private View docHeaderColumns;
	private TextView tvDescription, lblSelectedCategory, tvVersion, lblOffers, lblInfoSelectedCategory, lblOfferHeaderFrom, lblOfferHeaderTo;
	private CheckBox chkBatch, chkAppSettingsDefaultStore, chkAppSettingsDefaultPC, chkAllowMultipleLines, chDefaultAllowMultiLines, chkUseFilters;

	private CustomerCustomAdapter customerListAdapter;

	public boolean isDBInitialized;
	// Menu items
	private MenuItem miSettings;

	// Gui Variables
	private boolean adapterSetted = false, itemCategorySelected = false;
	private ItemCategory selectedCategory;
	//
	private BroadcastReceiver networkStateReceiver;
	private IntentFilter networkStatefilter;

	// Helpers
	public DatabaseHelper dbHelper;
	private WebServiceHelper wsHelper;

	// Database Object Collections
	private Dao<Customer, Long> customerDao;
	private Dao<Item, Long> itemDao;
	private Dao<DocumentHeader, Long> documentHeadersDao;
	private Dao<User, Long> usersDao;

	// Class Variables
	private boolean isQtyPopupShowing = false;
	private User loggedInUser;
	private List<Customer> customerList;
	private List<ItemCategory> itemCategoryList;
	private List<Store> loggedInUserStores;
	private List<Item> itemsInCategoryList, newItemsList, itemsByNameResult;
	private Queue<Item> currentItems = new ArrayDeque<Item>();
	private Queue<Barcode> currentBarcodes = new ArrayDeque<Barcode>();
	private Customer selectedCustomer;
	private Item currentItem;
	private DocumentHeader currentDocumentHeader;
	private String selectedCode;
	private Barcode currentBarcode;
	private DocumentDetail[] docDetailsArray = {};
	private DocumentStatus[] docStatusArray = {};
	private List<PriceCatalog> allPriceCatalogs = null;
	private List<Store> allStores = null;
	public ApplicationSettings appsettings;
	// private String[] strCmbNewItems;// Initialized at InitializeGui()
	private double selectedQty = 1;
	// private ArrayAdapter<Customer> customerAdapter;
	public List<Customer> foundCustomers;
	private List<Offer> customerOffers;
	private View tabItemCategories;
	private List<ItemPrice> itemPriceCategoriesList;
	private List<ItemPrice> itemPriceOffer;
	private List<ItemPrice> newItemPriceList;
	private List<ItemPrice> itemPriceItemsByNameResult;
	private BluetoothConnector bluetoothThread;
	private BluetoothScanningEventListener bluetoothThreadListener;
	private Button btnRestoreDeliveryAddress;
	public Button btnCustomers, btnItems;

	private boolean isOnCustomerView, isOnItemView;
	private TextView offerSortingSource = null;
	private int offerSortingDirection = 0;

	private Handler CustomerSearchHandler;

	final private int customerHandlerDelay = 500;

	private int customerHandleVariableDelay = customerHandlerDelay;

	@Override
	public void onConfigurationChanged(Configuration newConfig)
	{
		super.onConfigurationChanged(newConfig);
		setRequestedOrientation(ActivityInfo.SCREEN_ORIENTATION_LANDSCAPE);
	}

	// This is the event the barcode scanner will trigger after reading the
	// barcode
	@Override
	public void onActivityResult(int requestCode, int resultCode, Intent intent)
	{
		IntentResult scanResult = IntentIntegrator.parseActivityResult(requestCode, resultCode, intent);
		if (tabCheckPrice.getVisibility() == View.VISIBLE)
		{
			txtCheckPriceCode.setText(scanResult.getContents());
			if (!txtCheckPriceCode.getText().toString().equals(""))
			{
				txtCheckPriceCodeOnEnterPressed();
			}
		}
		else
		{
			if (scanResult != null) // TODO
			{
				txtCode.setText(scanResult.getContents());
				if (!txtCode.getText().toString().equals(""))
				{
					SearchItemOffline();
					if (currentItem != null)
					{
						getQtyPopup();
					}
				}
			}
		}
	}

	@Override
	protected void onPause()
	{
		// TODO Auto-generated method stub
		super.onPause();
	}

	@Override
	protected void onResume()
	{
		// TODO Auto-generated method stub
		super.onResume();
	}

	protected void ErrorBeep()
	{
		final ToneGenerator tg = new ToneGenerator(AudioManager.STREAM_NOTIFICATION, 100);
		tg.startTone(ToneGenerator.TONE_PROP_BEEP);
		tg.release();
	}

	@Override
	protected void onSaveInstanceState(Bundle outState)
	{
		// TODO Auto-generated method stub
		super.onSaveInstanceState(outState);
		/*
		 * private User loggedInUser; private List<Customer> customerList;
		 * private Customer selectedCustomer; private Item currentItem; private
		 * DocumentHeader currentDocumentHeader; private String selectedCode;
		 * private DocumentDetail[] docDetailsArray = {}; private
		 * DocumentStatus[] docStatusArray = {};
		 */
		if (loggedInUser != null)
		{
			outState.putString("loggedInUserID", loggedInUser.getRemoteGuid());
			if (selectedCustomer != null)
			{
				outState.putString("selectedCustomer", selectedCustomer.getRemoteGuid());
				if (currentDocumentHeader != null)
				{
					outState.putString("currentDocumentHeader", "" + currentDocumentHeader.getID());
				}
				if (currentItem != null)
				{
					outState.putString("item", currentItem.getRemoteOid());
				}
			}
		}

	}

	public void goBackToCustomersView(boolean resetCustomersList)
	{
		tabLogin.setVisibility(View.GONE);
		tabMain.setVisibility(View.GONE);
		tabOrders.setVisibility(View.GONE);
		tabItemCategories.setVisibility(View.GONE);
		tabOrderByNewItems.setVisibility(View.GONE);
		tabOrdersTotal.setVisibility(View.GONE);
		tabOrderByOffers.setVisibility(View.GONE);
		tabCustomers.setVisibility(View.VISIBLE);
		tabCustomerInfo.setVisibility(View.GONE);
		tabCustomers.performClick();
		txtCode.setText("");
		lblSelectedCategory.setVisibility(View.INVISIBLE);
		lblSelectedCategory.setText("");
		txtItemNameSearch.setText("");
		if (lstItemsByNameResult.getAdapter() != null)
		{
			ItemCustomAdapter adapter = (ItemCustomAdapter) lstItemsByNameResult.getAdapter();
			adapter.clear();
			adapter.notifyDataSetChanged();
			lstItemsByNameResult.invalidateViews();
		}

		if (resetCustomersList)
		{
			ResetCustomerList();
		}
	}

	@Override
	public void onBackPressed()
	{
		if (mTabHost.getCurrentTabView() == tabItemList && selectedCategory != null)
		{
			try
			{
				selectedCategory = dbHelper.getItemCategories().queryForId(selectedCategory.getParent().getID());
			}
			catch (Exception ex)
			{
				selectedCategory = null;
			}
			itemCategoryList = dbHelper.getItemCategoryByParentID(selectedCategory == null ? null : selectedCategory.getID());
			lstInfoItemCategories.setAdapter(new ArrayAdapter<ItemCategory>(this, android.R.layout.simple_list_item_1, itemCategoryList));
			return;
		}
		if (mTabHost.getCurrentTabView() == tabItemCategories && selectedCategory != null)
		{
			try
			{
				selectedCategory = dbHelper.getItemCategories().queryForId(selectedCategory.getParent().getID());
			}
			catch (Exception ex)
			{
				selectedCategory = null;
			}
			itemCategoryList = dbHelper.getItemCategoryByParentID(selectedCategory == null ? null : selectedCategory.getID());
			lstItemCategories.setAdapter(new ArrayAdapter<ItemCategory>(this, android.R.layout.simple_list_item_1, itemCategoryList));
			return;
		}
		if (tabCustomers.getVisibility() == View.VISIBLE)
		{
			tabLogin.setVisibility(View.GONE);
			tabCustomers.setVisibility(View.GONE);
			tabOrders.setVisibility(View.GONE);
			tabOrdersTotal.setVisibility(View.GONE);
			tabOrderByOffers.setVisibility(View.GONE);
			tabMain.setVisibility(View.VISIBLE);
			tabMain.performClick();

		}
		else if (tabOrders.getVisibility() == View.VISIBLE)
		{

			AlertDialog ad = new AlertDialog.Builder(this).create();
			ad.setCancelable(false); // This blocks the 'BACK' button
			ad.setTitle("");
			ad.setMessage(mContext.getString(R.string.exitOrderConfirmation));
			ad.setButton(mContext.getString(android.R.string.cancel), new DialogInterface.OnClickListener()
			{
				public void onClick(DialogInterface dialog, int which)
				{
					dialog.dismiss();
				}
			});
			ad.setButton2(mContext.getString(android.R.string.ok), new DialogInterface.OnClickListener()
			{
				public void onClick(DialogInterface dialog, int which)
				{
					dialog.dismiss();
					goBackToCustomersView(true);
				}
			});
			ad.show();
		}
		else if (tabCustomerInfo.getVisibility() == View.VISIBLE)
		{
			goBackToCustomersView(false);
		}
		else if (tabMain.getVisibility() == View.VISIBLE)
		{
			tabCustomers.setVisibility(View.GONE);
			tabMain.setVisibility(View.GONE);
			tabOrders.setVisibility(View.GONE);
			tabOrdersTotal.setVisibility(View.GONE);
			tabOrderByOffers.setVisibility(View.GONE);
			tabLogin.setVisibility(View.VISIBLE);
			btnExportDB.setVisibility(View.GONE);
			importLinearLayout.setVisibility(View.GONE);
			tabLogin.performClick();
		}
		else if (tabLogin.getVisibility() != View.VISIBLE && tabSettings.getVisibility() == View.VISIBLE)
		{
			if (this.loggedInUser == null)
			{
				ExitApplication();
			}
			tabSettings.setVisibility(View.GONE);
			tabMain.setVisibility(View.VISIBLE);
			tabMain.performClick();
			UpdateSettingsTab();
		}
		else if (tabItemSearchByName.getVisibility() == View.VISIBLE)
		{
			tabItemSearchByName.setVisibility(View.GONE);
			tabItemCategories.setVisibility(View.VISIBLE);
			tabOrdersTotal.setVisibility(View.VISIBLE);
			tabOrderByOffers.setVisibility(View.VISIBLE);
			tabOrderByNewItems.setVisibility(View.VISIBLE);
			tabOrders.setVisibility(View.VISIBLE);

			tabOrders.performClick();
		}
		else if (tabItemInfo.getVisibility() == View.VISIBLE && !isOnItemView)
		{
			tabItemInfo.setVisibility(View.GONE);
			tabItemCategories.setVisibility(View.VISIBLE);
			tabOrdersTotal.setVisibility(View.VISIBLE);
			tabOrderByOffers.setVisibility(View.VISIBLE);
			tabOrderByNewItems.setVisibility(View.VISIBLE);
			tabOrders.setVisibility(View.VISIBLE);
			tabOrders.setVisibility(View.VISIBLE);
			tabOrders.performClick();

		}
		else if (tabItemInfo.getVisibility() == View.VISIBLE && isOnItemView)// &&
		// previousTab
		// ==
		// tabItemList)
		{
			tabItemInfo.setVisibility(View.GONE);
			tabItemList.setVisibility(View.VISIBLE);
			tabItemList.performClick();

		}
		else if (tabAbout.getVisibility() == View.VISIBLE)
		{
			tabAbout.setVisibility(View.GONE);
			tabMain.setVisibility(View.VISIBLE);
			tabMain.performClick();
		}
		else if (tabItemList.getVisibility() == View.VISIBLE)
		{
			tabItemList.setVisibility(View.GONE);
			tabMain.setVisibility(View.VISIBLE);
			tabMain.performClick();
		}
		else if (tabCheckPrice.getVisibility() == View.VISIBLE)
		{
			tabCheckPrice.setVisibility(View.GONE);
			tabCustomerInfo.setVisibility(View.VISIBLE);
			tabCustomerInfo.performClick();
			txtCheckPriceCode.setText("");
			txtCheckPriceValue.setText("");
			txtCheckPriceDiscount.setText("");
			txtCheckPriceDescription.setText("");
		}
		else
		{
			ExitApplication();
		}

	}

	private void ExitApplication()
	{
		AlertDialog ad = new AlertDialog.Builder(this).create();
		ad.setCancelable(false); // This blocks the 'BACK' button
		ad.setTitle("");
		ad.setMessage(getString(R.string.exit_confirmation));
		ad.setButton(getString(android.R.string.cancel), new DialogInterface.OnClickListener()
		{
			public void onClick(DialogInterface dialog, int which)
			{
				dialog.dismiss();
			}
		});
		ad.setButton2(getString(android.R.string.ok), new DialogInterface.OnClickListener()
		{
			public void onClick(DialogInterface dialog, int which)
			{
				dialog.dismiss();
				finish();
				System.exit(0);
			}
		});
		ad.show();

	}

	private void RecalculateQuantities()
	{
		if (itemPriceCategoriesList != null)
		{
			RetailHelper.GetItemsExistingQuantity(itemPriceCategoriesList, currentDocumentHeader, dbHelper);
			lstItemInCategory.setAdapter(new ItemCustomAdapter(mContext, R.layout.item_row, itemPriceCategoriesList));
		}
		if (newItemPriceList != null)
		{
			RetailHelper.GetItemsExistingQuantity(newItemPriceList, currentDocumentHeader, dbHelper);
			lstNewItems.setAdapter(new ItemCustomAdapter(mContext, R.layout.item_row, newItemPriceList));
		}
		if (itemPriceOffer != null) // Items By Offer List
		{
			RetailHelper.GetItemsExistingQuantity(itemPriceOffer, currentDocumentHeader, dbHelper);
			lstItemsInOffer.setAdapter(new ItemCustomAdapter(mContext, R.layout.item_row, itemPriceOffer));
		}
		if (itemPriceItemsByNameResult != null)
		{
			RetailHelper.GetItemsExistingQuantity(itemPriceItemsByNameResult, currentDocumentHeader, dbHelper);
			lstItemsByNameResult.setAdapter(new ItemCustomAdapter(mContext, R.layout.item_row, itemPriceItemsByNameResult, true));
		}

	}

	private List<DocumentDetail> CheckForZeroQuantities(DocumentHeader header) throws SQLException
	{
		List<DocumentDetail> invalidLines = new ArrayList<DocumentDetail>();
		for (DocumentDetail detail : header.getDetails())
		{
			if (detail.getBarcode() == null)
			{
				detail = dbHelper.getDetails().queryForId(detail.getID());
			}

			if (detail.getQty() == 0)
			{
				invalidLines.add(detail);
			}
		}

		return invalidLines;
	}

	private boolean SendOrder(DocumentHeader docHead, boolean verbose, boolean validate)
	{
		// Force reload
		if (docHead.getCustomer() == null)
		{
			try
			{
				docHead = dbHelper.getDocumentHeaders().queryForId(docHead.getID());
			}
			catch (SQLException e)
			{
				e.printStackTrace();
				Utilities.ShowSimpleDialog(mContext, e.getMessage());
				return false;
			}
		}

		if (docHead.getDetails() == null || docHead.getDetails().size() <= 0)
		{
			Utilities.ShowSimpleDialog(mContext, getString(R.string.orderIsEmpty));
			return false;
		}
		List<DocumentDetail> detailsWithZeroQty = null;
		try
		{
			detailsWithZeroQty = CheckForZeroQuantities(docHead);
		}
		catch (SQLException e1)
		{
			e1.printStackTrace();
		}

		if (detailsWithZeroQty == null || detailsWithZeroQty.size() != 0)
		{
			Utilities.ShowSimpleDialog(mContext, getString(R.string.zeroQtysMessage));
			return false;
		}

		String toSend = RetailHelper.PrepareXmlToSend(docHead, dbHelper);
		if (validate)
		{
			List<InvalidItem> invalidItems = wsHelper.ValidateOrder(toSend);
			if (invalidItems != null && invalidItems.size() > 0)
			{
				String message = "<b>" + getString(R.string.invalidOrderMessage) + "</b>:<br/><table width='100%'><tr><td>" + getString(R.string.itemCode) + "</td><td>" + getString(R.string.problem) + "</td></tr><tr><td colspan='2'><hr></td></tr>";
				List<String> invalidItemMessageLines = new ArrayList<String>();
				for (InvalidItem invalidItem : invalidItems)
				{
					String invalidItemMessage = "<td>" + dbHelper.getItemByRemoteGuid(invalidItem.getItemRemoteGuid().toString()).getCode() + "</td>";
					switch (invalidItem.getReason())
					{
					case INACTIVE:
						invalidItemMessage += "<td>" + getString(R.string.itemIsInactive) + "</td>";
						break;
					case NOPRICE:
						invalidItemMessage += "<td>" + getString(R.string.noPrice) + "</td>";
						break;
					}
					invalidItemMessageLines.add(invalidItemMessage);

				}

				for (String line : invalidItemMessageLines)
				{
					message += "<tr>" + line + "</tr>";
				}
				message += "</table>";

				Utilities.showWebViewDialog(this, message, mContext.getResources().getString(R.string.ok));

				return false;
			}
			else if (invalidItems == null)
			{
				Utilities.ShowSimpleDialog(mContext, getString(R.string.noConnectionMessage));
				return false;
			}

		}

		if (wsHelper.SendOrder(toSend))
		{
			try
			{
				dbHelper.getDetails().delete(docHead.getDetails());
				dbHelper.getDocumentHeaders().delete(docHead);
				docHead = null;
				docDetailsArray = null;
				if (verbose)
				{
					AlertDialog ad = new AlertDialog.Builder(this).create();
					ad.setCancelable(false); // This blocks the 'BACK' button
					ad.setMessage(getResources().getString(R.string.orderSentSuccessfully));
					ad.setButton(mContext.getResources().getString(R.string.ok), new DialogInterface.OnClickListener()
					{
						public void onClick(DialogInterface dialog, int which)
						{
							dialog.dismiss();
						}
					});
					ad.show();
				}
				return true;

			}
			catch (Exception e)
			{
				// e.printStackTrace();
				if (verbose)
				{
					AlertDialog ad = new AlertDialog.Builder(this).create();
					ad.setCancelable(false); // This blocks the 'BACK' button
					ad.setMessage(getResources().getString(R.string.orderSendSuccesfullyButLocalNotDel));
					ad.setButton(getString(R.string.ok), new DialogInterface.OnClickListener()
					{
						public void onClick(DialogInterface dialog, int which)
						{
							dialog.dismiss();
						}
					});
					ad.show();
				}
				return true;

			}

		}
		else
		{
			if (verbose)
			{
				AlertDialog ad = new AlertDialog.Builder(this).create();
				ad.setCancelable(false); // This blocks the 'BACK' button
				ad.setMessage(getResources().getString(R.string.orderSendFail));
				ad.setButton(getString(R.string.ok), new DialogInterface.OnClickListener()
				{
					public void onClick(DialogInterface dialog, int which)
					{
						dialog.dismiss();
					}
				});
				ad.show();
			}
			return false;
		}
	}

	private void SearchItem()
	{
		selectedCode = txtCode.getText().toString();
		double price;
		currentItem = wsHelper.GetItemWithBarcode(selectedCode, selectedCustomer.getRemoteGuid());
		if (currentItem != null)
		{
			txtDescription.setText(currentItem.getName());
			price = wsHelper.GetPrice(selectedCustomer, currentItem, selectedCode);
			txtPrice.setText(RetailHelper.euroNormal.format(price));
			if (!txtQty.hasFocus())
			{
				if (!txtQty.requestFocus())
				{
					Log.d("txtQty", "request focus failed");
				}
			}
		}
		else
		{
			Utilities.ShowUninterruptingMessage(mContext, getResources().getString(R.string.itemNotFound), 4);
			// AlertDialog ad = new AlertDialog.Builder(this).create();
			// ad.setCancelable(false); // This blocks the 'BACK' button
			// ad.setMessage(getResources().getString(R.string.itemNotFound));
			// ad.setButton(getString(R.string.ok), new
			// DialogInterface.OnClickListener()
			// {
			// public void onClick(DialogInterface dialog, int which)
			// {
			// txtCode.setText("");
			// dialog.dismiss();
			// }
			// });
			// ad.show();

		}
	}

	private void SearchItemOffline()
	{
		selectedCode = txtCode.getText().toString();
		double price;
		currentItem = dbHelper.getItemByBarcode(selectedCode);
		if (currentItem != null)
		{
			txtDescription.setText(currentItem.getName());
			currentBarcode = dbHelper.getBarcodeByCode(RetailHelper.GetPaddedBarcode(selectedCode, appsettings));
			price = dbHelper.getItemPrice(selectedCustomer.getPc(), currentBarcode, currentItem);
			txtPrice.setText(RetailHelper.euroNormal.format(price));
			/*
			 * if (!txtQty.hasFocus()) { if (!txtQty.requestFocus()) {
			 * Log.d("txtQty", "request focus failed"); } }
			 */
		}
		else
		{
			ErrorBeep();
			Utilities.ShowUninterruptingMessage(mContext, getString(R.string.itemNotFound), 3);
		}
	}

	private void SearchItemCheckPrice()
	{
		selectedCode = txtCheckPriceCode.getText().toString();
		double price;
		currentItem = dbHelper.getItemByBarcode(selectedCode);
		if (currentItem != null)
		{
			txtCheckPriceDescription.setText(currentItem.getName());
			currentBarcode = dbHelper.getBarcodeByCode(RetailHelper.GetPaddedBarcode(selectedCode, appsettings));
			/*
			 * if (!txtQty.hasFocus()) { if (!txtQty.requestFocus()) {
			 * Log.d("txtQty", "request focus failed"); } }
			 */
		}
		else
		{
			ErrorBeep();
			Utilities.ShowUninterruptingMessage(mContext, getString(R.string.itemNotFound), 3);
		}
	}

	private void AddDocumentDetail()
	{
		try
		{
			ArrayList<DocumentDetail> details = wsHelper.GetDocumentDetails(selectedCustomer.getRemoteGuid(), selectedCode, txtQty.getText().toString(), currentItem);
			for (int i = 0; i < details.size(); i++)
			{
				DocumentDetail det = details.get(i);
				det.setHeader(currentDocumentHeader);
				dbHelper.getDetails().create(det);
			}
			UpdateDocumentDetailsView();

			SetupAddNewItem();
		}
		catch (Exception ex)
		{
			ex.printStackTrace();
		}
	}

	private void AddDocumentDetailOffline()
	{
		try
		{
			PriceCatalogDetail pcDetail = dbHelper.getItemPriceDetail(selectedCustomer.getPc(), currentBarcode, currentItem);
			DocumentDetail detail = DocumentHelper.computeDocumentLine(selectedCustomer, currentBarcode, selectedQty, pcDetail.getDiscount(), selectedDiscount, dbHelper, false, appsettings);
			DocumentHelper.AddItem(currentDocumentHeader, detail, dbHelper, appsettings);

			UpdateDocumentDetailsView();

			SetupAddNewItem();

			// RecalculateQuantities();
		}
		catch (Exception ex)
		{
			ex.printStackTrace();
		}
	}

	private void UpdateDocumentDetail(DocumentDetail detail)
	{
		try
		{
			DocumentHelper.ChangeDetail(currentDocumentHeader, detail, selectedQty, selectedDiscount, dbHelper, detail.getLinkedLine() != null, appsettings, mContext);
			UpdateDocumentDetailsView();
			SetupAddNewItem();
		}
		catch (Exception ex)
		{
			ex.printStackTrace();
		}
	}

	public void UpdateDocumentDetailsView()
	{

		try
		{
			currentDocumentHeader = documentHeadersDao.queryForId(currentDocumentHeader.getID());

			docDetailsArray = new DocumentDetail[] {};
			docDetailsArray = currentDocumentHeader.getDetails().toArray(docDetailsArray);
			for (int i = 0; i < docDetailsArray.length; i++)
			{
				docDetailsArray[i].setItem(itemDao.queryForId(docDetailsArray[i].getItem().getID()));
			}
			currentDocumentHeader.UpdateSums();
			((TextView) findViewById(R.id.txtCustomer)).setText("" + selectedCustomer.getCode() + "-" + selectedCustomer.getCompanyName());
			((TextView) findViewById(R.id.txtFinalAmount)).setText(RetailHelper.euroNormal.format(currentDocumentHeader.getGrossTotal()));
			((TextView) findViewById(R.id.txtFinalAmount)).setBackgroundColor(Color.parseColor("#D8F2A9"));

			((TextView) findViewById(R.id.txtTotal)).setText(RetailHelper.euroNormal.format(currentDocumentHeader.getNetTotal()));
			((TextView) findViewById(R.id.txtTotalDiscount)).setText(RetailHelper.euroNormal.format(currentDocumentHeader.getTotalDiscountAmount()));
			((TextView) findViewById(R.id.txtTotalVat)).setText(RetailHelper.euroNormal.format(currentDocumentHeader.getTotalVatAmount()));
			((TextView) findViewById(R.id.txtNetTotal)).setText(RetailHelper.euroNormal.format(currentDocumentHeader.getNetTotalAfterDiscount()));
			((TextView) findViewById(R.id.txtItemsCount)).setText("" + docDetailsArray.length);
			if (appsettings != null && appsettings.isDiscountPermited())
			{
				((Button) findViewById(R.id.btnExtraDiscount)).setVisibility(View.VISIBLE);
			}
			else
			{
				((Button) findViewById(R.id.btnExtraDiscount)).setVisibility(View.GONE);
			}
			txtComments.setText(currentDocumentHeader.getComments() == null || docDetailsArray.length <= 0 ? "" : currentDocumentHeader.getComments());
			txtAddress.setText(currentDocumentHeader.getDeliveryAddress());

			docDetailsListAdapter = new DocumentDetailAdapter(this, R.layout.row, docDetailsArray);
			docDetailsView.setAdapter(docDetailsListAdapter);
			adapterSetted = true;

			if (docDetailsArray.length > 0)
			{
				long ticks = currentDocumentHeader.getDocumentDate();
				long millis = (ticks - 621355968000000000L) / 10000L;
				Date dt = new Date(millis);

				dtFinalizeDate.updateDate(dt.getYear() + 1900, dt.getMonth(), dt.getDate());
			}
			else
			{
				Calendar c = Calendar.getInstance();
				int mYear = c.get(Calendar.YEAR);
				int mMonth = c.get(Calendar.MONTH);
				int mDay = c.get(Calendar.DAY_OF_MONTH);

				dtFinalizeDate.updateDate(mYear, mMonth, mDay);
			}
			txtPriceCatalog.setEnabled(false);
			PriceCatalog pc = selectedCustomer.getPc();
			try
			{
				if (pc.getName() == null)
				{
					pc = dbHelper.getPriceCatalogDao().queryForId(pc.getID());
				}
				txtPriceCatalog.setText(pc.getName());
			}
			catch (Exception ex)
			{
				txtPriceCatalog.setText("");
			}

			RecalculateQuantities();
		}
		catch (Exception ex)
		{
			ex.printStackTrace();
		}
	}

	private void SetupAddNewItem()
	{
		txtCode.setText("");
		txtDescription.setText("");
		txtPrice.setText("");
		txtQty.setText("");
		// txtCode.requestFocus();

		if (tabItemSearchByName.getVisibility() == View.VISIBLE)
		{
			tabItemSearchByName.setVisibility(View.GONE);
			tabItemCategories.setVisibility(View.VISIBLE);
			tabOrdersTotal.setVisibility(View.VISIBLE);
			tabOrderByOffers.setVisibility(View.VISIBLE);
			tabOrderByNewItems.setVisibility(View.VISIBLE);
			tabOrders.setVisibility(View.VISIBLE);
			tabOrders.performClick();
		}
	}

	private void UpdateDocumentStatusComboBoxes()
	{
		int position;
		docStatusArray = new DocumentStatus[] {};
		List<DocumentStatus> filterList = null;
		try
		{

			List<DocumentStatus> list = dbHelper.getDocumentStatuses().queryForAll();
			if (list.size() == 0)
			{
				wsHelper.SynchronizeStatus();
				list = dbHelper.getDocumentStatuses().queryForAll();
			}
			docStatusArray = list.toArray(docStatusArray);
			filterList = list;
			// position =
			// list.indexOf(currentDocumentHeader.getDocumentStatus());
			position = -1;
			if (currentDocumentHeader != null)
			{
				if (currentDocumentHeader.getDetails().size() > 0)
				{
					for (int i = 0; i < list.size(); i++)
					{
						if (currentDocumentHeader.getDocumentStatus().equals(list.get(i)))
						{
							position = i;
							break;
						}
					}
				}
				else
				{
					for (int i = 0; i < list.size(); i++)
					{
						if (appsettings.getDefaultDocumentStatus() != null && appsettings.getDefaultDocumentStatus().getID() == list.get(i).getID())
						{
							position = i;
							break;
						}
					}
				}
			}

		}
		catch (Exception ex)
		{
			position = 0;
			ex.printStackTrace();
		}
		DocumentStatus emptyStatus = new DocumentStatus();
		filterList.add(0, emptyStatus);
		ArrayAdapter<DocumentStatus> docStatusAdapter = new ArrayAdapter<DocumentStatus>(this, android.R.layout.simple_dropdown_item_1line, docStatusArray);
		cmbDocumentStatus.setAdapter(docStatusAdapter);
		cmbDocumentStatus.setSelection(position);
		cmbStatusFilter.setAdapter(new ArrayAdapter<DocumentStatus>(this, android.R.layout.simple_dropdown_item_1line, filterList));
	}

	public List<String> GetBluetoothDevices()
	{
		List<String> deviceNames = new ArrayList<String>();
		Set<BluetoothDevice> devices = BluetoothConnector.GetBondedDevices();
		if (devices == null)
		{
			return null;
		}

		for (BluetoothDevice device : devices)
		{
			deviceNames.add(device.getName());
		}

		return deviceNames;
	}

	public Object UpdateCustomers(boolean forceReDownload)
	{
		try
		{
			// customers need price catalogs and vats. If they are not yet
			// downloaded,
			// redownload them
			List<PriceCatalog> pcs = dbHelper.getPriceCatalogDao().queryForAll();
			if (pcs.size() <= 0)
			{
				Log.i("UpdateCustomers", "Start PriceCatalogs");
				wsHelper.DownloadPriceCatalogs(loggedInUser);
				Log.i("UpdateCustomers", "End PriceCatalogs");
			}

		}
		catch (SQLException e)
		{
			e.printStackTrace();
			// Utilities.ShowSimpleDialog(mContext, e.getMessage(),
			// "Exception checking Price Catalogs");
			return "Exception checking Price Catalogs";
		}

		try
		{
			// customers need price catalogs and vats. If they are not yet
			// downloaded,
			// redownload them
			List<VATLevel> vls = dbHelper.getVATLevelDao().queryForAll();
			if (vls.size() <= 0)
			{
				Log.i("UpdateCustomers", "Start DownloadVATs");
				wsHelper.DownloadVATs(loggedInUser);
				Log.i("UpdateCustomers", "End DownloadVATs");
			}

		}
		catch (SQLException e)
		{
			e.printStackTrace();
			// Utilities.ShowSimpleDialog(mContext, e.getMessage(),
			// "Exception checking VATS");
			return "Exception checking VATS";
		}

		loggedInUserStores = dbHelper.getLocalStoresOfUser(loggedInUser);
//		if (loggedInUserStores.size() <= 0)
//		{
//			Log.i("UpdateCustomers", "Start GetStoresOfUser");
//			loggedInUserStores = wsHelper.GetStoresOfUser(loggedInUser, forceReDownload);
//			Log.i("UpdateCustomers", "End GetStoresOfUser");
			if (loggedInUserStores.size() <= 0)
			{
				return "Logged in user has no stores";
			}
		//}
		customerList = dbHelper.getLocalCustomersOfUser(loggedInUser);
//		if (customerList.size() <= 0)
//		{
//			Log.i("UpdateCustomers", "Start GetCustomersOfUser");
//			customerList = wsHelper.GetCustomersOfUser(loggedInUser, forceReDownload);
//			Log.i("UpdateCustomers", "End GetCustomersOfUser");
			if (customerList.size() <= 0)
			{
				return "Logged in user has no Customers";
			}
//		}

		foundCustomers.clear();
		foundCustomers.addAll(customerList);
		// Collections.sort(foundCustomers, new CustomerComparator(dbHelper));
		CustomerCustomAdapter adapter = new CustomerCustomAdapter(this.mContext, R.layout.customer_row, foundCustomers);
		// adapter.sort(new CustomerComparator(dbHelper));
		customerListAdapter = adapter;
		customerListAdapter.notifyDataSetChanged();
		lstCustomers.invalidateViews();
		// customerListAdapter.notifyDataSetInvalidated();

		return null;
	}

	protected boolean canUserAddOrders(User user)
	{

		List<UserStoreAccess> usas = dbHelper.getLocalUserStoreAccesses(loggedInUser);
		if (usas == null || usas.size() <= 0)
		{
			return false;
		}

		loggedInUserStores = dbHelper.getLocalStoresOfUser(loggedInUser);
		if (loggedInUserStores == null || loggedInUserStores.size() <= 0)
		{
			return false;
		}
		customerList = dbHelper.getLocalCustomersOfUser(loggedInUser);
		if (customerList == null || customerList.size() <= 0)
		{
			return false;
		}
		return true;
	}

//	public void UpdateCustomers()
//	{
//		UpdateCustomers(true);
//	}

	private void PerformLogin()
	{

		String username = ((EditText) findViewById(R.id.txtUsername)).getText().toString();
		String password = ((EditText) findViewById(R.id.txtPassword)).getText().toString();
		boolean Success = false;
		try
		{
			// search for user locally
			User user = dbHelper.findObject(User.class, OperatorType.AND, new BinaryOperator("username", username,FilterType.LIKE), 
					new BinaryOperator("password", password));
			if (user != null)
			{
				loggedInUser = user;
				user.setUpdatedOn(GregorianCalendar.getInstance().getTimeInMillis());
				usersDao.update(user);
			}
			else
			// search online
			{
				String result = wsHelper.Login(username, password);
				Log.i("Login", result);
				// Long userID = Long.fromString(result);
				loggedInUser = null;
				if (!result.equals("00000000-0000-0000-0000-000000000000"))
				{
					user = dbHelper.findObject(User.class, "username", username, FilterType.LIKE);
					if (user == null)
					{
						user = new User();
					}
					user.setUsername(username);
					user.setCreatedOn(GregorianCalendar.getInstance().getTimeInMillis());
					user.setPassword(password);
					user.setUpdatedOn(GregorianCalendar.getInstance().getTimeInMillis());
					user.setRemoteGuid(result);
					usersDao.createOrUpdate(user);
					loggedInUser = user;
				}
			}
		}
		catch (Exception ex)
		{
			ex.printStackTrace();
			Utilities.ShowSimpleDialog(mContext, getString(R.string.noConnectionMessage));
			((MainActivity) mContext).getWindow().clearFlags(WindowManager.LayoutParams.FLAG_KEEP_SCREEN_ON);
			return;
		}
		Success = loggedInUser != null;
		if (Success)
		{
			tabLogin.setVisibility(View.GONE);
			tabSettings.setVisibility(View.GONE);
			tabCustomers.setVisibility(View.GONE);
			tabOrders.setVisibility(View.GONE);
			tabOrdersTotal.setVisibility(View.GONE);
			tabOrderByOffers.setVisibility(View.GONE);
			tabItemCategories.setVisibility(View.GONE);
			tabOrderByNewItems.setVisibility(View.GONE);
			tabItemInfo.setVisibility(View.GONE);
			tabMain.setVisibility(View.VISIBLE);
			tabMain.performClick();
			// importLinearLayout.setVisibility(View.VISIBLE);
			// btnExportDB.setVisibility(View.VISIBLE);
			findViewById(R.id.layoutSettingsForLoggedin).setVisibility(View.VISIBLE);

			if (!isDBInitialized || !canUserAddOrders(loggedInUser))
			{
				btnOrders.setEnabled(false);
				btnCustomers.setEnabled(false);
				btnItems.setEnabled(false);
				if (!isDBInitialized)
				{
					Utilities.ShowUninterruptingMessage(this, getString(R.string.mustSynchronize), 10);
				}
				else
				{
					Utilities.ShowUninterruptingMessage(this, getString(R.string.userHasNoStores), 10);
				}

			}
			else
			{
				btnOrders.setEnabled(true);
				btnCustomers.setEnabled(true);
				btnItems.setEnabled(true);
				foundCustomers.clear();
				foundCustomers.addAll(customerList);
				// Collections.sort(foundCustomers, new
				// CustomerComparator(dbHelper));
				CustomerCustomAdapter adapter = new CustomerCustomAdapter(this.mContext, R.layout.customer_row, foundCustomers);
				// adapter.sort(new CustomerComparator(dbHelper));
				customerListAdapter = adapter;
				customerListAdapter.notifyDataSetChanged();
			}

		}
		else
		{
			AlertDialog ad = new AlertDialog.Builder(this).create();
			ad.setCancelable(false); // This blocks the 'BACK' button
			ad.setMessage(getResources().getString(R.string.invalidLogin));
			ad.setButton(getString(R.string.ok), new DialogInterface.OnClickListener()
			{
				public void onClick(DialogInterface dialog, int which)
				{
					((EditText) findViewById(R.id.txtUsername)).setText("");
					((EditText) findViewById(R.id.txtPassword)).setText("");
					dialog.dismiss();

				}
			});
			ad.show();

		}
	}

	private synchronized void SearchCustomers()
	{

		//if(customerList)
		String filter = txtCustomerSearchBox.getText().toString();

		if (filter.isEmpty())
		{
			foundCustomers.clear();
			if(customerList == null)
			{
				customerList = dbHelper.getLocalCustomersOfUser(loggedInUser);
			}
			foundCustomers.addAll(customerList);
			// Collections.sort(foundCustomers, new
			// CustomerComparator(dbHelper));
			customerListAdapter.notifyDataSetChanged();
			lstCustomers.invalidate();
			lstCustomers.invalidateViews();
		}
		else
		{
			foundCustomers.clear();
			// for (int i = 0; i < customerList.size(); i++)
			// {
			// if
			// (customerList.get(i).getLowerCompanyName().contains(filter.toLowerCase())
			// || customerList.get(i).getCode().contains(filter.toLowerCase())
			// ||
			// customerList.get(i).getTaxCode().contains(filter.toLowerCase()))
			// {
			// foundCustomers.add(customerList.get(i));
			// }
			//
			// }
			foundCustomers.addAll(dbHelper.getLocalCustomersOfUser(loggedInUser, filter));
			// Collections.sort(foundCustomers, new
			// CustomerComparator(dbHelper));
			customerListAdapter.notifyDataSetChanged();
			lstCustomers.invalidate();
			lstCustomers.invalidateViews();
		}

		// Old Algorithm
		// int textLength = txtCustomerSearchBox.getText().length();
		// foundCustomers.clear();
		// if (textLength != 0)
		// {
		// for (int i = 0; i < customerList.size(); i++)
		// {
		// if (textLength <= customerList.get(i).getCompanyName().length() ||
		// textLength <= customerList.get(i).getCode().length() || textLength <=
		// customerList.get(i).getTaxCode().length())
		// {
		// if
		// (customerList.get(i).getCompanyName().toLowerCase().contains(txtCustomerSearchBox.getText().toString().toLowerCase())
		// ||
		// customerList.get(i).getCode().toLowerCase().contains(txtCustomerSearchBox.getText().toString().toLowerCase())
		// ||
		// customerList.get(i).getTaxCode().toLowerCase().contains(txtCustomerSearchBox.getText().toString().toLowerCase()))
		// {
		// foundCustomers.add(customerList.get(i));
		// }
		// }
		// }
		// lstCustomers.setAdapter(new CustomerCustomAdapter(this.mContext,
		// R.layout.customer_row, foundCustomers));
		// }
		// else
		// {
		// lstCustomers.setAdapter(new CustomerCustomAdapter(this.mContext,
		// R.layout.customer_row, customerList));
		// }
	}

	private boolean AddItem(long selectedItemIndex, int type) throws SQLException
	{
		if (type == 0) // Item Category List
			currentItem = itemPriceCategoriesList.get((int) selectedItemIndex).item;
		// itemsInCategoryList.get((int) selectedItemIndex);
		else if (type == 1) // New items List
			currentItem = newItemPriceList.get((int) selectedItemIndex).item;
		// newItemsList.get((int) selectedItemIndex);
		else if (type == 2) // Items By Name List
			currentItem = itemPriceItemsByNameResult.get((int) selectedItemIndex).item;
		else if (type == 3) // Items By Offer List
			currentItem = itemPriceOffer.get((int) selectedItemIndex).item;
		// (Item) lstItemsInOffer.getItemAtPosition((int) selectedItemIndex);
		if (currentItem != null)
		{

			try
			{
				currentBarcode = dbHelper.getBarcodes().queryForId(currentItem.getDefaultBarcodeObject().getID());
			}
			catch (SQLException e)
			{
				e.printStackTrace();
				return false;
			}
			catch (Exception e)
			{
				Utilities.ShowUninterruptingMessage(this, getString(R.string.itemHasNoDefaultBarcode), 5);
				return false;
			}

			double price = dbHelper.getItemPrice(selectedCustomer.getPc(), currentBarcode, currentItem);
			if (price < 0)
			{
				Utilities.ShowUninterruptingMessage(this, getString(R.string.unableToOrderItemNoPCD), 5);
				return false;
			}
			else if (!currentItem.isActive())
			{
				Utilities.ShowUninterruptingMessage(this, getString(R.string.unableToOrderItemInactive), 5);
				return false;
			}

			DocumentDetail existingDetail = DocumentHelper.checkIfBarcodeExists(currentDocumentHeader, currentBarcode, currentItem, dbHelper, appsettings);
			if (existingDetail == null || chkAllowMultipleLines.isChecked())
			{
				getQtyPopup();
			}
			else
			{
				getQtyPopup(existingDetail.getQty(), existingDetail.getSecondDiscount());
			}

			return true;
		}
		else
		{
			Utilities.ShowSimpleDialog(mContext, getResources().getString(R.string.itemNotFound));
			return false;
		}
	}

	private void AddCurrentItem()
	{
		if (currentItem != null)
		{
			txtDescription.setText(currentItem.getName());
			// currentBarcode =
			// dbHelper.getBarcodeByCode(RetailHelper.GetPaddedBarcode(selectedCode,
			// appsettings));
			try
			{
				currentBarcode = dbHelper.getBarcodes().queryForId(currentItem.getDefaultBarcodeObject().getID());
			}
			catch (SQLException e)
			{
				e.printStackTrace();
				return;
			}
			double price = dbHelper.getItemPrice(selectedCustomer.getPc(), currentBarcode, currentItem);
			txtPrice.setText(RetailHelper.euroValue.format(price));
			if (!txtQty.hasFocus())
			{
				if (!txtQty.requestFocus())
				{
					Log.d("txtQty", "request focus failed");
				}
			}
		}
		else
		{
			AlertDialog ad = new AlertDialog.Builder(this).create();
			ad.setCancelable(false); // This blocks the 'BACK' button
			ad.setMessage(getResources().getString(R.string.itemNotFound));
			ad.setButton(getString(R.string.ok), new DialogInterface.OnClickListener()
			{
				public void onClick(DialogInterface dialog, int which)
				{
					txtCode.setText("");
					dialog.dismiss();
				}
			});
			ad.show();

		}

	}

	private List<Offer> GetOffers(PriceCatalog pc) throws SQLException
	{
		List<Offer> offers = new ArrayList<Offer>();
		Queue<PriceCatalog> priceCatalogQueue = new LinkedList<PriceCatalog>();
		priceCatalogQueue.add(pc);
		while (priceCatalogQueue.size() > 0)
		{
			PriceCatalog currentPc = priceCatalogQueue.remove();
			if (currentPc.getName() == null) // reload if lazy loaded
			{
				currentPc = dbHelper.getPriceCatalogDao().queryForId(currentPc.getID());
			}
			long ticks = 621355968000000000L + (Calendar.getInstance().getTimeInMillis() - Calendar.getInstance().getTimeInMillis() % 86400000) * 10000;
			long ticks2 = 621355968000000000L + (Calendar.getInstance().getTimeInMillis() - Calendar.getInstance().getTimeInMillis() % 86400000 + 86300000) * 10000;
			offers.addAll(dbHelper.findObjects(Offer.class, OperatorType.AND, new BinaryOperator("PriceCatalog_id", currentPc.getID(), FilterType.EQUALS), new BinaryOperator("Active", true, FilterType.EQUALS), new BinaryOperator("StartDate", ticks, FilterType.LESS_OR_EQUAL),
					new BinaryOperator("EndDate", ticks2, FilterType.GREATER_OR_EQUAL)));
			if (currentPc.getParent() != null)
			{
				priceCatalogQueue.add(currentPc.getParent());
			}
		}
		Collections.sort(offers, new Comparator<Offer>()
		{

			public int compare(Offer lhs, Offer rhs)
			{
				if (lhs.getEndDate() < rhs.getEndDate())
					return -1;
				else if (lhs.getEndDate() > rhs.getEndDate())
					return 1;
				return 0;
			}
		});
		return offers;
	}

	private void ShowItemsOfCategory()
	{
		try
		{
			btnShowWarning.setVisibility(View.INVISIBLE);
			itemsInCategoryList = dbHelper.getItemsOfCategory(selectedCategory != null ? selectedCategory.getID() : null, categoryFilter);
			String lblString = (selectedCategory == null) ? getString(R.string.allCategories) : selectedCategory.getName();
			if (itemsInCategoryList == null)
				itemsInCategoryList = new ArrayList<Item>();
			else if (itemsInCategoryList.size() >= DatabaseHelper.MAX_ITEMS && (selectedCategory == null || this.dbHelper.getItemCategoryByParentID(selectedCategory.getID()).size() > 0))
			{
				final String message = getString(R.string.limitedItemsFromCategory).replace("var1", "" + itemsInCategoryList.size());
				lblString += getString(R.string.topItems).replace("var1", "" + itemsInCategoryList.size());
				Utilities.ShowUninterruptingMessage(this, message, 5);

				btnShowWarning.setVisibility(View.VISIBLE);
				btnShowWarning.setOnClickListener(new View.OnClickListener()
				{
					public void onClick(View arg0)
					{

						AlertDialog ad = new AlertDialog.Builder(mContext).create();
						ad.setCancelable(false); // This blocks the 'BACK'
						// button
						ad.setMessage(message);
						ad.setButton(getString(R.string.ok), new DialogInterface.OnClickListener()
						{
							public void onClick(DialogInterface dialog, int which)
							{
								dialog.dismiss();
								btnShowWarning.setVisibility(View.INVISIBLE);
							}
						});
						ad.show();

					}
				});

			}
			lblSelectedCategory.setVisibility((lblString.length() > 0) ? View.VISIBLE : View.INVISIBLE);
			lblSelectedCategory.setText(lblString);
			itemPriceCategoriesList = dbHelper.filterItemsForPriceCatalog(itemsInCategoryList, selectedCustomer.getPc());
			RetailHelper.GetItemsExistingQuantity(itemPriceCategoriesList, currentDocumentHeader, dbHelper);

			lstItemInCategory.setAdapter(new ItemCustomAdapter(this, R.layout.item_row, itemPriceCategoriesList));
			// lstItemInCategory.setAdapter(new ArrayAdapter<Item>(this,
			// android.R.layout.simple_list_item_1, itemsInCategoryList));

		}
		catch (Exception ex)
		{
			ex.printStackTrace();
		}
	}

	private void ShowInfoItemsOfCategory()
	{
		try
		{

			btnInfoShowWarning.setVisibility(View.INVISIBLE);
			itemsInCategoryList = dbHelper.getItemsOfCategory(selectedCategory != null ? selectedCategory.getID() : null, categoryFilter);
			String lblString = (selectedCategory == null) ? getString(R.string.allCategories) : selectedCategory.getName();
			if (itemsInCategoryList == null)
				itemsInCategoryList = new ArrayList<Item>();
			else if (itemsInCategoryList.size() >= DatabaseHelper.MAX_ITEMS && (selectedCategory == null || this.dbHelper.getItemCategoryByParentID(selectedCategory.getID()).size() > 0))
			{
				final String message = getString(R.string.limitedItemsFromCategory).replace("var1", "" + itemsInCategoryList.size());
				lblString += getString(R.string.topItems).replace("var1", "" + itemsInCategoryList.size());
				Utilities.ShowUninterruptingMessage(this, message, 5);

				btnInfoShowWarning.setVisibility(View.VISIBLE);
				btnInfoShowWarning.setOnClickListener(new View.OnClickListener()
				{

					public void onClick(View arg0)
					{

						AlertDialog ad = new AlertDialog.Builder(mContext).create();
						ad.setCancelable(false); // This blocks the 'BACK'
						// button
						ad.setMessage(message);
						ad.setButton(getString(R.string.ok), new DialogInterface.OnClickListener()
						{
							public void onClick(DialogInterface dialog, int which)
							{
								dialog.dismiss();
								btnInfoShowWarning.setVisibility(View.INVISIBLE);
							}
						});
						ad.show();

					}
				});

			}
			lblInfoSelectedCategory.setVisibility((lblString.length() > 0) ? View.VISIBLE : View.INVISIBLE);
			lblInfoSelectedCategory.setText(lblString);
			// itemPriceCategoriesList =
			// dbHelper.filterItemsForPriceCatalog(itemsInCategoryList,
			// selectedCustomer.getPc());
			// RetailHelper.GetItemsExistingQuantity(itemPriceCategoriesList,
			// currentDocumentHeader, dbHelper);

			lstInfoItemList.setAdapter(new ItemGridAdapter(this, R.layout.item_grid_element, itemsInCategoryList));
			// lstItemInCategory.setAdapter(new ItemGridAdapter(this,
			// R.layout.item_grid_element, itemsInCategoryList));
			// lstItemInCategory.setAdapter(new ArrayAdapter<Item>(this,
			// android.R.layout.simple_list_item_1, itemsInCategoryList));

		}
		catch (Exception ex)
		{
			ex.printStackTrace();
		}
	}

	private void SelectItemCategory(long selectedIndex, ListView lst)
	{
		ItemCategory oldCategory = selectedCategory;
		selectedCategory = this.itemCategoryList.get((int) selectedIndex);
		itemCategoryList = dbHelper.getItemCategoryByParentID(selectedCategory.getID());
		if (itemCategoryList.size() == 0)
		{
			// selectedCategory = oldCategory;
			itemCategoryList = new ArrayList<ItemCategory>();// dbHelper.getItemCategoryByParentID(selectedCategory.getID());
			itemCategoryList.add(selectedCategory);
		}
		lst.setAdapter(new ArrayAdapter<ItemCategory>(this, android.R.layout.simple_list_item_1, itemCategoryList));
	}

	private Offer selectedOffer;
	private EditText txtComments;

	private void SelectOffer(int selectedIndex)
	{
		if (selectedIndex >= 0)
			selectedOffer = (Offer) lstOffers.getItemAtPosition(selectedIndex);
		if (selectedOffer == null)
			return;
		ArrayList<OfferDetail> detailsList = new ArrayList<OfferDetail>();
		detailsList.addAll(selectedOffer.getOfferDetails());

		itemPriceOffer = dbHelper.filterItemsForPriceCatalog(RetailHelper.GetOfferItems(selectedOffer, dbHelper, offerFilter), selectedCustomer.getPc());
		RetailHelper.GetItemsExistingQuantity(itemPriceOffer, currentDocumentHeader, dbHelper);
		lstItemsInOffer.setAdapter(new ItemCustomAdapter(this, R.layout.item_row, itemPriceOffer));

	}

	private void UpdateDocumentTypeFilterComboBox()
	{
		try
		{

			List<DocumentType> list = dbHelper.getDocumentTypes().queryForAll();
			DocumentType emptyType = new DocumentType();
			list.add(0, emptyType);
			if (list.size() != 0)
			{
				ArrayAdapter<DocumentType> docTypeAdapter = new ArrayAdapter<DocumentType>(this, android.R.layout.simple_dropdown_item_1line, list);
				cmbTypeFilter.setAdapter(docTypeAdapter);

			}
		}
		catch (Exception ex)
		{
			ex.printStackTrace();
		}

	}

	private void SelectCustomer(final long selectedIndex)
	{
		if (isOnCustomerView)
		{
			if (foundCustomers.size() == 0)
			{
				selectedCustomer = customerList.get((int) selectedIndex);
			}
			else
			{
				selectedCustomer = foundCustomers.get((int) selectedIndex);
			}

			PriceCatalog pc = selectedCustomer.getPc();
			if (pc.getName() == null)
			{
				try
				{
					pc = dbHelper.getPriceCatalogDao().queryForId(pc.getID());
				}
				catch (Exception ex)
				{

				}
			}
			((EditText) findViewById(R.id.txtCustomerCode)).setText(selectedCustomer.getCode());
			((EditText) findViewById(R.id.txtCustomerTaxCode)).setText(selectedCustomer.getTaxCode());
			((EditText) findViewById(R.id.txtCustomerCompanyName)).setText(selectedCustomer.getCompanyName());
			((EditText) findViewById(R.id.txtCustomerDefaultAddress)).setText(selectedCustomer.getDefaultAddress());
			((EditText) findViewById(R.id.txtCustomerDefaultPhone)).setText(selectedCustomer.getDefaultPhone());
			((EditText) findViewById(R.id.txtCustomerPriceCatatlog)).setText(pc.getName());
			tabCustomers.setVisibility(View.GONE);
			UpdateDocumentStatusComboBoxes();
			UpdateDocumentTypeFilterComboBox();
			Calendar cal = Calendar.getInstance();
			Date date = cal.getTime();
			dtFromFilter.updateDate(date.getYear() + 1900, date.getMonth(), date.getDate());
			dtToFilter.updateDate(date.getYear() + 1900, date.getMonth(), date.getDate());
			UpdateDocumentHeaders(true); // clear customer history
			tabCustomerInfo.setVisibility(View.VISIBLE);
			tabCustomerInfo.performClick();
		}
		else
		{
			GeneralAsyncTask selectCustomerTask = new GeneralAsyncTask();

			ProgressDialog dialog = new ProgressDialog(this);
			dialog.setProgressStyle(ProgressDialog.STYLE_SPINNER);
			dialog.setCancelable(false);
			dialog.setMessage(getString(R.string.pleaseWait));

			selectCustomerTask.setDialog(dialog);

			newItemFilter = "";
			categoryFilter = "";
			offerFilter = "";

			selectCustomerTask.setRunner(new AsyncTaskRunnable()
			{
				public Object Run(Object... params)
				{
					try
					{
						if (foundCustomers.size() == 0)
						{
							selectedCustomer = customerList.get((int) selectedIndex);
						}
						else
						{
							selectedCustomer = foundCustomers.get((int) selectedIndex);
						}
						currentDocumentHeader = dbHelper.getDocumentHeaderByCustomer(selectedCustomer);

						if (selectedCustomer.getPc() != null)
						{
							customerOffers = GetOffers(selectedCustomer.getPc());
						}
						else
						{
							throw new Exception("Customer Price Catalog is null");
						}

						if (currentDocumentHeader == null)
						{
							DocumentStatus docStatus = dbHelper.getDefaultDocumentStatus();
							if (docStatus == null)
							{
								try
								{
									List<DocumentStatus> statuses = dbHelper.getDocumentStatuses().queryForAll();
									if (statuses.size() == 0)
									{
										throw new Exception("No Document Statuses Found");
									}
									docStatus = statuses.get(0);
								}
								catch (Exception e)
								{
									e.printStackTrace();
								}

							}
							currentDocumentHeader = new DocumentHeader();
							// currentDocumentHeader.setID(Long.randomLong());
							currentDocumentHeader.setRemoteDeviceDocumentHeaderGuid(UUID.randomUUID().toString());
							currentDocumentHeader.setCustomer(selectedCustomer);
							currentDocumentHeader.setCreatedOn(Calendar.getInstance().getTimeInMillis());
							currentDocumentHeader.setDocumentStatus(docStatus);
							currentDocumentHeader.setDeliveryAddress(selectedCustomer.getDefaultAddress());
							currentDocumentHeader.setCreatedBy(loggedInUser.getRemoteGuid());
							currentDocumentHeader.setAllowMultipleLines(appsettings.getDefaultAllowMultiLines());
							long v = Calendar.getInstance().getTimeInMillis();
							long ticks = 621355968000000000L + (v - v % 86400000) * 10000;
							currentDocumentHeader.setDocumentDate(ticks);
							try
							{
								documentHeadersDao.create(currentDocumentHeader);
							}
							catch (SQLException e)
							{
								e.printStackTrace();
							}
						}
						else if (currentDocumentHeader.getDetails().size() <= 0)
						{
							currentDocumentHeader.setRemoteDeviceDocumentHeaderGuid(UUID.randomUUID().toString());
						}

					}
					catch (Exception e1)
					{
						e1.printStackTrace();
						return e1;
					}
					return null;
				}
			});

			selectCustomerTask.setPostExecuteRunner(new PostExecuteRunnable()
			{

				public void Run(Object result)
				{
					if (result != null && result instanceof Exception)
					{
						Utilities.ShowUninterruptingMessage(mContext, ((Exception) result).getMessage(), 10);
					}
					else
					{
						lstOffers.setAdapter(new OfferHeaderCustomAdapter(mContext, android.R.layout.simple_list_item_1, customerOffers));
						// lstOffers.setAdapter(new
						// ArrayAdapter<Offer>(mContext,
						// android.R.layout.simple_list_item_1,
						// customerOffers));
						tabLogin.setVisibility(View.GONE);
						tabMain.setVisibility(View.GONE);
						tabCustomers.setVisibility(View.GONE);
						tabOrders.setVisibility(View.VISIBLE);
						tabOrdersTotal.setVisibility(View.VISIBLE);
						tabOrderByOffers.setVisibility(View.VISIBLE);
						tabItemCategories.setVisibility(View.VISIBLE);
						tabOrderByNewItems.setVisibility(View.VISIBLE);
						tabOrders.performClick();
						UpdateDocumentDetailsView();
						UpdateDocumentStatusComboBoxes();
						InitializeOrderTabs();

					}
				}
			});

			selectCustomerTask.execute((Object) null);
		}

	}

	private void InitializeGUI()
	{
		// String html;
		// try
		// {
		// File xmlFile = new File(Environment.getExternalStorageDirectory(),
		// "example_android_documents.xml");
		// StreamSource xmlSource = new StreamSource(xmlFile);
		// File xslFile = new File(Environment.getExternalStorageDirectory(),
		// "example_android_documents.xsl");
		// StreamSource xslSource = new StreamSource(xslFile);
		//
		// html = Utilities.transformXML(xmlSource, xslSource);
		// }
		// catch (TransformerException e1)
		// {
		// // TODO Auto-generated catch block
		// }
		setContentView(R.layout.main_layout);

		// Set Tabs
		mTabHost = (TabHost) findViewById(android.R.id.tabhost);
		mTabHost.setup();
		mTabHost.addTab(mTabHost.newTabSpec("tab_test1").setIndicator(getString(R.string.Login)).setContent(R.id.login_tab));
		mTabHost.addTab(mTabHost.newTabSpec("tab_test2").setIndicator(getString(R.string.MainMenu)).setContent(R.id.main_tab));
		mTabHost.addTab(mTabHost.newTabSpec("tab_test3").setIndicator(getString(R.string.SelectCustomer)).setContent(R.id.select_customer_tab));
		mTabHost.addTab(mTabHost.newTabSpec("tab_test4").setIndicator(getString(R.string.OrderForm)).setContent(R.id.tab_4));
		mTabHost.addTab(mTabHost.newTabSpec("tab_settings").setIndicator(getString(R.string.ItemCategories)).setContent(R.id.order_by_category_tab));
		mTabHost.addTab(mTabHost.newTabSpec("tab_set").setIndicator(getString(R.string.ordernewitems)).setContent(R.id.order_by_new_items));
		mTabHost.addTab(mTabHost.newTabSpec("tab_orders").setIndicator(getString(R.string.offers)).setContent(R.id.order_by_offers));
		mTabHost.addTab(mTabHost.newTabSpec("tab_test4b").setIndicator(getString(R.string.OrderTotals)).setContent(R.id.orders_total_tab));
		mTabHost.addTab(mTabHost.newTabSpec("tab_settings2").setIndicator(getString(R.string.settingsHeader)).setContent(R.id.tab_settings));
		mTabHost.addTab(mTabHost.newTabSpec("tab_search_items").setIndicator(getString(R.string.itemSearch)).setContent(R.id.tabItemSearch));
		mTabHost.addTab(mTabHost.newTabSpec("tab_item_info").setIndicator(getString(R.string.itemInfo)).setContent(R.id.tabItemInfo));
		mTabHost.addTab(mTabHost.newTabSpec("tab_about").setIndicator(getString(R.string.aboutHeader)).setContent(R.id.about_tab));
		mTabHost.addTab(mTabHost.newTabSpec("tab_customerInfo").setIndicator(getString(R.string.customer)).setContent(R.id.tabCustomerInfo));
		mTabHost.addTab(mTabHost.newTabSpec("tab_item_list").setIndicator(getString(R.string.items)).setContent(R.id.tab_item_list));
		mTabHost.addTab(mTabHost.newTabSpec("tab_check_price").setIndicator(getString(R.string.checkPrice)).setContent(R.id.tab_check_price));
		//

		tabLogin = mTabHost.getTabWidget().getChildAt(0);
		tabMain = mTabHost.getTabWidget().getChildAt(1);
		tabCustomers = mTabHost.getTabWidget().getChildAt(2);
		tabOrders = mTabHost.getTabWidget().getChildAt(3);
		tabItemCategories = mTabHost.getTabWidget().getChildAt(4);
		tabOrderByNewItems = mTabHost.getTabWidget().getChildAt(5);
		tabOrderByOffers = mTabHost.getTabWidget().getChildAt(6);
		tabOrdersTotal = mTabHost.getTabWidget().getChildAt(7);
		tabSettings = mTabHost.getTabWidget().getChildAt(8);
		tabItemSearchByName = mTabHost.getTabWidget().getChildAt(9);
		tabItemInfo = mTabHost.getTabWidget().getChildAt(10);
		tabAbout = mTabHost.getTabWidget().getChildAt(11);
		tabCustomerInfo = mTabHost.getTabWidget().getChildAt(12);
		tabItemList = mTabHost.getTabWidget().getChildAt(13);
		tabCheckPrice = mTabHost.getTabWidget().getChildAt(14);

		// Hide tabs
		tabMain.setVisibility(View.GONE);
		tabCustomers.setVisibility(View.GONE);
		tabOrders.setVisibility(View.GONE);
		tabOrdersTotal.setVisibility(View.GONE);
		tabOrderByOffers.setVisibility(View.GONE);
		tabItemCategories.setVisibility(View.GONE);
		tabOrderByNewItems.setVisibility(View.GONE);
		tabItemSearchByName.setVisibility(View.GONE);
		tabItemInfo.setVisibility(View.GONE);
		tabAbout.setVisibility(View.GONE);
		tabOrderByOffers.setVisibility(View.GONE);
		tabCustomerInfo.setVisibility(View.GONE);
		tabItemList.setVisibility(View.GONE);
		tabCheckPrice.setVisibility(View.GONE);

		if (appsettings != null && appsettings.getServiceUrl() != null && appsettings.getServiceUrl().length() > 4)
		{
			tabSettings.setVisibility(View.VISIBLE);
			tabLogin.performClick();
		}
		else
		{
			tabLogin.setVisibility(View.GONE);
			tabSettings.performClick();
		}
		// buttons
		btnLogin = (Button) findViewById(R.id.btnLogin);
		btnExit = (Button) findViewById(R.id.btnExit);
		btnExitMenu = (Button) findViewById(R.id.btnExitMenu);
		btnOrders = (Button) findViewById(R.id.btnOrder);
		btnUpdate = (Button) findViewById(R.id.btnUpdate);
		btnSettings = (Button) findViewById(R.id.btnSettings);
		btnSaveSettings = (Button) findViewById(R.id.btnSaveSettings);
		btnRevertSettings = (Button) findViewById(R.id.btnRevertSettings);
		btnSendCurrentOrder = (Button) findViewById(R.id.btnSendCurrentOrder);
		btnShowItems = (Button) findViewById(R.id.btnShowItems);
		btnScan = (Button) findViewById(R.id.btnScan);
		btnCheckPriceScan = (Button) findViewById(R.id.btnCheckPriceScan);
		btnUploadAll = (Button) findViewById(R.id.btnUploadAll);
		btnExportDB = (Button) findViewById(R.id.btnExportToSD);
		btnImportDB = (Button) findViewById(R.id.btnImportDB);
		btnShowNewitems = (Button) findViewById(R.id.btnShowNewItems);
		btnSearchItemByName = (Button) findViewById(R.id.btnSearchItemByName);
		btnShowWarning = (ImageButton) findViewById(R.id.btnShowWarning);
		btnInfoShowWarning = (ImageButton) findViewById(R.id.btnInfoShowWarning);
		btnAbout = (Button) findViewById(R.id.btnAbout);
		btnInfoShowItems = (Button) findViewById(R.id.btnInfoShowItems);
		btnRestoreDeliveryAddress = (Button) findViewById(R.id.btnRestoreDeliveryAddress);

		btnFilterCategory = (ImageButton) findViewById(R.id.btnFilterCategory);
		btnFilterOffers = (ImageButton) findViewById(R.id.btnFilterOffer);
		btnFilterNewItems = (ImageButton) findViewById(R.id.btnFilterNewItems);
		btnInfoFilterCategory = (ImageButton) findViewById(R.id.btnInfoFilterCategory);

		btnItems = (Button) findViewById(R.id.btnHistory);
		btnCustomers = (Button) findViewById(R.id.btnCustomers);
		btnAddNewCustomer = (Button) findViewById(R.id.btnAddNewCustomer);
		btnLoadHeaders = (Button) findViewById(R.id.btnLoadHeaders);
		btnCheckPrice = (Button) findViewById(R.id.btnCheckPrice);
		// menu items
		// miSettings = (MenuItem) findViewById(R.id.menu_settings);

		// Textfileds
		txtCode = (EditText) findViewById(R.id.txtCode);
		txtCheckPriceCode = (EditText) findViewById(R.id.txtCheckPriceCode);
		txtDescription = (EditText) findViewById(R.id.txtDescription);
		txtCheckPriceDescription = (EditText) findViewById(R.id.txtCheckPriceDescription);
		txtPrice = (EditText) findViewById(R.id.txtPrice);
		txtCheckPriceValue = (EditText) findViewById(R.id.txtCheckPriceValue);
		txtCheckPriceDiscount = (EditText) findViewById(R.id.txtCheckPriceDiscount);

		txtQty = (EditText) findViewById(R.id.txtQty);
		txtServerUrl = (EditText) findViewById(R.id.txtServer);
		dtNewItemsFrom = (DatePicker) findViewById(R.id.dtNewItemsFrom);
		dtFinalizeDate = (DatePicker) findViewById(R.id.dtFinalizeDate);
		dtFromFilter = (DatePicker) findViewById(R.id.dtFromDateFilter);
		dtToFilter = (DatePicker) findViewById(R.id.dtToDateFilter);

		txtCustomerSearchBox = (EditText) findViewById(R.id.acCustomersSearchBox);
		txtDBImportPath = (EditText) findViewById(R.id.txtImportDBPath);
		txtItemNameSearch = (EditText) findViewById(R.id.txtItemName);
		txtDBImportPath.setText(Environment.getExternalStorageDirectory() + "/" + dbHelper.getDatabaseName());

		txtItemInfoCode = (EditText) findViewById(R.id.txtInfoItemCode);
		txtItemInfoBarcode = (EditText) findViewById(R.id.txtInfoItemBarcode);
		txtItemInfoGrossTotal = (EditText) findViewById(R.id.txtInfoItemGrossTotal);
		txtItemInfoQty = (EditText) findViewById(R.id.txtInfoItemQuantity);
		txtItemInfoUnitPrice = (EditText) findViewById(R.id.txtInfoItemUnitPrice);
		txtItemInfoVat = (EditText) findViewById(R.id.txtInfoItemVat);
		txtItemInfoVatCategory = (EditText) findViewById(R.id.txtInfoItemVatCategory);

		txtItemInfoDescription = (EditText) findViewById(R.id.txtInfoItemDescription);
		txtAddress = (EditText) findViewById(R.id.txtAddress);
		txtComments = (EditText) findViewById(R.id.txtComments);
		txtDefaultQty = (EditText) findViewById(R.id.txtDefaultQty);
		txtItemInfoPackingQty = (EditText) findViewById(R.id.txtInfoItemPackingQty);
		txtItemInfoMaxOrderQty = (EditText) findViewById(R.id.txtInfoItemMaxOrderQty);
		txtItemInfoMeasurementUnit = (EditText) findViewById(R.id.txtInfoItemMeasurementUnit);
		txtItemInfoTotalDiscount = (EditText) findViewById(R.id.txtInfoItemTotalDiscount);
		txtPriceCatalog = (EditText) findViewById(R.id.txtPriceCatalog);
		txtPathToXSL = (EditText) findViewById(R.id.txtPathToXSL);
		txtPathToXSL.setText(DEFAULT_XSL_PATH);

		cmbNewItems = (Spinner) findViewById(R.id.cmbNewItemsFrom);
		cmbBluetoothDevices = (Spinner) findViewById(R.id.cmbBluetoothDevices);
		cmbDocumentStatus = (Spinner) findViewById(R.id.cmbDocumentStatus);
		cmbStore = (Spinner) findViewById(R.id.cmbStore);

		cmbAppSettingsDefaultStore = (Spinner) findViewById(R.id.cmbAppSettingsDefaultStore);
		cmbAppSettingsDefaultDocumentStatus = (Spinner) findViewById(R.id.cmbAppSettingsDefaultDocumentStatus);
		cmbAppSettingsDefaultPriceCatalog = (Spinner) findViewById(R.id.cmbAppSettingsDefaultPriceCatalog);
		cmbHasBeenCheckedFilter = (Spinner) findViewById(R.id.cmbHasBeenCheckedFilter);
		cmbHasBeenExecutedFilter = (Spinner) findViewById(R.id.cmbHasBeenExecutedFilter);
		List<String> yesNoList = new ArrayList<String>();
		yesNoList.add("");
		yesNoList.add(getString(R.string.yes));
		yesNoList.add(getString(R.string.no));

		cmbHasBeenCheckedFilter.setAdapter(new ArrayAdapter<String>(mContext, android.R.layout.simple_dropdown_item_1line, yesNoList));
		cmbHasBeenExecutedFilter.setAdapter(new ArrayAdapter<String>(mContext, android.R.layout.simple_dropdown_item_1line, yesNoList));

		cmbStatusFilter = (Spinner) findViewById(R.id.cmbStatusFilter);
		cmbTypeFilter = (Spinner) findViewById(R.id.cmbTypeFilter);

		lstCustomers = (ListView) findViewById(R.id.lstCustomers);
		lstDocumentHeaders = (ListView) findViewById(R.id.lstDocumentHeaders);
		docHeaderColumns = findViewById(R.id.documentHeaderColumns);

		// getLayoutInflater().inflate(R.layout.customer_row, null)));
		lstOffers = (ListView) findViewById(R.id.lstItemOffers);
		lstItemsInOffer = (ListView) findViewById(R.id.lstItemsInsideOffer);
		// lstItemsInOffer.addHeaderView((View)
		// getLayoutInflater().inflate(R.layout.item_row_header_no_code, null));
		docDetailsView = (ListView) findViewById(R.id.lstDocumentLines);
		lstItemCategories = (ListView) findViewById(R.id.lstItemCategories);
		lstInfoItemCategories = (ListView) findViewById(R.id.lstInfoItemCategories);
		lstItemInCategory = (ListView) findViewById(R.id.lstItemsInsideCategory);
		// lstItemInCategory.addHeaderView((View)
		// getLayoutInflater().inflate(R.layout.item_row_header_no_code, null));
		lstNewItems = (ListView) findViewById(R.id.lstNewItems);
		// lstNewItems.addHeaderView((View)
		// getLayoutInflater().inflate(R.layout.item_row_header_no_code, null));
		lstItemsByNameResult = (ListView) findViewById(R.id.lvSearchItemByNameResult);
		// lstItemsByNameResult.addHeaderView((View)
		// getLayoutInflater().inflate(R.layout.item_row_header, null));
		lstInfoItemList = (GridView) findViewById(R.id.lstInfoItemList);

		importLinearLayout = (LinearLayout) findViewById(R.id.importDBLayout);
		tvDescription = (TextView) findViewById(R.id.tvDescription);
		lblSelectedCategory = (TextView) findViewById(R.id.lblSelectedCategory);
		lblOffers = (TextView) findViewById(R.id.lblOffers);
		lblInfoSelectedCategory = (TextView) findViewById(R.id.lblInfoSelectedCategory);
		filterTable = (TableLayout) findViewById(R.id.filtersTable);
		tvVersion = (TextView) findViewById(R.id.tvVersion);
		chkBatch = (CheckBox) findViewById(R.id.chkBatch);
		chkAllowMultipleLines = (CheckBox) findViewById(R.id.chkAllowMultipleLines);
		chkAppSettingsDefaultPC = (CheckBox) findViewById(R.id.chkAppSettingsDefaultPC);
		chkAppSettingsDefaultStore = (CheckBox) findViewById(R.id.chkAppSettingsDefaultStore);
		chDefaultAllowMultiLines = (CheckBox) findViewById(R.id.chDefaultAllowMultiLines);
		chkUseFilters = (CheckBox) findViewById(R.id.chkUseFilters);
		itemInfoImage = (ImageView) findViewById(R.id.itemInfoImage);

		lblOfferHeaderFrom = (TextView) findViewById(R.id.lblOfferHeaderFrom);
		lblOfferHeaderTo = (TextView) findViewById(R.id.lblOfferHeaderTo);
		try
		{
			PackageInfo pInfo;
			pInfo = getPackageManager().getPackageInfo(getPackageName(), 0);
			tvVersion.setText("v " + pInfo.versionName);
		}
		catch (NameNotFoundException e)
		{
			e.printStackTrace();
		}

		ArrayList<Customer> customers = new ArrayList<Customer>();
		itemsByNameResult = new ArrayList<Item>();
		foundCustomers = new ArrayList<Customer>();
		loggedInUserStores = new ArrayList<Store>();
		// lstCustomers.setAdapter(new ArrayAdapter<Customer>(this,
		// android.R.layout.simple_list_item_1, customers));
		lstCustomers.setClickable(false);
		// lstItemsByNameResult.setAdapter(new ArrayAdapter<Item>(this,
		// android.R.layout.simple_list_item_1, itemsByNameResult));

		// ItemsAutoCompleteAdapter itemsACAdapter = new
		// ItemsAutoCompleteAdapter(this,
		// android.R.layout.simple_dropdown_item_1line, dbHelper);
		// txtDescription.setAdapter(itemsACAdapter);
		// txtDescription.setThreshold(4);
		// txtDescription.

		InitializeOrderTabs();
		InitializeGUIListeners();

		// colors
		lstItemCategories.setBackgroundColor(Color.LTGRAY);
		lstOffers.setBackgroundColor(Color.LTGRAY);
		// lblOffers.setBackgroundColor(Color.LTGRAY);

		txtDefaultQty.setFilters(new InputFilter[] { new InputFilterIntegerMinMax(1, 999999) });

		// Load Stores and PC combo boxes----

		UpdateSettingsComboBoxes();

		// ----------------------------------

		UpdateSettingsTab();

		findViewById(R.id.layoutSettingsForLoggedin).setVisibility(View.GONE);
	}

	public void UpdateSettingsComboBoxes()
	{
		try
		{
			allStores = dbHelper.getStoresDao().queryForAll();
		}
		catch (Exception ex)
		{
			allStores = new ArrayList<Store>();
		}
		try
		{
			allPriceCatalogs = dbHelper.getPriceCatalogDao().queryForAll();
		}
		catch (Exception ex)
		{
			allPriceCatalogs = new ArrayList<PriceCatalog>();
		}
		try
		{
			List<DocumentStatus> statuses = dbHelper.getDocumentStatuses().queryForAll();
			docStatusArray = statuses.toArray(new DocumentStatus[statuses.size()]);
			cmbAppSettingsDefaultDocumentStatus.setAdapter(new ArrayAdapter<DocumentStatus>(mContext, android.R.layout.simple_dropdown_item_1line, docStatusArray));
		}
		catch (Exception ex)
		{
			ex.printStackTrace();
		}

		cmbAppSettingsDefaultPriceCatalog.setAdapter(new ArrayAdapter<PriceCatalog>(mContext, android.R.layout.simple_dropdown_item_1line, allPriceCatalogs));
		cmbAppSettingsDefaultStore.setAdapter(new ArrayAdapter<Store>(mContext, android.R.layout.simple_dropdown_item_1line, allStores));
	}

	public void UpdateSettingsTab()
	{
		if (appsettings != null)
		{
			txtServerUrl.setText(appsettings.getServiceUrl());
			chkAppSettingsDefaultPC.setChecked(appsettings.getDefaultPriceCatalog() != null);
			chkAppSettingsDefaultStore.setChecked(appsettings.getDefaultStore() != null);
			chDefaultAllowMultiLines.setChecked(appsettings.getDefaultAllowMultiLines());

			if (appsettings.getDefaultPriceCatalog() == null)
				cmbAppSettingsDefaultPriceCatalog.setEnabled(false);
			else
			{
				for (int i = 0; i < allPriceCatalogs.size(); i++)
				{
					if (appsettings.getDefaultPriceCatalog().getID() == allPriceCatalogs.get(i).getID())
					{
						cmbAppSettingsDefaultPriceCatalog.setSelection(i);
						break;
					}
				}
			}

			if (appsettings.getDefaultDocumentStatus() != null)
			{
				for (int i = 0; i < docStatusArray.length; i++)
				{
					if (appsettings.getDefaultDocumentStatus().getID() == docStatusArray[i].getID())
					{
						cmbAppSettingsDefaultDocumentStatus.setSelection(i);
						break;
					}
				}
			}

			if (appsettings.getDefaultStore() == null)
				cmbAppSettingsDefaultStore.setEnabled(false);
			else
			{
				for (int i = 0; i < allStores.size(); i++)
				{
					if (appsettings.getDefaultStore().getID() == allStores.get(i).getID())
					{
						cmbAppSettingsDefaultStore.setSelection(i);
						break;
					}
				}
			}

			if (appsettings.getPathToXSL() != null && !appsettings.getPathToXSL().isEmpty())
			{
				txtPathToXSL.setText(appsettings.getPathToXSL());
			}
		}
	}

	public void UpdateUI()
	{
		if (appsettings != null)
		{
			RetailHelper.euroNormal = new DecimalFormat(GetFormat(appsettings.getDisplayDigits()));
			RetailHelper.euroValue = new DecimalFormat(GetFormat(appsettings.getDisplayValueDigits()));
		}

		itemCategoryList = dbHelper.getItemCategoryByParentID((selectedCategory == null) ? null : selectedCategory.getID());
		lstItemCategories.setAdapter(new ArrayAdapter<ItemCategory>(this, android.R.layout.simple_list_item_1, itemCategoryList));
		String[] strCmbNewItems = { getResources().getString(R.string.day), getResources().getString(R.string.week), getResources().getString(R.string.month), getResources().getString(R.string.specify) };
		cmbNewItems.setAdapter(new ArrayAdapter<String>(this, android.R.layout.simple_dropdown_item_1line, strCmbNewItems));

	}

	private void btnSaveSettingsOnClick()
	{
		boolean newEntry = false;
		if (appsettings == null)
		{
			newEntry = true;
			appsettings = new ApplicationSettings();
			// appsettings.setID(Long.randomLong());
		}
		appsettings.setServiceUrl(txtServerUrl.getText().toString());
		appsettings.setDefaultPriceCatalog(chkAppSettingsDefaultPC.isChecked() ? (PriceCatalog) cmbAppSettingsDefaultPriceCatalog.getSelectedItem() : null);
		appsettings.setDefaultStore(chkAppSettingsDefaultStore.isChecked() ? (Store) cmbAppSettingsDefaultStore.getSelectedItem() : null);
		appsettings.setDefaultDocumentStatus((DocumentStatus) cmbAppSettingsDefaultDocumentStatus.getSelectedItem());
		appsettings.setDefaultAllowMultiLines(chDefaultAllowMultiLines.isChecked());
		appsettings.setPathToXSL(txtPathToXSL.getText().toString());

		if (cmbBluetoothDevices.getSelectedItem() != null)
		{
			appsettings.setBluetoothDeviceName(cmbBluetoothDevices.getSelectedItem().toString());
		}

		if (loggedInUser == null)
		{
			// switch to Login tab
			tabLogin.setVisibility(View.VISIBLE);
			tabLogin.performClick();
		}
		else
		{

			// switch to Main tab
			tabMain.setVisibility(View.VISIBLE);
			tabSettings.setVisibility(View.GONE);
			tabMain.performClick();

		}

		try
		{
			if (newEntry)
				dbHelper.getApplicationSettings().create(appsettings);
			else
				dbHelper.getApplicationSettings().update(appsettings);
		}
		catch (Exception ex)
		{
			ex.printStackTrace();
		}
	}

	private void btnRevertSettingsOnClick()
	{
		if (loggedInUser == null)
		{
			ExitApplication();
		}
		else
		{
			appsettings = null;
			appsettings = dbHelper.getSettings();
			tabMain.setVisibility(View.VISIBLE);
			tabSettings.setVisibility(View.GONE);
			tabMain.performClick();
			UpdateSettingsTab();
		}
	}

	private void UpdateNewItems(boolean displayOnly)
	{
		long afterThisJavaMillis = Calendar.getInstance().getTimeInMillis();
		String dateForMessage = cmbNewItems.getSelectedItem().toString();
		switch (cmbNewItems.getSelectedItemPosition())
		{
		case 3:
			dtNewItemsFrom.setEnabled(true);
			btnShowNewitems.setVisibility(View.VISIBLE);
			dtNewItemsFrom.setVisibility(View.VISIBLE);
			Calendar cc = new GregorianCalendar(dtNewItemsFrom.getYear(), dtNewItemsFrom.getMonth(), dtNewItemsFrom.getDayOfMonth());
			SimpleDateFormat dateFormat = new SimpleDateFormat("dd/MM/yyyy");
			dateForMessage = dateFormat.format(cc.getTime());
			afterThisJavaMillis = cc.getTimeInMillis();
			break;
		case 2: // Month
			afterThisJavaMillis -= 23 * 86400000;
		case 1: // Week
			afterThisJavaMillis -= 7 * 86400000;
		case 0: // Day
			displayOnly = false;
			afterThisJavaMillis -= afterThisJavaMillis % 86400000 + 86400000;
			dtNewItemsFrom.setEnabled(false);
			btnShowNewitems.setVisibility(View.INVISIBLE);
			dtNewItemsFrom.setVisibility(View.INVISIBLE);
			break;
		default: // Unknown
			Log.e("Update new Items", "Unsupported selection");
			return;
		}
		if (!displayOnly)
		{
			int resultLimit = 1000;
			newItemsList = dbHelper.getNewItems(afterThisJavaMillis, resultLimit, newItemFilter);
			if (newItemsList.size() >= resultLimit)
			{
				Utilities.ShowUninterruptingMessage(this, getString(R.string.limitedItemsFromNewItems).replace("var1", "" + resultLimit), 3);
			}
			else if (newItemsList.size() == 0)
			{
				Utilities.ShowUninterruptingMessage(this, getString(R.string.noNewItemsFound).replace("var1", dateForMessage), 3);
			}

			newItemPriceList = dbHelper.filterItemsForPriceCatalog(newItemsList, selectedCustomer.getPc());
			RetailHelper.GetItemsExistingQuantity(newItemPriceList, currentDocumentHeader, dbHelper);
			lstNewItems.setAdapter(new ItemCustomAdapter(this, R.layout.item_row, newItemPriceList));
		}
	}

	private void ChangeDeliveryAddress()
	{
		txtAddress.setText(selectedCustomer.getDefaultAddress());
	}

	private boolean CheckIfUserNeedsUpdate(boolean checkConnectivity)
	{
		long lastUpdateInMilis = getPreferences(MODE_PRIVATE).getLong("lastUpdate", 0);
		long currentTime = Calendar.getInstance().getTimeInMillis();
		if (checkConnectivity)
		{
			if (currentTime - lastUpdateInMilis >= UPDATE_CHECKS_TIMESPAN && wsHelper.IsOnline())
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		else
		{
			if (currentTime - lastUpdateInMilis >= UPDATE_CHECKS_TIMESPAN)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

	}

	private void ValidateBatchMode()
	{
		if (chkBatch.isChecked())
		{
			int value = -1;
			try
			{
				value = Integer.parseInt(txtDefaultQty.getText().toString());
			}
			catch (Exception ex)
			{

			}
			if (value < 0)
			{
				txtDefaultQty.setText("0");
			}
		}
	}

	private int customerSearchCount = 0;

	private void ResetCustomerList()
	{
		customerHandleVariableDelay = 0;
		txtCustomerSearchBox.setText("");
		customerHandleVariableDelay = customerHandlerDelay;
		UpdateCustomers(false);
	}

	private void InitiateCameraScan()
	{
		IntentIntegrator intentIntegrator = new IntentIntegrator(MainActivity.this);
		intentIntegrator.setTitleByID(R.string.bcScannerMessageTitle);
		intentIntegrator.setMessageByID(R.string.bcScannerMessage);
		intentIntegrator.setButtonNoByID(R.string.no);
		intentIntegrator.setButtonYesByID(R.string.yes);
		intentIntegrator.initiateScan();

	}

	private void SortOffers(TextView source)
	{
		if (source == this.offerSortingSource)
		{
			offerSortingDirection = (offerSortingDirection < 1) ? offerSortingDirection + 1 : -1;
		}
		else
		{
			offerSortingDirection = 1;
			offerSortingSource = source;
		}
		lblOfferHeaderFrom.setCompoundDrawablesWithIntrinsicBounds(0, 0, 0, 0);
		lblOfferHeaderTo.setCompoundDrawablesWithIntrinsicBounds(0, 0, 0, 0);
		Comparator<Offer> cmp;
		if(offerSortingDirection==0)
		{
			cmp = new Comparator<Offer>(){

				public int compare(Offer lhs, Offer rhs)
				{
					if(lhs.getID() < rhs.getID())
						return 1;
					else if (lhs.getID() > rhs.getID())
						return -1;
					return 0;
				}			
			};
		}
		else if (offerSortingSource == lblOfferHeaderFrom)
		{
			cmp = new Comparator<Offer>(){

				public int compare(Offer lhs, Offer rhs)
				{
					if(lhs.getStartDate() < rhs.getStartDate())
						return offerSortingDirection;
					else if (lhs.getStartDate() > rhs.getStartDate())
						return -offerSortingDirection;
					return 0;
				}			
			};
		}
		else if (offerSortingSource == lblOfferHeaderTo)
		{
			cmp = new Comparator<Offer>(){

				public int compare(Offer lhs, Offer rhs)
				{
					if(lhs.getEndDate() < rhs.getEndDate())
						return offerSortingDirection;
					else if (lhs.getEndDate() > rhs.getEndDate())
						return -offerSortingDirection;
					return 0; 
				}			
			};
		}		
		else 
		{
			return;
		}
		if(offerSortingDirection!=0)
		{
			//Drawable dr = getResources().getDrawable(offerSortingDirection==1?R.raw.arrow_down_color:R.raw.arrow_up_color);
			offerSortingSource.setCompoundDrawablesWithIntrinsicBounds(0, 0, offerSortingDirection==1?R.raw.arrow_down_color:R.raw.arrow_up_color, 0);
			//offerSortingSource.postInvalidate();
		}
		Collections.sort(customerOffers, cmp);
		//lstOffers.invalidate();
		//lstOffers.postInvalidate();
		lstOffers.invalidateViews();
		
	}

	private void InitializeGUIListeners()
	{

		lblOfferHeaderFrom.setOnClickListener(new View.OnClickListener()
		{

			public void onClick(View v)
			{
				SortOffers(lblOfferHeaderFrom);

			}
		});

		lblOfferHeaderTo.setOnClickListener(new View.OnClickListener()
		{

			public void onClick(View v)
			{
				SortOffers(lblOfferHeaderTo);

			}
		});

		CustomerSearchHandler = new Handler()
		{
			public void handleMessage(android.os.Message msg)
			{
				if (msg.what == customerSearchCount)
				{
					SearchCustomers();
					// System.out.println("Success" + searchCount + "=" +
					// msg.what);
				}
				else
				{
					// System.out.println(msg.what + " - " + searchCount);
				}
			}
		};

		txtCustomerSearchBox.addTextChangedListener(new TextWatcher()
		{

			public void beforeTextChanged(CharSequence s, int start, int before, int count)
			{

			}

			public void onTextChanged(CharSequence s, int start, int before, int count)
			{
				// SearchCustomers();
				customerSearchCount++;
				if (customerSearchCount > Integer.MAX_VALUE >> 1) // reset if
				// counter
				// gets crazy
				{
					customerSearchCount = 0;
				}
				CustomerSearchHandler.sendMessageDelayed(CustomerSearchHandler.obtainMessage(customerSearchCount), customerHandleVariableDelay);
			}

			public void afterTextChanged(Editable arg0)
			{

			}

		});

		btnInfoShowItems.setOnClickListener(new View.OnClickListener()
		{

			public void onClick(View v)
			{
				ShowInfoItemsOfCategory();
			}
		});

		chkAppSettingsDefaultPC.setOnCheckedChangeListener(new OnCheckedChangeListener()
		{

			public void onCheckedChanged(CompoundButton buttonView, boolean isChecked)
			{
				cmbAppSettingsDefaultPriceCatalog.setEnabled(isChecked);
			}
		});

		chkAppSettingsDefaultStore.setOnCheckedChangeListener(new OnCheckedChangeListener()
		{

			public void onCheckedChanged(CompoundButton buttonView, boolean isChecked)
			{
				cmbAppSettingsDefaultStore.setEnabled(isChecked);
			}
		});

		btnLoadHeaders.setOnClickListener(new View.OnClickListener()
		{

			public void onClick(View v)
			{

				if (appsettings != null)
				{
					File file = new File(appsettings.getPathToXSL());
					if (file.exists())
					{
						ShowDocumentHeadersWebView();
					}
					else
					{
						docHeaderColumns.setVisibility(View.VISIBLE);
						lstDocumentHeaders.setVisibility(View.VISIBLE);
						UpdateDocumentHeaders();
					}

				}

			}
		});

		btnAddNewCustomer.setOnClickListener(new View.OnClickListener()
		{

			public void onClick(View v)
			{
				AddNewCustomer();
			}
		});

		chkUseFilters.setOnCheckedChangeListener(new OnCheckedChangeListener()
		{

			public void onCheckedChanged(CompoundButton arg0, boolean isChecked)
			{
				if (isChecked)
				{
					filterTable.setVisibility(View.VISIBLE);
				}
				else
				{
					filterTable.setVisibility(View.INVISIBLE);
				}
			}
		});

		btnCustomers.setOnClickListener(new View.OnClickListener()
		{

			public void onClick(View v)
			{
				// TODO Auto-generated method stub
				isOnCustomerView = true;
				isOnItemView = false;

				// UpdateCustomers(false); NEEDS TEST
				customerListAdapter.setMarkCustomerOrders(false);
				lstCustomers.setAdapter(customerListAdapter);
				// (new CustomerCustomAdapter(mContext, R.layout.customer_row,
				// customerList));
				tabLogin.setVisibility(View.GONE);
				tabMain.setVisibility(View.GONE);
				tabOrders.setVisibility(View.GONE);
				tabOrdersTotal.setVisibility(View.GONE);
				tabOrderByOffers.setVisibility(View.GONE);
				tabCustomers.setVisibility(View.VISIBLE);
				tabCustomers.performClick();
				ResetCustomerList();

			}
		});

		btnCheckPrice.setOnClickListener(new View.OnClickListener()
		{

			public void onClick(View arg0)
			{
				tabCustomerInfo.setVisibility(View.GONE);
				tabCheckPrice.setVisibility(View.VISIBLE);
				tabCheckPrice.performClick();
			}
		});

		// dbHelper.getItemsOfCategory(ic, filter)
		// dbHelper.getItems().queryForFieldValues(fieldValues)

		btnItems.setOnClickListener(new View.OnClickListener()
		{

			public void onClick(View v)
			{
				// TODO Auto-generated method stub
				isOnCustomerView = false;
				isOnItemView = true;
				selectedCategory = null;
				tabMain.setVisibility(View.GONE);
				tabItemList.setVisibility(View.VISIBLE);
				categoryFilter = "";
				tabItemList.performClick();
				try
				{
					// SelectItemCategory(selectedIndex, lstInfoItemCategories);
					itemCategoryList = dbHelper.getItemCategoryByParentID(null);
					lstInfoItemCategories.setAdapter(new ArrayAdapter<ItemCategory>(mContext, android.R.layout.simple_list_item_1, itemCategoryList));
					lstInfoItemList.setAdapter(new ItemGridAdapter(mContext, R.layout.item_grid_element, new ArrayList<Item>()));
				}
				catch (Exception e)
				{
					// TODO Auto-generated catch block
					e.printStackTrace();
				}

			}
		});

		chkBatch.setOnCheckedChangeListener(new OnCheckedChangeListener()
		{

			public void onCheckedChanged(CompoundButton buttonView, boolean isChecked)
			{
				if (isChecked)
				{
					txtDefaultQty.setVisibility(View.VISIBLE);
				}
				else
				{
					txtDefaultQty.setVisibility(View.INVISIBLE);
				}
				ValidateBatchMode();

			}
		});

		btnFilterNewItems.setOnClickListener(new View.OnClickListener()
		{

			public void onClick(View v)
			{
				getFilterPopup(newItemFilter, 2);

			}
		});

		btnFilterCategory.setOnClickListener(new View.OnClickListener()
		{

			public void onClick(View v)
			{
				getFilterPopup(categoryFilter, 0);
			}
		});

		btnInfoFilterCategory.setOnClickListener(new View.OnClickListener()
		{

			public void onClick(View v)
			{
				getFilterPopup(categoryFilter, 3);
			}
		});

		btnFilterOffers.setOnClickListener(new View.OnClickListener()
		{

			public void onClick(View v)
			{
				getFilterPopup(offerFilter, 1);
			}
		});

		btnRestoreDeliveryAddress.setOnClickListener(new View.OnClickListener()
		{

			public void onClick(View v)
			{
				ChangeDeliveryAddress();
			}
		});
		dtNewItemsFrom.init(Calendar.getInstance().get(Calendar.YEAR), Calendar.getInstance().get(Calendar.MONTH), Calendar.getInstance().get(Calendar.DAY_OF_MONTH), new OnDateChangedListener()
		{
			int oldyear = Calendar.getInstance().get(Calendar.YEAR), oldmonth = Calendar.getInstance().get(Calendar.MONTH), oldday = Calendar.getInstance().get(Calendar.DAY_OF_MONTH);

			public void onDateChanged(DatePicker view, int year, int monthOfYear, int dayOfMonth)
			{
				if (year != oldyear || oldmonth != monthOfYear || oldday != dayOfMonth)
				{
					oldyear = year;
					oldmonth = monthOfYear;
					oldday = dayOfMonth;
					// UpdateNewItems();
				}

			}
		});

		dtFinalizeDate.init(Calendar.getInstance().get(Calendar.YEAR), Calendar.getInstance().get(Calendar.MONTH), Calendar.getInstance().get(Calendar.DAY_OF_MONTH), new OnDateChangedListener()
		{
			int oldyear = Calendar.getInstance().get(Calendar.YEAR), oldmonth = Calendar.getInstance().get(Calendar.MONTH), oldday = Calendar.getInstance().get(Calendar.DAY_OF_MONTH);

			public void onDateChanged(DatePicker view, int year, int monthOfYear, int dayOfMonth)
			{
				if (year != oldyear || oldmonth != monthOfYear || oldday != dayOfMonth)
				{
					oldyear = year;
					oldmonth = monthOfYear;
					oldday = dayOfMonth;

					Calendar calendar = Calendar.getInstance();
					calendar.set(Calendar.MILLISECOND, 0); // Clear the millis
					// part. Silly API.
					calendar.set(year, monthOfYear, dayOfMonth + 1, 0, 0, 0); // Note
					// that
					// months
					// are
					// 0-based

					long millis = calendar.getTimeInMillis();

					currentDocumentHeader.setDocumentDate(Utilities.convertUnixTimestampToTicks(millis));
					try
					{
						dbHelper.getDocumentHeaders().update(currentDocumentHeader);
					}
					catch (Exception e)
					{
						Utilities.ShowUninterruptingMessage(mContext, getString(R.string.problem) + " :" + e.getMessage(), 10);
					}
				}

			}
		});

		lstNewItems.setOnItemClickListener(new OnItemClickListener()
		{

			public void onItemClick(AdapterView<?> arg0, View arg1, int arg2, long arg3)
			{
				if (arg3 >= 0)
				{
					try
					{
						AddItem(arg3, 1);

					}
					catch (Exception e)
					{
						e.printStackTrace();
						Utilities.ShowUninterruptingMessage(mContext, e.getMessage(), 7);
					}
				}
			}

		});
		lstItemInCategory.setOnItemClickListener(new OnItemClickListener()
		{

			public void onItemClick(AdapterView<?> arg0, View arg1, int arg2, long arg3)
			{
				if (arg3 >= 0)
				{
					try
					{
						AddItem(arg3, 0);

					}
					catch (Exception e)
					{
						e.printStackTrace();
						Utilities.ShowUninterruptingMessage(mContext, e.getMessage(), 7);
					}
				}
			}
		});
		btnShowNewitems.setOnClickListener(new View.OnClickListener()
		{

			public void onClick(View v)
			{

				UpdateNewItems(false);
			}
		});

		btnShowItems.setOnClickListener(new View.OnClickListener()
		{

			public void onClick(View v)
			{
				ShowItemsOfCategory();
			}
		});
		lstItemCategories.setOnItemClickListener(new OnItemClickListener()
		{

			public void onItemClick(AdapterView<?> arg0, View arg1, int arg2, long arg3)
			{
				SelectItemCategory(arg3, lstItemCategories);
			}
		});
		lstInfoItemCategories.setOnItemClickListener(new OnItemClickListener()
		{

			public void onItemClick(AdapterView<?> arg0, View arg1, int arg2, long arg3)
			{
				SelectItemCategory(arg3, lstInfoItemCategories);
			}
		});
		btnSaveSettings.setOnClickListener(new View.OnClickListener()
		{
			public void onClick(View v)
			{
				btnSaveSettingsOnClick();
			}
		});

		btnRevertSettings.setOnClickListener(new View.OnClickListener()
		{
			public void onClick(View v)
			{
				btnRevertSettingsOnClick();
			}
		});

		View.OnClickListener btnExitOnClick = new View.OnClickListener()
		{
			public void onClick(View v)
			{
				btnExitOnClick(v);
			}
		};
		View.OnClickListener btnLoginOnClick = new View.OnClickListener()
		{

			public void onClick(View v)
			{
				PerformLogin();
			}
		};

		View.OnClickListener btnOrdersOnClick = new View.OnClickListener()
		{

			public void onClick(View v)
			{

				AlertDialog ad = new AlertDialog.Builder(mContext).create();
				ad.setCancelable(false); // This blocks the 'BACK' button
				ad.setTitle("");
				ad.setMessage(mContext.getString(R.string.shouldUpdateConfirmation));
				ad.setButton(mContext.getString(android.R.string.no), new DialogInterface.OnClickListener()
				{
					public void onClick(DialogInterface dialog, int which)
					{
						dialog.dismiss();
						ShowOrdersTabs();
					}
				});

				ad.setButton2(mContext.getString(android.R.string.yes), new DialogInterface.OnClickListener()
				{
					public void onClick(DialogInterface dialog, int which)
					{
						dialog.dismiss();
						wsHelper.SynchronizeAll(loggedInUser);
					}
				});

				if (CheckIfUserNeedsUpdate(true))
				{
					ad.show();
				}
				else
				{
					ShowOrdersTabs();
				}
			}
		};

		chkAllowMultipleLines.setOnCheckedChangeListener(new OnCheckedChangeListener()
		{

			public void onCheckedChanged(CompoundButton buttonView, boolean isChecked)
			{
				currentDocumentHeader.setAllowMultipleLines(isChecked);
				try
				{
					dbHelper.getDocumentHeaders().update(currentDocumentHeader);
				}
				catch (SQLException e)
				{
					e.printStackTrace();
				}
				// getPreferences(mContext.MODE_PRIVATE).edit().putBoolean("allow_multiple_lines",
				// isChecked).commit();
			}
		});

		OnItemClickListener lstCustomerListener = new OnItemClickListener()
		{

			public void onItemClick(AdapterView<?> arg0, View arg1, int arg2, long arg3)
			{
				SelectCustomer(arg3);
			}
		};

		OnItemSelectedListener cmbDocumentStatusOnSelect = new OnItemSelectedListener()
		{

			public void onItemSelected(AdapterView<?> arg0, View arg1, int arg2, long arg3)
			{
				try
				{
					currentDocumentHeader.setDocumentStatus(docStatusArray[(int) arg3]);
					dbHelper.getDocumentHeaders().update(currentDocumentHeader);

				}
				catch (Exception ex)
				{
					ex.printStackTrace();
				}

			}

			public void onNothingSelected(AdapterView<?> arg0)
			{

			}

		};

		btnSendCurrentOrder.setOnClickListener(new View.OnClickListener()
		{

			public void onClick(View v)
			{
				AlertDialog ad = new AlertDialog.Builder(mContext).create();
				ad.setCancelable(false); // This blocks the 'BACK' button
				ad.setTitle("");
				ad.setMessage(mContext.getString(R.string.sendOrderConfirmation));
				ad.setButton(mContext.getString(android.R.string.cancel), new DialogInterface.OnClickListener()
				{
					public void onClick(DialogInterface dialog, int which)
					{
						dialog.dismiss();
					}
				});

				ad.setButton2(mContext.getString(android.R.string.ok), new DialogInterface.OnClickListener()
				{
					public void onClick(DialogInterface dialog, int which)
					{
						dialog.dismiss();
						if (SendOrder(currentDocumentHeader, true, true))
						{
							goBackToCustomersView(true);
							// onBackPressed();
							currentDocumentHeader = null;
						}
					}
				});
				ad.show();
			}
		});

		// *
		txtCode.setOnKeyListener(new View.OnKeyListener()
		{

			public boolean onKey(View arg0, int arg1, KeyEvent arg2)
			{
				if (arg2.getKeyCode() == KeyEvent.KEYCODE_ENTER && arg2.getAction() == KeyEvent.ACTION_DOWN)
				{
					txtCodeEnterPressed(true);
					return true;
				}
				return false;

			}
		});
		txtCheckPriceCode.setOnKeyListener(new View.OnKeyListener()
		{

			public boolean onKey(View arg0, int arg1, KeyEvent arg2)
			{
				if (arg2.getKeyCode() == KeyEvent.KEYCODE_ENTER && arg2.getAction() == KeyEvent.ACTION_DOWN)
				{
					txtCheckPriceCodeOnEnterPressed();
					return true;
				}
				return false;

			}
		});

		/*
		 * /
		 * 
		 * txtCode.setOnEditorActionListener(new OnEditorActionListener() {
		 * 
		 * public boolean onEditorAction(TextView v, int actionId, KeyEvent
		 * event) { if (actionId == EditorInfo.IME_NULL && event.getKeyCode() ==
		 * KeyEvent.KEYCODE_ENTER) { // SearchItem(); SearchItemOffline(); if
		 * (currentItem != null) { getQtyPopup(); } } return false; } });//
		 */
		txtQty.setOnEditorActionListener(new OnEditorActionListener()
		{

			public boolean onEditorAction(TextView v, int actionId, KeyEvent event)
			{
				if (actionId == EditorInfo.IME_ACTION_NEXT || ((event != null) && event.getKeyCode() == KeyEvent.KEYCODE_ENTER))
				{
					// AddDocumentDetail();
					AddDocumentDetailOffline();
				}
				return true;
			}
		});

		btnUpdate.setOnClickListener(new View.OnClickListener()
		{

			public void onClick(View v)
			{
				wsHelper.SynchronizeAll(loggedInUser);
			}
		});

		btnSettings.setOnClickListener(new View.OnClickListener()
		{

			public void onClick(View v)
			{
				tabMain.setVisibility(View.GONE);
				tabSettings.setVisibility(View.VISIBLE);
				tabSettings.performClick();
			}
		});

		cmbBluetoothDevices.setOnItemSelectedListener(new OnItemSelectedListener()
		{

			public void onItemSelected(AdapterView<?> arg0, View arg1, int arg2, long arg3)
			{
				bluetoothThread.interrupt();
				bluetoothThread = new BluetoothConnector(cmbBluetoothDevices.getSelectedItem().toString());
				bluetoothThread.addBluetoothScanningEventListener(bluetoothThreadListener);
				bluetoothThread.start();
			}

			public void onNothingSelected(AdapterView<?> arg0)
			{

			}

		});

		cmbNewItems.setOnItemSelectedListener(new OnItemSelectedListener()
		{

			public void onItemSelected(AdapterView<?> arg0, View arg1, int arg2, long arg3)
			{
				UpdateNewItems(true);
			}

			public void onNothingSelected(AdapterView<?> arg0)
			{
				// TODO Auto-generated method stub

			}
		});

		cmbStore.setOnItemSelectedListener(new OnItemSelectedListener()
		{

			public void onItemSelected(AdapterView<?> arg0, View arg1, int arg2, long arg3)
			{
				UpdateStore();
			}

			public void onNothingSelected(AdapterView<?> arg0)
			{
				// TODO Auto-generated method stub

			}
		});

		btnScan.setOnClickListener(new View.OnClickListener()
		{

			public void onClick(View arg0)
			{
				InitiateCameraScan();

			}
		});

		btnCheckPriceScan.setOnClickListener(new View.OnClickListener()
		{

			public void onClick(View arg0)
			{
				// TODO Auto-generated method stub
				InitiateCameraScan();
			}
		});

		btnUploadAll.setOnClickListener(new View.OnClickListener()
		{

			public void onClick(View arg0)
			{

				List<DocumentHeader> allHeaders;
				try
				{
					allHeaders = dbHelper.getDocumentHeaders().queryForAll();
				}
				catch (SQLException e)
				{
					e.printStackTrace();
					return;
				}

				int totalOrdersCount = 0;
				for (DocumentHeader header : allHeaders)
				{
					if (header.getDetails().size() > 0)
					{
						totalOrdersCount++;
					}
				}
				if (totalOrdersCount == 0) // no orders to send, display message
				{

					AlertDialog ad = new AlertDialog.Builder(mContext).create();
					ad.setCancelable(false); // This blocks the 'BACK' button
					ad.setMessage(getResources().getString(R.string.noOrders));
					ad.setButton(getString(R.string.ok), new DialogInterface.OnClickListener()
					{
						public void onClick(DialogInterface dialog, int which)
						{
							dialog.dismiss();
						}
					});
					ad.show();
				}
				else
				{
					AlertDialog ad = new AlertDialog.Builder(mContext).create();
					ad.setCancelable(false); // This blocks the 'BACK' button
					ad.setMessage(getResources().getString(R.string.ordersFoundMessage).replace("var1", "" + totalOrdersCount));
					ad.setButton2(getString(R.string.ok), new DialogInterface.OnClickListener()
					{
						public void onClick(DialogInterface dialog, int which)
						{

							UploadAllDocuments();
							dialog.dismiss();
						}
					});
					ad.setButton(getResources().getString(R.string.cancel), new DialogInterface.OnClickListener()
					{
						public void onClick(DialogInterface dialog, int which)
						{
							dialog.dismiss();
						}
					});

					ad.show();
				}

			}
		});

		btnExportDB.setOnClickListener(new View.OnClickListener()
		{

			public void onClick(View v)
			{
				try
				{
					ExportDatabaseFileTask exporter = dbHelper.new ExportDatabaseFileTask();
					exporter.execute();

				}
				catch (Exception e)
				{
					AlertDialog ad = new AlertDialog.Builder(mContext).create();
					ad.setCancelable(false); // This blocks the 'BACK' button
					ad.setMessage("Export Failed: " + e.getMessage());
					ad.setButton(getString(R.string.ok), new DialogInterface.OnClickListener()
					{
						public void onClick(DialogInterface dialog, int which)
						{
							dialog.dismiss();
						}
					});
					ad.show();

				}

			}
		});

		btnImportDB.setOnClickListener(new View.OnClickListener()
		{

			public void onClick(View arg0)
			{
				try
				{
					String importPath = txtDBImportPath.getText().toString();
					ImportDatabaseFileTask importTask = dbHelper.new ImportDatabaseFileTask();
					importTask.execute(importPath);

				}
				catch (Exception e)
				{
					AlertDialog ad = new AlertDialog.Builder(mContext).create();
					ad.setCancelable(false); // This blocks the 'BACK' button
					if (e instanceof IOException)
					{
						ad.setMessage("File not found.");
					}
					else
					{
						ad.setMessage("Import Failed, " + e.getMessage());
					}
					ad.setButton(getString(R.string.ok), new DialogInterface.OnClickListener()
					{
						public void onClick(DialogInterface dialog, int which)
						{
							dialog.dismiss();
						}
					});
					ad.show();
				}
			}
		});

		View.OnClickListener searchItemOnClickListener = new View.OnClickListener()
		{

			public void onClick(View v)
			{
				tabItemCategories.setVisibility(View.GONE);
				tabOrdersTotal.setVisibility(View.GONE);
				tabOrderByOffers.setVisibility(View.GONE);
				tabOrderByNewItems.setVisibility(View.GONE);
				tabOrders.setVisibility(View.GONE);
				tabItemSearchByName.setVisibility(View.VISIBLE);
				tabItemSearchByName.performClick();
				txtItemNameSearch.selectAll();
				txtItemNameSearch.requestFocus();
				InputMethodManager imm = (InputMethodManager) getSystemService(Context.INPUT_METHOD_SERVICE);
				imm.hideSoftInputFromWindow(getWindow().getCurrentFocus().getWindowToken(), 0);
			}
		};

		tvDescription.setOnClickListener(searchItemOnClickListener);

		txtDescription.setOnClickListener(searchItemOnClickListener);

		txtItemNameSearch.setOnKeyListener(new View.OnKeyListener()
		{

			public boolean onKey(View v, int keyCode, KeyEvent event)
			{
				if (event.getAction() == KeyEvent.ACTION_DOWN && event.getKeyCode() == KeyEvent.KEYCODE_ENTER)
				{
					btnSearchItemByName.performClick();
					return true;
				}
				return false;
			}
		});

		btnSearchItemByName.setOnClickListener(new View.OnClickListener()
		{

			public void onClick(View v)
			{
				if (SearchItemsByName() == false)
				{
					AlertDialog ad = new AlertDialog.Builder(mContext).create();
					ad.setCancelable(false); // This blocks the 'BACK' button
					ad.setMessage(getResources().getString(R.string.itemSearchFailMessage));
					ad.setButton(getString(R.string.ok), new DialogInterface.OnClickListener()
					{
						public void onClick(DialogInterface dialog, int which)
						{
							dialog.dismiss();
						}
					});
					ad.show();
				}
			}
		});

		lstItemsByNameResult.setOnItemClickListener(new OnItemClickListener()
		{

			public void onItemClick(AdapterView<?> arg0, View arg1, int arg2, long arg3)
			{
				if (arg3 >= 0)
				{
					try
					{
						AddItem(arg3, 2);

					}
					catch (Exception e)
					{
						e.printStackTrace();
						Utilities.ShowUninterruptingMessage(mContext, e.getMessage(), 7);
					}

				}
			}
		});

		btnAbout.setOnClickListener(new View.OnClickListener()
		{

			public void onClick(View arg0)
			{
				tabMain.setVisibility(View.GONE);
				tabAbout.setVisibility(View.VISIBLE);
				tabAbout.performClick();
			}
		});

		lstOffers.setOnItemClickListener(new OnItemClickListener()
		{

			public void onItemClick(AdapterView<?> parent, View view, int position, long ID)
			{
				SelectOffer((int) ID);
			}

		});

		lstItemsInOffer.setOnItemClickListener(new OnItemClickListener()
		{
			public void onItemClick(AdapterView<?> arg0, View arg1, int arg2, long arg3)
			{
				if (arg3 >= 0)
				{
					try
					{
						AddItem(arg3, 3);

					}
					catch (Exception e)
					{
						e.printStackTrace();
						Utilities.ShowUninterruptingMessage(mContext, e.getMessage(), 7);
					}
				}
			}

		});

		txtAddress.addTextChangedListener(new TextWatcher()
		{

			public void onTextChanged(CharSequence s, int start, int before, int count)
			{

			}

			public void beforeTextChanged(CharSequence s, int start, int count, int after)
			{

			}

			public void afterTextChanged(Editable s)
			{				
				String adr = (txtAddress.getText()==null)?"":txtAddress.getText().toString();
				currentDocumentHeader.setDeliveryAddress(adr);
				try
				{
					dbHelper.getDocumentHeaders().update(currentDocumentHeader);
				}
				catch (SQLException e)
				{
					e.printStackTrace();
				}
			}
		});
		txtComments.addTextChangedListener(new TextWatcher()
		{

			public void onTextChanged(CharSequence s, int start, int before, int count)
			{
				// TODO Auto-generated method stub

			}

			public void beforeTextChanged(CharSequence s, int start, int count, int after)
			{
				// TODO Auto-generated method stub

			}

			public void afterTextChanged(Editable s)
			{
				currentDocumentHeader.setComments(txtComments.getText().toString());
				try
				{
					dbHelper.getDocumentHeaders().update(currentDocumentHeader);
				}
				catch (SQLException e)
				{
					e.printStackTrace();
				}

			}
		});

		// txtAddress.setOnFocusChangeListener(new View.OnFocusChangeListener()
		// {
		//
		// public void onFocusChange(View v, boolean hasFocus)
		// {
		// if (!hasFocus)
		// {
		// currentDocumentHeader.setDeliveryAddress(txtAddress.getText().toString());
		// try
		// {
		// dbHelper.getDocumentHeaders().update(currentDocumentHeader);
		// }
		// catch (SQLException e)
		// {
		// e.printStackTrace();
		// }
		// }
		//
		// }
		// });

		this.lstInfoItemList.setOnItemClickListener(new OnItemClickListener()
		{
			public void onItemClick(AdapterView<?> parent, View view, int position, long ID)
			{
				Item item = (Item) lstInfoItemList.getItemAtPosition(position);
				DisplayItemInfo(item);
			}
		});

		cmbDocumentStatus.setOnItemSelectedListener(cmbDocumentStatusOnSelect);
		btnOrders.setOnClickListener(btnOrdersOnClick);
		lstCustomers.setOnItemClickListener(lstCustomerListener);
		btnLogin.setOnClickListener(btnLoginOnClick);
		btnExit.setOnClickListener(btnExitOnClick);
		btnExitMenu.setOnClickListener(btnExitOnClick);

	}

	private boolean SearchItemsByName()
	{
		if (txtItemNameSearch.getText() != null && txtItemNameSearch.getText().length() > 2)
		{
			try
			{
				QueryBuilder<Item, Long> qb = dbHelper.getItems().queryBuilder();
				qb.where().like("loweCaseName", "%" + txtItemNameSearch.getText().toString().toLowerCase() + "%");
				PreparedQuery<Item> pq = qb.prepare();
				List<Item> itemsFound = dbHelper.getItems().query(pq);
				itemsByNameResult = itemsFound;
				itemPriceItemsByNameResult = dbHelper.filterItemsForPriceCatalog(itemsByNameResult, selectedCustomer.getPc());
				RetailHelper.GetItemsExistingQuantity(itemPriceItemsByNameResult, currentDocumentHeader, dbHelper);
				lstItemsByNameResult.setAdapter(new ItemCustomAdapter(mContext, R.layout.item_row, itemPriceItemsByNameResult, true));
				if (itemPriceItemsByNameResult.size() == 0)
					Utilities.ShowUninterruptingMessage(this, getResources().getString(R.string.noItemsFound), 6);
				return true;
			}
			catch (Exception e)
			{
				e.printStackTrace();
				return false;
			}
		}
		else
		{
			return false;
		}
	}

	private void getQtyPopup(double quantity, double discountValue)
	{
		isQtyPopupShowing = true;
		final AlertDialog.Builder alertb = new AlertDialog.Builder(mContext);

		alertb.setTitle(/*
						 * getResources().getString(R.string.chooseQty) +
						 * ":\r\n" +
						 */currentItem.getName() + ", " + getResources().getString(R.string.price) + ": " + RetailHelper.euroValue.format(dbHelper.getItemPrice(selectedCustomer.getPc(), currentBarcode, currentItem)) + " " + getResources().getString(R.string.packingQty) + ": "
				+ (int) currentItem.getPackingQty());
		alertb.setCancelable(false);
		alertb.setView(getLayoutInflater().inflate(R.layout.qty_dialog, null));
		final AlertDialog alert = alertb.create();
		alert.show();
		// Set an EditText view to get user input
		final EditText input = (EditText) alert.findViewById(R.id.txtQty);
		final EditText discount = (EditText) alert.findViewById(R.id.txtDiscount);
		final TextView lblDiscount = (TextView) alert.findViewById(R.id.lblDiscount);

		input.setInputType(InputType.TYPE_CLASS_NUMBER | InputType.TYPE_NUMBER_FLAG_DECIMAL);
		input.setText("" + RetailHelper.qtyFormat.format(quantity));
		discount.setInputType(InputType.TYPE_CLASS_NUMBER | InputType.TYPE_NUMBER_FLAG_DECIMAL);
		discount.setText("" + discountValue);
		MeasurementUnit mm = currentBarcode.getMeasurementUnit();
		if (mm.getDescription() == null)
		{
			try
			{
				mm = dbHelper.getMeasurementUnitDao().queryForId(mm.getID());
			}
			catch (Exception e)
			{
				e.printStackTrace();
			}
		}
		if (mm.isSupportDecimals())
		{
			input.setFilters(new InputFilter[] { new InputFilterDecimalMinMax(1, 999999, 2) });
		}
		else
		{
			input.setFilters(new InputFilter[] { new InputFilterIntegerMinMax(1, 999999) });
		}
		discount.setOnKeyListener(new View.OnKeyListener()
		{

			public boolean onKey(View v, int keyCode, KeyEvent event)
			{
				if (event.getAction() == KeyEvent.ACTION_UP && event.getKeyCode() == KeyEvent.KEYCODE_ENTER)
				{
					setQtyAndAddItem(input.getText().toString(), discount.getText().toString());
					alert.dismiss();
					isQtyPopupShowing = false;
					return true;
				}
				return false;
			}
		});

		((Button) alert.findViewById(R.id.btnOk)).setOnClickListener(new View.OnClickListener()
		{

			public void onClick(View v)
			{
				setQtyAndAddItem(input.getText().toString(), discount.getText().toString());
				isQtyPopupShowing = false;
				alert.dismiss();
			}
		});

		((Button) alert.findViewById(R.id.btnCancel)).setOnClickListener(new View.OnClickListener()
		{

			public void onClick(View v)
			{
				txtCode.setText("");
				txtDescription.setText("");
				txtPrice.setText("");
				txtQty.setText("");
				currentBarcode = null;
				currentItem = null;
				selectedCode = "";
				selectedQty = 0;

				isQtyPopupShowing = false;
				alert.dismiss();
			}
		});
		input.setSelectAllOnFocus(true);
		discount.setSelectAllOnFocus(true);
		input.selectAll();
		if (!appsettings.isDiscountPermited())
		{
			discount.setVisibility(View.GONE);
			lblDiscount.setVisibility(View.GONE);

		}

	}

	String categoryFilter, offerFilter, newItemFilter;

	private BroadcastReceiver btStateReceiver;

	private IntentFilter btStatefilter;

	private double selectedDiscount;

	private void getFilterPopup(String oldFilter, final int mode)
	{
		final AlertDialog alert = new AlertDialog.Builder(mContext).create();

		alert.setTitle(getResources().getString(R.string.Filter));
		// alert.setMessage("Change Quantity");

		// Set an EditText view to get user input
		final EditText input = new EditText(alert.getContext());
		input.setText(oldFilter);
		input.setOnKeyListener(new View.OnKeyListener()
		{

			public boolean onKey(View v, int keyCode, KeyEvent event)
			{
				if (event.getAction() == KeyEvent.ACTION_DOWN && event.getKeyCode() == KeyEvent.KEYCODE_ENTER)
				{
					// on ok
					if (mode == 0)
					{
						categoryFilter = input.getText().toString();
						ShowItemsOfCategory();
					}
					else if (mode == 2)
					{
						newItemFilter = input.getText().toString();
						UpdateNewItems(false);
					}
					else if (mode == 3)
					{
						categoryFilter = input.getText().toString();
						ShowInfoItemsOfCategory();
					}
					else
					{
						offerFilter = input.getText().toString();
						SelectOffer(-1);
					}
					alert.dismiss();
					return true;
				}
				return false;
			}
		});

		input.setSelectAllOnFocus(true);
		alert.setView(input);
		alert.setCancelable(false);
		alert.setButton2(getString(R.string.ok), new DialogInterface.OnClickListener()
		{
			public void onClick(DialogInterface dialog, int whichButton)
			{
				// on ok
				if (mode == 0)
				{
					categoryFilter = input.getText().toString();
					ShowItemsOfCategory();
				}
				else if (mode == 2)
				{
					newItemFilter = input.getText().toString();
					UpdateNewItems(false);
				}
				else if (mode == 3)
				{
					categoryFilter = input.getText().toString();
					ShowInfoItemsOfCategory();
				}
				else
				{
					offerFilter = input.getText().toString();
					SelectOffer(-1);
				}
				alert.dismiss();
			}
		});

		alert.setButton(getString(R.string.cancel), new DialogInterface.OnClickListener()
		{
			public void onClick(DialogInterface dialog, int whichButton)
			{
				// on ok
				alert.dismiss();
			}
		});

		alert.show();

	}

	private void getQtyPopup()
	{
		getQtyPopup(1.0, 0.0);
	}

	private void setQtyAndAddItem(String inputQuantity, String discountStr)
	{
		try
		{
			selectedQty = Double.parseDouble(inputQuantity);
			selectedDiscount = Double.parseDouble(discountStr) / 100;
//			if (currentItem.getMaxOrderQty() > 0 && currentItem.getMaxOrderQty() < selectedQty)
//			{
//				throw new Exception(getResources().getString(R.string.invalidQty) + "\n" + getResources().getString(R.string.maxOrderQtyExceeded).replace("var1", "" + currentItem.getMaxOrderQty()));
//			}
			if (selectedQty >= 0)
			{
				DocumentDetail existingDetail = DocumentHelper.checkIfBarcodeExists(currentDocumentHeader, currentBarcode, currentItem, dbHelper, appsettings);
				if (existingDetail == null || chkAllowMultipleLines.isChecked())
				{
					AddDocumentDetailOffline();
				}
				else
				{
					UpdateDocumentDetail(existingDetail);
				}
			}

		}
		catch (Exception e)
		{
			AlertDialog ad = new AlertDialog.Builder(mContext).create();
			ad.setCancelable(false); // This blocks the 'BACK' button
			ad.setMessage(e.getMessage());
			ad.setButton(getString(R.string.ok), new DialogInterface.OnClickListener()
			{
				public void onClick(DialogInterface dialog, int which)
				{
					dialog.dismiss();
				}
			});
			ad.show();
			selectedQty = -1;

		}

	}

	private void btnExitOnClick(View v)
	{
		ExitApplication();
	}

	private void txtCodeEnterPressed(boolean showPopup)
	{
		txtCodeEnterPressed(-1, showPopup);
	}

	private void txtCheckPriceCodeOnEnterPressed()
	{
		SearchItemCheckPrice();
		if (currentItem != null)
		{
			try
			{
				PriceCatalogDetail pcDetail = dbHelper.getItemPriceDetail(selectedCustomer.getPc(), currentBarcode, currentItem);
				DocumentDetail dummyDetail = DocumentHelper.computeDocumentLine(selectedCustomer, currentBarcode, 1, pcDetail.getDiscount(), 0, dbHelper, false, appsettings);
				txtCheckPriceValue.setText("" + RetailHelper.euroNormal.format(dummyDetail.getGrossTotal()));
				txtCheckPriceDiscount.setText("" + RetailHelper.percent.format(dummyDetail.getFirstDiscount()));
				txtCheckPriceCode.setText("");
			}
			catch (Exception e)
			{
				e.printStackTrace();
			}
		}
		else
		{
			Utilities.ShowUninterruptingMessage(mContext, getString(R.string.itemNotFound), 3);
		}

	}

	private void txtCodeEnterPressed(double defaultqty, boolean showPopup)
	{
		SearchItemOffline();
		if (currentItem != null)
		{
			try
			{
				DocumentDetail existingDetail = DocumentHelper.checkIfBarcodeExists(currentDocumentHeader, currentBarcode, currentItem, dbHelper, appsettings);
				if (existingDetail == null || chkAllowMultipleLines.isChecked())
				{
					if (showPopup)
						getQtyPopup();
					else
						setQtyAndAddItem("" + defaultqty, "0");
				}
				else
				{
					if (showPopup)
						getQtyPopup(existingDetail.getQty(), existingDetail.getSecondDiscount() * 100);
				}
			}
			catch (Exception e)
			{
				e.printStackTrace();
			}
			if (defaultqty >= 0)
			{
				Handler refresh = new Handler(Looper.getMainLooper());
				refresh.post(new Runnable()
				{
					public void run()
					{
						txtCode.setText("");
					}
				});
			}
		}

	}

	private String GetFormat(int digits)
	{
		String format = "#,##0";
		for (int i = 0; i < digits; i++)
		{
			if (i == 0)
			{
				format += ".";
			}
			format += "0";
		}
		format += " \u20AC";
		return format;
	}

	@Override
	public void onCreate(Bundle savedInstanceState)
	{
		super.onCreate(savedInstanceState);
		// Setting image file path
		IMAGES_PATH = Environment.getExternalStorageDirectory().toString() + "/retail/images/";
		File tmpImageFile = new File(IMAGES_PATH + "empty");
		tmpImageFile.mkdirs();
		tmpImageFile = null;
		// end image file path

		mContext = this;
		isDBInitialized = getPreferences(MODE_PRIVATE).getBoolean("init", false);
		dbHelper = new DatabaseHelper(mContext);

		wsHelper = new WebServiceHelper(dbHelper, mContext);

		try
		{
			usersDao = dbHelper.getUsers();
			customerDao = dbHelper.getCustomers();
			itemDao = dbHelper.getItems();
			documentHeadersDao = dbHelper.getDocumentHeaders();
			// dbHelper.getWritableDatabase().rawQuery("PRAGMA journal_mode = WAL ;",null);
			dbHelper.getWritableDatabase().rawQuery("PRAGMA cache_size=10000  ;", null);
			dbHelper.getWritableDatabase().rawQuery("PRAGMA automatic_index =false  ;", null);
			dbHelper.getWritableDatabase().rawQuery("PRAGMA  ignore_check_constraints  =true  ;", null);
			dbHelper.getWritableDatabase().rawQuery("PRAGMA temp_store = MEMORY;", null);

			// dbHelper.getWritableDatabase().execSQL("ALTER TABLE 'documentheader' ADD COLUMN 'RemoteDeviceDocumentHeaderGuid' VARCHAR");

			appsettings = dbHelper.getSettings();
			if (appsettings != null)
			{
				RetailHelper.euroNormal = new DecimalFormat(GetFormat(appsettings.getDisplayDigits()));
				RetailHelper.euroValue = new DecimalFormat(GetFormat(appsettings.getDisplayValueDigits()));

				File file = new File(appsettings.getPathToXSL());
				if (!file.exists())
				{
					try
					{
						InputStream in = getResources().openRawResource(R.raw.transform);
						FileOutputStream out = new FileOutputStream(appsettings.getPathToXSL());
						Utilities.copyFile(in, out);
					}
					catch (Exception e)
					{
						e.printStackTrace();
						Utilities.ShowUninterruptingMessage(this, e.getMessage(), 10);
					}
				}
			}

		}
		catch (SQLException e)
		{
			Log.e("Init: Database", "Could not retrieve database");
			e.printStackTrace();
		}
		InitializeGUI();

		bluetoothThread = null;
		bluetoothThreadListener = new BluetoothScanningEventListener()
		{
			public void onBluetoothScan(BluetoothScanningEvent event, final String message)
			{
				if (tabOrders.getVisibility() == View.VISIBLE)
				{
					Handler refresh = new Handler(Looper.getMainLooper());
					refresh.post(new Runnable()
					{
						public void run()
						{
							if (isQtyPopupShowing)
								return;
							tabOrders.performClick();
							txtCode.setText(message.replace("\r", ""));
							int value = -1;
							boolean showPopup = true;
							if (chkBatch.isChecked())
							{
								showPopup = false;
								try
								{
									value = Integer.parseInt(txtDefaultQty.getText().toString());
								}
								catch (Exception e)
								{
									value = 1;
								}
								if (value <= 0)
								{
									value = 1;
								}
							}

							txtCodeEnterPressed(value, showPopup);
						}
					});
				}
				else if (tabCustomers.getVisibility() == View.VISIBLE)
				{
					Handler refresh = new Handler(Looper.getMainLooper());
					refresh.post(new Runnable()
					{
						public void run()
						{
							txtCustomerSearchBox.setText(message.replace("\r", ""));
							SearchCustomers();
						}
					});
				}
				else if (tabCheckPrice.getVisibility() == View.VISIBLE)
				{
					Handler refresh = new Handler(Looper.getMainLooper());
					refresh.post(new Runnable()
					{
						public void run()
						{
							txtCheckPriceCode.setText(message.replace("\r", ""));
							txtCheckPriceCodeOnEnterPressed();
						}
					});
				}
				else
				{
					ErrorBeep();
				}
				Log.i("Bluetooth", message);
			}
		};

		if (BluetoothAdapter.getDefaultAdapter() == null)
		{
			// NO BLUETOOTH ADAPTER AVAILABLE
		}
		else if (BluetoothAdapter.getDefaultAdapter().isEnabled())
		{
			List<String> devices = GetBluetoothDevices();
			if (devices != null && devices.size() > 0)
			{
				cmbBluetoothDevices.setAdapter(new ArrayAdapter(this, android.R.layout.simple_dropdown_item_1line, devices));

				if (appsettings == null || appsettings.getBluetoothDeviceName() == null || appsettings.getBluetoothDeviceName().isEmpty())
				{
					bluetoothThread = new BluetoothConnector(devices.get(0));
					cmbBluetoothDevices.setSelection(0);
				}
				else
				{
					bluetoothThread = new BluetoothConnector(appsettings.getBluetoothDeviceName());
					int index = devices.indexOf(appsettings.getBluetoothDeviceName());
					if (index != -1)
					{
						cmbBluetoothDevices.setSelection(index);
					}
				}
				if (bluetoothThread.getStatus() == BTScanState.INITIALIZED)
				{

					bluetoothThread.addBluetoothScanningEventListener(bluetoothThreadListener);
					bluetoothThread.start();
				}
			}
		}
		else
		{
			// BLUETOOTH IS INACTIVE
		}

		networkStateReceiver = new BroadcastReceiver()
		{

			@Override
			public void onReceive(Context arg0, Intent arg1)
			{
				wsHelper.UpdateStatus(mContext);
			}

		};

		btStateReceiver = new BroadcastReceiver()
		{

			@Override
			public void onReceive(Context arg0, Intent arg1)
			{
				try
				{
					// bluetooth state change
					if (BluetoothAdapter.ACTION_STATE_CHANGED == arg1.getAction())
					{
						if (BluetoothAdapter.getDefaultAdapter().isEnabled() && bluetoothThread != null)
						{
							bluetoothThread.interrupt();
							bluetoothThread = null;

						}
						else if ((!BluetoothAdapter.getDefaultAdapter().isEnabled()) && bluetoothThread == null)
						{
							List<String> devices = GetBluetoothDevices();
							if (devices != null && devices.size() > 0)
							{
								cmbBluetoothDevices.setAdapter(new ArrayAdapter(mContext, android.R.layout.simple_dropdown_item_1line, devices));

								if (appsettings == null || appsettings.getBluetoothDeviceName() == null || appsettings.getBluetoothDeviceName().isEmpty())
								{
									bluetoothThread = new BluetoothConnector(devices.get(0));
									cmbBluetoothDevices.setSelection(0);
								}
								else
								{
									bluetoothThread = new BluetoothConnector(appsettings.getBluetoothDeviceName());
									int index = devices.indexOf(appsettings.getBluetoothDeviceName());
									if (index != -1)
									{
										cmbBluetoothDevices.setSelection(index);
									}
								}
								if (bluetoothThread.getStatus() == BTScanState.INITIALIZED)
								{

									bluetoothThread.addBluetoothScanningEventListener(bluetoothThreadListener);
									bluetoothThread.start();
								}
							}
						}
					}
				}
				catch (Exception ex)
				{
					ex.printStackTrace();
				}
			}

		};

		networkStatefilter = new IntentFilter(ConnectivityManager.CONNECTIVITY_ACTION);
		registerReceiver(networkStateReceiver, networkStatefilter);
		btStatefilter = new IntentFilter(BluetoothAdapter.ACTION_STATE_CHANGED);
		registerReceiver(btStateReceiver, btStatefilter);

	}

	@Override
	public boolean dispatchTouchEvent(MotionEvent event)
	{
		View v = getCurrentFocus();
		boolean ret = super.dispatchTouchEvent(event);

		if (v instanceof EditText)
		{
			View w = getCurrentFocus();
			int scrcoords[] = new int[2];
			w.getLocationOnScreen(scrcoords);
			float x = event.getRawX() + w.getLeft() - scrcoords[0];
			float y = event.getRawY() + w.getTop() - scrcoords[1];

			if (event.getAction() == MotionEvent.ACTION_UP && (x < w.getLeft() || x >= w.getRight() || y < w.getTop() || y > w.getBottom()))
			{
				InputMethodManager imm = (InputMethodManager) getSystemService(Context.INPUT_METHOD_SERVICE);
				imm.hideSoftInputFromWindow(getWindow().getCurrentFocus().getWindowToken(), 0);
			}
		}
		return ret;
	}

	@Override
	public boolean onCreateOptionsMenu(Menu menu)
	{
		getMenuInflater().inflate(R.menu.activity_main, menu);
		return true;
	}

	public void onTabReselected(Tab arg0, FragmentTransaction arg1)
	{
		int r = 0;
		r++;
		Log.v("Dummy", "" + r);
	}

	public void onTabSelected(Tab tab, FragmentTransaction ft)
	{
		int r = 0;
		r++;
		Log.v("Dummy", "" + r);
	}

	public void onTabUnselected(Tab tab, FragmentTransaction ft)
	{
		int r = 0;
		r++;
		Log.v("Dummy", "" + r);

	}

	public void SetImage(ImageView imageView, String imageOid, int size)
	{

		if (!imageOid.equals("00000000-0000-0000-0000-000000000000"))
		{
			String img = MainActivity.IMAGES_PATH + size + "/img_" + imageOid.replace("-", "") + ".png";
			Drawable d = Drawable.createFromPath(img);
			imageView.setImageDrawable(d);
		}
		else
		{
			imageView.setImageResource(R.raw.no_image_small);
		}

	}

	public void DisplayItemInfo(Item item)
	{
		try
		{
			if (isOnItemView)
			{
				tabItemList.setVisibility(View.GONE);
			}
			tabItemInfo.setVisibility(View.VISIBLE);
			tabItemInfo.performClick();

			isOnItemView = true;

			this.SetImage(itemInfoImage, item.getImageOid(), 128);

			// reload if needed
			if (item.getCode() == null)
			{

				item = dbHelper.getItems().queryForId(item.getID());

			}

			Barcode defaultBC = item.getDefaultBarcodeObject();
			// reload if needed
			if (defaultBC.getCode() == null)
			{
				defaultBC = dbHelper.getBarcodes().queryForId(defaultBC.getID());
			}

			MeasurementUnit mu = defaultBC.getMeasurementUnit();

			// reload if needed
			if (mu != null && mu.getDescription() == null)
			{
				mu = dbHelper.getMeasurementUnitDao().queryForId(mu.getID());
			}

			VATCategory vc = item.getVatCategory();
			// reload if needed
			if (vc != null && vc.getName() == null)
			{
				vc = dbHelper.getVATCategoryDao().queryForId(vc.getID());
			}

			txtItemInfoCode.setText(Utilities.TrimLeft(item.getCode(), '0'));
			txtItemInfoBarcode.setText(defaultBC.getCode());
			txtItemInfoDescription.setText(item.getName());
			TableRow tableRow12 = (TableRow) findViewById(R.id.tableRow12);
			TableRow tableRow13 = (TableRow) findViewById(R.id.tableRow13);
			TableRow tableRow15 = (TableRow) findViewById(R.id.tableRow15);

			tableRow12.setVisibility(View.GONE);
			tableRow13.setVisibility(View.GONE);
			tableRow15.setVisibility(View.GONE);

			// txtItemInfoGrossTotal.setText("" +
			// RetailHelper.euroNormal.format(detail.getGrossTotal()));
			// txtItemInfoQty.setText("" +
			// RetailHelper.qtyFormat.format(detail.getQty()));
			// txtItemInfoUnitPrice.setText("" +
			// RetailHelper.euroValue.format(detail.getItemPrice()));
			// txtItemInfoVat.setText("" +
			// RetailHelper.euroNormal.format(detail.getTotalVatAmount()));
			txtItemInfoVat.setText("" + vc.getName());
			txtItemInfoPackingQty.setText("" + (int) item.getPackingQty());
			txtItemInfoMaxOrderQty.setText("-");
			txtItemInfoMeasurementUnit.setText("" + (mu == null ? "" : mu.getDescription()));
			// txtItemInfoTotalDiscount.setText("" + detail.getTotalDiscount());
		}
		catch (SQLException e)
		{
			e.printStackTrace();
			Utilities.ShowUninterruptingMessage(this, "An Error Occured" + e.getMessage(), 5);
		}
	}

	public void DisplayItemInfo(DocumentDetail detail)
	{
		try
		{
			Item item = detail.getItem();
			MeasurementUnit mu = dbHelper.getBarcodeByCode(detail.getBarcode(), appsettings.getBarcodePadLength(), appsettings.getBarcodePadChar()).getMeasurementUnit();
			// reload if needed
			if (item.getCode() == null)
			{
				item = dbHelper.getItems().queryForId(item.getID());
			}

			// reload if needed
			if (mu != null && mu.getDescription() == null)
			{
				mu = dbHelper.getMeasurementUnitDao().queryForId(mu.getID());
			}

			tabItemCategories.setVisibility(View.GONE);
			tabOrdersTotal.setVisibility(View.GONE);
			tabOrderByOffers.setVisibility(View.GONE);
			tabOrderByNewItems.setVisibility(View.GONE);
			tabOrders.setVisibility(View.GONE);
			isOnItemView = false;
			// previousTab = tabOrders;
			tabItemInfo.setVisibility(View.VISIBLE);
			tabItemInfo.performClick();

			this.SetImage(itemInfoImage, item.getImageOid(), 128);

			TableRow tableRow12 = (TableRow) findViewById(R.id.tableRow12);
			TableRow tableRow13 = (TableRow) findViewById(R.id.tableRow12);
			TableRow tableRow15 = (TableRow) findViewById(R.id.tableRow15);

			tableRow12.setVisibility(View.VISIBLE);
			tableRow13.setVisibility(View.VISIBLE);
			tableRow15.setVisibility(View.VISIBLE);

			txtItemInfoCode.setText(Utilities.TrimLeft(item.getCode(), '0'));
			txtItemInfoBarcode.setText(detail.getBarcode());
			txtItemInfoDescription.setText(item.getName());
			txtItemInfoGrossTotal.setText("" + RetailHelper.euroNormal.format(detail.getGrossTotal()));
			txtItemInfoQty.setText("" + RetailHelper.qtyFormat.format(detail.getQty()));
			txtItemInfoUnitPrice.setText("" + RetailHelper.euroValue.format(detail.getItemPrice()));
			txtItemInfoVat.setText("" + RetailHelper.euroNormal.format(detail.getTotalVatAmount()));
			txtItemInfoVatCategory.setText("" + RetailHelper.percent.format(detail.getVatFactor()));
			txtItemInfoPackingQty.setText("" + (int) item.getPackingQty());
			txtItemInfoMaxOrderQty.setText("-");
			txtItemInfoMeasurementUnit.setText("" + (mu == null ? "" : mu.getDescription()));
			txtItemInfoTotalDiscount.setText("" + RetailHelper.euroNormal.format(detail.getTotalDiscount()));

		}
		catch (SQLException e)
		{
			e.printStackTrace();
			Utilities.ShowUninterruptingMessage(this, "An Error Occured" + e.getMessage(), 5);
		}

	}

	public void UploadAllDocuments()
	{
		List<DocumentHeader> allHeadersOfUser;
		try
		{
			allHeadersOfUser = dbHelper.findObjects(DocumentHeader.class, "CreatedBy", loggedInUser.getRemoteGuid(), FilterType.EQUALS);
			List<DocumentHeader> nonEmptyHeaders = new ArrayList<DocumentHeader>();
			List<DocumentHeader> validHeaders = new ArrayList<DocumentHeader>();
			int totalOrdersCount = 0;
			for (DocumentHeader header : allHeadersOfUser)
			{
				if (header.getDetails().size() > 0)
				{
					nonEmptyHeaders.add(header);
					totalOrdersCount++;
				}
			}
			int errorsCounter = 0;

			List<String> invalidItemMessageLines = new ArrayList<String>();
			List<String> zeroQuantitiesMessageLines = new ArrayList<String>();

			// Validate the orders with the data on the server
			for (DocumentHeader header : nonEmptyHeaders)
			{
				List<DocumentDetail> detailsWithZeroQtys = CheckForZeroQuantities(header);
				String toSend = RetailHelper.PrepareXmlToSend(header, dbHelper);
				List<InvalidItem> result = wsHelper.ValidateOrder(toSend);

				if (detailsWithZeroQtys == null || detailsWithZeroQtys.size() > 0)
				{
					for (DocumentDetail detail : detailsWithZeroQtys)
					{
						Item itemOfDetail = detail.getItem();
						if (itemOfDetail.getRemoteOid() == null)
						{
							itemOfDetail = dbHelper.getItems().queryForId(itemOfDetail.getID());
						}
						String message = "<td>" + header.getCustomer().getCompanyName() + "</td><td>" + itemOfDetail.getCode() + "</td>";
						zeroQuantitiesMessageLines.add(message);
					}

				}
				else if (result != null && result.size() > 0)
				{
					for (InvalidItem invalidItem : result)
					{
						String invalidItemMessage = "<td>" + header.getCustomer().getCompanyName() + "</td><td>" + dbHelper.getItemByRemoteGuid(invalidItem.getItemRemoteGuid().toString()).getCode() + "</td>";
						switch (invalidItem.getReason())
						{
						case INACTIVE:
							invalidItemMessage += "<td>" + getString(R.string.itemIsInactive) + "</td>";
							break;
						case NOPRICE:
							invalidItemMessage += "<td>" + getString(R.string.noPrice) + "</td>";
							break;
						}
						invalidItemMessageLines.add(invalidItemMessage);

					}

				}
				else if (result == null)
				{
					Utilities.ShowSimpleDialog(this, getString(R.string.noConnectionMessage));
					return;
				}
				else
				{
					validHeaders.add(header);
				}
			}

			for (DocumentHeader header : validHeaders)
			{
				if (SendOrder(header, false, false) == false)
				{
					errorsCounter++;
				}

			}
			String message = "";

			// Zero quantities found, display message and return
			if (zeroQuantitiesMessageLines.size() > 0)
			{
				message = "<b>" + getString(R.string.zeroQtysMultipleOrdersMessage) + "</b>:<br/><table width='100%'><tr><td>" + getString(R.string.customer) + "</td><td>" + getString(R.string.itemCode) + "</td></tr>";
				for (String line : zeroQuantitiesMessageLines)
				{
					message += "<tr>" + line + "</tr>";
				}
				message += "</table>";

				Utilities.showWebViewDialog(this, message, getString(R.string.ok));
			}
			// Invalid items found, display message and return
			else if (invalidItemMessageLines.size() > 0)
			{
				message = "<b>" + getString(R.string.invalidOrderMessage) + "</b>:<br/><table width='100%'><tr><td>" + getString(R.string.customer) + "</td><td>" + getString(R.string.itemCode) + "</td><td>" + getString(R.string.problem)
						+ "</td></tr><tr><td colspan='3'><hr></td></tr>";
				for (String line : invalidItemMessageLines)
				{
					message += "<tr>" + line + "</tr>";
				}
				message += "</table>";

				Utilities.showWebViewDialog(this, message, getString(R.string.ok));
			}
			else if (errorsCounter > 0)
			{
				message = getResources().getString(R.string.uploadAllOrdersReportWithErrors);
				message = message.replace("var1", "" + (totalOrdersCount - errorsCounter)).replace("var2", "" + errorsCounter);
				Utilities.ShowSimpleDialog(this, message);

			}
			else
			{
				message = getResources().getString(R.string.uploadAllOrdersReportNoErrors);
				message = message.replace("var1", "" + totalOrdersCount);
				Utilities.ShowSimpleDialog(this, message);
			}

		}
		catch (SQLException e)
		{
			// TODO Auto-generated catch block
			e.printStackTrace();
		}

	}

	@Override
	public void finish()
	{
		if (bluetoothThread != null)
		{
			bluetoothThread.interrupt();
			bluetoothThread = null;
		}
		super.finish();
	}

	public void invalidateDocumentDetailGrid()
	{
		docDetailsView.invalidate();
	}

	protected void InitializeOrderTabs()
	{
		selectedCategory = null;
		selectedOffer = null;
		itemCategoryList = dbHelper.getItemCategoryByParentID((selectedCategory == null) ? null : selectedCategory.getID());
		lstItemCategories.setAdapter(new ArrayAdapter<ItemCategory>(this, android.R.layout.simple_list_item_1, itemCategoryList));

		String[] strCmbNewItems = { getResources().getString(R.string.day), getResources().getString(R.string.week), getResources().getString(R.string.month), getResources().getString(R.string.specify) };
		cmbNewItems.setAdapter(new ArrayAdapter<String>(this, android.R.layout.simple_dropdown_item_1line, strCmbNewItems));

		Address[] addresses = null;
		if (selectedCustomer != null)
		{
			try
			{
				selectedCustomer = dbHelper.getCustomers().queryForId(selectedCustomer.getID());
				addresses = new Address[selectedCustomer.getAddresses().size()];
				addresses = selectedCustomer.getAddresses().toArray(addresses);
			}
			catch (SQLException e)
			{
				e.printStackTrace();
			}
		}
		else
			addresses = new Address[0];
		cmbStore.setAdapter(new ArrayAdapter<Address>(this, android.R.layout.simple_dropdown_item_1line, addresses));

		//
		lstItemInCategory.setAdapter(new ItemCustomAdapter(this, R.layout.item_row, new ArrayList<ItemPrice>()));
		lstItemsInOffer.setAdapter(new ItemCustomAdapter(this, R.layout.item_row, new ArrayList<ItemPrice>()));
		lstNewItems.setAdapter(new ItemCustomAdapter(this, R.layout.item_row, new ArrayList<ItemPrice>()));
		if (currentDocumentHeader != null)
		{
			if (currentDocumentHeader.getDetails() == null || currentDocumentHeader.getDetails().size() == 0)
			{
				chkAllowMultipleLines.setChecked(appsettings.getDefaultAllowMultiLines());
			}
			else
			{
				chkAllowMultipleLines.setChecked(currentDocumentHeader.getAllowMultipleLines());
			}
		}
		//
	}

	protected void UpdateStore()
	{
		txtAddress.setText(cmbStore.getSelectedItem().toString());
	}

	protected void AddNewCustomer()
	{

		final AlertDialog.Builder alertb = new AlertDialog.Builder(mContext);

		alertb.setTitle(getResources().getString(R.string.AddNewCustomer));
		alertb.setCancelable(false);
		alertb.setView(getLayoutInflater().inflate(R.layout.addnewcustomer_dialog, null));
		final AlertDialog alert = alertb.create();
		alert.show();
		// Set an EditText view to get user input
		final EditText customer_code = (EditText) alert.findViewById(R.id.txtCustDlgCode);
		final EditText customer_taxcode = (EditText) alert.findViewById(R.id.txtCustDlgTaxCode);

		final Spinner customer_pc = (Spinner) alert.findViewById(R.id.cmbDialogPriceCatalog);
		final Spinner customer_store = (Spinner) alert.findViewById(R.id.cmbDialogStore);
		customer_code.setInputType(InputType.TYPE_CLASS_NUMBER | InputType.TYPE_NUMBER_FLAG_DECIMAL);
		customer_code.setText("");
		customer_taxcode.setInputType(InputType.TYPE_CLASS_NUMBER | InputType.TYPE_NUMBER_FLAG_DECIMAL);
		customer_taxcode.setText("");

		customer_pc.setAdapter(new ArrayAdapter<PriceCatalog>(mContext, android.R.layout.simple_dropdown_item_1line, allPriceCatalogs));
		if (appsettings.getDefaultPriceCatalog() != null)
		{
			for (int i = 0; i < allPriceCatalogs.size(); i++)
			{
				if (allPriceCatalogs.get(i).getID() == appsettings.getDefaultPriceCatalog().getID())
				{
					customer_pc.setSelection(i);
					break;
				}
			}
			customer_pc.setEnabled(false);
		}
		List<Store> userStores = dbHelper.getLocalStoresOfUser(loggedInUser);
		customer_store.setAdapter(new ArrayAdapter<Store>(mContext, android.R.layout.simple_dropdown_item_1line, userStores));

		if (appsettings.getDefaultStore() != null)
		{
			boolean found = false;
			for (int i = 0; i < userStores.size(); i++)
			{
				if (userStores.get(i).getID() == appsettings.getDefaultStore().getID())
				{
					found = true;
					customer_store.setSelection(i);
					break;
				}
			}
			customer_store.setEnabled(!found);
		}

		((Button) alert.findViewById(R.id.btnCustDlgOk)).setOnClickListener(new View.OnClickListener()
		{
			public void onClick(View v)
			{
				if (customer_taxcode.getText().toString().length() == 0)
				{
					Utilities.ShowSimpleDialog(mContext, getResources().getString(R.string.TaxCodeRequired));
					return;
				}
				try
				{
					if (loggedInUser.getUsername() == null)
						loggedInUser = usersDao.queryForId(loggedInUser.getID());
				}
				catch (Exception ex)
				{
					ex.printStackTrace();
					// TODO
				}
				CustomerInsertResult ct = wsHelper.CreateCustomer(customer_taxcode.getText().toString(), customer_code.getText().toString(), ((PriceCatalog) customer_pc.getSelectedItem()).getRemoteGuid(), ((Store) customer_store.getSelectedItem()).getRemoteGuid(),
						loggedInUser.getRemoteGuid());
				switch (ct)
				{
				case CUSTOMERCODENOTEXIST:
					Utilities.ShowSimpleDialog(mContext, getResources().getString(R.string.CustomerCodeNotExist));
					return;
				case INVALIDUSER:
					Utilities.ShowSimpleDialog(mContext, getResources().getString(R.string.InvalidUser));
					return;
				case NETWORKERROR:
					Utilities.ShowSimpleDialog(mContext, getResources().getString(R.string.NetworkError));
					return;
				case GSIS_SERVICE_ERROR:
					Utilities.ShowSimpleDialog(mContext, getResources().getString(R.string.gsisError));
					return;
				case OTHERERROR:
					Utilities.ShowSimpleDialog(mContext, getResources().getString(R.string.problem));
					return;
				case PRICECATALOGNOTFOUND:
				case PRICECATALOGNOTINSTORE:
					Utilities.ShowSimpleDialog(mContext, getResources().getString(R.string.PriceCatalogProblem));
					alert.dismiss();
					return;
				case TAXCODEFOUNDNOACCESS:
				case USERHASNOSTOREACCESS:
					Utilities.ShowSimpleDialog(mContext, getResources().getString(R.string.AccessProblem));
					return;
				case TAXCODENOTFOUND:
					Utilities.ShowSimpleDialog(mContext, getResources().getString(R.string.TaxCodeNotFound));
					return;
				case TAXCODECREATED:
					Utilities.ShowSimpleDialog(mContext, getResources().getString(R.string.TaxCodeCreated));

					break;
				case TAXCODEEXISTS:
					Utilities.ShowSimpleDialog(mContext, getResources().getString(R.string.CustomerExists));
					break;
				}
				//
				alert.dismiss();
				Utilities.ShowSimpleDialog(mContext, getResources().getString(R.string.InitiatingUpdate));
				wsHelper.SynchronizeAll(loggedInUser);

			}
		});

		((Button) alert.findViewById(R.id.btnCustDlgCancel)).setOnClickListener(new View.OnClickListener()
		{
			public void onClick(View v)
			{
				alert.dismiss();
			}
		});
		customer_code.setSelectAllOnFocus(true);
		customer_taxcode.setSelectAllOnFocus(true);
		customer_code.selectAll();
	}

	private void UpdateDocumentHeaders(boolean clear)
	{
		List<TDocHead> list = new ArrayList<TDocHead>();
		if (!clear)
		{
			try
			{
				list = wsHelper.getDocumentHeaders(selectedCustomer, loggedInUser);

			}
			catch (Exception e)
			{
				Utilities.ShowUninterruptingMessage(this, getResources().getString(R.string.noConnectionMessage), 3);
			}
		}
		lstDocumentHeaders.setAdapter(new DocumentHeaderCustomAdapter(this, R.layout.document_header_row, list));
	}

	private void UpdateDocumentHeaders()
	{
		UpdateDocumentHeaders(false);
	}

	private void ShowOrdersTabs()
	{
		try
		{
			ResetCustomerList();
			isOnCustomerView = isOnItemView = false;
			// UpdateCustomers(false); NEEDS TEST
			customerListAdapter.setMarkCustomerOrders(true);
			lstCustomers.setAdapter(customerListAdapter);
			// new CustomerCustomAdapter(mContext, R.layout.customer_row,
			// customerList));
			tabLogin.setVisibility(View.GONE);
			tabMain.setVisibility(View.GONE);
			tabOrders.setVisibility(View.GONE);
			tabOrdersTotal.setVisibility(View.GONE);
			tabOrderByOffers.setVisibility(View.GONE);
			tabCustomers.setVisibility(View.VISIBLE);
			tabCustomers.performClick();
			
		}
		catch (Exception e)
		{
			// TODO: handle exception
			e.printStackTrace();
		}
	}

	private void ShowDocumentHeadersWebView()
	{

		GeneralAsyncTask task = new GeneralAsyncTask();

		ProgressDialog dialog = new ProgressDialog(this);
		dialog.setProgressStyle(ProgressDialog.STYLE_SPINNER);
		dialog.setCancelable(false);
		dialog.setMessage(getString(R.string.pleaseWait));

		task.setDialog(dialog);

		task.setRunner(new AsyncTaskRunnable()
		{

			public Object Run(Object... params)
			{
				try
				{
					String xml = null;
					if (chkUseFilters.isChecked())
					{
						Calendar gc = new GregorianCalendar(dtFromFilter.getYear(), dtFromFilter.getMonth(), dtFromFilter.getDayOfMonth());
						long fromJavaMillis = gc.getTimeInMillis();
						gc = new GregorianCalendar(dtToFilter.getYear(), dtToFilter.getMonth(), dtToFilter.getDayOfMonth());
						long toJavaMillis = gc.getTimeInMillis();

						toJavaMillis += 86399999; // adds a day minus a
						// millisecond

						long fromTicks = Utilities.convertUnixTimestampToTicks(fromJavaMillis);
						long toTicks = Utilities.convertUnixTimestampToTicks(toJavaMillis);

						String docStatusID = cmbStatusFilter.getSelectedItem() == null || ((DocumentStatus) cmbStatusFilter.getSelectedItem()).getRemoteGuid() == null ? "" : ((DocumentStatus) cmbStatusFilter.getSelectedItem()).getRemoteGuid();
						String docTypeID = cmbTypeFilter.getSelectedItem() == null || ((DocumentType) cmbTypeFilter.getSelectedItem()).getRemoteGuid() == null ? "" : ((DocumentType) cmbTypeFilter.getSelectedItem()).getRemoteGuid();
						String hasBeenCheckedFilter = cmbHasBeenCheckedFilter.getSelectedItem() == null ? "" : cmbHasBeenCheckedFilter.getSelectedItem().toString();
						String hasBeenExecutedFilter = cmbHasBeenExecutedFilter.getSelectedItem() == null ? "" : cmbHasBeenExecutedFilter.getSelectedItem().toString();

						if (hasBeenCheckedFilter.equals(getString(R.string.yes)))
						{
							hasBeenCheckedFilter = "true";
						}
						else if (hasBeenCheckedFilter.equals(getString(R.string.no)))
						{
							hasBeenCheckedFilter = "false";
						}
						else
						{
							hasBeenCheckedFilter = "";
						}
						if (hasBeenExecutedFilter.equals(getString(R.string.yes)))
						{
							hasBeenExecutedFilter = "true";
						}
						else if (hasBeenExecutedFilter.equals(getString(R.string.no)))
						{
							hasBeenExecutedFilter = "false";
						}
						else
						{
							hasBeenExecutedFilter = "";
						}
						try
						{
							xml = wsHelper.getDocumentHeadersXMLWithFilters(selectedCustomer, loggedInUser, fromTicks, toTicks, docStatusID, docTypeID, hasBeenCheckedFilter, hasBeenExecutedFilter);
						}
						catch (Exception e)
						{
							throw new Exception(getString(R.string.NetworkError));
						}
					}
					else
					{
						try
						{
							xml = wsHelper.getDocumentHeadersXML(selectedCustomer, loggedInUser);
							if (xml == null)
							{
								throw new Exception(getString(R.string.NetworkError));
							}
						}
						catch (Exception e)
						{
							throw new Exception(getString(R.string.NetworkError));
						}

					}
					if (xml.equals("anyType{}") || xml.equals(""))
					{
						return "";
					}
					StreamSource xmlSource = new StreamSource(new ByteArrayInputStream(xml.getBytes()));
					StreamSource xslSource = new StreamSource(new FileInputStream(txtPathToXSL.getText().toString()));
					String html = Utilities.transformXML(xmlSource, xslSource);
					return html;
				}
				catch (Exception e)
				{
					e.printStackTrace();
					return e;
				}

			}
		});

		task.setPostExecuteRunner(new PostExecuteRunnable()
		{

			public void Run(Object result)
			{
				if (result != null && result.getClass().equals(String.class))
				{
					if (result.equals(""))
					{
						Utilities.ShowUninterruptingMessage(mContext, getString(R.string.noDocumentsFound), 5);
					}
					else
					{
						WebView wv = new WebView(mContext);

						// "file:///android_asset/"
						wv.loadDataWithBaseURL(null, (String) result, "text/html", "UTF-8", null);
						// TODO add javascript api
						Utilities.showWebViewDialog(mContext, wv, mContext.getResources().getString(android.R.string.ok));
					}

				}
				else
				{
					Utilities.ShowUninterruptingMessage(mContext, ((Exception) result).getMessage(), 10);
				}
			}
		});

		task.execute((Object[]) null);

	}

}
