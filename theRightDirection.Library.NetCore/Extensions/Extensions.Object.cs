﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using theRightDirection.Attributes;
using theRightDirection.Comparers;

namespace theRightDirection
{
    public static partial class Extensions
    {
        /// <summary>
        /// Return a deep clone of an object of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T DeepClone<T>(this T obj)
        {
            using (var memory_stream = new MemoryStream())
            {
                // Serialize the object into the memory stream.
                var formatter = new BinaryFormatter();
                formatter.Serialize(memory_stream, obj);

                // Rewind the stream and use it to create a new object.
                memory_stream.Position = 0;
                return (T)formatter.Deserialize(memory_stream);
            }
        }

        public static List<string> ListProperties(this object from, bool includePropertiesFromBaseType = false)
        {
            Dictionary<string, PropertyInfo> sourceProperties = GetProperties(from.GetType(), includePropertiesFromBaseType);
            return sourceProperties.Keys.ToList();
        }

        public static void CopyProperties(this object from, object to, bool includePropertiesFromBaseType = false, string[] excludedProperties = null)
        {
            if (to == null)
            {
                return;
            }
            var sourceProperties = GetProperties(from.GetType(), includePropertiesFromBaseType);
            var targetProperties = GetProperties(to.GetType(), includePropertiesFromBaseType);
            var commonPropertiesName = sourceProperties.Values.Intersect(targetProperties.Values, new PropertyInfoComparer()).Select(x => x.Name);
            foreach (var commonPropertyName in commonPropertiesName)
            {
                if (excludedProperties != null
                  && excludedProperties.Contains(commonPropertyName))
                    continue;
                var hasExcludeAttribute = FindAttribute(targetProperties[commonPropertyName]);
                if (!hasExcludeAttribute)
                {
                    var value = sourceProperties[commonPropertyName].GetValue(from, null);
                    if (targetProperties[commonPropertyName].CanWrite)
                    {
                        targetProperties[commonPropertyName].SetValue(to, value, null);
                    }
                }
            }
        }

        public static bool Compare<T>(this T object1, T object2)
        {
            //Get the type of the object
            var type = typeof(T);

            //return false if any of the object is false
            if (object1 == null || object2 == null)
                return false;

            //Loop through each properties inside class and get values for the property from both the objects and compare
            foreach (PropertyInfo property in type.GetProperties())
            {
                if (property.Name != "ExtensionData")
                {
                    string object1Value = string.Empty;
                    string object2Value = string.Empty;
                    if (type.GetProperty(property.Name).GetValue(object1, null) != null)
                        object1Value = type.GetProperty(property.Name).GetValue(object1, null).ToString();
                    if (type.GetProperty(property.Name).GetValue(object2, null) != null)
                        object2Value = type.GetProperty(property.Name).GetValue(object2, null).ToString();
                    if (object1Value.Trim() != object2Value.Trim())
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static int GetHashCodeOnProperties(this object inspect)
        {
            return inspect.GetType().GetTypeInfo().DeclaredProperties.Select(o => o.GetValue(inspect)).GetListHashCode();
        }

        private static bool FindAttribute(PropertyInfo commonProperty)
        {
            var attribute = commonProperty.GetCustomAttribute<ExcludeFromCopyPropertyAttribute>();
            return attribute != null;
        }

        private static Dictionary<string, PropertyInfo> GetProperties(Type type, bool recursive)
        {
            if (recursive)
            {
                Dictionary<string, PropertyInfo> properties = new Dictionary<string, PropertyInfo>();
                var retrievedProperties = GetProperties(type);
                foreach (var retrievedProperty in retrievedProperties)
                {
                    properties.Add(retrievedProperty.Key, retrievedProperty.Value);
                }
                var typeInfo = type.GetTypeInfo();
                while (typeInfo.BaseType != null)
                {
                    var baseType = typeInfo.BaseType;
                    retrievedProperties = GetProperties(baseType);
                    foreach (var retrievedProperty in retrievedProperties)
                    {
                        if (!properties.ContainsKey(retrievedProperty.Key))
                        {
                            properties.Add(retrievedProperty.Key, retrievedProperty.Value);
                        }
                    }
                    typeInfo = baseType.GetTypeInfo();
                }
                return properties;
            }
            return GetProperties(type);
        }

        private static Dictionary<string, PropertyInfo> GetProperties(Type type)
        {
            var typeInfo = type.GetTypeInfo();
            Dictionary<string, PropertyInfo> properties = new Dictionary<string, PropertyInfo>();
            foreach (var property in typeInfo.DeclaredProperties)
            {
                properties.Add(property.Name, property);
            }
            return properties;
        }
    }
}