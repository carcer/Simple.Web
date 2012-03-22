﻿namespace Simple.Web
{
    using System;
    using System.Linq;
    sealed class PublicFileHandler
    {
        private readonly IConfiguration _configuration;
        private readonly IWebEnvironment _environment;

        public PublicFileHandler() : this(null, null)
        {
        }

        public PublicFileHandler(IConfiguration configuration, IWebEnvironment environment)
        {
            _configuration = configuration ?? SimpleWeb.Configuration;
            _environment = environment ?? SimpleWeb.Environment;
        }

        public bool TryHandleAsFile(Uri uri, IResponse response)
        {
            if (_configuration.PublicFolders.Any(folder => uri.AbsolutePath.StartsWith(folder, StringComparison.OrdinalIgnoreCase)))
            {
                var file = _environment.PathUtility.MapPath(uri.AbsolutePath);
                if (_environment.FileUtility.Exists(file))
                {
                    response.StatusCode = 200;
                    response.TransmitFile(file);
                    return true;
                }
            }

            return false;
        }
    }

    public interface IFileUtility
    {
        bool Exists(string path);
    }
}
