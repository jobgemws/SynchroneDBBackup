using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ServerInfoBackup
{
    public sealed class Config: IConfig
    {
        private string __filelog;
        private string __isdelfile;

        public ICollection<string> Directories { get; } = new List<string>();
        public ICollection<string> SourceServers { get; } = new List<string>();
        public ICollection<string> TargetServers { get; } = new List<string>();

        public string FileLog { get { return __filelog; } }
        public string IsDelFile { get { return __isdelfile; } }

        private Config() { }

        public Config(string conf)
        { 
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile(conf, optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            this.__filelog = configuration.GetSection("Root:FileLog").Value;
            this.__isdelfile = configuration.GetSection("Root:IsDelFile").Value;

            var Directories = configuration.GetSection("Root:Directories").GetChildren();

            foreach (var dr in Directories)
            {
                this.Directories.Add(dr.Value);
            }

            var SourceServers = configuration.GetSection("Root:SourceServers").GetChildren();

            foreach (var ss in SourceServers)
            {
                this.SourceServers.Add(ss.Value);
            }

            var TargetServers = configuration.GetSection("Root:TargetServers").GetChildren();

            foreach (var ts in TargetServers)
            {
                this.TargetServers.Add(ts.Value);
            }
        }
    }
}
