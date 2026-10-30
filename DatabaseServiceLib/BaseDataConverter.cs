/** 
 * Copyright (c) 2026 Igor Shkabar. All Rights reserved.
 * The file from the Class Library (.NET Standard) project.
 * I create a universal abstract base class (or interface) for data mapping. 
 * The library itself would provide an "engine" for converting `DataTable` 
 * rows into arbitrary objects, while you could implement the specific logic 
 * for populating fields within your own projects (for example, in `Buying.GIS.UI`).
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace DatabaseServiceLib
{
    public abstract class BaseDataConverter<T> where T : new()
    {
        /// <summary>
        /// An abstract method for mapping a single DataRow to an object of type T. 
        /// It is implemented on the client side (within the UI project).
        /// </summary>
        
        public abstract T MapRow(DataRow row);

        /// <summary>
        /// A universal method for converting an entire table into a list of objects.
        /// </summary>
        
        public virtual List<T> ConvertAll(DataTable dataTable)
        {
            var list = new List<T>();
            if (dataTable == null || dataTable.Rows.Count == 0)
                return list;

            foreach (DataRow row in dataTable.Rows)
            {
                list.Add(MapRow(row));
            }
            return list;
        }
    }
}
