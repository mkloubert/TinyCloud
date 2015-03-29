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
using System;
using System.Collections.Generic;

namespace MarcelJoachimKloubert.TinyCloud.Handlers
{
    /// <summary>
    /// Handler that sends server information.
    /// </summary>
    [RouteHttpHandler("api/hello")]
    public sealed class HelloHttpHandler : JsonHttpHandlerBase
    {
        #region Properties (1)

        /// <inheriteddoc />
        public override IEnumerable<string> SupportedHttpMethods
        {
            get
            {
                return new string[] { "GET" };
            }
        }

        #endregion Properties (1)

        #region Methods (1)

        /// <inheriteddoc />
        protected override void OnProcessJsonRequest(IHttpRequest request, ref dynamic result)
        {
            dynamic serverInfo = new global::System.Dynamic.ExpandoObject();
            serverInfo.machine = new
            {
                name = Environment.MachineName,
            };
            serverInfo.version = this.GetType().Assembly.GetName().Version.ToString();

            result.data = serverInfo;
        }

        #endregion Methods (1)
    }
}