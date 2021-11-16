using System;
using System.Collections;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Windows.Forms;


namespace STCT.DBInstall
{
    public class CreateDB
    {
        /// <summary>
        /// -1未选择,1云平台对接,0标准版
        /// </summary>
        public static int Init_mode = 0;//

        #region 创建库及在库内执行相关脚本文件

        public static string _sLan = "zh-CN";

        /// <summary>
        /// 创建库及在库内执行相关脚本文件
        /// </summary>
        /// <param name="_Server">服务器</param>
        /// <param name="_user_id">登陆帐号</param>
        /// <param name="_Password">登陆密码</param>
        /// <param name="_Db">要新建的数据库名称</param>
        /// <param name="_SqlFile">脚本文件路径</param>
        /// <returns>为空表示正常，否则表示出错</returns>
        public string execfile(string _Server, String _user_id, string _Password, string _Db, string _SqlFile)
        {
            string s_Error = "";
            try
            {
                string connStr = "data source={0};user id={1};password={2};persist security info=false;packet size=4096";
                connStr = string.Format(connStr, _Server, _user_id, _Password);
                //MessageBox.Show(connStr);
                s_Error = ExecuteSql(connStr, "master", "CREATE DATABASE " + _Db);   //这个数据库名是指你要新建的数据库名称 下同
                System.Diagnostics.Process sqlProcess = new System.Diagnostics.Process();
                sqlProcess.StartInfo.FileName = "osql.exe";
                sqlProcess.StartInfo.Arguments = " -U " + _user_id + " -P " + _Password + " -d " + _Db + " -s " + _Server + " -i " + _SqlFile;
                sqlProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                sqlProcess.Start();
                sqlProcess.WaitForExit();
                sqlProcess.Close();
            }
            catch (Exception ex)
            {
                YEasyInstaller.Comm.Log.i(ex.Message);
            }
            return s_Error;
        }

        private string ExecuteSql(string conn, string DatabaseName, string Sql)
        {
            string s_err = "";
            System.Data.SqlClient.SqlConnection mySqlConnection = new System.Data.SqlClient.SqlConnection(conn);
            System.Data.SqlClient.SqlCommand Command = new System.Data.SqlClient.SqlCommand(Sql, mySqlConnection);
            Command.Connection.Open();
            Command.Connection.ChangeDatabase(DatabaseName);
            try
            {
                Command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                s_err = ex.Message.ToString();
            }
            finally
            {
                Command.Connection.Close();
            }
            return s_err;
        }
        #endregion

        public static string connectionStr = "";
        public void SetConnection(string Server, string Database, string uid, string pwd)
        {
            //this.connectionStr = "server=" + Server + ";database=" + Database + ";uid=" + uid + ";pwd=" + pwd + ";";
            connectionStr = "server=" + Server + ";database=master;uid=" + uid + ";pwd=" + pwd + ";Connect Timeout=36000;";
        }

        private void ExecuteSql(string DataBaseName, string Sql)
        {
            SqlConnection sqlConnection1 = new SqlConnection(connectionStr);
            SqlCommand Command = new SqlCommand(Sql, sqlConnection1);
            Command.CommandTimeout = 0;

            Command.Connection.Open();
            Command.Connection.ChangeDatabase(DataBaseName);
            try
            {
                Command.ExecuteNonQuery();
            }
            finally
            {
                Command.Connection.Close();
            }
        }

