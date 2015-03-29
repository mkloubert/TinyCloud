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

namespace MarcelJoachimKloubert.TinyCloud.SDK.IO
{
    /// <summary>
    /// A basic file system object.
    /// </summary>
    public abstract class FileSystemObjectBase : CloudObjectBase, IFileSystemObject
    {
        #region Constructors (4)

        /// <inheriteddoc />
        protected FileSystemObjectBase(IFileSystem system)
            : this(system: system,
                   isSynchronized: true)
        {
        }

        /// <inheriteddoc />
        protected FileSystemObjectBase(IFileSystem system, bool isSynchronized)
            : this(system: system,
                   isSynchronized: isSynchronized, sync: new object())
        {
        }

        /// <inheriteddoc />
        protected FileSystemObjectBase(IFileSystem system, object sync)
            : this(system: system,
                   sync: sync, isSynchronized: true)
        {
        }

        /// <inheriteddoc />
        protected FileSystemObjectBase(IFileSystem system, bool isSynchronized, object sync)
            : base(isSynchronized: isSynchronized,
                   sync: sync)
        {
            if (system == null)
            {
                throw new ArgumentNullException("system");
            }

            this.FileSystem = system;
        }

        #endregion Constructors (4)

        #region Properties (2)

        /// <inheriteddoc />
        public IFileSystem FileSystem
        {
            get;
            private set;
        }

        /// <inheriteddoc />
        public abstract string Name
        {
            get;
        }

        #endregion Properties (2)
    }
}