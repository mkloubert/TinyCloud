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

using MarcelJoachimKloubert.TinyCloud.SDK.IO;
using MarcelJoachimKloubert.TinyCloud.SDK.Security;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Xml.Linq;
using System.Xml.XPath;

namespace MarcelJoachimKloubert.TinyCloud.SDK.Handlers.Http
{
    partial class HttpHandlerBase
    {
        private sealed class CloudPrincipal : CloudObjectBase, ICloudPrincipal
        {
            #region Properties (6)

            public IDirectory Directory
            {
                get;
                internal set;
            }

            internal CloudIdentity Identity
            {
                get;
                set;
            }

            ICloudIdentity ICloudPrincipal.Identity
            {
                get { return this.Identity; }
            }

            IIdentity IPrincipal.Identity
            {
                get { return this.Identity; }
            }

            public bool IsSuperAdmin
            {
                get { return this.IsResourceAllowed("is_super_admin"); }
            }

            internal XElement Xml
            {
                get;
                set;
            }

            #endregion Properties (6)

            #region Methods (6)

            public void Decrypt(Stream src, Stream dest)
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
                    byte[] pwd;
                    byte[] salt;
                    int iterations;
                    this.GetCryptoData(out pwd, out salt, out iterations);

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

            public void Encrypt(Stream src, Stream dest)
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
                    byte[] pwd;
                    byte[] salt;
                    int iterations;
                    this.GetCryptoData(out pwd, out salt, out iterations);

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

            private void GetCryptoData(out byte[] pwd, out byte[] salt, out int iterations)
            {
                pwd = null;
                salt = null;
                iterations = int.MinValue;

                var xml = this.Xml;
                if (xml == null)
                {
                    return;
                }

                var passwordElement = xml.XPathSelectElement("encryption/password");
                if (passwordElement != null)
                {
                    if (string.IsNullOrWhiteSpace(passwordElement.Value) == false)
                    {
                        pwd = Convert.FromBase64String(passwordElement.Value.Trim());
                    }
                }

                var saltElement = xml.XPathSelectElement("encryption/salt");
                if (saltElement != null)
                {
                    if (string.IsNullOrWhiteSpace(saltElement.Value) == false)
                    {
                        salt = Convert.FromBase64String(saltElement.Value.Trim());
                    }
                }

                var iterationsElement = xml.XPathSelectElement("encryption/iterations");
                if (iterationsElement != null)
                {
                    if (string.IsNullOrWhiteSpace(iterationsElement.Value) == false)
                    {
                        iterations = int.Parse(iterationsElement.Value.Trim(),
                                               AppServices.DataCulture);
                    }
                }
            }

            public DirectoryInfo GetDataDirectory()
            {
                return new DirectoryInfo(Path.Combine(AppServices.GetDataDirectory().FullName,
                                                      this.Identity.Name));
            }

            public bool IsInRole(string role)
            {
                //TODO
                return false;
            }

            public bool IsResourceAllowed(string name)
            {
                name = (name ?? string.Empty).ToLower().Trim();
                if (name != string.Empty)
                {
                    var xml = this.Xml;
                    if (xml != null)
                    {
                        foreach (var resourceElement in xml.XPathSelectElements("resources/resource"))
                        {
                            var resName = (resourceElement.Value ?? string.Empty).ToLower().Trim();
                            if (resName == name)
                            {
                                var isAllowed = true;

                                var isAllowedAttrib = resourceElement.Attribute("isAllowed");
                                if (isAllowedAttrib != null)
                                {
                                    switch ((isAllowedAttrib.Value ?? string.Empty).ToLower().Trim())
                                    {
                                        case "0":
                                        case "f":
                                        case "false":
                                        case "n":
                                        case "no":
                                            isAllowed = false;
                                            break;
                                    }
                                }

                                // found
                                if (isAllowed)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }

                // not found
                return false;
            }

            #endregion Methods (6)
        }
    }
}