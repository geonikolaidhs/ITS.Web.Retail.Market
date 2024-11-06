package gr.net.its.retail.data;

import gr.net.its.retail.data.Address;
import gr.net.its.retail.MainActivity;
import gr.net.its.retail.R;
import gr.net.its.retail.RetailHelper;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.lang.reflect.Method;
import java.lang.reflect.ParameterizedType;
import java.nio.channels.FileChannel;
import java.sql.SQLException;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.UUID;

import android.app.AlertDialog;
import android.app.ProgressDialog;
import android.content.Context;
import android.content.DialogInterface;
import android.database.Cursor;
import android.database.DatabaseUtils;
import android.database.sqlite.SQLiteDatabase;
import android.os.AsyncTask;
import android.os.Environment;
import android.util.Log;
import android.widget.Toast;

import com.j256.ormlite.android.AndroidDatabaseResults;
import com.j256.ormlite.android.apptools.*;
import com.j256.ormlite.dao.CloseableIterator;
import com.j256.ormlite.dao.Dao;
import com.j256.ormlite.dao.GenericRawResults;
import com.j256.ormlite.stmt.PreparedQuery;
import com.j256.ormlite.stmt.QueryBuilder;
import com.j256.ormlite.stmt.StatementBuilder;
import com.j256.ormlite.stmt.Where;
import com.j256.ormlite.support.ConnectionSource;
import com.j256.ormlite.table.*;

public class DatabaseHelper extends OrmLiteSqliteOpenHelper
{

	private static final String DATABASE_NAME = "Retail.db";
	public static final int MAX_ITEMS = 50;// 400;
	private static final String DATABASE_PATH = "/data/data/gr.net.its.retail/databases/" + DATABASE_NAME;
	// private static final String DATABASE_EXPORT_PATH =
	// "/gr.net.its.retail.export/";
	private static final int DATABASE_VERSION = 11;
	private final String LOG_NAME = getClass().getName();
	private MainActivity context;
	private Dao<Customer, Long> customerDao;
	private Dao<Item, Long> itemDao;
	private Dao<DocumentHeader, Long> documentHeadersDao;
	private Dao<User, Long> usersDao;
	private Dao<DocumentDetail, Long> detailsDao;
	private Dao<ItemCategory, Long> itemCategoryDao;
	private Dao<ItemAnalyticTree, Long> itemAnalyticTreeDao;
	private Dao<DocumentStatus, Long> documentStatusDao;
	private Dao<DocumentType, Long> documentTypeDao;
	private Dao<Barcode, Long> barcodesDao;
	private Dao<ApplicationSettings, Long> appsettingsDao;
	private Dao<Offer, Long> offersDao;
	private Dao<OfferDetail, Long> offerDetailsDao;
	private Dao<Store, Long> storesDao;
	private Dao<UserStoreAccess, Long> userStoreAccessDao;
	private Dao<MeasurementUnit, Long> measurementUnitDao;

	private Dao<PriceCatalog, Long> PriceCatalogDao;

	private Dao<PriceCatalogDetail, Long> PriceCatalogDetailDao;
	private Dao<VATCategory, Long> VATCategoryDao;
	private Dao<VATFactor, Long> VATFactorDao;
	private Dao<VATLevel, Long> VATLevelDao;
	private Dao<LinkedItem, Long> LinkedItemDao;
	private Dao<Address, Long> AddressDao;

	public Dao<Address, Long> getAddresstDao() throws SQLException
	{
		if (AddressDao == null)
			AddressDao = getDao(Address.class);
		return AddressDao;
	}

	public Dao<MeasurementUnit, Long> getMeasurementUnitDao() throws SQLException
	{
		if (measurementUnitDao == null)
			measurementUnitDao = getDao(MeasurementUnit.class);
		return measurementUnitDao;
	}

	public Dao<PriceCatalog, Long> getPriceCatalogDao() throws SQLException
	{
		if (PriceCatalogDao == null)
			PriceCatalogDao = getDao(PriceCatalog.class);
		return PriceCatalogDao;
	}

	public String getDatabaseName()
	{
		return DATABASE_NAME;
	}

	public String getDataBasePath()
	{
		return DATABASE_PATH;
	}

	public Dao<PriceCatalogDetail, Long> getPriceCatalogDetailDao() throws SQLException
	{
		if (PriceCatalogDetailDao == null)
			PriceCatalogDetailDao = getDao(PriceCatalogDetail.class);
		return PriceCatalogDetailDao;
	}

	public Dao<VATCategory, Long> getVATCategoryDao() throws SQLException
	{
		if (VATCategoryDao == null)
			VATCategoryDao = getDao(VATCategory.class);
		return VATCategoryDao;
	}

	public Dao<Offer, Long> getOffersDao() throws SQLException
	{
		if (offersDao == null)
			offersDao = getDao(Offer.class);
		return offersDao;
	}

	public Dao<OfferDetail, Long> getOfferDetailsDao() throws SQLException
	{
		if (offerDetailsDao == null)
			offerDetailsDao = getDao(OfferDetail.class);
		return offerDetailsDao;
	}

	public Dao<Store, Long> getStoresDao() throws SQLException
	{
		if (storesDao == null)
		{
			Log.i("Store Dao", "start");
			storesDao = getDao(Store.class);
			Log.i("Store Dao", "end");
		}
		return storesDao;
	}

	public Dao<UserStoreAccess, Long> getUserStoreAccessDao() throws SQLException
	{
		if (userStoreAccessDao == null)
			userStoreAccessDao = getDao(UserStoreAccess.class);
		return userStoreAccessDao;
	}

	public Dao<VATFactor, Long> getVATFactorDao() throws SQLException
	{
		if (VATFactorDao == null)
			VATFactorDao = getDao(VATFactor.class);
		return VATFactorDao;
	}

	public Dao<VATLevel, Long> getVATLevelDao() throws SQLException
	{
		if (VATLevelDao == null)
			VATLevelDao = getDao(VATLevel.class);
		return VATLevelDao;
	}

	public Dao<LinkedItem, Long> getLinkedItemDao() throws SQLException
	{
		if (LinkedItemDao == null)
			LinkedItemDao = getDao(LinkedItem.class);
		return LinkedItemDao;
	}

	public DatabaseHelper(Context context)
	{
		super(context, DATABASE_NAME, null, DATABASE_VERSION, R.raw.ormlite_config);
		ConnectionSource ct = this.getConnectionSource();
		this.context = (MainActivity) context;
		ct.getClass();

	}

