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

namespace MarcelJoachimKloubert.TinyCloud.SDK.IO
{
    /// <summary>
    /// Describes a file.
    /// </summary>
    public interface IFile : IFileSystemObject
    {
        #region Properties (3)

        /// <summary>
        /// Gets the directory the file belongs to.
        /// </summary>
        IDirectory Directory { get; }

        /// <summary>
        /// Gets the last write time of the file.
        /// </summary>
        DateTimeOffset? LastWriteTime { get; }

        /// <summary>
        /// Gets the size of the file.
        /// </summary>
        long? Size { get; }

        #endregion Properties (3)
    }
}