﻿//  TinyCloud SDK (https://github.com/mkloubert/TinyCloud)
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
    /// A basic file.
    /// </summary>
    public abstract class FileBase : FileSystemObjectBase, IFile
    {
        #region Constructors (4)

        /// <inheriteddoc />
        protected FileBase(IFileSystem system)
            : base(system: system)
        {
        }

        /// <inheriteddoc />
        protected FileBase(IFileSystem system, bool isSynchronized)
            : base(system: system,
                   isSynchronized: isSynchronized)
        {
        }

        /// <inheriteddoc />
        protected FileBase(IFileSystem system, object sync)
            : base(system: system,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        protected FileBase(IFileSystem system, bool isSynchronized, object sync)
            : base(system: system,
                   isSynchronized: isSynchronized, sync: sync)
        {
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
    }
}