	@Override
	public void onCreate(SQLiteDatabase sqLiteDatabase, ConnectionSource connectionSource)
	{
		try
		{
			TableUtils.createTable(connectionSource, User.class);
			TableUtils.createTable(connectionSource, Barcode.class);
			TableUtils.createTable(connectionSource, Customer.class);
			TableUtils.createTable(connectionSource, DocumentDetail.class);
			TableUtils.createTable(connectionSource, DocumentHeader.class);
			TableUtils.createTable(connectionSource, Item.class);
			TableUtils.createTable(connectionSource, ItemCategory.class);
			TableUtils.createTable(connectionSource, ItemAnalyticTree.class);
			TableUtils.createTable(connectionSource, DocumentStatus.class);
			TableUtils.createTable(connectionSource, DocumentType.class);
			TableUtils.createTable(connectionSource, ApplicationSettings.class);

			TableUtils.createTable(connectionSource, Offer.class);
			TableUtils.createTable(connectionSource, OfferDetail.class);
			TableUtils.createTable(connectionSource, Store.class);
			TableUtils.createTable(connectionSource, UserStoreAccess.class);

			TableUtils.createTable(connectionSource, VATCategory.class);
			TableUtils.createTable(connectionSource, VATLevel.class);
			TableUtils.createTable(connectionSource, VATFactor.class);

			TableUtils.createTable(connectionSource, PriceCatalog.class);
			TableUtils.createTable(connectionSource, PriceCatalogDetail.class);

			TableUtils.createTable(connectionSource, LinkedItem.class);
			TableUtils.createTable(connectionSource, MeasurementUnit.class);
			TableUtils.createTable(connectionSource, Address.class);
			sqLiteDatabase.execSQL("CREATE INDEX idx_address_guids ON address (customerRemoteGuid, remoteGuid);");

			// sqLiteDatabase.execSQL("CREATE TABLE 'Item_Temp' ('CreatedOn' BIGINT , 'UpdatedOn' BIGINT , 'ID' INTEGER PRIMARY KEY AUTOINCREMENT , 'Code' VARCHAR , 'Name' VARCHAR , 'loweCaseName' VARCHAR , 'remoteGuid' VARCHAR , 'imageOid' VARCHAR , 'defaultBarcode' VARCHAR , 'defaultBarcodeRemoteGuid' VARCHAR , 'isactive' SMALLINT , 'defaultBarcodeObject_id' BIGINT , 'vatCategory_id' BIGINT , 'vatCategoryRemoteGuid' VARCHAR , 'VatIncluded' SMALLINT , 'InsertedOn' BIGINT , 'packingQty' DOUBLE PRECISION , 'maxOrderQty' DOUBLE PRECISION )");
			// sqLiteDatabase.execSQL("CREATE TABLE 'Offer_Temp' ('ID' INTEGER PRIMARY KEY AUTOINCREMENT , 'CreatedOn' BIGINT , 'UpdatedOn' BIGINT , 'RemoteGuid' VARCHAR , 'Description' VARCHAR , 'Description2' VARCHAR , 'StartDate' BIGINT , 'EndDate' BIGINT , 'PriceCatalog_id' BIGINT , 'PriceCatalogRemoteGuid' VARCHAR , 'Active' SMALLINT )");
			// sqLiteDatabase.execSQL("CREATE TABLE 'OfferDetail_Temp' ('ID' INTEGER PRIMARY KEY AUTOINCREMENT , 'CreatedOn' BIGINT , 'UpdatedOn' BIGINT , 'RemoteGuid' VARCHAR , 'Item_id' BIGINT , 'ItemRemoteGuid' VARCHAR , 'Offer_id' BIGINT , 'OfferRemoteGuid' VARCHAR , 'Active' SMALLINT )");
			// sqLiteDatabase.execSQL("CREATE TABLE 'Store_Temp' ('CreatedOn' BIGINT , 'UpdatedOn' BIGINT , 'ID' INTEGER PRIMARY KEY AUTOINCREMENT , 'remoteGuid' VARCHAR , 'code' VARCHAR , 'name' VARCHAR , 'isCentralStore' SMALLINT )");
			// sqLiteDatabase.execSQL("CREATE TABLE 'address_Temp' ('UpdatedOn' BIGINT , 'ID' INTEGER PRIMARY KEY AUTOINCREMENT , 'customer_id' BIGINT , 'Address' VARCHAR , 'customerRemoteGuid' VARCHAR , 'remoteGuid' VARCHAR )");
			// sqLiteDatabase.execSQL("CREATE TABLE 'barcode_Temp' ('ID' INTEGER PRIMARY KEY AUTOINCREMENT , 'Code' VARCHAR , 'MeasurementUnit_id' BIGINT , 'measurementUnitRemoteGuid' VARCHAR , 'item_id' BIGINT , 'itemRemoteGuid' VARCHAR , 'CreatedOn' BIGINT , 'UpdatedOn' BIGINT , 'remoteGuid' VARCHAR )");
			// sqLiteDatabase.execSQL("CREATE TABLE 'customer_Temp' ('UpdatedOn' BIGINT , 'ID' INTEGER PRIMARY KEY AUTOINCREMENT , 'CompanyName' VARCHAR , 'lowerCompanyName' VARCHAR , 'TaxCode' VARCHAR , 'DefaultAddress' VARCHAR , 'DefaultPhone' VARCHAR , 'remoteGuid' VARCHAR , 'pc_id' BIGINT , 'vl_id' BIGINT , 'store_id' BIGINT , 'Code' VARCHAR , 'pcRemoteGuid' VARCHAR , 'storeRemoteGuid' VARCHAR , 'vatLevelRemoteGuid' VARCHAR )");
			// sqLiteDatabase.execSQL("CREATE TABLE 'documentstatus_Temp' ('CreatedOn' BIGINT , 'UpdatedOn' BIGINT , 'ID' INTEGER PRIMARY KEY AUTOINCREMENT , 'IsDefault' SMALLINT , 'Description' VARCHAR , 'remoteGuid' VARCHAR )");
			// sqLiteDatabase.execSQL("CREATE TABLE 'itemanalytictree_Temp' ('CreatedOn' BIGINT , 'UpdatedOn' BIGINT , 'ID' INTEGER PRIMARY KEY AUTOINCREMENT , 'remoteGuid' VARCHAR , 'itemCategory_id' BIGINT , 'item_id' BIGINT , 'itemRemoteGuid' VARCHAR , 'itemCategoryRemoteGuid' VARCHAR )");
			// sqLiteDatabase.execSQL("CREATE TABLE 'itemcategory_Temp' ('CreatedOn' BIGINT , 'UpdatedOn' BIGINT , 'ID' INTEGER PRIMARY KEY AUTOINCREMENT , 'Code' VARCHAR , 'Name' VARCHAR , 'remoteGuid' VARCHAR , 'remoteParentGuid' VARCHAR , 'parent_id' BIGINT )");
			// sqLiteDatabase.execSQL("CREATE TABLE 'linkeditem_Temp' ('ID' INTEGER PRIMARY KEY AUTOINCREMENT , 'remoteGuid' VARCHAR , 'itemGuid' VARCHAR , 'subItemGuid' VARCHAR , 'UpdatedOn' BIGINT )");
			// sqLiteDatabase.execSQL("CREATE TABLE 'measurementunit_Temp' ('ID' INTEGER PRIMARY KEY AUTOINCREMENT , 'remoteGuid' VARCHAR , 'UpdatedOn' BIGINT , 'Description' VARCHAR , 'supportDecimals' SMALLINT )");
			// sqLiteDatabase.execSQL("CREATE TABLE 'pricecatalog_Temp' ('ID' INTEGER PRIMARY KEY AUTOINCREMENT , 'Code' VARCHAR , 'Name' VARCHAR , 'CreatedOn' BIGINT , 'parent_id' BIGINT , 'UpdatedOn' BIGINT , 'remoteGuid' VARCHAR , 'remoteParentGuid' VARCHAR )");
			// sqLiteDatabase.execSQL("CREATE TABLE 'pricecatalogdetail_Temp' ('ID' INTEGER PRIMARY KEY AUTOINCREMENT , 'Code' VARCHAR , 'remoteGuid' VARCHAR , 'CreatedOn' BIGINT , 'UpdatedOn' BIGINT , 'barcodeRemoteGuid' VARCHAR , 'itemRemoteGuid' VARCHAR , 'pcRemoteGuid' VARCHAR , 'pc_id' BIGINT , 'bc_id' BIGINT , 'item_id' BIGINT , 'price' DOUBLE PRECISION , 'discount' DOUBLE PRECISION , 'VATIncluded' SMALLINT )");
			// sqLiteDatabase.execSQL("CREATE TABLE 'vatcategory_Temp' ('name' VARCHAR , 'id' INTEGER PRIMARY KEY AUTOINCREMENT , 'UpdatedOn' BIGINT , 'remoteGuid' VARCHAR )");
			// sqLiteDatabase.execSQL("CREATE TABLE 'vatfactor_Temp' ('VatLevel_id' BIGINT , 'VatCategory_id' BIGINT , 'id' INTEGER PRIMARY KEY AUTOINCREMENT , 'UpdatedOn' BIGINT , 'remoteGuid' VARCHAR , 'vatLevelRemoteGuid' VARCHAR , 'vatCategoryRemoteGuid' VARCHAR , 'vatFactor' DOUBLE PRECISION )");
			// sqLiteDatabase.execSQL("CREATE TABLE 'vatlevel_Temp' ('name' VARCHAR , 'id' INTEGER PRIMARY KEY AUTOINCREMENT , 'UpdatedOn' BIGINT , 'remoteGuid' VARCHAR , 'Default' SMALLINT )");
		}
		catch (SQLException e)
		{
			Log.e(LOG_NAME, "Could not create new tables", e);
		}
	}

	@Override
	public void onUpgrade(SQLiteDatabase sqLiteDatabase, ConnectionSource connectionSource, int oldVersion, int newVersion)
	{
		try
		{

			TableUtils.dropTable(connectionSource, UserStoreAccess.class, true);

			TableUtils.dropTable(connectionSource, User.class, true);
			TableUtils.dropTable(connectionSource, Item.class, true);
			TableUtils.dropTable(connectionSource, Barcode.class, true);
			TableUtils.dropTable(connectionSource, Customer.class, true);
			TableUtils.dropTable(connectionSource, DocumentDetail.class, true);
			TableUtils.dropTable(connectionSource, ItemCategory.class, true);
			TableUtils.dropTable(connectionSource, ItemAnalyticTree.class, true);
			TableUtils.dropTable(connectionSource, DocumentStatus.class, true);
			TableUtils.dropTable(connectionSource, DocumentType.class, true);
			TableUtils.dropTable(connectionSource, ApplicationSettings.class, true);

			TableUtils.dropTable(connectionSource, VATCategory.class, true);
			TableUtils.dropTable(connectionSource, VATLevel.class, true);
			TableUtils.dropTable(connectionSource, VATFactor.class, true);
			TableUtils.dropTable(connectionSource, Offer.class, true);
			TableUtils.dropTable(connectionSource, OfferDetail.class, true);
			TableUtils.dropTable(connectionSource, Store.class, true);

			TableUtils.dropTable(connectionSource, PriceCatalog.class, true);
			TableUtils.dropTable(connectionSource, PriceCatalogDetail.class, true);

			TableUtils.dropTable(connectionSource, LinkedItem.class, true);
			TableUtils.dropTable(connectionSource, MeasurementUnit.class, true);
			TableUtils.dropTable(connectionSource, Address.class, true);
			onCreate(sqLiteDatabase, connectionSource);
		}
		catch (Exception e)
		{
			Log.e(LOG_NAME, "Could not upgrade the tables", e);
		}
	}

	public Dao<Barcode, Long> getBarcodes() throws SQLException
	{
		if (barcodesDao == null)
		{
			barcodesDao = getDao(Barcode.class);
		}
		return barcodesDao;
	}

	public Dao<Customer, Long> getCustomers() throws SQLException
	{
		if (customerDao == null)
		{
			customerDao = getDao(Customer.class);
		}
		return customerDao;
	}

