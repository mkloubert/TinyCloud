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

using MarcelJoachimKloubert.TinyCloud.SDK.Handlers.Http;
using MarcelJoachimKloubert.TinyCloud.SDK.Security;
using System;
using System.Collections.Generic;

namespace MarcelJoachimKloubert.TinyCloud.SDK.Collections.Concurrency
{
    /// <summary>
    /// Manages objects for thead safe operations for users.
    /// </summary>
    public sealed class UserSyncCollection
    {
        #region Fields (2)

        private readonly object _SYNC = new object();
        private readonly Dictionary<string, object> _SYNC_OBJECTS = new Dictionary<string, object>();

        #endregion Fields (2)

        #region Properties (3)

        /// <summary>
        /// Returns the sync object for the user of a HTTP request.
        /// </summary>
        /// <param name="request">The HTTP request.</param>
        /// <returns>The sync object.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="request" /> is <see langword="null" />.
        /// </exception>
        public object this[IHttpRequest request]
        {
            get
            {
                if (request == null)
                {
                    throw new ArgumentNullException("request");
                }

                return this[request.User];
            }
        }

        /// <summary>
        /// Returns the sync object for a user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>The sync object.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="user" /> is <see langword="null" />.
        /// </exception>
        public object this[ICloudPrincipal user]
        {
            get
            {
                if (user == null)
                {
                    throw new ArgumentNullException("user");
                }

                return this[user.Identity.Name];
            }
        }

        /// <summary>
        /// Returns the sync object for a username.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns>The sync object.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="username" /> is invalid.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="username" /> is <see langword="null" />.
        /// </exception>
        public object this[string username]
        {
            get
            {
                if (username == null)
                {
                    throw new ArgumentNullException("username");
                }

                username = username.ToLower().Trim();
                if (username == string.Empty)
                {
                    throw new ArgumentException("username");
                }

                lock (this._SYNC)
                {
                    return this.GetSyncObject(username);
                }
            }
        }

        #endregion Properties (3)

        #region Methods (2)

        private object GetSyncObject(string username)
        {
            object result;
            if (this._SYNC_OBJECTS.TryGetValue(username, out result) == false)
            {
                result = new object();
                this._SYNC_OBJECTS[username] = result;
            }

            return result;
        }

        /// <summary>
        /// Invokes an action for the user of a HTTP request thread safe.
        /// </summary>
        /// <typeparam name="T">Type of the state object for <paramref name="action" />.</typeparam>
        /// <param name="request">The HTTP request.</param>
        /// <param name="action">The action to invoke.</param>
        /// <param name="actionState">The object for the first argument of <paramref name="action" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="request" /> and/or <paramref name="action" /> are <see langword="null" />.
        /// </exception>
        public void Invoke<T>(IHttpRequest request, Action<T> action, T actionState)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            lock (this[request])
            {
                action(actionState);
            }
        }

        #endregion Methods (2)
    }
}