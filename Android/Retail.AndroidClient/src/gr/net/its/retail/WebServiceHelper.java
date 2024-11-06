package gr.net.its.retail;

import gr.net.its.common.Utilities;
import gr.net.its.retail.data.Address;
import gr.net.its.retail.data.ApplicationSettings;
import gr.net.its.retail.data.Barcode;
import gr.net.its.retail.data.BinaryOperator;
import gr.net.its.retail.data.Customer;
import gr.net.its.retail.data.DatabaseHelper;
import gr.net.its.retail.data.DocumentDetail;
import gr.net.its.retail.data.DocumentHeader;
import gr.net.its.retail.data.DocumentStatus;
import gr.net.its.retail.data.DocumentType;
import gr.net.its.retail.data.FilterType;
import gr.net.its.retail.data.IRetailPersistent;
import gr.net.its.retail.data.InvalidItem;
import gr.net.its.retail.data.InvalidItemReason;
import gr.net.its.retail.data.Item;
import gr.net.its.retail.data.ItemAnalyticTree;
import gr.net.its.retail.data.ItemCategory;
import gr.net.its.retail.data.LinkedItem;
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
import gr.net.its.retail.data.VATFactor;
import gr.net.its.retail.data.VATLevel;

import java.io.BufferedInputStream;
import java.io.BufferedReader;
import java.io.ByteArrayInputStream;
import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.net.MalformedURLException;
import java.net.URL;
import java.net.URLConnection;
import java.sql.SQLException;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.HashMap;
import java.util.HashSet;
import java.util.List;
import java.util.Map.Entry;
import java.util.Set;
import java.util.UUID;
import java.util.concurrent.ExecutionException;
import java.util.zip.ZipEntry;
import java.util.zip.ZipInputStream;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;

import org.ksoap2.SoapEnvelope;
import org.ksoap2.serialization.SoapObject;
import org.ksoap2.serialization.SoapPrimitive;
import org.ksoap2.serialization.SoapSerializationEnvelope;
import org.ksoap2.transport.HttpTransportSE;
import org.w3c.dom.Document;
import org.w3c.dom.Node;
import org.w3c.dom.NodeList;

import com.j256.ormlite.dao.Dao;
import com.j256.ormlite.misc.TransactionManager;
import com.j256.ormlite.stmt.PreparedQuery;
import com.j256.ormlite.stmt.QueryBuilder;
import com.j256.ormlite.support.DatabaseConnection;

import android.annotation.SuppressLint;
import android.app.AlertDialog;
import android.app.Application;
import android.app.ProgressDialog;
import android.content.Context;
import android.content.DialogInterface;
import android.content.SharedPreferences;
import android.content.SyncResult;
import android.database.Cursor;
import android.database.sqlite.SQLiteProgram;
import android.database.sqlite.SQLiteStatement;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.os.AsyncTask;
import android.os.AsyncTask.Status;
import android.os.Environment;
import android.os.Looper;
import android.text.TextUtils;
import android.util.Log;
import android.util.Pair;
import android.view.WindowManager;
import android.widget.Toast;

import com.j256.ormlite.support.DatabaseConnection;

@SuppressLint("DefaultLocale")
public class WebServiceHelper
{
    public static int currentRecords = 0, totalRecords = 0, TIMEOUT = 20000;

    private static final String NAMESPACE = "https://weborders.masoutis.gr/";
    private static final int COMMIT_SIZE = 500;
    // private static final String URL =
    // "http://10.3.1.172/RetailV1.Debug/RetailService.asmx";

    private static final int BYTES_PER_RECORD = 100;

    private boolean isOnline = false, saveData = false;
    private Context mContext;

    class WebMethodParameters
    {
	public String MethodName;
	public HashMap<String, Object> parameters;
    }

    class WebMethodCaller extends AsyncTask<WebMethodParameters, Integer, Object>
    {

	private Object callWebMethod2(String MethodName, HashMap<String, Object> parameters)
	{
	    SoapSerializationEnvelope envelope = new SoapSerializationEnvelope(SoapEnvelope.VER12);
	    try
	    {
		String METHOD_NAME = MethodName;
		String SOAP_ACTION = NAMESPACE + METHOD_NAME;

		SoapObject request = new SoapObject(NAMESPACE, METHOD_NAME);

		if (parameters != null)
		{
		    for (Entry<String, Object> para : parameters.entrySet())
		    {
			request.addProperty(para.getKey(), para.getValue());
		    }
		}

		envelope.dotNet = true;
		envelope.setOutputSoapObject(request);

		HttpTransportSE androidHttpTransport = new HttpTransportSE(((MainActivity) mContext).appsettings.getServiceUrl(), TIMEOUT);

		androidHttpTransport.call(SOAP_ACTION, envelope);
		Object result = envelope.getResponse();
		return result;

	    }
	    catch (Exception e)
	    {
		e.printStackTrace();
		return null;
	    }
	}

	@Override
	protected Object doInBackground(WebMethodParameters... params)
	{

	    WebMethodParameters p = params[0];
	    Log.v("WebMethodCaller", "Start " + p.MethodName);
	    Object result = callWebMethod2(p.MethodName, p.parameters);
	    Log.v("WebMethodCaller", "End " + p.MethodName);
	    return result;
	}

    }

    //
    // @SuppressLint("DefaultLocale")
    // class SynchronizerFromText extends AsyncTask<User, Integer, Object>
    // {
    //
    // private boolean SynchronizeApplicationSettings(User user)
    // {
    // try
    // {
    // // VATCategory
    // HashMap<String, Object> params = new HashMap<String, Object>();
    //
    // params.put("userID", user.getRemoteGuid());
    // Object result = callWebMethod("getAppSettings", params);
    // SoapObject sresult = (SoapObject) result;
    // // int limit = sresult.getPropertyCount();
    // // for (int i = 0; i < limit; i++)
    // {
    // SoapObject obj = sresult;
    // ApplicationSettings ct;
    // ct = dbHelper.getApplicationSettings().queryForAll().get(0);
    // ct.setBarcodePadChar(obj.getPropertyAsString("BarcodePadChar").charAt(0));
    // ct.setCodePadChar(obj.getPropertyAsString("CodePadChar").charAt(0));
    //
    // ct.setBarcodePadding(Boolean.parseBoolean(obj.getPropertyAsString("BarcodePad")));
    // ct.setCodePadding(Boolean.parseBoolean(obj.getPropertyAsString("CodePad")));
    //
    // ct.setBarcodePadLength(Integer.parseInt(obj.getPropertyAsString("BarcodePadLength")));
    // ct.setCodePadLength(Integer.parseInt(obj.getPropertyAsString("CodePadLength")));
    //
    // ct.setComputeDigits(Integer.parseInt(obj.getPropertyAsString("ComputeDigits")));
    // ct.setComputeValueDigits(Integer.parseInt(obj.getPropertyAsString("ComputeValueDigits")));
    //
    // ct.setDisplayDigits(Integer.parseInt(obj.getPropertyAsString("DisplayDigits")));
    // ct.setDisplayValueDigits(Integer.parseInt(obj.getPropertyAsString("DisplayValueDigits")));
    //
    // dbHelper.getApplicationSettings().update(ct);
    // ((MainActivity) mContext).appsettings = ct;
    //
    // }
    // return true;
    // }
    // catch (Exception ex)
    // {
    // ex.printStackTrace();
    // }
    // return false;
    // }
    //
    // @Override
    // protected void onPostExecute(Object result)
    // {
    // // TODO Auto-generated method stub
    // super.onPostExecute(result);
    // ((MainActivity)
    // mContext).getWindow().clearFlags(WindowManager.LayoutParams.FLAG_KEEP_SCREEN_ON);
    //
    // progDialog.dismiss();
    // if (result == null || (Boolean) result == true)
    // {
    // ((MainActivity) mContext).btnOrders.setEnabled(true);
    // ((MainActivity) mContext).foundCustomers.clear();
    // ((MainActivity) mContext).UpdateCustomers(false);
    // ((MainActivity) mContext).UpdateUI();
    // ((MainActivity) mContext).isDBInitialized = true;
    // Utilities.ShowUninterruptingMessage(mContext,
    // mContext.getString(R.string.updateCompletedSuccessfully), 5);
    // }
    // else
    // {
    // Utilities.ShowUninterruptingMessage(mContext,
    // mContext.getString(R.string.problem), 5);
    // }
    //
    // }
    //
    // @Override
    // protected void onProgressUpdate(Integer... values)
    // {
    // // TODO Auto-generated method stub
    // super.onProgressUpdate(values);
    // progDialog.setMessage(progDialogMessage);
    // if (!progDialog.isShowing())
    // {
    // progDialog.show();
    // }
    // progDialog.setProgress(values[0]);
    // }
    //
    // private ProgressDialog progDialog;
    // private String[] stores = null, documentstatus = null, customers = null,
    // items = null, itemATs = null, barcodes = null, itemcategory = null,
    // pricecatalog = null,
    // pricecatalogdetail = null, addresses = null;
    // private SQLiteStatement customerInsert = null, customerUpdate = null,
    // itemInsert = null, itemUpdate = null, barcodeInsert = null, barcodeUpdate
    // = null,
    // addressInsert = null, addressUpdate = null;
    // private SQLiteStatement itemAnalyticTreeInsert, itemAnalyticTreeUpdate,
    // itemCategoryInsert;
    // private SQLiteStatement itemCategoryUpdate, pc_in, pc_up, pcd_in,
    // mesurmentUnitsInsert, mesurmentUnitsUpdate;
    // private SQLiteStatement pcd_up, documentStatusInsert,
    // documentStatusUpdate;
    // private SQLiteStatement storeInsert, storeUpdate, linkedItemsInsert,
    // linkedItemsUpdate;
    // private String[] linkedItems = null, mesurmentUnits = null;
    // private String[] vatCategories;
    // private SQLiteStatement vcInsert;
    // private SQLiteStatement vcUpdate;
    // private String[] vatLevels;
    // private SQLiteStatement vlInsert;
    // private SQLiteStatement vlUpdate;
    // private String[] vatFactors;
    // private SQLiteStatement vfInsert;
    // private SQLiteStatement vfUpdate;
    // private String[] offers;
    // private SQLiteStatement offerInsert;
    // private SQLiteStatement offerUpdate;
    // private SQLiteStatement offerDetailInsert;
    // private String[] offerDetails;
    // private SQLiteStatement offerDetailrUpdate;
    //
    // @SuppressLint("DefaultLocale")
    // protected boolean parseLine(String line)
    // {
    // String[] elements = TextUtils.split(line, "\t&\t");
    // // line.split("\t&\t");
    // String query;
    // if (elements[0].equals("DocumentStatus"))
    // {
    // if (documentstatus == null)
    // {
    // documentstatus = elements;
    // Log.i("Parsing", "documentstatus start");
    // }
    // else
    // {
    //
    // DocumentStatus ct = dbHelper.getDocumentStatusByRemoteGuid(elements[1]);
    // if (ct == null || elements[1].equals("Oid"))
    // {
    // if (documentStatusInsert == null)
    // {
    // // Insert
    // Log.i("Parse Line", "Compiling documentStatusInsert");
    // query =
    // "insert into DocumentStatus(remoteGuid,Description,IsDefault,UpdatedOn) ";
    // query += "values(?,?,?,?)";
    // documentStatusInsert =
    // dbHelper.getWritableDatabase().compileStatement(query);
    // }
    // for (int i = 1; i < 5; i++)
    // documentStatusInsert.bindString(i, elements[i]);
    // documentStatusInsert.executeInsert();
    //
    // }
    // else
    // {
    //
    // if (documentStatusUpdate == null)
    // {
    // Log.i("Parse Line", "Compiling documentStatusUpdate");
    // query =
    // "update DocumentStatus set Description=?,IsDefault=?,UpdatedOn=? where remoteGuid = ?";
    // documentStatusUpdate =
    // dbHelper.getWritableDatabase().compileStatement(query);
    // }
    // for (int i = 2; i < 5; i++)
    // {
    // documentStatusUpdate.bindString(i - 1, elements[i]);
    // }
    // documentStatusUpdate.bindString(4, elements[1]);
    // documentStatusUpdate.executeUpdateDelete();
    // }
    // }
    // }
    // else if (elements[0].equals("Store"))
    // {
    // if (stores == null || elements[1].equals("Oid"))
    // {
    // stores = elements;
    // Log.i("Parsing", "Store start");
    // }
    // else
    // {
    //
    // Store ct = dbHelper.getStoreByRemoteGuid(elements[1]);
    // if (ct == null)
    // {
    // if (storeInsert == null)
    // {
    // // Insert
    // Log.i("Parse Line", "Compiling storeInsert");
    // query =
    // "insert into Store(remoteGuid,Code,Name,IsCentralStore,UpdatedOn) ";
    // query += "values(?,?,?,?,?)";
    // storeInsert = dbHelper.getWritableDatabase().compileStatement(query);
    // }
    // for (int i = 1; i < 6; i++)
    // storeInsert.bindString(i, elements[i]);
    // storeInsert.executeInsert();
    //
    // }
    // else
    // {
    //
    // if (storeUpdate == null)
    // {
    // Log.i("Parse Line", "Compiling storeUpdate");
    // query =
    // "update Store set Code=?,Name=?,IsCentralStore=?,UpdatedOn=? where remoteGuid = ?";
    // storeUpdate = dbHelper.getWritableDatabase().compileStatement(query);
    // }
    // for (int i = 2; i < 6; i++)
    // {
    // storeUpdate.bindString(i - 1, elements[i]);
    // }
    // storeUpdate.bindString(5, elements[1]);
    // storeUpdate.executeUpdateDelete();
    // }
    // }
    // }
    // else if (elements[0].equals("LinkedItem"))
    // {
    // if (linkedItems == null || elements[1].equals("Oid"))
    // {
    // linkedItems = elements;
    // Log.i("Parsing", "LinkedItem start");
    // }
    // else
    // {
    //
    // LinkedItem ct = dbHelper.getLinkedItemByRemoteGuid(elements[1]);
    // if (ct == null)
    // {
    // if (linkedItemsInsert == null)
    // {
    // // Insert
    // Log.i("Parse Line", "Compiling linkedItemsInsert");
    // query =
    // "insert into LinkedItem(remoteGuid,itemGuid,subItemGuid,UpdatedOn) ";
    // query += "values(?,?,?,?)";
    // linkedItemsInsert =
    // dbHelper.getWritableDatabase().compileStatement(query);
    // }
    // for (int i = 1; i < 5; i++)
    // linkedItemsInsert.bindString(i, elements[i]);
    // linkedItemsInsert.executeInsert();
    //
    // }
    // else
    // {
    //
    // if (linkedItemsUpdate == null)
    // {
    // Log.i("Parse Line", "Compiling linkedItemsUpdate");
    // query =
    // "update LinkedItem set itemGuid=?,subItemGuid=?,UpdatedOn=? where remoteGuid = ?";
    // linkedItemsUpdate =
    // dbHelper.getWritableDatabase().compileStatement(query);
    // }
    // for (int i = 2; i < 5; i++)
    // {
    // linkedItemsUpdate.bindString(i - 1, elements[i]);
    // }
    // linkedItemsUpdate.bindString(4, elements[1]);
    // linkedItemsUpdate.executeUpdateDelete();
    // }
    // }
    // }
    // else if (elements[0].equals("MesurmentUnits"))
    // {
    // if (mesurmentUnits == null || elements[1].equals("Oid"))
    // {
    // mesurmentUnits = elements;
    // Log.i("Parsing", "mesurmentUnits start");
    // }
    // else
    // {
    //
    // MeasurementUnit ct =
    // dbHelper.getMeasurementUnitByRemoteGuid(elements[1]);
    // if (ct == null)
    // {
    // if (mesurmentUnitsInsert == null)
    // {
    // // Insert
    // Log.i("Parse Line", "Compiling mesurmentUnitsInsert");
    // query =
    // "insert into MeasurementUnit(remoteGuid,Description,supportDecimals,UpdatedOn) ";
    // query += "values(?,?,?,?)";
    // mesurmentUnitsInsert =
    // dbHelper.getWritableDatabase().compileStatement(query);
    // }
    // for (int i = 1; i < 5; i++)
    // mesurmentUnitsInsert.bindString(i, elements[i]);
    // mesurmentUnitsInsert.executeInsert();
    //
    // }
    // else
    // {
    //
    // if (mesurmentUnitsUpdate == null)
    // {
    // Log.i("Parse Line", "Compiling mesurmentUnitsUpdate");
    // query =
    // "update MeasurementUnit set Description=?,supportDecimals=?,UpdatedOn=? where remoteGuid = ?";
    // mesurmentUnitsUpdate =
    // dbHelper.getWritableDatabase().compileStatement(query);
    // }
    // for (int i = 2; i < 5; i++)
    // {
    // mesurmentUnitsUpdate.bindString(i - 1, elements[i]);
    // }
    // mesurmentUnitsUpdate.bindString(4, elements[1]);
    // mesurmentUnitsUpdate.executeUpdateDelete();
    // }
    // }
    // }
    // else if (elements[0].equals("VatCategory"))
    // {
    // if (vatCategories == null || elements[1].equals("Oid"))
    // {
    // vatCategories = elements;
    // Log.i("Parsing", "vatCategories start");
    // }
    // else
    // {
    //
    // VATCategory ct = dbHelper.getVatCategoryByRemoteGuid(elements[1]);
    // if (ct == null)
    // {
    // if (vcInsert == null)
    // {
    // // Insert
    // Log.i("Parse Line", "Compiling vcInsert");
    // query = "insert into VATCategory(remoteGuid,name,UpdatedOn) ";
    // query += "values(?,?,?)";
    // vcInsert = dbHelper.getWritableDatabase().compileStatement(query);
    // }
    // for (int i = 1; i < 4; i++)
    // vcInsert.bindString(i, elements[i]);
    // vcInsert.executeInsert();
    //
    // }
    // else
    // {
    //
    // if (vcUpdate == null)
    // {
    // Log.i("Parse Line", "Compiling vcUpdate");
    // query = "update VATCategory set name=?,UpdatedOn=? where remoteGuid = ?";
    // vcUpdate = dbHelper.getWritableDatabase().compileStatement(query);
    // }
    // for (int i = 2; i < 4; i++)
    // {
    // vcUpdate.bindString(i - 1, elements[i]);
    // }
    // vcUpdate.bindString(3, elements[1]);
    // vcUpdate.executeUpdateDelete();
    // }
    // }
    // }
    // else if (elements[0].equals("VatLevel"))
    // {
    // if (vatLevels == null || elements[1].equals("Oid"))
    // {
    // vatLevels = elements;
    // Log.i("Parsing", "vatLevels start");
    // }
    // else
    // {
    //
    // VATLevel ct = dbHelper.getVatLevelByRemoteGuid(elements[1]);
    // if (ct == null)
    // {
    // if (vlInsert == null)
    // {
    // // Insert
    // Log.i("Parse Line", "Compiling vlInsert");
    // query = "insert into VatLevel(remoteGuid,name,'Default',UpdatedOn) ";
    // query += "values(?,?,?,?)";
    // vlInsert = dbHelper.getWritableDatabase().compileStatement(query);
    // }
    // for (int i = 1; i < 5; i++)
    // vlInsert.bindString(i, elements[i]);
    // vlInsert.executeInsert();
    //
    // }
    // else
    // {
    //
    // if (vlUpdate == null)
    // {
    // Log.i("Parse Line", "Compiling vlUpdate");
    // query =
    // "update VatLevel set name=?,'Default'=?,UpdatedOn=? where remoteGuid = ?";
    // vlUpdate = dbHelper.getWritableDatabase().compileStatement(query);
    // }
    // for (int i = 2; i < 5; i++)
    // {
    // vlUpdate.bindString(i - 1, elements[i]);
    // }
    // vlUpdate.bindString(4, elements[1]);
    // vlUpdate.executeUpdateDelete();
    // }
    // }
    // }
    // else if (elements[0].equals("VatFactor"))
    // {
    // if (vatFactors == null || elements[1].equals("Oid"))
    // {
    // vatFactors = elements;
    // Log.i("Parsing", "vatFactors start");
    // }
    // else
    // {
    //
    // VATFactor ct = dbHelper.getVatFactorByRemoteGuid(elements[1]);
    // if (ct == null)
    // {
    // if (vfInsert == null)
    // {
    // // Insert
    // Log.i("Parse Line", "Compiling vfInsert");
    // query =
    // "insert into VatFactor(remoteGuid,vatCategoryRemoteGuid,vatLevelRemoteGuid,vatFactor,UpdatedOn) ";
    // query += "values(?,?,?,?,?)";
    // vfInsert = dbHelper.getWritableDatabase().compileStatement(query);
    // }
    // for (int i = 1; i < 6; i++)
    // vfInsert.bindString(i, elements[i].replace(",", "."));
    // vfInsert.executeInsert();
    //
    // }
    // else
    // {
    //
    // if (vfUpdate == null)
    // {
    // Log.i("Parse Line", "Compiling vfUpdate");
    // query =
    // "update VatFactor set vatCategoryRemoteGuid=?,vatLevelRemoteGuid=?,vatFactor=?,UpdatedOn=? where remoteGuid = ?";
    // vfUpdate = dbHelper.getWritableDatabase().compileStatement(query);
    // }
    // for (int i = 2; i < 6; i++)
    // {
    // vfUpdate.bindString(i - 1, elements[i].replace(",", "."));
    // }
    // vfUpdate.bindString(5, elements[1]);
    // vfUpdate.executeUpdateDelete();
    // }
    // }
    // }
    // else if (elements[0].equals("Offer"))
    // {
    // if (offers == null || elements[1].equals("Oid"))
    // {
    // offers = elements;
    // Log.i("Parsing", "offers start");
    // }
    // else
    // {
    //
    // Offer ct = dbHelper.getOfferByRemoteGuid(elements[1]);
    // if (ct == null)
    // {
    // if (offerInsert == null)
    // {
    // // Insert
    // // "Oid" + DELIMITER + "PriceCatalogOid" +
    // // "Description" + "Description2" + "isActive" +
    // // "StartDate" + "EndDate" + "UpdatedOn");
    // Log.i("Parse Line", "Compiling offerInsert");
    // query =
    // "insert into Offer(remoteGuid,PriceCatalogRemoteGuid,Description,Description2,Active,StartDate,EndDate,UpdatedOn) ";
    // query += "values(?,?,?,?,?,?,?,?)";
    // offerInsert = dbHelper.getWritableDatabase().compileStatement(query);
    // }
    // for (int i = 1; i < 9; i++)
    // offerInsert.bindString(i, elements[i]);
    // offerInsert.executeInsert();
    //
    // }
    // else
    // {
    //
    // if (offerUpdate == null)
    // {
    // Log.i("Parse Line", "Compiling offerUpdate");
    // query =
    // "update Offer set PriceCatalogRemoteGuid=?,Description=?,Description2=?,Active=?,StartDate=?,EndDate=?,UpdatedOn=? where remoteGuid = ?";
    // offerUpdate = dbHelper.getWritableDatabase().compileStatement(query);
    // }
    // for (int i = 2; i < 9; i++)
    // {
    // offerUpdate.bindString(i - 1, elements[i]);
    // }
    // offerUpdate.bindString(8, elements[1]);
    // offerUpdate.executeUpdateDelete();
    // }
    // }
    // }
    // else if (elements[0].equals("OfferDetail"))
    // {
    // if (offerDetails == null || elements[1].equals("Oid"))
    // {
    // offerDetails = elements;
    // Log.i("Parsing", "offers start");
    // }
    // else
    // {
    //
    // OfferDetail ct = dbHelper.getOfferDetailByRemoteGuid(elements[1]);
    // if (ct == null)
    // {
    // if (offerDetailInsert == null)
    // {
    // // Insert
    // // txt.WriteLine("OfferDetail" + + "Oid" + +
    // // "OfferOid" + + "ItemOid" + + "isActive" + +
    // // "UpdatedOn");
    // Log.i("Parse Line", "Compiling offerDetailInsert");
    // query =
    // "insert into OfferDetail(remoteGuid,OfferRemoteGuid,ItemRemoteGuid,Active,UpdatedOn) ";
    // query += "values(?,?,?,?,?)";
    // offerDetailInsert =
    // dbHelper.getWritableDatabase().compileStatement(query);
    // }
    // for (int i = 1; i < 6; i++)
    // offerDetailInsert.bindString(i, elements[i]);
    // offerDetailInsert.executeInsert();
    //
    // }
    // else
    // {
    // if (offerDetailrUpdate == null)
    // {
    // Log.i("Parse Line", "Compiling offerUpdate");
    // query =
    // "update OfferDetail set OfferRemoteGuid=?,ItemRemoteGuid=?,Active=?,UpdatedOn=? where remoteGuid = ?";
    // offerDetailrUpdate =
    // dbHelper.getWritableDatabase().compileStatement(query);
    // }
    // for (int i = 2; i < 6; i++)
    // {
    // offerDetailrUpdate.bindString(i - 1, elements[i]);
    // }
    // offerDetailrUpdate.bindString(5, elements[1]);
    // offerDetailrUpdate.executeUpdateDelete();
    // }
    // }
    // }
    // else if (elements[0].equals("Address"))
    // {
    // if (addresses == null || elements[1].equals("Oid"))
    // {
    // addresses = elements;
    // Log.i("Parsing", "Addresses start");
    // }
    // else
    // {
    //
    // // Address ct =
    // // dbHelper.getAddressByRemoteGuid(elements[1]);
    // Address ct = null;
    // try
    // {
    // ct = dbHelper.findObject(Address.class, OperatorType.AND, new
    // BinaryOperator("remoteGuid", elements[1]), new
    // BinaryOperator("customerRemoteGuid", elements[2]));
    // }
    // catch (SQLException e)
    // {
    // Log.e("Query Failed", "Address query failed");
    // e.printStackTrace();
    // }
    //
    // if (ct == null)
    // {
    // if (addressInsert == null)
    // {
    // // Insert
    // Log.i("Parse Line", "Compiling address insert");
    // query = "insert into address(remoteGuid, customerRemoteGuid, Address) ";
    // query += "values(?,?,?)";
    // addressInsert = dbHelper.getWritableDatabase().compileStatement(query);
    // }
    // for (int i = 1; i < 4; i++)
    // addressInsert.bindString(i, elements[i]);
    // addressInsert.executeInsert();
    //
    // }
    // else
    // {
    //
    // if (addressUpdate == null)
    // {
    // Log.i("Parse Line", "Compiling address update");
    // query =
    // "update address set customerRemoteGuid=?,Address=? where remoteGuid = ?";
    // addressUpdate = dbHelper.getWritableDatabase().compileStatement(query);
    // }
    // for (int i = 2; i < 4; i++)
    // {
    // addressUpdate.bindString(i - 1, elements[i]);
    // }
    //
    // addressUpdate.bindString(3, elements[1]);
    // addressUpdate.executeUpdateDelete();
    // }
    // }
    // }
    // else if (elements[0].equals("Customer"))
    // {
    // if (customers == null || elements[5].equals("Oid"))
    // {
    // customers = elements;
    // Log.i("Parsing", "Customers start");
    // }
    // else
    // {
    //
    // Customer ct = dbHelper.getCustomerByRemoteGuid(elements[5]);
    // if (ct == null)
    // {
    // if (customerInsert == null)
    // {
    // // Insert
    // Log.i("Parse Line", "Compiling customer insert");
    // query =
    // "insert into customer(Code, CompanyName,DefaultAddress,DefaultPhone,remoteGuid,pcRemoteGuid,storeRemoteGuid,UpdatedOn,TaxCode,vatLevelRemoteGuid,lowerCompanyName) ";
    // query += "values(?,?,?,?,?,?,?,?,?,?,?)";
    // customerInsert = dbHelper.getWritableDatabase().compileStatement(query);
    // }
    // for (int i = 1; i < 11; i++)
    // customerInsert.bindString(i, elements[i]);
    // customerInsert.bindString(11, elements[2].toLowerCase());
    // customerInsert.executeInsert();
    //
    // }
    // else
    // {
    //
    // if (customerUpdate == null)
    // {
    // Log.i("Parse Line", "Compiling customer update");
    // query =
    // "update customer set Code=?, CompanyName=?,DefaultAddress=?,DefaultPhone=?,pcRemoteGuid=?,storeRemoteGuid=?,UpdatedOn=?,TaxCode=?,vatLevelRemoteGuid=?,lowerCompanyName=? where remoteGuid = ?";
    // customerUpdate = dbHelper.getWritableDatabase().compileStatement(query);
    // }
    // for (int i = 1; i < 5; i++)
    // {
    // customerUpdate.bindString(i, elements[i]);
    // }
    // for (int i = 6; i < 11; i++)
    // {
    // customerUpdate.bindString(i - 1, elements[i]);
    // }
    // customerUpdate.bindString(10, elements[2].toLowerCase());
    // customerUpdate.bindString(11, elements[5]);
    // customerUpdate.executeUpdateDelete();
    // }
    // }
    // }
    // else if (elements[0].equals("Item"))
    // {
    // if (items == null || elements[8].equals("Oid"))
    // {
    // items = elements;
    // Log.i("Parsing", "items start");
    // }
    // else
    // {
    // Item ct = dbHelper.getItemByRemoteGuid(elements[8]);
    // if (ct == null)
    // {
    // if (itemInsert == null)
    // {
    // Log.i("Parse Line", "Compiling item insert");
    // query =
    // "insert into item(Code, defaultBarcodeRemoteGuid,imageOid,InsertedOn,isactive,maxOrderQty,Name,remoteOid,packingQty,UpdatedOn,vatCategoryRemoteGuid,loweCaseName)";
    // query += "values(?,?,?,?,?,?,?,?,?,?,?,?)";
    // itemInsert = dbHelper.getWritableDatabase().compileStatement(query);
    // }
    // for (int i = 1; i < 12; i++)
    // itemInsert.bindString(i, elements[i]);
    // itemInsert.bindString(12, elements[7].toLowerCase());
    // itemInsert.executeInsert();
    // }
    // else
    // {
    // if (itemUpdate == null)
    // {
    // Log.i("Parse Line", "Compiling item update");
    // query =
    // "update item set Code=?, defaultBarcodeRemoteGuid=?,imageOid=?,InsertedOn=?,isactive=?,maxOrderQty=?,Name=?,remoteOid=?,packingQty=?,UpdatedOn=?,vatCategoryRemoteGuid=?,loweCaseName=?";
    // query += " where remoteOid=?";
    // itemUpdate = dbHelper.getWritableDatabase().compileStatement(query);
    // }
    // for (int i = 1; i < 8; i++)
    // itemUpdate.bindString(i, elements[i]);
    // for (int i = 9; i < 12; i++)
    // itemUpdate.bindString(i - 1, elements[i]);
    // itemUpdate.bindString(11, elements[7].toLowerCase());
    // itemUpdate.bindString(12, elements[8]);
    // itemUpdate.executeUpdateDelete();
    // }
    // }
    //
    // }
    // else if (elements[0].equals("ItemAnalyticTree"))
    // {
    // if (itemATs == null || elements[1].equals("Oid"))
    // {
    // itemATs = elements;
    // Log.i("Parsing", "Itemanalytictree start");
    // }
    // else
    // {
    // ItemAnalyticTree ct =
    // dbHelper.getItemAnalyticTreeByRemoteGuid(elements[1]);
    // if (ct == null)
    // {
    // if (itemAnalyticTreeInsert == null)
    // {
    // // Insert
    // Log.i("Parse Line", "Compiling itemAT insert");
    // query =
    // "insert into itemanalytictree(remoteGuid, itemRemoteGuid,itemCategoryRemoteGuid,UpdatedOn) ";
    // query += " values(?,?,?,?)";
    // itemAnalyticTreeInsert =
    // dbHelper.getWritableDatabase().compileStatement(query);
    // }
    // for (int i = 1; i < 5; i++)
    // itemAnalyticTreeInsert.bindString(i, elements[i]);
    // itemAnalyticTreeInsert.executeInsert();
    // }
    // else
    // {
    //
    // // update
    // if (itemAnalyticTreeUpdate == null)
    // {
    // Log.i("Parse Line", "Compiling itemAT update");
    // query =
    // "update itemanalytictree set itemRemoteGuid=?,itemCategoryRemoteGuid=?,UpdatedOn=?";
    // query += " where remoteGuid = ?";
    // itemAnalyticTreeUpdate =
    // dbHelper.getWritableDatabase().compileStatement(query);
    // }
    //
    // for (int i = 2; i < 5; i++)
    // itemAnalyticTreeUpdate.bindString(i - 1, elements[i]);
    // itemAnalyticTreeUpdate.bindString(4, elements[1]);
    // itemAnalyticTreeUpdate.executeUpdateDelete();
    // }
    // }
    //
    // }
    // else if (elements[0].equals("Barcode"))
    // {
    // if (barcodes == null || elements[1].equals("Oid"))
    // {
    // barcodes = elements;
    // Log.i("Parsing", "Barcode start");
    // }
    // else
    // {
    // Barcode ct = dbHelper.getBarcodeByRemoteGuid(elements[1]);
    // if (ct == null)
    // {
    // // Insert
    // if (barcodeInsert == null)
    // {
    // Log.i("Parse Line", "Compiling barcodeInsert");
    // query =
    // "insert into barcode(remoteGuid, itemRemoteGuid,measurementUnitRemoteGuid,Code,UpdatedOn) ";
    // query += " values(?,?,?,?,?)";
    // barcodeInsert = dbHelper.getWritableDatabase().compileStatement(query);
    // }
    // for (int i = 1; i < 6; i++)
    // barcodeInsert.bindString(i, elements[i]);
    // barcodeInsert.executeInsert();
    // }
    // else
    // {
    // // update
    // if (barcodeUpdate == null)
    // {
    // Log.i("Parse Line", "Compiling barcodeUpdate");
    // query =
    // "update barcode set itemRemoteGuid=?,measurementUnitRemoteGuid=?,Code=?,UpdatedOn=?";
    // query += " where remoteGuid = ?";
    // barcodeUpdate = dbHelper.getWritableDatabase().compileStatement(query);
    // }
    // for (int i = 2; i < 6; i++)
    // {
    // barcodeUpdate.bindString(i - 1, elements[i]);
    // }
    // barcodeUpdate.bindString(5, elements[1]);
    // }
    // }
    //
    // }
    // else if (elements[0].equals("ItemCategory"))
    // {
    // if (itemcategory == null || elements[1].equals("Oid"))
    // {
    // itemcategory = elements;
    // Log.i("Parsing", "itemcategory start");
    // }
    // else
    // {
    // ItemCategory ct = dbHelper.getItemCategoryByRemoteGuid(elements[1]);
    // if (ct == null)
    // {
    // if (itemCategoryInsert == null)
    // {
    // // Insert
    // Log.i("Parse Line", "Compiling itemCategoryInsert");
    // query =
    // "insert into itemcategory(remoteGuid, remoteParentGuid,Name,Code,UpdatedOn) ";
    // query += " values(?,?,?,?,?)";
    // itemCategoryInsert =
    // dbHelper.getWritableDatabase().compileStatement(query);
    // }
    // for (int i = 1; i < 6; i++)
    // itemCategoryInsert.bindString(i, elements[i]);
    // itemCategoryInsert.executeInsert();
    // }
    // else
    // {
    // if (itemCategoryUpdate == null)
    // {
    // // update
    // Log.i("Parse Line", "Compiling itemCategoryUpdate");
    // query =
    // "update itemcategory set remoteParentGuid=?,Name=?,Code=?,UpdatedOn=?";
    // query += " where remoteGuid =?";
    // itemCategoryUpdate =
    // dbHelper.getWritableDatabase().compileStatement(query);
    //
    // }
    // for (int i = 2; i < 6; i++)
    // {
    // itemCategoryUpdate.bindString(i - 1, elements[i]);
    // }
    // itemCategoryUpdate.bindString(5, elements[1]);
    // itemCategoryUpdate.executeUpdateDelete();
    // }
    // }
    // }
    // else if (elements[0].equals("PriceCatalog"))
    // {
    // if (pricecatalog == null || elements[1].equals("Oid"))
    // {
    // pricecatalog = elements;
    // Log.i("Parsing", "pricecatalog start");
    // }
    // else
    // {
    // PriceCatalog ct = dbHelper.getPriceCatalogByRemoteGuid(elements[1]);
    // if (ct == null)
    // {
    // if (pc_in == null)
    // {
    // // Insert
    // Log.i("Parse Line", "Compiling pc_in");
    // query =
    // "insert into pricecatalog(remoteGuid, remoteParentGuid,Name,Code,UpdatedOn) ";
    // query += " values(?,?,?,?,?)";
    // pc_in = dbHelper.getWritableDatabase().compileStatement(query);
    // }
    // for (int i = 1; i < 6; i++)
    // pc_in.bindString(i, elements[i]);
    // pc_in.executeInsert();
    // }
    // else
    // {
    // if (pc_up == null)
    // {
    // // update
    // Log.i("Parse Line", "Compiling pc_up");
    // query =
    // "update pricecatalog set remoteParentGuid=?,Name=?,Code=?,UpdatedOn=?";
    // query += " where remoteGuid =?";
    // pc_up = dbHelper.getWritableDatabase().compileStatement(query);
    //
    // }
    // for (int i = 2; i < 6; i++)
    // {
    // pc_up.bindString(i - 1, elements[i]);
    // }
    // pc_up.bindString(5, elements[1]);
    // pc_up.executeUpdateDelete();
    // }
    // }
    // }
    // else if (elements[0].equals("PriceCatalogDetail"))
    // {
    // if (pricecatalogdetail == null || elements[1].equals("Oid"))
    // {
    // pricecatalogdetail = elements;
    // Log.i("Parsing", "pricecatalogdetail start");
    // }
    // else
    // {
    // PriceCatalogDetail ct =
    // dbHelper.getPriceCatalogDetailByRemoteGuid(elements[1]);
    // if (ct == null)
    // {
    // if (pcd_in == null)
    // {
    // // Insert
    // Log.i("Parse Line", "Compiling pcd_in");
    // query =
    // "insert into pricecatalogdetail(remoteGuid, barcodeRemoteGuid,pcRemoteGuid,itemRemoteGuid,price,discount,VATIncluded,UpdatedOn) ";
    // query += " values(?,?,?,?,?,?,?,?)";
    // pcd_in = dbHelper.getWritableDatabase().compileStatement(query);
    // }
    // for (int i = 1; i < 9; i++)
    // pcd_in.bindString(i, elements[i].replace(",", "."));
    // pcd_in.executeInsert();
    // }
    // else
    // {
    // if (pcd_up == null)
    // {
    // // update
    // query =
    // "update pricecatalogdetail set barcodeRemoteGuid=?,pcRemoteGuid=?,itemRemoteGuid=?,price=?,discount=?,VATIncluded=?,UpdatedOn=?";
    // query += " where remoteGuid = ?";
    // pcd_up = dbHelper.getWritableDatabase().compileStatement(query);
    // }
    // for (int i = 2; i < 9; i++)
    // pcd_up.bindString(i - 1, elements[i].replace(",", "."));
    //
    // pcd_up.bindString(8, elements[1]);
    // pcd_up.executeUpdateDelete();
    // }
    // }
    // }
    // // dbHelper.getWritableDatabase().compileStatement(query);
    // // dbHelper.getWritableDatabase().execSQL(query);
    //
    // elements = null;
    // line = null;
    // // PriceCatalogDetail, Oid, BarcodeOid, PriceCatalogOid, ItemOid,
    // // value, discount, vatIncluded, UpdatedOn
    // return true;
    // }
    //
    // protected boolean parseFile(String filename)
    // {
    // try
    // {
    // DatabaseConnection conn =
    // dbHelper.getConnectionSource().getReadWriteConnection();
    // try
    // {
    // int lines = 0;
    // File fl = new File(filename);
    // int flsize = (int) fl.length(), readbytes = 0;
    // progDialog.setMax(flsize);
    // fl = null;
    // FileInputStream fs = new FileInputStream(filename);
    // InputStreamReader st = new InputStreamReader(fs);
    // BufferedReader br = new BufferedReader(st);
    // conn.setAutoCommit(false);
    // String line;
    // while ((line = br.readLine()) != null)
    // {
    // lines++;
    // readbytes += line.length();
    // if (lines % 1000 == 0)
    // {
    // publishProgress(readbytes);
    // if (lines % 20000 == 0)
    // {
    // Log.i("Pasrser", "Commiting 20000 records - start");
    // conn.setAutoCommit(true);
    // conn.setAutoCommit(false);
    // Log.i("Pasrser", "Commiting 20000 records - end");
    // }
    // }
    // if (!parseLine(line.replace("\0", "")))
    // {
    // conn.setAutoCommit(true);
    // return false;
    // }
    // line = null;
    // }
    // }
    // catch (Exception ex)
    // {
    // ex.printStackTrace();
    // conn.setAutoCommit(true);
    // return false;
    // }
    // conn.setAutoCommit(true);
    // return true;
    // }
    // catch (Exception ex)
    // {
    // ex.printStackTrace();
    // return false;
    // }
    // }
    //
    // @Override
    // protected Object doInBackground(User... arg0)
    // {
    // Log.i("Syncrhonize Text", "Start");
    // progDialogMessage = "Initializing...";
    // publishProgress(0);
    // SynchronizeApplicationSettings(arg0[0]);
    // SynchronizeUserStoreAccessOfUser(arg0[0]);
    //
    // // STEP 1 Download File
    // String url = ((MainActivity) mContext).appsettings.getServiceUrl();
    // HashMap<String, Object> parameters = new HashMap<String, Object>();
    //
    // long customerUpdatedOn = dbHelper.getMaxCustomerUpdatedOn(),
    // itemupdatedon = dbHelper.getMaxItemUpdatedOn(), barcodeupdatedon =
    // dbHelper.getMaxBarcodeUpdatedOn(), pcdupdatedon =
    // dbHelper.getMaxPriceCatalogDetail(), iatupdatedon =
    // dbHelper.getMaxItemAnalyticTreesUpdatedOn();
    // parameters.put("Customerupdatedon", "" + customerUpdatedOn);
    // parameters.put("Itemupdatedon", "" + dbHelper.getMaxItemUpdatedOn());
    // parameters.put("IATupdatedon", "" + iatupdatedon);
    // parameters.put("Barcodeupdatedon", "" + barcodeupdatedon);
    //
    // parameters.put("ICupdatedon", "" +
    // dbHelper.getMaxItemCategoriesUpdatedOn());
    // parameters.put("PCupdatedon", "" + dbHelper.getMaxPriceCatalog());
    // parameters.put("PCDupdatedon", "" + pcdupdatedon);
    // parameters.put("VCupdatedon", "" + dbHelper.getMaxVatCategory());
    //
    // parameters.put("VFupdatedon", "" + dbHelper.getMaxVatFactor());
    // parameters.put("VLupdatedon", "" + dbHelper.getMaxVatLevel());
    // parameters.put("offerUpdatedOn", "" + dbHelper.getMaxOffersUpdatedOn());
    // parameters.put("offerDetailUpdatedOn", "" +
    // dbHelper.getMaxOfferDetailsUpdatedOn());
    //
    // parameters.put("documentStatusUpdatedOn", "" +
    // dbHelper.getMaxDocumentStatusUpdatedOn());
    // parameters.put("storesUpdatedOn", "" + dbHelper.getMaxStoresUpdatedOn());
    // parameters.put("linkedItemsUpdatedOn", "" + dbHelper.getMaxLinkedItem());
    // parameters.put("mmUpdatedOn", "" +
    // dbHelper.getMaxMeasurementUnitUpdatedOn());
    //
    // parameters.put("userID", arg0[0].getRemoteGuid());
    // Log.d("ws params", parameters.toString());
    // int oldTimeout = TIMEOUT;
    //
    // TIMEOUT = TIMEOUT * 10;
    //
    // progDialogMessage = "Contacting update server...";
    // publishProgress(0);
    // Object filenameobj = SynchronousCallWebMethod("GetTotalUpdatesLink",
    // parameters);
    // TIMEOUT = oldTimeout;
    //
    // if (filenameobj == null)
    // {
    // Log.i("SynchronizerText", "Filename problem");
    // return false;
    // }
    // String filename = filenameobj.toString();
    //
    // url = url.toLowerCase().replace("retailservice.asmx", "Downloads/" +
    // filename);
    // String outputFile = Environment.getExternalStorageDirectory().toString()
    // + "/retail/temp.zip";
    // progDialogMessage = "Download update file...";
    // publishProgress(0);
    // Log.i("Syncrhonize Text", "Download start");
    // if (downloadFile(url, outputFile))
    // {
    // Log.i("Syncrhonize Text", "Download end");
    //
    // parameters = new HashMap<String, Object>();
    // parameters.put("userID", arg0[0].getRemoteGuid());
    // parameters.put("filename", filename);
    // SynchronousCallWebMethod("CleanFile", parameters);
    //
    // Log.i("Syncrhonize Text", "Grow Heap");
    // char[] tmp = new char[1024 * 1024 * 10];
    // tmp = null;
    // Log.i("Syncrhonize Text", "Grow Heap end");
    //
    // // UNZIP the file
    // Log.i("Syncrhonize Text", "Unpack start");
    // unpackZip(Environment.getExternalStorageDirectory().toString() +
    // "/retail/", "temp.zip");
    // Log.i("Syncrhonize Text", "Unpack end");
    // // Parse file
    // Log.i("Syncrhonize Text", "Processing start");
    // try
    // {
    // progDialogMessage = "Applying update file..."; // todo add
    // // info
    // publishProgress(0);
    // dbHelper.getWritableDatabase().execSQL("PRAGMA synchronous=1  ;");
    //
    // if (!parseFile(outputFile.replace("temp.zip", "Database.txt")))
    // return false;
    //
    // progDialogMessage = "Optimizing database for faster performance...";
    //
    // // Queries to restore fks
    // String[] queries = new String[] {
    // "update customer set pc_id =(select id from pricecatalog where pricecatalog.remoteGuid = customer.pcRemoteGuid), vl_id = (select id from Vatlevel where vatlevel.remoteGuid = customer.vatlevelremoteGuid ), store_id = (select id from store where store.remoteGuid = customer.storeremoteGuid ) where store_id is null or vl_id is null or pc_id is null or updatedon >= "
    // + customerUpdatedOn,
    // "update item set vatcategory_id = (select id from Vatcategory where Vatcategory.remoteGuid = Item.VatcategoryRemoteGuid), defaultBarcodeObject_id = (select id from Barcode where Barcode.remoteGuid = Item.defaultBarcodeRemoteGuid) where vatcategory_id is null or defaultBarcodeObject_id is null or updatedon >="
    // + itemupdatedon,
    // "update barcode set item_id= (select id from Item where item.remoteOid = barcode.itemRemoteGuid), MeasurementUnit_id= (select id from MeasurementUnit where MeasurementUnit.remoteGuid = barcode.measurementUnitRemoteGuid) where item_id is null or MeasurementUnit_id is null or updatedon >="
    // + barcodeupdatedon,
    // "update itemanalytictree set item_id = (select id from item where item.remoteOid = itemanalytictree .itemRemoteGuid), itemcategory_id = (select id from itemcategory where itemcategory.remoteguid = itemanalytictree .itemcategoryRemoteGuid) where item_id is null or itemcategory_id is null or updatedon >= "
    // + iatupdatedon,
    // "update itemcategory set parent_id = (select id from itemcategory a where a.remoteGuid = itemcategory.remoteparentguid)",
    // "update pricecatalog set parent_id = (select id from pricecatalog a where a.remoteGuid = pricecatalog .remoteparentguid)",
    // "update pricecatalogdetail set pc_id = (select id from pricecatalog pc where pc.remoteGuid = pricecatalogdetail.pcremoteguid), bc_id = (select id from barcode bc where bc.remoteGuid = pricecatalogdetail.barcoderemoteguid), item_id = (select id from item  where item.remoteoid = pricecatalogdetail.itemremoteguid) where item_id is null or bc_id is null or pc_id is null or updatedon >="
    // + pcdupdatedon,
    // "update vatfactor set vatlevel_id = (select id from vatlevel where vatlevel.remoteguid = vatfactor.vatlevelremoteguid), vatcategory_id = (select id from vatcategory where vatcategory.remoteguid = vatfactor.vatcategoryremoteguid)",
    // "update offer set PriceCatalog_id = (select id from pricecatalog where offer.PriceCatalogremoteguid = pricecatalog.remoteguid)",
    // "update offerdetail set item_id = (select id from item where item.remoteoid = offerdetail.itemremoteguid), offer_id = (select id from offer where offer.remoteguid = offerdetail.offerremoteguid)",
    // "update address set customer_id = (select id from customer where customer.remoteGuid=address.customerRemoteGuid)"
    // };
    //
    // progDialog.setMax(queries.length + 1);
    // publishProgress(0);
    // int i = 0;
    // DatabaseConnection conn =
    // dbHelper.getConnectionSource().getReadWriteConnection();
    // conn.setAutoCommit(false);
    // for (String query : queries)
    // {
    // Log.i("Sync", "Executing:" + query);
    // dbHelper.getWritableDatabase().compileStatement(query).executeUpdateDelete();
    // publishProgress((++i));
    // }
    // conn.setAutoCommit(true);
    // // restoring database status
    // dbHelper.getWritableDatabase().execSQL("PRAGMA synchronous=2  ;");
    // SynchronizeItemImages(itemupdatedon, arg0[0]);
    // }
    // catch (Exception ex)
    // {
    // ex.printStackTrace();
    // return false;
    // }
    // Log.i("Syncrhonize Text", "Processing end");
    // }
    // else
    // {
    // return false;
    // }
    // Log.i("Syncrhonize Text", "End");
    // ((MainActivity)
    // mContext).getPreferences(mContext.MODE_PRIVATE).edit().putBoolean("init",
    // true).commit();
    // ((MainActivity) mContext).isDBInitialized = true;
    // return true;
    // }
    //
    // private boolean SynchronizeItemImages(long iuo, User usr)
    // {
    // progDialogMessage = "Preparing request to update images from server ...";
    // HashSet<String> toRequest = new HashSet<String>();
    // publishProgress(0);
    // try
    // {
    // // List<Item> allItems = dbHelper.getItems().queryForAll();
    //
    // for (Item itm : dbHelper.getItems())
    // {
    // if (itm.getImageOid() == null)
    // itm = dbHelper.getItems().queryForId(itm.getID());
    // String filename = MainActivity.IMAGES_PATH + "32/img_" +
    // itm.getImageOid().replace("-", "").toLowerCase();
    // if (itm.getUpdatedOn() >= iuo)
    // toRequest.add(itm.getImageOid().replace("-", "").toLowerCase());
    // else
    // {
    // File file = new File(filename);
    // if (!file.exists())
    // toRequest.add(itm.getImageOid().replace("-", "").toLowerCase());
    // }
    // }
    // }
    // catch (Exception ex)
    // {
    // Log.w("SynchronizeItemImages", "error in database");
    // return false;
    // }
    //
    // // //////
    // StringArraySerializer toSend = new StringArraySerializer();
    // toSend.addAll(toRequest);
    // // String [] toSend = toRequest.toArray(new String[0]);
    // HashMap parameters = new HashMap();
    //
    // parameters.put("userID", usr.getRemoteGuid());
    // parameters.put("imageGuids", toSend);
    //
    // String url = ((MainActivity) mContext).appsettings.getServiceUrl();
    // int oldTimeout = TIMEOUT;
    // TIMEOUT = 300000;
    // progDialogMessage = "Requesting images update from server ...";
    // publishProgress(0);
    // Object filenameobj = SynchronousCallWebMethod("GetImagesLink",
    // parameters);
    // TIMEOUT = oldTimeout;
    // if (filenameobj == null)
    // {
    // Log.i("SynchronizerText", "Filename problem");
    // return false;
    // }
    // String filename = filenameobj.toString();
    //
    // url = url.toLowerCase().replace("retailservice.asmx", "Downloads/" +
    // filename);
    // String outputFile = Environment.getExternalStorageDirectory().toString()
    // + "/retail/images/imgs.zip";
    // progDialogMessage = "Download update file...";
    // publishProgress(0);
    // if (downloadFile(url, outputFile))
    // {
    // // MainActivity.IMAGES_PATH
    // unpackZip(Environment.getExternalStorageDirectory().toString() +
    // "/retail/images/", "imgs.zip");
    // parameters = new HashMap<String, Object>();
    // parameters.put("userID", usr.getRemoteGuid());
    // parameters.put("filename", filename);
    // SynchronousCallWebMethod("CleanFile", parameters);
    //
    // return true;
    // }
    // return false;
    // }
    //
    // private boolean downloadFile(String url, String outputFile)
    // {
    // try
    // {
    // progDialogMessage = "Downloading File";
    // URL u = new URL(url);
    // URLConnection conn = u.openConnection();
    // int contentLength = conn.getContentLength(), current = 0;
    //
    // progDialog.setMax(contentLength);
    // publishProgress(current);
    //
    // File file = new File(outputFile.substring(0,
    // outputFile.lastIndexOf('/')));
    // file.mkdirs();
    //
    // DataInputStream stream = new DataInputStream(u.openStream());
    // DataOutputStream fos = new DataOutputStream(new
    // FileOutputStream(outputFile));
    //
    // byte[] buffer = new byte[32768];
    // int len, ct = 0;
    // while ((len = stream.read(buffer)) != -1)
    // {
    // fos.write(buffer, 0, len);
    // fos.flush();
    // current += len;
    // ct++;
    // if (ct % 20 == 0)
    // publishProgress(current);
    // }
    // // stream.readFully(buffer);
    // stream.close();
    //
    // // fos.write(buffer);
    // // fos.flush();
    // fos.close();
    // return true;
    // }
    // catch (FileNotFoundException e)
    // {
    // return false; // swallow a 404
    // }
    // catch (IOException e)
    // {
    // return false; // swallow a 404
    // }
    // }
    //
    // private boolean SynchronizeUserStoreAccessOfUser(User user)
    // {
    // try
    // {
    // DatabaseConnection conn =
    // dbHelper.getConnectionSource().getReadWriteConnection();
    // HashMap<String, Object> params;
    // params = new HashMap<String, Object>();
    // params.put("userID", user.getRemoteGuid());
    // Object result = callWebMethod("getUserStoreAccess", params);
    // SoapObject sresult = (SoapObject) result;
    // int limit = sresult.getPropertyCount();
    // for (int i = 0; i < limit; i++)
    // {
    // SoapObject obj = (SoapObject) sresult.getProperty(i);
    // UserStoreAccess ct;
    // Store store = dbHelper.findObject(Store.class, "remoteGuid",
    // obj.getProperty("StoreID"), FilterType.EQUALS);
    // if (store != null)
    // {
    // ct = dbHelper.findObject(UserStoreAccess.class, OperatorType.AND, new
    // BinaryOperator("store_id", store.getID(), FilterType.EQUALS), new
    // BinaryOperator("user_id", user.getID(), FilterType.EQUALS));
    // boolean newct;
    // if (ct == null)
    // {
    // newct = true;
    // ct = new UserStoreAccess();
    // }
    // else
    // {
    // newct = false;
    // }
    // ct.setUpdatedOn(Long.parseLong(obj.getProperty("UpdatedOn").toString()));
    // ct.setStore(dbHelper.getStoreByRemoteGuid(obj.getProperty("StoreID").toString()));
    // ct.setUser(dbHelper.getUserByRemoteGuid(obj.getProperty("UserID").toString()));
    //
    // if (newct)
    // dbHelper.getUserStoreAccessDao().create(ct);
    // else
    // dbHelper.getUserStoreAccessDao().update(ct);
    // if (i % COMMIT_SIZE == 0)
    // {
    // conn.setAutoCommit(true);
    // conn.setAutoCommit(false);
    // }
    // }
    //
    // }
    // Log.i("Sync UserStoreAccesses", limit + " completed successfully");
    // conn.setAutoCommit(true);
    // return true;
    // }
    // catch (Exception ex)
    // {
    // ex.printStackTrace();
    // return false;
    // }
    // }
    //
    // private boolean unpackZip(String path, String zipname)
    // {
    // InputStream is;
    // ZipInputStream zis;
    // try
    // {
    // String filename;
    // is = new FileInputStream(path + zipname);
    // zis = new ZipInputStream(new BufferedInputStream(is));
    // ZipEntry ze;
    // byte[] buffer = new byte[32768];
    // int count;
    //
    // while ((ze = zis.getNextEntry()) != null)
    // {
    //
    // filename = ze.getName();
    // String totalpath = path + filename;
    // totalpath = totalpath.substring(0, totalpath.lastIndexOf('/'));
    // File ffout = new File(totalpath);
    // ffout.mkdirs();
    // FileOutputStream fout = new FileOutputStream(path + filename);
    // progDialogMessage = "Unpacking...";
    // progDialog.setMax((int) ze.getSize());
    // int total = 0, ct = 0;
    // publishProgress(total);
    //
    // while ((count = zis.read(buffer)) != -1)
    // {
    // fout.write(buffer, 0, count);
    // // fout.flush();
    // total += count;
    // ct++;
    // if (ct % 400 == 0)
    // publishProgress(total);
    // }
    //
    // fout.close();
    // zis.closeEntry();
    // }
    //
    // zis.close();
    // }
    // catch (IOException e)
    // {
    // e.printStackTrace();
    // return false;
    // }
    //
    // return true;
    // }
    //
    // private String progDialogMessage = "";
    //
    // @Override
    // protected void onPreExecute()
    // {
    // super.onPreExecute();
    // progDialog = new ProgressDialog(mContext);
    // progDialog.setProgressStyle(ProgressDialog.STYLE_HORIZONTAL);
    // progDialog.setMessage(progDialogMessage);
    // progDialog.setCancelable(false);
    // ((MainActivity)
    // mContext).getWindow().addFlags(WindowManager.LayoutParams.FLAG_KEEP_SCREEN_ON);
    // }
    // }

