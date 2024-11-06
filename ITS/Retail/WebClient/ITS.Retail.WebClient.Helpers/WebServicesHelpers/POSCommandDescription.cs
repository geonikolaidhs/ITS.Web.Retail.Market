using ITS.Retail.Platform.Enumerations;
using System;

namespace POSCommandsLibrary
{
    public class POSCommandDescription
    {

        public POSCommandDescription()
        {
            this.POSCommandSet = new POSCommandSet();
        }

        private Guid _POSOid;
        private POSCommandSet _POSCommandSet;
        public Guid POSOid
        {
            get
            {
                return _POSOid;
            }
            set
            {
                _POSOid = value;
            }
        }

        public POSCommandSet POSCommandSet
        {
            get
            {
                return _POSCommandSet;
            }
            set 
            {
                _POSCommandSet = value;
            }
        }

        public void AddPOSCommand(ePosCommand posCommand, string parameters)
        {
            SerializableTuple<ePosCommand, string> tuple = new SerializableTuple<ePosCommand, string>();
            tuple.Item1 = posCommand;
            tuple.Item2 = parameters;
            this.POSCommandSet.Commands.Add(tuple);
            this._POSCommandSet.Expire = DateTime.Now.AddSeconds(60).Ticks;
        }
    }
}
