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
using System;
using System.Configuration;
using System.Security.Cryptography;
using System.Web;
using System.Web.Configuration;
using System.Web.Routing;

namespace MarcelJoachimKloubert.TinyCloud
{
    /// <summary>
    /// The global application class.
    /// </summary>
    public class Global : HttpApplication
    {
        #region Methods (9)

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
        }

        protected void Application_End(object sender, EventArgs e)
        {
        }

        protected void Application_Error(object sender, EventArgs e)
        {
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            AppServices.ConfigProvider = GetWebConfig;

            InitRoutes(RouteTable.Routes);
        }

        private static Configuration GetWebConfig()
        {
            return WebConfigurationManager.OpenWebConfiguration("~/");
        }

        private static void InitRoutes(RouteCollection routes)
        {
            routes.Add(new Route("api/files",
                                 new global::MarcelJoachimKloubert.TinyCloud.Handlers.Files.FileHttpHandler()));

            routes.Add(new Route("api/hello",
                                new global::MarcelJoachimKloubert.TinyCloud.Handlers.HelloHttpHandler()));
        }

        protected void Session_End(object sender, EventArgs e)
        {
        }

        protected void Session_Start(object sender, EventArgs e)
        {
        }

        #endregion Methods (9)
    }
}