    class SynchronizerFromMultiText extends AsyncTask<User, Integer, Object>
    {

	private <T extends IRetailPersistent> boolean SynchronizeActiveInactive(Dao<T, Long> dao, String objectType, User usr, Long originalUpdatedOn)
	{

	    HashMap parameters = new HashMap();
	    parameters.put("mobileUpdatedOn", originalUpdatedOn);
	    parameters.put("objectType", objectType);
	    parameters.put("userid", usr.getRemoteGuid());

	    Object result = SynchronousCallWebMethod("GetInactiveObjects", parameters);
	    SoapObject sresult = (SoapObject) result;
	    int limit = sresult == null ? 0 : sresult.getPropertyCount();
	    for (int i = 0; i < limit; i++)
	    {
		Object o = sresult.getProperty(i);
		String obj2Delete = ((SoapPrimitive) o).toString();
		try
		{

//		    if (objectType.equals("Item"))
//		    {
//			List<T> objs = dao.queryForEq("remoteOid", obj2Delete);
//			if (objs.size() > 0)
//			{
//			    T object = objs.get(0);
//			    dao.delete(object);
//			}
//
//		    }
//		    else
//		    {
			List<T> objs = dao.queryForEq("remoteGuid", obj2Delete);
			if (objs.size() > 0)
			{
			    T object = objs.get(0);
			    dao.delete(object);
			}
//		    }
		}
		catch (Exception ex)
		{
		    ex.printStackTrace();
		}
	    }

	    return true;
	}

	private boolean SynchronizeApplicationSettings(User user)
	{
	    try
	    {
		// VATCategory
		HashMap<String, Object> params = new HashMap<String, Object>();

		params.put("userID", user.getRemoteGuid());
		Object result = callWebMethod("getAppSettings", params);
		SoapObject sresult = (SoapObject) result;
		// int limit = sresult.getPropertyCount();
		// for (int i = 0; i < limit; i++)
		{
		    SoapObject obj = sresult;
		    ApplicationSettings ct;
		    ct = dbHelper.getApplicationSettings().queryForAll().get(0);
		    ct.setBarcodePadChar(obj.getPropertyAsString("BarcodePadChar").charAt(0));
		    ct.setCodePadChar(obj.getPropertyAsString("CodePadChar").charAt(0));
		    try
		    {
			ct.setDiscountPermited(Boolean.parseBoolean(obj.getPropertyAsString("DiscountPermited")));
		    }
		    catch (Exception e)
		    {

		    }
		    ct.setBarcodePadding(Boolean.parseBoolean(obj.getPropertyAsString("BarcodePad")));
		    ct.setCodePadding(Boolean.parseBoolean(obj.getPropertyAsString("CodePad")));

		    ct.setBarcodePadLength(Integer.parseInt(obj.getPropertyAsString("BarcodePadLength")));
		    ct.setCodePadLength(Integer.parseInt(obj.getPropertyAsString("CodePadLength")));

		    ct.setComputeDigits(Integer.parseInt(obj.getPropertyAsString("ComputeDigits")));
		    ct.setComputeValueDigits(Integer.parseInt(obj.getPropertyAsString("ComputeValueDigits")));

		    ct.setDisplayDigits(Integer.parseInt(obj.getPropertyAsString("DisplayDigits")));
		    ct.setDisplayValueDigits(Integer.parseInt(obj.getPropertyAsString("DisplayValueDigits")));

		    dbHelper.getApplicationSettings().update(ct);
		    ((MainActivity) mContext).appsettings = ct;

		}
		return true;
	    }
	    catch (Exception ex)
	    {
		ex.printStackTrace();
	    }
	    return false;
	}

	@Override
	protected void onPostExecute(Object result)
	{
	    
	    // TODO Auto-generated method stub
	    super.onPostExecute(result);
	    ((MainActivity) mContext).getWindow().clearFlags(WindowManager.LayoutParams.FLAG_KEEP_SCREEN_ON);

	    progDialog.dismiss();
	    if (result == null || (Boolean) result == true)
	    {
		((MainActivity) mContext).btnOrders.setEnabled(true);
		((MainActivity) mContext).btnCustomers.setEnabled(true);
		((MainActivity) mContext).btnItems.setEnabled(true);
		((MainActivity) mContext).foundCustomers.clear();
		((MainActivity) mContext).UpdateCustomers(false);
		((MainActivity) mContext).UpdateUI();
		((MainActivity) mContext).isDBInitialized = true;
		((MainActivity) mContext).UpdateSettingsComboBoxes();
		Utilities.ShowUninterruptingMessage(mContext, mContext.getString(R.string.updateCompletedSuccessfully), 5);
		((MainActivity) mContext).getPreferences(Context.MODE_PRIVATE).edit().putLong("lastUpdate", Calendar.getInstance().getTimeInMillis()).commit();
	    }
	    else
	    {
		Utilities.ShowUninterruptingMessage(mContext, mContext.getString(R.string.problem), 5);
	    }

	}
	

	@Override
	protected void onProgressUpdate(Integer... values)
	{
	    // TODO Auto-generated method stub
	    super.onProgressUpdate(values);
	    progDialog.setMessage(progDialogMessage);
	    if (!progDialog.isShowing())
	    {
		progDialog.show();
	    }
	    progDialog.setProgress(values[0]);
	}

	private ProgressDialog progDialog;
	private String[] stores = null, documentstatus = null, documentType = null, customers = null, items = null, itemATs = null, barcodes = null, itemcategory = null,
		pricecatalog = null, pricecatalogdetail = null, addresses = null;
	private SQLiteStatement customerInsert = null, customerUpdate = null, itemInsert = null, itemUpdate = null, barcodeInsert = null, barcodeUpdate = null,
		addressInsert = null, addressUpdate = null, addressSelect = null;
	private SQLiteStatement itemAnalyticTreeInsert, itemAnalyticTreeUpdate, itemCategoryInsert;
	private SQLiteStatement itemCategoryUpdate, pc_in, pc_up, pcd_in, measurementUnitsInsert, measurementUnitsUpdate;
	private SQLiteStatement pcd_up, documentStatusInsert, documentStatusUpdate, documentTypeInsert, documentTypeUpdate;
	private SQLiteStatement storeInsert, storeUpdate, linkedItemsInsert, linkedItemsUpdate;
	private String[] linkedItems = null, measurementUnits = null;
	private String[] vatCategories;
	private SQLiteStatement vcInsert;
	private SQLiteStatement vcUpdate;
	private String[] vatLevels;
	private SQLiteStatement vlInsert;
	private SQLiteStatement vlUpdate;
	private String[] vatFactors;
	private SQLiteStatement vfInsert;
	private SQLiteStatement vfUpdate;
	private String[] offers;
	private SQLiteStatement offerInsert;
	private SQLiteStatement offerUpdate;
	private SQLiteStatement offerDetailInsert;
	private String[] offerDetails;
	private SQLiteStatement offerDetailrUpdate;