	public Dao<ApplicationSettings, Long> getApplicationSettings() throws SQLException
	{
		if (appsettingsDao == null)
		{
			appsettingsDao = getDao(ApplicationSettings.class);
		}
		return appsettingsDao;
	}

	public Dao<Item, Long> getItems() throws SQLException
	{
		if (itemDao == null)
		{
			itemDao = getDao(Item.class);
		}
		return itemDao;
	}

	public Dao<DocumentHeader, Long> getDocumentHeaders() throws SQLException
	{
		if (documentHeadersDao == null)
		{
			documentHeadersDao = getDao(DocumentHeader.class);
		}
		return documentHeadersDao;
	}

	public Dao<ItemCategory, Long> getItemCategories() throws SQLException
	{
		if (itemCategoryDao == null)
		{
			itemCategoryDao = getDao(ItemCategory.class);
		}
		return itemCategoryDao;
	}

	public Dao<ItemAnalyticTree, Long> getItemAnalyticTrees() throws SQLException
	{
		if (itemAnalyticTreeDao == null)
		{
			itemAnalyticTreeDao = getDao(ItemAnalyticTree.class);
		}
		return itemAnalyticTreeDao;
	}

	public Dao<User, Long> getUsers() throws SQLException
	{
		if (usersDao == null)
		{
			usersDao = getDao(User.class);
		}
		return usersDao;
	}

	public Dao<DocumentDetail, Long> getDetails() throws SQLException
	{
		if (detailsDao == null)
		{
			detailsDao = getDao(DocumentDetail.class);
		}
		return detailsDao;
	}

	public Dao<DocumentStatus, Long> getDocumentStatuses() throws SQLException
	{
		if (documentStatusDao == null)
		{
			documentStatusDao = getDao(DocumentStatus.class);
		}
		return documentStatusDao;
	}

	public Dao<DocumentType, Long> getDocumentTypes() throws SQLException
	{
		if (documentTypeDao == null)
		{
			documentTypeDao = getDao(DocumentType.class);
		}
		return documentTypeDao;
	}

	public User getByUsername(String username)
	{
		try
		{
			QueryBuilder<User, Long> qb = getUsers().queryBuilder();
			qb.where().eq("username", username);
			PreparedQuery<User> pq = qb.prepare();
			return getUsers().queryForFirst(pq);
		}
		catch (SQLException e)
		{
			// TODO: Exception Handling
			e.printStackTrace();
		}
		return null;
	}

	public User getUserByRemoteGuid(String remoteGuid)
	{
		try
		{
			QueryBuilder<User, Long> qb = getUsers().queryBuilder();
			qb.where().eq("remoteGuid", remoteGuid);
			PreparedQuery<User> pq = qb.prepare();
			return getUsers().queryForFirst(pq);
		}
		catch (SQLException e)
		{
			e.printStackTrace();
		}
		return null;
	}

	public Item getItemByRemoteGuid(String remoteGuid)
	{
		try
		{
			QueryBuilder<Item, Long> qb = getItems().queryBuilder();
			qb.where().eq("remoteGuid", remoteGuid);
			PreparedQuery<Item> pq = qb.prepare();
			return getItems().queryForFirst(pq);
		}
		catch (SQLException e)
		{
			e.printStackTrace();
		}
		return null;
	}

	public Customer getCustomerByRemoteGuid(String remoteGuid)
	{
		try
		{
			QueryBuilder<Customer, Long> qb = getCustomers().queryBuilder();
			qb.where().eq("remoteGuid", remoteGuid);
			PreparedQuery<Customer> pq = qb.prepare();
			return getCustomers().queryForFirst(pq);
		}
		catch (SQLException e)
		{
			e.printStackTrace();
		}
		return null;
	}

	public ItemCategory getItemCategoryByRemoteGuid(String remoteGuid)
	{
		try
		{
			QueryBuilder<ItemCategory, Long> qb = getItemCategories().queryBuilder();
			qb.where().eq("remoteGuid", remoteGuid);
			PreparedQuery<ItemCategory> pq = qb.prepare();
			return getItemCategories().queryForFirst(pq);
		}
		catch (SQLException e)
		{
			e.printStackTrace();
		}
		return null;
	}

	public DocumentStatus getDocumentStatusByRemoteGuid(String remoteGuid)
	{
		try
		{
			QueryBuilder<DocumentStatus, Long> qb = getDocumentStatuses().queryBuilder();
			qb.where().eq("remoteGuid", remoteGuid);
			PreparedQuery<DocumentStatus> pq = qb.prepare();
			return getDocumentStatuses().queryForFirst(pq);
		}
		catch (SQLException e)
		{
			e.printStackTrace();
		}
		return null;
	}

	public DocumentType getDocumentTypeByRemoteGuid(String remoteGuid)
	{
		try
		{
			QueryBuilder<DocumentType, Long> qb = getDocumentTypes().queryBuilder();
			qb.where().eq("remoteGuid", remoteGuid);
			PreparedQuery<DocumentType> pq = qb.prepare();
			return getDocumentTypes().queryForFirst(pq);
		}
		catch (SQLException e)
		{
			e.printStackTrace();
		}
		return null;
	}

	public Barcode getBarcodeByRemoteGuid(String remoteGuid)
	{
		try
		{
			QueryBuilder<Barcode, Long> qb = getBarcodes().queryBuilder();
			qb.where().eq("remoteGuid", remoteGuid);
			PreparedQuery<Barcode> pq = qb.prepare();
			return getBarcodes().queryForFirst(pq);
		}
		catch (SQLException e)
		{
			e.printStackTrace();
		}
		return null;
	}

	public Barcode getBarcodeByCode(String code)
	{
		return getBarcodeByCode(code, 14, '0');
	}

	public Barcode getBarcodeByCode(String code, int paddingLength, char paddingChar)
	{
		String code2search = code;// org.apache.commons.lang3.StringUtils.leftPad(code,
		// paddingLength, paddingChar);
		try
		{
			QueryBuilder<Barcode, Long> qb = getBarcodes().queryBuilder();
			qb.where().eq("Code", code2search);
			PreparedQuery<Barcode> pq = qb.prepare();
			return getBarcodes().queryForFirst(pq);
		}
		catch (SQLException e)
		{
			e.printStackTrace();
		}
		return null;
	}

	public long getMaxCustomerUpdatedOn()
	{
		try
		{
			QueryBuilder<Customer, Long> qb = getCustomers().queryBuilder();
			qb.orderBy("UpdatedOn", false);
			PreparedQuery<Customer> pq = qb.prepare();
			long v = getCustomers().queryForFirst(pq).getUpdatedOn();
			return v;
		}
		catch (Exception e)
		{
			return 0;
		}
	}

	public long getMaxCustomerUpdatedOn(User user)
	{
		try
		{
			QueryBuilder<Customer, Long> qb = getCustomers().queryBuilder();
			qb.orderBy("UpdatedOn", false).where().eq("owner_id", user.getID());
			PreparedQuery<Customer> pq = qb.prepare();
			long v = getCustomers().queryForFirst(pq).getUpdatedOn();
			return v;
		}
		catch (Exception e)
		{
			return 0;
		}
	}

	public HashMap getAllMaxUpdatedOn()
	{
		HashMap<Class, Long> result = new HashMap<Class, Long>();
		List<Class> classes = new ArrayList<Class>();
		// classes.add(Address.class);
		// classes.add(ApplicationSettings.class);
		classes.add(Barcode.class);
		classes.add(Customer.class);
		// classes.add(DocumentDetail.class);
		// classes.add(DocumentHeader.class);
		classes.add(DocumentStatus.class);
		classes.add(DocumentType.class);
		classes.add(Item.class);
		classes.add(ItemAnalyticTree.class);
		classes.add(ItemCategory.class);
		classes.add(LinkedItem.class);
		classes.add(MeasurementUnit.class);
		classes.add(Offer.class);
		classes.add(OfferDetail.class);
		classes.add(PriceCatalog.class);
		classes.add(PriceCatalogDetail.class);
		classes.add(Store.class);
		// classes.add(User.class);
		classes.add(VATCategory.class);
		classes.add(VATFactor.class);
		classes.add(VATLevel.class);

		for (Class cl : classes)
		{
			try
			{
				Method method;
				method = DatabaseHelper.class.getMethod("getMaxUpdatedOn", cl.getClass());
				long updatedOn = (Long) method.invoke(this, cl);
				result.put(cl, updatedOn);
			}
			catch (Exception e)
			{
				e.printStackTrace();
				result.put(cl, -1L);
				continue;
			}
		}

		return result;

	}

	public <T extends IRetailPersistent> long getMaxUpdatedOn(Class<T> type)
	{

		try
		{
			Class<T> persistentClass = type;
			Dao<T, Long> dao = getDao(persistentClass);

			QueryBuilder<T, Long> qb = (QueryBuilder<T, Long>) dao.queryBuilder();
			qb.orderBy("UpdatedOn", false);
			PreparedQuery<T> pq = qb.prepare();
			T obj = dao.queryForFirst(pq);
			if (obj == null)
			{
				return 0;
			}
			else
			{
				return obj.getUpdatedOn();
			}
		}
		catch (SQLException e)
		{
			return 0;
		}
	}

