﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using TuesPechkin.Properties;
using SysPath = System.IO.Path;

namespace TuesPechkin
{
    [Serializable]
    public class WinEmbeddedDeployment : EmbeddedDeployment
    {
        public WinEmbeddedDeployment(IDeployment physical) : base(physical) { }

        public override string Path
        {
            get
            {
                return System.IO.Path.Combine(
                    base.Path,
                    GetType().Assembly.GetName().Version.ToString());
            }
        }

        protected override IEnumerable<KeyValuePair<string, Stream>> GetContents()
        {
            return new[]
            { 
                new KeyValuePair<string, Stream>(
                    key: WkhtmltoxBindings.DLLNAME,
                    value: new GZipStream(
                        new MemoryStream(Resources.wkhtmltox_64_dll), 
                        CompressionMode.Decompress))
            };
        }
    }
}
