using System;
using System.Collections.Generic;

namespace VRLabs.SimpleShaderInspectors
{
    /// <summary>
    /// Class used to identify a dictionary that also contains the date of last edit.
    /// </summary>
    /// <typeparam name="TKey">Type of the key</typeparam>
    /// <typeparam name="TValue">Type of the value</typeparam>
    public class TimedDictionary<TKey, TValue>
    {
        private Dictionary<TKey, (DateTime, TValue)> _dictionary;

        /// <summary>
        /// Keys stored in the dictionary
        /// </summary>
        public  Dictionary<TKey, (DateTime, TValue)>.KeyCollection Keys => _dictionary.Keys;

        /// <summary>
        /// Default constructor
        /// </summary>
        public TimedDictionary()
        {
            _dictionary = new Dictionary<TKey, (DateTime, TValue)>();
        }

        /// <summary>
        /// Try to get the value with the specified key
        /// </summary>
        /// <param name="key">Key to get the value from</param>
        /// <param name="value">return value</param>
        /// <returns>true if the value was in the dictionary, false otherwise</returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            if(_dictionary.TryGetValue(key, out (DateTime, TValue) v))
            {
                value = v.Item2;
                return true;
            }
            value = default(TValue);
            return false;
        }

        /// <summary>
        /// Set value with the specified key
        /// </summary>
        /// <param name="key">Key to use</param>
        /// <param name="value">value to insert</param>
        public void SetValue(TKey key, TValue value)
        {
            _dictionary[key] = (DateTime.Now, value);
        }

        /// <summary>
        /// Set value and inserted date with the specified key. should be used only when loading the dictionary from storage.
        /// </summary>
        /// <param name="key">Key to use</param>
        /// <param name="value">value to insert</param>
        /// <param name="date">date to insert</param>
        public void SetValue(TKey key, TValue value, DateTime date)
        {
            _dictionary[key] = (date, value);
        }

        /// <summary>
        /// Clears values older than 30 days from the dictionary.
        /// </summary>
        public void ClearOld()
        {
            var keys = _dictionary.Keys;
            var oldestDate = DateTime.Now.AddDays(-30);
            foreach(var key in keys)
            {
                if(_dictionary[key].Item1 < oldestDate)
                    _dictionary.Remove(key);
            }
        }

        /// <summary>
        /// Gets a list of all values stored in the dictionary in an easier to serialize form.
        /// </summary>
        /// <returns>A list with the data</returns>
        public List<(TKey, TValue, DateTime)> GetSerializedDictionary()
        {
            var serializedDictionary = new List<(TKey, TValue, DateTime)>();
            var keys = _dictionary.Keys;
            foreach(var key in keys)
                serializedDictionary.Add((key, _dictionary[key].Item2, _dictionary[key].Item1));
            return serializedDictionary;
        }
    }
}