	public long getMaxItemUpdatedOn()
	{
		try
		{
			QueryBuilder<Item, Long> qb = getItems().queryBuilder();
			qb.orderBy("UpdatedOn", false);
			PreparedQuery<Item> pq = qb.prepare();
			return getItems().queryForFirst(pq).getUpdatedOn();
		}
		catch (Exception e)
		{
			return 0;
		}
	}

	public long getMaxDocumentStatusUpdatedOn()
	{
		try
		{
			QueryBuilder<DocumentStatus, Long> qb = getDocumentStatuses().queryBuilder();
			qb.orderBy("UpdatedOn", false);
			PreparedQuery<DocumentStatus> pq = qb.prepare();
			return getDocumentStatuses().queryForFirst(pq).getUpdatedOn();
		}
		catch (Exception e)
		{
			return 0;
		}
	}

	public long getMaxDocumentTypesUpdatedOn()
	{
		try
		{
			QueryBuilder<DocumentType, Long> qb = getDocumentTypes().queryBuilder();
			qb.orderBy("UpdatedOn", false);
			PreparedQuery<DocumentType> pq = qb.prepare();
			return getDocumentTypes().queryForFirst(pq).getUpdatedOn();
		}
		catch (Exception e)
		{
			return 0;
		}
	}

	public long getMaxMeasurementUnitUpdatedOn()
	{
		try
		{
			QueryBuilder<MeasurementUnit, Long> qb = getMeasurementUnitDao().queryBuilder();
			qb.orderBy("UpdatedOn", false);
			PreparedQuery<MeasurementUnit> pq = qb.prepare();
			return getMeasurementUnitDao().queryForFirst(pq).getUpdatedOn();
		}
		catch (Exception e)
		{
			return 0;
		}
	}

	public long getMaxOffersUpdatedOn()
	{
		try
		{
			QueryBuilder<Offer, Long> qb = getOffersDao().queryBuilder();
			qb.orderBy("UpdatedOn", false);
			PreparedQuery<Offer> pq = qb.prepare();
			return getOffersDao().queryForFirst(pq).getUpdatedOn();
		}
		catch (Exception e)
		{
			return 0;
		}
	}

	public long getMaxStoresUpdatedOn()
	{
		try
		{
			QueryBuilder<Store, Long> qb = getStoresDao().queryBuilder();
			qb.orderBy("UpdatedOn", false);
			PreparedQuery<Store> pq = qb.prepare();
			return getStoresDao().queryForFirst(pq).getUpdatedOn();
		}
		catch (Exception e)
		{
			return 0;
		}
	}

	public long getMaxUserStoreAccessUpdatedOn()
	{
		try
		{
			QueryBuilder<UserStoreAccess, Long> qb = getUserStoreAccessDao().queryBuilder();
			qb.orderBy("UserStoreAccess", false);
			PreparedQuery<UserStoreAccess> pq = qb.prepare();
			return getUserStoreAccessDao().queryForFirst(pq).getUpdatedOn();
		}
		catch (Exception e)
		{
			return 0;
		}
	}

	public long getMaxOfferDetailsUpdatedOn()
	{
		try
		{
			QueryBuilder<OfferDetail, Long> qb = getOfferDetailsDao().queryBuilder();
			qb.orderBy("UpdatedOn", false);
			PreparedQuery<OfferDetail> pq = qb.prepare();
			return getOfferDetailsDao().queryForFirst(pq).getUpdatedOn();
		}
		catch (Exception e)
		{
			return 0;
		}
	}

	public long getMaxBarcodeUpdatedOn()
	{
		try
		{
			QueryBuilder<Barcode, Long> qb = getBarcodes().queryBuilder();
			qb.orderBy("UpdatedOn", false);
			PreparedQuery<Barcode> pq = qb.prepare();
			return getBarcodes().queryForFirst(pq).getUpdatedOn();
		}
		catch (Exception e)
		{
			return 0;
		}
	}

	public long getMaxItemAnalyticTreesUpdatedOn()
	{
		try
		{
			QueryBuilder<ItemAnalyticTree, Long> qb = getItemAnalyticTrees().queryBuilder();
			qb.orderBy("UpdatedOn", false);
			PreparedQuery<ItemAnalyticTree> pq = qb.prepare();
			return getItemAnalyticTrees().queryForFirst(pq).getUpdatedOn();
		}
		catch (Exception e)
		{
			return 0;
		}
	}

	public long getMaxItemCategoriesUpdatedOn()
	{
		try
		{
			QueryBuilder<ItemCategory, Long> qb = getItemCategories().queryBuilder();
			qb.orderBy("UpdatedOn", false);
			PreparedQuery<ItemCategory> pq = qb.prepare();
			return getItemCategories().queryForFirst(pq).getUpdatedOn();
		}
		catch (Exception e)
		{
			return 0;
		}
	}

	public DocumentHeader getDocumentHeaderByCustomer(Customer selectedCustomer)
	{
		try
		{
			QueryBuilder<DocumentHeader, Long> qb = getDocumentHeaders().queryBuilder();
			qb.where().eq("customer_id", selectedCustomer.getID());
			PreparedQuery<DocumentHeader> pq = qb.prepare();
			return getDocumentHeaders().queryForFirst(pq);
		}
		catch (SQLException e)
		{
			e.printStackTrace();
		}
		return null;
	}

	public List<Customer> getLocalCustomersOfUser(User user, String filter)
	{
		try
		{
			// QueryBuilder<Customer, Long> customerQb =
			// getCustomers().queryBuilder();
			// //Where customerWhere =customerQb.where();
			// //customerWhere.or(customerWhere.like("lowerCompanyName",
			// filter),customerWhere.like("TaxCode",
			// filter),customerWhere.like("Code", filter));
			//
			// QueryBuilder<UserStoreAccess, Long> usaQb =
			// getUserStoreAccessDao().queryBuilder();
			// // join with the order query
			// //usaQb.where().eq("user_id", user.getID());
			//
			// //List<Customer> results = customerQb.join(usaQb).where().and()
			// Where joinWhere = customerQb.join(usaQb).where();
			// joinWhere.and(joinWhere.eq("userstoreaccess.user_id",
			// user.getID()),joinWhere.or(joinWhere.like("customer.lowerCompanyName",
			// filter),joinWhere.like("customer.TaxCode",
			// filter),joinWhere.like("customer.Code", filter)));
			// PreparedQuery<Customer> pq = joinWhere.prepare();
			// List<Customer> customers = getCustomers().query(pq);
			/*
			 * GenericRawResults<Customer> rawResults = getCustomers().queryRaw(
			 * "SELECT customer.* FROM customer inner join userstoreaccess on customer.store_id = userstoreaccess.store_id "
			 * + "where userstoreaccess.user_id='"+user.getID()+
			 * "' and (customer.lowerCompanyName like '%"+filter+"%' or " +
			 * "customer.TaxCode like '%"
			 * +filter+"%' or customer.Code like '%"+filter+"%')",
			 * getCustomers().getRawRowMapper());
			 */
			String query = "SELECT customer.* FROM customer inner join userstoreaccess on customer.store_id = userstoreaccess.store_id " + "where userstoreaccess.user_id='" + user.getID() + "' and (customer.lowerCompanyName like '%" + filter.toLowerCase() + "%' or "
					+ "customer.TaxCode like '%" + filter + "%' or customer.Code like '%" + filter + "%') limit 100";
			// List<Customer> customers =rawResults.getResults();
			return rawQuery(getCustomers(), query);// customers;
		}
		catch (SQLException e)
		{
			e.printStackTrace();
		}
		return null;
	}

	public static <T> List<T> rawQuery(Dao<T, Long> dao, String query)
	{
		try
		{
			GenericRawResults<T> rawResults;
			rawResults = dao.queryRaw(query, dao.getRawRowMapper());
			return rawResults.getResults();
		}
		catch (Exception e)
		{
			e.printStackTrace();
			return new ArrayList<T>();
		}

	}

	public List<Customer> getLocalCustomersOfUser(User user)
	{
		try
		{
			// List<Customer> customers = new ArrayList<Customer>();
			// QueryBuilder<UserStoreAccess, Long> qb =
			// getUserStoreAccessDao().queryBuilder();
			// qb.where().eq("user_id", user.getID());
			// PreparedQuery<UserStoreAccess> pq = qb.prepare();
			// List<UserStoreAccess> usas = getUserStoreAccessDao().query(pq);
			// List<Store> userStores = new ArrayList<Store>();
			// // List<Long> storeIds = new ArrayList<Long>();
			// for (UserStoreAccess usa : usas)
			// {
			// userStores.add(usa.getStore());
			// // storeIds.add(usa.getStore().getID());
			// }
			// Log.i("For each", "Start");
			// for (Store store : userStores)
			// {
			// if (store.getName() == null)
			// {
			// Log.i("Store Query", "start");
			// store = getStoresDao().queryForId(store.getID());
			// Log.i("Store Query", "end");
			// }
			// customers.addAll(store.getCustomers());
			// }
			// Log.i("For each", "End");
			//
			// // Log.i("In operator","Start");
			// // QueryBuilder<Customer, Long> qb2 =
			// getCustomers().queryBuilder();
			// // qb.where().in("store_id", storeIds);
			// // PreparedQuery<Customer> pq2 = qb2.prepare();
			// // customers = getCustomers().query(pq2);
			// // Log.i("In operator","End");
			// //
			String query = "SELECT distinct customer.* FROM customer inner join userstoreaccess on customer.store_id = userstoreaccess.store_id left join documentheader on customer.ID = documentheader.customer_id" + " where userstoreaccess.user_id='" + user.getID()
					+ "' order by documentheader.GrossTotal desc ,customer.lowerCompanyName asc limit 100";
			// List<Customer> customers =rawResults.getResults();
			return rawQuery(getCustomers(), query);// customers;
		}
		catch (SQLException e)
		{
			e.printStackTrace();
		}
		return null;
	}

