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
using System.IO;
using System.Linq;

namespace MarcelJoachimKloubert.TinyCloud.SDK.IO.Users
{
    /// <summary>
    /// A user directory.
    /// </summary>
    public sealed class UserDirectory : DirectoryBase
    {
        #region Fields (1)

        private readonly IDirectory _PARENT;

        #endregion Fields (1)

        #region Constructors (1)

        /// <inheriteddoc />
        public UserDirectory(UserFileSystem system, DirectoryInfo dir, IDirectory parent = null)
            : base(system: system)
        {
            if (dir == null)
            {
                throw new ArgumentNullException("dir");
            }

            this._PARENT = parent;
            this.LocalDirectory = new DirectoryInfo(Path.GetFullPath(dir.FullName));
        }

        #endregion Constructors (1)

        #region Properties (5)

        /// <inheriteddoc />
        public new UserFileSystem FileSystem
        {
            get { return (UserFileSystem)base.FileSystem; }
        }

        /// <inheriteddoc />
        public override DateTimeOffset? LastWriteTime
        {
            get { return GetValueSafe(() => (DateTimeOffset?)this.LocalDirectory.LastWriteTimeUtc); }
        }

        /// <summary>
        /// Gets the underlying local directory.
        /// </summary>
        public DirectoryInfo LocalDirectory
        {
            get;
            private set;
        }

        /// <inheriteddoc />
        public override string Name
        {
            get
            {
                return this.IsRoot == false ? this.LocalDirectory.Name
                                            : null;
            }
        }

        /// <inheriteddoc />
        public override IDirectory Parent
        {
            get { return this._PARENT; }
        }

        #endregion Properties (5)

        #region Methods (2)

        /// <inheriteddoc />
        protected override IEnumerable<IDirectory> OnGetDirectories()
        {
            return this.LocalDirectory
                       .EnumerateDirectories()
                       .Select(x =>
                       {
                           return new UserDirectory(this.FileSystem,
                                                    x, this);
                       });
        }

        /// <inheriteddoc />
        protected override IEnumerable<IFile> OnGetFiles()
        {
            return this.LocalDirectory
                       .EnumerateFiles()
                       .Select(x =>
                       {
                           return new UserFile(this.FileSystem,
                                               x, this);
                       });
        }

        #endregion Methods (2)
    }
}