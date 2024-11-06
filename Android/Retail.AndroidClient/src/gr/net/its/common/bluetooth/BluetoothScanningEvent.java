package gr.net.its.common.bluetooth;

import java.util.EventObject;

public class BluetoothScanningEvent extends EventObject {

	private String value;

	public BluetoothScanningEvent(Object source, String value) {
		super(source);
		this.value = value;
	}

	public String getValue() {
		return value;
	}

	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;

}