	public List<Store> getLocalStoresOfUser(User user)
	{
		try
		{
			// QueryBuilder<UserStoreAccess, Long> qb =
			// getUserStoreAccessDao().queryBuilder();
			// qb.where().eq("user_id", user.getID());
			// PreparedQuery<UserStoreAccess> pq = qb.prepare();
			// List<UserStoreAccess> usas = getUserStoreAccessDao().query(pq);
			// List<Store> userStores = new ArrayList<Store>();
			// for (UserStoreAccess usa : usas)
			// {
			// userStores.add(usa.getStore());
			// }
			String query = "SELECT store.* FROM store inner join userstoreaccess on store.id = userstoreaccess.store_id where userstoreaccess.user_id='" + user.getID() + "'";
			return rawQuery(getStoresDao(), query);
		}
		catch (SQLException e)
		{
			e.printStackTrace();
		}
		return null;
	}

	public List<UserStoreAccess> getLocalUserStoreAccesses(User user)
	{
		try
		{
			QueryBuilder<UserStoreAccess, Long> qb = getUserStoreAccessDao().queryBuilder();
			qb.where().eq("user_id", user.getID());
			PreparedQuery<UserStoreAccess> pq = qb.prepare();
			List<UserStoreAccess> usas = getUserStoreAccessDao().query(pq);
			return usas;
		}
		catch (SQLException e)
		{
			e.printStackTrace();
		}
		return null;
	}

	public DocumentStatus getDefaultDocumentStatus()
	{
		try
		{
			QueryBuilder<DocumentStatus, Long> qb = getDocumentStatuses().queryBuilder();
			qb.where().eq("IsDefault", true);
			PreparedQuery<DocumentStatus> pq = qb.prepare();
			Dao<DocumentStatus, Long> docstats = getDocumentStatuses();
			DocumentStatus dt = docstats.queryForFirst(pq);
			return dt;
		}
		catch (Exception e)
		{
			try
			{
				return getDocumentStatuses().queryForAll().get(0);
			}
			catch (Exception ex)
			{
				ex.printStackTrace();
				return null;
			}
		}

	}

	public ItemAnalyticTree getItemAnalyticTreeByRemoteGuid(String remoteGuid)
	{
		try
		{
			QueryBuilder<ItemAnalyticTree, Long> qb = getItemAnalyticTrees().queryBuilder();
			qb.where().eq("remoteGuid", remoteGuid);
			PreparedQuery<ItemAnalyticTree> pq = qb.prepare();
			return getItemAnalyticTrees().queryForFirst(pq);
		}
		catch (SQLException e)
		{
			e.printStackTrace();
		}
		return null;
	}

	public ApplicationSettings getSettings()
	{
		try
		{
			QueryBuilder<ApplicationSettings, Long> qb = getApplicationSettings().queryBuilder();
			PreparedQuery<ApplicationSettings> pq = qb.prepare();
			return getApplicationSettings().queryForFirst(pq);
		}
		catch (Exception ex)
		{
			ex.printStackTrace();
			return null;
		}
	}

	public double getUnitPrice(PriceCatalogDetail pcd)
	{
		if (!pcd.isVATIncluded())
		{
			return pcd.getPrice();
		}
		else
		{
			try
			{
				Item item = getItems().queryForId(pcd.getItem().getID());
				if (item.getVatCategory() == null)
				{
					return pcd.getPrice();
				}
				else
				{
					VATCategory vatCat = getVATCategoryDao().queryForId(item.getVatCategory().getID());
					VATLevel vatLevel = findObject(VATLevel.class, "Default", true, FilterType.EQUALS);
					if (vatLevel == null)
					{
						return pcd.getPrice();
					}
					else
					{
						QueryBuilder<VATFactor, Long> qb = getVATFactorDao().queryBuilder();
						qb.where().eq("VatLevel_id", vatLevel.getID()).and().eq("VatCategory_id", vatCat.getID());
						PreparedQuery<VATFactor> pq = qb.prepare();
						VATFactor factor = getVATFactorDao().queryForFirst(pq);
						if ((factor == null) || (factor.getVatFactor() == 0))
							return pcd.getPrice();
						else
							return (pcd.getPrice()) / (1 + Math.abs(factor.getVatFactor()));
					}
				}
			}
			catch (SQLException e)
			{

				e.printStackTrace();
				return -1;
			}

		}

	}

	public double getItemPrice(PriceCatalog pc, Barcode bc, Item itm)
	{
		try
		{
			// Force reload
			if (pc.getRemoteGuid() == null)
			{
				pc = getPriceCatalogDao().queryForId(pc.getID());
			}
			QueryBuilder<PriceCatalogDetail, Long> qb = getPriceCatalogDetailDao().queryBuilder();
			qb.where().eq("bc_id", bc.getID()).and().eq("pc_id", pc.getID());
			PreparedQuery<PriceCatalogDetail> pq = qb.prepare();
			PriceCatalogDetail pcd = getPriceCatalogDetailDao().queryForFirst(pq);
			if (pcd != null)
				return getUnitPrice(pcd);
			if(itm.getRemoteGuid()==null)
			{
				itm = getItems().queryForId(itm.getID());
			}
			if (itm.getDefaultBarcodeObject().getID() != bc.getID())
			{
				qb = getPriceCatalogDetailDao().queryBuilder();
				qb.where().eq("bc_id", itm.getDefaultBarcodeObject().getID()).and().eq("pc_id", pc.getID());
				pq = qb.prepare();
				pcd = getPriceCatalogDetailDao().queryForFirst(pq);
			}
			if (pcd != null)
				return getUnitPrice(pcd);
			if (pc.getParent() != null)
			{
				PriceCatalog ppc = getPriceCatalogDao().queryForId(pc.getParentCatalogID());
				return getItemPrice(ppc, bc, itm);
			}
			return -1;
		}
		catch (Exception ex)
		{
			ex.printStackTrace();
			return -2;
		}
	}

	public <T> T findObject(Class<T> type, String fieldName, Object fieldValue, FilterType filterType) throws SQLException
	{
		Class<T> persistentClass = type;
		QueryBuilder<T, Long> qb = (QueryBuilder<T, Long>) getDao(persistentClass).queryBuilder();
		switch (filterType)
		{
		case EQUALS:
			qb.where().eq(fieldName, fieldValue);
			break;
		case GREATER:
			qb.where().gt(fieldName, fieldValue);
			break;
		case GREATER_OR_EQUAL:
			qb.where().ge(fieldName, fieldValue);
			break;
		case LESS:
			qb.where().lt(fieldName, fieldValue);
			break;
		case LESS_OR_EQUAL:
			qb.where().le(fieldName, fieldValue);
			break;
		case LIKE:
			qb.where().like(fieldName, fieldValue);
			break;
		}
		PreparedQuery<T> pq = qb.prepare();
		return ((Dao<T, Long>) getDao(persistentClass)).queryForFirst(pq);

		// List<T> result = findObjects(type, fieldName, fieldValue,
		// filterType);
		// if (result != null && result.size() > 0)
		// {
		// return result.get(0);
		// }
		// else
		// {
		// return null;
		// }
	}

	public <T> T findObject(Class<T> type, OperatorType operatorType, BinaryOperator... binaryOperators) throws SQLException
	{
		Class<T> persistentClass = type;
		QueryBuilder<T, Long> qb = (QueryBuilder<T, Long>) getDao(persistentClass).queryBuilder();
		Where<T, Long> where = qb.where();
		int counter = 0;
		for (BinaryOperator bo : binaryOperators)
		{
			switch (bo.filterType)
			{
			case EQUALS:
				where = where.eq(bo.fieldName, bo.fieldValue);
				break;
			case GREATER:
				where = where.gt(bo.fieldName, bo.fieldValue);
				break;
			case GREATER_OR_EQUAL:
				where = where.ge(bo.fieldName, bo.fieldValue);
				break;
			case LESS:
				where = where.lt(bo.fieldName, bo.fieldValue);
				break;
			case LESS_OR_EQUAL:
				where = where.le(bo.fieldName, bo.fieldValue);
				break;
			case LIKE:
				where = where.like(bo.fieldName, bo.fieldValue);
				break;
			}
			if (counter < binaryOperators.length)
			{
				switch (operatorType)
				{
				case AND:
					where = where.and();
					break;
				case OR:
					where = where.or();
					break;
				}
			}
			counter++;
		}
		PreparedQuery<T> pq = qb.prepare();
		return ((Dao<T, Long>) getDao(persistentClass)).queryForFirst(pq);
		// List<T> result = findObjects(type, operatorType, binaryOperators);
		// if (result != null && result.size() > 0)
		// {
		// return result.get(0);
		// }
		// else
		// {
		// return null;
		// }
	}

