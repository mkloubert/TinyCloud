//  TinyCloud SDK (https://github.com/mkloubert/TinyCloud)
//  Copyright (C) 2015  Marcel Joachim Kloubert <marcel.kloubert@gmx.net>
//
//  This library is free software; you can redistribute it and/or
//  modify it under the terms of the GNU Lesser General Public
//  License as published by the Free Software Foundation; either
//  version 3.0 of the License, or (at your option) any later version.
//
//  This library is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//  Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public
//  License along with this library.

using System;

namespace MarcelJoachimKloubert.TinyCloud.SDK.Extensions
{
    /// <summary>
    /// Extension methods for handling objects and values.
    /// </summary>
    public static class TinyCloudValueExtensionMethods
    {
        #region Methods (1)

        /// <summary>
        /// Returns an object as string.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="dbNullAsNull">Handle <see cref="DBNull" /> as <see langword="null" /> reference or not.</param>
        /// <returns>The input object as string.</returns>
        public static string AsString(this object value, bool dbNullAsNull = true)
        {
            if (value is string)
            {
                return (string)value;
            }

            if (dbNullAsNull &&
                DBNull.Value.Equals(value))
            {
                value = null;
            }

            if (value == null)
            {
                return null;
            }

            return value.ToString();
        }

        #endregion Methods (1)
    }
}