using System;
using System.Collections.Generic;
using System.Text;

namespace ServerInfoBackup
{
    /// <summary>
    /// Интерфейс операций
    /// </summary>
    public interface IOperations
    {
        /// <summary>
        /// Конфигурация
        /// </summary>
        IConfig Conf { get; }
        /// <summary>
        /// Экземпляры MS SQL Server (неименованные), откуда узнаем актуальный список баз данных
        /// </summary>
        ICollection<SourceServer> SourceServerList { get; }
        /// <summary>
        /// Экземпляры MS SQL Server (неименованные), в которых ищем резервные копии несуществующих баз данных
        /// </summary>
        ICollection<TargetServer> TargetServerList { get; }
        /// <summary>
        /// Актуальный список баз данных с источников
        /// </summary>
        ICollection<string> SourceDBList { get; }
    }
}
