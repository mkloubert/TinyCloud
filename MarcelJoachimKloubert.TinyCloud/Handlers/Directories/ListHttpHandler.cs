//  TinyCloud (https://github.com/mkloubert/TinyCloud)
//  Copyright (C) 2015  Marcel Joachim Kloubert <marcel.kloubert@gmx.net>
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Affero General Public License as
//  published by the Free Software Foundation, either version 3 of the
//  License, or (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Affero General Public License for more details.
//
//  You should have received a copy of the GNU Affero General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using MarcelJoachimKloubert.TinyCloud.SDK;
using MarcelJoachimKloubert.TinyCloud.SDK.Extensions;
using MarcelJoachimKloubert.TinyCloud.SDK.Handlers.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MarcelJoachimKloubert.TinyCloud.Handlers.Directories
{
    [RouteHttpHandler("api/list")]
    public sealed class ListHttpHandler : JsonHttpHandlerBase
    {
        #region Properties (1)

        /// <inheriteddoc />
        public override IEnumerable<string> SupportedHttpMethods
        {
            get
            {
                return new string[] { "POST" };
            }
        }

        #endregion Properties (1)

        #region Methods (1)

        /// <inheriteddoc />
        protected override void OnProcessJsonRequest(IHttpRequest request, ref dynamic result)
        {
            IDictionary<string, object> requestData;
            try
            {
                requestData = request.GetRequestBodyAsJson();
            }
            catch
            {
                requestData = null;
            }

            var path = "";

            if (requestData != null)
            {
                if (requestData.ContainsKey("path"))
                {
                    path = requestData["path"].AsString();
                }
            }

            var dir = GetDirectory(request, path);
            if (dir == null)
            {
                // not found

                result.code = 404;
                return;
            }

            var isTest = false;
            if (requestData.ContainsKey("test"))
            {
                var testStr = requestData["test"].AsString();
                if (string.IsNullOrWhiteSpace(testStr) == false)
                {
                    isTest = Convert.ToBoolean(testStr.Trim(), AppServices.DataCulture);
                }
            }

            var resultData = CreateDynamicObject();
            {
                // sub directories
                var dirs = new List<object>();
                if (isTest == false)
                {
                    foreach (var subDir in dir.GetDirectories())
                    {
                        dirs.Add(new
                            {
                                name = subDir.Name,
                                lastWriteTime = GetValueSafe(() => subDir.LastWriteTime),
                            });
                    }
                }

                // files
                var files = new List<object>();
                if (isTest == false)
                {
                    foreach (var file in dir.GetFiles())
                    {
                        files.Add(new
                            {
                                name = file.Name,
                                size = GetValueSafe(() => file.Size, -1),
                                lastWriteTime = GetValueSafe(() => file.LastWriteTime),
                            });
                    }
                }

                resultData.dirs = dirs;
                resultData.files = files;
                resultData.path = "/" + string.Join("/",
                                                    dir.GetDirectoryTree()
                                                       .Reverse()
                                                       .Select(x => x.Name)
                                                       .Where(x => string.IsNullOrWhiteSpace(x) == false));
            }

            result.data = resultData;
        }

        #endregion Methods (1)
    }
}