	@SuppressLint("DefaultLocale")
	protected boolean parseLine(String line) throws Exception
	{
	    String[] elements = TextUtils.split(line, "\t&\t");
	    // line.split("\t&\t");
	    String query;
	    if (elements[0].equals("DocumentStatus"))
	    {
		if (documentstatus == null || elements[1].equals("Oid"))
		{
		    documentstatus = elements;
		    Log.i("Parsing", "documentstatus start");
		}
		else
		{

		    DocumentStatus ct = dbHelper.getDocumentStatusByRemoteGuid(elements[1]);
		    if (ct == null)
		    {
			if (documentStatusInsert == null)
			{
			    // Insert
			    Log.i("Parse Line", "Compiling documentStatusInsert");
			    query = "insert into DocumentStatus(remoteGuid,Description,IsDefault,UpdatedOn) ";
			    query += "values(?,?,?,?)";
			    documentStatusInsert = dbHelper.getWritableDatabase().compileStatement(query);
			}
			for (int i = 1; i < 5; i++)
			    documentStatusInsert.bindString(i, elements[i]);
			documentStatusInsert.executeInsert();

		    }
		    else
		    {

			if (documentStatusUpdate == null)
			{
			    Log.i("Parse Line", "Compiling documentStatusUpdate");
			    query = "update DocumentStatus set Description=?,IsDefault=?,UpdatedOn=? where remoteGuid = ?";
			    documentStatusUpdate = dbHelper.getWritableDatabase().compileStatement(query);
			}
			for (int i = 2; i < 5; i++)
			{
			    documentStatusUpdate.bindString(i - 1, elements[i]);
			}
			documentStatusUpdate.bindString(4, elements[1]);
			documentStatusUpdate.executeUpdateDelete();
		    }
		}
	    }
	    else if (elements[0].equals("DocumentType"))
	    {
		if (documentType == null || elements[1].equals("Oid"))
		{
		    documentType = elements;
		    Log.i("Parsing", "documentType start");
		}
		else
		{

		    DocumentType ct = dbHelper.getDocumentTypeByRemoteGuid(elements[1]);
		    if (ct == null)
		    {
			if (documentTypeInsert == null)
			{
			    // Insert
			    Log.i("Parse Line", "Compiling documentTypeInsert");
			    query = "insert into DocumentType(remoteGuid,Code,Description,IsDefault,UpdatedOn) ";
			    query += "values(?,?,?,?,?)";
			    documentTypeInsert = dbHelper.getWritableDatabase().compileStatement(query);
			}
			for (int i = 1; i < 6; i++)
			    documentTypeInsert.bindString(i, elements[i]);
			documentTypeInsert.executeInsert();

		    }
		    else
		    {

			if (documentTypeUpdate == null)
			{
			    Log.i("Parse Line", "Compiling documentTypeUpdate");
			    query = "update DocumentType set Code=?,Description=?,IsDefault=?,UpdatedOn=? where remoteGuid = ?";
			    documentTypeUpdate = dbHelper.getWritableDatabase().compileStatement(query);
			}
			for (int i = 2; i < 6; i++)
			{
			    documentTypeUpdate.bindString(i - 1, elements[i]);
			}
			documentTypeUpdate.bindString(4, elements[1]);
			documentTypeUpdate.executeUpdateDelete();
		    }
		}
	    }
	    else if (elements[0].equals("Store"))
	    {
		if (stores == null || elements[1].equals("Oid"))
		{
		    stores = elements;
		    Log.i("Parsing", "Store start");
		}
		else
		{

		    Store ct = dbHelper.getStoreByRemoteGuid(elements[1]);
		    if (ct == null)
		    {
			if (storeInsert == null)
			{
			    // Insert
			    Log.i("Parse Line", "Compiling storeInsert");
			    query = "insert into Store(remoteGuid,Code,Name,IsCentralStore,UpdatedOn) ";
			    query += "values(?,?,?,?,?)";
			    storeInsert = dbHelper.getWritableDatabase().compileStatement(query);
			}
			for (int i = 1; i < 6; i++)
			    storeInsert.bindString(i, elements[i]);
			storeInsert.executeInsert();

		    }
		    else
		    {

			if (storeUpdate == null)
			{
			    Log.i("Parse Line", "Compiling storeUpdate");
			    query = "update Store set Code=?,Name=?,IsCentralStore=?,UpdatedOn=? where remoteGuid = ?";
			    storeUpdate = dbHelper.getWritableDatabase().compileStatement(query);
			}
			for (int i = 2; i < 6; i++)
			{
			    storeUpdate.bindString(i - 1, elements[i]);
			}
			storeUpdate.bindString(5, elements[1]);
			storeUpdate.executeUpdateDelete();
		    }
		}
	    }
	    else if (elements[0].equals("LinkedItem"))
	    {
		if (linkedItems == null || elements[1].equals("Oid"))
		{
		    linkedItems = elements;
		    Log.i("Parsing", "LinkedItem start");
		}
		else
		{

		    LinkedItem ct = dbHelper.getLinkedItemByRemoteGuid(elements[1]);
		    if (ct == null)
		    {
			if (linkedItemsInsert == null)
			{
			    // Insert
			    Log.i("Parse Line", "Compiling linkedItemsInsert");
			    query = "insert into LinkedItem(remoteGuid,itemGuid,subItemGuid,UpdatedOn) ";
			    query += "values(?,?,?,?)";
			    linkedItemsInsert = dbHelper.getWritableDatabase().compileStatement(query);
			}
			for (int i = 1; i < 5; i++)
			    linkedItemsInsert.bindString(i, elements[i]);
			linkedItemsInsert.executeInsert();

		    }
		    else
		    {

			if (linkedItemsUpdate == null)
			{
			    Log.i("Parse Line", "Compiling linkedItemsUpdate");
			    query = "update LinkedItem set itemGuid=?,subItemGuid=?,UpdatedOn=? where remoteGuid = ?";
			    linkedItemsUpdate = dbHelper.getWritableDatabase().compileStatement(query);
			}
			for (int i = 2; i < 5; i++)
			{
			    linkedItemsUpdate.bindString(i - 1, elements[i]);
			}
			linkedItemsUpdate.bindString(4, elements[1]);
			linkedItemsUpdate.executeUpdateDelete();
		    }
		}
	    }
	    else if (elements[0].equals("MeasurementUnit") || elements[0].equals("MesurmentUnits"))  //"MesurmentUnits" for backwards compartibility
	    {
		if (measurementUnits == null || elements[1].equals("Oid"))
		{
		    measurementUnits = elements;
		    Log.i("Parsing", "MeasurementUnit start");
		}
		else
		{

		    MeasurementUnit ct = dbHelper.getMeasurementUnitByRemoteGuid(elements[1]);
		    if (ct == null)
		    {
			if (measurementUnitsInsert == null)
			{
			    // Insert
			    Log.i("Parse Line", "Compiling measurementUnitsInsert");
			    query = "insert into MeasurementUnit(remoteGuid,Description,supportDecimals,UpdatedOn) ";
			    query += "values(?,?,?,?)";
			    measurementUnitsInsert = dbHelper.getWritableDatabase().compileStatement(query);
			}
			for (int i = 1; i < 5; i++)
			    measurementUnitsInsert.bindString(i, elements[i]);
			measurementUnitsInsert.executeInsert();

		    }
		    else
		    {

			if (measurementUnitsUpdate == null)
			{
			    Log.i("Parse Line", "Compiling measurementUnitsUpdate");
			    query = "update MeasurementUnit set Description=?,supportDecimals=?,UpdatedOn=? where remoteGuid = ?";
			    measurementUnitsUpdate = dbHelper.getWritableDatabase().compileStatement(query);
			}
			for (int i = 2; i < 5; i++)
			{
			    measurementUnitsUpdate.bindString(i - 1, elements[i]);
			}
			measurementUnitsUpdate.bindString(4, elements[1]);
			measurementUnitsUpdate.executeUpdateDelete();
		    }
		}
	    }
	    else if (elements[0].equals("VatCategory"))
	    {
		if (vatCategories == null || elements[1].equals("Oid"))
		{
		    vatCategories = elements;
		    Log.i("Parsing", "vatCategories start");
		}
		else
		{

		    VATCategory ct = dbHelper.getVatCategoryByRemoteGuid(elements[1]);
		    if (ct == null)
		    {
			if (vcInsert == null)
			{
			    // Insert
			    Log.i("Parse Line", "Compiling vcInsert");
			    query = "insert into VATCategory(remoteGuid,name,UpdatedOn) ";
			    query += "values(?,?,?)";
			    vcInsert = dbHelper.getWritableDatabase().compileStatement(query);
			}
			for (int i = 1; i < 4; i++)
			    vcInsert.bindString(i, elements[i]);
			vcInsert.executeInsert();

		    }
		    else
		    {

			if (vcUpdate == null)
			{
			    Log.i("Parse Line", "Compiling vcUpdate");
			    query = "update VATCategory set name=?,UpdatedOn=? where remoteGuid = ?";
			    vcUpdate = dbHelper.getWritableDatabase().compileStatement(query);
			}
			for (int i = 2; i < 4; i++)
			{
			    vcUpdate.bindString(i - 1, elements[i]);
			}
			vcUpdate.bindString(3, elements[1]);
			vcUpdate.executeUpdateDelete();
		    }
		}
	    }
	    else if (elements[0].equals("VatLevel"))
	    {
		if (vatLevels == null || elements[1].equals("Oid"))
		{
		    vatLevels = elements;
		    Log.i("Parsing", "vatLevels start");
		}
		else
		{

		    VATLevel ct = dbHelper.getVatLevelByRemoteGuid(elements[1]);
		    if (ct == null)
		    {
			if (vlInsert == null)
			{
			    // Insert
			    Log.i("Parse Line", "Compiling vlInsert");
			    query = "insert into VatLevel(remoteGuid,name,'Default',UpdatedOn) ";
			    query += "values(?,?,?,?)";
			    vlInsert = dbHelper.getWritableDatabase().compileStatement(query);
			}
			for (int i = 1; i < 5; i++)
			    vlInsert.bindString(i, elements[i]);
			vlInsert.executeInsert();

		    }
		    else
		    {

			if (vlUpdate == null)
			{
			    Log.i("Parse Line", "Compiling vlUpdate");
			    query = "update VatLevel set name=?,'Default'=?,UpdatedOn=? where remoteGuid = ?";
			    vlUpdate = dbHelper.getWritableDatabase().compileStatement(query);
			}
			for (int i = 2; i < 5; i++)
			{
			    vlUpdate.bindString(i - 1, elements[i]);
			}
			vlUpdate.bindString(4, elements[1]);
			vlUpdate.executeUpdateDelete();
		    }
		}
	    }
	    else if (elements[0].equals("VatFactor"))
	    {
		if (vatFactors == null || elements[1].equals("Oid"))
		{
		    vatFactors = elements;
		    Log.i("Parsing", "vatFactors start");
		}
		else
		{

		    VATFactor ct = dbHelper.getVatFactorByRemoteGuid(elements[1]);
		    if (ct == null)
		    {
			if (vfInsert == null)
			{
			    // Insert
			    Log.i("Parse Line", "Compiling vfInsert");
			    query = "insert into VatFactor(remoteGuid,vatCategoryRemoteGuid,vatLevelRemoteGuid,vatFactor,UpdatedOn) ";
			    query += "values(?,?,?,?,?)";
			    vfInsert = dbHelper.getWritableDatabase().compileStatement(query);
			}
			for (int i = 1; i < 6; i++)
			    vfInsert.bindString(i, elements[i].replace(",", "."));
			vfInsert.executeInsert();

		    }
		    else
		    {

			if (vfUpdate == null)
			{
			    Log.i("Parse Line", "Compiling vfUpdate");
			    query = "update VatFactor set vatCategoryRemoteGuid=?,vatLevelRemoteGuid=?,vatFactor=?,UpdatedOn=? where remoteGuid = ?";
			    vfUpdate = dbHelper.getWritableDatabase().compileStatement(query);
			}
			for (int i = 2; i < 6; i++)
			{
			    vfUpdate.bindString(i - 1, elements[i].replace(",", "."));
			}
			vfUpdate.bindString(5, elements[1]);
			vfUpdate.executeUpdateDelete();
		    }
		}
	    }
	    else if (elements[0].equals("Offer"))
	    {
		if (offers == null || elements[1].equals("Oid"))
		{
		    offers = elements;
		    Log.i("Parsing", "offers start");
		}
		else
		{

		    Offer ct = dbHelper.getOfferByRemoteGuid(elements[1]);
		    if (ct == null)
		    {
			if (offerInsert == null)
			{
			    // Insert
			    // "Oid" + DELIMITER + "PriceCatalogOid" +
			    // "Description" + "Description2" + "isActive" +
			    // "StartDate" + "EndDate" + "UpdatedOn");
			    Log.i("Parse Line", "Compiling offerInsert");
			    query = "insert into Offer(remoteGuid,PriceCatalogRemoteGuid,Description,Description2,Active,StartDate,EndDate,UpdatedOn) ";
			    query += "values(?,?,?,?,?,?,?,?)";
			    offerInsert = dbHelper.getWritableDatabase().compileStatement(query);
			}
			for (int i = 1; i < 9; i++)
			    offerInsert.bindString(i, elements[i]);
			offerInsert.executeInsert();

		    }
		    else
		    {

			if (offerUpdate == null)
			{
			    Log.i("Parse Line", "Compiling offerUpdate");
			    query = "update Offer set PriceCatalogRemoteGuid=?,Description=?,Description2=?,Active=?,StartDate=?,EndDate=?,UpdatedOn=? where remoteGuid = ?";
			    offerUpdate = dbHelper.getWritableDatabase().compileStatement(query);
			}
			for (int i = 2; i < 9; i++)
			{
			    offerUpdate.bindString(i - 1, elements[i]);
			}
			offerUpdate.bindString(8, elements[1]);
			offerUpdate.executeUpdateDelete();
		    }
		}
	    }
	    else if (elements[0].equals("OfferDetail"))
	    {
		if (offerDetails == null || elements[1].equals("Oid"))
		{
		    offerDetails = elements;
		    Log.i("Parsing", "offers start");
		}
		else
		{

		    OfferDetail ct = dbHelper.getOfferDetailByRemoteGuid(elements[1]);
		    if (ct == null)
		    {
			if (offerDetailInsert == null)
			{
			    // Insert
			    // txt.WriteLine("OfferDetail" + + "Oid" + +
			    // "OfferOid" + + "ItemOid" + + "isActive" + +
			    // "UpdatedOn");
			    Log.i("Parse Line", "Compiling offerDetailInsert");
			    query = "insert into OfferDetail(remoteGuid,OfferRemoteGuid,ItemRemoteGuid,Active,UpdatedOn) ";
			    query += "values(?,?,?,?,?)";
			    offerDetailInsert = dbHelper.getWritableDatabase().compileStatement(query);
			}
			for (int i = 1; i < 6; i++)
			    offerDetailInsert.bindString(i, elements[i]);
			offerDetailInsert.executeInsert();

		    }
		    else
		    {
			if (offerDetailrUpdate == null)
			{
			    Log.i("Parse Line", "Compiling offerUpdate");
			    query = "update OfferDetail set OfferRemoteGuid=?,ItemRemoteGuid=?,Active=?,UpdatedOn=? where remoteGuid = ?";
			    offerDetailrUpdate = dbHelper.getWritableDatabase().compileStatement(query);
			}
			for (int i = 2; i < 6; i++)
			{
			    offerDetailrUpdate.bindString(i - 1, elements[i]);
			}
			offerDetailrUpdate.bindString(5, elements[1]);
			offerDetailrUpdate.executeUpdateDelete();
		    }
		}
	    }
	    else if (elements[0].equals("Address"))
	    {
		if (addresses == null)
		{
		    addresses = elements;
		    Log.i("Parsing", "Addresses start");
		}
		else
		{

		    // Address ct =
		    // dbHelper.getAddressByRemoteGuid(elements[1]);
		    // if(addressSelect == null)
		    // {
		    // query =
		    // "select ID from address where remoteGuid = ? and customerRemoteGuid = ?";
		    // addressSelect =
		    // dbHelper.getWritableDatabase().compileStatement(query);
		    // }
		    // else
		    // {
		    // addressSelect.clearBindings();
		    // }

		    Address ct = null;
		    try
		    {
			// Log.v("Address Insert", "Begin Find");

			// addressSelect.bindString(0, elements[1]);
			// addressSelect.bindString(1, elements[2]);
			// addressSelect.
			// ct = dbHelper.getAddressByRemoteGuid(elements[1]);
			ct = dbHelper.getAddressByRemoteGuid(elements[1], elements[2]);
			// ct = dbHelper.findObject(Address.class,
			// OperatorType.AND, new BinaryOperator("remoteGuid",
			// elements[1]), new
			// BinaryOperator("customerRemoteGuid", elements[2]));
			// Log.v("Address Insert", "End Find");
		    }
		    catch (Exception e)
		    {
			Log.e("Query Failed", "Address query failed");
			e.printStackTrace();
		    }

		    if (ct == null || !ct.getCustomerRemoteGuid().equals(elements[2]))
		    {
			if (addressInsert == null)
			{

			    // Insert
			    Log.i("Parse Line", "Compiling address insert");
			    query = "insert into address(remoteGuid, customerRemoteGuid, Address) ";
			    query += "values(?,?,?)";
			    addressInsert = dbHelper.getWritableDatabase().compileStatement(query);
			}
			for (int i = 1; i < 4; i++)
			{
			    addressInsert.bindString(i, elements[i]);
			}

			// Log.v("Address Insert", "Begin Insert");
			addressInsert.executeInsert();
			// Log.v("Address Insert", "End Insert");

		    }
		    else
		    {

			if (addressUpdate == null)
			{
			    Log.i("Parse Line", "Compiling address update");
			    query = "update address set customerRemoteGuid=?,Address=? where remoteGuid = ?";
			    addressUpdate = dbHelper.getWritableDatabase().compileStatement(query);
			}
			for (int i = 2; i < 4; i++)
			{
			    addressUpdate.bindString(i - 1, elements[i]);
			}

			addressUpdate.bindString(3, elements[1]);
			addressUpdate.executeUpdateDelete();
		    }
		}
	    }
	    else if (elements[0].equals("Customer"))
	    {
		try
		{
		    if (customers == null || elements[8].equals("UpdatedOn"))
		    {
			customers = elements;
			Log.i("Parsing", "Customers start");
		    }
		    else
		    {
			// Log.v("Customer Insert", "Begin Find");
			Customer ct = dbHelper.getCustomerByRemoteGuid(elements[5]);
			// Log.v("Customer Insert", "End Find");
			if (ct == null)
			{
			    if (customerInsert == null)
			    {
				// Insert
				Log.i("Parse Line", "Compiling customer insert");
				query = "insert into customer(Code, CompanyName,DefaultAddress,DefaultPhone,remoteGuid,pcRemoteGuid,storeRemoteGuid,UpdatedOn,TaxCode,vatLevelRemoteGuid,lowerCompanyName) ";
				query += "values(?,?,?,?,?,?,?,?,?,?,?)";
				customerInsert = dbHelper.getWritableDatabase().compileStatement(query);
			    }
			    for (int i = 1; i < 11; i++)
				customerInsert.bindString(i, elements[i]);
			    customerInsert.bindString(11, elements[2].toLowerCase());
			    customerInsert.executeInsert();

			}
			else
			{

			    if (customerUpdate == null)
			    {
				Log.i("Parse Line", "Compiling customer update");
				query = "update customer set Code=?, CompanyName=?,DefaultAddress=?,DefaultPhone=?,pcRemoteGuid=?,storeRemoteGuid=?,UpdatedOn=?,TaxCode=?,vatLevelRemoteGuid=?,lowerCompanyName=? where remoteGuid = ?";
				customerUpdate = dbHelper.getWritableDatabase().compileStatement(query);
			    }
			    for (int i = 1; i < 5; i++)
			    {
				customerUpdate.bindString(i, elements[i]);
			    }
			    for (int i = 6; i < 11; i++)
			    {
				customerUpdate.bindString(i - 1, elements[i]);
			    }
			    customerUpdate.bindString(10, elements[2].toLowerCase());
			    customerUpdate.bindString(11, elements[5]);
			    customerUpdate.executeUpdateDelete();
			}
		    }
		}
		catch (Exception e)
		{
		    throw e;
		}
	    }
	    else if (elements[0].equals("Item"))
	    {
		if (items == null || elements[1].equals("Code"))
		{
		    items = elements;
		    Log.i("Parsing", "items start");
		}
		else
		{
		    Item ct = dbHelper.getItemByRemoteGuid(elements[8]);
		    if (ct == null)
		    {
			if (itemInsert == null)
			{
			    Log.i("Parse Line", "Compiling item insert");
			    query = "insert into item(Code, defaultBarcodeRemoteGuid,imageOid,InsertedOn,isactive,maxOrderQty,Name,remoteGuid,packingQty,UpdatedOn,vatCategoryRemoteGuid,loweCaseName)";
			    query += "values(?,?,?,?,?,?,?,?,?,?,?,?)";
			    itemInsert = dbHelper.getWritableDatabase().compileStatement(query);
			}
			for (int i = 1; i < 12; i++)
			    itemInsert.bindString(i, elements[i]);
			itemInsert.bindString(12, elements[7].toLowerCase());
			itemInsert.executeInsert();
		    }
		    else
		    {
			if (itemUpdate == null)
			{
			    Log.i("Parse Line", "Compiling item update");
			    query = "update item set Code=?, defaultBarcodeRemoteGuid=?,imageOid=?,InsertedOn=?,isactive=?,maxOrderQty=?,Name=?,packingQty=?,UpdatedOn=?,vatCategoryRemoteGuid=?,loweCaseName=?";
			    query += " where remoteGuid=?";
			    itemUpdate = dbHelper.getWritableDatabase().compileStatement(query);
			}
			for (int i = 1; i < 8; i++)
			    itemUpdate.bindString(i, elements[i]);
			for (int i = 9; i < 12; i++)
			    itemUpdate.bindString(i - 1, elements[i]);
			itemUpdate.bindString(11, elements[7].toLowerCase());
			itemUpdate.bindString(12, elements[8]);
			itemUpdate.executeUpdateDelete();
			
		    }
		}

	    }
	    else if (elements[0].equals("ItemAnalyticTree"))
	    {
		if (itemATs == null || elements[1].equals("Oid"))
		{
		    itemATs = elements;
		    Log.i("Parsing", "Itemanalytictree start");
		}
		else
		{
		    ItemAnalyticTree ct = dbHelper.getItemAnalyticTreeByRemoteGuid(elements[1]);
		    if (ct == null)
		    {
			if (itemAnalyticTreeInsert == null)
			{
			    // Insert
			    Log.i("Parse Line", "Compiling itemAT insert");
			    query = "insert into itemanalytictree(remoteGuid, itemRemoteGuid,itemCategoryRemoteGuid,UpdatedOn) ";
			    query += " values(?,?,?,?)";
			    itemAnalyticTreeInsert = dbHelper.getWritableDatabase().compileStatement(query);
			}
			for (int i = 1; i < 5; i++)
			    itemAnalyticTreeInsert.bindString(i, elements[i]);
			itemAnalyticTreeInsert.executeInsert();
		    }
		    else
		    {

			// update
			if (itemAnalyticTreeUpdate == null)
			{
			    Log.i("Parse Line", "Compiling itemAT update");
			    query = "update itemanalytictree set itemRemoteGuid=?,itemCategoryRemoteGuid=?,UpdatedOn=?";
			    query += " where remoteGuid = ?";
			    itemAnalyticTreeUpdate = dbHelper.getWritableDatabase().compileStatement(query);
			}

			for (int i = 2; i < 5; i++)
			    itemAnalyticTreeUpdate.bindString(i - 1, elements[i]);
			itemAnalyticTreeUpdate.bindString(4, elements[1]);
			itemAnalyticTreeUpdate.executeUpdateDelete();
		    }
		}

	    }
	    else if (elements[0].equals("Barcode"))
	    {
		if (barcodes == null || elements[1].equals("Oid"))
		{
		    barcodes = elements;
		    Log.i("Parsing", "Barcode start");
		}
		else
		{
		    Barcode ct = dbHelper.getBarcodeByRemoteGuid(elements[1]);
		    if (ct == null)
		    {
			// Insert
			if (barcodeInsert == null)
			{
			    Log.i("Parse Line", "Compiling barcodeInsert");
			    query = "insert into barcode(remoteGuid, itemRemoteGuid,measurementUnitRemoteGuid,Code,UpdatedOn) ";
			    query += " values(?,?,?,?,?)";
			    barcodeInsert = dbHelper.getWritableDatabase().compileStatement(query);
			}
			for (int i = 1; i < 6; i++)
			    barcodeInsert.bindString(i, elements[i]);
			barcodeInsert.executeInsert();
		    }
		    else
		    {
			// update
			if (barcodeUpdate == null)
			{
			    Log.i("Parse Line", "Compiling barcodeUpdate");
			    query = "update barcode set itemRemoteGuid=?,measurementUnitRemoteGuid=?,Code=?,UpdatedOn=?";
			    query += " where remoteGuid = ?";
			    barcodeUpdate = dbHelper.getWritableDatabase().compileStatement(query);
			}
			for (int i = 2; i < 6; i++)
			{
			    barcodeUpdate.bindString(i - 1, elements[i]);
			}
			barcodeUpdate.bindString(5, elements[1]);
		    }
		}

	    }
	    else if (elements[0].equals("ItemCategory"))
	    {
		if (itemcategory == null || elements[1].equals("Oid"))
		{
		    itemcategory = elements;
		    Log.i("Parsing", "itemcategory start");
		}
		else
		{
		    ItemCategory ct = dbHelper.getItemCategoryByRemoteGuid(elements[1]);
		    if (ct == null)
		    {
			if (itemCategoryInsert == null)
			{
			    // Insert
			    Log.i("Parse Line", "Compiling itemCategoryInsert");
			    query = "insert into itemcategory(remoteGuid, remoteParentGuid,Name,Code,UpdatedOn) ";
			    query += " values(?,?,?,?,?)";
			    itemCategoryInsert = dbHelper.getWritableDatabase().compileStatement(query);
			}
			for (int i = 1; i < 6; i++)
			    itemCategoryInsert.bindString(i, elements[i]);
			itemCategoryInsert.executeInsert();
		    }
		    else
		    {
			if (itemCategoryUpdate == null)
			{
			    // update
			    Log.i("Parse Line", "Compiling itemCategoryUpdate");
			    query = "update itemcategory set remoteParentGuid=?,Name=?,Code=?,UpdatedOn=?";
			    query += " where remoteGuid =?";
			    itemCategoryUpdate = dbHelper.getWritableDatabase().compileStatement(query);

			}
			for (int i = 2; i < 6; i++)
			{
			    itemCategoryUpdate.bindString(i - 1, elements[i]);
			}
			itemCategoryUpdate.bindString(5, elements[1]);
			itemCategoryUpdate.executeUpdateDelete();
		    }
		}
	    }
	    else if (elements[0].equals("PriceCatalog"))
	    {
		if (pricecatalog == null || elements[1].equals("Oid"))
		{
		    pricecatalog = elements;
		    Log.i("Parsing", "pricecatalog start");
		}
		else
		{
		    PriceCatalog ct = dbHelper.getPriceCatalogByRemoteGuid(elements[1]);
		    if (ct == null)
		    {
			if (pc_in == null)
			{
			    // Insert
			    Log.i("Parse Line", "Compiling pc_in");
			    query = "insert into pricecatalog(remoteGuid, remoteParentGuid,Name,Code,UpdatedOn) ";
			    query += " values(?,?,?,?,?)";
			    pc_in = dbHelper.getWritableDatabase().compileStatement(query);
			}
			for (int i = 1; i < 6; i++)
			    pc_in.bindString(i, elements[i]);
			pc_in.executeInsert();
		    }
		    else
		    {
			if (pc_up == null)
			{
			    // update
			    Log.i("Parse Line", "Compiling pc_up");
			    query = "update pricecatalog set remoteParentGuid=?,Name=?,Code=?,UpdatedOn=?";
			    query += " where remoteGuid =?";
			    pc_up = dbHelper.getWritableDatabase().compileStatement(query);

			}
			for (int i = 2; i < 6; i++)
			{
			    pc_up.bindString(i - 1, elements[i]);
			}
			pc_up.bindString(5, elements[1]);
			pc_up.executeUpdateDelete();
		    }
		}
	    }
	    else if (elements[0].equals("PriceCatalogDetail"))
	    {
		if (pricecatalogdetail == null || elements[1].equals("Oid"))
		{
		    pricecatalogdetail = elements;
		    Log.i("Parsing", "pricecatalogdetail start");
		}
		else
		{
		    PriceCatalogDetail ct = dbHelper.getPriceCatalogDetailByRemoteGuid(elements[1]);
		    if (ct == null)
		    {
			if (pcd_in == null)
			{
			    // Insert
			    Log.i("Parse Line", "Compiling pcd_in");
			    query = "insert into pricecatalogdetail(remoteGuid, barcodeRemoteGuid,pcRemoteGuid,itemRemoteGuid,price,discount,VATIncluded,UpdatedOn) ";
			    query += " values(?,?,?,?,?,?,?,?)";
			    pcd_in = dbHelper.getWritableDatabase().compileStatement(query);
			}
			for (int i = 1; i < 9; i++)
			    pcd_in.bindString(i, elements[i].replace(",", "."));
			pcd_in.executeInsert();
		    }
		    else
		    {
			if (pcd_up == null)
			{
			    // update
			    query = "update pricecatalogdetail set barcodeRemoteGuid=?,pcRemoteGuid=?,itemRemoteGuid=?,price=?,discount=?,VATIncluded=?,UpdatedOn=?";
			    query += " where remoteGuid = ?";
			    pcd_up = dbHelper.getWritableDatabase().compileStatement(query);
			}
			for (int i = 2; i < 9; i++)
			    pcd_up.bindString(i - 1, elements[i].replace(",", "."));

			pcd_up.bindString(8, elements[1]);
			pcd_up.executeUpdateDelete();
		    }
		}
	    }
	    // dbHelper.getWritableDatabase().compileStatement(query);
	    // dbHelper.getWritableDatabase().execSQL(query);

	    elements = null;
	    line = null;
	    // PriceCatalogDetail, Oid, BarcodeOid, PriceCatalogOid, ItemOid,
	    // value, discount, vatIncluded, UpdatedOn
	    return true;
	}

	protected boolean parseFile(String filename)
	{
	    try
	    {
		DatabaseConnection conn = dbHelper.getConnectionSource().getReadWriteConnection();
		try
		{
		    int lines = 0;
		    File fl = new File(filename);
		    int flsize = (int) fl.length(), readbytes = 0;
		    progDialog.setMax(flsize);
		    fl = null;
		    FileInputStream fs = new FileInputStream(filename);
		    InputStreamReader st = new InputStreamReader(fs);
		    BufferedReader br = new BufferedReader(st);
		    conn.setAutoCommit(false);
		    String line;
		    while ((line = br.readLine()) != null)
		    {
			lines++;
			readbytes += line.length();
			if (lines % 500 == 0)
			{
			    publishProgress(readbytes);
			    if (lines % 20000 == 0)
			    {
				Log.i("Parser", "Commiting 20000 records - start");
				conn.setAutoCommit(true);
				conn.setAutoCommit(false);
				Log.i("Parser", "Commiting 20000 records - end");
			    }
			}
			if (!parseLine(line.replace("\0", "")))
			{
			    conn.setAutoCommit(true);
			    return false;
			}
			line = null;
		    }
		    publishProgress(flsize);
		}
		catch (Exception ex)
		{
		    ex.printStackTrace();
		    conn.setAutoCommit(true);
		    return false;
		}
		conn.setAutoCommit(true);
		return true;
	    }
	    catch (Exception ex)
	    {
		ex.printStackTrace();
		return false;
	    }
	}

	@Override
	protected Object doInBackground(User... arg0)
	{

	    Log.i("Syncrhonize Text", "Start");
	    progDialogMessage = "Initializing...";
	    publishProgress(0);
	    SynchronizeApplicationSettings(arg0[0]);
	    HashMap<Class, Long> originalUpdatedOns = dbHelper.getAllMaxUpdatedOn();

	    long originalItemUpdatedOn = originalUpdatedOns.get(Item.class);// dbHelper.getMaxItemUpdatedOn();
	    long originalBarcodeUpdatedOn = originalUpdatedOns.get(Barcode.class);// dbHelper.getMaxBarcodeUpdatedOn();
	    long originalCustomerUpdatedOn = originalUpdatedOns.get(Customer.class);
	    long originalIatUpdatedOn = originalUpdatedOns.get(ItemAnalyticTree.class);
	    long originalPCDUpdatedOn = originalUpdatedOns.get(PriceCatalogDetail.class);
	    // long originalDocumentStatusUpdatedOn =
	    // dbHelper.getMaxDocumentStatusUpdatedOn();
	    // long originalItemCategories =
	    // dbHelper.getMaxItemCategoriesUpdatedOn();
	    // long originalPriceCatalogUpdatedOn =
	    // dbHelper.getMaxPriceCatalog();

	    boolean isUpdateOver = false;

	    Log.i("Syncrhonize Text", "Grow Heap");
	    char[] tmp = new char[1024 * 1024 * 10];
	    tmp = null;
	    Log.i("Syncrhonize Text", "Grow Heap end");
	    int previousRowsRemaining = -1;
	    while (!isUpdateOver)
	    {
		// STEP 1 Download File
		String url = ((MainActivity) mContext).appsettings.getServiceUrl();
		HashMap<String, Object> parameters = new HashMap<String, Object>();

		long customerUpdatedOn = dbHelper.getMaxCustomerUpdatedOn(), itemupdatedon = dbHelper.getMaxItemUpdatedOn(), barcodeupdatedon = dbHelper.getMaxBarcodeUpdatedOn(), pcdupdatedon = dbHelper.getMaxPriceCatalogDetail(), iatupdatedon = dbHelper.getMaxItemAnalyticTreesUpdatedOn();
		parameters.put("Customerupdatedon", "" + customerUpdatedOn);
		parameters.put("Itemupdatedon", "" + dbHelper.getMaxItemUpdatedOn());
		parameters.put("IATupdatedon", "" + iatupdatedon);
		parameters.put("Barcodeupdatedon", "" + barcodeupdatedon);

		parameters.put("ICupdatedon", "" + dbHelper.getMaxItemCategoriesUpdatedOn());
		parameters.put("PCupdatedon", "" + dbHelper.getMaxPriceCatalog());
		parameters.put("PCDupdatedon", "" + pcdupdatedon);
		parameters.put("VCupdatedon", "" + dbHelper.getMaxVatCategory());

		parameters.put("VFupdatedon", "" + dbHelper.getMaxVatFactor());
		parameters.put("VLupdatedon", "" + dbHelper.getMaxVatLevel());
		parameters.put("offerUpdatedOn", "" + dbHelper.getMaxOffersUpdatedOn());
		parameters.put("offerDetailUpdatedOn", "" + dbHelper.getMaxOfferDetailsUpdatedOn());

		parameters.put("documentStatusUpdatedOn", "" + dbHelper.getMaxDocumentStatusUpdatedOn());
		parameters.put("storesUpdatedOn", "" + dbHelper.getMaxStoresUpdatedOn());
		parameters.put("linkedItemsUpdatedOn", "" + dbHelper.getMaxLinkedItem());
		parameters.put("mmUpdatedOn", "" + dbHelper.getMaxMeasurementUnitUpdatedOn());
		parameters.put("documentTypeUpdatedOn", "" + dbHelper.getMaxDocumentTypesUpdatedOn());

		parameters.put("userID", arg0[0].getRemoteGuid());
		Log.d("ws params", parameters.toString());
		int oldTimeout = TIMEOUT;

		TIMEOUT = 200000;
		progDialogMessage = "Contacting update server...";
		publishProgress(0);
		int triesLeft = 5;
		Object filenameobj = null;

		while (triesLeft != 0 && filenameobj == null)
		{
		    Log.i("Syncrhonize Text", "GetTotalUpdatesLink tries left: "+triesLeft);
		    filenameobj = SynchronousCallWebMethod("GetTotalUpdatesLink", parameters);
		    triesLeft--;
		}

		TIMEOUT = oldTimeout;
		if (filenameobj == null)
		{
		    Log.i("SynchronizerText", "Filename problem");
		    return false;
		}
		String filename = filenameobj.toString();

		url = url.toLowerCase().replace("retailservice.asmx", "Downloads/" + filename);
		String outputFile = Environment.getExternalStorageDirectory().toString() + "/retail/temp.zip";
		progDialogMessage = "Download update file...";
		publishProgress(0);
		Log.i("Syncrhonize Text", "Download start");
		if (downloadFile(url, outputFile))
		{
		    Log.i("Syncrhonize Text", "Download end");

		    parameters = new HashMap<String, Object>();
		    parameters.put("userID", arg0[0].getRemoteGuid());
		    parameters.put("filename", filename);
		    SynchronousCallWebMethod("CleanFile", parameters);

		    // UNZIP the file
		    Log.i("Syncrhonize Text", "Unpack start");
		    unpackZip(Environment.getExternalStorageDirectory().toString() + "/retail/", "temp.zip");
		    Log.i("Syncrhonize Text", "Unpack end");
		    // Parse file
		    Log.i("Syncrhonize Text", "Processing start");
		    try
		    {
			progDialogMessage = "Applying update file...";
			publishProgress(0);
			dbHelper.getWritableDatabase().execSQL("PRAGMA synchronous=1  ;");

			if (!parseFile(outputFile.replace("temp.zip", "Database.txt")))
			    return false;

		    }
		    catch (Exception ex)
		    {
			ex.printStackTrace();
			return false;
		    }
		    Log.i("Syncrhonize Text", "Processing end");
		}
		else
		{
		    return false;
		}

		oldTimeout = TIMEOUT;

		progDialogMessage = "Fetching next result set...";
		publishProgress(0);

		TIMEOUT = 100000;
		int rowsRemaining = -1;
		triesLeft = 5;
		while (rowsRemaining == -1 && triesLeft != 0)
		{
		    Log.i("Syncrhonize Text", "GetTotalUpdates tries left: "+triesLeft);
		    rowsRemaining = getTotalUpdates(dbHelper.getMaxCustomerUpdatedOn(), dbHelper.getMaxItemUpdatedOn(), dbHelper.getMaxItemAnalyticTreesUpdatedOn(), dbHelper.getMaxBarcodeUpdatedOn(), dbHelper.getMaxItemCategoriesUpdatedOn(), dbHelper.getMaxPriceCatalog(), dbHelper.getMaxPriceCatalogDetail(), dbHelper.getMaxVatCategory(), dbHelper.getMaxVatFactor(), dbHelper.getMaxVatLevel(), dbHelper.getMaxOffersUpdatedOn(), dbHelper.getMaxOfferDetailsUpdatedOn(), dbHelper.getMaxDocumentStatusUpdatedOn(), dbHelper.getMaxStoresUpdatedOn(), dbHelper.getMaxLinkedItem(), dbHelper.getMaxMeasurementUnitUpdatedOn(), dbHelper.getMaxDocumentTypesUpdatedOn(),arg0[0]);
		    triesLeft--;
		}

		TIMEOUT = oldTimeout;

		if (rowsRemaining == -1)
		    return false;

		// logo tis apoklisis,an 2 fores stin seira apomenoun ta idia
		// rows tote to update exei teliosei
		isUpdateOver = (rowsRemaining == previousRowsRemaining || rowsRemaining == 0);


		previousRowsRemaining = rowsRemaining;

		Log.i("Syncrhonize Text", "File loop end. Rows remaining: " + rowsRemaining);
	    }
	    
	    SynchronizeUserStoreAccessOfUser(arg0[0]);
	    
	    progDialogMessage = "Optimizing database for faster performance...";
	    // Queries to restore fks
	    String[] queries = new String[] { 
		    "update customer set pc_id =(select id from pricecatalog where pricecatalog.remoteGuid = customer.pcRemoteGuid),"
		    + " vl_id = (select id from Vatlevel where vatlevel.remoteGuid = customer.vatlevelremoteGuid ), "
		    + "store_id = (select id from store where store.remoteGuid = customer.storeremoteGuid ) "
		    + "where store_id is null "
		    + "or store_id <> (select id from store where store.remoteGuid = customer.storeremoteGuid ) "
		    + "or vl_id is null "
		    + "or vl_id <> (select id from Vatlevel where vatlevel.remoteGuid = customer.vatlevelremoteGuid) "
		    + "or pc_id is null "
		    + "or pc_id <> (select id from pricecatalog where pricecatalog.remoteGuid = customer.pcRemoteGuid) "
		    + "or updatedon >= " + originalCustomerUpdatedOn,
		    
		    "update item set vatcategory_id = (select id from Vatcategory where Vatcategory.remoteGuid = Item.VatcategoryRemoteGuid), "
		    + "defaultBarcodeObject_id = (select id from Barcode where Barcode.remoteGuid = Item.defaultBarcodeRemoteGuid) "
		    + "where item.vatcategory_id is null "
		    + "or item.vatcategory_id <> (select id from Vatcategory where Vatcategory.remoteGuid = Item.VatcategoryRemoteGuid) "
		    + "or item.defaultBarcodeObject_id is null "
		    + "or item.defaultBarcodeObject_id <> (select id from Barcode where Barcode.remoteGuid = Item.defaultBarcodeRemoteGuid) "
		    + "or updatedon >= " + originalItemUpdatedOn,
		    
		    "update barcode set item_id= (select id from Item where item.remoteGuid = barcode.itemRemoteGuid), "
		    + "MeasurementUnit_id= (select id from MeasurementUnit where MeasurementUnit.remoteGuid = barcode.measurementUnitRemoteGuid) "
		    + "where item_id is null "
		    + "or MeasurementUnit_id is null "
		    + "or item_id <> (select id from item where item.remoteGuid = barcode.itemRemoteGuid) "
		    + "or MeasurementUnit_id <> (select id from MeasurementUnit where MeasurementUnit.remoteGuid = barcode.measurementUnitRemoteGuid) "
		    + "or updatedon >= " + originalBarcodeUpdatedOn,
		    
		    "update itemanalytictree set item_id = (select id from item where item.remoteGuid = itemanalytictree .itemRemoteGuid), "
		    + "itemcategory_id = (select id from itemcategory where itemcategory.remoteguid = itemanalytictree .itemcategoryRemoteGuid) "
		    + "where item_id is null "
		    + "or item_id <> (select id from item where item.remoteGuid = itemanalytictree .itemRemoteGuid) "
		    + "or itemcategory_id is null "
		    + "or itemcategory_id <> (select id from itemcategory where itemcategory.remoteguid = itemanalytictree .itemcategoryRemoteGuid) "
		    + "or updatedon >= " + originalIatUpdatedOn,
		    
		    "update itemcategory set parent_id = (select id from itemcategory a where a.remoteGuid = itemcategory.remoteparentguid)",
		    
		    "update pricecatalog set parent_id = (select id from pricecatalog a where a.remoteGuid = pricecatalog .remoteparentguid)",
		    
		    "update pricecatalogdetail set pc_id = (select id from pricecatalog pc where pc.remoteGuid = pricecatalogdetail.pcremoteguid), "
		    + "bc_id = (select id from barcode bc where bc.remoteGuid = pricecatalogdetail.barcoderemoteguid), "
		    + "item_id = (select id from item  where item.remoteGuid = pricecatalogdetail.itemremoteguid) "
		    + "where item_id is null "
		    + "or item_id <> (select id from item  where item.remoteGuid = pricecatalogdetail.itemremoteguid) "
		    + "or bc_id is null "
		    + "or bc_id <> (select id from barcode bc where bc.remoteGuid = pricecatalogdetail.barcoderemoteguid) "
		    + "or pc_id is null "
		    + "or pc_id <> (select id from pricecatalog pc where pc.remoteGuid = pricecatalogdetail.pcremoteguid) "
		    + "or updatedon >=" + originalPCDUpdatedOn, 
		    
		    "update vatfactor set vatlevel_id = (select id from vatlevel where vatlevel.remoteguid = vatfactor.vatlevelremoteguid), vatcategory_id = (select id from vatcategory where vatcategory.remoteguid = vatfactor.vatcategoryremoteguid)",
		    
		    "update offer set PriceCatalog_id = (select id from pricecatalog where offer.PriceCatalogremoteguid = pricecatalog.remoteguid)",
		    
		    "update offerdetail set item_id = (select id from item where item.remoteGuid = offerdetail.itemRemoteGuid), offer_id = (select id from offer where offer.remoteGuid = offerdetail.OfferRemoteGuid)",
		    
		    "update address set customer_id = (select id from customer where customer.remoteGuid=address.customerRemoteGuid)" };

	    progDialog.setMax(queries.length + 1);
	    publishProgress(0);
	    int i = 0;
	    DatabaseConnection conn;
	    try
	    {
		conn = dbHelper.getConnectionSource().getReadWriteConnection();

		conn.setAutoCommit(false);
		for (String query : queries)
		{
		    Log.i("Sync", "Executing:" + query);
		    dbHelper.getWritableDatabase().compileStatement(query).executeUpdateDelete();
		    publishProgress((++i));
		}

		TIMEOUT = 100000;
		Log.i("Syncrhonize Inactive", "Start");

		conn.setAutoCommit(true);

		progDialogMessage = "Deleting Inactive objects...";
		progDialog.setMax(originalUpdatedOns.size() + 1);
		publishProgress(0);
		int j = 0;
		// TODO ta address eksartountai apo tous customers
		for (Entry<Class, Long> entry : originalUpdatedOns.entrySet())
		{
		    if (entry.getValue() > 0)
		    {
			String fixedClassName = entry.getKey().getSimpleName().replace("VAT", "Vat");//.replace("MeasurementUnit", "MesurmentUnits");

			Log.i("Syncrhonize Inactive: " + entry.getKey().getSimpleName(), "Start");
			SynchronizeActiveInactive(dbHelper.getDao(entry.getKey()), fixedClassName, arg0[0], entry.getValue());
			Log.i("Syncrhonize Inactive: " + entry.getKey().getSimpleName(), "End");
			publishProgress((++j));
		    }
		}

		// if (originalDocumentStatusUpdatedOn > 0)
		// {
		// SynchronizeActiveInactive(dbHelper.getDocumentStatuses(),
		// "DocumentStatus", arg0[0], originalDocumentStatusUpdatedOn);
		// }
		// if (originalItemUpdatedOn > 0)
		// {
		// SynchronizeActiveInactive(dbHelper.getItems(), "Item",
		// arg0[0], originalItemUpdatedOn);
		// }
		// if (originalBarcodeUpdatedOn > 0)
		// {
		// SynchronizeActiveInactive(dbHelper.getBarcodes(), "Barcode",
		// arg0[0], originalBarcodeUpdatedOn);
		// }
		//
		// if (originalPCDUpdatedOn > 0)
		// {
		// SynchronizeActiveInactive(dbHelper.getPriceCatalogDetailDao(),
		// "PriceCatalogDetail", arg0[0], originalPCDUpdatedOn);
		// }
		//
		// if (originalCustomerUpdatedOn > 0)
		// {
		// SynchronizeActiveInactive(dbHelper.getCustomers(),
		// "Customer", arg0[0], originalCustomerUpdatedOn);
		// }
		// TODO ta address eksartountai apo tous customers
		// SynchronizeActiveInactive(dbHelper.getAddresstDao(),"Address",
		// arg0[0]);

		Log.i("Syncrhonize Inactive", "End");

		// restoring database status
		dbHelper.getWritableDatabase().execSQL("PRAGMA synchronous=2  ;");
		// SynchronizeItemImages(originalItemUpdatedOn, arg0[0]);

		Log.i("Syncrhonize Text", "End");
		((MainActivity) mContext).getPreferences(mContext.MODE_PRIVATE).edit().putBoolean("init", true).commit();
		((MainActivity) mContext).isDBInitialized = true;
		int oldTimeout = TIMEOUT;

		TIMEOUT = oldTimeout;

		return true;

	    }
	    catch (SQLException e)
	    {
		e.printStackTrace();
		return false;
	    }

	}

	private boolean SynchronizeItemImages(long iuo, User usr)
	{
	    progDialogMessage = "Preparing request to update images from server ...";
	    HashSet<String> toRequest = new HashSet<String>();
	    publishProgress(0);
	    try
	    {
		// List<Item> allItems = dbHelper.getItems().queryForAll();

		for (Item itm : dbHelper.getItems())
		{
		    if (itm.getImageOid() == null)
			itm = dbHelper.getItems().queryForId(itm.getID());
		    String filename = MainActivity.IMAGES_PATH + "32/img_" + itm.getImageOid().replace("-", "").toLowerCase();
		    if (itm.getUpdatedOn() >= iuo)
			toRequest.add(itm.getImageOid().replace("-", "").toLowerCase());
		    else
		    {
			File file = new File(filename);
			if (!file.exists())
			    toRequest.add(itm.getImageOid().replace("-", "").toLowerCase());
		    }
		}
	    }
	    catch (Exception ex)
	    {
		Log.w("SynchronizeItemImages", "error in database");
		return false;
	    }

	    // //////
	    StringArraySerializer toSend = new StringArraySerializer();
	    toSend.addAll(toRequest);
	    // String [] toSend = toRequest.toArray(new String[0]);
	    HashMap parameters = new HashMap();

	    parameters.put("userID", usr.getRemoteGuid());
	    parameters.put("imageGuids", toSend);

	    String url = ((MainActivity) mContext).appsettings.getServiceUrl();
	    int oldTimeout = TIMEOUT;
	    TIMEOUT = 100000;
	    progDialogMessage = "Requesting images update from server ...";
	    publishProgress(0);
	    Object filenameobj = SynchronousCallWebMethod("GetImagesLink", parameters);
	    TIMEOUT = oldTimeout;
	    if (filenameobj == null)
	    {
		Log.i("SynchronizerText", "Filename problem");
		return false;
	    }
	    String filename = filenameobj.toString();

	    url = url.toLowerCase().replace("retailservice.asmx", "Downloads/" + filename);
	    String outputFile = Environment.getExternalStorageDirectory().toString() + "/retail/images/imgs.zip";
	    progDialogMessage = "Download update file...";
	    publishProgress(0);
	    if (downloadFile(url, outputFile))
	    {
		// MainActivity.IMAGES_PATH
		unpackZip(Environment.getExternalStorageDirectory().toString() + "/retail/images/", "imgs.zip");
		parameters = new HashMap<String, Object>();
		parameters.put("userID", usr.getRemoteGuid());
		parameters.put("filename", filename);
		SynchronousCallWebMethod("CleanFile", parameters);

		return true;
	    }
	    return false;
	}

	private boolean downloadFile(String url, String outputFile)
	{
	    try
	    {
		progDialogMessage = "Downloading File";
		URL u = new URL(url);
		URLConnection conn = u.openConnection();
		int contentLength = conn.getContentLength(), current = 0;

		progDialog.setMax(contentLength);
		publishProgress(current);

		File file = new File(outputFile.substring(0, outputFile.lastIndexOf('/')));
		file.mkdirs();

		DataInputStream stream = new DataInputStream(u.openStream());
		DataOutputStream fos = new DataOutputStream(new FileOutputStream(outputFile));

		byte[] buffer = new byte[32768];
		int len, ct = 0;
		while ((len = stream.read(buffer)) != -1)
		{
		    fos.write(buffer, 0, len);
		    fos.flush();
		    current += len;
		    ct++;
		    if (ct % 20 == 0)
			publishProgress(current);
		}
		// stream.readFully(buffer);
		stream.close();

		// fos.write(buffer);
		// fos.flush();
		fos.close();
		return true;
	    }
	    catch (FileNotFoundException e)
	    {
		return false; // swallow a 404
	    }
	    catch (IOException e)
	    {
		return false; // swallow a 404
	    }
	}

	private boolean SynchronizeUserStoreAccessOfUser(User user)
	{
	    try
	    {
		DatabaseConnection conn = dbHelper.getConnectionSource().getReadWriteConnection();
		HashMap<String, Object> params;
		params = new HashMap<String, Object>();
		params.put("userID", user.getRemoteGuid());
		Object result = callWebMethod("getUserStoreAccess", params);
		SoapObject sresult = (SoapObject) result;
		int limit = sresult == null ? 0 : sresult.getPropertyCount();
		for (int i = 0; i < limit; i++)
		{
		    SoapObject obj = (SoapObject) sresult.getProperty(i);
		    UserStoreAccess ct;
		    Store store = dbHelper.findObject(Store.class, "remoteGuid", obj.getProperty("StoreID"), FilterType.EQUALS);
		    if (store != null)
		    {
			ct = dbHelper.findObject(UserStoreAccess.class, OperatorType.AND, new BinaryOperator("store_id", store.getID(), FilterType.EQUALS), new BinaryOperator("user_id", user.getID(), FilterType.EQUALS));
			boolean newct;
			if (ct == null)
			{
			    newct = true;
			    ct = new UserStoreAccess();
			}
			else
			{
			    newct = false;
			}
			ct.setUpdatedOn(Long.parseLong(obj.getProperty("UpdatedOn").toString()));
			ct.setStore(dbHelper.getStoreByRemoteGuid(obj.getProperty("StoreID").toString()));
			ct.setUser(dbHelper.getUserByRemoteGuid(obj.getProperty("UserID").toString()));

			if (newct)
			    dbHelper.getUserStoreAccessDao().create(ct);
			else
			    dbHelper.getUserStoreAccessDao().update(ct);
			if (i % COMMIT_SIZE == 0)
			{
			    conn.setAutoCommit(true);
			    conn.setAutoCommit(false);
			}
		    }

		}
		Log.i("Sync UserStoreAccesses", limit + " completed successfully");
		conn.setAutoCommit(true);
		return true;
	    }
	    catch (Exception ex)
	    {
		ex.printStackTrace();
		return false;
	    }
	}

