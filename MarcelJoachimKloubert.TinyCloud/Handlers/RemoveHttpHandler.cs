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
using MarcelJoachimKloubert.TinyCloud.SDK.IO;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MarcelJoachimKloubert.TinyCloud.Handlers
{
    /// <summary>
    ///
    /// </summary>
    [RouteHttpHandler("api/remove")]
    public sealed class RemoveHttpHandler : JsonHttpHandlerBase
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
            var path = NormalizePath(request.GetRequestBodyAsString());

            IFileSystemObject obj = null;

            var dir = GetDirectory(request, path);
            if (dir != null)
            {
                if (dir.IsRoot == false)
                {
                    obj = dir;
                }
            }
            else
            {
                var dirPath = Path.GetDirectoryName(path);

                dir = GetDirectory(request, dirPath);
                if (dir != null)
                {
                    var filename = Path.GetFileName(path);

                    obj = dir.GetFiles()
                             .FirstOrDefault(x => x.Name == filename);
                }
            }

            if (obj is IDirectory)
            {
                ((IDirectory)obj).Delete();
            }
            else if (obj is IFile)
            {
                ((IFile)obj).Delete();
            }
            else
            {
                result.code = 404;
            }
        }

        #endregion Methods (1)
    }
}