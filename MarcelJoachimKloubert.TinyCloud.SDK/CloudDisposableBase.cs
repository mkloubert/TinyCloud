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
    /// A basic disposable object.
    /// </summary>
    public abstract class CloudDisposableBase : CloudObjectBase, ICloudDisposable
    {
        #region Constructors (5)

        /// <inheriteddoc />
        protected CloudDisposableBase()
            : base()
        {
        }

        /// <inheriteddoc />
        protected CloudDisposableBase(bool isSynchronized)
            : base(isSynchronized: isSynchronized)
        {
        }

        /// <inheriteddoc />
        protected CloudDisposableBase(object sync)
            : base(sync: sync)
        {
        }

        /// <inheriteddoc />
        protected CloudDisposableBase(bool isSynchronized, object sync)
            : base(isSynchronized: isSynchronized,
                   sync: sync)
        {
        }

        ~CloudDisposableBase()
        {
        }

        #endregion Constructors (5)

        #region Properties (1)

        /// <inheriteddoc />
        public bool IsDisposed
        {
            get;
            private set;
        }

        #endregion Properties (1)

        #region Methods (4)

        /// <inheriteddoc />
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            lock (this._SYNC)
            {
                if (disposing && this.IsDisposed)
                {
                    return;
                }

                this.OnDispose(disposing);

                if (disposing)
                {
                    this.IsDisposed = true;
                }
            }
        }

        /// <summary>
        /// The logic for the <see cref="CloudDisposableBase.Dispose()" /> method and the finalizer.
        /// </summary>
        /// <param name="disposing">
        /// <see cref="CloudDisposableBase.Dispose()" /> method was called (<see langword="true" />)
        /// or the finalizer (<see langword="false" />).
        /// </param>
        protected abstract void OnDispose(bool disposing);

        /// <summary>
        /// Throws an exception if that object has been disposed.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Object has been disposed.</exception>
        protected void ThrowIfDisposed()
        {
            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(objectName: this.GetType().FullName,
                                                  message: string.Format("Instance {0} has already been disposed!",
                                                                         this.GetHashCode()));
            }
        }

        #endregion Methods (4)
    }
}