	private boolean unpackZip(String path, String zipname)
	{
	    InputStream is;
	    ZipInputStream zis;
	    try
	    {
		String filename;
		is = new FileInputStream(path + zipname);
		zis = new ZipInputStream(new BufferedInputStream(is));
		ZipEntry ze;
		byte[] buffer = new byte[32768];
		int count;

		while ((ze = zis.getNextEntry()) != null)
		{

		    filename = ze.getName();
		    String totalpath = path + filename;
		    totalpath = totalpath.substring(0, totalpath.lastIndexOf('/'));
		    File ffout = new File(totalpath);
		    ffout.mkdirs();
		    FileOutputStream fout = new FileOutputStream(path + filename);
		    progDialogMessage = "Unpacking...";
		    progDialog.setMax((int) ze.getSize());
		    int total = 0, ct = 0;
		    publishProgress(total);

		    while ((count = zis.read(buffer)) != -1)
		    {
			fout.write(buffer, 0, count);
			// fout.flush();
			total += count;
			ct++;
			if (ct % 400 == 0)
			    publishProgress(total);
		    }

		    fout.close();
		    zis.closeEntry();
		}

		zis.close();
	    }
	    catch (IOException e)
	    {
		e.printStackTrace();
		return false;
	    }

	    return true;
	}

	private String progDialogMessage = "";

	@Override
	protected void onPreExecute()
	{
	    super.onPreExecute();
	    progDialog = new ProgressDialog(mContext);
	    progDialog.setProgressStyle(ProgressDialog.STYLE_HORIZONTAL);
	    progDialog.setMessage(progDialogMessage);
	    progDialog.setCancelable(false);
	    ((MainActivity) mContext).getWindow().addFlags(WindowManager.LayoutParams.FLAG_KEEP_SCREEN_ON);
	}
    }

