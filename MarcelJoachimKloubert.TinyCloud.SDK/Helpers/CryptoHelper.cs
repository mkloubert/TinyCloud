﻿//  TinyCloud SDK (https://github.com/mkloubert/TinyCloud)
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
using System.IO;
using System.Security.Cryptography;

namespace MarcelJoachimKloubert.TinyCloud.SDK.Helpers
{
    /// <summary>
    /// Helper class for crypto operations.
    /// </summary>
    public static class CryptoHelper
    {
        #region Methods (2)

        /// <summary>
        /// Decrypts data.
        /// </summary>
        /// <param name="src">The stream with crypted data.</param>
        /// <param name="dest">The stream where to write the uncrypted data to.</param>
        /// <param name="pwd">The password.</param>
        /// <param name="salt">The salt.</param>
        /// <param name="iterations">The iterations.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="src" /> and/or <paramref name="dest" /> are <see langword="null" />.
        /// </exception>
        public static void Decrypt(Stream src, Stream dest, byte[] pwd, byte[] salt, int iterations)
        {
            if (src == null)
            {
                throw new ArgumentNullException("src");
            }

            if (dest == null)
            {
                throw new ArgumentNullException("dest");
            }

            using (var alg = Rijndael.Create())
            {
                using (var db = new Rfc2898DeriveBytes(pwd, salt, iterations))
                {
                    alg.Key = db.GetBytes(32);
                    alg.IV = db.GetBytes(16);

                    using (var transform = alg.CreateDecryptor())
                    {
                        var cryptoStream = new CryptoStream(src, transform, CryptoStreamMode.Read);

                        cryptoStream.CopyTo(dest);
                    }
                }
            }
        }

        /// <summary>
        /// Encrypts data.
        /// </summary>
        /// <param name="src">The stream with uncrypted data.</param>
        /// <param name="dest">The stream where to write the crypted data to.</param>
        /// <param name="pwd">The password.</param>
        /// <param name="salt">The salt.</param>
        /// <param name="iterations">The iterations.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="src" /> and/or <paramref name="dest" /> are <see langword="null" />.
        /// </exception>
        public static void Encrypt(Stream src, Stream dest, byte[] pwd, byte[] salt, int iterations)
        {
            if (src == null)
            {
                throw new ArgumentNullException("src");
            }

            if (dest == null)
            {
                throw new ArgumentNullException("dest");
            }

            using (var alg = Rijndael.Create())
            {
                using (var db = new Rfc2898DeriveBytes(pwd, salt, iterations))
                {
                    alg.Key = db.GetBytes(32);
                    alg.IV = db.GetBytes(16);

                    using (var transform = alg.CreateEncryptor())
                    {
                        var cryptoStream = new CryptoStream(dest, transform, CryptoStreamMode.Write);

                        src.CopyTo(cryptoStream);
                        cryptoStream.FlushFinalBlock();
                    }
                }
            }
        }

        #endregion Methods (2)
    }
}