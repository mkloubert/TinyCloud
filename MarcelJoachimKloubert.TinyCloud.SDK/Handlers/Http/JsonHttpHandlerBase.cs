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

using Newtonsoft.Json;
using System;
using System.IO;

namespace MarcelJoachimKloubert.TinyCloud.SDK.Handlers.Http
{
    /// <summary>
    /// A basic HTTP handler that returns JSON data.
    /// </summary>
    public abstract class JsonHttpHandlerBase : HttpHandlerBase
    {
        #region Constructors (4)

        /// <inheriteddoc />
        protected JsonHttpHandlerBase()
            : base()
        {
        }

        /// <inheriteddoc />
        protected JsonHttpHandlerBase(bool isSynchronized)
            : base(isSynchronized: isSynchronized)
        {
        }

        /// <inheriteddoc />
        protected JsonHttpHandlerBase(object sync)
            : base(sync: sync)
        {
        }

        /// <inheriteddoc />
        protected JsonHttpHandlerBase(bool isSynchronized, object sync)
            : base(isSynchronized: isSynchronized,
                   sync: sync)
        {
        }

        #endregion Constructors (4)

        #region Methods (4)

        /// <summary>
        /// Creates an error object from an <see cref="Exception" />.
        /// </summary>
        /// <param name="ex">The underlying exception.</param>
        /// <returns>The created object.</returns>
        protected static object CreateErrorJsonObject(Exception ex)
        {
            if (ex == null)
            {
                return null;
            }

            return new
            {
                innerEx = CreateErrorJsonObject(ex.InnerException),
                msg = ex.Message,
                type = ex.GetType().FullName,
            };
        }

        /// <summary>
        /// Creates a result object.
        /// </summary>
        /// <param name="code">The result code.</param>
        /// <param name="msg">The result message.</param>
        /// <param name="data">The data that describe the result.</param>
        /// <returns>The created object.</returns>
        protected static dynamic CreateResultObject(int? code, string msg = null, object data = null)
        {
            dynamic result = CreateDynamicObject();
            result.code = code;
            result.msg = msg;
            result.data = data;

            return result;
        }

        /// <summary>
        /// Processes a JSON request.
        /// </summary>
        /// <param name="request">The request context.</param>
        /// <param name="result">The variable where to write the result object.</param>
        protected abstract void OnProcessJsonRequest(IHttpRequest request, ref dynamic result);

        /// <inheriteddoc />
        protected sealed override void OnProcessRequest(IHttpRequest request)
        {
            dynamic result = CreateResultObject(code: 0);

            try
            {
                this.OnProcessJsonRequest(request, ref result);
            }
            catch (Exception ex)
            {
                result = CreateResultObject(code: -1,
                                            msg: (ex.GetBaseException() ?? ex).Message,
                                            data: CreateErrorJsonObject(ex));
            }

            // write to output
            var resultContent = "";
            try
            {
                if (result != null)
                {
                    var serializer = new JsonSerializer();

                    using (var strWriter = new StringWriter())
                    {
                        using (var jsonWriter = new JsonTextWriter(strWriter))
                        {
                            serializer.Serialize(jsonWriter, result);

                            resultContent = strWriter.ToString();
                        }
                    }
                }

                request.Response.ContentType = string.Format("application/json; charset={0}",
                                                             this.Charset.WebName);
                request.AddResponseHeader("Content-Length", resultContent.Length);

                request.Write(this.Charset
                                  .GetBytes(resultContent));
            }
            finally
            {
                resultContent = null;
            }
        }

        #endregion Methods (4)
    }
}