    //
    // class SynchronizerFromMultiTextWithTempTables extends AsyncTask<User,
    // Integer, Object>
    // {
    //
    // String errorMessage;
    //
    // private boolean SynchronizeApplicationSettings(User user)
    // {
    // try
    // {
    // // VATCategory
    // HashMap<String, Object> params = new HashMap<String, Object>();
    //
    // params.put("userID", user.getRemoteGuid());
    // Object result = callWebMethod("getAppSettings", params);
    // SoapObject sresult = (SoapObject) result;
    // // int limit = sresult.getPropertyCount();
    // // for (int i = 0; i < limit; i++)
    // {
    // SoapObject obj = sresult;
    // ApplicationSettings ct;
    // ct = dbHelper.getApplicationSettings().queryForAll().get(0);
    // ct.setBarcodePadChar(obj.getPropertyAsString("BarcodePadChar").charAt(0));
    // ct.setCodePadChar(obj.getPropertyAsString("CodePadChar").charAt(0));
    //
    // ct.setBarcodePadding(Boolean.parseBoolean(obj.getPropertyAsString("BarcodePad")));
    // ct.setCodePadding(Boolean.parseBoolean(obj.getPropertyAsString("CodePad")));
    //
    // ct.setBarcodePadLength(Integer.parseInt(obj.getPropertyAsString("BarcodePadLength")));
    // ct.setCodePadLength(Integer.parseInt(obj.getPropertyAsString("CodePadLength")));
    //
    // ct.setComputeDigits(Integer.parseInt(obj.getPropertyAsString("ComputeDigits")));
    // ct.setComputeValueDigits(Integer.parseInt(obj.getPropertyAsString("ComputeValueDigits")));
    //
    // ct.setDisplayDigits(Integer.parseInt(obj.getPropertyAsString("DisplayDigits")));
    // ct.setDisplayValueDigits(Integer.parseInt(obj.getPropertyAsString("DisplayValueDigits")));
    //
    // dbHelper.getApplicationSettings().update(ct);
    // ((MainActivity) mContext).appsettings = ct;
    //
    // }
    // return true;
    // }
    // catch (Exception ex)
    // {
    // ex.printStackTrace();
    // }
    // return false;
    // }
    //
    // @Override
    // protected void onPostExecute(Object result)
    // {
    // // TODO Auto-generated method stub
    // super.onPostExecute(result);
    // ((MainActivity)
    // mContext).getWindow().clearFlags(WindowManager.LayoutParams.FLAG_KEEP_SCREEN_ON);
    //
    // progDialog.dismiss();
    // if (result == null || (Boolean) result == true)
    // {
    // ((MainActivity) mContext).btnOrders.setEnabled(true);
    // ((MainActivity) mContext).foundCustomers.clear();
    // ((MainActivity) mContext).UpdateCustomers(false);
    // ((MainActivity) mContext).UpdateUI();
    // ((MainActivity) mContext).isDBInitialized = true;
    // Utilities.ShowUninterruptingMessage(mContext,
    // mContext.getString(R.string.updateCompletedSuccessfully), 5);
    // }
    // else
    // {
    // Utilities.ShowUninterruptingMessage(mContext,
    // mContext.getString(R.string.problem) + " :" + errorMessage, 15);
    // }
    //
    // }
    //
    // @Override
    // protected void onProgressUpdate(Integer... values)
    // {
    // // TODO Auto-generated method stub
    // super.onProgressUpdate(values);
    // progDialog.setMessage(progDialogMessage);
    // if (!progDialog.isShowing())
    // {
    // progDialog.show();
    // }
    // progDialog.setProgress(values[0]);
    // }
    //
    // private ProgressDialog progDialog;
    // private String[] stores = null, documentstatus = null, customers = null,
    // items = null, itemATs = null, barcodes = null, itemcategory = null,
    // pricecatalog = null,
    // pricecatalogdetail = null, addresses = null;
    // private SQLiteStatement customerInsert = null, customerUpdate = null,
    // itemInsert = null, itemUpdate = null, barcodeInsert = null, barcodeUpdate
    // = null,
    // addressInsert = null, addressUpdate = null, addressSelect = null;
    // private SQLiteStatement itemAnalyticTreeInsert, itemAnalyticTreeUpdate,
    // itemCategoryInsert;
    // private SQLiteStatement itemCategoryUpdate, pc_in, pc_up, pcd_in,
    // mesurmentUnitsInsert, mesurmentUnitsUpdate;
    // private SQLiteStatement pcd_up, documentStatusInsert,
    // documentStatusUpdate;
    // private SQLiteStatement storeInsert, storeUpdate, linkedItemsInsert,
    // linkedItemsUpdate;
    // private String[] linkedItems = null, mesurmentUnits = null;
    // private String[] vatCategories;
    // private SQLiteStatement vcInsert;
    // private SQLiteStatement vcUpdate;
    // private String[] vatLevels;
    // private SQLiteStatement vlInsert;
    // private SQLiteStatement vlUpdate;
    // private String[] vatFactors;
    // private SQLiteStatement vfInsert;
    // private SQLiteStatement vfUpdate;
    // private String[] offers;
    // private SQLiteStatement offerInsert;
    // private SQLiteStatement offerUpdate;
    // private SQLiteStatement offerDetailInsert;
    // private String[] offerDetails;
    // private SQLiteStatement offerDetailrUpdate;
    //
    // @SuppressLint("DefaultLocale")
    // protected boolean parseLine(String line) throws Exception
    // {
    // String[] elements = TextUtils.split(line, "\t&\t");
    // // line.split("\t&\t");
    // String query;
    // if (elements[0].equals("DocumentStatus"))
    // {
    // if (documentstatus == null || elements[1].equals("Oid"))
    // {
    // documentstatus = elements;
    // Log.i("Parsing", "documentstatus start");
    // }
    // else
    // {
    //
    // DocumentStatus ct = dbHelper.getDocumentStatusByRemoteGuid(elements[1]);
    // if (ct == null)
    // {
    // if (documentStatusInsert == null)
    // {
    // // Insert
    // Log.i("Parse Line", "Compiling documentStatusInsert");
    // query =
    // "insert into DocumentStatus_Temp(remoteGuid,Description,IsDefault,UpdatedOn) ";
    // query += "values(?,?,?,?)";
    // documentStatusInsert =
    // dbHelper.getWritableDatabase().compileStatement(query);
    // }
    // for (int i = 1; i < 5; i++)
    // documentStatusInsert.bindString(i, elements[i]);
    // documentStatusInsert.executeInsert();
    //
    // }
    // else
    // {
    // if (documentStatusUpdate == null)
    // {
    // Log.i("Parse Line", "Compiling documentStatusUpdate");
    // query =
    // "update DocumentStatus set Description=?,IsDefault=?,UpdatedOn=? where remoteGuid = ?";
    // documentStatusUpdate =
    // dbHelper.getWritableDatabase().compileStatement(query);
    // }
    // for (int i = 2; i < 5; i++)
    // {
    // documentStatusUpdate.bindString(i - 1, elements[i]);
    // }
    // documentStatusUpdate.bindString(4, elements[1]);
    // documentStatusUpdate.executeUpdateDelete();
    // }
    //
    // docstatusupdatedon = Long.parseLong(elements[4]);
    //
    // }
    // }
    // else if (elements[0].equals("Store"))
    // {
    // if (stores == null || elements[1].equals("Oid"))
    // {
    // stores = elements;
    // Log.i("Parsing", "Store start");
    // }
    // else
    // {
    //
    // Store ct = dbHelper.getStoreByRemoteGuid(elements[1]);
    // if (ct == null)
    // {
    // if (storeInsert == null)
    // {
    // // Insert
    // Log.i("Parse Line", "Compiling storeInsert");
    // query =
    // "insert into Store_Temp(remoteGuid,Code,Name,IsCentralStore,UpdatedOn) ";
    // query += "values(?,?,?,?,?)";
    // storeInsert = dbHelper.getWritableDatabase().compileStatement(query);
    // }
    // for (int i = 1; i < 6; i++)
    // storeInsert.bindString(i, elements[i]);
    // storeInsert.executeInsert();
    //
    // }
    // else
    // {
    //
    // if (storeUpdate == null)
    // {
    // Log.i("Parse Line", "Compiling storeUpdate");
    // query =
    // "update Store set Code=?,Name=?,IsCentralStore=?,UpdatedOn=? where remoteGuid = ?";
    // storeUpdate = dbHelper.getWritableDatabase().compileStatement(query);
    // }
    // for (int i = 2; i < 6; i++)
    // {
    // storeUpdate.bindString(i - 1, elements[i]);
    // }
    // storeUpdate.bindString(5, elements[1]);
    // storeUpdate.executeUpdateDelete();
    // }
    //
    // storesupdatedon = Long.parseLong(elements[5]);
    // }
    // }
    // else if (elements[0].equals("LinkedItem"))
    // {
    // if (linkedItems == null || elements[1].equals("Oid"))
    // {
    // linkedItems = elements;
    // Log.i("Parsing", "LinkedItem start");
    // }
    // else
    // {
    //
    // LinkedItem ct = dbHelper.getLinkedItemByRemoteGuid(elements[1]);
    // if (ct == null)
    // {
    // if (linkedItemsInsert == null)
    // {
    // // Insert
    // Log.i("Parse Line", "Compiling linkedItemsInsert");
    // query =
    // "insert into LinkedItem_Temp(remoteGuid,itemGuid,subItemGuid,UpdatedOn) ";
    // query += "values(?,?,?,?)";
    // linkedItemsInsert =
    // dbHelper.getWritableDatabase().compileStatement(query);
    // }
    // for (int i = 1; i < 5; i++)
    // linkedItemsInsert.bindString(i, elements[i]);
    // linkedItemsInsert.executeInsert();
    //
    // }
    // else
    // {
    //
    // if (linkedItemsUpdate == null)
    // {
    // Log.i("Parse Line", "Compiling linkedItemsUpdate");
    // query =
    // "update LinkedItem set itemGuid=?,subItemGuid=?,UpdatedOn=? where remoteGuid = ?";
    // linkedItemsUpdate =
    // dbHelper.getWritableDatabase().compileStatement(query);
    // }
    // for (int i = 2; i < 5; i++)
    // {
    // linkedItemsUpdate.bindString(i - 1, elements[i]);
    // }
    // linkedItemsUpdate.bindString(4, elements[1]);
    // linkedItemsUpdate.executeUpdateDelete();
    // }
    //
    // linkeditemupdatedon = Long.parseLong(elements[4]);
    // }
    // }
    // else if (elements[0].equals("MesurmentUnits"))
    // {
    // if (mesurmentUnits == null || elements[1].equals("Oid"))
    // {
    // mesurmentUnits = elements;
    // Log.i("Parsing", "mesurmentUnits start");
    // }
    // else
    // {
    //
    // MeasurementUnit ct =
    // dbHelper.getMeasurementUnitByRemoteGuid(elements[1]);
    // if (ct == null)
    // {
    // if (mesurmentUnitsInsert == null)
    // {
    // // Insert
    // Log.i("Parse Line", "Compiling mesurmentUnitsInsert");
    // query =
    // "insert into MeasurementUnit_Temp(remoteGuid,Description,supportDecimals,UpdatedOn) ";
    // query += "values(?,?,?,?)";
    // mesurmentUnitsInsert =
    // dbHelper.getWritableDatabase().compileStatement(query);
    // }
    // for (int i = 1; i < 5; i++)
    // mesurmentUnitsInsert.bindString(i, elements[i]);
    // mesurmentUnitsInsert.executeInsert();
    //
    // }
    // else
    // {
    //
    // if (mesurmentUnitsUpdate == null)
    // {
    // Log.i("Parse Line", "Compiling mesurmentUnitsUpdate");
    // query =
    // "update MeasurementUnit set Description=?,supportDecimals=?,UpdatedOn=? where remoteGuid = ?";
    // mesurmentUnitsUpdate =
    // dbHelper.getWritableDatabase().compileStatement(query);
    // }
    // for (int i = 2; i < 5; i++)
    // {
    // mesurmentUnitsUpdate.bindString(i - 1, elements[i]);
    // }
    // mesurmentUnitsUpdate.bindString(4, elements[1]);
    // mesurmentUnitsUpdate.executeUpdateDelete();
    // }
    //
    // measurementunitupdatedon = Long.parseLong(elements[4]);
    // }
    // }
    // else if (elements[0].equals("VatCategory"))
    // {
    // if (vatCategories == null || elements[1].equals("Oid"))
    // {
    // vatCategories = elements;
    // Log.i("Parsing", "vatCategories start");
    // }
    // else
    // {
    //
    // VATCategory ct = dbHelper.getVatCategoryByRemoteGuid(elements[1]);
    // if (ct == null)
    // {
    // if (vcInsert == null)
    // {
    // // Insert
    // Log.i("Parse Line", "Compiling vcInsert");
    // query = "insert into VATCategory_Temp(remoteGuid,name,UpdatedOn) ";
    // query += "values(?,?,?)";
    // vcInsert = dbHelper.getWritableDatabase().compileStatement(query);
    // }
    // for (int i = 1; i < 4; i++)
    // vcInsert.bindString(i, elements[i]);
    // vcInsert.executeInsert();
    //
    // }
    // else
    // {
    //
    // if (vcUpdate == null)
    // {
    // Log.i("Parse Line", "Compiling vcUpdate");
    // query = "update VATCategory set name=?,UpdatedOn=? where remoteGuid = ?";
    // vcUpdate = dbHelper.getWritableDatabase().compileStatement(query);
    // }
    // for (int i = 2; i < 4; i++)
    // {
    // vcUpdate.bindString(i - 1, elements[i]);
    // }
    // vcUpdate.bindString(3, elements[1]);
    // vcUpdate.executeUpdateDelete();
    // }
    //
    // vcupdatedon = Long.parseLong(elements[3]);
    // }
    // }
    // else if (elements[0].equals("VatLevel"))
    // {
    // if (vatLevels == null || elements[1].equals("Oid"))
    // {
    // vatLevels = elements;
    // Log.i("Parsing", "vatLevels start");
    // }
    // else
    // {
    //
    // VATLevel ct = dbHelper.getVatLevelByRemoteGuid(elements[1]);
    // if (ct == null)
    // {
    // if (vlInsert == null)
    // {
    // // Insert
    // Log.i("Parse Line", "Compiling vlInsert");
    // query =
    // "insert into VatLevel_Temp(remoteGuid,name,'Default',UpdatedOn) ";
    // query += "values(?,?,?,?)";
    // vlInsert = dbHelper.getWritableDatabase().compileStatement(query);
    // }
    // for (int i = 1; i < 5; i++)
    // vlInsert.bindString(i, elements[i]);
    // vlInsert.executeInsert();
    //
    // }
    // else
    // {
    //
    // if (vlUpdate == null)
    // {
    // Log.i("Parse Line", "Compiling vlUpdate");
    // query =
    // "update VatLevel set name=?,'Default'=?,UpdatedOn=? where remoteGuid = ?";
    // vlUpdate = dbHelper.getWritableDatabase().compileStatement(query);
    // }
    // for (int i = 2; i < 5; i++)
    // {
    // vlUpdate.bindString(i - 1, elements[i]);
    // }
    // vlUpdate.bindString(4, elements[1]);
    // vlUpdate.executeUpdateDelete();
    // }
    // vlupdatedon = Long.parseLong(elements[4]);
    // }
    // }
    // else if (elements[0].equals("VatFactor"))
    // {
    // if (vatFactors == null || elements[1].equals("Oid"))
    // {
    // vatFactors = elements;
    // Log.i("Parsing", "vatFactors start");
    // }
    // else
    // {
    //
    // VATFactor ct = dbHelper.getVatFactorByRemoteGuid(elements[1]);
    // if (ct == null)
    // {
    // if (vfInsert == null)
    // {
    // // Insert
    // Log.i("Parse Line", "Compiling vfInsert");
    // query =
    // "insert into VatFactor_Temp(remoteGuid,vatCategoryRemoteGuid,vatLevelRemoteGuid,vatFactor,UpdatedOn) ";
    // query += "values(?,?,?,?,?)";
    // vfInsert = dbHelper.getWritableDatabase().compileStatement(query);
    // }
    // for (int i = 1; i < 6; i++)
    // vfInsert.bindString(i, elements[i].replace(",", "."));
    // vfInsert.executeInsert();
    //
    // }
    // else
    // {
    //
    // if (vfUpdate == null)
    // {
    // Log.i("Parse Line", "Compiling vfUpdate");
    // query =
    // "update VatFactor set vatCategoryRemoteGuid=?,vatLevelRemoteGuid=?,vatFactor=?,UpdatedOn=? where remoteGuid = ?";
    // vfUpdate = dbHelper.getWritableDatabase().compileStatement(query);
    // }
    // for (int i = 2; i < 6; i++)
    // {
    // vfUpdate.bindString(i - 1, elements[i].replace(",", "."));
    // }
    // vfUpdate.bindString(5, elements[1]);
    // vfUpdate.executeUpdateDelete();
    // }
    //
    // vfupdatedon = Long.parseLong(elements[5]);
    // }
    // }
    // else if (elements[0].equals("Offer"))
    // {
    // if (offers == null || elements[1].equals("Oid"))
    // {
    // offers = elements;
    // Log.i("Parsing", "offers start");
    // }
    // else
    // {
    //
    // Offer ct = dbHelper.getOfferByRemoteGuid(elements[1]);
    // if (ct == null)
    // {
    // if (offerInsert == null)
    // {
    // // Insert
    // // "Oid" + DELIMITER + "PriceCatalogOid" +
    // // "Description" + "Description2" + "isActive" +
    // // "StartDate" + "EndDate" + "UpdatedOn");
    // Log.i("Parse Line", "Compiling offerInsert");
    // query =
    // "insert into Offer_Temp(RemoteGuid,PriceCatalogRemoteGuid,Description,Description2,Active,StartDate,EndDate,UpdatedOn) ";
    // query += "values(?,?,?,?,?,?,?,?)";
    // offerInsert = dbHelper.getWritableDatabase().compileStatement(query);
    // }
    // for (int i = 1; i < 9; i++)
    // offerInsert.bindString(i, elements[i]);
    // offerInsert.executeInsert();
    //
    // }
    // else
    // {
    //
    // if (offerUpdate == null)
    // {
    // Log.i("Parse Line", "Compiling offerUpdate");
    // query =
    // "update Offer set PriceCatalogRemoteGuid=?,Description=?,Description2=?,Active=?,StartDate=?,EndDate=?,UpdatedOn=? where remoteGuid = ?";
    // offerUpdate = dbHelper.getWritableDatabase().compileStatement(query);
    // }
    // for (int i = 2; i < 9; i++)
    // {
    // offerUpdate.bindString(i - 1, elements[i]);
    // }
    // offerUpdate.bindString(8, elements[1]);
    // offerUpdate.executeUpdateDelete();
    // }
    //
    // offerupdatedon = Long.parseLong(elements[8]);
    // }
    // }
    // else if (elements[0].equals("OfferDetail"))
    // {
    // if (offerDetails == null || elements[1].equals("Oid"))
    // {
    // offerDetails = elements;
    // Log.i("Parsing", "offers start");
    // }
    // else
    // {
    //
    // OfferDetail ct = dbHelper.getOfferDetailByRemoteGuid(elements[1]);
    // if (ct == null)
    // {
    // if (offerDetailInsert == null)
    // {
    // // Insert
    // // txt.WriteLine("OfferDetail" + + "Oid" + +
    // // "OfferOid" + + "ItemOid" + + "isActive" + +
    // // "UpdatedOn");
    // Log.i("Parse Line", "Compiling offerDetailInsert");
    // query =
    // "insert into OfferDetail_Temp(RemoteGuid,OfferRemoteGuid,ItemRemoteGuid,Active,UpdatedOn) ";
    // query += "values(?,?,?,?,?)";
    // offerDetailInsert =
    // dbHelper.getWritableDatabase().compileStatement(query);
    // }
    // for (int i = 1; i < 6; i++)
    // offerDetailInsert.bindString(i, elements[i]);
    // offerDetailInsert.executeInsert();
    //
    // }
    // else
    // {
    // if (offerDetailrUpdate == null)
    // {
    // Log.i("Parse Line", "Compiling offerUpdate");
    // query =
    // "update OfferDetail set OfferRemoteGuid=?,ItemRemoteGuid=?,Active=?,UpdatedOn=? where remoteGuid = ?";
    // offerDetailrUpdate =
    // dbHelper.getWritableDatabase().compileStatement(query);
    // }
    // for (int i = 2; i < 6; i++)
    // {
    // offerDetailrUpdate.bindString(i - 1, elements[i]);
    // }
    // offerDetailrUpdate.bindString(5, elements[1]);
    // offerDetailrUpdate.executeUpdateDelete();
    // }
    //
    // offerdetailupdatedon = Long.parseLong(elements[5]);
    // }
    // }
    // else if (elements[0].equals("Address"))
    // {
    // if (addresses == null)
    // {
    // addresses = elements;
    // Log.i("Parsing", "Addresses start");
    // }
    // else
    // {
    //
    // // Address ct =
    // // dbHelper.getAddressByRemoteGuid(elements[1]);
    // // if(addressSelect == null)
    // // {
    // // query =
    // //
    // "select ID from address where remoteGuid = ? and customerRemoteGuid = ?";
    // // addressSelect =
    // // dbHelper.getWritableDatabase().compileStatement(query);
    // // }
    // // else
    // // {
    // // addressSelect.clearBindings();
    // // }
    //
    // Address ct = null;
    // try
    // {
    // // Log.v("Address Insert", "Begin Find");
    //
    // // addressSelect.bindString(0, elements[1]);
    // // addressSelect.bindString(1, elements[2]);
    // // addressSelect.
    // // ct = dbHelper.getAddressByRemoteGuid(elements[1]);
    // ct = dbHelper.getAddressByRemoteGuid(elements[1], elements[2]);
    // // ct = dbHelper.findObject(Address.class,
    // // OperatorType.AND, new BinaryOperator("remoteGuid",
    // // elements[1]), new
    // // BinaryOperator("customerRemoteGuid", elements[2]));
    // // Log.v("Address Insert", "End Find");
    // }
    // catch (Exception e)
    // {
    // Log.e("Query Failed", "Address query failed");
    // e.printStackTrace();
    // }
    //
    // if (ct == null || !ct.getCustomerRemoteGuid().equals(elements[2]))
    // {
    // if (addressInsert == null)
    // {
    //
    // // Insert
    // Log.i("Parse Line", "Compiling address insert");
    // query =
    // "insert into address_Temp(remoteGuid, customerRemoteGuid, Address) ";
    // query += "values(?,?,?)";
    // addressInsert = dbHelper.getWritableDatabase().compileStatement(query);
    // }
    // for (int i = 1; i < 4; i++)
    // {
    // addressInsert.bindString(i, elements[i]);
    // }
    //
    // // Log.v("Address Insert", "Begin Insert");
    // addressInsert.executeInsert();
    // // Log.v("Address Insert", "End Insert");
    //
    // }
    // else
    // {
    //
    // if (addressUpdate == null)
    // {
    // Log.i("Parse Line", "Compiling address update");
    // query =
    // "update address set customerRemoteGuid=?,Address=? where remoteGuid = ?";
    // addressUpdate = dbHelper.getWritableDatabase().compileStatement(query);
    // }
    // for (int i = 2; i < 4; i++)
    // {
    // addressUpdate.bindString(i - 1, elements[i]);
    // }
    //
    // addressUpdate.bindString(3, elements[1]);
    // addressUpdate.executeUpdateDelete();
    // }
    // }
    // }
    // else if (elements[0].equals("Customer"))
    // {
    // try
    // {
    // if (customers == null || elements[8].equals("UpdatedOn"))
    // {
    // customers = elements;
    // Log.i("Parsing", "Customers start");
    // }
    // else
    // {
    // // Log.v("Customer Insert", "Begin Find");
    // Customer ct = dbHelper.getCustomerByRemoteGuid(elements[5]);
    // // Log.v("Customer Insert", "End Find");
    // if (ct == null)
    // {
    // if (customerInsert == null)
    // {
    // // Insert
    // Log.i("Parse Line", "Compiling customer insert");
    // query =
    // "insert into customer_Temp(Code, CompanyName,DefaultAddress,DefaultPhone,remoteGuid,pcRemoteGuid,storeRemoteGuid,UpdatedOn,TaxCode,vatLevelRemoteGuid,lowerCompanyName) ";
    // query += "values(?,?,?,?,?,?,?,?,?,?,?)";
    // customerInsert = dbHelper.getWritableDatabase().compileStatement(query);
    // }
    // for (int i = 1; i < 11; i++)
    // customerInsert.bindString(i, elements[i]);
    // customerInsert.bindString(11, elements[2].toLowerCase());
    // customerInsert.executeInsert();
    //
    // }
    // else
    // {
    //
    // if (customerUpdate == null)
    // {
    // Log.i("Parse Line", "Compiling customer update");
    // query =
    // "update customer set Code=?, CompanyName=?,DefaultAddress=?,DefaultPhone=?,pcRemoteGuid=?,storeRemoteGuid=?,UpdatedOn=?,TaxCode=?,vatLevelRemoteGuid=?,lowerCompanyName=? where remoteGuid = ?";
    // customerUpdate = dbHelper.getWritableDatabase().compileStatement(query);
    // }
    // for (int i = 1; i < 5; i++)
    // {
    // customerUpdate.bindString(i, elements[i]);
    // }
    // for (int i = 6; i < 11; i++)
    // {
    // customerUpdate.bindString(i - 1, elements[i]);
    // }
    // customerUpdate.bindString(10, elements[2].toLowerCase());
    // customerUpdate.bindString(11, elements[5]);
    // customerUpdate.executeUpdateDelete();
    // }
    //
    // customerUpdatedOn = Long.parseLong(elements[8]);
    // }
    // }
    // catch (Exception e)
    // {
    // throw e;
    // }
    // }
    // else if (elements[0].equals("Item"))
    // {
    // if (items == null || elements[1].equals("Code"))
    // {
    // items = elements;
    // Log.i("Parsing", "items start");
    // }
    // else
    // {
    // Item ct = dbHelper.getItemByRemoteGuid(elements[8]);
    // if (ct == null)
    // {
    // if (itemInsert == null)
    // {
    // Log.i("Parse Line", "Compiling item insert");
    // query =
    // "insert into item_Temp(Code, defaultBarcodeRemoteGuid,imageOid,InsertedOn,isactive,maxOrderQty,Name,remoteGuid,packingQty,UpdatedOn,vatCategoryRemoteGuid,loweCaseName)";
    // query += "values(?,?,?,?,?,?,?,?,?,?,?,?)";
    // itemInsert = dbHelper.getWritableDatabase().compileStatement(query);
    // }
    // for (int i = 1; i < 12; i++)
    // itemInsert.bindString(i, elements[i]);
    // itemInsert.bindString(12, elements[7].toLowerCase());
    // itemInsert.executeInsert();
    // }
    // else
    // {
    // if (itemUpdate == null)
    // {
    // Log.i("Parse Line", "Compiling item update");
    // query =
    // "update item set Code=?, defaultBarcodeRemoteGuid=?,imageOid=?,InsertedOn=?,isactive=?,maxOrderQty=?,Name=?,remoteGuid=?,packingQty=?,UpdatedOn=?,vatCategoryRemoteGuid=?,loweCaseName=?";
    // query += " where remoteGuid=?";
    // itemUpdate = dbHelper.getWritableDatabase().compileStatement(query);
    // }
    // for (int i = 1; i < 8; i++)
    // itemUpdate.bindString(i, elements[i]);
    // for (int i = 9; i < 12; i++)
    // itemUpdate.bindString(i - 1, elements[i]);
    // itemUpdate.bindString(11, elements[7].toLowerCase());
    // itemUpdate.bindString(12, elements[8]);
    // itemUpdate.executeUpdateDelete();
    // }
    //
    // itemupdatedon = Long.parseLong(elements[10]);
    // }
    //
    // }
    // else if (elements[0].equals("ItemAnalyticTree"))
    // {
    // if (itemATs == null || elements[1].equals("Oid"))
    // {
    // itemATs = elements;
    // Log.i("Parsing", "Itemanalytictree start");
    // }
    // else
    // {
    // ItemAnalyticTree ct =
    // dbHelper.getItemAnalyticTreeByRemoteGuid(elements[1]);
    // if (ct == null)
    // {
    // if (itemAnalyticTreeInsert == null)
    // {
    // // Insert
    // Log.i("Parse Line", "Compiling itemAT insert");
    // query =
    // "insert into itemanalytictree_Temp(remoteGuid, itemRemoteGuid,itemCategoryRemoteGuid,UpdatedOn) ";
    // query += " values(?,?,?,?)";
    // itemAnalyticTreeInsert =
    // dbHelper.getWritableDatabase().compileStatement(query);
    // }
    // for (int i = 1; i < 5; i++)
    // itemAnalyticTreeInsert.bindString(i, elements[i]);
    // itemAnalyticTreeInsert.executeInsert();
    // }
    // else
    // {
    //
    // // update
    // if (itemAnalyticTreeUpdate == null)
    // {
    // Log.i("Parse Line", "Compiling itemAT update");
    // query =
    // "update itemanalytictree set itemRemoteGuid=?,itemCategoryRemoteGuid=?,UpdatedOn=?";
    // query += " where remoteGuid = ?";
    // itemAnalyticTreeUpdate =
    // dbHelper.getWritableDatabase().compileStatement(query);
    // }
    //
    // for (int i = 2; i < 5; i++)
    // itemAnalyticTreeUpdate.bindString(i - 1, elements[i]);
    // itemAnalyticTreeUpdate.bindString(4, elements[1]);
    // itemAnalyticTreeUpdate.executeUpdateDelete();
    // }
    //
    // iatupdatedon = Long.parseLong(elements[4]);
    //
    // }
    //
    // }
    // else if (elements[0].equals("Barcode"))
    // {
    // if (barcodes == null || elements[1].equals("Oid"))
    // {
    // barcodes = elements;
    // Log.i("Parsing", "Barcode start");
    // }
    // else
    // {
    // Barcode ct = dbHelper.getBarcodeByRemoteGuid(elements[1]);
    // if (ct == null)
    // {
    // // Insert
    // if (barcodeInsert == null)
    // {
    // Log.i("Parse Line", "Compiling barcodeInsert");
    // query =
    // "insert into barcode_Temp(remoteGuid, itemRemoteGuid,measurementUnitRemoteGuid,Code,UpdatedOn) ";
    // query += " values(?,?,?,?,?)";
    // barcodeInsert = dbHelper.getWritableDatabase().compileStatement(query);
    // }
    // for (int i = 1; i < 6; i++)
    // barcodeInsert.bindString(i, elements[i]);
    // barcodeInsert.executeInsert();
    // }
    // else
    // {
    // // update
    // if (barcodeUpdate == null)
    // {
    // Log.i("Parse Line", "Compiling barcodeUpdate");
    // query =
    // "update barcode set itemRemoteGuid=?,measurementUnitRemoteGuid=?,Code=?,UpdatedOn=?";
    // query += " where remoteGuid = ?";
    // barcodeUpdate = dbHelper.getWritableDatabase().compileStatement(query);
    // }
    // for (int i = 2; i < 6; i++)
    // {
    // barcodeUpdate.bindString(i - 1, elements[i]);
    // }
    // barcodeUpdate.bindString(5, elements[1]);
    // }
    // barcodeupdatedon = Long.parseLong(elements[5]);
    // }
    //
    // }
    // else if (elements[0].equals("ItemCategory"))
    // {
    // if (itemcategory == null || elements[1].equals("Oid"))
    // {
    // itemcategory = elements;
    // Log.i("Parsing", "itemcategory start");
    // }
    // else
    // {
    // ItemCategory ct = dbHelper.getItemCategoryByRemoteGuid(elements[1]);
    // if (ct == null)
    // {
    // if (itemCategoryInsert == null)
    // {
    // // Insert
    // Log.i("Parse Line", "Compiling itemCategoryInsert");
    // query =
    // "insert into itemcategory_Temp(remoteGuid, remoteParentGuid,Name,Code,UpdatedOn) ";
    // query += " values(?,?,?,?,?)";
    // itemCategoryInsert =
    // dbHelper.getWritableDatabase().compileStatement(query);
    // }
    // for (int i = 1; i < 6; i++)
    // itemCategoryInsert.bindString(i, elements[i]);
    // itemCategoryInsert.executeInsert();
    // }
    // else
    // {
    // if (itemCategoryUpdate == null)
    // {
    // // update
    // Log.i("Parse Line", "Compiling itemCategoryUpdate");
    // query =
    // "update itemcategory set remoteParentGuid=?,Name=?,Code=?,UpdatedOn=?";
    // query += " where remoteGuid =?";
    // itemCategoryUpdate =
    // dbHelper.getWritableDatabase().compileStatement(query);
    //
    // }
    // for (int i = 2; i < 6; i++)
    // {
    // itemCategoryUpdate.bindString(i - 1, elements[i]);
    // }
    // itemCategoryUpdate.bindString(5, elements[1]);
    // itemCategoryUpdate.executeUpdateDelete();
    // }
    // itemcatupdatedon = Long.parseLong(elements[5]);
    // }
    // }
    // else if (elements[0].equals("PriceCatalog"))
    // {
    // if (pricecatalog == null || elements[1].equals("Oid"))
    // {
    // pricecatalog = elements;
    // Log.i("Parsing", "pricecatalog start");
    // }
    // else
    // {
    // PriceCatalog ct = dbHelper.getPriceCatalogByRemoteGuid(elements[1]);
    // if (ct == null)
    // {
    // if (pc_in == null)
    // {
    // // Insert
    // Log.i("Parse Line", "Compiling pc_in");
    // query =
    // "insert into pricecatalog_Temp(remoteGuid, remoteParentGuid,Name,Code,UpdatedOn) ";
    // query += " values(?,?,?,?,?)";
    // pc_in = dbHelper.getWritableDatabase().compileStatement(query);
    // }
    // for (int i = 1; i < 6; i++)
    // pc_in.bindString(i, elements[i]);
    // pc_in.executeInsert();
    // }
    // else
    // {
    // if (pc_up == null)
    // {
    // // update
    // Log.i("Parse Line", "Compiling pc_up");
    // query =
    // "update pricecatalog set remoteParentGuid=?,Name=?,Code=?,UpdatedOn=?";
    // query += " where remoteGuid =?";
    // pc_up = dbHelper.getWritableDatabase().compileStatement(query);
    //
    // }
    // for (int i = 2; i < 6; i++)
    // {
    // pc_up.bindString(i - 1, elements[i]);
    // }
    // pc_up.bindString(5, elements[1]);
    // pc_up.executeUpdateDelete();
    // }
    // pcupdatedon = Long.parseLong(elements[5]);
    // }
    // }
    // else if (elements[0].equals("PriceCatalogDetail"))
    // {
    // if (pricecatalogdetail == null || elements[1].equals("Oid"))
    // {
    // pricecatalogdetail = elements;
    // Log.i("Parsing", "pricecatalogdetail start");
    // }
    // else
    // {
    // PriceCatalogDetail ct =
    // dbHelper.getPriceCatalogDetailByRemoteGuid(elements[1]);
    // if (ct == null)
    // {
    // if (pcd_in == null)
    // {
    // // Insert
    // Log.i("Parse Line", "Compiling pcd_in");
    // query =
    // "insert into pricecatalogdetail_Temp(remoteGuid, barcodeRemoteGuid,pcRemoteGuid,itemRemoteGuid,price,discount,VATIncluded,UpdatedOn) ";
    // query += " values(?,?,?,?,?,?,?,?)";
    // pcd_in = dbHelper.getWritableDatabase().compileStatement(query);
    // }
    // for (int i = 1; i < 9; i++)
    // pcd_in.bindString(i, elements[i].replace(",", "."));
    // pcd_in.executeInsert();
    // }
    // else
    // {
    // if (pcd_up == null)
    // {
    // // update
    // query =
    // "update pricecatalogdetail set barcodeRemoteGuid=?,pcRemoteGuid=?,itemRemoteGuid=?,price=?,discount=?,VATIncluded=?,UpdatedOn=?";
    // query += " where remoteGuid = ?";
    // pcd_up = dbHelper.getWritableDatabase().compileStatement(query);
    // }
    // for (int i = 2; i < 9; i++)
    // pcd_up.bindString(i - 1, elements[i].replace(",", "."));
    //
    // pcd_up.bindString(8, elements[1]);
    // pcd_up.executeUpdateDelete();
    // }
    //
    // pcdupdatedon = Long.parseLong(elements[8]);
    // }
    // }
    // // dbHelper.getWritableDatabase().compileStatement(query);
    // // dbHelper.getWritableDatabase().execSQL(query);
    //
    // elements = null;
    // line = null;
    // // PriceCatalogDetail, Oid, BarcodeOid, PriceCatalogOid, ItemOid,
    // // value, discount, vatIncluded, UpdatedOn
    // return true;
    // }
    //
    // protected boolean parseFile(String filename)
    // {
    // try
    // {
    // DatabaseConnection conn =
    // dbHelper.getConnectionSource().getReadWriteConnection();
    // try
    // {
    // int lines = 0;
    // File fl = new File(filename);
    // int flsize = (int) fl.length(), readbytes = 0;
    // progDialog.setMax(flsize);
    // fl = null;
    // FileInputStream fs = new FileInputStream(filename);
    // InputStreamReader st = new InputStreamReader(fs);
    // BufferedReader br = new BufferedReader(st);
    // conn.setAutoCommit(false);
    // String line;
    // while ((line = br.readLine()) != null)
    // {
    // lines++;
    // readbytes += line.length();
    // if (lines % 500 == 0)
    // {
    // publishProgress(readbytes);
    // if (lines % 20000 == 0)
    // {
    // Log.i("Parser", "Commiting 20000 records - start");
    // conn.setAutoCommit(true);
    // conn.setAutoCommit(false);
    // Log.i("Parser", "Commiting 20000 records - end");
    // }
    // }
    // if (!parseLine(line.replace("\0", "")))
    // {
    // conn.setAutoCommit(true);
    // return false;
    // }
    // line = null;
    // }
    // }
    // catch (Exception ex)
    // {
    // ex.printStackTrace();
    // conn.setAutoCommit(true);
    // return false;
    // }
    // conn.setAutoCommit(true);
    // return true;
    // }
    // catch (Exception ex)
    // {
    // ex.printStackTrace();
    // return false;
    // }
    // }
    //
    // protected void clearTempTables()
    // {
    // try
    // {
    // dbHelper.getWritableDatabase().execSQL("drop table Item_Temp");
    // dbHelper.getWritableDatabase().execSQL("create table Item_Temp as select * from Item where 1<>1");
    //
    // dbHelper.getWritableDatabase().execSQL("drop table Offer_Temp");
    // dbHelper.getWritableDatabase().execSQL("create table Offer_Temp as select * from Offer where 1<>1");
    //
    // dbHelper.getWritableDatabase().execSQL("drop table OfferDetail_Temp");
    // dbHelper.getWritableDatabase().execSQL("create table OfferDetail_Temp as select * from OfferDetail where 1<>1");
    //
    // dbHelper.getWritableDatabase().execSQL("drop table Store_Temp");
    // dbHelper.getWritableDatabase().execSQL("create table Store_Temp as select * from Store where 1<>1");
    //
    // dbHelper.getWritableDatabase().execSQL("drop table address_Temp");
    // dbHelper.getWritableDatabase().execSQL("create table address_Temp as select * from address where 1<>1");
    //
    // dbHelper.getWritableDatabase().execSQL("drop table barcode_Temp");
    // dbHelper.getWritableDatabase().execSQL("create table barcode_Temp as select * from barcode where 1<>1");
    //
    // dbHelper.getWritableDatabase().execSQL("drop table customer_Temp");
    // dbHelper.getWritableDatabase().execSQL("create table customer_Temp as select * from customer where 1<>1");
    //
    // dbHelper.getWritableDatabase().execSQL("drop table documentstatus_Temp");
    // dbHelper.getWritableDatabase().execSQL("create table documentstatus_Temp as select * from documentstatus where 1<>1");
    //
    // dbHelper.getWritableDatabase().execSQL("drop table itemanalytictree_Temp");
    // dbHelper.getWritableDatabase().execSQL("create table itemanalytictree_Temp as select * from itemanalytictree where 1<>1");
    //
    // dbHelper.getWritableDatabase().execSQL("drop table itemcategory_Temp");
    // dbHelper.getWritableDatabase().execSQL("create table itemcategory_Temp as select * from itemcategory where 1<>1");
    //
    // dbHelper.getWritableDatabase().execSQL("drop table linkeditem_Temp");
    // dbHelper.getWritableDatabase().execSQL("create table linkeditem_Temp as select * from linkeditem where 1<>1");
    //
    // dbHelper.getWritableDatabase().execSQL("drop table measurementunit_Temp");
    // dbHelper.getWritableDatabase().execSQL("create table measurementunit_Temp as select * from measurementunit where 1<>1");
    //
    // dbHelper.getWritableDatabase().execSQL("drop table pricecatalog_Temp");
    // dbHelper.getWritableDatabase().execSQL("create table pricecatalog_Temp as select * from pricecatalog where 1<>1");
    //
    // dbHelper.getWritableDatabase().execSQL("drop table pricecatalogdetail_Temp");
    // dbHelper.getWritableDatabase().execSQL("create table pricecatalogdetail_Temp as select * from pricecatalogdetail where 1<>1");
    //
    // dbHelper.getWritableDatabase().execSQL("drop table vatcategory_Temp");
    // dbHelper.getWritableDatabase().execSQL("create table vatcategory_Temp as select * from vatcategory where 1<>1");
    //
    // dbHelper.getWritableDatabase().execSQL("drop table vatfactor_Temp");
    // dbHelper.getWritableDatabase().execSQL("create table vatfactor_Temp as select * from vatfactor where 1<>1");
    //
    // dbHelper.getWritableDatabase().execSQL("drop table vatlevel_Temp");
    // dbHelper.getWritableDatabase().execSQL("create table vatlevel_Temp as select * from vatlevel where 1<>1");
    //
    // }
    // catch (Exception e)
    // {
    // e.printStackTrace();
    // }
    //
    // }
    //
    // protected void clearTempTable(String tableName)
    // {
    // dbHelper.getWritableDatabase().execSQL("drop table " + tableName);
    // dbHelper.getWritableDatabase().execSQL("create table " + tableName +
    // " as select * from " + tableName.replace("_Temp", "") + " where 1<>1");
    // }
    //
    // private long customerUpdatedOn;
    // private long itemupdatedon;
    // private long barcodeupdatedon;
    // private long pcdupdatedon;
    // private long iatupdatedon;
    // private long itemcatupdatedon;
    // private long pcupdatedon;
    // private long vcupdatedon;
    // private long vfupdatedon;
    // private long vlupdatedon;
    // private long offerupdatedon;
    // private long offerdetailupdatedon;
    // private long docstatusupdatedon;
    // private long storesupdatedon;
    // private long linkeditemupdatedon;
    // private long measurementunitupdatedon;
    //
    // @Override
    // protected Object doInBackground(User... arg0)
    // {
    //
    // MoveTempRowsIntoTables(); // data left from previous update that
    // // timed out
    // clearTempTables();
    //
    // Log.i("Syncrhonize Text", "Start");
    // progDialogMessage = "Initializing...";
    // publishProgress(0);
    // SynchronizeApplicationSettings(arg0[0]);
    // SynchronizeUserStoreAccessOfUser(arg0[0]);
    // long originalItemUpdatedOn = dbHelper.getMaxItemUpdatedOn();
    // long originalBarcodeUpdatedOn = dbHelper.getMaxBarcodeUpdatedOn();
    // long originalCustomerUpdatedOn = dbHelper.getMaxCustomerUpdatedOn();
    // long originalIatUpdatedOn = dbHelper.getMaxItemAnalyticTreesUpdatedOn();
    // long originalPCDUpdatedOn = dbHelper.getMaxPriceCatalogDetail();
    //
    // boolean isUpdateOver = false;
    //
    // Log.i("Syncrhonize Text", "Grow Heap");
    // char[] tmp = new char[1024 * 1024 * 10];
    // tmp = null;
    // Log.i("Syncrhonize Text", "Grow Heap end");
    // int previousRowsRemaining = -1;
    //
    // customerUpdatedOn = dbHelper.getMaxCustomerUpdatedOn();
    // itemupdatedon = dbHelper.getMaxItemUpdatedOn();
    // barcodeupdatedon = dbHelper.getMaxBarcodeUpdatedOn();
    // pcdupdatedon = dbHelper.getMaxPriceCatalogDetail();
    // iatupdatedon = dbHelper.getMaxItemAnalyticTreesUpdatedOn();
    // itemcatupdatedon = dbHelper.getMaxItemCategoriesUpdatedOn();
    // pcupdatedon = dbHelper.getMaxPriceCatalog();
    // vcupdatedon = dbHelper.getMaxVatCategory();
    // vfupdatedon = dbHelper.getMaxVatFactor();
    // vlupdatedon = dbHelper.getMaxVatLevel();
    // offerupdatedon = dbHelper.getMaxOffersUpdatedOn();
    // offerdetailupdatedon = dbHelper.getMaxOfferDetailsUpdatedOn();
    // docstatusupdatedon = dbHelper.getMaxDocumentStatusUpdatedOn();
    // storesupdatedon = dbHelper.getMaxStoresUpdatedOn();
    // linkeditemupdatedon = dbHelper.getMaxLinkedItem();
    // measurementunitupdatedon = dbHelper.getMaxMeasurementUnitUpdatedOn();
    //
    // while (!isUpdateOver)
    // {
    // // STEP 1 Download File
    // String url = ((MainActivity) mContext).appsettings.getServiceUrl();
    // HashMap<String, Object> parameters = new HashMap<String, Object>();
    //
    // // long customerUpdatedOn = dbHelper.getMaxCustomerUpdatedOn(),
    // // itemupdatedon = dbHelper.getMaxItemUpdatedOn(),
    // // barcodeupdatedon = dbHelper.getMaxBarcodeUpdatedOn(),
    // // pcdupdatedon = dbHelper.getMaxPriceCatalogDetail(),
    // // iatupdatedon = dbHelper.getMaxItemAnalyticTreesUpdatedOn();
    // parameters.put("Customerupdatedon", "" + customerUpdatedOn);
    // parameters.put("Itemupdatedon", "" + itemupdatedon);
    // parameters.put("IATupdatedon", "" + iatupdatedon);
    // parameters.put("Barcodeupdatedon", "" + barcodeupdatedon);
    //
    // parameters.put("ICupdatedon", "" + itemcatupdatedon);
    // parameters.put("PCupdatedon", "" + pcupdatedon);
    // parameters.put("PCDupdatedon", "" + pcdupdatedon);
    // parameters.put("VCupdatedon", "" + vcupdatedon);
    //
    // parameters.put("VFupdatedon", "" + vfupdatedon);
    // parameters.put("VLupdatedon", "" + vlupdatedon);
    // parameters.put("offerUpdatedOn", "" + offerupdatedon);
    // parameters.put("offerDetailUpdatedOn", "" + offerdetailupdatedon);
    //
    // parameters.put("documentStatusUpdatedOn", "" + docstatusupdatedon);
    // parameters.put("storesUpdatedOn", "" + storesupdatedon);
    // parameters.put("linkedItemsUpdatedOn", "" + linkeditemupdatedon);
    // parameters.put("mmUpdatedOn", "" + measurementunitupdatedon);
    //
    // parameters.put("userID", arg0[0].getRemoteGuid());
    // Log.d("ws params", parameters.toString());
    // int oldTimeout = TIMEOUT;
    //
    // TIMEOUT = 100000;
    // progDialogMessage = "Contacting update server...";
    // publishProgress(0);
    // int triesLeft = 3;
    // Object filenameobj = null;
    //
    // while (triesLeft != 0 && filenameobj == null)
    // {
    // filenameobj = SynchronousCallWebMethod("GetTotalUpdatesLink",
    // parameters);
    // triesLeft--;
    // }
    //
    // TIMEOUT = oldTimeout;
    // if (filenameobj == null)
    // {
    // Log.i("SynchronizerText", "Filename problem");
    // errorMessage = "Timeout trying to get file link";
    // return false;
    // }
    // String filename = filenameobj.toString();
    //
    // url = url.toLowerCase().replace("retailservice.asmx", "Downloads/" +
    // filename);
    // String outputFile = Environment.getExternalStorageDirectory().toString()
    // + "/retail/temp.zip";
    // progDialogMessage = "Download update file...";
    // publishProgress(0);
    // Log.i("Syncrhonize Text", "Download start");
    // if (downloadFile(url, outputFile))
    // {
    // Log.i("Syncrhonize Text", "Download end");
    //
    // parameters = new HashMap<String, Object>();
    // parameters.put("userID", arg0[0].getRemoteGuid());
    // parameters.put("filename", filename);
    // SynchronousCallWebMethod("CleanFile", parameters);
    //
    // // UNZIP the file
    // Log.i("Syncrhonize Text", "Unpack start");
    // unpackZip(Environment.getExternalStorageDirectory().toString() +
    // "/retail/", "temp.zip");
    // Log.i("Syncrhonize Text", "Unpack end");
    // // Parse file
    // Log.i("Syncrhonize Text", "Processing start");
    // try
    // {
    // progDialogMessage = "Applying update file...";
    // publishProgress(0);
    // dbHelper.getWritableDatabase().execSQL("PRAGMA synchronous=1  ;");
    //
    // if (!parseFile(outputFile.replace("temp.zip", "Database.txt")))
    // {
    // errorMessage = "Parsing of file failed.";
    // return false;
    // }
    //
    // }
    // catch (Exception ex)
    // {
    // ex.printStackTrace();
    // errorMessage = ex.getMessage();
    // return false;
    // }
    // Log.i("Syncrhonize Text", "Processing end");
    // }
    // else
    // {
    // return false;
    // }
    //
    // oldTimeout = TIMEOUT;
    //
    // TIMEOUT = 100000;
    //
    // int rowsRemaining = -1;
    // triesLeft = 5;
    // while (rowsRemaining == -1 && triesLeft != 0)
    // {
    // rowsRemaining = getTotalUpdates(customerUpdatedOn, itemupdatedon,
    // iatupdatedon, barcodeupdatedon, itemcatupdatedon, pcupdatedon,
    // pcdupdatedon, vcupdatedon, vfupdatedon, vlupdatedon, offerupdatedon,
    // offerdetailupdatedon, docstatusupdatedon, storesupdatedon,
    // linkeditemupdatedon, measurementunitupdatedon);
    // triesLeft--;
    // }
    //
    // TIMEOUT = oldTimeout;
    //
    // if (rowsRemaining == -1)
    // {
    // errorMessage = "Time out trying to count remaining rows";
    // return false;
    // }
    //
    // // logo tis apoklisis, an 2 fores stin seira apomenoun ta idia
    // // rows tote to update exei teliosei
    // isUpdateOver = (rowsRemaining == previousRowsRemaining || rowsRemaining
    // == 0);
    // previousRowsRemaining = rowsRemaining;
    //
    // Log.i("Syncrhonize Text", "File loop end. Rows remaining: " +
    // rowsRemaining);
    // }
    //
    // MoveTempRowsIntoTables();
    //
    // progDialogMessage = "Optimizing database for faster performance...";
    // // Queries to restore fks
    // String[] queries = new String[] {
    // "update customer set pc_id =(select id from pricecatalog where pricecatalog.remoteGuid = customer.pcRemoteGuid), vl_id = (select id from Vatlevel where vatlevel.remoteGuid = customer.vatlevelremoteGuid ), store_id = (select id from store where store.remoteGuid = customer.storeremoteGuid ) where store_id is null or vl_id is null or pc_id is null or updatedon >= "
    // + originalCustomerUpdatedOn,
    // "update item set vatcategory_id = (select id from Vatcategory where Vatcategory.remoteGuid = Item.VatcategoryRemoteGuid), defaultBarcodeObject_id = (select id from Barcode where Barcode.remoteGuid = Item.defaultBarcodeRemoteGuid) where vatcategory_id is null or defaultBarcodeObject_id is null or updatedon >="
    // + originalItemUpdatedOn,
    // "update barcode set item_id= (select id from Item where item.remoteGuid = barcode.itemRemoteGuid), MeasurementUnit_id= (select id from MeasurementUnit where MeasurementUnit.remoteGuid = barcode.measurementUnitRemoteGuid) where item_id is null or MeasurementUnit_id is null or updatedon >="
    // + originalBarcodeUpdatedOn,
    // "update itemanalytictree set item_id = (select id from item where item.remoteGuid = itemanalytictree .itemRemoteGuid), itemcategory_id = (select id from itemcategory where itemcategory.remoteguid = itemanalytictree .itemcategoryRemoteGuid) where item_id is null or itemcategory_id is null or updatedon >= "
    // + originalIatUpdatedOn,
    // "update itemcategory set parent_id = (select id from itemcategory a where a.remoteGuid = itemcategory.remoteparentguid)",
    // "update pricecatalog set parent_id = (select id from pricecatalog a where a.remoteGuid = pricecatalog .remoteparentguid)",
    // "update pricecatalogdetail set pc_id = (select id from pricecatalog pc where pc.remoteGuid = pricecatalogdetail.pcremoteguid), bc_id = (select id from barcode bc where bc.remoteGuid = pricecatalogdetail.barcoderemoteguid), item_id = (select id from item  where item.remoteGuid = pricecatalogdetail.itemremoteguid) where item_id is null or bc_id is null or pc_id is null or updatedon >="
    // + originalPCDUpdatedOn,
    // "update vatfactor set vatlevel_id = (select id from vatlevel where vatlevel.remoteguid = vatfactor.vatlevelremoteguid), vatcategory_id = (select id from vatcategory where vatcategory.remoteguid = vatfactor.vatcategoryremoteguid)",
    // "update offer set PriceCatalog_id = (select id from pricecatalog where offer.PriceCatalogremoteguid = pricecatalog.remoteguid)",
    // "update offerdetail set item_id = (select id from item where item.remoteGuid = offerdetail.itemremoteguid), offer_id = (select id from offer where offer.remoteguid = offerdetail.offerremoteguid)",
    // "update address set customer_id = (select id from customer where customer.remoteGuid=address.customerRemoteGuid)"
    // };
    //
    // progDialog.setMax(queries.length + 1);
    // publishProgress(0);
    // int i = 0;
    // DatabaseConnection conn;
    // try
    // {
    // conn = dbHelper.getConnectionSource().getReadWriteConnection();
    //
    // conn.setAutoCommit(false);
    // for (String query : queries)
    // {
    // Log.i("Sync", "Executing:" + query);
    // dbHelper.getWritableDatabase().compileStatement(query).executeUpdateDelete();
    // publishProgress((++i));
    // }
    // conn.setAutoCommit(true);
    // // restoring database status
    // dbHelper.getWritableDatabase().execSQL("PRAGMA synchronous=2  ;");
    // SynchronizeItemImages(originalItemUpdatedOn, arg0[0]);
    //
    // Log.i("Syncrhonize Text", "End");
    // ((MainActivity)
    // mContext).getPreferences(mContext.MODE_PRIVATE).edit().putBoolean("init",
    // true).commit();
    // ((MainActivity) mContext).isDBInitialized = true;
    //
    // clearTempTables();
    //
    // return true;
    //
    // }
    // catch (SQLException e)
    // {
    // e.printStackTrace();
    // errorMessage = e.getMessage();
    // return false;
    // }
    //
    // }
    //
    // private void MoveTempRowsIntoTables()
    // {
    // try
    // {
    // progDialogMessage = "Moving data...";
    // // move temp rows into tables
    //
    // progDialog.setMax(16);
    // int progress = 0;
    // publishProgress(progress);
    //
    // Log.i("Moving Temp Rows", "DocumentStatus start");
    //
    // // removes duplicate rows
    // dbHelper.getWritableDatabase().compileStatement("DELETE FROM DocumentStatus_Temp WHERE ID > (SELECT min(ID) FROM DocumentStatus_Temp b WHERE remoteGuid = b.remoteGuid);").executeUpdateDelete();
    // dbHelper.getWritableDatabase().compileStatement("INSERT INTO DocumentStatus SELECT * FROM DocumentStatus_Temp WHERE remoteGuid NOT IN ( SELECT remoteGuid FROM DocumentStatus)").executeInsert();
    //
    // Log.i("Moving Temp Rows", "DocumentStatus end");
    // this.clearTempTable("DocumentStatus_Temp");
    //
    // progress++;
    // publishProgress(progress);
    //
    // Log.i("Moving Temp Rows", "Store start");
    // // removes duplicate rows
    // dbHelper.getWritableDatabase().compileStatement("DELETE FROM Store_Temp WHERE ID > (SELECT min(ID) FROM Store_Temp b WHERE remoteGuid = b.remoteGuid);").executeUpdateDelete();
    // dbHelper.getWritableDatabase().compileStatement("INSERT INTO Store SELECT * FROM Store_Temp WHERE remoteGuid NOT IN ( SELECT remoteGuid FROM Store)").executeInsert();
    //
    // Log.i("Moving Temp Rows", "Store end");
    // this.clearTempTable("Store_Temp");
    //
    // progress++;
    // publishProgress(progress);
    //
    // Log.i("Moving Temp Rows", "MeasurementUnit start");
    // // removes duplicate rows
    // dbHelper.getWritableDatabase().compileStatement("DELETE FROM MeasurementUnit_Temp WHERE ID > (SELECT min(ID) FROM MeasurementUnit_Temp b WHERE remoteGuid = b.remoteGuid);").executeUpdateDelete();
    // dbHelper.getWritableDatabase().compileStatement("INSERT INTO MeasurementUnit SELECT * FROM MeasurementUnit_Temp WHERE remoteGuid NOT IN ( SELECT remoteGuid FROM MeasurementUnit)").executeInsert();
    //
    // Log.i("Moving Temp Rows", "MeasurementUnit end");
    // this.clearTempTable("MeasurementUnit_Temp");
    //
    // progress++;
    // publishProgress(progress);
    //
    // Log.i("Moving Temp Rows", "VATCategory start");
    //
    // // removes duplicate rows
    // dbHelper.getWritableDatabase().compileStatement("DELETE FROM VATCategory_Temp WHERE ID > (SELECT min(ID) FROM VATCategory_Temp b WHERE remoteGuid = b.remoteGuid);").executeUpdateDelete();
    // dbHelper.getWritableDatabase().compileStatement("INSERT INTO VATCategory SELECT * FROM VATCategory_Temp WHERE remoteGuid NOT IN ( SELECT remoteGuid FROM VATCategory)").executeInsert();
    //
    // Log.i("Moving Temp Rows", "VATCategory end");
    // this.clearTempTable("VATCategory_Temp");
    //
    // progress++;
    // publishProgress(progress);
    //
    // Log.i("Moving Temp Rows", "VatLevel start");
    // // removes duplicate rows
    // dbHelper.getWritableDatabase().compileStatement("DELETE FROM VatLevel_Temp WHERE ID > (SELECT min(ID) FROM VatLevel_Temp b WHERE remoteGuid = b.remoteGuid);").executeUpdateDelete();
    // dbHelper.getWritableDatabase().compileStatement("INSERT INTO VatLevel SELECT * FROM VatLevel_Temp WHERE remoteGuid NOT IN ( SELECT remoteGuid FROM VatLevel)").executeInsert();
    //
    // Log.i("Moving Temp Rows", "VatLevel end");
    // this.clearTempTable("VatLevel_Temp");
    //
    // progress++;
    // publishProgress(progress);
    //
    // Log.i("Moving Temp Rows", "Item start");
    //
    // // removes duplicate rows
    // dbHelper.getWritableDatabase().compileStatement("DELETE FROM Item_Temp WHERE ID > (SELECT min(ID) FROM Item_Temp b WHERE remoteGuid = b.remoteGuid);").executeUpdateDelete();
    // dbHelper.getWritableDatabase().compileStatement("INSERT INTO Item SELECT * FROM Item_Temp WHERE remoteGuid NOT IN ( SELECT remoteGuid FROM Item)").executeInsert();
    //
    // Log.i("Moving Temp Rows", "Item end");
    // this.clearTempTable("Item_Temp");
    //
    // progress++;
    // publishProgress(progress);
    //
    // Log.i("Moving Temp Rows", "VatFactor start");
    // // removes duplicate rows
    // dbHelper.getWritableDatabase().compileStatement("DELETE FROM VatFactor_Temp WHERE ID > (SELECT min(ID) FROM VatFactor_Temp b WHERE remoteGuid = b.remoteGuid);").executeUpdateDelete();
    // dbHelper.getWritableDatabase().compileStatement("INSERT INTO VatFactor SELECT * FROM VatFactor_Temp WHERE remoteGuid NOT IN ( SELECT remoteGuid FROM VatFactor)").executeInsert();
    //
    // Log.i("Moving Temp Rows", "VatFactor start");
    // this.clearTempTable("VatFactor_Temp");
    //
    // progress++;
    // publishProgress(progress);
    //
    // Log.i("Moving Temp Rows", "Offer start");
    // // removes duplicate rows
    // dbHelper.getWritableDatabase().compileStatement("DELETE FROM Offer_Temp WHERE ID > (SELECT min(ID) FROM Offer_Temp b WHERE RemoteGuid = b.RemoteGuid);").executeUpdateDelete();
    // dbHelper.getWritableDatabase().compileStatement("INSERT INTO Offer SELECT * FROM Offer_Temp WHERE RemoteGuid NOT IN ( SELECT RemoteGuid FROM Offer)").executeInsert();
    // //
    // dbHelper.getWritableDatabase().execSQL("update Offer set PriceCatalogRemoteGuid=b.PriceCatalogRemoteGuid,Description=b.Description,"
    // // +
    // //
    // "Description2=b.Description2,Active=b.Active,StartDate=b.StartDate,EndDate=b.StartDate,UpdatedOn=b.UpdatedOn"+
    // // " from Offer_Temp b , Offer a " +
    // // "WHERE a.RemoteGuid = b.RemoteGuid");
    // Log.i("Moving Temp Rows", "Offer end");
    // this.clearTempTable("Offer_Temp");
    //
    // progress++;
    // publishProgress(progress);
    //
    // Log.i("Moving Temp Rows", "OfferDetail start");
    // // removes duplicate rows
    // dbHelper.getWritableDatabase().compileStatement("DELETE FROM OfferDetail_Temp WHERE ID > (SELECT min(ID) FROM OfferDetail_Temp b WHERE RemoteGuid = b.RemoteGuid);").executeUpdateDelete();
    // dbHelper.getWritableDatabase().compileStatement("INSERT INTO OfferDetail SELECT * FROM OfferDetail_Temp WHERE RemoteGuid NOT IN ( SELECT RemoteGuid FROM OfferDetail)").executeInsert();
    //
    // Log.i("Moving Temp Rows", "OfferDetail end");
    // this.clearTempTable("Offer_Temp");
    //
    // progress++;
    // publishProgress(progress);
    //
    // Log.i("Moving Temp Rows", "Address start");
    // // removes duplicate rows
    // dbHelper.getWritableDatabase().compileStatement("DELETE FROM address_Temp WHERE ID > (SELECT min(ID) FROM address_Temp b WHERE remoteGuid = b.remoteGuid AND customerRemoteGuid = b.customerRemoteGuid);").executeUpdateDelete();
    // dbHelper.getWritableDatabase().compileStatement("INSERT INTO address SELECT * FROM address_Temp WHERE remoteGuid NOT IN ( SELECT remoteGuid FROM address)").executeInsert();
    //
    // Log.i("Moving Temp Rows", "Address end");
    // this.clearTempTable("address_Temp");
    //
    // progress++;
    // publishProgress(progress);
    //
    // Log.i("Moving Temp Rows", "Customer start");
    // // removes duplicate rows
    // dbHelper.getWritableDatabase().compileStatement("DELETE FROM customer_Temp WHERE ID > (SELECT min(ID) FROM customer_Temp b WHERE remoteGuid = b.remoteGuid);").executeUpdateDelete();
    // dbHelper.getWritableDatabase().compileStatement("INSERT INTO customer SELECT * FROM customer_Temp WHERE remoteGuid NOT IN ( SELECT remoteGuid FROM customer)").executeInsert();
    //
    // Log.i("Moving Temp Rows", "Customer end");
    // this.clearTempTable("customer_Temp");
    //
    // progress++;
    // publishProgress(progress);
    //
    // Log.i("Moving Temp Rows", "itemanalytictree start");
    // // removes duplicate rows
    // dbHelper.getWritableDatabase().compileStatement("DELETE FROM itemanalytictree_Temp WHERE ID > (SELECT min(ID) FROM itemanalytictree_Temp b WHERE remoteGuid = b.remoteGuid);").executeUpdateDelete();
    // dbHelper.getWritableDatabase().compileStatement("INSERT INTO itemanalytictree SELECT * FROM itemanalytictree_Temp WHERE remoteGuid NOT IN ( SELECT remoteGuid FROM itemanalytictree)").executeInsert();
    //
    // Log.i("Moving Temp Rows", "itemanalytictree end");
    // this.clearTempTable("itemanalytictree_Temp");
    //
    // progress++;
    // publishProgress(progress);
    //
    // Log.i("Moving Temp Rows", "barcode start");
    // dbHelper.getWritableDatabase().compileStatement("INSERT INTO barcode SELECT * FROM barcode_Temp WHERE remoteGuid NOT IN ( SELECT remoteGuid FROM barcode)").executeInsert();
    // //
    // dbHelper.getWritableDatabase().execSQL("update barcode set itemRemoteGuid=b.itemRemoteGuid,measurementUnitRemoteGuid=b.measurementUnitRemoteGuid,Code=b.Code,UpdatedOn=b.UpdatedOn"+
    // // " from barcode_Temp b , barcode a " +
    // // "WHERE a.remoteGuid = b.remoteGuid");
    // Log.i("Moving Temp Rows", "barcode end");
    // this.clearTempTable("barcode_Temp");
    //
    // progress++;
    // publishProgress(progress);
    //
    // Log.i("Moving Temp Rows", "itemcategory start");
    // dbHelper.getWritableDatabase().compileStatement("INSERT INTO itemcategory SELECT * FROM itemcategory_Temp WHERE remoteGuid NOT IN ( SELECT remoteGuid FROM itemcategory)").executeInsert();
    // //
    // dbHelper.getWritableDatabase().execSQL("update itemcategory set remoteParentGuid=b.remoteParentGuid,Name=b.Name,Code=b.Code,UpdatedOn=b.UpdatedOn"+
    // // " from itemcategory_Temp b , itemcategory a " +
    // // "WHERE a.remoteGuid = b.remoteGuid");
    // Log.i("Moving Temp Rows", "itemcategory end");
    // this.clearTempTable("itemcategory_Temp");
    //
    // progress++;
    // publishProgress(progress);
    //
    // Log.i("Moving Temp Rows", "pricecatalog start");
    // dbHelper.getWritableDatabase().compileStatement("INSERT INTO pricecatalog SELECT * FROM pricecatalog_Temp WHERE remoteGuid NOT IN ( SELECT remoteGuid FROM pricecatalog)").executeInsert();
    // //
    // dbHelper.getWritableDatabase().execSQL("update pricecatalog set remoteParentGuid=b.remoteParentGuid,Name=b.Name,Code=b.Code,UpdatedOn=b.UpdatedOn"+
    // // " from pricecatalog_Temp b , pricecatalog a " +
    // // "WHERE a.remoteGuid = b.remoteGuid");
    // Log.i("Moving Temp Rows", "pricecatalog end");
    // this.clearTempTable("pricecatalog_Temp");
    //
    // progress++;
    // publishProgress(progress);
    //
    // Log.i("Moving Temp Rows", "pricecatalogdetail start");
    // dbHelper.getWritableDatabase().compileStatement("INSERT INTO pricecatalogdetail SELECT * FROM pricecatalogdetail_Temp WHERE remoteGuid NOT IN ( SELECT remoteGuid FROM pricecatalogdetail)").executeInsert();
    // //
    // dbHelper.getWritableDatabase().execSQL("update pricecatalogdetail set remoteParentGuid=b.remoteParentGuid,Name=b.Name,Code=b.Code,UpdatedOn=b.UpdatedOn"+
    // // " from pricecatalogdetail_Temp b , pricecatalogdetail a " +
    // // "WHERE a.remoteGuid = b.remoteGuid");
    // Log.i("Moving Temp Rows", "pricecatalogdetail end");
    // this.clearTempTable("pricecatalogdetail_Temp");
    //
    // progress++;
    // publishProgress(progress);
    // }
    // catch (Exception e)
    // {
    // e.printStackTrace();
    // }
    //
    // }
    //
    // private boolean SynchronizeItemImages(long iuo, User usr)
    // {
    // progDialogMessage = "Preparing request to update images from server ...";
    // HashSet<String> toRequest = new HashSet<String>();
    // publishProgress(0);
    // try
    // {
    // // List<Item> allItems = dbHelper.getItems().queryForAll();
    //
    // for (Item itm : dbHelper.getItems())
    // {
    // if (itm.getImageOid() == null)
    // itm = dbHelper.getItems().queryForId(itm.getID());
    // String filename = MainActivity.IMAGES_PATH + "32/img_" +
    // itm.getImageOid().replace("-", "").toLowerCase();
    // if (itm.getUpdatedOn() >= iuo)
    // toRequest.add(itm.getImageOid().replace("-", "").toLowerCase());
    // else
    // {
    // File file = new File(filename);
    // if (!file.exists())
    // toRequest.add(itm.getImageOid().replace("-", "").toLowerCase());
    // }
    // }
    // }
    // catch (Exception ex)
    // {
    // Log.w("SynchronizeItemImages", "error in database");
    // return false;
    // }
    //
    // // //////
    // StringArraySerializer toSend = new StringArraySerializer();
    // toSend.addAll(toRequest);
    // // String [] toSend = toRequest.toArray(new String[0]);
    // HashMap parameters = new HashMap();
    //
    // parameters.put("userID", usr.getRemoteGuid());
    // parameters.put("imageGuids", toSend);
    //
    // String url = ((MainActivity) mContext).appsettings.getServiceUrl();
    // int oldTimeout = TIMEOUT;
    // TIMEOUT = 100000;
    // progDialogMessage = "Requesting images update from server ...";
    // publishProgress(0);
    // Object filenameobj = SynchronousCallWebMethod("GetImagesLink",
    // parameters);
    // TIMEOUT = oldTimeout;
    // if (filenameobj == null)
    // {
    // Log.i("SynchronizerText", "Filename problem");
    // return false;
    // }
    // String filename = filenameobj.toString();
    //
    // url = url.toLowerCase().replace("retailservice.asmx", "Downloads/" +
    // filename);
    // String outputFile = Environment.getExternalStorageDirectory().toString()
    // + "/retail/images/imgs.zip";
    // progDialogMessage = "Download update file...";
    // publishProgress(0);
    // if (downloadFile(url, outputFile))
    // {
    // // MainActivity.IMAGES_PATH
    // unpackZip(Environment.getExternalStorageDirectory().toString() +
    // "/retail/images/", "imgs.zip");
    // parameters = new HashMap<String, Object>();
    // parameters.put("userID", usr.getRemoteGuid());
    // parameters.put("filename", filename);
    // SynchronousCallWebMethod("CleanFile", parameters);
    //
    // return true;
    // }
    // return false;
    // }
    //
    // private boolean downloadFile(String url, String outputFile)
    // {
    // try
    // {
    // progDialogMessage = "Downloading File";
    // URL u = new URL(url);
    // URLConnection conn = u.openConnection();
    // int contentLength = conn.getContentLength(), current = 0;
    //
    // progDialog.setMax(contentLength);
    // publishProgress(current);
    //
    // File file = new File(outputFile.substring(0,
    // outputFile.lastIndexOf('/')));
    // file.mkdirs();
    //
    // DataInputStream stream = new DataInputStream(u.openStream());
    // DataOutputStream fos = new DataOutputStream(new
    // FileOutputStream(outputFile));
    //
    // byte[] buffer = new byte[32768];
    // int len, ct = 0;
    // while ((len = stream.read(buffer)) != -1)
    // {
    // fos.write(buffer, 0, len);
    // fos.flush();
    // current += len;
    // ct++;
    // if (ct % 20 == 0)
    // publishProgress(current);
    // }
    // // stream.readFully(buffer);
    // stream.close();
    //
    // // fos.write(buffer);
    // // fos.flush();
    // fos.close();
    // return true;
    // }
    // catch (FileNotFoundException e)
    // {
    // errorMessage = e.getMessage();
    // return false; // swallow a 404
    // }
    // catch (IOException e)
    // {
    // errorMessage = e.getMessage();
    // return false; // swallow a 404
    // }
    // }
    //
    // private boolean SynchronizeUserStoreAccessOfUser(User user)
    // {
    // try
    // {
    // DatabaseConnection conn =
    // dbHelper.getConnectionSource().getReadWriteConnection();
    // HashMap<String, Object> params;
    // params = new HashMap<String, Object>();
    // params.put("userID", user.getRemoteGuid());
    // Object result = callWebMethod("getUserStoreAccess", params);
    // SoapObject sresult = (SoapObject) result;
    // int limit = sresult.getPropertyCount();
    // for (int i = 0; i < limit; i++)
    // {
    // SoapObject obj = (SoapObject) sresult.getProperty(i);
    // UserStoreAccess ct;
    // Store store = dbHelper.findObject(Store.class, "remoteGuid",
    // obj.getProperty("StoreID"), FilterType.EQUALS);
    // if (store != null)
    // {
    // ct = dbHelper.findObject(UserStoreAccess.class, OperatorType.AND, new
    // BinaryOperator("store_id", store.getID(), FilterType.EQUALS), new
    // BinaryOperator("user_id", user.getID(), FilterType.EQUALS));
    // boolean newct;
    // if (ct == null)
    // {
    // newct = true;
    // ct = new UserStoreAccess();
    // }
    // else
    // {
    // newct = false;
    // }
    // ct.setUpdatedOn(Long.parseLong(obj.getProperty("UpdatedOn").toString()));
    // ct.setStore(dbHelper.getStoreByRemoteGuid(obj.getProperty("StoreID").toString()));
    // ct.setUser(dbHelper.getUserByRemoteGuid(obj.getProperty("UserID").toString()));
    //
    // if (newct)
    // dbHelper.getUserStoreAccessDao().create(ct);
    // else
    // dbHelper.getUserStoreAccessDao().update(ct);
    // if (i % COMMIT_SIZE == 0)
    // {
    // conn.setAutoCommit(true);
    // conn.setAutoCommit(false);
    // }
    // }
    //
    // }
    // Log.i("Sync UserStoreAccesses", limit + " completed successfully");
    // conn.setAutoCommit(true);
    // return true;
    // }
    // catch (Exception ex)
    // {
    // ex.printStackTrace();
    // return false;
    // }
    // }
    //
    // private boolean unpackZip(String path, String zipname)
    // {
    // InputStream is;
    // ZipInputStream zis;
    // try
    // {
    // String filename;
    // is = new FileInputStream(path + zipname);
    // zis = new ZipInputStream(new BufferedInputStream(is));
    // ZipEntry ze;
    // byte[] buffer = new byte[32768];
    // int count;
    //
    // while ((ze = zis.getNextEntry()) != null)
    // {
    //
    // filename = ze.getName();
    // String totalpath = path + filename;
    // totalpath = totalpath.substring(0, totalpath.lastIndexOf('/'));
    // File ffout = new File(totalpath);
    // ffout.mkdirs();
    // FileOutputStream fout = new FileOutputStream(path + filename);
    // progDialogMessage = "Unpacking...";
    // progDialog.setMax((int) ze.getSize());
    // int total = 0, ct = 0;
    // publishProgress(total);
    //
    // while ((count = zis.read(buffer)) != -1)
    // {
    // fout.write(buffer, 0, count);
    // // fout.flush();
    // total += count;
    // ct++;
    // if (ct % 400 == 0)
    // publishProgress(total);
    // }
    //
    // fout.close();
    // zis.closeEntry();
    // }
    //
    // zis.close();
    // }
    // catch (IOException e)
    // {
    // e.printStackTrace();
    // return false;
    // }
    //
    // return true;
    // }
    //
    // private String progDialogMessage = "";
    //
    // @Override
    // protected void onPreExecute()
    // {
    // super.onPreExecute();
    // progDialog = new ProgressDialog(mContext);
    // progDialog.setProgressStyle(ProgressDialog.STYLE_HORIZONTAL);
    // progDialog.setMessage(progDialogMessage);
    // progDialog.setCancelable(false);
    // ((MainActivity)
    // mContext).getWindow().addFlags(WindowManager.LayoutParams.FLAG_KEEP_SCREEN_ON);
    // }
    // }

