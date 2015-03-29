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

namespace MarcelJoachimKloubert.TinyCloud.SDK.IO
{
    /// <summary>
    /// A basic file system.
    /// </summary>
    public abstract class FileSystemBase : CloudObjectBase, IFileSystem
    {
        #region Constructors (4)

        /// <inheriteddoc />
        protected FileSystemBase()
            : this(isSynchronized: true)
        {
        }

        /// <inheriteddoc />
        protected FileSystemBase(bool isSynchronized)
            : base(isSynchronized: isSynchronized)
        {
        }

        /// <inheriteddoc />
        protected FileSystemBase(object sync)
            : this(sync: sync,
                   isSynchronized: true)
        {
        }

        /// <inheriteddoc />
        protected FileSystemBase(bool isSynchronized, object sync)
            : base(isSynchronized: isSynchronized,
                   sync: sync)
        {
        }

        #endregion Constructors (4)

        #region Properties (1)

        /// <inheriteddoc />
        public abstract IDirectory Root
        {
            get;
        }

        #endregion Properties (1)

        #region Methods (2)

        /// <inheriteddoc />
        public abstract IDirectory GetDirectory(string path);

        /// <inheriteddoc />
        public abstract IFile GetFile(string path);

        #endregion Methods (2)
    }
}