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
using System.Net;
using System.Text;

namespace MarcelJoachimKloubert.TinyCloud.SDK.Extensions
{
    /// <summary>
    /// Extension methods for web operations.
    /// </summary>
    public static class TinyCloudWebExtensionMethods
    {
        #region Methods (6)

        /// <summary>
        /// Returns the UTF-8 data of the response of an empty request as JSON object.
        /// </summary>
        /// <param name="request">The request context.</param>
        /// <returns>The JSON object.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="request" /> is <see langword="null" />.
        /// </exception>
        public static dynamic GetJson(this WebRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            return GetJson(request, Encoding.UTF8);
        }

        /// <summary>
        /// Returns the of the response of an empty request as JSON object.
        /// </summary>
        /// <param name="request">The request context.</param>
        /// <param name="enc">The encoding to use.</param>
        /// <returns>The JSON object.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="request" /> and/or <paramref name="enc" /> are <see langword="null" />.
        /// </exception>
        public static dynamic GetJson(this WebRequest request, Encoding enc)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            // send nothing
            using (var stream = request.GetRequestStream())
            { }

            return GetJson(request.GetResponse(), enc);
        }

        /// <summary>
        /// Returns the UTF-8 data of a response as JSON object.
        /// </summary>
        /// <param name="response">The response context.</param>
        /// <returns>The JSON object.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="response" /> is <see langword="null" />.
        /// </exception>
        public static dynamic GetJson(this WebResponse response)
        {
            return GetJson(response, Encoding.UTF8);
        }

        /// <summary>
        /// Returns the data of a response as JSON object.
        /// </summary>
        /// <param name="response">The response context.</param>
        /// <param name="enc">The encoding to use.</param>
        /// <returns>The JSON object.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="response" /> and/or <paramref name="enc" /> are <see langword="null" />.
        /// </exception>
        public static dynamic GetJson(this WebResponse response, Encoding enc)
        {
            if (response == null)
            {
                throw new ArgumentNullException("response");
            }

            if (enc == null)
            {
                throw new ArgumentNullException("enc");
            }

            using (var textReader = new StreamReader(response.GetResponseStream(), enc))
            {
                var serializer = new JsonSerializer();

                using (var jsonReader = new JsonTextReader(textReader))
                {
                    return serializer.Deserialize<global::System.Dynamic.ExpandoObject>(jsonReader);
                }
            }
        }

        /// <summary>
        /// Sends an object as JSON data.
        /// </summary>
        /// <param name="request">The request context.</param>
        /// <param name="obj">The object to send.</param>
        /// <param name="enc">The encoding to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="request" /> is <see langword="null" />.
        /// </exception>
        public static void SendJson(this WebRequest request, object obj)
        {
            SendJson(request, obj, Encoding.UTF8);
        }

        /// <summary>
        /// Sends an object as JSON data.
        /// </summary>
        /// <param name="request">The request context.</param>
        /// <param name="obj">The object to send.</param>
        /// <param name="enc">The encoding to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="request" /> and/or <paramref name="enc" /> are <see langword="null" />.
        /// </exception>
        public static void SendJson(this WebRequest request, object obj, Encoding enc)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            if (enc == null)
            {
                throw new ArgumentNullException("enc");
            }

            if (DBNull.Value.Equals(obj))
            {
                obj = null;
            }

            request.ContentType = "application/json; charset=" + enc.WebName;

            using (var stream = request.GetRequestStream())
            {
                var serializer = new JsonSerializer();

                using (var streamWriter = new StreamWriter(stream, enc))
                {
                    using (var jsonWriter = new JsonTextWriter(streamWriter))
                    {
                        serializer.Serialize(jsonWriter, obj);

                        streamWriter.Flush();
                    }
                }
            }
        }

        #endregion Methods (6)
    }
}