    // class Synchronizer extends AsyncTask<User, Integer, Object>
    // {
    // private static final int MAX_ITEMS_TO_UPDATE = 50000;
    //
    // // private int currentRecords, totalRecords;
    //
    // @Override
    // protected void onProgressUpdate(Integer... values)
    // {
    // if (currentRecords > totalRecords)
    // {
    // currentRecords = totalRecords;
    // }
    // // TODO Auto-generated method stub
    // // super.onProgressUpdate(values);
    // // setProgressPercent(values[0]);
    // if (progDialog == null)
    // {
    // progDialog = new ProgressDialog(mContext);
    // progDialog.setProgressStyle(ProgressDialog.STYLE_HORIZONTAL);
    // progDialog.setMessage(mContext.getResources().getString(R.string.syncWaitMessage));
    // progDialog.setCancelable(false);
    // }
    //
    // if (!progDialog.isShowing())
    // {
    // progDialog.show();
    // }
    // progDialog.setProgress(values[0]);
    // }
    //
    // @Override
    // protected void onPostExecute(Object result)
    // {
    // super.onPostExecute(result);
    // progDialog.dismiss();
    // ((MainActivity)
    // mContext).getWindow().clearFlags(WindowManager.LayoutParams.FLAG_KEEP_SCREEN_ON);
    //
    // if (result != null && result instanceof Exception)
    // {
    // Utilities.ShowSimpleDialog(mContext, "An Error Occured: " + ((Exception)
    // result).getMessage() + ". Please Try again.");
    // }
    // else
    // {
    // ((MainActivity) mContext).btnOrders.setEnabled(true);
    // ((MainActivity) mContext).foundCustomers.clear();
    // ((MainActivity) mContext).UpdateCustomers(false);
    // Utilities.ShowUninterruptingMessage(mContext,
    // mContext.getString(R.string.updateCompletedSuccessfully), 5);
    // }
    // }
    //
    // ProgressDialog progDialog;
    //
    // @Override
    // protected void onPreExecute()
    // {
    // super.onPreExecute();
    // progDialog = new ProgressDialog(mContext);
    // progDialog.setProgressStyle(ProgressDialog.STYLE_HORIZONTAL);
    // progDialog.setMessage(mContext.getResources().getString(R.string.syncWaitMessage));
    // progDialog.setCancelable(false);
    // ((MainActivity)
    // mContext).getWindow().addFlags(WindowManager.LayoutParams.FLAG_KEEP_SCREEN_ON);
    // }
    //
    // @Override
    // protected Object doInBackground(User... params)
    // {
    // dbHelper.getWritableDatabase().rawQuery("PRAGMA synchronous=1  ;", null);
    //
    // Integer[] values = { 0 };
    // publishProgress(values);
    // try
    // {
    // try
    // {
    // totalRecords =
    // getTotalUpdates(dbHelper.getMaxCustomerUpdatedOn(params[0]),
    // dbHelper.getMaxItemUpdatedOn(),
    // dbHelper.getMaxItemAnalyticTreesUpdatedOn(),
    // dbHelper.getMaxBarcodeUpdatedOn(),
    // dbHelper.getMaxItemCategoriesUpdatedOn(), dbHelper.getMaxPriceCatalog(),
    // dbHelper.getMaxPriceCatalogDetail(), dbHelper.getMaxVatCategory(),
    // dbHelper.getMaxVatFactor(), dbHelper.getMaxVatLevel(),
    // dbHelper.getMaxOffersUpdatedOn(), dbHelper.getMaxOfferDetailsUpdatedOn(),
    // dbHelper.getMaxDocumentStatusUpdatedOn(),
    // dbHelper.getMaxStoresUpdatedOn(), dbHelper.getMaxLinkedItem(),
    // dbHelper.getMaxMeasurementUnitUpdatedOn());
    // }
    // catch (Exception e)
    // {
    // Log.e("getTotalUpdates", "GetTotalUpdates Failed");
    // throw new Exception("GetTotalUpdates Failed");
    // }
    // if (totalRecords == 0)
    // {
    // publishProgress(100);
    // return null;
    // }
    // currentRecords = 0;
    // if (!SynchronizeApplicationSettings(params[0]))
    // {
    // Log.e("SynchronizeApplicationSettings",
    // "SynchronizeApplicationSettings Failed");
    // throw new Exception("SynchronizeApplicationSettings Failed");
    // }
    // if (!SynchronizeStores(params[0]))
    // {
    // Log.e("SynchronizeStores", "SynchronizeStores Failed");
    // throw new Exception("SynchronizeStores Failed");
    // }
    //
    // if (!SynchronizeVATs(params[0]))
    // {
    // Log.e("SynchronizeVATs", "SynchronizeVATs Failed");
    // throw new Exception("SynchronizeVATs Failed");
    // }
    // if (!SynchronizePriceCatalogs(params[0]))
    // {
    // Log.e("SynchronizePriceCatalogs", "SynchronizePriceCatalogs Failed");
    // throw new Exception("SynchronizePriceCatalogs Failed");
    // }
    // if (!SynchronizeCustomers(params[0]))
    // {
    // Log.e("SynchronizeCustomers", "SynchronizeCustomers Failed");
    // throw new Exception("SynchronizeCustomers Failed");
    // }
    //
    // if (!SynchronizeUserStoreAccessOfUser(params[0]))
    // {
    // Log.e("SynchronizeUserStoreAccessOfUser",
    // "SynchronizeUserStoreAccessOfUser Failed");
    // throw new Exception("SynchronizeUserStoreAccessOfUser Failed");
    // }
    // // List<Customer> list = GetCustomersOfUser(params[0], true);
    // // currentRecords += list.size();
    //
    // if (currentRecords > totalRecords)
    // currentRecords = totalRecords;
    // values[0] = (99 * currentRecords) / totalRecords;
    // publishProgress(values);
    //
    // int res = SynchronizeStatus();
    // if (res < 0)
    // {
    // Log.e("SynchronizeStatus", "SynchronizeStatus Failed");
    // throw new Exception("SynchronizeStatus Failed");
    // }
    // currentRecords += res;
    // if (currentRecords > totalRecords)
    // currentRecords = totalRecords;
    // values[0] = (99 * currentRecords) / totalRecords;
    // publishProgress(values);
    //
    // Log.d("ItemCategories Start", Calendar.getInstance().toString());
    // res = SynchronizeItemCategories(params[0]);
    // Log.d("ItemCategories Stop", Calendar.getInstance().toString());
    // if (res < 0)
    // Log.e("Sync", "Status Problem");
    // if (currentRecords > totalRecords)
    // currentRecords = totalRecords;
    // values[0] = (99 * currentRecords) / totalRecords;
    // publishProgress(values);
    //
    // res = SynchronizeItems(params[0]);
    // if (res < 0)
    // {
    // Log.e("SynchronizeItems", "SynchronizeItems Failed");
    // throw new Exception("SynchronizeItems Failed");
    // }
    // // currentRecords += res;
    // res = SynchronizeItemLinkedItems(params[0]);
    // if (res < 0)
    // {
    // Log.e("SynchronizeItemLinkedItems", "SynchronizeItemLinkedItems Failed");
    // throw new Exception("SynchronizeItemLinkedItems Failed");
    // }
    // // values[0] = 100;
    // // publishProgress(values);
    //
    // res = SynchronizeItemAnalyticTrees(params[0]);
    // if (res < 0)
    // {
    // Log.e("SynchronizeItemLinkedItems", "SynchronizeItemLinkedItems Failed");
    // throw new Exception("SynchronizeItemLinkedItems Failed");
    // }
    //
    // if (!SynchronizePriceCatalogDetails(params[0]))
    // {
    // Log.e("SynchronizePriceCatalogDetails",
    // "SynchronizePriceCatalogDetails Failed");
    // throw new Exception("SynchronizePriceCatalogDetails Failed");
    // }
    //
    // if (!SynchronizeOffers(params[0]))
    // {
    // Log.e("SynchronizeOffers", "SynchronizeOffers Failed");
    // throw new Exception("SynchronizeOffers Failed");
    // }
    //
    // if (!SynchronizeOfferDetails(params[0]))
    // {
    // Log.e("SynchronizeOfferDetails", "SynchronizeOfferDetails Failed");
    // throw new Exception("SynchronizeOfferDetails Failed");
    // }
    //
    // values[0] = 100;
    // publishProgress(values);
    //
    // // android activity shared preference. It stores a persistent
    // // bool that idicates if the db has been initialized
    // ((MainActivity)
    // mContext).getPreferences(mContext.MODE_PRIVATE).edit().putBoolean("init",
    // true).commit();
    //
    // ((MainActivity) mContext).isDBInitialized = true;
    // // dbHelper.getWritableDatabase().rawQuery("PRAGMA shrink_memory;",
    // // null);
    //
    // }
    // catch (Exception e)
    // {
    // e.printStackTrace();
    // return e;
    // // Utilities.ShowUninterruptingMessage(mContext,"An Error Occured: "
    // // + e.getMessage(),15);
    // }
    // return null;
    // }
    //
    // private boolean downloadDatabase(String sUrl, String outputFile) throws
    // Exception
    // {
    // URL url = new URL(sUrl);
    // URLConnection connection = url.openConnection();
    // connection.connect();
    // InputStream input = new BufferedInputStream(url.openStream());
    // OutputStream output = new FileOutputStream(outputFile);
    //
    // byte data[] = new byte[1024];
    // long total = 0;
    // int count;
    // while ((count = input.read(data)) != -1)
    // {
    // total += count;
    // output.write(data, 0, count);
    // }
    //
    // output.flush();
    // output.close();
    // input.close();
    //
    // return true;
    // }
    //
    // private boolean SynchronizeStores(User user)
    // {
    // Integer[] values = { 0 };
    // try
    // {
    // // Stores
    // DatabaseConnection conn =
    // dbHelper.getConnectionSource().getReadWriteConnection();
    // HashMap<String, Object> params = new HashMap<String, Object>();
    // params.put("updatedon", "" + dbHelper.getMaxStoresUpdatedOn());
    // params.put("userID", user.getRemoteGuid());
    // Object result = callWebMethod("getStores", params);
    // SoapObject sresult = (SoapObject) result;
    // int limit = sresult.getPropertyCount();
    // conn.setAutoCommit(false);
    // for (int i = 0; i < limit; i++)
    // {
    // SoapObject obj = (SoapObject) sresult.getProperty(i);
    // Store ct;
    // ct = dbHelper.getStoreByRemoteGuid(obj.getProperty("Oid").toString());
    // boolean newct;
    // if (ct == null)
    // {
    // newct = true;
    // ct = new Store();
    // }
    // else
    // {
    // newct = false;
    // }
    // ct.setUpdatedOn(Long.parseLong(obj.getProperty("UpdatedOn").toString()));
    // ct.setRemoteGuid(obj.getProperty("Oid").toString());
    // ct.setCentralStore(Boolean.parseBoolean(obj.getProperty("IsCentralStore").toString()));
    // ct.setCode(obj.getProperty("Code").toString());
    // ct.setName(obj.getProperty("Name").toString());
    // if (newct)
    // dbHelper.getStoresDao().create(ct);
    // else
    // dbHelper.getStoresDao().update(ct);
    // currentRecords++;
    // if (currentRecords > totalRecords)
    // currentRecords = totalRecords;
    // if (i % COMMIT_SIZE == 0)
    // {
    // values[0] = (99 * currentRecords) / totalRecords;
    // publishProgress(values);
    // conn.setAutoCommit(true);
    // conn.setAutoCommit(false);
    // }
    //
    // }
    // Log.i("Sync Stores", limit + " completed successfully");
    // values[0] = (99 * currentRecords) / totalRecords;
    // publishProgress(values);
    // conn.setAutoCommit(true);
    // return true;
    // }
    // catch (Exception ex)
    // {
    // ex.printStackTrace();
    // return false;
    // }
    // }
    //
    // private boolean SynchronizeUserStoreAccessOfUser(User user)
    // {
    // Integer[] values = { 0 };
    // try
    // {
    // // usa
    // DatabaseConnection conn =
    // dbHelper.getConnectionSource().getReadWriteConnection();
    // HashMap<String, Object> params = new HashMap<String, Object>();
    // params = new HashMap<String, Object>();
    // params.put("userID", user.getRemoteGuid());
    // Object result = callWebMethod("getUserStoreAccess", params);
    // SoapObject sresult = (SoapObject) result;
    // int limit = sresult.getPropertyCount();
    // conn.setAutoCommit(false);
    // for (int i = 0; i < limit; i++)
    // {
    // SoapObject obj = (SoapObject) sresult.getProperty(i);
    // UserStoreAccess ct;
    // Store store = dbHelper.findObject(Store.class, "remoteGuid",
    // obj.getProperty("StoreID"), FilterType.EQUALS);
    // if (store != null)
    // {
    // ct = dbHelper.findObject(UserStoreAccess.class, OperatorType.AND, new
    // BinaryOperator("store_id", store.getID(), FilterType.EQUALS), new
    // BinaryOperator("user_id", user.getID(), FilterType.EQUALS));
    // boolean newct;
    // if (ct == null)
    // {
    // newct = true;
    // ct = new UserStoreAccess();
    // }
    // else
    // {
    // newct = false;
    // }
    // ct.setUpdatedOn(Long.parseLong(obj.getProperty("UpdatedOn").toString()));
    // ct.setStore(dbHelper.getStoreByRemoteGuid(obj.getProperty("StoreID").toString()));
    // ct.setUser(dbHelper.getUserByRemoteGuid(obj.getProperty("UserID").toString()));
    //
    // if (newct)
    // dbHelper.getUserStoreAccessDao().create(ct);
    // else
    // dbHelper.getUserStoreAccessDao().update(ct);
    // if (i % COMMIT_SIZE == 0)
    // {
    // conn.setAutoCommit(true);
    // conn.setAutoCommit(false);
    // }
    // }
    //
    // }
    // Log.i("Sync UserStoreAccesses", limit + " completed successfully");
    // conn.setAutoCommit(true);
    // return true;
    // }
    // catch (Exception ex)
    // {
    // ex.printStackTrace();
    // return false;
    // }
    // }
    //
    // // UNUSED & UNFINISHED, USERS ARE NOT STORED LOCALLY
    // private boolean SynchronizeUserStoreAccess(User user)
    // {
    // Integer[] values = { 0 };
    // try
    // {
    // // Stores
    // DatabaseConnection conn =
    // dbHelper.getConnectionSource().getReadWriteConnection();
    // HashMap<String, Object> params = new HashMap<String, Object>();
    // params.put("updatedon", "" + dbHelper.getMaxUserStoreAccessUpdatedOn());
    // params.put("userID", user.getRemoteGuid());
    // Object result = callWebMethod("getAllUserStoreAccess", params);
    // SoapObject sresult = (SoapObject) result;
    // int limit = sresult.getPropertyCount();
    // conn.setAutoCommit(false);
    // for (int i = 0; i < limit; i++)
    // {
    // SoapObject obj = (SoapObject) sresult.getProperty(i);
    // UserStoreAccess ct;
    // ct = dbHelper.findObject(UserStoreAccess.class, OperatorType.AND, new
    // BinaryOperator("store_id", obj.getProperty("StoreID").toString(),
    // FilterType.EQUALS), new BinaryOperator("user_id",
    // obj.getProperty("UserID").toString(), FilterType.EQUALS));
    // boolean newct;
    // if (ct == null)
    // {
    // newct = true;
    // ct = new UserStoreAccess();
    // }
    // else
    // {
    // newct = false;
    // }
    // ct.setUpdatedOn(Long.parseLong(obj.getProperty("UpdatedOn").toString()));
    // ct.setStore(dbHelper.getStoreByRemoteGuid(obj.getProperty("StoreID").toString()));
    // ct.setUser(dbHelper.getUserByRemoteGuid(obj.getProperty("UserID").toString()));
    //
    // if (newct)
    // dbHelper.getUserStoreAccessDao().create(ct);
    // else
    // dbHelper.getUserStoreAccessDao().update(ct);
    // currentRecords++;
    // if (currentRecords > totalRecords)
    // currentRecords = totalRecords;
    // if (i % COMMIT_SIZE == 0)
    // {
    // values[0] = (99 * currentRecords) / totalRecords;
    // publishProgress(values);
    // conn.setAutoCommit(true);
    // conn.setAutoCommit(false);
    // }
    //
    // }
    // Log.i("Sync Stores", limit + " completed successfully");
    // values[0] = (99 * currentRecords) / totalRecords;
    // publishProgress(values);
    // conn.setAutoCommit(true);
    // return true;
    // }
    // catch (Exception ex)
    // {
    // ex.printStackTrace();
    // return false;
    // }
    // }
    //
    // private boolean SynchronizePriceCatalogs(User user)
    // {
    // Integer[] values = { 0 };
    // try
    // {
    // // VATCategory
    // DatabaseConnection conn =
    // dbHelper.getConnectionSource().getReadWriteConnection();
    // HashMap<String, Object> params = new HashMap<String, Object>();
    // params.put("updatedon", "" + dbHelper.getMaxPriceCatalog());
    // params.put("userID", user.getRemoteGuid());
    // Object result = callWebMethod("getPriceCatalogs", params);
    // SoapObject sresult = (SoapObject) result;
    // int limit = sresult.getPropertyCount();
    // ArrayList<PriceCatalog> noParentYetAvailable = new
    // ArrayList<PriceCatalog>();
    // conn.setAutoCommit(false);
    // for (int i = 0; i < limit; i++)
    // {
    // SoapObject obj = (SoapObject) sresult.getProperty(i);
    // PriceCatalog ct;
    // ct =
    // dbHelper.getPriceCatalogByRemoteGuid(obj.getProperty("Oid").toString());
    // boolean newct;
    // if (ct == null)
    // {
    // newct = true;
    // ct = new PriceCatalog();
    // // ct.setID(Long.randomLong());
    // }
    // else
    // {
    // newct = false;
    // }
    // ct.setName(obj.getProperty("Description").toString());
    // ct.setCode((obj.getProperty("Code").toString()));
    // ct.setRemoteGuid(obj.getProperty("Oid").toString());
    // ct.setUpdatedOn(Long.parseLong(obj.getProperty("UpdatedOn").toString()));
    //
    // String parentOid = obj.getProperty("parentOid").toString();
    // ct.setRemoteParentGuid(parentOid);
    // if (!parentOid.equals("00000000-0000-0000-0000-000000000000"))
    // {
    // PriceCatalog parent = dbHelper.getPriceCatalogByRemoteGuid(parentOid);
    // if (parent != null)
    // {
    // ct.setParent(parent);
    // }
    // else
    // {
    // noParentYetAvailable.add(ct);
    // }
    //
    // }
    //
    // if (newct)
    // dbHelper.getPriceCatalogDao().create(ct);
    // else
    // dbHelper.getPriceCatalogDao().update(ct);
    // currentRecords++;
    // if (currentRecords > totalRecords)
    // currentRecords = totalRecords;
    // if (i % COMMIT_SIZE == 0)
    // {
    // values[0] = (99 * currentRecords) / totalRecords;
    // publishProgress(values);
    // conn.setAutoCommit(true);
    // conn.setAutoCommit(false);
    // }
    //
    // }
    // values[0] = (99 * currentRecords) / totalRecords;
    // publishProgress(values);
    // conn.setAutoCommit(true);
    // conn.setAutoCommit(false);
    // for (int i = 0; i < noParentYetAvailable.size(); i++)
    // {
    // PriceCatalog toproc = noParentYetAvailable.get(i), parent;
    // parent =
    // dbHelper.getPriceCatalogByRemoteGuid(toproc.getRemoteParentGuid());
    // if (parent != null)
    // {
    // toproc.setParent(parent);
    // dbHelper.getPriceCatalogDao().update(toproc);
    //
    // }
    // }
    // values[0] = (99 * currentRecords) / totalRecords;
    // publishProgress(values);
    // conn.setAutoCommit(true);
    // return true;
    // }
    // catch (Exception ex)
    // {
    // ex.printStackTrace();
    // return false;
    // }
    // }
    //
    // private boolean SynchronizeCustomers(User user)
    // {
    // Integer[] values = { 0 };
    // boolean returnValue;
    // try
    // {
    // // Customers
    // DatabaseConnection conn =
    // dbHelper.getConnectionSource().getReadWriteConnection();
    // HashMap<String, Object> params = new HashMap<String, Object>();
    // boolean repeat;
    // long total = 0;
    // long maxupdatedOn = dbHelper.getMaxCustomerUpdatedOn(), olMaxUpdatedOn =
    // -1;
    //
    // do
    // {
    // params.put("updatedOn", "" + maxupdatedOn);
    // params.put("userID", user.getRemoteGuid());
    // Object result = callWebMethod("GetCustomers", params);
    // SoapObject sresult = (SoapObject) result;
    // int limit = sresult.getPropertyCount();
    // repeat = false;
    // conn.setAutoCommit(false);
    //
    // if (maxupdatedOn == olMaxUpdatedOn)
    // break;
    // olMaxUpdatedOn = maxupdatedOn;
    // for (int i = 0; i < limit; i++)
    // {
    // repeat = true;
    // SoapObject obj = (SoapObject) sresult.getProperty(i);
    // Customer ct;
    // ct = dbHelper.getCustomerByRemoteGuid(obj.getProperty("Oid").toString());
    // boolean newct;
    // if (ct == null)
    // {
    // newct = true;
    // ct = new Customer();
    // // ct.setID(Long.randomLong());
    // }
    // else
    // {
    // newct = false;
    // }
    //
    // ct.setCompanyName(obj.getProperty("CompanyName").toString());
    // ct.setCode(obj.getProperty("Code").toString());
    // ct.setRemoteGuid(obj.getProperty("Oid").toString());
    // ct.setDefaultAddress(obj.getProperty("DefaultAddress").toString());
    // ct.setDefaultPhone(obj.getProperty("DefaultPhone").toString());
    // ct.setTaxCode(obj.getProperty("TaxCode").toString());
    // ct.setUpdatedOn(Long.parseLong(obj.getProperty("UpdateOn").toString()));
    // ct.setPcRemoteGuid(obj.getPropertyAsString("PriceListOid"));
    // long parsedUpdatedOn =
    // Long.parseLong(obj.getProperty("UpdateOn").toString());
    // Store store = dbHelper.findObject(Store.class, "remoteGuid",
    // obj.getProperty("StoreOid").toString(), FilterType.EQUALS);
    // ct.setStore(store);
    // // ct.setOwner(user);
    // VATLevel vl =
    // dbHelper.getVatLevelByRemoteGuid(obj.getPropertyAsString("VatLevel"));
    // ct.setVl(vl);
    // // PriceCatalog pc =
    // //
    // dbHelper.getPriceCatalogByRemoteGuid(obj.getPropertyAsString("PriceListOid"));
    // // (obj.getProperyAsString("PriceListOid"));
    // // ct.setPc(pc);
    // if (newct)
    // dbHelper.getCustomers().create(ct);
    // else
    // dbHelper.getCustomers().update(ct);
    //
    // currentRecords++;
    // if (currentRecords > totalRecords)
    // currentRecords = totalRecords;
    // if (i % COMMIT_SIZE == 0)
    // {
    // conn.setAutoCommit(true);
    // conn.setAutoCommit(false);
    // values[0] = (99 * currentRecords) / totalRecords;
    // publishProgress(values);
    //
    // }
    // if (maxupdatedOn < parsedUpdatedOn)
    // maxupdatedOn = parsedUpdatedOn;
    // }
    // conn.setAutoCommit(true);
    // total += limit;
    //
    // Log.i("Sync Customers", limit + " completed successfully");
    // values[0] = (99 * currentRecords) / totalRecords;
    // publishProgress(values);
    // conn.setAutoCommit(true);
    // }
    // while (repeat);
    // returnValue = true;
    // }
    // catch (Exception ex)
    // {
    // ex.printStackTrace();
    // returnValue = false;
    // }
    // String query =
    // "update customer set pc_id = (select id from PriceCatalog where PriceCatalog.remoteGuid = Customer.pcRemoteGuid)";
    // Log.i("Sync Items", "Executing: " + query);
    // dbHelper.getWritableDatabase().execSQL(query);
    // return returnValue;
    // }
    //
    // private boolean SynchronizeOffers(User user)
    // {
    // Integer[] values = { 0 };
    // try
    // {
    // // Offer
    // DatabaseConnection conn =
    // dbHelper.getConnectionSource().getReadWriteConnection();
    // HashMap<String, Object> params = new HashMap<String, Object>();
    // params.put("updatedon", "" + dbHelper.getMaxOffersUpdatedOn());
    // params.put("userID", user.getRemoteGuid());
    // Object result = callWebMethod("getOffers", params);
    // SoapObject sresult = (SoapObject) result;
    // int limit = sresult.getPropertyCount();
    // conn.setAutoCommit(false);
    // for (int i = 0; i < limit; i++)
    // {
    // SoapObject obj = (SoapObject) sresult.getProperty(i);
    // Offer ct;
    // ct = dbHelper.getOfferByRemoteGuid(obj.getProperty("Oid").toString());
    // boolean newct;
    // if (ct == null)
    // {
    // newct = true;
    // ct = new Offer();
    // }
    // else
    // {
    // newct = false;
    // }
    // ct.setActive(Boolean.parseBoolean(obj.getProperty("isActive").toString()));
    // ct.setDescription(obj.getProperty("Description").toString());
    // ct.setDescription2(obj.getProperty("Description2").toString());
    // ct.setUpdatedOn(Long.parseLong(obj.getProperty("UpdatedOn").toString()));
    // ct.setEndDate(Long.parseLong(obj.getProperty("EndDate").toString()));
    // ct.setStartDate(Long.parseLong(obj.getProperty("StartDate").toString()));
    // ct.setPriceCatalogRemoteGuid(obj.getProperty("PriceCatalogOid").toString());
    // ct.setPriceCatalog(dbHelper.getPriceCatalogByRemoteGuid(ct.getPriceCatalogRemoteGuid()));
    // ct.setRemoteGuid(obj.getProperty("Oid").toString());
    //
    // if (newct)
    // dbHelper.getOffersDao().create(ct);
    // else
    // dbHelper.getOffersDao().update(ct);
    // currentRecords++;
    // if (currentRecords > totalRecords)
    // currentRecords = totalRecords;
    // if (i % COMMIT_SIZE == 0)
    // {
    // values[0] = (99 * currentRecords) / totalRecords;
    // publishProgress(values);
    // conn.setAutoCommit(true);
    // conn.setAutoCommit(false);
    // }
    //
    // }
    // Log.i("Sync Offers", limit + " completed successfully");
    // values[0] = (99 * currentRecords) / totalRecords;
    // publishProgress(values);
    // conn.setAutoCommit(true);
    // return true;
    // }
    // catch (Exception ex)
    // {
    // ex.printStackTrace();
    // return false;
    // }
    // }
    //
    // private boolean SynchronizeOfferDetails(User user)
    // {
    // Integer[] values = { 0 };
    // try
    // {
    // // OfferDetails
    // DatabaseConnection conn =
    // dbHelper.getConnectionSource().getReadWriteConnection();
    // HashMap<String, Object> params = new HashMap<String, Object>();
    // params.put("updatedon", "" + dbHelper.getMaxOfferDetailsUpdatedOn());
    // params.put("userID", user.getRemoteGuid());
    // Object result = callWebMethod("getOfferDetails", params);
    // SoapObject sresult = (SoapObject) result;
    // int limit = sresult.getPropertyCount();
    // conn.setAutoCommit(false);
    // for (int i = 0; i < limit; i++)
    // {
    // SoapObject obj = (SoapObject) sresult.getProperty(i);
    // OfferDetail ct;
    // ct =
    // dbHelper.getOfferDetailByRemoteGuid(obj.getProperty("Oid").toString());
    // boolean newct;
    // if (ct == null)
    // {
    // newct = true;
    // ct = new OfferDetail();
    // }
    // else
    // {
    // newct = false;
    // }
    // ct.setActive(Boolean.parseBoolean(obj.getProperty("isActive").toString()));
    // ct.setItemRemoteGuid(obj.getProperty("ItemOid").toString());
    // ct.setItem(dbHelper.getItemByRemoteGuid(ct.getItemRemoteGuid()));
    // ct.setOfferRemoteGuid(obj.getProperty("OfferOid").toString());
    // ct.setOffer(dbHelper.getOfferByRemoteGuid(ct.getOfferRemoteGuid()));
    // ct.setRemoteGuid(obj.getProperty("Oid").toString());
    // ct.setUpdatedOn(Long.parseLong(obj.getProperty("UpdatedOn").toString()));
    // ct.setRemoteGuid(obj.getProperty("Oid").toString());
    //
    // if (newct)
    // dbHelper.getOfferDetailsDao().create(ct);
    // else
    // dbHelper.getOfferDetailsDao().update(ct);
    // currentRecords++;
    // if (currentRecords > totalRecords)
    // currentRecords = totalRecords;
    // if (i % COMMIT_SIZE == 0)
    // {
    // values[0] = (99 * currentRecords) / totalRecords;
    // publishProgress(values);
    // conn.setAutoCommit(true);
    // conn.setAutoCommit(false);
    // }
    //
    // }
    // values[0] = (99 * currentRecords) / totalRecords;
    // publishProgress(values);
    // conn.setAutoCommit(true);
    // Log.i("Sync OfferDetails", limit + " completed successfully");
    // return true;
    // }
    // catch (Exception ex)
    // {
    // ex.printStackTrace();
    // return false;
    // }
    // }
    //
    // private boolean SynchronizeApplicationSettings(User user)
    // {
    // try
    // {
    // // VATCategory
    // HashMap<String, Object> params = new HashMap<String, Object>();
    //
    // params.put("userID", user.getRemoteGuid());
    // Object result = callWebMethod("getAppSettings", params);
    // SoapObject sresult = (SoapObject) result;
    // // int limit = sresult.getPropertyCount();
    // // for (int i = 0; i < limit; i++)
    // {
    // SoapObject obj = sresult;
    // ApplicationSettings ct;
    // ct = dbHelper.getApplicationSettings().queryForAll().get(0);
    // ct.setBarcodePadChar(obj.getPropertyAsString("BarcodePadChar").charAt(0));
    // ct.setCodePadChar(obj.getPropertyAsString("CodePadChar").charAt(0));
    //
    // ct.setBarcodePadding(Boolean.parseBoolean(obj.getPropertyAsString("BarcodePad")));
    // ct.setCodePadding(Boolean.parseBoolean(obj.getPropertyAsString("CodePad")));
    //
    // ct.setBarcodePadLength(Integer.parseInt(obj.getPropertyAsString("BarcodePadLength")));
    // ct.setCodePadLength(Integer.parseInt(obj.getPropertyAsString("CodePadLength")));
    //
    // ct.setComputeDigits(Integer.parseInt(obj.getPropertyAsString("ComputeDigits")));
    // ct.setComputeValueDigits(Integer.parseInt(obj.getPropertyAsString("ComputeValueDigits")));
    //
    // ct.setDisplayDigits(Integer.parseInt(obj.getPropertyAsString("DisplayDigits")));
    // ct.setDisplayValueDigits(Integer.parseInt(obj.getPropertyAsString("DisplayValueDigits")));
    //
    // dbHelper.getApplicationSettings().update(ct);
    // ((MainActivity) mContext).appsettings = ct;
    //
    // }
    // return true;
    // }
    // catch (Exception ex)
    // {
    // ex.printStackTrace();
    // }
    // return false;
    // }
    //
    // private boolean SynchronizeVATs(User user)
    // {
    // Integer[] values = { 0 };
    // try
    // {
    // // VATCategory
    // DatabaseConnection conn =
    // dbHelper.getConnectionSource().getReadWriteConnection();
    // HashMap<String, Object> params = new HashMap<String, Object>();
    // params.put("updatedon", "" + dbHelper.getMaxVatCategory());
    // params.put("userID", user.getRemoteGuid());
    // Object result = callWebMethod("getVatCategories", params);
    // SoapObject sresult = (SoapObject) result;
    // int limit = sresult.getPropertyCount();
    // conn.setAutoCommit(false);
    // for (int i = 0; i < limit; i++)
    // {
    // SoapObject obj = (SoapObject) sresult.getProperty(i);
    // VATCategory ct;
    // ct =
    // dbHelper.getVatCategoryByRemoteGuid(obj.getProperty("Oid").toString());
    // boolean newct;
    // if (ct == null)
    // {
    // newct = true;
    // ct = new VATCategory();
    // // ct.setID(Long.randomLong());
    // }
    // else
    // {
    // newct = false;
    // }
    // ct.setName(obj.getProperty("Description").toString());
    // ct.setRemoteGuid(obj.getProperty("Oid").toString());
    // ct.setUpdatedOn(Long.parseLong(obj.getProperty("UpdatedOn").toString()));
    // if (newct)
    // dbHelper.getVATCategoryDao().create(ct);
    // else
    // dbHelper.getVATCategoryDao().update(ct);
    // currentRecords++;
    // if (currentRecords > totalRecords)
    // currentRecords = totalRecords;
    // if (i % COMMIT_SIZE == 0)
    // {
    // values[0] = (99 * currentRecords) / totalRecords;
    // publishProgress(values);
    // conn.setAutoCommit(true);
    // conn.setAutoCommit(false);
    // }
    //
    // }
    // values[0] = (100 * currentRecords) / totalRecords;
    // publishProgress(values);
    // conn.setAutoCommit(true);
    //
    // // VATLevel
    // conn = dbHelper.getConnectionSource().getReadWriteConnection();
    // params = new HashMap<String, Object>();
    // params.put("updatedon", "" + dbHelper.getMaxVatLevel());
    // params.put("userID", user.getRemoteGuid());
    // result = callWebMethod("getVatLevels", params);
    // sresult = (SoapObject) result;
    // limit = sresult.getPropertyCount();
    // conn.setAutoCommit(false);
    // for (int i = 0; i < limit; i++)
    // {
    // SoapObject obj = (SoapObject) sresult.getProperty(i);
    // VATLevel ct;
    // ct = dbHelper.getVatLevelByRemoteGuid(obj.getProperty("Oid").toString());
    // boolean newct;
    // if (ct == null)
    // {
    // newct = true;
    // ct = new VATLevel();
    // // ct.setID(Long.randomLong());
    // }
    // else
    // {
    // newct = false;
    // }
    // ct.setName(obj.getProperty("Description").toString());
    // ct.setRemoteGuid(obj.getProperty("Oid").toString());
    // ct.setUpdatedOn(Long.parseLong(obj.getProperty("UpdatedOn").toString()));
    // ct.setDefault(Boolean.parseBoolean(obj.getProperty("IsDefault").toString()));
    // if (newct)
    // dbHelper.getVATLevelDao().create(ct);
    // else
    // dbHelper.getVATLevelDao().update(ct);
    // currentRecords++;
    // if (i % COMMIT_SIZE == 0)
    // {
    // if (currentRecords > totalRecords)
    // currentRecords = totalRecords;
    // values[0] = (99 * currentRecords) / totalRecords;
    // publishProgress(values);
    // conn.setAutoCommit(true);
    // conn.setAutoCommit(false);
    // }
    //
    // }
    // if (currentRecords > totalRecords)
    // currentRecords = totalRecords;
    // values[0] = (99 * currentRecords) / totalRecords;
    // publishProgress(values);
    // conn.setAutoCommit(true);
    //
    // // VATFactors
    // conn = dbHelper.getConnectionSource().getReadWriteConnection();
    // params = new HashMap<String, Object>();
    // params.put("updatedon", "" + dbHelper.getMaxVatLevel());
    // params.put("userID", user.getRemoteGuid());
    // result = callWebMethod("getVatFactors", params);
    // sresult = (SoapObject) result;
    // limit = sresult.getPropertyCount();
    // conn.setAutoCommit(false);
    // for (int i = 0; i < limit; i++)
    // {
    // SoapObject obj = (SoapObject) sresult.getProperty(i);
    // VATFactor ct;
    // ct =
    // dbHelper.getVatFactorByRemoteGuid(obj.getProperty("Oid").toString());
    // boolean newct;
    // if (ct == null)
    // {
    // newct = true;
    // ct = new VATFactor();
    // // ct.setID(Long.randomLong());
    // }
    // else
    // {
    // newct = false;
    // }
    // ct.setRemoteGuid(obj.getProperty("Oid").toString());
    // ct.setUpdatedOn(Long.parseLong(obj.getProperty("UpdatedOn").toString()));
    // ct.setVatFactor(Double.parseDouble(obj.getProperty("VatFactor").toString()));
    // VATCategory vc =
    // dbHelper.getVatCategoryByRemoteGuid(obj.getProperty("VatCategoryOid").toString());
    // ct.setVatCategory(vc);
    // VATLevel vl =
    // dbHelper.getVatLevelByRemoteGuid(obj.getProperty("VatLevelOid").toString());
    // ct.setVatLevel(vl);
    // if (newct)
    // dbHelper.getVATFactorDao().create(ct);
    // else
    // dbHelper.getVATFactorDao().update(ct);
    // currentRecords++;
    // if (currentRecords > totalRecords)
    // currentRecords = totalRecords;
    // if (i % COMMIT_SIZE == 0)
    // {
    // if (currentRecords > totalRecords)
    // currentRecords = totalRecords;
    // values[0] = (99 * currentRecords) / totalRecords;
    // publishProgress(values);
    // conn.setAutoCommit(true);
    // conn.setAutoCommit(false);
    // }
    // }
    // if (currentRecords > totalRecords)
    // currentRecords = totalRecords;
    // values[0] = (99 * currentRecords) / totalRecords;
    // publishProgress(values);
    // conn.setAutoCommit(true);
    // return true;
    // }
    // catch (Exception ex)
    // {
    // ex.printStackTrace();
    // return false;
    // }
    //
    // }
    //
    // private int SynchronizeItemAnalyticTrees(User user)
    // {
    // Integer[] values = { 0 };
    // try
    // {
    // DatabaseConnection conn =
    // dbHelper.getConnectionSource().getReadWriteConnection();
    // long maxupdatedOn = dbHelper.getMaxItemAnalyticTreesUpdatedOn(),
    // olMaxUpdatedOn = -1;
    // HashMap<String, Object> params;
    // boolean repeat;
    // do
    // {
    // if (maxupdatedOn == olMaxUpdatedOn)
    // break;
    // olMaxUpdatedOn = maxupdatedOn;
    //
    // params = new HashMap<String, Object>();
    // params.put("updatedon", "" + maxupdatedOn);
    // params.put("userID", user.getRemoteGuid());
    // repeat = false;
    // Object result = callWebMethod("getItemAnalyticTrees", params);
    // SoapObject sresult = (SoapObject) result;
    // int limit = sresult.getPropertyCount();
    // conn.setAutoCommit(false);
    // for (int i = 0; i < limit; i++)
    // {
    // repeat = true;
    // SoapObject obj = (SoapObject) sresult.getProperty(i);
    // ItemAnalyticTree itemAnalyticTree =
    // dbHelper.getItemAnalyticTreeByRemoteGuid(obj.getProperty("Oid").toString());
    // boolean newstatus = false;
    // if (itemAnalyticTree == null)
    // {
    // newstatus = true;
    // itemAnalyticTree = new ItemAnalyticTree();
    // // itemAnalyticTree.setID(Long.randomLong());
    // itemAnalyticTree.setRemoteGuid(obj.getProperty("Oid").toString());
    // }
    // itemAnalyticTree.setUpdatedOn(Long.parseLong(obj.getProperty("UpdatedOn").toString()));
    // Item itm =
    // dbHelper.getItemByRemoteGuid(obj.getProperty("ItemOid").toString());
    // ItemCategory cat =
    // dbHelper.getItemCategoryByRemoteGuid(obj.getProperty("ItemCategoryOid").toString());
    // itemAnalyticTree.setItem(itm);
    // itemAnalyticTree.setItemCategory(cat);
    //
    // if (newstatus)
    // dbHelper.getItemAnalyticTrees().create(itemAnalyticTree);
    // else
    // dbHelper.getItemAnalyticTrees().update(itemAnalyticTree);
    // currentRecords++;
    // if (currentRecords > totalRecords)
    // currentRecords = totalRecords;
    // if (i % COMMIT_SIZE == 0)
    // {
    // conn.setAutoCommit(true);
    // conn.setAutoCommit(false);
    // values[0] = (99 * currentRecords) / totalRecords;
    // publishProgress(values);
    //
    // }
    // if (maxupdatedOn < itemAnalyticTree.getUpdatedOn())
    // maxupdatedOn = itemAnalyticTree.getUpdatedOn();
    // }
    // Log.i("Sync ItemAnalyticTree", limit + " completed successfully");
    // }
    // while (repeat);
    // conn.setAutoCommit(false);
    // return 0;
    // }
    // catch (Exception ex)
    // {
    // return -1;
    // }
    //
    // }
    //
    // private boolean SynchronizePriceCatalogDetails(User user)
    // {
    // Integer[] values = { 0 };
    // long firstUpdatedOn = 0;
    // boolean returnValue = false;
    // try
    // {
    // HashMap<String, Object> params;
    // Object result;
    // SoapObject sresult;
    // boolean repeat;
    // int total = 0;
    // int limit;
    // long maxupdatedOn = dbHelper.getMaxPriceCatalogDetail(), olMaxUpdatedOn =
    // -1;
    // firstUpdatedOn = maxupdatedOn;
    // DatabaseConnection conn =
    // dbHelper.getConnectionSource().getReadWriteConnection();
    //
    // do
    // {
    // if (maxupdatedOn == olMaxUpdatedOn)
    // break;
    // olMaxUpdatedOn = maxupdatedOn;
    //
    // params = new HashMap<String, Object>();
    // params.put("updatedon", "" + maxupdatedOn);
    // params.put("userID", user.getRemoteGuid());
    // repeat = false;
    // result = callWebMethod("getPriceCatalogDetails", params);
    // sresult = (SoapObject) result;
    // limit = sresult.getPropertyCount();
    // conn.setAutoCommit(false);
    // for (int i = 0; i < limit; i++)
    // {
    // repeat = true;
    // SoapObject obj = (SoapObject) sresult.getProperty(i);
    // PriceCatalogDetail pcd =
    // dbHelper.getPriceCatalogDetailByRemoteGuid(obj.getProperty("Oid").toString());
    // boolean newstatus = false;
    // if (pcd == null)
    // {
    // newstatus = true;
    // pcd = new PriceCatalogDetail();
    // // pcd.setID(Long.randomLong());
    // pcd.setRemoteGuid(obj.getProperty("Oid").toString());
    // }
    //
    // // pcd.setCode(obj.getProperty("Code").toString());
    // pcd.setUpdatedOn(Long.parseLong(obj.getProperty("UpdatedOn").toString()));
    // pcd.setPrice(Double.parseDouble(obj.getPropertyAsString("value")));
    // pcd.setDiscount(Double.parseDouble(obj.getPropertyAsString("discount")));
    // pcd.setVATIncluded(Boolean.parseBoolean(obj.getPropertyAsString("vatIncluded")));
    //
    // // Item it =
    // //
    // dbHelper.getItemByRemoteGuid(obj.getPrimitivePropertyAsString("ItemOid"));
    // // Barcode bc =
    // //
    // dbHelper.getBarcodeByRemoteGuid(obj.getPrimitivePropertyAsString("BarcodeOid"));
    // pcd.setBarcodeRemoteGuid(obj.getProperty("BarcodeOid").toString());
    // pcd.setItemRemoteGuid(obj.getProperty("ItemOid").toString());
    // PriceCatalog pc =
    // dbHelper.getPriceCatalogByRemoteGuid(obj.getPrimitivePropertyAsString("PriceCatalogOid"));
    //
    // // pcd.setBarcode(bc);
    // pcd.setPriceCatalog(pc);
    // // pcd.setItem(it);
    //
    // if (newstatus)
    // dbHelper.getPriceCatalogDetailDao().create(pcd);
    // else
    // dbHelper.getPriceCatalogDetailDao().update(pcd);
    // currentRecords++;
    // if (currentRecords > totalRecords)
    // currentRecords = totalRecords;
    // if (i % COMMIT_SIZE == 0)
    // {
    // conn.setAutoCommit(true);
    // conn.setAutoCommit(false);
    // values[0] = (99 * currentRecords) / totalRecords;
    // publishProgress(values);
    //
    // }
    // if (maxupdatedOn < pcd.getUpdatedOn())
    // maxupdatedOn = pcd.getUpdatedOn();
    // }
    // conn.setAutoCommit(true);
    // Log.i("Sync PriceCatalogDetail", limit + " completed successfully");
    // }
    // while (repeat);
    // returnValue = true;
    // }
    // catch (Exception ex)
    // {
    // ex.printStackTrace();
    // returnValue = false;
    // }
    //
    // dbHelper.getWritableDatabase().execSQL("update pricecatalogdetail set bc_id = (select id from Barcode where Barcode.remoteGuid = pricecatalogdetail.barcodeRemoteGuid) where UpdatedOn >= "
    // + firstUpdatedOn);
    // dbHelper.getWritableDatabase().execSQL("update pricecatalogdetail set item_id = (select id from Item where item.remoteGuid = pricecatalogdetail.itemRemoteGuid) where UpdatedOn >= "
    // + firstUpdatedOn);
    // return returnValue;
    // }
    //
    // private int SynchronizeItems(User user)
    // {
    // int exitCode = 0;
    // try
    // {
    // // MeasurementUtin
    // DatabaseConnection conn =
    // dbHelper.getConnectionSource().getReadWriteConnection();
    // HashMap<String, Object> params = new HashMap<String, Object>();
    // params.put("updatedon", "" + dbHelper.getMaxMeasurementUnitUpdatedOn());
    // params.put("userID", user.getRemoteGuid());
    // Object result = callWebMethod("getMeasurementUnits", params);
    // SoapObject sresult = (SoapObject) result;
    // int limit = sresult.getPropertyCount();
    // conn.setAutoCommit(false);
    // for (int i = 0; i < limit; i++)
    // {
    // SoapObject obj = (SoapObject) sresult.getProperty(i);
    // MeasurementUnit ct;
    // ct =
    // dbHelper.getMeasurementUnitByRemoteGuid(obj.getProperty("Oid").toString());
    // boolean newct;
    // if (ct == null)
    // {
    // newct = true;
    // ct = new MeasurementUnit();
    // }
    // else
    // {
    // newct = false;
    // }
    //
    // ct.setDescription(obj.getProperty("Description").toString());
    // ct.setUpdatedOn(Long.parseLong(obj.getProperty("UpdatedOn").toString()));
    // ct.setRemoteGuid(obj.getProperty("Oid").toString());
    // String supdec = obj.getProperty("SupportDecimal").toString();
    // ct.setSupportDecimals(supdec.compareToIgnoreCase("true") == 0);
    // if (newct)
    // dbHelper.getMeasurementUnitDao().create(ct);
    // else
    // dbHelper.getMeasurementUnitDao().update(ct);
    // if (i % COMMIT_SIZE == 0)
    // {
    // conn.setAutoCommit(true);
    // conn.setAutoCommit(false);
    // }
    //
    // }
    // Log.i("Sync MeasurementUnit", limit + " completed successfully");
    //
    // conn.setAutoCommit(true);
    //
    // }
    // catch (Exception ex)
    // {
    // ex.printStackTrace();
    // exitCode = -1;
    // }
    //
    // Integer[] values = { 0 };
    // long firstItemUpdatedOn = 0;
    // long firstBarcodeUpdatedOn = 0;
    //
    // try
    // {
    // HashMap<String, Object> params;
    // Object result;
    // SoapObject sresult;
    // // ArrayList<Pair<Item, String>> noBarcodeYetAvailable = new
    // // ArrayList<Pair<Item, String>>();
    // boolean repeat;
    // int total = 0;
    // int limit;
    // long maxupdatedOn = dbHelper.getMaxItemUpdatedOn(), olMaxUpdatedOn = -1;
    // firstItemUpdatedOn = maxupdatedOn;
    // DatabaseConnection conn =
    // dbHelper.getConnectionSource().getReadWriteConnection();
    //
    // do
    // {
    // if (maxupdatedOn == olMaxUpdatedOn)
    // break;
    // olMaxUpdatedOn = maxupdatedOn;
    //
    // params = new HashMap<String, Object>();
    // params.put("updatedon", "" + maxupdatedOn);
    // params.put("userID", user.getRemoteGuid());
    // repeat = false;
    // result = callWebMethod("getItems", params);
    // sresult = (SoapObject) result;
    // limit = sresult.getPropertyCount();
    // conn.setAutoCommit(false);
    // for (int i = 0; i < limit; i++)
    // {
    // repeat = true;
    // SoapObject obj = (SoapObject) sresult.getProperty(i);
    // Item item =
    // dbHelper.getItemByRemoteGuid(obj.getProperty("Oid").toString());
    // VATCategory vc =
    // dbHelper.getVatCategoryByRemoteGuid(obj.getPropertyAsString("VatCategory"));
    // // boolean newstatus = false;
    //
    // String name = obj.getProperty("Name").toString().replace("'", "''");
    // String code = obj.getProperty("Code").toString().replace("'", "''");
    // String imageOid = obj.getProperty("ImageOid").toString().replace("'",
    // "''");
    // String updatedOn = obj.getProperty("UpdatedOn").toString().replace("'",
    // "''");
    // String insertedDay =
    // obj.getProperty("InsertedDay").toString().replace("'", "''");
    // String isActive = obj.getProperty("isActive").toString().replace("'",
    // "''");
    // String pacQty = obj.getProperty("packingQty").toString().replace("'",
    // "''");
    // String maxQty = obj.getProperty("maxOrderQty").toString().replace("'",
    // "''");
    // String isActiveSQL = "0";
    // if (isActive.toLowerCase().trim().equals("true"))
    // {
    // isActiveSQL = "1";
    // }
    //
    // String defbarcode = obj.getPropertyAsString("DefaultBarcodeOid");
    // long vc_id = vc.getID(), parsedUpdatedOn = Long.parseLong(updatedOn);
    // String query;
    // if (item == null)
    // {
    //
    // query =
    // "INSERT INTO item (loweCaseName,Name,Code,ImageOid,UpdatedOn,InsertedOn,isactive,vatCategory_id,defaultBarcodeRemoteGuid,remoteGuid, packingQty  , maxOrderQty) VALUES('"
    // + name.toLowerCase() + "','" + name + "','" + code + "','" + imageOid +
    // "','" + updatedOn + "','" + insertedDay + "','" + isActiveSQL + "','" +
    // vc_id + "','" + defbarcode + "','" + obj.getProperty("Oid").toString() +
    // "','" + pacQty + "','" + maxQty + "')";
    // }
    // else
    // {
    // query = "UPDATE Item set loweCaseName='" + name.toLowerCase() +
    // "', Name = '" + name + "',Code = '" + code + "',ImageOid = '" + imageOid
    // + "',UpdatedOn = '" + updatedOn + "',InsertedOn = '" + insertedDay +
    // "',isactive = '" + isActiveSQL + "', vatCategory_id = '" + vc_id +
    // "', defaultBarcodeRemoteGuid ='" + defbarcode + "', packingQty='" +
    // pacQty + "',maxOrderQty='" + maxQty + "'  where id='" + item.getID() +
    // "'";
    //
    // }
    //
    // dbHelper.getWritableDatabase().execSQL(query);
    //
    // currentRecords++;
    // if (currentRecords > totalRecords)
    // currentRecords = totalRecords;
    // if (i % COMMIT_SIZE == 0)
    // {
    // conn.setAutoCommit(true);
    // conn.setAutoCommit(false);
    // values[0] = (99 * currentRecords) / totalRecords;
    // publishProgress(values);
    //
    // }
    // if (maxupdatedOn < parsedUpdatedOn)
    // maxupdatedOn = parsedUpdatedOn;
    // }
    // conn.setAutoCommit(true);
    // total += limit;
    // Log.i("Sync Item", limit + " completed successfully");
    // }
    // while (repeat);
    //
    // maxupdatedOn = dbHelper.getMaxBarcodeUpdatedOn();
    // firstBarcodeUpdatedOn = maxupdatedOn;
    // olMaxUpdatedOn = -1;
    // do
    // {
    // if (maxupdatedOn == olMaxUpdatedOn)
    // break;
    // olMaxUpdatedOn = maxupdatedOn;
    // repeat = false;
    // params = new HashMap<String, Object>();
    // params.put("updatedon", "" + maxupdatedOn);
    // params.put("userID", user.getRemoteGuid());
    // result = callWebMethod("getBarcodes", params);
    // sresult = (SoapObject) result;
    // limit = sresult.getPropertyCount();
    // conn.setAutoCommit(false);
    // for (int i = 0; i < limit; i++)
    // {
    // repeat = true;
    // SoapObject obj = (SoapObject) sresult.getProperty(i);
    // Barcode barcode =
    // dbHelper.getBarcodeByRemoteGuid(obj.getProperty("Oid").toString());
    // boolean newstatus = false;
    // if (barcode == null)
    // {
    // newstatus = true;
    // barcode = new Barcode();
    // barcode.setRemoteGuid(obj.getProperty("Oid").toString());
    // }
    // barcode.setCode(obj.getProperty("Code").toString());
    // barcode.setUpdatedOn(Long.parseLong(obj.getProperty("UpdatedOn").toString()));
    // String itemOid = obj.getPropertyAsString("ItemOid");
    // barcode.setItemRemoteGuid(itemOid);
    // barcode.setMeasurementUnitRemoteGuid(obj.getPropertyAsString("MeasurementUnitOid"));
    // if (newstatus)
    // dbHelper.getBarcodes().create(barcode);
    // else
    // dbHelper.getBarcodes().update(barcode);
    // currentRecords++;
    // if (currentRecords > totalRecords)
    // currentRecords = totalRecords;
    // if (i % COMMIT_SIZE == 0)
    // {
    // values[0] = (99 * currentRecords) / totalRecords;
    // publishProgress(values);
    // conn.setAutoCommit(true);
    // conn.setAutoCommit(false);
    // }
    // if (maxupdatedOn < barcode.getUpdatedOn())
    // maxupdatedOn = barcode.getUpdatedOn();
    // }
    // conn.setAutoCommit(true);
    // Log.i("Sync Barcode", limit + " completed successfully");
    // total += limit;
    // }
    // while (repeat);
    //
    // conn.setAutoCommit(true);
    //
    // Log.d("Sync Items", "All completed successfully");
    // conn.setAutoCommit(true);
    // exitCode = 0;
    // }
    // catch (Exception ex)
    // {
    // ex.printStackTrace();
    // exitCode = -1;
    // }
    //
    // String query =
    // "update item set defaultBarcodeObject_id = (select id from Barcode where Barcode.remoteGuid = Item.defaultBarcodeRemoteGuid) where defaultBarcodeObject_id is null or UpdatedOn >="
    // + firstItemUpdatedOn;
    // Log.i("Sync Items", "Executing: " + query);
    // dbHelper.getWritableDatabase().execSQL(query);
    //
    // query =
    // "update barcode set item_id= (select id from Item where item.remoteGuid = barcode.itemRemoteGuid) where item_id is null or UpdatedOn >="
    // + firstBarcodeUpdatedOn;
    // Log.i("Sync Barcodes", "Executing:" + query);
    // dbHelper.getWritableDatabase().execSQL(query);
    //
    // query =
    // "update barcode set MeasurementUnit_id= (select id from MeasurementUnit where MeasurementUnit.remoteGuid = barcode.measurementUnitRemoteGuid) where MeasurementUnit_id is null or UpdatedOn >="
    // + firstBarcodeUpdatedOn;
    // Log.i("Sync MeasurementUnits", "Executing:" + query);
    // dbHelper.getWritableDatabase().execSQL(query);
    //
    // return exitCode;
    //
    // }
    //
    // private int SynchronizeItemLinkedItems(User user)
    // {
    //
    // Integer[] values = { 0 };
    // try
    // {
    // DatabaseConnection conn =
    // dbHelper.getConnectionSource().getReadWriteConnection();
    // HashMap<String, Object> params = new HashMap<String, Object>();
    // params.put("updatedon", "" + dbHelper.getMaxLinkedItem());
    // params.put("userID", user.getRemoteGuid());
    // Object result = callWebMethod("getLinkedItems", params);
    // SoapObject sresult = (SoapObject) result;
    // int limit = sresult.getPropertyCount();
    // conn.setAutoCommit(false);
    // for (int i = 0; i < limit; i++)
    // {
    // SoapObject obj = (SoapObject) sresult.getProperty(i);
    // LinkedItem ct;
    // ct =
    // dbHelper.getLinkedItemByRemoteGuid(obj.getProperty("Oid").toString());
    // if (ct == null)
    // {
    // ct = new LinkedItem();
    // // ct.setID(Long.randomLong());
    // }
    // ct.setRemoteGuid(obj.getPropertyAsString("Oid"));
    // ct.setItemGuid(obj.getPropertyAsString("ItemOid"));
    // ct.setSubItemGuid(obj.getPropertyAsString("SubitemOid"));
    // ct.setUpdatedOn(Long.parseLong(obj.getProperty("UpdatedOn").toString()));
    //
    // dbHelper.getLinkedItemDao().createOrUpdate(ct);
    // currentRecords++;
    // if (i % COMMIT_SIZE == 0)
    // {
    // if (currentRecords > totalRecords)
    // currentRecords = totalRecords;
    // values[0] = (99 * currentRecords) / totalRecords;
    // publishProgress(values);
    // conn.setAutoCommit(true);
    // conn.setAutoCommit(false);
    // }
    //
    // }
    // conn.setAutoCommit(true);
    // if (currentRecords > totalRecords)
    // currentRecords = totalRecords;
    // return limit;
    // }
    // catch (Exception ex)
    // {
    // ex.printStackTrace();
    // return -1;
    // }
    //
    // }
    //
    // private int SynchronizeItemCategories(User user)
    // {
    //
    // Integer[] values = { 0 };
    // try
    // {
    // DatabaseConnection conn =
    // dbHelper.getConnectionSource().getReadWriteConnection();
    // HashMap<String, Object> params = new HashMap<String, Object>();
    // params.put("updatedon", "" + dbHelper.getMaxItemCategoriesUpdatedOn());
    // params.put("userID", user.getRemoteGuid());
    // Object result = callWebMethod("getItemCategories", params);
    // SoapObject sresult = (SoapObject) result;
    // ArrayList<ItemCategory> noParentYetAvailable = new
    // ArrayList<ItemCategory>();
    // int limit = sresult.getPropertyCount();
    // conn.setAutoCommit(false);
    // for (int i = 0; i < limit; i++)
    // {
    // SoapObject obj = (SoapObject) sresult.getProperty(i);
    // ItemCategory ct;
    // ct =
    // dbHelper.getItemCategoryByRemoteGuid(obj.getProperty("Oid").toString());
    // boolean newct;
    // if (ct == null)
    // {
    // newct = true;
    // ct = new ItemCategory();
    // // ct.setID(Long.randomLong());
    // }
    // else
    // {
    // newct = false;
    // }
    // String parentOid = obj.getProperty("parentOid").toString();
    // ct.setCode(obj.getProperty("Code").toString());
    // ct.setName(obj.getProperty("Description").toString());
    // ct.setRemoteGuid(obj.getProperty("Oid").toString());
    // ct.setRemoteParentGuid(parentOid);
    // ct.setUpdatedOn(Long.parseLong(obj.getProperty("UpdatedOn").toString()));
    // if (!parentOid.equals("00000000-0000-0000-0000-000000000000"))
    // {
    // ItemCategory parent = dbHelper.getItemCategoryByRemoteGuid(parentOid);
    // if (parent != null)
    // {
    // ct.setParent(parent);
    //
    // }
    // else
    // {
    // noParentYetAvailable.add(ct);
    // }
    //
    // }
    // if (newct)
    // dbHelper.getItemCategories().create(ct);
    // else
    // dbHelper.getItemCategories().update(ct);
    // currentRecords++;
    // if (currentRecords > totalRecords)
    // currentRecords = totalRecords;
    // if (i % COMMIT_SIZE == 0)
    // {
    // values[0] = (99 * currentRecords) / totalRecords;
    // publishProgress(values);
    // conn.setAutoCommit(true);
    // conn.setAutoCommit(false);
    // }
    //
    // }
    // conn.setAutoCommit(true);
    // conn.setAutoCommit(false);
    // for (int i = 0; i < noParentYetAvailable.size(); i++)
    // {
    // ItemCategory toproc = noParentYetAvailable.get(i), parent;
    // parent =
    // dbHelper.getItemCategoryByRemoteGuid(toproc.getRemoteParentGuid());
    // if (parent != null)
    // {
    // toproc.setParent(parent);
    // dbHelper.getItemCategories().update(toproc);
    // currentRecords++;
    // if (currentRecords > totalRecords)
    // currentRecords = totalRecords;
    // values[0] = (99 * currentRecords) / totalRecords;
    // publishProgress(values);
    // }
    // }
    // conn.setAutoCommit(true);
    // dbHelper.getConnectionSource().releaseConnection(conn);
    // return limit;
    // }
    // catch (Exception ex)
    // {
    // ex.printStackTrace();
    // return -1;
    // }
    //
    // }
    //
    // }

