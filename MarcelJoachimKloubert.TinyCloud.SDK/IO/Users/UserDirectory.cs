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
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;

namespace MarcelJoachimKloubert.TinyCloud.SDK.IO.Users
{
    /// <summary>
    /// A user directory.
    /// </summary>
    public sealed class UserDirectory : DirectoryBase
    {
        #region Fields (8)

        private readonly IDirectory _PARENT;
        private readonly XElement _XML;

        /// <summary>
        /// Stores the name of a 'name' XML attribute.
        /// </summary>
        public const string XML_ATTRIB_NAME = "name";

        /// <summary>
        /// Stores the name of an attributes for a "real" directory name.
        /// </summary>
        public const string XML_ATTRIB_REAL_DIRECTORY = "realDir";

        /// <summary>
        /// Stores the name of an attributes for a "real" file name.
        /// </summary>
        public const string XML_ATTRIB_REAL_FILE = "realFile";

        /// <summary>
        /// Stores the name of an element with directory information.
        /// </summary>
        public const string XML_ELEMENT_DIRECTORY = "dir";

        /// <summary>
        /// Stores the name of an elements that contains directory elements.
        /// </summary>
        public const string XML_ELEMENT_DIRECTORY_LIST = "dirs";

        /// <summary>
        /// Stores the name of an element with file information.
        /// </summary>
        public const string XML_ELEMENT_FILE = "file";

        /// <summary>
        /// Stores the name of an elements that contains file elements.
        /// </summary>
        public const string XML_ELEMENT_FILE_LIST = "files";

        #endregion Fields (8)

        #region Constructors (1)

        /// <inheriteddoc />
        public UserDirectory(UserFileSystem system, DirectoryInfo dir, XElement xml, IDirectory parent = null)
            : base(system: system)
        {
            if (dir == null)
            {
                throw new ArgumentNullException("dir");
            }

            this._PARENT = parent;
            this._XML = xml;

            this.LocalDirectory = dir;
        }

        #endregion Constructors (1)

        #region Properties (7)

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
                if (this.IsRoot)
                {
                    return null;
                }

                string result = null;

                if (this._XML != null)
                {
                    var nameAttrib = this._XML.Attribute(XML_ATTRIB_NAME);
                    if (nameAttrib != null)
                    {
                        result = nameAttrib.Value;
                    }
                }

                if (string.IsNullOrWhiteSpace(result))
                {
                    result = this.LocalDirectory.Name;
                }

