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
                result.code = 2;

                return;
            }

            var path = (requestData["path"].AsString() ?? string.Empty).Trim();
            if (path == string.Empty)
            {
                result.code = 3;

                return;
            }

            while (path.EndsWith("/"))
            {
                path = path.Substring(0, path.Length - 1);
            }

            if (path.StartsWith("/") == false)
            {
                path = "/" + path;
            }

            var parts = path.Split('/');
            if (parts.Length < 2)
            {
                result.code = 4;

                return;
            }

            var fullPath = "/" + string.Join("/",
                                             parts.Take(parts.Length - 1));
            var dir = request.User.Directory.FileSystem.GetDirectory(fullPath);
            if (dir == null)
            {
                result.code = 404;

                return;
            }

            dir.CreateDirectory(parts.Last());
        }

        #endregion Methods (1)
    }
}