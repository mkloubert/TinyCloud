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

namespace MarcelJoachimKloubert.TinyCloud.SDK
{
    /// <summary>
    /// The mother of all objects.
    /// </summary>
    public abstract class CloudObjectBase : MarshalByRefObject, ICloudObject
    {
        #region Fields (2)

        /// <summary>
        /// Stores the value that defines if that object should work thread safe or not.
        /// </summary>
        protected readonly bool _IS_SYNCHRONIZED;

        /// <summary>
        /// Stores the object for the thread safe operations.
        /// </summary>
        protected readonly object _SYNC;

        #endregion Fields (2)

        #region Constructors (4)

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudObjectBase" /> class.
        /// </summary>
        /// <remarks>The new instance will not become thread safe.</remarks>
        protected CloudObjectBase()
            : this(isSynchronized: false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudObjectBase" /> class.
        /// </summary>
        /// <param name="isSynchronized">Defines if that instance should be thread safe or not.</param>
        protected CloudObjectBase(bool isSynchronized)
            : this(isSynchronized: isSynchronized,
                   sync: new object())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudObjectBase" /> class.
        /// </summary>
        /// <param name="sync">The object for the thread safe operations.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sync" /> is <see langword="null" />.
        /// </exception>
        /// <remarks>The new instance will not become thread safe.</remarks>
        protected CloudObjectBase(object sync)
            : this(isSynchronized: false,
                   sync: sync)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudObjectBase" /> class.
        /// </summary>
        /// <param name="isSynchronized">Defines if that instance should be thread safe or not.</param>
        /// <param name="sync">The object for the thread safe operations.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sync" /> is <see langword="null" />.
        /// </exception>
        protected CloudObjectBase(bool isSynchronized, object sync)
        {
            if (sync == null)
            {
                throw new ArgumentNullException("sync");
            }

            this._IS_SYNCHRONIZED = isSynchronized;
            this._SYNC = sync;
        }

        #endregion Constructors (4)

        #region Properties (2)

        /// <inheriteddoc />
        public bool IsSynchronized
        {
            get { return this._IS_SYNCHRONIZED; }
        }

        /// <inheriteddoc />
        public object SyncRoot
        {
            get { return this._SYNC; }
        }

        #endregion Properties (2)

        #region Methods (1)

        /// <summary>
        /// Returns a value without throwing an exception.
        /// </summary>
        /// <typeparam name="T">Result type.</typeparam>
        /// <param name="provider">The function that provides the return value.</param>
        /// <param name="fallbackValue">The fallback value if execution failed.</param>
        /// <returns>The return value.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" /> is <see langword="null" />.
        /// </exception>
        protected static T GetValueSafe<T>(Func<T> provider, T fallbackValue = default(T))
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            try
            {
                return provider();
            }
            catch
            {
                return fallbackValue;
            }
        }

        #endregion Methods (1)
    }
}