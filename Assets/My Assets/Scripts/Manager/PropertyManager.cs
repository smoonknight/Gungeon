using System;
using System.Collections.Generic;
using UnityEngine;

public class PropertyManager : Singleton<PropertyManager>
{
    public Property[] propertys;

    public GameObject GetPorperty(PropertyType type)
    {
        Property property = Array.Find(propertys, prop => prop.propertyType == type);
        return property.propertyPrefab;
    }


    [System.Serializable]
    public struct Property
    {
        public PropertyType propertyType;
        public GameObject propertyPrefab;
    }

    public enum PropertyType
    {
        rock,
    }
}