using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace VRLabs.SimpleShaderInspectors
{
    /// <summary>
    /// Class used to save the dictionaries used by the API
    /// </summary>
    /// <remarks>
    /// The API provides dictionaries to store data by identifier up to 30 days, those dictionaries are automatically saved into this asset.
    /// </remarks>
    public class SerializedDictionaries : ScriptableObject
    {
        [Serializable]
        public struct BoolItem
        {
            public string key;
            public bool value;
            public long date;
        }

        [Serializable]
        public struct IntItem
        {
            public string key;
            public int value;
            public long date;
        }

        /// <summary>
        /// list for the bool dictionary
        /// </summary>
        public List<BoolItem> boolDictionary;
        public List<IntItem> intDictionary;

        public SerializedDictionaries()
        {
            boolDictionary = new List<BoolItem>();
            intDictionary = new List<IntItem>();
        }

        public void SetBoolDictionary(List<(string, bool, DateTime)> list)
        {
            boolDictionary.Clear();
            foreach(var item in list)
                boolDictionary.Add(new BoolItem{key=item.Item1, value=item.Item2, date=item.Item3.ToBinary()});
        }

        public void SetIntDictionary(List<(string, int, DateTime)> list)
        {
            intDictionary.Clear();
            foreach(var item in list)
                intDictionary.Add(new IntItem{key=item.Item1, value=item.Item2, date=item.Item3.ToBinary()});
        }
    }
}