	public <T> List<T> findObjects(Class<T> type, String fieldName, Object fieldValue, FilterType filterType) throws SQLException
	{
		Class<T> persistentClass = type;
		QueryBuilder<T, Long> qb = (QueryBuilder<T, Long>) getDao(persistentClass).queryBuilder();
		switch (filterType)
		{
		case EQUALS:
			qb.where().eq(fieldName, fieldValue);
			break;
		case GREATER:
			qb.where().gt(fieldName, fieldValue);
			break;
		case GREATER_OR_EQUAL:
			qb.where().ge(fieldName, fieldValue);
			break;
		case LESS:
			qb.where().lt(fieldName, fieldValue);
			break;
		case LESS_OR_EQUAL:
			qb.where().le(fieldName, fieldValue);
			break;
		case LIKE:
			qb.where().like(fieldName, fieldValue);
			break;
		}
		PreparedQuery<T> pq = qb.prepare();
		return ((Dao<T, Long>) getDao(persistentClass)).query(pq);
	}

	public <T> List<T> findObjects(Class<T> type, OperatorType operatorType, BinaryOperator... binaryOperators) throws SQLException
	{
		Class<T> persistentClass = type;
		QueryBuilder<T, Long> qb = (QueryBuilder<T, Long>) getDao(persistentClass).queryBuilder();
		Where<T, Long> where = qb.where();
		int counter = 0;
		for (BinaryOperator bo : binaryOperators)
		{
			switch (bo.filterType)
			{
			case EQUALS:
				where = where.eq(bo.fieldName, bo.fieldValue);
				break;
			case GREATER:
				where = where.gt(bo.fieldName, bo.fieldValue);
				break;
			case GREATER_OR_EQUAL:
				where = where.ge(bo.fieldName, bo.fieldValue);
				break;
			case LESS:
				where = where.lt(bo.fieldName, bo.fieldValue);
				break;
			case LESS_OR_EQUAL:
				where = where.le(bo.fieldName, bo.fieldValue);
				break;
			case LIKE:
				where = where.like(bo.fieldName, bo.fieldValue);
				break;
			}
			if (counter < binaryOperators.length)
			{
				switch (operatorType)
				{
				case AND:
					where = where.and();
					break;
				case OR:
					where = where.or();
					break;
				}
			}
			counter++;
		}
		PreparedQuery<T> pq = qb.prepare();
		return ((Dao<T, Long>) getDao(persistentClass)).query(pq);
	}

	public PriceCatalogDetail getItemPriceDetail(PriceCatalog pc, Barcode bc, Item itm)
	{
		try
		{
			// Force reload
			if (pc.getCode() == null)
			{
				pc = getPriceCatalogDao().queryForId(pc.getID());
			}
			QueryBuilder<PriceCatalogDetail, Long> qb = getPriceCatalogDetailDao().queryBuilder();
			qb.where().eq("pc_id", pc.getID()).and().eq("bc_id", bc.getID());
			PreparedQuery<PriceCatalogDetail> pq = qb.prepare();
			PriceCatalogDetail pcd = getPriceCatalogDetailDao().queryForFirst(pq);
			if (pcd != null)
				return pcd;
			if (itm.getDefaultBarcodeObject().getID() != bc.getID())
			{
				qb = getPriceCatalogDetailDao().queryBuilder();
				qb.where().eq("pc_id", pc.getID()).and().eq("bc_id", itm.getDefaultBarcodeObject().getID());
				pq = qb.prepare();
				pcd = getPriceCatalogDetailDao().queryForFirst(pq);
			}
			if (pcd != null)
				return pcd;
			if (pc.getParent() != null)
			{
				PriceCatalog ppc = getPriceCatalogDao().queryForId(pc.getParentCatalogID());
				return getItemPriceDetail(ppc, bc, itm);
			}
			return null;
		}
		catch (Exception ex)
		{
			ex.printStackTrace();
			return null;
		}
	}

	public Barcode getBarcodeFromItemCode(Item item, ApplicationSettings settings)
	{
		for (Barcode bc : item.getBarcodes())
		{
			if (RetailHelper.GetPaddedBarcode(bc.getCode(), settings) == RetailHelper.GetPaddedBarcode(item.getCode(), settings))
			{
				return bc;
			}
		}
		return null;
	}

	public DocumentDetail addDocumentDetail(Barcode bc, PriceCatalog pc, double qty) throws Exception
	{
		Item itm = getItems().queryForId(bc.getItem().getID());
		PriceCatalogDetail pcd = this.getItemPriceDetail(pc, bc, itm);
		if (pcd == null)
			throw new Exception("This product cannot be ordered right now");
		DocumentDetail dt = new DocumentDetail();
		dt.setRemoteDeviceDocumentDetailGuid(UUID.randomUUID().toString());
		// TODO: Add correct values
		dt.setBarcode(bc.getCode());
		dt.setFinalUnitPrice(pcd.getPrice());

		return dt;
	}

	public long getMaxPriceCatalogDetail()
	{
		try
		{
			QueryBuilder<PriceCatalogDetail, Long> qb = getPriceCatalogDetailDao().queryBuilder();
			qb.orderBy("UpdatedOn", false);
			PreparedQuery<PriceCatalogDetail> pq = qb.prepare();
			return getPriceCatalogDetailDao().queryForFirst(pq).getUpdatedOn();
		}
		catch (Exception e)
		{
			return 0;
		}
	}

	public long getMaxPriceCatalog()
	{
		try
		{
			QueryBuilder<PriceCatalog, Long> qb = getPriceCatalogDao().queryBuilder();
			qb.orderBy("UpdatedOn", false);
			PreparedQuery<PriceCatalog> pq = qb.prepare();
			return getPriceCatalogDao().queryForFirst(pq).getUpdatedOn();
		}
		catch (Exception e)
		{
			return 0;
		}
	}

	public long getMaxVatCategory()
	{
		try
		{
			QueryBuilder<VATCategory, Long> qb = getVATCategoryDao().queryBuilder();
			qb.orderBy("UpdatedOn", false);
			PreparedQuery<VATCategory> pq = qb.prepare();
			return getVATCategoryDao().queryForFirst(pq).getUpdatedOn();
		}
		catch (Exception e)
		{
			return 0;
		}
	}

	public long getMaxVatFactor()
	{
		try
		{
			QueryBuilder<VATFactor, Long> qb = getVATFactorDao().queryBuilder();
			qb.orderBy("UpdatedOn", false);
			PreparedQuery<VATFactor> pq = qb.prepare();
			return getVATFactorDao().queryForFirst(pq).getUpdatedOn();
		}
		catch (Exception e)
		{
			return 0;
		}

	}

	public long getMaxVatLevel()
	{
		try
		{
			QueryBuilder<VATLevel, Long> qb = getVATLevelDao().queryBuilder();
			qb.orderBy("UpdatedOn", false);
			PreparedQuery<VATLevel> pq = qb.prepare();
			return getVATLevelDao().queryForFirst(pq).getUpdatedOn();
		}
		catch (Exception e)
		{
			return 0;
		}
	}

	public long getMaxLinkedItem()
	{
		try
		{
			QueryBuilder<LinkedItem, Long> qb = getLinkedItemDao().queryBuilder();
			qb.orderBy("UpdatedOn", false);
			PreparedQuery<LinkedItem> pq = qb.prepare();
			return getLinkedItemDao().queryForFirst(pq).getUpdatedOn();
		}
		catch (Exception e)
		{
			return 0;
		}
	}

	public VATCategory getVatCategoryByRemoteGuid(String remoteGuid)
	{
		try
		{
			QueryBuilder<VATCategory, Long> qb = getVATCategoryDao().queryBuilder();
			qb.where().eq("remoteGuid", remoteGuid);
			PreparedQuery<VATCategory> pq = qb.prepare();
			return getVATCategoryDao().queryForFirst(pq);
		}
		catch (SQLException e)
		{
			e.printStackTrace();
		}
		return null;
	}

	public VATLevel getVatLevelByRemoteGuid(String remoteGuid)
	{
		try
		{
			QueryBuilder<VATLevel, Long> qb = getVATLevelDao().queryBuilder();
			qb.where().eq("remoteGuid", remoteGuid);
			PreparedQuery<VATLevel> pq = qb.prepare();
			return getVATLevelDao().queryForFirst(pq);
		}
		catch (SQLException e)
		{
			e.printStackTrace();
		}
		return null;
	}

	public VATFactor getVatFactorByRemoteGuid(String remoteGuid)
	{
		try
		{
			QueryBuilder<VATFactor, Long> qb = getVATFactorDao().queryBuilder();
			qb.where().eq("remoteGuid", remoteGuid);
			PreparedQuery<VATFactor> pq = qb.prepare();
			return getVATFactorDao().queryForFirst(pq);
		}
		catch (SQLException e)
		{
			e.printStackTrace();
		}
		return null;
	}

	public LinkedItem getLinkedItemByRemoteGuid(String remoteGuid)
	{
		try
		{
			QueryBuilder<LinkedItem, Long> qb = getLinkedItemDao().queryBuilder();
			qb.where().eq("remoteGuid", remoteGuid);
			PreparedQuery<LinkedItem> pq = qb.prepare();
			return getLinkedItemDao().queryForFirst(pq);
		}
		catch (SQLException e)
		{
			e.printStackTrace();
		}
		return null;
	}

