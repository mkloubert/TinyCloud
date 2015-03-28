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

using System.IO;
using System.Security.Principal;

namespace MarcelJoachimKloubert.TinyCloud.SDK.Security
{
    /// <summary>
    /// Extension of <see cref="IPrincipal" />.
    /// </summary>
    public interface ICloudPrincipal : IPrincipal, ICloudObject
    {
        #region Properties (2)

        /// <summary>
        /// <see cref="IPrincipal.Identity" />
        /// </summary>
        new ICloudIdentity Identity { get; }

        /// <summary>
        /// Gets if that principal has super admin rights or not.
        /// </summary>
        bool IsSuperAdmin { get; }

        #endregion Properties (1)

        #region Methods (3)

        /// <summary>
        /// Decrypts data for that principal.
        /// </summary>
        /// <param name="src">The stream with crypted data.</param>
        /// <param name="dest">The stream where to write the uncrypted data to.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="src" /> and/or <paramref name="dest" /> are <see langword="null" />.
        /// </exception>
        void Decrypt(Stream src, Stream dest);

        /// <summary>
        /// Encrypts data for that principal.
        /// </summary>
        /// <param name="src">The stream with uncrypted data.</param>
        /// <param name="dest">The stream where to write the crypted data to.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="src" /> and/or <paramref name="dest" /> are <see langword="null" />.
        /// </exception>
        void Encrypt(Stream src, Stream dest);

        /// <summary>
        /// Checks if a resource is allowed.
        /// </summary>
        /// <param name="name">The name of the resource.</param>
        /// <returns>Is allowed or now.</returns>
        bool IsResourceAllowed(string name);

        #endregion Methods (3)
    }
}