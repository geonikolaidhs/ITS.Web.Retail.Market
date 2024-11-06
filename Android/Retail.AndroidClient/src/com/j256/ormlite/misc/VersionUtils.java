package com.j256.ormlite.misc;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;

import com.j256.ormlite.logger.Logger;
import com.j256.ormlite.logger.LoggerFactory;

/**
 * A class which helps us verify that we are running symmetric versions.
 * 
 * @author graywatson
 */
public class VersionUtils {

	private static final String CORE_VERSION_FILE = "/com/j256/ormlite/core/VERSION.txt";
	private static final String JDBC_VERSION_FILE = "/com/j256/ormlite/jdbc/VERSION.txt";
	private static final String ANDROID_VERSION_FILE = "/com/j256/ormlite/android/VERSION.txt";

	private static Logger logger;
	private static String coreVersionFile = CORE_VERSION_FILE;
	private static String jdbcVersionFile = JDBC_VERSION_FILE;
	private static String androidVersionFile = ANDROID_VERSION_FILE;
	private static boolean thrownOnErrors = false;

	private VersionUtils() {
		// only for static methods
	}

	/**
	 * Verifies that the ormlite-core and -jdbc version files hold the same string.
	 */
	public static final void checkCoreVersusJdbcVersions() {
		String core = readCoreVersion();
		String jdbc = readJdbcVersion();
		logVersionErrors("core", core, "jdbc", jdbc);
	}

	/**
	 * Verifies that the ormlite-core and -android version files hold the same string.
	 */
	public static final void checkCoreVersusAndroidVersions() {
		String core = readCoreVersion();
		String android = readAndroidVersion();
		logVersionErrors("core", core, "android", android);
	}

	/**
	 * For testing purposes.
	 */
	static void setCoreVersionFile(String coreVersionFile) {
		VersionUtils.coreVersionFile = coreVersionFile;
	}

	/**
	 * For testing purposes.
	 */
	static void setJdbcVersionFile(String jdbcVersionFile) {
		VersionUtils.jdbcVersionFile = jdbcVersionFile;
	}

	/**
	 * For testing purposes.
	 */
	static void setAndroidVersionFile(String androidVersionFile) {
		VersionUtils.androidVersionFile = androidVersionFile;
	}

	/**
	 * For testing purposes.
	 */
	static void setThrownOnErrors(boolean thrownOnErrors) {
		VersionUtils.thrownOnErrors = thrownOnErrors;
	}

	/**
	 * Log error information
	 */
	private static void logVersionErrors(String label1, String version1, String label2, String version2) {
		if (version1 == null) {
			if (version2 != null) {
				error(null, "Unknown version for {}, version for {} is '{}'", label1, label2, version2);
			}
		} else {
			if (version2 == null) {
				error(null, "Unknown version for {}, version for {} is '{}'", label2, label1, version1);
			} else if (!version1.equals(version2)) {
				error(null, "Mismatched versions: {} is '{}', while {} is '{}'", new Object[] { label1, version1,
						label2, version2 });
			}
		}
	}

	/**
	 * Read and return the version for the core package.
	 */
	private static String readCoreVersion() {
		return getVersionFromFile(coreVersionFile);
	}

	/**
	 * Read and return the version for the core package.
	 */
	private static String readJdbcVersion() {
		return getVersionFromFile(jdbcVersionFile);
	}

	/**
	 * Read and return the version for the core package.
	 */
	private static String readAndroidVersion() {
		return getVersionFromFile(androidVersionFile);
	}

	private static String getVersionFromFile(String file) {
		InputStream inputStream = VersionUtils.class.getResourceAsStream(file);
		if (inputStream == null) {
			error(null, "Could not find version file {}", file, null, null);
			return null;
		}
		BufferedReader reader = new BufferedReader(new InputStreamReader(inputStream));
		String version;
		try {
			version = reader.readLine();
		} catch (IOException e) {
			// exception ignored
			error(e, "Could not read version from {}", file, null, null);
			return null;
		} finally {
			try {
				reader.close();
			} catch (IOException e) {
				// ignored
			}
		}
		if (version == null) {
			error(null, "No version specified in {}", file, null, null);
		}
		return version;
	}

	private static void error(Throwable th, String msg, Object arg0, Object arg1, Object arg2) {
		getLogger().error(th, msg, arg0, arg1, arg2);
		if (VersionUtils.thrownOnErrors) {
			throw new IllegalStateException("See error log for details: " + msg);
		}
	}

	private static void error(Throwable th, String msg, Object[] args) {
		getLogger().error(th, msg, args);
		if (VersionUtils.thrownOnErrors) {
			throw new IllegalStateException("See error log for details:" + msg);
		}
	}

	/**
	 * Get the logger for the class. We do this so we don't have to create it all of the time.
	 */
	private static Logger getLogger() {
		if (logger == null) {
			logger = LoggerFactory.getLogger(VersionUtils.class);
		}
		return logger;
	}
}