	public PriceCatalog getPriceCatalogByRemoteGuid(String remoteGuid)
	{
		try
		{
			QueryBuilder<PriceCatalog, Long> qb = getPriceCatalogDao().queryBuilder();
			qb.where().eq("remoteGuid", remoteGuid);
			PreparedQuery<PriceCatalog> pq = qb.prepare();
			return getPriceCatalogDao().queryForFirst(pq);
		}
		catch (SQLException e)
		{
			e.printStackTrace();
		}
		return null;
	}

	public Offer getOfferByRemoteGuid(String remoteGuid)
	{
		try
		{
			QueryBuilder<Offer, Long> qb = getOffersDao().queryBuilder();
			qb.where().eq("remoteGuid", remoteGuid);
			PreparedQuery<Offer> pq = qb.prepare();
			return getOffersDao().queryForFirst(pq);
		}
		catch (SQLException e)
		{
			e.printStackTrace();
		}
		return null;
	}

	public Store getStoreByRemoteGuid(String remoteGuid)
	{
		try
		{
			QueryBuilder<Store, Long> qb = getStoresDao().queryBuilder();
			qb.where().eq("remoteGuid", remoteGuid);
			PreparedQuery<Store> pq = qb.prepare();
			return getStoresDao().queryForFirst(pq);
		}
		catch (SQLException e)
		{
			e.printStackTrace();
		}
		return null;
	}

	public OfferDetail getOfferDetailByRemoteGuid(String remoteGuid)
	{
		try
		{
			QueryBuilder<OfferDetail, Long> qb = getOfferDetailsDao().queryBuilder();
			qb.where().eq("remoteGuid", remoteGuid);
			PreparedQuery<OfferDetail> pq = qb.prepare();
			return getOfferDetailsDao().queryForFirst(pq);
		}
		catch (SQLException e)
		{
			e.printStackTrace();
		}
		return null;
	}

	public PriceCatalogDetail getPriceCatalogDetailByRemoteGuid(String remoteGuid)
	{
		try
		{
			QueryBuilder<PriceCatalogDetail, Long> qb = getPriceCatalogDetailDao().queryBuilder();
			qb.where().eq("remoteGuid", remoteGuid);
			PreparedQuery<PriceCatalogDetail> pq = qb.prepare();
			return getPriceCatalogDetailDao().queryForFirst(pq);
		}
		catch (SQLException e)
		{
			e.printStackTrace();
		}
		return null;
	}

	public Item getItemByBarcode(String code)
	{
		try
		{
			String paddedCode = RetailHelper.GetPaddedBarcode(code, context.appsettings);
			QueryBuilder<Barcode, Long> qb = getBarcodes().queryBuilder();
			qb.where().eq("Code", paddedCode);
			PreparedQuery<Barcode> pq = qb.prepare();
			Barcode bc = getBarcodes().queryForFirst(pq);
			if (bc == null || code.equals("") || bc.getItem() == null)
			{
				return null;
			}
			Item itm = getItems().queryForId(bc.getItem().getID());
			if (itm != null && itm.isActive())
			{
				return itm;
			}
			return null;
		}
		catch (SQLException e)
		{
			e.printStackTrace();
		}
		return null;
	}

	public Item getItemByItemCode(String code)
	{
		try
		{
			String itemcode = RetailHelper.GetPaddedItemCode(code, context.appsettings);
			Item itm = this.<Item> findObject(Item.class, "Code", itemcode, FilterType.EQUALS);
			if (itm != null && itm.isActive())
			{
				return itm;
			}
			return null;
		}
		catch (SQLException e)
		{
			e.printStackTrace();
		}
		return null;
	}

	public List<ItemCategory> getItemCategoryByParentID(Long parent)
	{
		try
		{
			QueryBuilder<ItemCategory, Long> qb = getItemCategories().queryBuilder().orderByRaw("length(Code), Code");
			if (parent != null)
				qb.where().eq("parent_id", parent);
			else
				qb.where().isNull("parent_id");
			PreparedQuery<ItemCategory> pq = qb.prepare();
			return getItemCategories().query(pq);
		}
		catch (SQLException e)
		{
			e.printStackTrace();
		}
		return new ArrayList<ItemCategory>();
	}

	private List<Item> getItemsOfSpecificCategory(Long ic, String filter)
	{
		try
		{
			if (ic == null)
			{
				return new ArrayList<Item>();
			}
			if (filter == null)
				filter = "";
			/*
			 * QueryBuilder<ItemAnalyticTree, Long> qb =
			 * getItemAnalyticTrees().queryBuilder();
			 * qb.where().eq("itemCategory_id", ic);
			 * 
			 * QueryBuilder<Item, Long> iqb = getItems().queryBuilder();
			 * iqb.join(qb).where().like("loweCaseName", "%" +
			 * filter.toLowerCase() + "%").// and().eq("isactive", // true).
			 * or().like("Code", "%" + filter.toLowerCase() +
			 * "%").and().eq("isactive", true);
			 * 
			 * QueryBuilder<Barcode,Long> bqb = getBarcodes().queryBuilder();
			 * 
			 * PreparedQuery<Item> ipq = iqb.prepare();
			 */
			String query = "select Item.* from Item inner join ItemAnalyticTree on Item.id = ItemAnalyticTree.item_id inner join barcode on barcode.item_id = Item.id where itemanalytictree.itemCategory_id=" + ic;
			query += " and (item.Code like '%" + filter + "%' or item.loweCaseName like '%" + filter.toLowerCase() + "%' or barcode.code like '%" + filter.toLowerCase() + "%')";

			List<Item> itemList = rawQuery(getItems(), query);
			return itemList;
		}
		catch (SQLException e)
		{
			e.printStackTrace();
		}
		return new ArrayList<Item>();
	}

	public List<Long> getSubCategoriesOfCategory(Long ic_id)
	{
		List<Long> ids = new ArrayList<Long>();
		List<ItemCategory> listSubCategories = getItemCategoryByParentID(ic_id);
		for (ItemCategory ic : listSubCategories)
		{
			ids.add(ic.getID());
			ids.addAll(getSubCategoriesOfCategory(ic.getID()));
		}
		return ids;
	}

	public List<Item> getItemsOfCategory(Long ic, String filter)
	{
		Log.i("getItemsOfCategory", "Start get ids");
		List<Long> ids = getSubCategoriesOfCategory(ic);
		Log.i("getItemsOfCategory", "End get ids");
		if (ic != null)
			ids.add(ic);
		try
		{
			if (filter == null)
				filter = "";
			/*
			 * QueryBuilder<ItemAnalyticTree, Long> qb =
			 * getItemAnalyticTrees().queryBuilder();
			 * qb.where().eq("itemCategory_id", ic);
			 * 
			 * QueryBuilder<Item, Long> iqb = getItems().queryBuilder();
			 * iqb.join(qb).where().like("loweCaseName", "%" +
			 * filter.toLowerCase() + "%").// and().eq("isactive", // true).
			 * or().like("Code", "%" + filter.toLowerCase() +
			 * "%").and().eq("isactive", true);
			 * 
			 * QueryBuilder<Barcode,Long> bqb = getBarcodes().queryBuilder();
			 * 
			 * PreparedQuery<Item> ipq = iqb.prepare();
			 */

			String ic_ids = "";
			StringBuilder sb = new StringBuilder(ids.size() * 6);
			for (Long id : ids)
			{
				if (id != null)
				{
					sb.append(((sb.length() == 0) ? "" : ",") + id.toString());

				}
			}
			ic_ids = sb.toString();
			String query = "select distinct Item.* from Item inner join ItemAnalyticTree on Item.id = ItemAnalyticTree.item_id inner join barcode on barcode.item_id = Item.id where itemanalytictree.itemCategory_id in (" + ic_ids;
			query += ") and (item.Code like '%" + filter + "%' or item.loweCaseName like '%" + filter.toLowerCase() + "%' or barcode.code like '%" + filter.toLowerCase() + "%')";
			if (ids.size() > 1)
			{
				query += " limit " + MAX_ITEMS;
			}
			Log.i("getItemsOfCategory", "Start query");
			List<Item> itemList = rawQuery(getItems(), query);
			Log.i("getItemsOfCategory", "End query");
			return itemList;
		}
		catch (SQLException e)
		{
			e.printStackTrace();
		}
		return new ArrayList<Item>();

		/*
		 * List<Item> list = getItemsOfSpecificCategory(ic, filter); if
		 * (list.size() > MAX_ITEMS) return list; List<ItemCategory>
		 * listSubCategories = getItemCategoryByParentID(ic);
		 * 
		 * for (ItemCategory itemc : listSubCategories) { List<Item> subList =
		 * getItemsOfCategory(itemc.getID(), filter); list.addAll(subList); if
		 * (list.size() > MAX_ITEMS) return list; } return list;
		 */

	}

	public List<ItemPrice> filterItemsForPriceCatalog(List<Item> inputList, PriceCatalog pc)
	{
		if (pc.getCode() == null)
		{
			try
			{
				pc = getPriceCatalogDao().queryForId(pc.getID());
			}
			catch (SQLException e)
			{
				e.printStackTrace();
			}

		}

		try
		{
			List<ItemPrice> toReturn = new ArrayList<ItemPrice>();
			for (Item itm : inputList)
			{
				double price = getItemPrice(pc, itm.getDefaultBarcodeObject(), itm);
				if (price >= 0)
				{
					ItemPrice newItmPrice = new ItemPrice();
					newItmPrice.item = itm;
					newItmPrice.price = price;
					toReturn.add(newItmPrice);
				}
			}
			return toReturn;
		}
		catch (Exception ex)
		{
			ex.printStackTrace();
			return null;
		}
	}

