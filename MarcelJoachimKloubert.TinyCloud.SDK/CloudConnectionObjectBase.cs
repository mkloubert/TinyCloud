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
    /// A basic child for a <see cref="CloudConnection" /> object.
    /// </summary>
    public abstract class CloudConnectionObjectBase : CloudObjectBase
    {
        #region Constructors (2)

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudConnectionObjectBase" />.
        /// </summary>
        /// <param name="conn">The value for the <see cref="CloudConnectionObjectBase.Connection" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="conn" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="NullReferenceException">
        /// <paramref name="conn" /> is <see langword="null" />.
        /// </exception>
        protected CloudConnectionObjectBase(CloudConnection conn)
            : base(sync: conn.SyncRoot, isSynchronized: true)
        {
            if (conn == null)
            {
                throw new ArgumentNullException("conn");
            }

            this.Connection = conn;
        }

        #endregion Constructors (2)

        #region Properties (1)

        /// <summary>
        /// Gets the underlying connection.
        /// </summary>
        public CloudConnection Connection
        {
            get;
            private set;
        }

        #endregion Properties (1)

        #region Methods (1)

        /// <summary>
        /// Throws an exception if that object has been disposed.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Object has been disposed.</exception>
        protected void ThrowIfDisposed()
        {
            if (this.Connection.IsDisposed)
            {
                throw new ObjectDisposedException(objectName: this.GetType().FullName,
                                                  message: string.Format("Instance {0} has already been disposed!",
                                                                         this.GetHashCode()));
            }
        }

        #endregion Methods (1)
    }
}