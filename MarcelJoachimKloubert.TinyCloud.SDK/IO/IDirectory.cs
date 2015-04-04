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

namespace MarcelJoachimKloubert.TinyCloud.SDK.IO
{
    /// <summary>
    /// Describes a directory.
    /// </summary>
    public interface IDirectory : IFileSystemObject
    {
        #region Properties (3)

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

        #endregion Properties (3)

        #region Methods (8)

        /// <summary>
        /// Creates a new sub directory.
        /// </summary>
        /// <param name="name">The name of the new directory.</param>
        /// <returns>The created directory.</returns>
        /// <exception cref="ArgumentException">
        /// Directory does already exist.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="name" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="FormatException">
        /// <paramref name="name" /> is invalid.
        /// </exception>
        IDirectory CreateDirectory(string name);

        /// <summary>
        /// Checks if a directory exists.
        /// </summary>
        /// <param name="name">The name of the directory.</param>
        /// <returns>Directory exists or not.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="name" /> is invalid.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="name" /> is <see langword="null" />.
        /// </exception>
        bool DirectoryExists(string name);

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

        /// <summary>
        /// Returns the chars that are not allowed in a directory name.
        /// </summary>
        /// <returns>The list of chars as one string.</returns>
        string GetInvalidCharsForDirectoryNames();

        /// <summary>
        /// Returns the chars that are not allowed in a file name.
        /// </summary>
        /// <returns>The list of chars as one string.</returns>
        string GetInvalidCharsForFileNames();

        /// <summary>
        /// Uploads a file to this directory.
        /// </summary>
        /// <param name="name">The name of the new file.</param>
        /// <param name="src">The data of the file to upload.</param>
        /// <param name="bytesToRead">The number of bytes to read.</param>
        /// <returns>The uploaded file.</returns>
        /// <exception cref="ArgumentException">
        /// File does already exist.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="name" /> and/or <see cref="src" /> are <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="bytesToRead" /> is invalid.
        /// </exception>
        /// <exception cref="FormatException">
        /// <paramref name="name" /> is invalid.
        /// </exception>
        IFile UploadFile(string name, Stream src, long bytesToRead);

        #endregion Methods (8)
    }
}