        public bool IsConnection(string connString)
        {
            string a = "";
            try
            {
                SqlConnection sqlConnection1 = new SqlConnection(connString);
                SqlCommand Command = new SqlCommand("select 1", sqlConnection1);

                Command.Connection.Open();
                a = Command.ExecuteScalar().ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            finally
            {
            }

            if (a == "1")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsConnection()
        {
            return IsConnection(connectionStr);
        }

        public bool IsDBExist(string sDBName, bool showExMsg = false)
        {
            SqlConnection sqlConnection1 = new SqlConnection(connectionStr);
            SqlCommand Command = new SqlCommand("select 1 from master.dbo.sysdatabases where name='" + sDBName + "'", sqlConnection1);

            string a = "";
            try
            {
                Command.Connection.Open();
                a = Command.ExecuteScalar().ToString();
            }
            catch (Exception ex)
            {
                if (showExMsg)
                    MessageBox.Show(ex.Message);
            }
            finally
            {
                Command.Connection.Close();
            }

            return a == "1";
        }

        public string GetDBVersion(string sDBName)
        {
            SqlConnection sqlConnection1 = new SqlConnection(connectionStr);
            SqlCommand Command = new SqlCommand("select VALUE from [" + sDBName + "].sys.extended_properties WHERE NAME = 'Version' and class_desc='DATABASE'", sqlConnection1);

            string version = "";
            try
            {
                Command.Connection.Open();
                object obj = Command.ExecuteScalar();
                version = obj == null ? "" : obj.ToString();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                Command.Connection.Close();
            }

            return version;
        }

        #region 添加数据库
        public bool AddDBTable(string strDBName, string Path1, string Path2, ref string message)
        {
            message = "";
            try
            {
                ExecuteSql("master", @" 
                    if object_id('killspid','P') is not null  
                    begin
                        drop proc killspid  
                    end
               ");
                ExecuteSql("master", @" 
   
                    create proc killspid (@dbname varchar(20))  
                    as  
                    begin  
                        declare @sql nvarchar(500)  
                        declare @spid int  
                        set @sql='declare getspid cursor for   
                        select spid from sysprocesses where dbid=db_id('''+@dbname+''')'  
                        exec (@sql)  
                        open getspid  
                        fetch next from getspid into @spid  
                        while @@fetch_status<>-1  
                        begin  
                        exec('kill '+@spid)  
                        fetch next from getspid into @spid  
                        end  
                        close getspid  
                        deallocate getspid  
                    end;
               ");

                YEasyInstaller.Comm.Log.i("\n开始创建数据库");
                if (string.IsNullOrEmpty(Path1))
                {
                    string sql = string.Format(@"            
                                exec killspid '{0}';
                                if  exists(select 1 from master.dbo.sysdatabases where name='{0}')            
                                begin
                                   drop database {0}
                                end
                                create database {0} on primary
                                (Name ='{0}_dat',FileName='{1}{0}.mdf' , SIZE = 102400KB , MAXSIZE = UNLIMITED, FILEGROWTH = 20%)
                                log on
                                (NAME = N'{0}_log_dat', FILENAME = '{2}{0}.ldf' , SIZE = 102400KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
                                ", strDBName, Path1, Path2);
                    ExecuteSql("master", sql);
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(" use master");
                    //sb.Append(" go");
                    sb.AppendFormat(" if exists(select * from sysdatabases where name='{0}')", strDBName);
                    sb.Append(" begin");
                    sb.Append(" select '该数据库已存在'");
                    sb.AppendFormat(" drop database {0}  ", strDBName);//      --如果该数据库已经存在，那么就删除它
                    sb.Append(" end");
                    sb.Append(" else");
                    sb.Append(" begin");
                    sb.AppendFormat("  create database {0}", strDBName);
                    sb.Append("  on  primary  ");//      --表示属于 primary 文件组
                    sb.Append("  (");
                    sb.AppendFormat("    name='{0}_data',", strDBName);//        -- 主数据文件的逻辑名称
                    sb.AppendFormat(@"   filename='{1}{0}_data.mdf', ", strDBName, Path1);//   -- 主数据文件的物理名称
                    sb.Append("    size=5mb,  ");//  --主数据文件的初始大小
                    //sb.Append("    maxsize=100mb, ");//    -- 主数据文件增长的最大值
                    sb.Append("    filegrowth=1mb   ");//     --主数据文件的增长率
                    sb.Append(" )");
                    sb.Append(" log on");
                    sb.Append("  (");
                    sb.AppendFormat(@"   name='{0}_log', ", strDBName);//       -- 日志文件的逻辑名称
                    sb.AppendFormat(@"   filename='{1}{0}_log.ldf',", strDBName, Path1);//    -- 日志文件的物理名称
                    sb.Append("    size=1mb, ");//           --日志文件的初始大小
                    sb.Append("    maxsize=2097152mb, ");//       --日志文件增长的最大值
                    sb.Append("    filegrowth=10%  ");//      --日志文件的增长率
                    sb.Append("  )");
                    sb.Append(" end");
                    //                string sql = string.Format(@"            
                    //                exec killspid '{0}';
                    //                if  exists(select 1 from master.dbo.sysdatabases where name='{0}')            
                    //                begin
                    //                   drop database {0}
                    //                end
                    //                create database {0} 
                    //                ", strDBName);
                    ExecuteSql("master", sb.ToString());
                }
                YEasyInstaller.Comm.Log.i("\n开始执行表结构创建");
                ExecuteSqlFile(connectionStr, "/Resources/DbScript.sql", strDBName);
                YEasyInstaller.Comm.Log.i("\n开始数据初始化");
                switch (Init_mode)
                {
                    case 0:
                        ExecuteSqlFile(connectionStr, "/Resources/Initial.sql", strDBName);
                        break;

                    case 1:
                        ExecuteSqlFile(connectionStr, "/Resources/Initial_Cloud.sql", strDBName);
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }

        /// <summary>
        /// 导入sql脚本
        /// </summary>
        /// <param name="sqlConnString">连接数据库字符串</param>
        /// <param name="varFileName">脚本路径</param>
        /// <returns></returns>
        public bool ExecuteSqlFile(string sqlConnString, string varFileName, string DataBaseName)
        {
            //Assembly Asm = Assembly.GetExecutingAssembly();
            //Stream strm = Asm.GetManifestResourceStream(Asm.GetName().Name + "." + varFileName);

            //StreamReader rs = new StreamReader(Application.StartupPath + "\\Sql\\" + varFileName);
            StreamReader rs;
            try
            {
                string path = varFileName;
                if (!varFileName.Contains(":"))
                {
                    path = Application.StartupPath + "\\" + varFileName;
                }
                rs = new StreamReader(path, Encoding.Default);
            }
            catch (Exception)
            {
                rs = new StreamReader(Application.StartupPath + "\\Sql\\" + varFileName);
            }

            ArrayList alSql = new ArrayList();
            string commandText = "";
            string varLine = "";
            while (rs.Peek() > -1)
            {
                varLine = rs.ReadLine();
                if (varLine == "")
                {
                    continue;
                }
                if (varLine.ToUpper().Trim() != "GO")
                {
                    //if (varLine.IndexOf("@database_name=N'STCT_3'") >= 0)//修改（替换）作业脚本里面的数据库名
                    //{
                    //    varLine = varLine.Replace("N'STCT_3'", string.Format("N'{0}'", DataBaseName));
                    //}
                    commandText += varLine;
                    commandText += "\r\n";
                }
                else
                {
                    commandText += "";
                    alSql.Add(commandText);
                    commandText = "";
                }
            }
            //alSql.Add(commandText);
            rs.Close();
            try
            {
                ExecuteCommand(sqlConnString, alSql, DataBaseName);
                return true;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                throw ex;
            }
        }

        private void ExecuteCommand(string sqlConnString, ArrayList varSqlList, string DataBaseName)
        {
            using (SqlConnection conn = new SqlConnection(sqlConnString))
            {
                conn.Open();
                //Don't use Transaction, because some commands cannot execute in one Transaction.
                //SqlTransaction varTrans = conn.BeginTransaction();
                SqlCommand command = new SqlCommand();
                command.Connection = conn;
                command.Connection.ChangeDatabase(DataBaseName);
                //command.Transaction = varTrans;
                //StringBuilder temp = new StringBuilder();
                //string sqlTemp = string.Empty;
                try
                {
                    foreach (string varcommandText in varSqlList)
                    {
                        command.CommandText = varcommandText;
                        command.ExecuteNonQuery();
                        //temp.Append(varcommandText);
                    }
                    //varTrans.Commit();
                }
                catch (Exception ex)
                {
                    //varTrans.Rollback();
                    YEasyInstaller.Comm.Log.i(ex.Message + "--" + command.CommandText);
                    throw ex;
                }
                finally
                {
                    conn.Close();
                }
            }
        }
        #endregion

        public void BackupDatabase(string dbName, string path, string fileName)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendFormat(@"BACKUP DATABASE {0} TO  DISK = N'{1}\{2}' WITH NOFORMAT,", dbName, path, fileName);
                stringBuilder.AppendFormat(" NOINIT, NAME = '{0}-完整 数据库 备份', SKIP, NOREWIND, NOUNLOAD,  STATS = 10;", dbName);
                ExecuteSql("master", stringBuilder.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
