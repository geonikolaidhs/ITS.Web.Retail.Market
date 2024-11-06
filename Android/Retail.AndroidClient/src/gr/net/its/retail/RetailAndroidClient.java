package gr.net.its.retail;

import org.acra.ACRA;
import org.acra.annotation.ReportsCrashes;

import android.app.Application;

@ReportsCrashes(formKey = "", formUri = "http://www.its.net.gr/retailandroidcrashes/report.php")
public class RetailAndroidClient extends Application
{
	 @Override
     public void onCreate() {
         super.onCreate();

         // The following line triggers the initialization of ACRA
         ACRA.init(this);
     }
}
