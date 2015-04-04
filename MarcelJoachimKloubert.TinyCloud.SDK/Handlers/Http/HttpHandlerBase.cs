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
using MarcelJoachimKloubert.TinyCloud.SDK.IO;
using MarcelJoachimKloubert.TinyCloud.SDK.IO.Users;
using MarcelJoachimKloubert.TinyCloud.SDK.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Routing;
using System.Web.SessionState;
using System.Xml.Linq;

namespace MarcelJoachimKloubert.TinyCloud.SDK.Handlers.Http
{
    /// <summary>
    /// A basic HTTP handler.
    /// </summary>
    public abstract partial class HttpHandlerBase : CloudObjectBase,
                                                    IHttpHandler, IRouteHandler, IRequiresSessionState
    {
        #region Fields (1)

        private readonly Action<HttpContext> _PROCESS_REQUEST_ACTION;

        #endregion Fields (1)

        #region Constructors (4)

        /// <inheriteddoc />
        protected HttpHandlerBase()
            : this(isSynchronized: false)
        {
        }

        /// <inheriteddoc />
        protected HttpHandlerBase(bool isSynchronized)
            : this(isSynchronized: isSynchronized,
                   sync: new object())
        {
        }

        /// <inheriteddoc />
        protected HttpHandlerBase(object sync)
            : base(sync: sync,
                   isSynchronized: false)
        {
        }

        /// <inheriteddoc />
        protected HttpHandlerBase(bool isSynchronized, object sync)
            : base(isSynchronized: isSynchronized,
                   sync: sync)
        {
            if (this._IS_SYNCHRONIZED)
            {
                this._PROCESS_REQUEST_ACTION = this.ProcessRequestInner_ThreadSafe;
            }
            else
            {
                this._PROCESS_REQUEST_ACTION = this.ProcessRequestInner;
            }
        }

        #endregion Constructors (4)

        #region Properties (4)

        /// <summary>
        /// Gets the default encoding / charset to use.
        /// </summary>
        public virtual Encoding Charset
        {
            get { return Encoding.UTF8; }
        }

        /// <inheriteddoc />
        public virtual bool IsReusable
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the name of the HTTP basic auth realm.
        /// </summary>
        public virtual string RealmName
        {
            get { return "TinyCloud"; }
        }

        /// <summary>
        /// Gets the list of supported HTTP methods.
        /// </summary>
        public virtual IEnumerable<string> SupportedHttpMethods
        {
            get
            {
                return new string[] { };
            }
        }

        #endregion Properties (4)

        #region Methods (9)

        /// <summary>
        /// Creates an empty, dynamic and exandable object.
        /// </summary>
        /// <returns>The new object.</returns>
        protected static dynamic CreateDynamicObject()
        {
            return new global::System.Dynamic.ExpandoObject();
        }

        /// <summary>
        /// Returns the directory of a user by path and HTTP request context.
        /// </summary>
        /// <param name="request">The HTTP request.</param>
        /// <param name="path">The path.</param>
        /// <returns>
        /// The directory or <see langword="null" /> if not found.
        /// </returns>
        protected static IDirectory GetDirectory(IHttpRequest request, string path)
        {
            if (request == null)
            {
                return null;
            }

            return GetDirectory(request.User, path);
        }

        /// <summary>
        /// Returns the directory of a user by path.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="path">The path.</param>
        /// <returns>
        /// The directory or <see langword="null" /> if not found.
        /// </returns>
        protected static IDirectory GetDirectory(ICloudPrincipal user, string path)
        {
            if (user == null)
            {
                return null;
            }

            var userDir = user.Directory;
            if (userDir == null)
            {
                return null;
            }

            var fs = userDir.FileSystem;
            if (fs == null)
            {
                return null;
            }

            path = (path ?? string.Empty).Trim();
            if (path == string.Empty)
            {
                path = "/";
            }

            return fs.GetDirectory(path);
        }

        /// <inheriteddoc />
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return this;
        }

        /// <summary>
        /// Returns a normalized and sorted list of all supported HTTP methods.
        /// </summary>
        /// <returns>The list of supported HTTP methods.</returns>
        protected List<string> GetSupportedHttpMethods()
        {
            var supportedMethods = new HashSet<string>();
            foreach (var m in (this.SupportedHttpMethods ?? Enumerable.Empty<string>()).OfType<string>()
                                                                                       .Select(x => x.ToUpper().Trim())
                                                                                       .Where(x => x != string.Empty))
            {
                supportedMethods.Add(m);
            }

            supportedMethods.Add("OPTIONS");
            supportedMethods.Add("TRACE");

            return supportedMethods.OrderBy(x => x, StringComparer.InvariantCultureIgnoreCase)
                                   .ToList();
        }

        /// <summary>
        /// Stores the logic for the <see cref="HttpHandlerBase.ProcessRequest(HttpContext)" /> method.
        /// </summary>
        /// <param name="request">The underlying HTTP request context.</param>
        protected abstract void OnProcessRequest(IHttpRequest request);

        /// <inheriteddoc />
        public void ProcessRequest(HttpContext context)
        {
            this._PROCESS_REQUEST_ACTION(context);
        }

