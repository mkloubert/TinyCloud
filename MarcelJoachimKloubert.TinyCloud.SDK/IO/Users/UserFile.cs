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

using MarcelJoachimKloubert.TinyCloud.SDK.Security;
using System;
using System.IO;
using System.Xml.Linq;

namespace MarcelJoachimKloubert.TinyCloud.SDK.IO.Users
{
    /// <summary>
    /// The file of a user.
    /// </summary>
    public sealed class UserFile : FileBase
    {
        #region Fields (1)

        private readonly IDirectory _DIRECTORY;

        #endregion Fields (1)

        #region Constructors (1)

        /// <inheriteddoc />
        public UserFile(UserFileSystem system, FileInfo file, XElement xml, IDirectory directory)
            : base(system: system)
        {
            if (file == null)
            {
                throw new ArgumentNullException("file");
            }

            this._DIRECTORY = directory;
            this.LocalFile = file;
        }

        #endregion Constructors (1)

        #region Properties (7)

        /// <inheriteddoc />
        public override IDirectory Directory
        {
            get { return this._DIRECTORY; }
        }

        /// <inheriteddoc />
        public new UserFileSystem FileSystem
        {
            get { return (UserFileSystem)base.FileSystem; }
        }

        /// <inheriteddoc />
        public override DateTimeOffset? LastWriteTime
        {
            get { return GetValueSafe(() => (DateTimeOffset?)this.LocalFile.LastWriteTimeUtc); }
        }

        /// <summary>
        /// Gets the underlying local directory.
        /// </summary>
        public FileInfo LocalFile
        {
            get;
            private set;
        }

        /// <inheriteddoc />
        public override string Name
        {
            get { return this.LocalFile.Name; }
        }

        /// <inheriteddoc />
        public override long? Size
        {
            get { return GetValueSafe(() => (long?)this.LocalFile.Length); }
        }

        /// <summary>
        /// Gets the underlying user.
        /// </summary>
        public ICloudPrincipal User
        {
            get { return this.FileSystem.User; }
        }

        #endregion Properties (7)
    }
}