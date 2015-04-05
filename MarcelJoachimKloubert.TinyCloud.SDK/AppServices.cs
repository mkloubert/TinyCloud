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

using MarcelJoachimKloubert.TinyCloud.SDK.Extensions;
using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web;

namespace MarcelJoachimKloubert.TinyCloud.SDK
{
    /// <summary>
    /// Application globals.
    /// </summary>
    public static class AppServices
    {
        #region Fields (2)

        /// <summary>
        /// Stores the function that provides the app configuration instance.
        /// </summary>
        public static Func<Configuration> ConfigProvider;

        /// <summary>
        /// Stores the default value for password iterations.
        /// </summary>
        public const int DEFAULT_PASSWORD_ITERATIONS = 1000;

        #endregion Fields (2)

        #region Properties (2)

        /// <summary>
        /// Gets the default charset / encoding to use.
        /// </summary>
        public static Encoding Charset
        {
            get { return Encoding.UTF8; }
        }

        /// <summary>
        /// Gets the culture that is used to convert data.
        /// </summary>
        public static CultureInfo DataCulture
        {
            get { return CultureInfo.InvariantCulture; }
        }

        #endregion Properties (2)

        #region Methods (15)

        /// <summary>
        /// Returns a value from the app settings.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="name">The name of the setting.</param>
        /// <param name="beforeReturn">
        /// Optional logic to invoke BEFORE setting value is converted.
        /// </param>
        /// <returns>The value.</returns>
        public static T GetAppSettingValue<T>(string name,
                                              Func<string, string, object> beforeConvert = null)
        {
            var targetType = typeof(T);

            var value = GetAppSettingValue(name);

            object objToConvert;
            if (beforeConvert != null)
            {
                objToConvert = beforeConvert(name, value);
            }
            else
            {
                objToConvert = value;
            }

            if (objToConvert == null)
            {
                var nullableType = Nullable.GetUnderlyingType(targetType);
                if (nullableType != null)
                {
                    // nullable struct
                    return default(T);
                }
                else
                {
                    // create default instance of target type
                    return (T)(targetType.IsValueType ? Activator.CreateInstance(targetType)
                                                      : null);
                }
            }

            return (T)Convert.ChangeType(objToConvert,
                                         targetType,
                                         DataCulture);
        }

        /// <summary>
        /// Returns a value from the app settings.
        /// </summary>
        /// <param name="name">The name of the setting.</param>
        /// <param name="beforeReturn">
        /// Optional logic to invoke BEFORE setting value is returned.
        /// </param>
        /// <returns>The value.</returns>
        public static string GetAppSettingValue(string name,
                                                Func<string, string, object> beforeReturn = null)
        {
            var value = GetConfig().AppSettings.Settings[name].Value;
            if (string.IsNullOrEmpty(value))
            {
                value = string.Empty;
            }

            object result;
            if (beforeReturn != null)
            {
                result = beforeReturn(name, value);
            }
            else
            {
                result = value;
            }

            return result.AsString();
        }

        /// <summary>
        /// Returns the configuration of the application.
        /// </summary>
        /// <returns>The app config.</returns>
        public static Configuration GetConfig()
        {
            return ConfigProvider();
        }

        /// <summary>
        /// Returns the data directory.
        /// </summary>
        /// <returns>The data directory.</returns>
        public static DirectoryInfo GetDataDirectory()
        {
            var path = GetAppSettingValue("DataDirectory");

            return new DirectoryInfo(GetFullPath(path));
        }

        /// <summary>
        /// Gets the system password.
        /// </summary>
        /// <returns>The password.</returns>
        public static byte[] GetPassword()
        {
            if (IsMyAssembly(Assembly.GetCallingAssembly()) == false)
            {
                return null;
            }

            string pwd = null;
            try
            {
                pwd = GetAppSettingValue("Password");

                return pwd == null ? null : Charset.GetBytes(pwd);
            }
            finally
            {
                pwd = null;
            }
        }

        /// <summary>
        /// Gets the number of iterations for system passwords.
        /// </summary>
        /// <returns>The number of iterations.</returns>
        public static int GetPasswordIterations()
        {
            if (IsMyAssembly(Assembly.GetCallingAssembly()) == false)
            {
                return int.MinValue;
            }

            string iterations = null;
            try
            {
                iterations = GetAppSettingValue("PasswordIterations");
                if (string.IsNullOrWhiteSpace(iterations) == false)
                {
                    return int.Parse(iterations.Trim(),
                                     DataCulture);
                }
            }
            finally
            {
                iterations = null;
            }

            return DEFAULT_PASSWORD_ITERATIONS;
        }

