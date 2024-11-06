using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Common.Attributes
{
    /// <summary>
    /// Use this attribute to bind Lists on View Models
    /// Prefix : Declares the prefix of the attribute as it is represented in the web form
    /// Also provide an increasing number to seperate items for the list
    /// as also the name of the property to assign the value e.g.
    /// Prefix_{number}_{property_of_the_object} //_Suffix.
    /// If the object is a primitive type skip property part e.g.
    /// Prefix_{number} //_Suffix.
    /// Take Note Suffix is declared using BindingListAttributeSuffix on
    /// view model's fields
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class BindingListAttribute : Attribute
    {
        public string Prefix { get; set; }      
    }
}
