﻿using System;

namespace MapsDirectlyToDatabaseTable
{
    /// <summary>
    /// Used to indicate when a property should be displayed with it's own column when visualising it in collection views
    ///  e.g. SelectIMapsDirectlyToDatabaseTableDialog
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class UsefulProperty : Attribute
    {
    }
}