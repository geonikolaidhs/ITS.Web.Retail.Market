package gr.net.its.common.bluetooth;

import java.io.InputStream;
import java.util.ArrayList;
import java.util.PriorityQueue;
import java.util.Queue;
import java.util.Set;
import java.util.UUID;

import android.bluetooth.BluetoothAdapter;
import android.bluetooth.BluetoothDevice;
import android.bluetooth.BluetoothSocket;

public class BluetoothConnector extends Thread
{

    public boolean readingEnabled = true, invokeEvent = true;
    // public String MY_UUID = "00001101-0000-1000-8000-00805F9B34FB";

    private BluetoothDevice device = null;
    private BTScanState status = BTScanState.NOTINITIALIZED;
    private BluetoothAdapter btAdapter;
    private BluetoothSocket socket;

    public static Set<BluetoothDevice> GetBondedDevices()
    {
	    if(BluetoothAdapter.getDefaultAdapter() == null)
	    {
		return null;
	    }
	    else
	    {
		return BluetoothAdapter.getDefaultAdapter().getBondedDevices();
	    }
	
    }

    public BluetoothConnector(String deviceName)
    {
	super();
	if (btAdapter == null)
	{
	    btAdapter = BluetoothAdapter.getDefaultAdapter();
	}

	Set<BluetoothDevice> pairedDevices = btAdapter.getBondedDevices();
	for (BluetoothDevice btDevice : pairedDevices)
	{
	    if (device == null && btDevice.getName().equalsIgnoreCase(deviceName))
		device = btDevice;
	}
	status = BTScanState.INITIALIZED;
    }

    public BTScanState getStatus()
    {
	return status;
    }

    @Override
    public void run()
    {
	while (true)
	{
	    try
	    {
		UUID ud = UUID.fromString("00001101-0000-1000-8000-00805F9B34FB");
		socket = device.createInsecureRfcommSocketToServiceRecord(ud);
		// socket = device.createRfcommSocketToServiceRecord(ud);
		btAdapter.cancelDiscovery();
		socket.connect();
		InputStream stream = socket.getInputStream();
		status = BTScanState.WAITING;
		currentValue = "";
		this.values = new PriorityQueue<String>();
		int read = 0;
		byte[] buffer = new byte[128];
		do
		{
		    try
		    {

			read = stream.read(buffer);
			String data = new String(buffer, 0, read);
			if (readingEnabled)
			    publishProgress(data);
		    }
		    catch (Exception ex)
		    {
			read = -1;
		    }
		}
		while (read > 0);
	    }
	    catch (Exception e)
	    {
		// e.printStackTrace();
		status = BTScanState.NOTCONNECTEDTODEVICE;
		try
		{
		    Thread.sleep(1000);
		}
		catch (Exception ex)
		{

		}
	    }
	}
    }

    protected void publishProgress(String... values)
    {
	currentValue += values[0];
	if (currentValue.contains("\r"))
	{
	    if (invokeEvent)
	    {
		fireEvent(currentValue);
	    }
	    else
	    {
		this.values.add(currentValue);
	    }
	    currentValue = new String("");
	}
    }

    public Queue<String> values;
    private String currentValue;

    // Events
    private ArrayList<BluetoothScanningEventListener> _listeners = new ArrayList<BluetoothScanningEventListener>();

    public synchronized void addBluetoothScanningEventListener(BluetoothScanningEventListener l)
    {
	_listeners.add(l);
    }

    public synchronized void removeBluetoothScanningEventListener(BluetoothScanningEventListener l)
    {
	_listeners.remove(l);
    }

    private synchronized void fireEvent(String value)
    {
	BluetoothScanningEvent evt = new BluetoothScanningEvent(this, value);
	for (BluetoothScanningEventListener l : _listeners)
	    l.onBluetoothScan(evt, value);
    }
}
