using System;
using System.Collections.Generic;
using System.Text;

namespace ITS.POS.Hardware.PosiFiscal.Protocol
{
    public abstract class Field
    {
        protected double _MinValue;
        public virtual double MinValue
        {
            get
            {
                return _MinValue;
            }
        }

        protected double _MaxValue;
        public virtual double MaxValue
        {
            get
            {
                return _MaxValue;
            }
        }

        protected int _MinDigits;
        public virtual int MinDigits
        {
            get
            {
                return _MinDigits;
            }
        }

        protected int _MaxDigits;
        public virtual int MaxDigits
        {
            get
            {
                return _MaxDigits;   
            }
        }

        protected string _Value;
        public virtual string Value
        {
            get
            {
                return _Value;
            }
            set
            {
                _Value = value;
            }
        }

        protected FieldClass _FieldClass;

        public FieldClass FieldClass
        {
            get
            {
                return _FieldClass;
            }
        }
    }
}
