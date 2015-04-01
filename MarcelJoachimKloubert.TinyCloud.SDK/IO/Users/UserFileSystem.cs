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
using System.Linq;

namespace MarcelJoachimKloubert.TinyCloud.SDK.IO.Users
{
    /// <summary>
    /// Gets the file system of a user.
    /// </summary>
    public sealed class UserFileSystem : FileSystemBase
    {
        #region Fields (2)

        private const char _PATH_SEPARATOR = '/';
        private readonly IDirectory _ROOT;

        #endregion Fields (2)

        #region Constructors (1)

        /// <inheriteddoc />
        public UserFileSystem(ICloudPrincipal user)
            : base()
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            this.User = user;

            this._ROOT = new UserDirectory(this,
                                           user.GetDataDirectory(), null);
        }

        #endregion Constructors (1)

        #region Properties (1)

        /// <inheriteddoc />
        public override IDirectory Root
        {
            get { return this._ROOT; }
        }

        /// <summary>
        /// Gets the underlying user.
        /// </summary>
        public ICloudPrincipal User
        {
            get;
            private set;
        }

        #endregion Properties (1)

        #region Methods (3)

        /// <inheriteddoc />
        public override IDirectory GetDirectory(string path)
        {
            path = NormalizePath(path);

            IDirectory result = this.Root;
            if (result == null)
            {
                return null;
            }

            if (path != string.Empty)
            {
                var parts = path.Split(_PATH_SEPARATOR)
                                .Select(x => x.Trim())
                                .Where(x => x != string.Empty)
                                .ToList();

                for (var i = 0; i < parts.Count; i++)
                {
                    var p = parts[i];

                    result = result.GetDirectories()
                                   .FirstOrDefault(x => x.Name == p);

                    if (result == null)
                    {
                        break;
                    }
                }
            }

            return result;
        }

        /// <inheriteddoc />
        public override IFile GetFile(string path)
        {
            path = NormalizePath(path);

            IFile result = null;

            if (string.IsNullOrWhiteSpace(path) == false)
            {
                IDirectory currentDir = this.Root;

                if (currentDir != null)
                {
                    var parts = path.Split(_PATH_SEPARATOR)
                                    .Select(x => x.Trim())
                                    .Where(x => x != string.Empty)
                                    .ToList();

                    for (var i = 0; i < parts.Count; i++)
                    {
                        var p = parts[i];

                        if (i == (parts.Count - 1))
                        {
                            // last element => file

                            result = currentDir.GetFiles()
                                               .FirstOrDefault(x => x.Name == p);
                        }
                        else
                        {
                            currentDir = currentDir.GetDirectories()
                                                   .FirstOrDefault(x => x.Name == p);

                            if (currentDir == null)
                            {
                                break;
                            }
                        }
                    }
                }
            }

            return result;
        }

        private static string NormalizePath(string path)
        {
            if (path == null)
            {
                path = string.Empty;
            }

            path = path.Replace(Path.DirectorySeparatorChar, _PATH_SEPARATOR)
                       .Trim();

            while (path.StartsWith(_PATH_SEPARATOR.ToString()))
            {
                path = path.Substring(1)
                           .Trim();
            }

            while (path.EndsWith(_PATH_SEPARATOR.ToString()))
            {
                path = path.Substring(0, path.Length - 1)
                           .Trim();
            }

            return path;
        }

        #endregion Methods (3)
    }
}