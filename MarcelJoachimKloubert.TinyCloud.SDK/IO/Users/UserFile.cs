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

using MarcelJoachimKloubert.TinyCloud.SDK.Helpers;
using MarcelJoachimKloubert.TinyCloud.SDK.Security;
using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace MarcelJoachimKloubert.TinyCloud.SDK.IO.Users
{
    /// <summary>
    /// The file of a user.
    /// </summary>
    public sealed class UserFile : FileBase
    {
        #region Fields (2)

        private readonly IDirectory _DIRECTORY;
        private readonly XElement _XML;

        #endregion Fields (2)

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
            this._XML = xml;

            this.LocalFile = file;
        }

        #endregion Constructors (1)

        #region Properties (8)

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
            get
            {
                string result = null;

                if (this._XML != null)
                {
                    var nameAttrib = this._XML.Attribute("name");
                    if (nameAttrib != null)
                    {
                        result = nameAttrib.Value;
                    }
                }

                if (string.IsNullOrWhiteSpace(result))
                {
                    result = this.LocalFile.Name;
                }

                return result;
            }
        }

        /// <inheriteddoc />
        public override long? Size
        {
            get
            {
                if (this._XML != null)
                {
                    string size = null;

                    var sizeElement = this._XML.Element("size");
                    if (sizeElement != null)
                    {
                        size = sizeElement.Value;
                    }

                    if (string.IsNullOrWhiteSpace(size) == false)
                    {
                        return GetValueSafe(() => (long?)long.Parse(size.Trim(),
                                                                    AppServices.DataCulture));
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the underlying user.
        /// </summary>
        public ICloudPrincipal User
        {
            get { return this.FileSystem.User; }
        }

        /// <summary>
        /// Gets the underlying XML data.
        /// </summary>
        public XElement Xml
        {
            get { return this._XML; }
        }

        #endregion Properties (8)

        #region Methods (4)

        private CryptoStream CreateCryptoStream(Stream baseStream, CryptoStreamMode mode)
        {
            byte[] pwd;
            byte[] salt;
            int iterations;
            this.GetCrypterData(out pwd, out salt, out iterations);

            return CryptoHelper.CreateCryptoStream(baseStream, CryptoStreamMode.Read,
                                                   pwd, salt, iterations);
        }

        private void GetCrypterData(out byte[] pwd, out byte[] salt, out int iterations)
        {
            pwd = null;
            salt = null;
            iterations = int.MinValue;

            if (this.Xml == null)
            {
                return;
            }

            var passwordElement = this.Xml.Element("password");
            if (passwordElement != null &&
                string.IsNullOrWhiteSpace(passwordElement.Value) == false)
            {
                pwd = Convert.FromBase64String(passwordElement.Value.Trim());
            }

            var saltElement = this.Xml.Element("salt");
            if (saltElement != null &&
                string.IsNullOrWhiteSpace(saltElement.Value) == false)
            {
                salt = Convert.FromBase64String(saltElement.Value.Trim());
            }

            var iterationsElement = this.Xml.Element("iterations");
            if (iterationsElement != null &&
                string.IsNullOrWhiteSpace(iterationsElement.Value) == false)
            {
                iterations = Convert.ToInt32(iterationsElement.Value.Trim(),
                                             AppServices.DataCulture);
            }
        }

        /// <inheriteddoc />
        protected override void OnDelete()
        {
            this.LocalFile.Refresh();
            if (this.LocalFile.Exists)
            {
                this.LocalFile.Delete();
                this.LocalFile.Refresh();
            }

            if (this.Xml != null)
            {
                var xmlDoc = this.Xml.Document;
                this.Xml.Remove();

                var dir = this.Directory as UserDirectory;
                if (dir != null &&
                    xmlDoc != null)
                {
                    dir.UpdateMetaXml(xmlDoc.Root);
                }
            }
        }

        /// <inheriteddoc />
        protected override Stream OnDownload()
        {
            var fileStream = this.LocalFile.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
            try
            {
                var cs = this.CreateCryptoStream(fileStream, CryptoStreamMode.Read);
                try
                {
                    return new GZipStream(cs, CompressionMode.Decompress, false);
                }
                catch
                {
                    cs.Dispose();

                    throw;
                }
            }
            catch
            {
                fileStream.Dispose();

                throw;
            }
        }

        #endregion Methods (4)
    }
}