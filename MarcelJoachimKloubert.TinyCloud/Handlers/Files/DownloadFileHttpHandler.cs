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

using MarcelJoachimKloubert.TinyCloud.SDK.Handlers.Http;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace MarcelJoachimKloubert.TinyCloud.Handlers.Files
{
    /// <summary>
    /// HTTP handler for uploading files.
    /// </summary>
    [RouteHttpHandler("api/download-file")]
    public sealed class DownloadFileHttpHandler : HttpHandlerBase
    {
        #region Properties (1)

        /// <inheriteddoc />
        public override IEnumerable<string> SupportedHttpMethods
        {
            get
            {
                yield return "POST";
            }
        }

        #endregion Properties (1)

        #region Methods (1)

        /// <inheriteddoc />
        protected override void OnProcessRequest(IHttpRequest request)
        {
            var filePath = NormalizePath((request.GetRequestBodyAsString() ?? string.Empty).Trim());

            var dir = GetDirectory(request, Path.GetDirectoryName(filePath));
            if (dir == null)
            {
                // directory not found

                request.StatusCode = HttpStatusCode.NotFound;
                return;
            }

            var filename = Path.GetFileName(filePath);

            var file = dir.GetFiles()
                          .SingleOrDefault(x => x.Name == filename);
            if (file == null)
            {
                // file not found

                request.StatusCode = HttpStatusCode.NotFound;
                return;
            }

            request.Response.ContentType = "application/octet-stream";

            var size = file.Size;
            if (size.HasValue)
            {
                request.Response.AddHeader("Content-Length",
                                           size.ToString());
            }

            using (var fileStream = file.Download())
            {
                fileStream.CopyTo(request.OutputStream);
            }
        }

        #endregion Methods (1)
    }
}