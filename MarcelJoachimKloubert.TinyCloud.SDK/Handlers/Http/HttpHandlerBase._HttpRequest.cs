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

using MarcelJoachimKloubert.TinyCloud.SDK.Extensions;
using MarcelJoachimKloubert.TinyCloud.SDK.Security;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace MarcelJoachimKloubert.TinyCloud.SDK.Handlers.Http
{
    partial class HttpHandlerBase
    {
        private sealed class _HttpRequest : CloudObjectBase, IHttpRequest
        {
            #region Fields (1)

            internal const string APP_HEADER_NAME_PREFIX = "X-TinyCloud-";

            #endregion Fields (1)

            #region Properties (9)

            public HttpContext Context
            {
                get;
                internal set;
            }

            public Stream InputStream
            {
                get { return this.Request.InputStream; }
            }

            public string Method
            {
                get
                {
                    var result = (this.Request.HttpMethod ?? string.Empty).ToUpper().Trim();

                    return result != string.Empty ? result : null;
                }
            }

            public Stream OutputStream
            {
                get { return this.Response.OutputStream; }
            }

            public HttpRequest Request
            {
                get { return this.Context.Request; }
            }

            public Encoding RequestEncoding
            {
                get { return this.Request.ContentEncoding ?? Encoding.UTF8; }
            }

            public HttpResponse Response
            {
                get { return this.Context.Response; }
            }

            public HttpStatusCode StatusCode
            {
                get { return (HttpStatusCode)this.Response.StatusCode; }

                set { this.Response.StatusCode = (int)value; }
            }

            public ICloudPrincipal User
            {
                get;
                internal set;
            }

            #endregion Properties (9)

            #region Methods (7)

            public IHttpRequest AddAppResponseHeader(string name, object value)
            {
                if (string.IsNullOrWhiteSpace(name) == false)
                {
                    name = APP_HEADER_NAME_PREFIX + name.Trim();

                    this.AddResponseHeader(name, value);
                }

                return this;
            }

            public IHttpRequest AddResponseHeader(string name, object value)
            {
                if (string.IsNullOrWhiteSpace(name) == false)
                {
                    this.Response.AddHeader(name.Trim(),
                                        value.AsString() ?? string.Empty);
                }

                return this;
            }

            public string GetAppRequestHeader(string name)
            {
                if (string.IsNullOrWhiteSpace(name) == false)
                {
                    name = APP_HEADER_NAME_PREFIX + name.Trim();

                    if (this.Request.Headers.AllKeys.Any(k => (k ?? string.Empty).ToLower().Trim() ==
                                                              name.ToLower().Trim()))
                    {
                        return this.Request.Headers[name] ?? string.Empty;
                    }
                }

                return null;
            }

            public Stream GetBufferlessInputStream()
            {
                return this.Request.GetBufferlessInputStream();
            }

            public dynamic GetRequestBodyAsJson(Encoding enc = null)
            {
                if (enc == null)
                {
                    enc = this.RequestEncoding;
                }

                using (var textReader = new StreamReader(this.GetBufferlessInputStream(), enc))
                {
                    using (var jsonReader = new JsonTextReader(textReader))
                    {
                        return new JsonSerializer().Deserialize<global::System.Dynamic.ExpandoObject>(jsonReader);
                    }
                }
            }

            public string GetRequestBodyAsString(Encoding enc = null)
            {
                if (enc == null)
                {
                    enc = this.RequestEncoding;
                }

                using (var reader = new StreamReader(this.GetBufferlessInputStream(), enc))
                {
                    return reader.ReadToEnd();
                }
            }

            public void Write(byte[] data)
            {
                if (data == null)
                {
                    return;
                }

                this.OutputStream
                    .Write(data, 0, data.Length);
            }

            #endregion Methods (7)
        }
    }
}