    // class ItemSynchronizer extends AsyncTask<User, Integer, Object>
    // {
    //
    // // private DatabaseHelper dbHelper;
    //
    // @Override
    // protected Object doInBackground(User... params)
    // {
    // // dbHelper = new DatabaseHelper(mContext);
    //
    // Log.i("ItemSynchronizer", "Starting Items Sync");
    // SynchronizeItems();
    // Log.i("ItemSynchronizer", "Starting Item Categories Sync");
    // SynchronizeItemCategories(params[0]);
    // Log.i("ItemSynchronizer", "Starting Item Analytic Trees Sync");
    // SynchronizeItemAnalyticTrees(params[0]);
    // // dbHelper.close();
    // return null;
    // }
    //
    // private int SynchronizeItems()
    // {
    //
    // Integer[] values = { 0 };
    // try
    // {
    // HashMap<String, Object> params;
    // Object result;
    // SoapObject sresult;
    // boolean repeat;
    // int total = 0;
    // int limit;
    // long maxupdatedOn = dbHelper.getMaxItemUpdatedOn(), olMaxUpdatedOn = -1;
    // DatabaseConnection conn =
    // dbHelper.getConnectionSource().getReadWriteConnection();
    //
    // do
    // {
    // if (maxupdatedOn == olMaxUpdatedOn)
    // break;
    // olMaxUpdatedOn = maxupdatedOn;
    //
    // params = new HashMap<String, Object>();
    // params.put("updatedon", "" + maxupdatedOn);
    // repeat = false;
    // result = callWebMethod("getItems", params);
    // sresult = (SoapObject) result;
    // limit = sresult.getPropertyCount();
    // conn.setAutoCommit(false);
    // for (int i = 0; i < limit; i++)
    // {
    // repeat = true;
    // SoapObject obj = (SoapObject) sresult.getProperty(i);
    // Item item =
    // dbHelper.getItemByRemoteGuid(obj.getProperty("Oid").toString());
    // VATCategory vc =
    // dbHelper.getVatCategoryByRemoteGuid(obj.getPropertyAsString("VatCategory"));
    // // boolean newstatus = false;
    //
    // String name = obj.getProperty("Name").toString().replace("'", "''");
    // String code = obj.getProperty("Code").toString().replace("'", "''");
    // String imageOid = obj.getProperty("ImageOid").toString().replace("'",
    // "''");
    // String updatedOn = obj.getProperty("UpdatedOn").toString().replace("'",
    // "''");
    // String insertedDay =
    // obj.getProperty("InsertedDay").toString().replace("'", "''");
    // String isActive = obj.getProperty("isActive").toString().replace("'",
    // "''");
    // String isActiveSQL = isActive.toLowerCase() == "true" ? "1" : "0";
    // String defbarcode = obj.getPropertyAsString("DefaultBarcodeOid");
    // long vc_id = vc.getID(), parsedUpdatedOn = Long.parseLong(updatedOn);
    // String query;
    // if (item == null)
    // {
    // // newstatus = true;
    // // item = new Item();
    // // item.setID(Long.randomLong());
    // // item.setRemoteOid(obj.getProperty("Oid").toString());
    //
    // query =
    // "INSERT INTO item (Name,Code,ImageOid,UpdatedOn,InsertedOn,isactive,vatCategory_id,defaultBarcodeRemoteGuid,remoteGuid) VALUES('"
    // + name + "','" + code + "','" + imageOid + "','" + updatedOn + "','" +
    // insertedDay + "','" + isActiveSQL + "','" + vc_id + "','" + defbarcode +
    // "','" + obj.getProperty("Oid").toString() + "')";
    // }
    // else
    // {
    // query = "UPDATE Item set Name = '" + name + "',Code = '" + code +
    // "',ImageOid = '" + imageOid + "',UpdatedOn = '" + updatedOn +
    // "',InsertedOn = '" + insertedDay + "',isactive = '" + isActiveSQL +
    // "', vatCategory_id = '" + vc_id + "', defaultBarcodeRemoteGuid ='" +
    // defbarcode + "' where id='" + item.getID() + "'";
    //
    // }
    //
    // dbHelper.getWritableDatabase().execSQL(query);
    //
    // currentRecords++;
    // if (currentRecords > totalRecords)
    // currentRecords = totalRecords;
    // if (i % COMMIT_SIZE == 0)
    // {
    // conn.setAutoCommit(true);
    // conn.setAutoCommit(false);
    // values[0] = (99 * currentRecords) / totalRecords;
    // publishProgress(values);
    //
    // }
    // if (maxupdatedOn < parsedUpdatedOn)
    // maxupdatedOn = parsedUpdatedOn;
    // }
    // conn.setAutoCommit(true);
    // total += limit;
    // Log.i("Sync Item", limit + " completed successfully");
    // }
    // while (repeat);
    //
    // conn.setAutoCommit(true);
    //
    // // Cursor resu =
    // //
    // dbHelper.getWritableDatabase().rawQuery("update item set defaultBarcodeObject_id = (select id from Barcode where Barcode.remoteGuid = Item.defaultBarcodeRemoteGuid)",
    // // null);
    // //
    // dbHelper.getWritableDatabase().execSQL("update item set defaultBarcodeObject_id = (select id from Barcode where Barcode.remoteGuid = Item.defaultBarcodeRemoteGuid)");
    // //
    // dbHelper.getWritableDatabase().execSQL("update barcode set item_id= (select id from Item where item.remoteGuid = barcode.itemRemoteGuid)");
    //
    // Log.d("Sync Items", "All completed successfully");
    // conn.setAutoCommit(true);
    // dbHelper.getConnectionSource().releaseConnection(conn);
    // return 0;
    // }
    // catch (Exception ex)
    // {
    // ex.printStackTrace();
    // return -1;
    // }
    // }
    //
    // private int SynchronizeItemAnalyticTrees(User user)
    // {
    // Integer[] values = { 0 };
    // try
    // {
    // DatabaseConnection conn =
    // dbHelper.getConnectionSource().getReadWriteConnection();
    // long maxupdatedOn = dbHelper.getMaxItemAnalyticTreesUpdatedOn(),
    // olMaxUpdatedOn = -1;
    // HashMap<String, Object> params;
    // boolean repeat;
    // do
    // {
    // if (maxupdatedOn == olMaxUpdatedOn)
    // break;
    // olMaxUpdatedOn = maxupdatedOn;
    //
    // params = new HashMap<String, Object>();
    // params.put("updatedon", "" + maxupdatedOn);
    // params.put("userID", user.getRemoteGuid());
    // repeat = false;
    // Object result = callWebMethod("getItemAnalyticTrees", params);
    // SoapObject sresult = (SoapObject) result;
    // int limit = sresult.getPropertyCount();
    // conn.setAutoCommit(false);
    // for (int i = 0; i < limit; i++)
    // {
    // repeat = true;
    // SoapObject obj = (SoapObject) sresult.getProperty(i);
    // ItemAnalyticTree itemAnalyticTree =
    // dbHelper.getItemAnalyticTreeByRemoteGuid(obj.getProperty("Oid").toString());
    // boolean newstatus = false;
    // if (itemAnalyticTree == null)
    // {
    // newstatus = true;
    // itemAnalyticTree = new ItemAnalyticTree();
    // // itemAnalyticTree.setID(Long.randomLong());
    // itemAnalyticTree.setRemoteGuid(obj.getProperty("Oid").toString());
    // }
    // itemAnalyticTree.setUpdatedOn(Long.parseLong(obj.getProperty("UpdatedOn").toString()));
    // Item itm =
    // dbHelper.getItemByRemoteGuid(obj.getProperty("ItemOid").toString());
    // ItemCategory cat =
    // dbHelper.getItemCategoryByRemoteGuid(obj.getProperty("ItemCategoryOid").toString());
    // itemAnalyticTree.setItem(itm);
    // itemAnalyticTree.setItemCategory(cat);
    //
    // if (newstatus)
    // dbHelper.getItemAnalyticTrees().create(itemAnalyticTree);
    // else
    // dbHelper.getItemAnalyticTrees().update(itemAnalyticTree);
    // currentRecords++;
    // if (currentRecords > totalRecords)
    // currentRecords = totalRecords;
    // if (i % COMMIT_SIZE == 0)
    // {
    // conn.setAutoCommit(true);
    // conn.setAutoCommit(false);
    // values[0] = (99 * currentRecords) / totalRecords;
    // publishProgress(values);
    //
    // }
    // if (maxupdatedOn < itemAnalyticTree.getUpdatedOn())
    // maxupdatedOn = itemAnalyticTree.getUpdatedOn();
    // }
    // Log.i("Sync ItemAnalyticTree", limit + " completed successfully");
    // }
    // while (repeat);
    // conn.setAutoCommit(true);
    // dbHelper.getConnectionSource().releaseConnection(conn);
    // return 0;
    // }
    // catch (Exception ex)
    // {
    // return -1;
    // }
    //
    // }
    //
    // private int SynchronizeItemCategories(User user)
    // {
    //
    // Integer[] values = { 0 };
    // try
    // {
    // DatabaseConnection conn =
    // dbHelper.getConnectionSource().getReadWriteConnection();
    // HashMap<String, Object> params = new HashMap<String, Object>();
    // params.put("updatedon", "" + dbHelper.getMaxItemCategoriesUpdatedOn());
    // params.put("userID", user.getRemoteGuid());
    // Object result = callWebMethod("getItemCategories", params);
    // SoapObject sresult = (SoapObject) result;
    // ArrayList<ItemCategory> noParentYetAvailable = new
    // ArrayList<ItemCategory>();
    // int limit = sresult.getPropertyCount();
    // conn.setAutoCommit(false);
    // for (int i = 0; i < limit; i++)
    // {
    // SoapObject obj = (SoapObject) sresult.getProperty(i);
    // ItemCategory ct;
    // ct =
    // dbHelper.getItemCategoryByRemoteGuid(obj.getProperty("Oid").toString());
    // boolean newct;
    // if (ct == null)
    // {
    // newct = true;
    // ct = new ItemCategory();
    // // ct.setID(Long.randomLong());
    // }
    // else
    // {
    // newct = false;
    // }
    // String parentOid = obj.getProperty("parentOid").toString();
    // ct.setCode(obj.getProperty("Code").toString());
    // ct.setName(obj.getProperty("Description").toString());
    // ct.setRemoteGuid(obj.getProperty("Oid").toString());
    // ct.setRemoteParentGuid(parentOid);
    // ct.setUpdatedOn(Long.parseLong(obj.getProperty("UpdatedOn").toString()));
    // if (!parentOid.equals("00000000-0000-0000-0000-000000000000"))
    // {
    // ItemCategory parent = dbHelper.getItemCategoryByRemoteGuid(parentOid);
    // if (parent != null)
    // {
    // ct.setParent(parent);
    //
    // }
    // else
    // {
    // noParentYetAvailable.add(ct);
    // }
    //
    // }
    // if (newct)
    // dbHelper.getItemCategories().create(ct);
    // else
    // dbHelper.getItemCategories().update(ct);
    // currentRecords++;
    // if (currentRecords > totalRecords)
    // currentRecords = totalRecords;
    // if (i % COMMIT_SIZE == 0)
    // {
    // values[0] = (99 * currentRecords) / totalRecords;
    // publishProgress(values);
    // conn.setAutoCommit(true);
    // conn.setAutoCommit(false);
    // }
    //
    // }
    // conn.setAutoCommit(true);
    // conn.setAutoCommit(false);
    // for (int i = 0; i < noParentYetAvailable.size(); i++)
    // {
    // ItemCategory toproc = noParentYetAvailable.get(i), parent;
    // parent =
    // dbHelper.getItemCategoryByRemoteGuid(toproc.getRemoteParentGuid());
    // if (parent != null)
    // {
    // toproc.setParent(parent);
    // dbHelper.getItemCategories().update(toproc);
    // currentRecords++;
    // if (currentRecords > totalRecords)
    // currentRecords = totalRecords;
    // values[0] = (99 * currentRecords) / totalRecords;
    // publishProgress(values);
    // }
    // }
    // conn.setAutoCommit(true);
    // dbHelper.getConnectionSource().releaseConnection(conn);
    // return limit;
    // }
    // catch (Exception ex)
    // {
    // ex.printStackTrace();
    // return -1;
    // }
    //
    // }
    //
    // }
    //
    // class BarcodeSynchronizer extends AsyncTask<User, Integer, Object>
    // {
    // // private DatabaseHelper dbHelper;
    //
    // @Override
    // protected Object doInBackground(User... params)
    // {
    // // dbHelper = new DatabaseHelper(mContext);
    // Log.i("BarcodeSynchronizer", "Starting Barcodes Sync");
    // SynchronizeBarcodes(params[0]);
    // // dbHelper.close();
    // return null;
    // }
    //
    // private int SynchronizeBarcodes(User user)
    // {
    // try
    // {
    // Integer[] values = { 0 };
    //
    // HashMap<String, Object> params;
    // Object result;
    // SoapObject sresult;
    // boolean repeat;
    // int total = 0;
    // int limit;
    // long maxupdatedOn = dbHelper.getMaxBarcodeUpdatedOn();
    // long olMaxUpdatedOn = -1;
    // DatabaseConnection conn =
    // dbHelper.getConnectionSource().getReadWriteConnection();
    //
    // do
    // {
    // if (maxupdatedOn == olMaxUpdatedOn)
    // break;
    // olMaxUpdatedOn = maxupdatedOn;
    // repeat = false;
    // params = new HashMap<String, Object>();
    // params.put("updatedon", "" + maxupdatedOn);
    // params.put("userID", user.getRemoteGuid());
    // result = callWebMethod("getBarcodes", params);
    // sresult = (SoapObject) result;
    // limit = sresult.getPropertyCount();
    // conn.setAutoCommit(false);
    // for (int i = 0; i < limit; i++)
    // {
    // repeat = true;
    // SoapObject obj = (SoapObject) sresult.getProperty(i);
    // Barcode barcode =
    // dbHelper.getBarcodeByRemoteGuid(obj.getProperty("Oid").toString());
    // boolean newstatus = false;
    // if (barcode == null)
    // {
    // newstatus = true;
    // barcode = new Barcode();
    // // barcode.setID(Long.randomLong());
    // barcode.setRemoteGuid(obj.getProperty("Oid").toString());
    // }
    // barcode.setCode(obj.getProperty("Code").toString());
    // barcode.setUpdatedOn(Long.parseLong(obj.getProperty("UpdatedOn").toString()));
    // String itemOid = obj.getPropertyAsString("ItemOid");
    // barcode.setItemRemoteGuid(itemOid);
    // // Item item = dbHelper.getItemByRemoteGuid(itemOid);
    // // barcode.setItem(item);
    // if (newstatus)
    // dbHelper.getBarcodes().create(barcode);
    // else
    // dbHelper.getBarcodes().update(barcode);
    // currentRecords++;
    // if (currentRecords > totalRecords)
    // currentRecords = totalRecords;
    // if (i % COMMIT_SIZE == 0)
    // {
    // values[0] = (99 * currentRecords) / totalRecords;
    // publishProgress(values);
    // conn.setAutoCommit(true);
    // conn.setAutoCommit(false);
    // }
    // if (maxupdatedOn < barcode.getUpdatedOn())
    // maxupdatedOn = barcode.getUpdatedOn();
    // }
    // conn.setAutoCommit(true);
    // Log.i("Sync Barcode", limit + " completed successfully");
    // total += limit;
    // }
    // while (repeat);
    // dbHelper.getConnectionSource().releaseConnection(conn);
    // }
    // catch (Exception e)
    // {
    // e.printStackTrace();
    // return -1;
    // }
    //
    // return 0;
    // }
    //
    // }
    //
    // class PriceCatalogDetailSynchronizer extends AsyncTask<User, Integer,
    // Object>
    // {
    // // private DatabaseHelper dbHelper;
    //
    // @Override
    // protected Object doInBackground(User... params)
    // {
    // // dbHelper = new DatabaseHelper(mContext);
    // Log.i("PriceCatalogDetailsSynchronizer",
    // "Starting Price Catalog Details Sync");
    // SynchronizePriceCatalogDetails(params[0]);
    // // dbHelper.close();
    // return null;
    // }
    //
    // private boolean SynchronizePriceCatalogDetails(User user)
    // {
    // Integer[] values = { 0 };
    // try
    // {
    // HashMap<String, Object> params;
    // Object result;
    // SoapObject sresult;
    // boolean repeat;
    // int total = 0;
    // int limit;
    // long maxupdatedOn = dbHelper.getMaxPriceCatalogDetail(), olMaxUpdatedOn =
    // -1;
    // DatabaseConnection conn =
    // dbHelper.getConnectionSource().getReadWriteConnection();
    //
    // do
    // {
    // if (maxupdatedOn == olMaxUpdatedOn)
    // break;
    // olMaxUpdatedOn = maxupdatedOn;
    //
    // params = new HashMap<String, Object>();
    // params.put("updatedon", "" + maxupdatedOn);
    // params.put("userID", user.getRemoteGuid());
    // repeat = false;
    // result = callWebMethod("getPriceCatalogDetails", params);
    // sresult = (SoapObject) result;
    // limit = sresult.getPropertyCount();
    // conn.setAutoCommit(false);
    // for (int i = 0; i < limit; i++)
    // {
    // repeat = true;
    // SoapObject obj = (SoapObject) sresult.getProperty(i);
    // PriceCatalogDetail pcd =
    // dbHelper.getPriceCatalogDetailByRemoteGuid(obj.getProperty("Oid").toString());
    // boolean newstatus = false;
    // if (pcd == null)
    // {
    // newstatus = true;
    // pcd = new PriceCatalogDetail();
    // // pcd.setID(Long.randomLong());
    // pcd.setRemoteGuid(obj.getProperty("Oid").toString());
    // }
    //
    // // pcd.setCode(obj.getProperty("Code").toString());
    // pcd.setUpdatedOn(Long.parseLong(obj.getProperty("UpdatedOn").toString()));
    // pcd.setPrice(Double.parseDouble(obj.getPropertyAsString("value")));
    // pcd.setDiscount(Double.parseDouble(obj.getPropertyAsString("discount")));
    // pcd.setVATIncluded(Boolean.parseBoolean(obj.getPropertyAsString("vatIncluded")));
    //
    // // Item it =
    // //
    // dbHelper.getItemByRemoteGuid(obj.getPrimitivePropertyAsString("ItemOid"));
    // // Barcode bc =
    // //
    // dbHelper.getBarcodeByRemoteGuid(obj.getPrimitivePropertyAsString("BarcodeOid"));
    // pcd.setBarcodeRemoteGuid(obj.getProperty("BarcodeOid").toString());
    // pcd.setItemRemoteGuid(obj.getProperty("ItemOid").toString());
    // PriceCatalog pc =
    // dbHelper.getPriceCatalogByRemoteGuid(obj.getPrimitivePropertyAsString("PriceCatalogOid"));
    //
    // // pcd.setBarcode(bc);
    // pcd.setPriceCatalog(pc);
    // // pcd.setItem(it);
    //
    // if (newstatus)
    // dbHelper.getPriceCatalogDetailDao().create(pcd);
    // else
    // dbHelper.getPriceCatalogDetailDao().update(pcd);
    // currentRecords++;
    // if (currentRecords > totalRecords)
    // currentRecords = totalRecords;
    // if (i % COMMIT_SIZE == 0)
    // {
    // conn.setAutoCommit(true);
    // conn.setAutoCommit(false);
    // values[0] = (99 * currentRecords) / totalRecords;
    // publishProgress(values);
    //
    // }
    // if (maxupdatedOn < pcd.getUpdatedOn())
    // maxupdatedOn = pcd.getUpdatedOn();
    // }
    // conn.setAutoCommit(true);
    // Log.i("Sync PriceCatalogDetail", limit + " completed successfully");
    // }
    // while (repeat);
    //
    // dbHelper.getConnectionSource().releaseConnection(conn);
    // //
    // dbHelper.getWritableDatabase().execSQL("update pricecatalogdetail set bc_id = (select id from Barcode where Barcode.remoteGuid = pricecatalogdetail.barcodeRemoteGuid)");
    // //
    // dbHelper.getWritableDatabase().execSQL("update pricecatalogdetail set item_id = (select id from Item where item.remoteGuid = pricecatalogdetail.itemRemoteGuid)");
    // return true;
    // }
    // catch (Exception ex)
    // {
    // ex.printStackTrace();
    // return false;
    // }
    // }
    //
    // }

