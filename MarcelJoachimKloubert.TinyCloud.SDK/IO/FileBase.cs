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
using System.IO;

namespace MarcelJoachimKloubert.TinyCloud.SDK.IO
{
    /// <summary>
    /// A basic file.
    /// </summary>
    public abstract class FileBase : FileSystemObjectBase, IFile
    {
        #region Fields (2)

        private readonly Action _DELETE_ACTION;
        private readonly Func<Stream> _DOWNLOAD_FUNC;

        #endregion Fields (2)

        #region Constructors (4)

        /// <inheriteddoc />
        protected FileBase(IFileSystem system)
            : this(system: system,
                   isSynchronized: true)
        {
        }

        /// <inheriteddoc />
        protected FileBase(IFileSystem system, bool isSynchronized)
            : this(system: system,
                   isSynchronized: isSynchronized, sync: new object())
        {
        }

        /// <inheriteddoc />
        protected FileBase(IFileSystem system, object sync)
            : this(system: system,
                   isSynchronized: true, sync: sync)
        {
        }

        /// <inheriteddoc />
        protected FileBase(IFileSystem system, bool isSynchronized, object sync)
            : base(system: system,
                   isSynchronized: isSynchronized, sync: sync)
        {
            if (this._IS_SYNCHRONIZED)
            {
                this._DELETE_ACTION = this.OnDelete_ThreadSafe;
                this._DOWNLOAD_FUNC = this.OnDownload_ThreadSafe;
            }
            else
            {
                this._DELETE_ACTION = this.OnDelete;
                this._DOWNLOAD_FUNC = this.OnDownload;
            }
        }

        #endregion Constructors (4)

        #region Properties (3)

        /// <inheriteddoc />
        public abstract IDirectory Directory { get; }

        /// <inheriteddoc />
        public abstract DateTimeOffset? LastWriteTime { get; }

        /// <inheriteddoc />
        public abstract long? Size { get; }

        #endregion Properties (3)

        #region Methods (6)

        /// <inheriteddoc />
        public void Delete()
        {
            this._DELETE_ACTION();
        }

        /// <inheriteddoc />
        public Stream Download()
        {
            return this._DOWNLOAD_FUNC();
        }

        /// <summary>
        /// The logic for the <see cref="FileBase.Delete()" /> method.
        /// </summary>
        protected abstract void OnDelete();

        private void OnDelete_ThreadSafe()
        {
            lock (this._SYNC)
            {
                this.OnDelete();
            }
        }

        /// <summary>
        /// The logic for the <see cref="FileBase.Download()" /> method.
        /// </summary>
        protected abstract Stream OnDownload();

        private Stream OnDownload_ThreadSafe()
        {
            Stream result;

            lock (this._SYNC)
            {
                result = this.OnDownload();
            }

            return result;
        }

        #endregion Methods (6)
    }
}