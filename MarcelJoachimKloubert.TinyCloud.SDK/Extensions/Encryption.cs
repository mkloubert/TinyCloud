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
using System;
using System.IO;

namespace MarcelJoachimKloubert.TinyCloud.SDK.Extensions
{
    /// <summary>
    /// Extension methods for handling encryption operations.
    /// </summary>
    public static class TinyCloudEncryptionExtensionMethods
    {
        #region Methods (2)

        /// <summary>
        /// Decrypts data with the system wide password.
        /// </summary>
        /// <param name="src">The stream with crypted data.</param>
        /// <param name="dest">The stream where to write the uncrypted data to.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="src" /> and/or <paramref name="dest" /> are <see langword="null" />.
        /// </exception>
        public static void Decrypt(this Stream src, Stream dest)
        {
            CryptoHelper.Decrypt(src, dest,
                                  AppServices.GetPassword(),
                                                       AppServices.GetPasswordSalt(),
                                                       AppServices.GetPasswordIterations());
        }

        /// <summary>
        /// Encrypts data with the system wide password.
        /// </summary>
        /// <param name="src">The stream with uncrypted data.</param>
        /// <param name="dest">The stream where to write the crypted data to.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="src" /> and/or <paramref name="dest" /> are <see langword="null" />.
        /// </exception>
        public static void Encrypt(this Stream src, Stream dest)
        {
            CryptoHelper.Encrypt(src, dest,
                                  AppServices.GetPassword(),
                                  AppServices.GetPasswordSalt(),
                                  AppServices.GetPasswordIterations());
        }

        #endregion Methods (2)
    }
}