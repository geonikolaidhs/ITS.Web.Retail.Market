using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ITS.Retail.Common.ViewModel
{
    public abstract class BasicViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;


        protected void Notify(string propertyName)
        {            
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected void Notify<T>(string propertyName, T oldValue, T newValue)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs<T>(propertyName, oldValue, newValue));
            }
        }

        protected void SetPropertyValue<T>(string propertyName, ref T privateField, T newValue )
        {
            T oldValue = privateField;
            privateField = newValue;
            Notify(propertyName, oldValue, newValue);
            AfterPropertyChange(propertyName);

        }
        
        protected virtual void AfterPropertyChange(string propertyName)
        {

        }

        protected virtual void Copy(BasicViewModel vm)
        {
            Type thisType = this.GetType();
            foreach(PropertyInfo pInfo in vm.GetType().GetProperties().Where(x=>x.CanWrite) )
            {
                PropertyInfo thisPropInfo = thisType.GetProperty(pInfo.Name);
                thisPropInfo.SetValue(this, pInfo.GetValue(vm, null), null);
            }
        }

        
    }

    public class PropertyChangedEventArgs<T> : PropertyChangedEventArgs
    {
        public PropertyChangedEventArgs(string propertyName, T oldValue, T newValue)
            : base(propertyName)
        {
            this.OldValue = oldValue;
            this.NewValue = newValue;
        }

        public T NewValue { get; private set; }

        public T OldValue { get; private set; }

    } 
}
