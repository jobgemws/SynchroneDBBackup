using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Linq;
using System.IO;

namespace ServerInfoBackup
{
    /// <summary>
    /// Операции
    /// </summary>
    public class Operations: IOperations
    {
        /// <summary>
        /// Конфигурация
        /// </summary>
        private IConfig __conf;

        /// <summary>
        /// Конфигурация
        /// </summary>
        public IConfig Conf { get { return __conf; } }
        /// <summary>
        /// Экземпляры MS SQL Server (неименованные), откуда узнаем актуальный список баз данных
        /// </summary>
        public ICollection<SourceServer> SourceServerList { get; } = new List<SourceServer>();
        /// <summary>
        /// Экземпляры MS SQL Server (неименованные), в которых ищем резервные копии несуществующих баз данных
        /// </summary>
        public ICollection<TargetServer> TargetServerList { get; } = new List<TargetServer>();
        /// <summary>
        /// Актуальный список баз данных с источников
        /// </summary>
        public ICollection<string> SourceDBList { get; } = new List<string>();

        /// <summary>
        /// Инициализация экземпляра типа операций
        /// </summary>
        /// <param name="conf">конфигурация</param>
        public void Initial(IConfig conf)
        {
            List<string> list = new List<string>();
            this.__conf = conf;

            var sources = conf.SourceServers;
            var targets = conf.TargetServers;

            this.SourceServerList.Clear();
            this.TargetServerList.Clear();

            SourceServer ss;

            foreach (var s in sources)
            {
                ss = new SourceServer(s, conf);
                this.SourceServerList.Add(ss);

                foreach(var db in ss.DBList)
                {
                    list.Add(db);
                }
            }

            list = list.Distinct().ToList();

            SourceDBList.Clear();

            foreach(var db in list)
            {
                SourceDBList.Add(db);
            }

            foreach (var t in targets)
            {
                this.TargetServerList.Add(new TargetServer(t, conf, this));
            }
        }
        /// <summary>
        /// Возвращает файлы резервных копий несуществующих баз данных со сцепкой со своим каталогом в виде FileDirectoryInfo
        /// </summary>
        /// <returns></returns>
        private ICollection<FileDirectoryInfo> GetOldFiles()
        {
            List<FileDirectoryInfo> res = new List<FileDirectoryInfo>();

            foreach(var item in this.TargetServerList)
            {
                res.AddRange(item.OldDBFilesList);
            }

            return res;
        }
        /// <summary>
        /// Признак корректности собранных данных (не было проблем при сборе)
        /// </summary>
        public bool IsCorrectInfo
        {
            get
            {
                var ss_exp_count = SourceServerList.Where(p => p.Exp != null).Count();
                var ts_exp_count = TargetServerList.Where(p => p.Exp != null).Count();

                return ((ss_exp_count + ts_exp_count) == 0);
            }
        }

        /// <summary>
        /// Запуск процесса анализа (поиска файлов резервных копий баз данных, которых нет на источниках)
        /// </summary>
        public void Run()
        {
            if (IsCorrectInfo)
            {
                var oldfiles = GetOldFiles().OrderBy(p => { return p.DirectoryName; });

                using (Stream st_log = new FileStream(this.Conf.FileLog, FileMode.Create, FileAccess.Write))
                {
                    using (TextWriter tw_log = new StreamWriter(st_log, Encoding.UTF8))
                    {
                        foreach (var dr in oldfiles)
                        {
                            RunDirection(dr, tw_log);
                        }

                        tw_log.WriteLine($"Конец {DateTime.Now}");
                    }
                }
            }
            else
            {
                using (Stream st_log = new FileStream(this.Conf.FileLog, FileMode.Create, FileAccess.Write))
                {
                    using (TextWriter tw_log = new StreamWriter(st_log, Encoding.UTF8))
                    {
                        tw_log.WriteLine($"{DateTime.Now} Невозможно начать процесс сравнения, т к были следующие ошибки при сборе информации:");

                        foreach(var item in this.SourceServerList)
                        {
                            if (item.Exp != null)
                            {
                                tw_log.WriteLine($"{DateTime.Now} сервер {item.Name}: {item.Exp.Message}");
                            }
                        }

                        foreach (var item in this.TargetServerList)
                        {
                            if (item.Exp != null)
                            {
                                tw_log.WriteLine($"{DateTime.Now} сервер {item.Name}: {item.Exp.Message}");
                            }
                        }

                        tw_log.WriteLine($"Конец {DateTime.Now}");
                    }
                }
            }
        }
        /// <summary>
        /// Запуск процесса анализа (поиска файлов резервных копий баз данных, которых нет в источниках) для заданного каталога
        /// </summary>
        /// <param name="dr">заданный каталог</param>
        /// <param name="tw_log">журнал лога</param>
        private void RunDirection(FileDirectoryInfo dr, TextWriter tw_log)
        {
            var list = dr.FilesList.OrderBy(p => p);
            tw_log.WriteLine($"{DateTime.Now} Каталог: {dr.DirectoryName}");

            foreach (var item in list)
            {
                if (this.Conf.IsDelFile == "true")
                {
                    try
                    {
                        File.Delete($@"{dr.DirectoryName}\{item}.bak");

                        tw_log.WriteLine($"{DateTime.Now} Файл: {item}.bak успешно удален");
                    }
                    catch (Exception exp)
                    {
                        tw_log.WriteLine($"{DateTime.Now} Файл: {item}.bak не удален, т к возникло исключение: {exp.Message}");
                    }
                }
                else
                {
                    tw_log.WriteLine($"{DateTime.Now} Файл: {item}.bak");
                }
            }
        }
    }
}
