using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace ServerInfoBackup
{
    /// <summary>
    /// Информация об экземпляре MS SQL Server (неименованном)
    /// </summary>
    public class Server
    {
        /// <summary>
        /// Конфигурация
        /// </summary>
        protected IConfig __conf;
        /// <summary>
        /// Название экземпляра MS SQL Server (неименованном)
        /// </summary>
        protected string __name;
        /// <summary>
        /// Последнее актуальное исключение времени выполнения
        /// </summary>
        protected Exception __exp;

        /// <summary>
        /// Последнее актуальное исключение времени выполнения
        /// </summary>
        public Exception Exp { get { return __exp; } }
        /// <summary>
        /// Конфигурация
        /// </summary>
        public IConfig Conf { get { return __conf; } }
        /// <summary>
        /// T-SQL запрос, по которому получаем список баз данных в конкретном экземпляре MS SQL Server
        /// </summary>
        public string SelectDBList { get; } = @"SELECT [name]
            FROM sys.databases
            WHERE is_read_only=0
            AND [name] NOT IN (N'msdb', N'model', N'tempdb', N'master', N'Distribution', N'SSISDB', N'SRV', N'ReportingService', N'ReportingServiceTempDB')
            ";
        /// <summary>
        /// Название экземпляра MS SQL Server (неименованном)
        /// </summary>
        public string Name { get { return __name; } }
        /// <summary>
        /// Список названий баз данных в данном экземпляре MS SQL Server (неименованном)
        /// </summary>
        public ICollection<string> DBList { get; } = new List<string>();
        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        protected Server() { }
        /// <summary>
        /// Основной конструктор
        /// </summary>
        /// <param name="name">название экземпляра MS SQL Server (неименованном)</param>
        /// <param name="conf">конфигурация</param>
        public Server(string name, IConfig conf)
        {
            this.__name = name;

            this.__exp = Refresh(conf);
        }
        /// <summary>
        /// Обновление всей информации по экземпляру MS SQL Server (неименованном)
        /// </summary>
        /// <param name="conf">конфигурация</param>
        /// <returns>Возвращает исключение (null-все ок)</returns>
        public virtual Exception Refresh(IConfig conf=null)
        {
            Exception exp0 = null;

            this.__conf = ((conf != null) ? conf : this.Conf);

            SqlConnectionStringBuilder sqlctb = new SqlConnectionStringBuilder();
            sqlctb.DataSource = this.Name;
            sqlctb.InitialCatalog = "master";
            sqlctb.IntegratedSecurity = true;
            sqlctb.ApplicationName = "SynchroneDBBackup";

            string connStr = sqlctb.ToString();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = conn;
                comm.CommandText = SelectDBList;

                SqlDataReader reader = null;

                try
                {
                    conn.Open();

                    reader = comm.ExecuteReader();

                    while (reader.Read())
                    {
                        this.DBList.Add(reader[0].ToString());
                    }
                    reader.Close();
                }
                catch (Exception exp)
                {
                    exp0 = exp;
                }
                finally
                {
                    conn.Close();

                    if (reader != null)
                        reader.Close();
                }
            }

            this.__exp = exp0;

            return exp0;
        }
    }
}
