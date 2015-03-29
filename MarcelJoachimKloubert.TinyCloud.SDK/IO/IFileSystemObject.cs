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
    /// Describes an object that is part of a file system.
    /// </summary>
    public interface IFileSystemObject : ICloudObject
    {
        #region Properties (2)

        /// <summary>
        /// Gets the file system the object belongs to.
        /// </summary>
        IFileSystem FileSystem { get; }

        /// <summary>
        /// Gets the name of the object.
        /// </summary>
        string Name { get; }

        #endregion Properties (2)
    }
}