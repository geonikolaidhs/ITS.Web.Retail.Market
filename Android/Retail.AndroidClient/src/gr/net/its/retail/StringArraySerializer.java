package gr.net.its.retail;

import java.util.Hashtable;
import java.util.Vector;

import org.ksoap2.serialization.KvmSerializable;
import org.ksoap2.serialization.PropertyInfo;

public class StringArraySerializer extends Vector<String> implements KvmSerializable {


      public Object getProperty(int arg0) {
              return this.get(arg0);
      }

      public int getPropertyCount() {
              return this.size();
      }

      public void getPropertyInfo(int arg0, Hashtable arg1, PropertyInfo arg2) {
              arg2.name = "string";
              arg2.type = PropertyInfo.STRING_CLASS;
      }

      public void setProperty(int arg0, Object arg1) {
              this.add(arg1.toString());
      }


}
