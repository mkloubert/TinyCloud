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

namespace MarcelJoachimKloubert.TinyCloud.SDK.IO
{
    /// <summary>
    /// Describes a directory.
    /// </summary>
    public interface IDirectory : IFileSystemObject
    {
        #region Properties (2)

        /// <summary>
        /// Gets if that directory is the root directory or not.
        /// </summary>
        bool IsRoot { get; }

        /// <summary>
        /// Gets the last write time of the directory.
        /// </summary>
        DateTimeOffset? LastWriteTime { get; }

        /// <summary>
        /// Gets the parent directory.
        /// </summary>
        IDirectory Parent { get; }

        #endregion Properties (2)

        #region Methods (3)

        /// <summary>
        /// Returns the sub directories of that directory.
        /// </summary>
        /// <returns>The sub directories.</returns>
        IEnumerable<IDirectory> GetDirectories();

        /// <summary>
        /// Returns the directory tree from that directory up to the root.
        /// </summary>
        /// <returns>The directory tree.</returns>
        IEnumerable<IDirectory> GetDirectoryTree();

        /// <summary>
        /// Returns the files of that directory.
        /// </summary>
        /// <returns>The files.</returns>
        IEnumerable<IFile> GetFiles();

        #endregion Methods (3)
    }
}