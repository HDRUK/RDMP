﻿using System;

namespace MapsDirectlyToDatabaseTable
{
    /// <summary>
    /// Used to indicate a property that contains sql e.g. Where logic, Select logic etc
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class SqlAttribute : Attribute
    {
    }
}