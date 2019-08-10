using System;
using System.Collections.Generic;
using System.Text;

namespace ServerInfoBackup
{
    /// <summary>
    /// Информация об экземпляре MS SQL Server (неименованном), который является источников
    /// </summary>
    public class SourceServer : Server
    {
        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        protected SourceServer() : base() { }

        /// <summary>
        /// Основной конструктор
        /// </summary>
        /// <param name="name">название экземпляра MS SQL Server (неименованном)</param>
        /// <param name="conf">конфигурация</param>
        public SourceServer(string Name, IConfig conf) : base(Name, conf)
        {
        }
    }
}
