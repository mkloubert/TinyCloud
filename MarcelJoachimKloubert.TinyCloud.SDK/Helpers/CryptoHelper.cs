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
using System.IO;
using System.Security.Cryptography;

namespace MarcelJoachimKloubert.TinyCloud.SDK.Helpers
{
    /// <summary>
    /// Helper class for crypto operations.
    /// </summary>
    public static class CryptoHelper
    {
        #region Methods (3)

        /// <summary>
        /// Creates a <see cref="CryptoStream" /> instance.
        /// </summary>
        /// <param name="baseStream">The base stream.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="pwd">The password.</param>
        /// <param name="salt">The salt.</param>
        /// <param name="iterations">The iterations.</param>
        /// <returns>The created instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="baseStream" /> is <see langword="null" />.
        /// </exception>
        public static CryptoStream CreateCryptoStream(Stream baseStream, CryptoStreamMode mode,
                                                      byte[] pwd, byte[] salt, int iterations)
        {
            if (baseStream == null)
            {
                throw new ArgumentNullException("baseStream");
            }

            ICryptoTransform transform = null;

            using (var db = new Rfc2898DeriveBytes(pwd, salt, iterations))
            {
                using (var alg = Rijndael.Create())
                {
                    alg.Key = db.GetBytes(32);
                    alg.IV = db.GetBytes(16);

                    switch (mode)
                    {
                        case CryptoStreamMode.Read:
                            transform = alg.CreateDecryptor();
                            break;

                        case CryptoStreamMode.Write:
                            transform = alg.CreateEncryptor();
                            break;
                    }
                }
            }

            return new CryptoStream(baseStream, transform, mode);
        }

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
        public static void Decrypt(Stream src, Stream dest,
                                   byte[] pwd, byte[] salt, int iterations)
        {
            if (src == null)
            {
                throw new ArgumentNullException("src");
            }

            if (dest == null)
            {
                throw new ArgumentNullException("dest");
            }

            var cs = CreateCryptoStream(src, CryptoStreamMode.Read,
                                        pwd, salt, iterations);
            cs.CopyTo(dest);
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

            var cs = CreateCryptoStream(dest, CryptoStreamMode.Write,
                                        pwd, salt, iterations);

            src.CopyTo(cs);
            cs.FlushFinalBlock();
        }

        #endregion Methods (3)
    }
}