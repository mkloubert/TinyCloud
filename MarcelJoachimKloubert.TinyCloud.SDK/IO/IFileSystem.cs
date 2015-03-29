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

namespace MarcelJoachimKloubert.TinyCloud.SDK.IO
{
    /// <summary>
    /// Describes a file system.
    /// </summary>
    public interface IFileSystem : ICloudObject
    {
        #region Properties (1)

        /// <summary>
        /// Gets the directory the represents the root.
        /// </summary>
        IDirectory Root { get; }

        #endregion Properties (1)

        #region Methods (2)

        /// <summary>
        /// Returns a directory.
        /// </summary>
        /// <param name="path">The path of the directory.</param>
        /// <returns>
        /// The directory or <see cref="null" /> if not found.
        /// </returns>
        IDirectory GetDirectory(string path);

        /// <summary>
        /// Returns a file.
        /// </summary>
        /// <param name="path">The path of the file.</param>
        /// <returns>
        /// The file or <see cref="null" /> if not found.
        /// </returns>
        IFile GetFile(string path);

        #endregion Methods (2)
    }
}