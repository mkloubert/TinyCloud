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

using MarcelJoachimKloubert.TinyCloud.SDK.Extensions;
using MarcelJoachimKloubert.TinyCloud.SDK.Handlers.Http;
using System.Collections.Generic;
using System.Linq;

namespace MarcelJoachimKloubert.TinyCloud.Handlers.Directories
{
    [RouteHttpHandler("api/create-directory")]
    public class CreateDirectoryHttpHandler : JsonHttpHandlerBase
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

            if (requestData == null)
            {
                result.code = 1;

                return;
            }

            if (requestData.ContainsKey("path") == false)
            {
                // no path defined (property not found)

                result.code = 2;
                return;
            }

            var path = (requestData["path"].AsString() ?? string.Empty).Trim();
            if (path == string.Empty)
            {
                // no path defined (no value)

                result.code = 3;
                return;
            }

            // remove ending 
            while (path.EndsWith("/"))
            {
                path = path.Substring(0, path.Length - 1);
            }

            if (path.StartsWith("/") == false)
            {
                path = "/" + path;
            }

            var parts = path.Split('/')
                            .Where(x => string.IsNullOrWhiteSpace(x) == false)
                            .ToList();
            if (parts.Count < 1)
            {
                // no directory defined

                result.code = 4;
                return;
            }

            var fullPath = "/" + string.Join("/",
                                             parts.Take(parts.Count - 1));
            var dir = GetDirectory(request, fullPath);
            if (dir == null)
            {
                // parent directory not found

                result.code = 404;
                return;
            }

            var newDir = parts.Last();
            if (string.IsNullOrWhiteSpace(newDir))
            {
                // invalid value

                result.code = 5;
                return;
            }

            newDir = newDir.Trim();
            if (dir.DirectoryExists(newDir))
            {
                // directory already exists

                result.code = 6;
                return;
            }

            dir.CreateDirectory(newDir);
        }

        #endregion Methods (1)
    }
}