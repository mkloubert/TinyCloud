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

using MarcelJoachimKloubert.TinyCloud.SDK.Security;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace MarcelJoachimKloubert.TinyCloud.SDK.Handlers.Http
{
    /// <summary>
    /// A HTTP request context.
    /// </summary>
    public interface IHttpRequest : ICloudObject
    {
        #region Properties (9)

        /// <summary>
        /// Gets the underlying context.
        /// </summary>
        HttpContext Context { get; }

        /// <summary>
        /// Gets the input stream.
        /// </summary>
        Stream InputStream { get; }

        /// <summary>
        /// Gets the upper case name of the HTTP request method.
        /// </summary>
        string Method { get; }

        /// <summary>
        /// Gets the output stream.
        /// </summary>
        Stream OutputStream { get; }

        /// <summary>
        /// Gets the request context.
        /// </summary>
        HttpRequest Request { get; }

        /// <summary>
        /// Gets the encoding of the request data.
        /// </summary>
        Encoding RequestEncoding { get; }

        /// <summary>
        /// Gets the response context.
        /// </summary>
        HttpResponse Response { get; }

        /// <summary>
        /// Gets or sets the status code by enumeration value.
        /// </summary>
        HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// Gets the requesting user.
        /// </summary>
        ICloudPrincipal User { get; }

        #endregion Properties (9)

        #region Methods (7)

        /// <summary>
        /// A application specific header.
        /// </summary>
        /// <param name="name">The name of the header to add.</param>
        /// <param name="value">The value of the header.</param>
        /// <returns>That instance.</returns>
        IHttpRequest AddAppResponseHeader(string name, object value);

        /// <summary>
        /// Adds a response header.
        /// </summary>
        /// <param name="name">The name of the header to add.</param>
        /// <param name="value">The value of the header.</param>
        /// <returns>That instance.</returns>
        IHttpRequest AddResponseHeader(string name, object value);

        /// <summary>
        /// Returns an app specific request header.
        /// </summary>
        /// <param name="name">The name of the header.</param>
        /// <returns>The value or <see langword="null" /> if not found.</returns>
        string GetAppRequestHeader(string name);

        /// <summary>
        /// <see cref="HttpRequest.GetBufferlessInputStream()" />
        /// </summary>
        Stream GetBufferlessInputStream();

        /// <summary>
        /// Handles the request body data as JSON.
        /// </summary>
        /// <param name="enc">The custom encoding to use.</param>
        /// <returns>The request body data as JSON data.</returns>
        dynamic GetRequestBodyAsJson(Encoding enc = null);

        /// <summary>
        /// Handles the request body data as string.
        /// </summary>
        /// <param name="enc">The custom encoding to use.</param>
        /// <returns>The request body data as string.</returns>
        string GetRequestBodyAsString(Encoding enc = null);

        /// <summary>
        /// Writes data to <see cref="IHttpRequest.OutputStream" />.
        /// </summary>
        /// <param name="data">The data to write.</param>
        void Write(byte[] data);

        #endregion Methods (7)
    }
}