    private DatabaseHelper dbHelper;

    private Object SynchronousCallWebMethod(String MethodName, HashMap<String, Object> parameters)
    {
	Log.v("WebMethodCaller", "Start " + MethodName);
	SoapSerializationEnvelope envelope = new SoapSerializationEnvelope(SoapEnvelope.VER12);
	try
	{
	    String METHOD_NAME = MethodName;
	    String SOAP_ACTION = NAMESPACE + METHOD_NAME;

	    SoapObject request = new SoapObject(NAMESPACE, METHOD_NAME);

	    if (parameters != null)
	    {
		for (Entry<String, Object> para : parameters.entrySet())
		{
		    request.addProperty(para.getKey(), para.getValue());
		}
	    }

	    envelope.dotNet = true;
	    Log.d("ws params", parameters.toString());
	    envelope.setOutputSoapObject(request);

	    HttpTransportSE androidHttpTransport = new HttpTransportSE(((MainActivity) mContext).appsettings.getServiceUrl(), TIMEOUT);

	    androidHttpTransport.call(SOAP_ACTION, envelope);
	    Object result = envelope.getResponse();
	    Log.v("WebMethodCaller", "End " + MethodName);
	    return result;

	}
	catch (Exception e)
	{
	    e.printStackTrace();
	    Log.v("WebMethodCaller", "End " + MethodName);
	    return null;
	}
    }

    public WebServiceHelper(DatabaseHelper dbHelper, Context ctx)
    {

	this.dbHelper = dbHelper;
	mContext = ctx;
	UpdateStatus(ctx);
	getVersion();
    }

    private int getTotalUpdates(long Customerupdatedon, long Itemupdatedon, long IATupdatedon, long Barcodeupdatedon, long ICupdatedon, long PCupdatedon, long PCDupdatedon, long VCupdatedon, long VFupdatedon, long VLupdatedon, long offerUpdatedOn, long offerDetailUpdatedOn, long documentStatusUpdatedOn, long storesUpdatedOn, long linkedItemsUpdatedOn, long mmUpdatedOn, long documentTypesUpdatedOn, User currentUser)
    {

	HashMap<String, Object> params = new HashMap<String, Object>();
	params.put("Customerupdatedon", "" + Customerupdatedon);
	params.put("Itemupdatedon", "" + Itemupdatedon);
	params.put("IATupdatedon", "" + IATupdatedon);
	params.put("Barcodeupdatedon", "" + Barcodeupdatedon);
	params.put("ICupdatedon", "" + ICupdatedon);
	params.put("PCupdatedon", "" + PCupdatedon);
	params.put("PCDupdatedon", "" + PCDupdatedon);
	params.put("VCupdatedon", "" + VCupdatedon);
	params.put("VFupdatedon", "" + VFupdatedon);
	params.put("VLupdatedon", "" + VLupdatedon);
	params.put("offerUpdatedOn", "" + offerUpdatedOn);
	params.put("offerDetailUpdatedOn", "" + offerDetailUpdatedOn);
	params.put("documentStatusUpdatedOn", "" + documentStatusUpdatedOn);
	params.put("storesUpdatedOn", "" + storesUpdatedOn);
	params.put("linkedItemsUpdatedOn", "" + linkedItemsUpdatedOn);
	params.put("mmUpdatedOn", "" + mmUpdatedOn);
	params.put("documentTypeUpdatedOn", "" + documentTypesUpdatedOn);
	params.put("userID", currentUser.getRemoteGuid());
	
	Object result = callWebMethod("GetTotalUpdates", params);
	if (result != null)
	    return Integer.parseInt(result.toString());
	else
	    return -1;
    }

    public String getVersion()
    {
	Object result;

	result = callWebMethod("GetVersion", null);
	return result == null ? null : result.toString();

    }

    private Object callWebMethod(String string, HashMap<String, Object> parameters)
    {
	if (Looper.myLooper() == Looper.getMainLooper())
	{
	    WebMethodCaller wm = new WebMethodCaller();
	    WebMethodParameters wmp = new WebMethodParameters();
	    wmp.MethodName = string;
	    wmp.parameters = parameters;
	    try
	    {
		return wm.execute(wmp).get();
	    }
	    catch (InterruptedException e)
	    {
		return null;
	    }
	    catch (ExecutionException e)
	    {
		// TODO Auto-generated catch block
		return null;
	    }
	}
	else
	    return SynchronousCallWebMethod(string, parameters);

    }

    /**
     * Returns a list of the problematic items
     * 
     * @param order
     * @return
     */
    public List<InvalidItem> ValidateOrder(String order)
    {
	try
	{

	    List<InvalidItem> problematicItems = new ArrayList<InvalidItem>();
	    if (!isOnline)
		return null;
	    HashMap<String, Object> params = new HashMap<String, Object>();
	    params.put("data", order);
	    Object result = callWebMethod("ValidateOrder", params);
	    SoapObject sresult = (SoapObject) result;
	    // if (sresult.getPropertyCount() <= 0)
	    // {
	    // return null;
	    // }

	    for (int i = 0; i < sresult.getPropertyCount(); i++)
	    {
		InvalidItem invItem = new InvalidItem();

		SoapObject obj = (SoapObject) sresult.getProperty(i);
		invItem.setItemRemoteGuid(UUID.fromString(obj.getProperty("ItemOid").toString()));
		invItem.setReason(InvalidItemReason.valueOf(obj.getProperty("Reason").toString()));
		problematicItems.add(invItem);
	    }

	    return problematicItems;
	}
	catch (Exception e)
	{
	    e.printStackTrace();
	    return null;
	}
    }

    public boolean IsOnline()
    {
	return isOnline;
    }

    public String Login(String username, String password)
    {
	if (isOnline)
	{
	    HashMap<String, Object> params = new HashMap<String, Object>();
	    params.put("username", username);
	    params.put("pass", password);
	    params.put("type", "2"); // this allows only suppliers to login
	    Object result = callWebMethod("Login", params);
	    return result.toString();
	}
	else
	{
	    User user = dbHelper.getByUsername(username);
	    if (user.isSamePassword(password))
		return user.getRemoteGuid();
	}
	return "00000000-0000-0000-0000-000000000000";

    }

    public double GetPrice(Customer ct, Item item, String barcode)
    {
	if (isOnline)
	{
	    HashMap<String, Object> params = new HashMap<String, Object>();
	    params.put("CustomerID", ct.getRemoteGuid());
	    params.put("ItemOid", item.getRemoteOid());
	    params.put("barcode", barcode);
	    Object result = callWebMethod("GetPriceCustomer", params);

	    return Double.parseDouble(result.toString());
	}
	return 0.;
    }

    public CustomerInsertResult CreateCustomer(String customer_taxcode, String customer_code, String customer_pc, String customer_store, String user)
    {
	if (!isOnline)
	    return CustomerInsertResult.NETWORKERROR;
	HashMap<String, Object> params = new HashMap<String, Object>();
	params.put("taxCode", customer_taxcode);
	params.put("customer_code", customer_code);
	params.put("userid", user);
	params.put("store", customer_store);
	params.put("priceCatalog", customer_pc);
	Object result = callWebMethod("CreateCustomer", params);

	if (result == null)
	    return CustomerInsertResult.NETWORKERROR;
	try
	{
	    int value = Integer.parseInt(result.toString());
	    return CustomerInsertResult.fromValue(value);
	}
	catch (Exception ex)
	{
	    return CustomerInsertResult.OTHERERROR;
	}
    }

    public boolean SendOrder(String order)
    {
	if (!isOnline)
	    return false;
	HashMap<String, Object> params = new HashMap<String, Object>();
	params.put("data", order);
	Object result = callWebMethod("PostDocument", params);

	if (result == null || !(result.toString().equals("1")))
	{
	    return false;
	}
	return true;
    }


    public ArrayList<DocumentDetail> GetDocumentDetails(String customer, String barcode, String qty, Item item)
    {
	try
	{
	    HashMap<String, Object> params = new HashMap<String, Object>();
	    params.put("customer", customer);
	    params.put("barcode", barcode);
	    params.put("qty", qty);
	    Object result = callWebMethod("GetDocumentDetailCustomer", params);
	    if (result == null)
		return null;

	    SoapObject sresult = (SoapObject) result;
	    ArrayList<DocumentDetail> listdet = new ArrayList<DocumentDetail>();
	    for (int i = 0, limit = sresult.getPropertyCount(); i < limit; i++)
	    {
		SoapObject obj = (SoapObject) sresult.getProperty(i);
		DocumentDetail ct = new DocumentDetail();
		ct.setRemoteDeviceDocumentDetailGuid(UUID.randomUUID().toString());

		// ct.setID(Long.randomLong());

		ct.setFinalUnitPrice(Double.parseDouble(obj.getProperty("FinalUnitPrice").toString()));
		ct.setFirstDiscount(Double.parseDouble(obj.getProperty("FirstDiscount").toString()));
		ct.setGrossTotal(Double.parseDouble(obj.getProperty("GrossTotal").toString()));
		ct.setItemPrice(Double.parseDouble(obj.getProperty("UnitPrice").toString()));
		ct.setNetTotal(Double.parseDouble(obj.getProperty("NetTotal").toString()));
		ct.setNetTotalAfterDiscount(Double.parseDouble(obj.getProperty("NetTotalAfterDiscount").toString()));
		ct.setQty(Double.parseDouble(obj.getProperty("Qty").toString()));
		ct.setSecondDiscount(Double.parseDouble(obj.getProperty("SecondDiscount").toString()));
		ct.setTotalDiscount(Double.parseDouble(obj.getProperty("TotalDiscount").toString()));
		ct.setTotalVatAmount(Double.parseDouble(obj.getProperty("TotalVatAmount").toString()));
		ct.setUnitPriceAfterDiscount(Double.parseDouble(obj.getProperty("UnitPriceAfterDiscount").toString()));
		ct.setVatAmount(Double.parseDouble(obj.getProperty("VatAmount").toString()));
		ct.setVatFactor(Double.parseDouble(obj.getProperty("VatFactor").toString()));
		ct.setBarcode(obj.getProperty("Barcode").toString());

		if (listdet.size() > 0)
		{
		    ct.setLinkedLine(listdet.get(0));
		    String itemCode = obj.getProperty("ItemCode").toString();
		    String itemName = obj.getProperty("ItemName").toString();
		    String itemOid = obj.getProperty("ItemOid").toString();
		    Item itm = dbHelper.getItemByRemoteGuid(itemOid);
		    if (itm == null)
		    {
			itm = new Item();
			// itm.setID(Long.randomLong());
			itm.setCode(itemCode);
			itm.setName(itemName);
			itm.setLoweCaseName(itemName.toLowerCase());
			itm.setRemoteOid(itemOid);
			dbHelper.getItems().create(itm);
		    }
		    else
		    {
			itm.setCode(itemCode);
			itm.setName(itemName);
			dbHelper.getItems().update(itm);
		    }
		    ct.setItem(itm);

		}
		else
		{
		    ct.setItem(item);
		}
		listdet.add(ct);
	    }
	    return listdet;// result.toString();
	}
	catch (Exception ex)
	{
	    ex.printStackTrace();
	}
	return null;
    }

    public Item GetItemWithBarcode(String barcode, String CustomerID)
    {
	if (isOnline)
	{
	    HashMap<String, Object> params = new HashMap<String, Object>();
	    params.put("barcode", barcode);
	    params.put("Sfields", "Code,Name,Oid,DefaultBarcode,ItemImageOid");
	    params.put("CustomerID", CustomerID);
	    Object result = callWebMethod("GetItemWithBarcodeCustomer", params);
	    if (result != null)
	    {
		try
		{
		    DocumentBuilder docBuilder = DocumentBuilderFactory.newInstance().newDocumentBuilder();
		    Document xmldoc = docBuilder.parse(new ByteArrayInputStream(result.toString().getBytes()));
		    Item itm = new Item();
		    NodeList codeNodeList = xmldoc.getElementsByTagName("Code");
		    if (codeNodeList.getLength() == 1)
		    {
			Node node = codeNodeList.item(0);
			itm.setCode(node.getTextContent());
		    }
		    NodeList oidNodeList = xmldoc.getElementsByTagName("Oid");
		    if (oidNodeList.getLength() == 1)
		    {
			Node node = oidNodeList.item(0);
			itm.setRemoteOid(node.getTextContent());
		    }
		    NodeList ItemImageOidNodeList = xmldoc.getElementsByTagName("ItemImageOid");
		    if (ItemImageOidNodeList.getLength() == 1)
		    {
			Node node = ItemImageOidNodeList.item(0);
			itm.setImageOid(node.getTextContent());
		    }
		    NodeList nameNodeList = xmldoc.getElementsByTagName("Name");
		    if (nameNodeList.getLength() == 1)
		    {
			Node node = nameNodeList.item(0);
			itm.setName(node.getTextContent());
		    }
		    NodeList dbNodeList = xmldoc.getElementsByTagName("DefaultBarcode");
		    if (dbNodeList.getLength() == 1)
		    {
			Node node = dbNodeList.item(0);
			itm.setDefaultBarcode(node.getTextContent());
		    }
		    // local cache
		    Item cachedItem = dbHelper.getItemByRemoteGuid(itm.getRemoteOid());
		    if (cachedItem == null)
		    {
			// itm.setID(Long.randomLong());
			dbHelper.getItems().create(itm);
		    }
		    else
		    {
			itm.setID(cachedItem.getID());
			dbHelper.getItems().update(itm);
		    }

		    return itm;

		}
		catch (Exception e)
		{
		    // TODO Auto-generated catch block
		    e.printStackTrace();
		}
	    }
	}
	return null;
    }

//    public List<Customer> GetCustomersOfUser(User user, boolean force)
//    {
//	if (isOnline && (force || !saveData))
//	{
//	    try
//	    {
//		DatabaseConnection conn = dbHelper.getConnectionSource().getReadWriteConnection();
//		HashMap<String, Object> params = new HashMap<String, Object>();
//		params.put("userid", user.getRemoteGuid());
//		params.put("updatedOn", "" + dbHelper.getMaxCustomerUpdatedOn(user));
//
//		Object result = callWebMethod("GetCustomersOfUser", params);
//		// return result.toString();
//		SoapObject sresult = (SoapObject) result;
//		conn.setAutoCommit(false);
//		for (int i = 0, limit = sresult.getPropertyCount(); i < limit; i++)
//		{
//		    SoapObject obj = (SoapObject) sresult.getProperty(i);
//		    Customer ct;
//		    ct = dbHelper.getCustomerByRemoteGuid(obj.getProperty("Oid").toString());
//		    boolean newct;
//		    if (ct == null)
//		    {
//			newct = true;
//			ct = new Customer();
//			// ct.setID(Long.randomLong());
//		    }
//		    else
//		    {
//			newct = false;
//		    }
//
//		    ct.setCompanyName(obj.getProperty("CompanyName").toString());
//		    ct.setCode(obj.getProperty("Code").toString());
//		    ct.setRemoteGuid(obj.getProperty("Oid").toString());
//		    ct.setDefaultAddress(obj.getProperty("DefaultAddress").toString());
//		    ct.setDefaultPhone(obj.getProperty("DefaultPhone").toString());
//		    ct.setTaxCode(obj.getProperty("TaxCode").toString());
//		    ct.setUpdatedOn(Long.parseLong(obj.getProperty("UpdateOn").toString()));
//		    ct.setPcRemoteGuid(obj.getPropertyAsString("PriceListOid"));
//		    Store store = dbHelper.findObject(Store.class, "remoteGuid", obj.getProperty("StoreOid").toString(), FilterType.EQUALS);
//		    ct.setStore(store);
//		    // ct.setOwner(user);
//		    VATLevel vl = dbHelper.getVatLevelByRemoteGuid(obj.getPropertyAsString("VatLevel"));
//		    ct.setVl(vl);
//		    // PriceCatalog pc =
//		    // dbHelper.getPriceCatalogByRemoteGuid(obj.getPropertyAsString("PriceListOid"));
//		    // (obj.getProperyAsString("PriceListOid"));
//		    // ct.setPc(pc);
//		    if (newct)
//			dbHelper.getCustomers().create(ct);
//		    else
//			dbHelper.getCustomers().update(ct);
//		    if (i % COMMIT_SIZE == 0)
//		    {
//			conn.setAutoCommit(true);
//			conn.setAutoCommit(false);
//		    }
//		}
//		conn.setAutoCommit(true);
//
//		String query = "update customer set pc_id = (select id from PriceCatalog where PriceCatalog.remoteGuid = Customer.pcRemoteGuid)";
//		Log.i("Sync Items", "Executing: " + query);
//		dbHelper.getWritableDatabase().execSQL(query);
//
//	    }
//	    catch (Exception ex)
//	    {
//		ex.printStackTrace();
//		return new ArrayList<Customer>();
//	    }
//	}
//	return dbHelper.getLocalCustomersOfUser(user);
//
//    }
//
//    public List<Store> GetStoresOfUser(User user, boolean force)
//    {
//	if (isOnline && (force || !saveData))
//	{
//	    try
//	    {
//		// Stores
//		DatabaseConnection conn = dbHelper.getConnectionSource().getReadWriteConnection();
//		HashMap<String, Object> params = new HashMap<String, Object>();
//		params.put("updatedon", "" + dbHelper.getMaxStoresUpdatedOn());
//		params.put("userID", user.getRemoteGuid());
//		Object result = callWebMethod("getStores", params);
//		SoapObject sresult = (SoapObject) result;
//		int limit = sresult.getPropertyCount();
//		conn.setAutoCommit(false);
//		for (int i = 0; i < limit; i++)
//		{
//		    SoapObject obj = (SoapObject) sresult.getProperty(i);
//		    Store ct;
//		    ct = dbHelper.getStoreByRemoteGuid(obj.getProperty("Oid").toString());
//		    boolean newct;
//		    if (ct == null)
//		    {
//			newct = true;
//			ct = new Store();
//		    }
//		    else
//		    {
//			newct = false;
//		    }
//		    ct.setUpdatedOn(Long.parseLong(obj.getProperty("UpdatedOn").toString()));
//		    ct.setRemoteGuid(obj.getProperty("Oid").toString());
//		    ct.setCentralStore(Boolean.parseBoolean(obj.getProperty("IsCentralStore").toString()));
//		    ct.setCode(obj.getProperty("Code").toString());
//		    ct.setName(obj.getProperty("Name").toString());
//		    if (newct)
//			dbHelper.getStoresDao().create(ct);
//		    else
//			dbHelper.getStoresDao().update(ct);
//		    if (i % COMMIT_SIZE == 0)
//		    {
//			conn.setAutoCommit(true);
//			conn.setAutoCommit(false);
//		    }
//
//		}
//		conn.setAutoCommit(true);
//		// usa
//		params = new HashMap<String, Object>();
//		params.put("userID", user.getRemoteGuid());
//		result = callWebMethod("getUserStoreAccess", params);
//		sresult = (SoapObject) result;
//		limit = sresult.getPropertyCount();
//		conn.setAutoCommit(false);
//		for (int i = 0; i < limit; i++)
//		{
//		    SoapObject obj = (SoapObject) sresult.getProperty(i);
//		    UserStoreAccess ct;
//		    Store store = dbHelper.findObject(Store.class, "remoteGuid", obj.getProperty("StoreID"), FilterType.EQUALS);
//		    if (store != null)
//		    {
//			ct = dbHelper.findObject(UserStoreAccess.class, OperatorType.AND, new BinaryOperator("store_id", store.getID(), FilterType.EQUALS), new BinaryOperator("user_id", user.getID(), FilterType.EQUALS));
//			boolean newct;
//			if (ct == null)
//			{
//			    newct = true;
//			    ct = new UserStoreAccess();
//			}
//			else
//			{
//			    newct = false;
//			}
//			ct.setUpdatedOn(Long.parseLong(obj.getProperty("UpdatedOn").toString()));
//			ct.setStore(dbHelper.getStoreByRemoteGuid(obj.getProperty("StoreID").toString()));
//			ct.setUser(dbHelper.getUserByRemoteGuid(obj.getProperty("UserID").toString()));
//
//			if (newct)
//			    dbHelper.getUserStoreAccessDao().create(ct);
//			else
//			    dbHelper.getUserStoreAccessDao().update(ct);
//			if (i % COMMIT_SIZE == 0)
//			{
//			    conn.setAutoCommit(true);
//			    conn.setAutoCommit(false);
//			}
//		    }
//
//		}
//		conn.setAutoCommit(true);
//	    }
//	    catch (Exception ex)
//	    {
//		ex.printStackTrace();
//	    }
//
//	}
//	return dbHelper.getLocalStoresOfUser(user);
//
//    }

    public boolean DownloadPriceCatalogs(User user) throws SQLException
    {
	Integer[] values = { 0 };
	DatabaseConnection conn = dbHelper.getConnectionSource().getReadWriteConnection();
	HashMap<String, Object> params = new HashMap<String, Object>();
	params.put("updatedon", "0");
	params.put("userID", user.getRemoteGuid());
	Object result = callWebMethod("getPriceCatalogs", params);
	SoapObject sresult = (SoapObject) result;
	int limit = sresult.getPropertyCount();
	ArrayList<PriceCatalog> noParentYetAvailable = new ArrayList<PriceCatalog>();
	conn.setAutoCommit(false);
	for (int i = 0; i < limit; i++)
	{
	    SoapObject obj = (SoapObject) sresult.getProperty(i);
	    PriceCatalog ct;
	    ct = dbHelper.getPriceCatalogByRemoteGuid(obj.getProperty("Oid").toString());
	    boolean newct;
	    if (ct == null)
	    {
		newct = true;
		ct = new PriceCatalog();
		// ct.setID(Long.randomLong());
	    }
	    else
	    {
		newct = false;
	    }
	    ct.setName(obj.getProperty("Description").toString());
	    ct.setCode((obj.getProperty("Code").toString()));
	    ct.setRemoteGuid(obj.getProperty("Oid").toString());
	    ct.setUpdatedOn(Long.parseLong(obj.getProperty("UpdatedOn").toString()));

	    String parentOid = obj.getProperty("parentOid").toString();
	    ct.setRemoteParentGuid(parentOid);
	    if (!parentOid.equals("00000000-0000-0000-0000-000000000000"))
	    {
		PriceCatalog parent = dbHelper.getPriceCatalogByRemoteGuid(parentOid);
		if (parent != null)
		{
		    ct.setParent(parent);
		}
		else
		{
		    noParentYetAvailable.add(ct);
		}

	    }

	    if (newct)
		dbHelper.getPriceCatalogDao().create(ct);
	    else
		dbHelper.getPriceCatalogDao().update(ct);
	    // currentRecords++;
	    // if (currentRecords > totalRecords)
	    // currentRecords = totalRecords;
	    if (i % COMMIT_SIZE == 0)
	    {
		// if(totalRecords >0)
		// values[0] = (99 * currentRecords) / totalRecords;
		conn.setAutoCommit(true);
		conn.setAutoCommit(false);
	    }

	}
	if (totalRecords > 0)
	    values[0] = (99 * currentRecords) / totalRecords;
	conn.setAutoCommit(true);
	conn.setAutoCommit(false);
	for (int i = 0; i < noParentYetAvailable.size(); i++)
	{
	    PriceCatalog toproc = noParentYetAvailable.get(i), parent;
	    parent = dbHelper.getPriceCatalogByRemoteGuid(toproc.getRemoteParentGuid());
	    if (parent != null)
	    {
		toproc.setParent(parent);
		dbHelper.getPriceCatalogDao().update(toproc);

	    }
	}
	if (totalRecords > 0)
	    values[0] = (99 * currentRecords) / totalRecords;
	conn.setAutoCommit(true);
	return true;
    }

    public boolean DownloadVATs(User user)
    {
	Integer[] values = { 0 };
	try
	{
	    // VATCategory
	    DatabaseConnection conn = dbHelper.getConnectionSource().getReadWriteConnection();
	    HashMap<String, Object> params = new HashMap<String, Object>();
	    params.put("updatedon", "0");
	    params.put("userID", user.getRemoteGuid());
	    Object result = callWebMethod("getVatCategories", params);
	    SoapObject sresult = (SoapObject) result;
	    int limit = sresult.getPropertyCount();
	    conn.setAutoCommit(false);
	    for (int i = 0; i < limit; i++)
	    {
		SoapObject obj = (SoapObject) sresult.getProperty(i);
		VATCategory ct;
		ct = dbHelper.getVatCategoryByRemoteGuid(obj.getProperty("Oid").toString());
		boolean newct;
		if (ct == null)
		{
		    newct = true;
		    ct = new VATCategory();
		    // ct.setID(Long.randomLong());
		}
		else
		{
		    newct = false;
		}
		ct.setName(obj.getProperty("Description").toString());
		ct.setRemoteGuid(obj.getProperty("Oid").toString());
		ct.setUpdatedOn(Long.parseLong(obj.getProperty("UpdatedOn").toString()));
		if (newct)
		    dbHelper.getVATCategoryDao().create(ct);
		else
		    dbHelper.getVATCategoryDao().update(ct);
		// currentRecords++;
		// if (currentRecords > totalRecords)
		// currentRecords = totalRecords;
		if (i % COMMIT_SIZE == 0)
		{
		    // if(totalRecords >0)
		    // values[0] = (99 * currentRecords) / totalRecords;
		    conn.setAutoCommit(true);
		    conn.setAutoCommit(false);
		}

	    }
	    // if(totalRecords >0)
	    // values[0] = (100 * currentRecords) / totalRecords;
	    conn.setAutoCommit(true);

	    // VATLevel
	    conn = dbHelper.getConnectionSource().getReadWriteConnection();
	    params = new HashMap<String, Object>();
	    params.put("updatedon", "0");
	    params.put("userID", user.getRemoteGuid());
	    result = callWebMethod("getVatLevels", params);
	    sresult = (SoapObject) result;
	    limit = sresult.getPropertyCount();
	    conn.setAutoCommit(false);
	    for (int i = 0; i < limit; i++)
	    {
		SoapObject obj = (SoapObject) sresult.getProperty(i);
		VATLevel ct;
		ct = dbHelper.getVatLevelByRemoteGuid(obj.getProperty("Oid").toString());
		boolean newct;
		if (ct == null)
		{
		    newct = true;
		    ct = new VATLevel();
		    // ct.setID(Long.randomLong());
		}
		else
		{
		    newct = false;
		}
		ct.setName(obj.getProperty("Description").toString());
		ct.setRemoteGuid(obj.getProperty("Oid").toString());
		ct.setUpdatedOn(Long.parseLong(obj.getProperty("UpdatedOn").toString()));
		ct.setDefault(Boolean.parseBoolean(obj.getProperty("IsDefault").toString()));
		if (newct)
		    dbHelper.getVATLevelDao().create(ct);
		else
		    dbHelper.getVATLevelDao().update(ct);
		// currentRecords++;
		if (i % COMMIT_SIZE == 0)
		{
		    // if (currentRecords > totalRecords)
		    // currentRecords = totalRecords;
		    // if(totalRecords >0)
		    // values[0] = (99 * currentRecords) / totalRecords;
		    conn.setAutoCommit(true);
		}

	    }
	    // if (currentRecords > totalRecords)
	    // currentRecords = totalRecords;
	    // if(totalRecords >0)
	    // values[0] = (99 * currentRecords) / totalRecords;
	    conn.setAutoCommit(true);

	    // VATFactors
	    conn = dbHelper.getConnectionSource().getReadWriteConnection();
	    params = new HashMap<String, Object>();
	    params.put("updatedon", "0");
	    params.put("userID", user.getRemoteGuid());
	    result = callWebMethod("getVatFactors", params);
	    sresult = (SoapObject) result;
	    limit = sresult.getPropertyCount();
	    conn.setAutoCommit(false);
	    for (int i = 0; i < limit; i++)
	    {
		SoapObject obj = (SoapObject) sresult.getProperty(i);
		VATFactor ct;
		ct = dbHelper.getVatFactorByRemoteGuid(obj.getProperty("Oid").toString());
		boolean newct;
		if (ct == null)
		{
		    newct = true;
		    ct = new VATFactor();
		    // ct.setID(Long.randomLong());
		}
		else
		{
		    newct = false;
		}
		ct.setRemoteGuid(obj.getProperty("Oid").toString());
		ct.setUpdatedOn(Long.parseLong(obj.getProperty("UpdatedOn").toString()));
		ct.setVatFactor(Double.parseDouble(obj.getProperty("VatFactor").toString()));
		VATCategory vc = dbHelper.getVatCategoryByRemoteGuid(obj.getProperty("VatCategoryOid").toString());
		ct.setVatCategory(vc);
		VATLevel vl = dbHelper.getVatLevelByRemoteGuid(obj.getProperty("VatLevelOid").toString());
		ct.setVatLevel(vl);
		if (newct)
		    dbHelper.getVATFactorDao().create(ct);
		else
		    dbHelper.getVATFactorDao().update(ct);
		// currentRecords++;
		// if (currentRecords > totalRecords)
		// currentRecords = totalRecords;
		if (i % COMMIT_SIZE == 0)
		{
		    // if (currentRecords > totalRecords)
		    // currentRecords = totalRecords;
		    // if(totalRecords >0)
		    // values[0] = (99 * currentRecords) / totalRecords;
		    conn.setAutoCommit(true);
		}
	    }
	    // if (currentRecords > totalRecords)
	    // currentRecords = totalRecords;
	    // if(totalRecords >0)
	    // values[0] = (99 * currentRecords) / totalRecords;
	    conn.setAutoCommit(true);
	    return true;
	}
	catch (Exception ex)
	{
	    ex.printStackTrace();
	    return false;
	}

    }

    public int SynchronizeStatus()
    {
	try
	{
	    Object result = callWebMethod("GetDocumentStatus", null);
	    SoapObject sresult = (SoapObject) result;
	    int limit = sresult.getPropertyCount();
	    for (int i = 0; i < limit; i++)
	    {
		SoapObject obj = (SoapObject) sresult.getProperty(i);
		DocumentStatus status = dbHelper.getDocumentStatusByRemoteGuid(obj.getProperty("Oid").toString());
		boolean newstatus = false;
		if (status == null)
		{
		    newstatus = true;
		    status = new DocumentStatus();
		    // status.setID(Long.randomLong());
		    status.setRemoteGuid(obj.getProperty("Oid").toString());
		}

		status.setDescription(obj.getProperty("Description").toString());
		status.setIsDefault(Boolean.parseBoolean((obj.getProperty("IsDefault").toString())));
		if (newstatus)
		    dbHelper.getDocumentStatuses().create(status);
		else
		    dbHelper.getDocumentStatuses().update(status);
	    }
	    return limit;
	}
	catch (Exception ex)
	{
	    return -1;
	}

    }

    public void UpdateStatus(Context ctx)
    {
	ConnectivityManager manager = (ConnectivityManager) ctx.getSystemService(Context.CONNECTIVITY_SERVICE);
	NetworkInfo info = manager.getActiveNetworkInfo();
	if (info == null || !info.isConnected())
	{
	    isOnline = false;
	}
	else
	{
	    isOnline = true;
	    if (info.getType() == ConnectivityManager.TYPE_MOBILE || info.getType() == ConnectivityManager.TYPE_MOBILE_MMS)
		saveData = true;
	    else
		saveData = false;
	}
    }

    private void startSyncAll(User user)
    {
	try
	{
	    // SynchronizerFromMultiTextWithTempTables sync = new
	    // SynchronizerFromMultiTextWithTempTables();
	    SynchronizerFromMultiText sync = new SynchronizerFromMultiText();

	    // SynchronizerFromText sync = new SynchronizerFromText();

	    // Synchronizer sync = new Synchronizer();
	    sync.execute(user);
	}
	catch (Exception e)
	{
	    e.printStackTrace();
	}
    }

    private int estimateTransfer(User currentUser)
    {
	int totalRecords;
	try
	{
	    totalRecords = getTotalUpdates(dbHelper.getMaxCustomerUpdatedOn(), dbHelper.getMaxItemUpdatedOn(), dbHelper.getMaxItemAnalyticTreesUpdatedOn(), dbHelper.getMaxBarcodeUpdatedOn(), dbHelper.getMaxItemCategoriesUpdatedOn(), dbHelper.getMaxPriceCatalog(), dbHelper.getMaxPriceCatalogDetail(), dbHelper.getMaxVatCategory(), dbHelper.getMaxVatFactor(), dbHelper.getMaxVatLevel(), dbHelper.getMaxOffersUpdatedOn(), dbHelper.getMaxOfferDetailsUpdatedOn(), dbHelper.getMaxDocumentStatusUpdatedOn(), dbHelper.getMaxStoresUpdatedOn(), dbHelper.getMaxLinkedItem(), dbHelper.getMaxMeasurementUnitUpdatedOn(), dbHelper.getMaxDocumentTypesUpdatedOn(),currentUser);
	    return totalRecords * BYTES_PER_RECORD;
	}
	catch (Exception e)
	{
	    Log.e("getTotalUpdates", "GetTotalUpdates Failed");
	    return -1;
	}
    }

    public void SynchronizeAll(User user)
    {
		final User usr = user;	
		if (!isOnline)
		{
			Utilities.ShowSimpleDialog(mContext, mContext.getResources().getString(R.string.noConnectionMessage));
		}
		else 
		{
			List<DocumentHeader> allHeaders;
			try
			{
				allHeaders = dbHelper.getDocumentHeaders().queryForAll();
			}
			catch (SQLException e)
			{
				e.printStackTrace();
				Utilities.ShowSimpleDialog(mContext, mContext.getResources().getString(R.string.problem) + ": " + e.getMessage());
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
			
			if(totalOrdersCount > 0)
			{
				Utilities.ShowSimpleDialog(mContext, mContext.getResources().getString(R.string.youCannotUpdatewhileYouHaveOrders));
			}
			else if (saveData)
			{
			    double estimatedBytes = estimateTransfer(user);
			    String message = mContext.getResources().getString(R.string.warning3G), replace = "";
			    if (estimatedBytes < 0)
			    {
				replace = mContext.getResources().getString(R.string.unavailable);
			    }
			    else
			    {
				String ext = "B";
				if (estimatedBytes > 1000)
				{
				    estimatedBytes /= 1024.0;
				    ext = "KB";
				    if (estimatedBytes > 1000)
				    {
					estimatedBytes /= 1024.0;
					ext = "MB";
				    }
				}
				replace = "~" + RetailHelper.qtyFormat.format(estimatedBytes) + " " + ext;
			    }
			    message = message.replace("var1", replace);
			    AlertDialog ad = new AlertDialog.Builder(mContext).create();
			    ad.setCancelable(false); // This blocks the 'BACK' button
			    ad.setMessage(message);
			    ad.setButton(mContext.getResources().getString(R.string.no), new DialogInterface.OnClickListener()
			    {
				public void onClick(DialogInterface dialog, int which)
				{
				    dialog.dismiss();
				}
			    });
			    ad.setButton2(mContext.getResources().getString(R.string.yes), new DialogInterface.OnClickListener()
			    {
				public void onClick(DialogInterface dialog, int which)
				{
				    dialog.dismiss();
				    startSyncAll(usr);
		
				}
			    });
			    ad.show();
			}
			else
			{
			    String message = mContext.getResources().getString(R.string.updateConfirmation);
			    AlertDialog ad = new AlertDialog.Builder(mContext).create();
			    ad.setCancelable(false); // This blocks the 'BACK' button
			    ad.setMessage(message);
			    ad.setButton(mContext.getResources().getString(R.string.no), new DialogInterface.OnClickListener()
			    {
				public void onClick(DialogInterface dialog, int which)
				{
				    dialog.dismiss();
				}
			    });
			    ad.setButton2(mContext.getResources().getString(R.string.yes), new DialogInterface.OnClickListener()
			    {
				public void onClick(DialogInterface dialog, int which)
				{
				    dialog.dismiss();
				    startSyncAll(usr);
				}
			    });
			    ad.show();
			    //startSyncAll(user);
			}
	    }

    }

    public List<TDocHead> getDocumentHeaders(Customer selectedCustomer, User user)
    {
	ArrayList<TDocHead> list = new ArrayList<TDocHead>();
	HashMap<String, Object> parameters = new HashMap<String, Object>();
	parameters.put("customerid", selectedCustomer.getRemoteGuid());
	parameters.put("userid", user.getRemoteGuid());
	Object result = callWebMethod("GetDocumentHeaders", parameters);
	if (result != null)
	{
	    SoapObject sresult = (SoapObject) result;
	    int limit = sresult.getPropertyCount();
	    for (int i = 0; i < limit; i++)
	    {
		SoapObject element = (SoapObject) sresult.getProperty(i);
		TDocHead tdh = new TDocHead();
		long finalizedDateTime = Long.parseLong(element.getPropertyAsString("finalizedDateTime"));
		double netTotal = Double.parseDouble(element.getPropertyAsString("netTotal"));
		double discount = Double.parseDouble(element.getPropertyAsString("discount"));
		double finalamount = Double.parseDouble(element.getPropertyAsString("final"));
		tdh.setGrossTotal(finalamount);
		tdh.setDiscountTotal(discount);
		tdh.setNetTotal(netTotal);
		tdh.setTicks(finalizedDateTime);
		list.add(tdh);
	    }
	}

	return list;
    }

    public String getDocumentHeadersXML(Customer selectedCustomer, User user)
    {
	HashMap<String, Object> parameters = new HashMap<String, Object>();
	parameters.put("customerid", selectedCustomer.getRemoteGuid());
	parameters.put("userid", user.getRemoteGuid());
	Object result = callWebMethod("GetDocumentHeadersXML", parameters);
	if (result != null)
	{
	    return result.toString();
	}
	else
	{
	    return null;
	}
    }
    
    public String getDocumentHeadersXMLWithFilters(Customer selectedCustomer, User user,long fromTicks,long toTicks,String docStatusID,String docTypeID,String hasBeenChecked,String hasBeenExecuted)
    {
	HashMap<String, Object> parameters = new HashMap<String, Object>();
	parameters.put("customerid", selectedCustomer.getRemoteGuid());
	parameters.put("userid", user.getRemoteGuid());
	parameters.put("fromTicks", fromTicks);
	parameters.put("toTicks", toTicks);
	parameters.put("docStatusID", docStatusID);
	parameters.put("docTypeID", docTypeID);
	parameters.put("hasBeenChecked", hasBeenChecked);
	parameters.put("hasBeenExecuted", hasBeenExecuted);
	
	Object result = callWebMethod("GetDocumentHeadersXMLWithFilters", parameters);
	if (result != null)
	{
	    return result.toString();
	}
	else
	{
	    return null;
	}
    }

    private <T extends IRetailPersistent> boolean SynchrGonizeActiveInactive(Dao<T, Long> dao)
    {
	ArrayList<String> elementsToSend = new ArrayList<String>();
	for (IRetailPersistent obj : dao)
	{
	    elementsToSend.add(obj.getRemoteGuid());
	}

	return true;
    }

}