	public List<Item> getNewItems(long afterThisJavaMillis, long resultLimit, String filter)
	{
		try
		{
			long ticks = 621355968000000000L + afterThisJavaMillis * 10000;
			QueryBuilder<Item, Long> qb = getItems().queryBuilder();
			if (filter == null || filter.equals(""))
				qb.where().ge("InsertedOn", ticks);
			else
			{
				Where where = qb.where();
				where.and(where.ge("InsertedOn", ticks), where.or(where.like("loweCaseName", "%" + filter.toLowerCase() + "%"), where.like("Code", "%" + filter.toLowerCase() + "%")));

			}
			qb.limit(resultLimit);
			PreparedQuery<Item> pq = qb.prepare();
			return getItems().query(pq);
		}
		catch (Exception ex)
		{
			ex.printStackTrace();
		}
		return null;
	}

	/**
	 * Copies the database file at the specified location over the current
	 * internal application database.
	 * */
	public boolean importDatabase(String dbPath) throws IOException
	{

		// Close the SQLiteOpenHelper so it will commit the created empty
		// database to internal storage.
		close();
		File newDb = new File(dbPath);
		File oldDb = new File(DATABASE_PATH);
		if (newDb.exists())
		{
			copyFile(new FileInputStream(newDb), new FileOutputStream(oldDb));
			// Access the copied database so SQLiteHelper will cache it and mark
			// it as created.
			getWritableDatabase().close();
			return true;
		}
		return false;
	}

	/**
	 * Creates the specified <code>toFile</code> as a byte for byte copy of the
	 * <code>fromFile</code>. If <code>toFile</code> already exists, then it
	 * will be replaced with a copy of <code>fromFile</code>. The name and path
	 * of <code>toFile</code> will be that of <code>toFile</code>.<br/>
	 * <br/>
	 * <i> Note: <code>fromFile</code> and <code>toFile</code> will be closed by
	 * this function.</i>
	 * 
	 * @param fromFile
	 *            - FileInputStream for the file to copy from.
	 * @param toFile
	 *            - FileInputStream for the file to copy to.
	 */
	private static void copyFile(FileInputStream fromFile, FileOutputStream toFile) throws IOException
	{
		FileChannel fromChannel = null;
		FileChannel toChannel = null;
		try
		{
			fromChannel = fromFile.getChannel();
			toChannel = toFile.getChannel();
			fromChannel.transferTo(0, fromChannel.size(), toChannel);
		}
		finally
		{
			try
			{
				if (fromChannel != null)
				{
					fromChannel.close();
				}
			}
			finally
			{
				if (toChannel != null)
				{
					toChannel.close();
				}
			}
		}
	}

	private void copyFile(File src, File dst) throws IOException
	{
		FileInputStream fromFile = new FileInputStream(src);
		FileOutputStream toFile = new FileOutputStream(dst);
		copyFile(fromFile, toFile);
	}

	public class ExportDatabaseFileTask extends AsyncTask<Void, Void, Boolean>
	{
		private final ProgressDialog dialog = new ProgressDialog(context);

		// can use UI thread here
		protected void onPreExecute()
		{
			this.dialog.setMessage(context.getResources().getString(R.string.exporting));
			this.dialog.setCancelable(false);
			this.dialog.show();
		}

		// automatically done on worker thread (separate from UI thread)
		protected Boolean doInBackground(final Void... args)
		{

			File dbFile = new File(DATABASE_PATH);
			File exportDir = new File(Environment.getExternalStorageDirectory(), "");
			if (!exportDir.exists())
			{
				exportDir.mkdirs();
			}
			File file = new File(exportDir, dbFile.getName());

			try
			{
				file.createNewFile();
				copyFile(dbFile, file);
				return true;
			}
			catch (IOException e)
			{
				Log.e("mypck", e.getMessage(), e);
				return false;
			}
		}

		// can use UI thread here
		protected void onPostExecute(final Boolean success)
		{
			if (this.dialog.isShowing())
			{
				this.dialog.dismiss();
			}

			AlertDialog ad = new AlertDialog.Builder(context).create();
			ad.setCancelable(false); // This blocks the 'BACK' button
			ad.setButton("OK", new DialogInterface.OnClickListener()
			{
				public void onClick(DialogInterface dialog, int which)
				{
					dialog.dismiss();
				}
			});

			if (success)
			{
				ad.setMessage(context.getResources().getString(R.string.exportSuccessMessage));
			}
			else
			{
				ad.setMessage(context.getResources().getString(R.string.exportFailMessage));
			}
			ad.show();
			/*
			 * if (success) { Toast.makeText(context, "Export successful!",
			 * Toast.LENGTH_SHORT).show(); } else { Toast.makeText(context,
			 * "Export failed", Toast.LENGTH_SHORT).show(); }
			 */
		}

	}

	public class ImportDatabaseFileTask extends AsyncTask<String, Void, Boolean>
	{
		private final ProgressDialog dialog = new ProgressDialog(context);

		// can use UI thread here
		protected void onPreExecute()
		{
			this.dialog.setMessage(context.getResources().getString(R.string.importing));
			this.dialog.setCancelable(false);
			this.dialog.show();
		}

		// automatically done on worker thread (separate from UI thread)
		protected Boolean doInBackground(final String... args)
		{

			try
			{
				return importDatabase(args[0]);
			}
			catch (IOException e)
			{

				e.printStackTrace();
				return false;
			}
		}

		// can use UI thread here
		protected void onPostExecute(final Boolean success)
		{
			if (this.dialog.isShowing())
			{
				this.dialog.dismiss();
			}

			AlertDialog ad = new AlertDialog.Builder(context).create();
			ad.setCancelable(false); // This blocks the 'BACK' button

			if (success)
			{
				ad.setMessage(context.getResources().getString(R.string.importSuccessMessage));
				ad.setButton("OK", new DialogInterface.OnClickListener()
				{
					public void onClick(DialogInterface dialog, int which)
					{
						dialog.dismiss();
						System.exit(0);
					}
				});
			}
			else
			{
				ad.setMessage(context.getResources().getString(R.string.importFailMessage));
				ad.setButton("OK", new DialogInterface.OnClickListener()
				{
					public void onClick(DialogInterface dialog, int which)
					{
						dialog.dismiss();
					}
				});
			}

			ad.show();
		}

	}

	public synchronized Cursor getItemsCursor() throws SQLException
	{
		CloseableIterator<Item> iterator = getItems().iterator();
		// get the raw results which can be cast under Android
		AndroidDatabaseResults results = (AndroidDatabaseResults) iterator.getRawResults();
		Cursor cursor = results.getRawCursor();
		return cursor;
	}

	public MeasurementUnit getMeasurementUnitByRemoteGuid(String remoteGuid)
	{
		try
		{
			QueryBuilder<MeasurementUnit, Long> qb = getMeasurementUnitDao().queryBuilder();
			qb.where().eq("remoteGuid", remoteGuid);
			PreparedQuery<MeasurementUnit> pq = qb.prepare();
			return getMeasurementUnitDao().queryForFirst(pq);
		}
		catch (SQLException e)
		{
			e.printStackTrace();
		}
		return null;
	}

	public Address getAddressByRemoteGuid(String remoteGuid)
	{
		try
		{
			QueryBuilder<Address, Long> qb = getAddresstDao().queryBuilder();
			qb.where().eq("remoteGuid", remoteGuid);
			PreparedQuery<Address> pq = qb.prepare();
			return getAddresstDao().queryForFirst(pq);
		}
		catch (SQLException e)
		{
			e.printStackTrace();
		}
		return null;
	}

	public Address getAddressByRemoteGuid(String remoteGuid, String customerRemoteGuid)
	{
		try
		{
			QueryBuilder<Address, Long> qb = getAddresstDao().queryBuilder();
			qb.where().eq("remoteGuid", remoteGuid).and().eq("customerRemoteGuid", customerRemoteGuid);
			PreparedQuery<Address> pq = qb.prepare();
			return getAddresstDao().queryForFirst(pq);
		}
		catch (SQLException e)
		{
			e.printStackTrace();
		}
		return null;
	}

	// public List<Store> getStoresOfUsers(User user)
	// {
	// List<Store> toReturn = new ArrayList<Store>();
	// List<UserStoreAccess> usas = getStoresAccessOfUsers(user);
	// if (usas != null)
	// {
	// try
	// {
	// for (UserStoreAccess usa : usas)
	// {
	// Store s = usa.getStore();
	// if (s.getName() == null)
	// s = getStoresDao().queryForId(s.getID());
	// toReturn.add(s);
	// }
	// }
	// catch (Exception ex)
	// {
	// ex.printStackTrace();
	// }
	// }
	// return toReturn;
	// }

	public List<UserStoreAccess> getStoresAccessOfUsers(User user)
	{
		try
		{
			QueryBuilder<UserStoreAccess, Long> qb = getUserStoreAccessDao().queryBuilder();
			qb.where().eq("user_id", user.getID());
			PreparedQuery<UserStoreAccess> pq = qb.prepare();
			return getUserStoreAccessDao().query(pq);
		}
		catch (Exception ex)
		{
			ex.printStackTrace();
			return new ArrayList<UserStoreAccess>();
		}
	}
}
