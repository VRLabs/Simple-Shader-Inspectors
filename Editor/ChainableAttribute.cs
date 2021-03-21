using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VRLabs.SimpleShaderInspectors
{
    /// <summary>
    /// Attribute for properties that is used when generating chainables.
    /// </summary>
    /// <remarks>
    /// The attribute by itself does nothing, but it is used by the chainables generator tool to generate extension methods for properties that have it
    /// </remarks>
    [System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = false)]
    public class ChainableAttribute : System.Attribute
    {
        public ChainableAttribute() {}
    }

    /// <summary>
    /// Attribute for constructors indicating that the chainable constructor can only be used from a specific type instead of the default IControlContainer type.
    /// </summary>
    /// <remarks>
    /// Sometimes you may need to limit which controls implementing IControlContainer can use the chainable version of the constructor, by having this attribute you can modify the generated 
    /// chainable constructor to reflect those needs.
    /// </remarks>
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