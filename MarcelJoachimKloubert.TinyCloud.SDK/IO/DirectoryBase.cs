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
using System.Collections.Generic;
using System.Linq;

namespace MarcelJoachimKloubert.TinyCloud.SDK.IO
{
    /// <summary>
    /// A basic directory.
    /// </summary>
    public abstract class DirectoryBase : FileSystemObjectBase, IDirectory
    {
        #region Fields (2)

        private readonly Func<IEnumerable<IDirectory>> _GET_DIRECTORIES_FUNC;
        private readonly Func<IEnumerable<IFile>> _GET_FILES_FUNC;

        #endregion Fields (2)

        #region Constructors (4)

        /// <inheriteddoc />
        protected DirectoryBase(IFileSystem system)
            : this(system: system,
                   isSynchronized: true)
        {
        }

        /// <inheriteddoc />
        protected DirectoryBase(IFileSystem system, bool isSynchronized)
            : this(system: system,
                   isSynchronized: isSynchronized, sync: new object())
        {
        }

        /// <inheriteddoc />
        protected DirectoryBase(IFileSystem system, object sync)
            : this(system: system,
                   sync: sync, isSynchronized: true)
        {
        }

        /// <inheriteddoc />
        protected DirectoryBase(IFileSystem system, bool isSynchronized, object sync)
            : base(system: system,
                   isSynchronized: isSynchronized, sync: sync)
        {
            if (this._IS_SYNCHRONIZED)
            {
                this._GET_DIRECTORIES_FUNC = this.OnGetDirectories_ThreadSafe;
                this._GET_FILES_FUNC = this.OnGetFiles_ThreadSafe;
            }
            else
            {
                this._GET_DIRECTORIES_FUNC = this.OnGetDirectories;
                this._GET_FILES_FUNC = this.OnGetFiles;
            }
        }

        #endregion Constructors (4)

        #region Properties (3)

        /// <inheriteddoc />
        public bool IsRoot
        {
            get { return this.Parent == null; }
        }

        /// <inheriteddoc />
        public abstract DateTimeOffset? LastWriteTime { get; }

        /// <inheriteddoc />
        public abstract IDirectory Parent { get; }

        #endregion Properties (3)

        #region Methods (7)

        /// <inheriteddoc />
        public IEnumerable<IDirectory> GetDirectories()
        {
            return (this._GET_DIRECTORIES_FUNC() ?? Enumerable.Empty<IDirectory>()).Where(x => x != null);
        }

        /// <inheriteddoc />
        public IEnumerable<IDirectory> GetDirectoryTree()
        {
            IDirectory currentDir = this;
            do
            {
                yield return currentDir;

                currentDir = currentDir.Parent;
            }
            while (currentDir != null);
        }

        /// <inheriteddoc />
        public IEnumerable<IFile> GetFiles()
        {
            return (this._GET_FILES_FUNC() ?? Enumerable.Empty<IFile>()).Where(x => x != null);
        }

        /// <summary>
        /// The logic for the <see cref="DirectoryBase.GetDirectories()" /> method.
        /// </summary>
        /// <returns>The directories.</returns>
        protected abstract IEnumerable<IDirectory> OnGetDirectories();

        private IEnumerable<IDirectory> OnGetDirectories_ThreadSafe()
        {
            IEnumerable<IDirectory> result;

            lock (this._SYNC)
            {
                result = this.OnGetDirectories();
            }

            return result;
        }

        /// <summary>
        /// The logic for the <see cref="DirectoryBase.GetFiles()" /> method.
        /// </summary>
        /// <returns>The files.</returns>
        protected abstract IEnumerable<IFile> OnGetFiles();

        private IEnumerable<IFile> OnGetFiles_ThreadSafe()
        {
            IEnumerable<IFile> result;

            lock (this._SYNC)
            {
                result = this.OnGetFiles();
            }

            return result;
        }

        #endregion Methods (7)
    }
}