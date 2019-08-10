using System;
using System.Collections.Generic;
using System.Text;

namespace ServerInfoBackup
{
    /// <summary>
    /// Интерфейс конфигурации
    /// </summary>
    public interface IConfig
    {
        /// <summary>
        /// Каталоги, в которых ищем резервные копии несуществующих баз данных
        /// </summary>
        ICollection<string> Directories { get; }
        /// <summary>
        /// Экземпляры MS SQL Server (неименованные), откуда узнаем актуальный список баз данных
        /// </summary>
        ICollection<string> SourceServers { get; }
        /// <summary>
        /// Экземпляры MS SQL Server (неименованные), в которых ищем резервные копии несуществующих баз данных
        /// </summary>
        ICollection<string> TargetServers { get; }
        /// <summary>
        /// Название файла лога (с расширением)
        /// </summary>
        string FileLog { get; }
        /// <summary>
        /// Признак удаления найденных резервных копий несуществующих баз данных
        /// </summary>
        string IsDelFile { get; }
    }
}
