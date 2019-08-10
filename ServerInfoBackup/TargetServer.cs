using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerInfoBackup
{
    /// <summary>
    /// Информация об экземпляре MS SQL Server (неименованном), который является приемником
    /// </summary>
    public class TargetServer : Server
    { 
        /// <summary>
        /// Список информации о каталогах резервных копий баз данных с названиями этих файлов без расширений
        /// </summary>
        public ICollection<FileDirectoryInfo> DBFilesList { get; } = new List<FileDirectoryInfo>();
        /// <summary>
        /// Список информации о каталогах резервных копий баз данных, которых нет в источниках, с названиями этих файлов без расширений
        /// </summary>
        public ICollection<FileDirectoryInfo> OldDBFilesList { get; } = new List<FileDirectoryInfo>();
        /// <summary>
        /// Список назаний баз данных, которых нет в источниках
        /// </summary>
        public ICollection<string> OldDBList { get; } = new List<string>();
        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        protected TargetServer() : base() { }
        /// <summary>
        /// Основной конструктор
        /// </summary>
        /// <param name="name">название экземпляра MS SQL Server (неименованном)</param>
        /// <param name="conf">конфигурация</param>
        /// <param name="ops">операции</param>
        public TargetServer(string name, IConfig conf, IOperations ops) : base(name, conf)
        {
            this.__exp = Refresh(ops, conf);
        }
        /// <summary>
        /// Обновление всей информации по экземпляру MS SQL Server (неименованном)
        /// </summary>
        /// <param name="conf">конфигурация</param>
        /// <returns>Возвращает исключение (null-все ок)</returns>
        public override Exception Refresh(IConfig conf = null)
        {
            Exception exp0 = null;

            exp0 = base.Refresh(conf);

            this.DBFilesList.Clear();

            foreach (var dr in this.Conf.Directories)
            {
                this.DBFilesList.Add(new FileDirectoryInfo(@"\\" + this.Name + @"\" + dr));
            }

            this.__exp = exp0;

            return exp0;
        }
        /// <summary>
        /// Обновление всей информации по экземпляру MS SQL Server (неименованном)
        /// </summary>
        /// <param name="ops">операции</param>
        /// <param name="conf">конфигурация</param>
        /// <returns>Возвращает исключение (null-все ок)</returns>
        public Exception Refresh(IOperations ops, IConfig conf = null)
        {
            Exception exp0 = null;

            exp0 = Refresh(conf);

            if (ops != null)
            {
                this.OldDBList.Clear();

                var resdb = this.DBList.Except(ops.SourceDBList);

                foreach(var db in resdb)
                {
                    this.OldDBList.Add(db);
                }

                this.OldDBFilesList.Clear();

                foreach (var fdi in this.DBFilesList)
                {
                    this.OldDBFilesList.Add(ExceptFiles(fdi, ops));
                }
            }

            this.__exp = exp0;

            return exp0;
        }
        /// <summary>
        /// Возвращает файлы резервных копий баз данных, которых нет в истониках, в сцепке со своими каталогами в заданном каталоге
        /// </summary>
        /// <param name="fdi">заданный каталог</param>
        /// <param name="ops">операции</param>
        /// <returns>Лишние файлы</returns>
        private FileDirectoryInfo ExceptFiles(FileDirectoryInfo fdi, IOperations ops)
        {
            var resdbfiles = fdi.FilesList.Except(ops.SourceDBList);

            FileDirectoryInfo nfdi = new FileDirectoryInfo(path: fdi.DirectoryName, isrefresh: false);

            foreach(var f in resdbfiles)
            {
                nfdi.FilesList.Add(f);
            }

            return nfdi;
        }
    }
}
