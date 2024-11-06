package gr.net.its.common.bluetooth;

import java.util.EventListener;

public interface BluetoothScanningEventListener extends EventListener {

	public void onBluetoothScan(BluetoothScanningEvent event,String message);
	
}
