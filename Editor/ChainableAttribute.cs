using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VRLabs.SimpleShaderInspectors
{
    /// <summary>
    /// Attribute for properties that is used when generating chainables.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = false)]
    public class ChainableAttribute : System.Attribute
    {
        public ChainableAttribute() {}
    }

    /// <summary>
    /// Attribute for constructors indicating that the chainable constructor can only be used from a specific type instead of the default IControlContainer type.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Constructor, AllowMultiple = false)]
    public class LimitAccessScopeAttribute : System.Attribute
    {
        public Type BaseType { get; }
        public LimitAccessScopeAttribute(Type type)
        {
            BaseType = type;
        }
    }
}