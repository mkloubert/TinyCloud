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
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace MarcelJoachimKloubert.TinyCloud.SDK
{
    /// <summary>
    /// A connection to a cloud.
    /// </summary>
    public sealed class CloudConnection : CloudDisposableBase
    {
        #region Fields (2)

        private readonly Uri _BASE_URL;
        private readonly NetworkCredential _CREDENTIALS;

        #endregion Fields (2)

        #region Constructors (1)

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudConnection" /> class.
        /// </summary>
        /// <param name="baseUrl">The base URL of the cloud.</param>
        /// <param name="credentials">The credentials to use.</param>
        public CloudConnection(Uri baseUrl, NetworkCredential credentials)
        {
            if (baseUrl == null)
            {
                throw new ArgumentNullException("baseUrl");
            }

            if (credentials == null)
            {
                throw new ArgumentNullException("credentials");
            }

            switch ((baseUrl.Scheme ?? string.Empty).ToLower().Trim())
            {
                case "http":
                case "https":
                    break;

                default:
                    throw new NotSupportedException(string.Format("URI scheme '{0}' is NOT supported!",
                                                                  baseUrl.Scheme));
            }

            this._BASE_URL = baseUrl;
            this._CREDENTIALS = credentials;
        }

        #endregion Constructors (1)

        #region Methods (2)

        public WebRequest CreateApiRequest(string action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            if (string.IsNullOrWhiteSpace(action))
            {
                throw new ArgumentException("action");
            }

            this.ThrowIfDisposed();

            var uri = this._BASE_URL.ToString();
            if (uri.EndsWith("/") == false)
            {
                uri += "/";
            }

            var result = (HttpWebRequest)HttpWebRequest.Create(uri + "api/" + action.ToLower().Trim());
            result.Method = "POST";

            // set auth information
            string domain = null;
            string user = null;
            string pwd = null;
            string authInfo = null;
            try
            {
                user = this._CREDENTIALS.UserName;
                if (string.IsNullOrWhiteSpace(user) == false)
                {
                    user = user.Trim();

                    domain = this._CREDENTIALS.Domain;
                    if (string.IsNullOrWhiteSpace(domain) == false)
                    {
                        user = domain.Trim() + "\\" + user;
                    }

                    if (user.Contains(":"))
                    {
                        throw new FormatException("User contains invalid chars!");
                    }

                    var secPwd = this._CREDENTIALS.SecurePassword;
                    if (secPwd != null)
                    {
                        var ptr = IntPtr.Zero;
                        try
                        {
                            ptr = Marshal.SecureStringToGlobalAllocUnicode(secPwd);
                            pwd = Marshal.PtrToStringUni(ptr);
                        }
                        finally
                        {
                            Marshal.ZeroFreeGlobalAllocUnicode(ptr);
                        }
                    }
                    else
                    {
                        pwd = this._CREDENTIALS.Password;
                    }

                    authInfo = string.Format("{0}:{1}",
                                             user, pwd);

                    result.Headers["Authorization"] = string.Format("Basic {0}",
                                                                    Convert.ToBase64String(Encoding.GetEncoding("ASCII")
                                                                                                   .GetBytes(authInfo)));
                }
            }
            finally
            {
                domain = null;
                user = null;
                pwd = null;
                authInfo = null;
            }

            return result;
        }

        /// <inheriteddoc />
        protected override void OnDispose(bool disposing)
        {
            // dummy
        }

        #endregion Methods (2)
    }
}