                return result;
            }
        }

        /// <inheriteddoc />
        public override IDirectory Parent
        {
            get { return this._PARENT; }
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

        #endregion Properties (7)

        #region Methods (11)

        /// <inheriteddoc />
        public override string GetInvalidCharsForDirectoryNames()
        {
            return new string(Path.GetInvalidFileNameChars());
        }

        /// <inheriteddoc />
        public override string GetInvalidCharsForFileNames()
        {
            return new string(Path.GetInvalidFileNameChars());
        }

        private FileInfo GetMetaFile()
        {
            return new FileInfo(Path.Combine(this.LocalDirectory.FullName,
                                             "0.bin"));
        }

        private FileInfo GetMetaBackupFile()
        {
            return new FileInfo(Path.Combine(this.LocalDirectory.FullName,
                                             "0.bak"));
        }

        private XElement GetMetaXml()
        {
            XElement result = null;

            try
            {
                var metaFile = this.GetMetaFile();

                if (metaFile.Exists)
                {
                    using (var cryptedStream = metaFile.OpenRead())
                    {
                        using (var compressedStream = new MemoryStream())
                        {
                            this.User
                                .Decrypt(cryptedStream, compressedStream);

                            compressedStream.Position = 0;
                            using (var gzip = new GZipStream(compressedStream, CompressionMode.Decompress, true))
                            {
                                result = XDocument.Load(gzip).Root;
                            }
                        }
                    }
                }
            }
            catch
            {
                result = null;
            }

            result = result ?? new XElement(XML_ELEMENT_DIRECTORY);

            var xmlDoc = result.Document;
            if (xmlDoc == null)
            {
                xmlDoc = new XDocument(new XDeclaration("1.0", Encoding.UTF8.WebName, "yes"));
                xmlDoc.Add(result);
            }

            return result;
        }

        /// <inheriteddoc />
        protected override IDirectory OnCreateDirectory(string name)
        {
            var xml = this.GetMetaXml();

            // find next unique and "real" directory
            ulong i = 0;
            DirectoryInfo di;
            do
            {
                di = new DirectoryInfo(Path.Combine(this.LocalDirectory.FullName,
                                                    i.ToString()));

                if (di.Exists == false)
                {
                    di.Create();

                    break;
                }
            }
            while (++i > 0);

            XElement newDirElement = null;

            try
            {
                var dirsElement = xml.Element(XML_ELEMENT_DIRECTORY_LIST);
                if (dirsElement == null)
                {
                    dirsElement = new XElement(XML_ELEMENT_DIRECTORY_LIST);
                    xml.Add(dirsElement);
                }

                newDirElement = new XElement(XML_ELEMENT_DIRECTORY);
                newDirElement.SetAttributeValue(XML_ATTRIB_NAME, name);
                newDirElement.SetAttributeValue(XML_ATTRIB_REAL_DIRECTORY, di.Name);

                dirsElement.Add(newDirElement);

                this.OnUpdateMetaXml(xml);
            }
            catch
            {
                di.Delete();

                throw;
            }

            return new UserDirectory(this.FileSystem, di, newDirElement, this);
        }

        /// <inheriteddoc />
        protected override void OnDelete()
        {
            this.LocalDirectory.Refresh();
            if (this.LocalDirectory.Exists)
            {
                this.LocalDirectory.Delete();
                this.LocalDirectory.Refresh();
            }

            if (this.Xml != null)
            {
                var xmlDoc = this.Xml.Document;
                this.Xml.Remove();

                var parent = this.Parent as UserDirectory;
                if (parent != null &&
                    xmlDoc != null)
                {
                    parent.UpdateMetaXml(xmlDoc.Root);
                }
            }
        }

        /// <inheriteddoc />
        protected override IEnumerable<IDirectory> OnGetDirectories()
        {
            return this.GetMetaXml()
                       .XPathSelectElements(XML_ELEMENT_DIRECTORY_LIST + "/" + XML_ELEMENT_DIRECTORY)
                       .Select(x =>
                       {
                           IDirectory dir = null;

                           try
                           {
                               var nameAttrib = x.Attribute(XML_ATTRIB_NAME);
                               if (nameAttrib != null)
                               {
                                   var name = (nameAttrib.Value ?? string.Empty).Trim();
                                   if (name != string.Empty)
                                   {
                                       var realDirAttrib = x.Attribute(XML_ATTRIB_REAL_DIRECTORY);
                                       if (realDirAttrib != null)
                                       {
                                           var realDir = (realDirAttrib.Value ?? string.Empty).Trim();
                                           if (realDir != string.Empty)
                                           {
                                               var di = new DirectoryInfo(Path.Combine(this.LocalDirectory.FullName,
                                                                                       realDir));

                                               if (di.Exists)
                                               {
                                                   dir = new UserDirectory(this.FileSystem, di, x, this);
                                               }
                                           }
                                       }
                                   }
                               }
                           }
                           catch
                           {
                               dir = null;
                           }

                           return dir;
                       });
        }

        /// <inheriteddoc />
        protected override IEnumerable<IFile> OnGetFiles()
        {
            return this.GetMetaXml()
                       .XPathSelectElements(XML_ELEMENT_FILE_LIST + "/" + XML_ELEMENT_FILE)
                       .Select(x =>
                       {
                           IFile file = null;

                           try
                           {
                               var nameAttrib = x.Attribute(XML_ATTRIB_NAME);
                               if (nameAttrib != null)
                               {
                                   var name = (nameAttrib.Value ?? string.Empty).Trim();
                                   if (name != string.Empty)
                                   {
                                       var realFileAttrib = x.Attribute(XML_ATTRIB_REAL_FILE);
                                       if (realFileAttrib != null)
                                       {
                                           var realFile = (realFileAttrib.Value ?? string.Empty).Trim();
                                           if (realFile != string.Empty)
                                           {
                                               var fi = new FileInfo(Path.Combine(this.LocalDirectory.FullName,
                                                                                  realFile));

                                               if (fi.Exists)
                                               {
                                                   file = new UserFile(this.FileSystem, fi, x, this);
                                               }
                                           }
                                       }
                                   }
                               }
                           }
                           catch
                           {
                               file = null;
                           }

                           return file;
                       });
        }

        private void OnUpdateMetaXml(XElement xml)
        {
            if (xml == null)
            {
                return;
            }

            var xmlDoc = xml.Document;

            FileInfo metaFile = null;
            FileInfo metaBackupFileToRestore = null;

            try
            {
                metaFile = this.GetMetaFile();
                if (metaFile.Exists)
                {
                    // first create backup
                    {
                        var metaBackupFile = this.GetMetaBackupFile();
                        if (metaBackupFile.Exists)
                        {
                            metaBackupFile.Delete();
                            metaBackupFile.Refresh();
                        }

                        File.Move(metaFile.FullName, metaBackupFile.FullName);
                        metaBackupFileToRestore = metaBackupFile;
                    }

                    // now delete old file
                    metaFile.Delete();
                    metaFile.Refresh();
                }

                // save XML
                using (var cryptedStream = metaFile.Open(FileMode.CreateNew, FileAccess.ReadWrite))
                {
                    using (var compressedStream = new MemoryStream())
                    {
                        using (var gzip = new GZipStream(compressedStream, CompressionMode.Compress, true))
                        {
                            xmlDoc.Save(gzip);

                            gzip.Flush();
                            gzip.Close();
                        }

                        compressedStream.Position = 0;
                        this.User
                            .Encrypt(compressedStream, cryptedStream);
                    }
                }

                // no need for backup anymore
                if (metaBackupFileToRestore != null)
                {
                    metaBackupFileToRestore.Refresh();
                    if (metaBackupFileToRestore.Exists)
                    {
                        metaBackupFileToRestore.Delete();
                    }
                }
            }
            catch
            {
                // restore backup if avalable
                // before rethrow exception
                if (metaBackupFileToRestore != null)
                {
                    if (metaFile != null)
                    {
                        metaFile.Refresh();
                        if (metaFile.Exists)
                        {
                            metaFile.Delete();
                        }
                    }

                    File.Move(metaBackupFileToRestore.FullName, metaFile.FullName);
                }

                throw;
            }
        }

        /// <inheriteddoc />
        protected override IFile OnUploadFile(string name, Stream src, long bytesToRead)
        {
            var xml = this.GetMetaXml();

            FileStream destStream = null;

            // find next unique and "real" directory
            ulong i = 1;
            FileInfo fi;
            do
            {
                fi = new FileInfo(Path.Combine(this.LocalDirectory.FullName,
                                               i.ToString() + ".bin"));

                if (fi.Exists == false)
                {
                    destStream = fi.Open(FileMode.CreateNew, FileAccess.ReadWrite);

                    break;
                }
            }
            while (++i > 0);

            XElement newFileElement = null;

            try
            {
                fi.Refresh();

                var filesElement = xml.Element(XML_ELEMENT_FILE_LIST);
                if (filesElement == null)
                {
                    filesElement = new XElement(XML_ELEMENT_FILE_LIST);
                    xml.Add(filesElement);
                }

                newFileElement = new XElement(XML_ELEMENT_FILE);
                newFileElement.SetAttributeValue(XML_ATTRIB_NAME, name);
                newFileElement.SetAttributeValue(XML_ATTRIB_REAL_FILE, fi.Name);

                var rand = new Random();
                var rng = new RNGCryptoServiceProvider();

                // password
                var pwd = new byte[64];
                {
                    rng.GetBytes(pwd);

                    var newPasswordElement = new XElement("password");
                    newPasswordElement.Value = Convert.ToBase64String(pwd);

                    newFileElement.Add(newPasswordElement);
                }

                // salt
                var salt = new byte[16];
                {
                    rng.GetBytes(salt);

                    var newSaltElement = new XElement("salt");
                    newSaltElement.Value = Convert.ToBase64String(salt);

                    newFileElement.Add(newSaltElement);
                }

                // iterations
                var iterations = rand.Next(1000, 2000);
                {
                    var newIterationsElement = new XElement("iterations");
                    newIterationsElement.Value = iterations.ToString();

                    newFileElement.Add(newIterationsElement);
                }

                long bytesWritten = 0;

                using (destStream)
                {
                    var cs = CryptoHelper.CreateCryptoStream(destStream, CryptoStreamMode.Write,
                                                             pwd, salt, iterations);

                    try
                    {
                        using (var gzip = new GZipStream(cs, CompressionMode.Compress, true))
                        {
                            try
                            {
                                while (bytesToRead > 0)
                                {
                                    var bufferSize = 81920L;
                                    if (bufferSize > bytesToRead)
                                    {
                                        bufferSize = bytesToRead;
                                    }

                                    var buffer = new byte[bufferSize];

                                    var bytesReadFromSrc = src.Read(buffer, 0, buffer.Length);
                                    if (bytesReadFromSrc < 1)
                                    {
                                        break;
                                    }

                                    gzip.Write(buffer, 0, bytesReadFromSrc);
                                    bytesWritten += bytesReadFromSrc;

                                    bytesToRead -= bytesReadFromSrc;
                                }
                            }
                            finally
                            {
                                gzip.Flush();
                                gzip.Close();
                            }
                        }
                    }
                    finally
                    {
                        cs.FlushFinalBlock();
                    }
                }

                // size
                {
                    var newSizeElement = new XElement("size");
                    newSizeElement.Value = bytesWritten.ToString();

                    newFileElement.Add(newSizeElement);
                }

                filesElement.Add(newFileElement);

                this.OnUpdateMetaXml(xml);
            }
            catch
            {
                using (var ds = destStream)
                {
                    destStream = null;
                }

                fi.Delete();

                throw;
            }

            return new UserFile(this.FileSystem, fi, newFileElement, this);
        }

        internal void UpdateMetaXml(XElement xml)
        {
            lock (this._SYNC)
            {
                this.OnUpdateMetaXml(xml);
            }
        }

        #endregion Methods (11)
    }
}