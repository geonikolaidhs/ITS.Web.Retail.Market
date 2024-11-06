using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ITS.POS.Tools.FormDesigner.Loader
{
    public class TypeDiscoveryService : ITypeDiscoveryService  
    {

        private IServiceProvider host;
        public TypeDiscoveryService(IServiceProvider host)
        {
            this.host = host;
        }

        #region ITypeDiscoveryService Members

        public System.Collections.ICollection GetTypes(Type baseType, bool excludeGlobalTypes)
        {
            //return AppDomain.CurrentDomain.Get
            List<Type> toreturn = new List<Type>();
            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                var v = asm.GetTypes().Where(g => g.IsSubclassOf(baseType));
                if (v.Count() > 0)
                {
                    toreturn.AddRange(v);
                }

            }
            return toreturn;
            //throw new NotImplementedException();
        }

        #endregion
    }
}