        private void ProcessRequestInner(HttpContext context)
        {
            var request = new _HttpRequest()
            {
                Context = context,
                StatusCode = HttpStatusCode.MethodNotAllowed,
            };

            CloudPrincipal user = null;

            try
            {
                var data = (context.Request.ServerVariables["HTTP_AUTHORIZATION"] ?? string.Empty).Trim();
                if (data != string.Empty)
                {
                    if (data.ToLower().StartsWith("basic "))
                    {
                        var enc = Encoding.UTF8;

                        var base64EncodedData = data.Substring(data.IndexOf(" ")).Trim();
                        var blobData = Convert.FromBase64String(base64EncodedData);

                        var strData = enc.GetString(blobData);
                        try
                        {
                            var semicolon = strData.IndexOf(":");
                            if (semicolon > -1)
                            {
                                var username = strData.Substring(0, semicolon).ToLower().Trim();
                                if (username == string.Empty)
                                {
                                    username = null;
                                }

                                if (username != null)
                                {
                                    var userDir = new DirectoryInfo(Path.Combine(AppServices.GetDataDirectory().FullName,
                                                                                 username));

                                    if (userDir.Exists)
                                    {
                                        string pwd = null;
                                        try
                                        {
                                            if (semicolon < (strData.Length - 1))
                                            {
                                                pwd = strData.Substring(semicolon + 1);
                                            }

                                            var metaFile = new FileInfo(Path.Combine(userDir.FullName, "meta.dat"));
                                            if (metaFile.Exists)
                                            {
                                                using (var metaStream = metaFile.OpenRead())
                                                {
                                                    var newUser = new CloudPrincipal();

                                                    newUser.Identity = new CloudIdentity();
                                                    newUser.Identity.AuthenticationType = "basicauth";
                                                    newUser.Identity.IsAuthenticated = false;
                                                    newUser.Identity.Name = username;

                                                    using (var uncryptedMetaStream = new MemoryStream())
                                                    {
                                                        CryptoHelper.Decrypt(metaStream, uncryptedMetaStream,
                                                                             pwd != null ? AppServices.Charset.GetBytes(pwd) : null,
                                                                             AppServices.GetUserPasswordSalt(),
                                                                             AppServices.GetUserPasswordIterations());
                                                        uncryptedMetaStream.Position = 0;

                                                        newUser.Xml = XDocument.Load(uncryptedMetaStream).Root;
                                                    }

                                                    if (newUser.Xml != null)
                                                    {
                                                        var nameAttrib = newUser.Xml.Attribute("name");
                                                        if (nameAttrib != null)
                                                        {
                                                            var name = (nameAttrib.Value ?? string.Empty).ToLower().Trim();
                                                            if (name == username)
                                                            {
                                                                newUser.Identity.IsAuthenticated = true;
                                                                newUser.Directory = new UserDirectory(new UserFileSystem(newUser),
                                                                                                      newUser.GetDataDirectory(), null);

                                                                user = newUser;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        finally
                                        {
                                            pwd = null;
                                        }
                                    }
                                }
                            }
                        }
                        finally
                        {
                            strData = null;
                        }
                    }
                }
            }
            catch
            {
                user = null;
            }

            request.User = user;

            try
            {
                request.AddAppReponseHeader("Version",
                                            this.GetType().Assembly.GetName().Version);

                if (request.User != null &&
                    request.User.Identity != null &&
                    request.User.Identity.IsAuthenticated)
                {
                    switch (request.Method)
                    {
                        case "OPTIONS":
                            {
                                var enc = Encoding.UTF8;
                                const string NEW_LINE = "\r\n";

                                request.Response.ContentType = "text/plain; charset=" + enc.WebName;
                                using (var writer = new StreamWriter(request.OutputStream, enc))
                                {
                                    foreach (var m in this.GetSupportedHttpMethods())
                                    {
                                        writer.Write("{0}{1}",
                                                     m, NEW_LINE);
                                    }
                                }
                            }
                            break;

                        case "TRACE":
                            {
                                // headers
                                foreach (var reqKey in context.Request.Headers.AllKeys)
                                {
                                    context.Response.Headers[reqKey] = context.Request.Headers[reqKey];
                                }

                                // body
                                request.GetBufferlessInputStream()
                                       .CopyTo(request.OutputStream);
                            }
                            break;

                        default:
                            if (this.GetSupportedHttpMethods().Contains(request.Method))
                            {
                                request.StatusCode = HttpStatusCode.OK;

                                this.OnProcessRequest(request);
                            }
                            else
                            {
                                request.StatusCode = HttpStatusCode.MethodNotAllowed;
                            }
                            break;
                    }
                }
                else
                {
                    request.StatusCode = HttpStatusCode.Unauthorized;

                    request.AddResponseHeader("WWW-Authenticate",
                                              string.Format("Basic realm=\"{0}\"",
                                                            this.RealmName));
                }
            }
            catch
            {
                request.StatusCode = HttpStatusCode.InternalServerError;
            }
        }

        private void ProcessRequestInner_ThreadSafe(HttpContext context)
        {
            lock (this._SYNC)
            {
                this.ProcessRequestInner(context);
            }
        }

        #endregion Methods (9)
    }
}