        /// <summary>
        /// Gets the salt for system passwords.
        /// </summary>
        /// <returns>The salt.</returns>
        public static byte[] GetPasswordSalt()
        {
            if (IsMyAssembly(Assembly.GetCallingAssembly()) == false)
            {
                return null;
            }

            string salt = null;
            try
            {
                salt = GetAppSettingValue("PasswordSalt");

                return salt == null ? null : Charset.GetBytes(salt);
            }
            finally
            {
                salt = null;
            }
        }

        /// <summary>
        /// Returns the normalized, full version of a path.
        /// </summary>
        /// <param name="path">The path to normalize.</param>
        /// <returns>The normalized path.</returns>
        public static string GetFullPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                path = GetRootDirectory().FullName;
            }

            if (Path.IsPathRooted(path) == false)
            {
                path = Path.GetFullPath(Path.Combine(GetRootDirectory().FullName,
                                                     path));
            }

            return Path.GetFullPath(path);
        }

        /// <summary>
        /// Returns the root directory based on the current HTTP context.
        /// </summary>
        /// <returns>The root directory.</returns>
        public static DirectoryInfo GetRootDirectory()
        {
            return GetRootDirectory(HttpContext.Current);
        }

        /// <summary>
        /// Returns the root directory based on a HTTP context.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <returns>The root directory.</returns>
        public static DirectoryInfo GetRootDirectory(HttpContext context)
        {
            if (context == null)
            {
                return null;
            }

            return GetRootDirectory(context.Server);
        }

        /// <summary>
        /// Returns the root directory based on a <see cref="HttpServerUtility" /> instance.
        /// </summary>
        /// <param name="server">The server utility instance.</param>
        /// <returns>The root directory.</returns>
        public static DirectoryInfo GetRootDirectory(HttpServerUtility server)
        {
            if (server == null)
            {
                return null;
            }

            var path = server.MapPath("~");
            if (string.IsNullOrWhiteSpace(path))
            {
                path = Environment.CurrentDirectory;
            }

            return new DirectoryInfo(path);
        }

        /// <summary>
        /// Returns the <see cref="StringComparer" /> instance that compares and sorts general strings.
        /// </summary>
        /// <param name="ignoreCase">Ignore case or not.</param>
        /// <returns>The string comparer and sorter.</returns>
        public static StringComparer GetStringComparer(bool ignoreCase = true)
        {
            return ignoreCase ? StringComparer.InvariantCultureIgnoreCase
                              : StringComparer.InvariantCulture;
        }

        /// <summary>
        /// Returns the temp directory.
        /// </summary>
        /// <returns>The temp directory.</returns>
        public static DirectoryInfo GetTempDirectory()
        {
            var path = GetAppSettingValue("TempDirectory");

            return new DirectoryInfo(GetFullPath(path));
        }

        /// <summary>
        /// Gets the number of iterations for user passwords.
        /// </summary>
        /// <returns>The number of iterations.</returns>
        public static int GetUserPasswordIterations()
        {
            if (IsMyAssembly(Assembly.GetCallingAssembly()) == false)
            {
                return int.MinValue;
            }

            string iterations = null;
            try
            {
                iterations = GetAppSettingValue("UserPasswordIterations");
                if (string.IsNullOrWhiteSpace(iterations) == false)
                {
                    return int.Parse(iterations.Trim(),
                                     DataCulture);
                }
            }
            finally
            {
                iterations = null;
            }

            return DEFAULT_PASSWORD_ITERATIONS;
        }

        /// <summary>
        /// Gets the salt for user passwords.
        /// </summary>
        /// <returns>The salt.</returns>
        public static byte[] GetUserPasswordSalt()
        {
            if (IsMyAssembly(Assembly.GetCallingAssembly()) == false)
            {
                return null;
            }

            string salt = null;
            try
            {
                salt = GetAppSettingValue("UserPasswordSalt");

                return salt == null ? null : Charset.GetBytes(salt);
            }
            finally
            {
                salt = null;
            }
        }

        private static bool IsMyAssembly(Assembly asm)
        {
            if (asm == null)
            {
                return false;
            }

            return asm.Equals(Assembly.GetExecutingAssembly());
        }

        #endregion Methods (15)
    }
}