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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace MarcelJoachimKloubert.TinyCloud.SDK.IO.Users
{
    /// <summary>
    /// A user directory.
    /// </summary>
    public sealed class UserDirectory : DirectoryBase
    {
        #region Fields (5)

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
        /// Stores the name of an element with directory information.
        /// </summary>
        public const string XML_ELEMENT_DIRECTORY = "dir";

        /// <summary>
        /// Stores the name of an elements that contains directory elements.
        /// </summary>
        public const string XML_ELEMENT_DIRECTORY_LIST = "dirs";

        #endregion Fields (5)

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

        #region Methods (7)

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
                        using (var uncryptedStream = new MemoryStream())
                        {
                            this.User
                                .Decrypt(cryptedStream, uncryptedStream);

                            uncryptedStream.Position = 0;
                            result = XDocument.Load(uncryptedStream).Root;
                        }
                    }
                }
            }
            catch
            {
                result = null;
            }

            return result ?? new XElement(XML_ELEMENT_DIRECTORY);
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

                this.UpdateMetaXml(xml);
            }
            catch
            {
                di.Delete();

                throw;
            }

            return new UserDirectory(this.FileSystem, di, newDirElement, this);
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
                       .XPathSelectElements("files/file")
                       .Select(x =>
                       {
                           IFile file = null;

                           try
                           {
                               var nameAttrib = x.Attribute("name");
                               if (nameAttrib != null)
                               {
                                   var name = (nameAttrib.Value ?? string.Empty).Trim();
                                   if (name != string.Empty)
                                   {
                                       var realFileAttrib = x.Attribute("realFile");
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

        private void UpdateMetaXml(XElement xml)
        {
            if (xml == null)
            {
                return;
            }

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
                    using (var uncryptedStream = new MemoryStream())
                    {
                        xml.Save(uncryptedStream);

                        uncryptedStream.Position = 0;
                        this.User
                            .Encrypt(uncryptedStream, cryptedStream);
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

        #endregion Methods (7)
    }
}