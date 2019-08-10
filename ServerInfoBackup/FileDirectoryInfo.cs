using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ServerInfoBackup
{
    /// <summary>
    /// Информация о каталоге резервных копий баз данных
    /// </summary>
    public class FileDirectoryInfo
    {
        /// <summary>
        /// Каталог резервных копий баз данных
        /// </summary>
        private string __path;

        /// <summary>
        /// Названия файлов резервных копий баз данных без расширений
        /// </summary>
        public ICollection<string> FilesList { get; } = new List<string>();
        /// <summary>
        /// Каталог резервных копий баз данных
        /// </summary>
        public string DirectoryName { get { return __path; } }

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        protected FileDirectoryInfo() { }

        /// <summary>
        /// Основной конструктор
        /// </summary>
        /// <param name="path">каталог резервных копий баз данных</param>
        /// <param name="isrefresh">признак обновления информации содержимого каталога резервных копий баз данных при создании экземпляра типа</param>
        public FileDirectoryInfo(string path, bool isrefresh=true)
        {
            this.__path = path;

            if (isrefresh)
            {
                Refresh();
            }
        }

        /// <summary>
        /// Обновление информации об содержимом каталога резервных копий баз данных
        /// </summary>
        public void Refresh()
        {
            var files = new DirectoryInfo(this.__path).GetFiles("*.bak");

            foreach (var f in files)
                this.FilesList.Add(f.Name.Substring(0, f.Name.Length - f.Extension.Length));
        }
    }
}
