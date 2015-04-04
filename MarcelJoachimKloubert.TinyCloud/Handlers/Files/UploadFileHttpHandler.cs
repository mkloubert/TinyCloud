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

namespace MarcelJoachimKloubert.TinyCloud.Handlers.Files
{
    /// <summary>
    /// HTTP handler for uploading files.
    /// </summary>
    [RouteHttpHandler("api/upload-file")]
    public sealed class UploadFileHttpHandler : JsonHttpHandlerBase
    {
        #region Properties (1)

        /// <inheriteddoc />
        public override IEnumerable<string> SupportedHttpMethods
        {
            get { return new string[] { "POST" }; }
        }

        #endregion Properties (1)

        #region Methods (1)

        /// <inheriteddoc />
        protected override void OnProcessJsonRequest(IHttpRequest request, ref dynamic result)
        {
            var contentLength = request.Request.Headers["Content-Length"];
            if (string.IsNullOrWhiteSpace(contentLength))
            {
                result.code = 1;

                return;
            }

            long length;
            if (long.TryParse(contentLength.Trim(), out length) == false)
            {
                result.code = 2;
                return;
            }

            if (length < 0)
            {
                result.code = 3;

                return;
            }

            var filename = request.GetAppRequestHeader("Filename");
            if (string.IsNullOrWhiteSpace(filename))
            {
                result.code = 4;

                return;
            }

            var fullPath = filename.Trim();

            var dir = request.User.Directory.FileSystem.GetDirectory(Path.GetDirectoryName(fullPath));
            if (dir == null)
            {
                result.code = 5;

                return;
            }

            filename = Path.GetFileName(fullPath);

            dir.UploadFile(filename,
                           request.GetBufferlessInputStream(),
                           length);
        }

        #endregion Methods (1)
    }
}