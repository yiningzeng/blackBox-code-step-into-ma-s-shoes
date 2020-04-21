using log4net;
using System;
using System.Collections.Generic;
using System.Data;
//using System.Data.SQLite;
using System.IO;
using System.Reflection;

namespace DeviceServer
{
    /// <summary>
    /// SqLite操作基类
    /// </summary>
    //public class SqLiteHelperDao : IDisposable
    //{
    //    public static readonly log4net.ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    //    public string FileName { get; private set; }
    //    private string _connString = null;
    //    /// <summary>
    //    /// 连接字符串
    //    /// </summary>
    //    public string ConnString { get { return _connString; } }

    //    /// <summary>
    //    /// 初始化连接字符串
    //    /// </summary>
    //    /// <param name="fileName"></param>
    //    public void InitFileName(string fileName)
    //    {
    //        this.FileName = fileName;
    //        string path = Path.GetDirectoryName(fileName);
    //        if (!Directory.Exists(path))
    //        {
    //            Directory.CreateDirectory(path);
    //        }
    //        _connString = GetConnString(fileName);
    //    }

    //    /// <summary>
    //    /// 根据指定资源sql语句创建数据库
    //    /// </summary>
    //    /// <param name="sql"></param>
    //    /// <returns></returns>
    //    public bool InitCreateSql(string sql)
    //    {
    //        if (File.Exists(this.FileName)) return false;

    //        List<string> list = new List<string>();
    //        string[] sqlList = sql.Split(new string[] { "GO" }, StringSplitOptions.RemoveEmptyEntries);
    //        for (int i = 0; i < sqlList.Length; i++)
    //        {
    //            string cmdLine = sqlList[i].Trim();
    //            if (string.IsNullOrEmpty(cmdLine)) continue;
    //            list.Add(cmdLine);
    //        }
    //        return TransExecuteNonQuery(list);
    //    }

    //    /// <summary>
    //    /// 获取连接字符串
    //    /// </summary>
    //    /// <param name="fileName"></param>
    //    /// <returns></returns>
    //    public string GetConnString(string fileName)
    //    {
    //        SQLiteConnectionStringBuilder sb = new SQLiteConnectionStringBuilder();
    //        sb.DataSource = fileName;
    //        sb.SyncMode = SynchronizationModes.Off;
    //        sb.PageSize = 4096;
    //        sb.CacheSize = 70 * 1024;
    //        return sb.ConnectionString;
    //    }

    //    /// <summary>
    //    /// 对连接执行 Transact-SQL 语句并返回受影响的行数。
    //    /// </summary>
    //    /// <param name="sql"></param>
    //    /// <returns></returns>
    //    public int ExecuteNonQuery(string sql)
    //    {
    //        try
    //        {
    //            using (SQLiteConnection conn = new SQLiteConnection(_connString))
    //            {
    //                conn.Open();
    //                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
    //                {
    //                    cmd.CommandType = System.Data.CommandType.Text;
    //                    return cmd.ExecuteNonQuery();
    //                }
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            log.Error(string.Format("ExecuteNonQuery.Error[{0}]\r\n{1}", sql, ex));
    //            throw;
    //        }
    //    }

    //    /// <summary>
    //    /// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略其他列或行。
    //    /// </summary>
    //    /// <param name="sql"></param>
    //    /// <returns></returns>
    //    public object ExecuteScalar(string sql)
    //    {
    //        try
    //        {
    //            using (SQLiteConnection conn = new SQLiteConnection(_connString))
    //            {
    //                conn.Open();
    //                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
    //                {
    //                    cmd.CommandType = System.Data.CommandType.Text;
    //                    return cmd.ExecuteScalar();
    //                }
    //            }
    //        }
    //        catch (Exception)
    //        {
    //            //log.Error(string.Format("ExecuteScalar.Error[{0}]\r\n{1}", sql, ex));
    //            throw;
    //        }
    //    }

    //    /// <summary>
    //    /// 查询结构返回一个数据集
    //    /// </summary>
    //    /// <param name="connectionString"></param>
    //    /// <param name="cmdText"></param>
    //    /// <param name="tableName"></param>
    //    /// <param name="commandParameters"></param>
    //    /// <returns></returns>
    //    public DataSet ExecuteDataSet(string cmdText)
    //    {
    //        DataSet ds = new DataSet();

    //        using (SQLiteConnection conn = new SQLiteConnection(this._connString))
    //        {
    //            SQLiteCommand cmd = new SQLiteCommand();
    //            using (SQLiteDataAdapter da = new SQLiteDataAdapter())
    //            {
    //                PrepareCommand(cmd, conn, cmdText);
    //                da.SelectCommand = cmd;
    //                da.Fill(ds, "tableName");
    //                cmd.Parameters.Clear();
    //                conn.Close();
    //            }
    //        }
    //        return ds;
    //    }

    //    private static void PrepareCommand(SQLiteCommand cmd, SQLiteConnection conn, string cmdText)
    //    {
    //        if (conn.State != ConnectionState.Open)
    //            conn.Open();

    //        cmd.Connection = conn;
    //        cmd.CommandText = cmdText;
    //    }

    //    /// <summary>
    //    /// 打开一个连接
    //    /// </summary>
    //    /// <returns></returns>
    //    public SQLiteCommand StartCommand()
    //    {
    //        return StartCommand(null);
    //    }
    //    /// <summary>
    //    /// 打开一个连接
    //    /// 返回SQLiteCommand实例
    //    /// </summary>
    //    /// <param name="sql"></param>
    //    /// <returns></returns>
    //    public SQLiteCommand StartCommand(string sql)
    //    {
    //        try
    //        {
    //            SQLiteConnection con = new SQLiteConnection(_connString);
    //            con.Open();
    //            SQLiteCommand cmd = new SQLiteCommand(sql, con);
    //            return cmd;
    //        }
    //        catch (Exception ex)
    //        {
    //            log.Error(string.Format("StartCommand.Error[{0}]\r\n{1}", sql, ex));
    //            throw;
    //        }
    //    }
    //    /// <summary>
    //    /// 关闭SQLiteCommand实例的连接，并释放
    //    /// </summary>
    //    /// <param name="cmd"></param>
    //    public void EndCommand(SQLiteCommand cmd)
    //    {
    //        if (cmd == null || cmd.Connection == null) return;
    //        try
    //        {
    //            if (cmd.Connection.State == ConnectionState.Open)
    //            {
    //                cmd.Connection.Close();
    //                cmd.Connection.Dispose();
    //                cmd.Dispose();
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            log.Error(string.Format("EndCommand.Error[{0}]\r\n{1}", cmd.CommandText, ex));
    //            throw;
    //        }
    //    }

    //    /// <summary>
    //    /// 事务处理
    //    /// 打开一个连接
    //    /// 返回SQLiteCommand实例
    //    /// </summary>
    //    /// <returns></returns>
    //    public SQLiteCommand TransStartCommand()
    //    {
    //        try
    //        {
    //            SQLiteConnection con = new SQLiteConnection(_connString);
    //            con.Open();
    //            SQLiteTransaction trans = con.BeginTransaction();
    //            SQLiteCommand cmd = new SQLiteCommand();
    //            cmd.Connection = con;
    //            cmd.Transaction = trans;

    //            return cmd;
    //        }
    //        catch (Exception ex)
    //        {
    //            log.Error(string.Format("TransStartCommand.Error\r\n{0}", ex));
    //            throw;
    //        }
    //    }
    //    /// <summary>
    //    /// 事务处理
    //    /// 关闭SQLiteCommand实例的连接，并释放
    //    /// </summary>
    //    /// <param name="cmd"></param>
    //    public void TransEndCommand(SQLiteCommand cmd)
    //    {
    //        if (cmd == null || cmd.Connection == null || cmd.Transaction == null) return;
    //        try
    //        {
    //            cmd.Transaction.Commit();
    //        }
    //        catch (Exception ex)
    //        {
    //            TransEndError(cmd);
    //            log.Error(string.Format("TransEndCommand.Execute.Error[{0}]\r\n{1}", cmd.CommandText, ex));
    //            throw;
    //        }
    //    }
    //    /// <summary>
    //    /// 事务处理异常回退
    //    /// 关闭SQLiteCommand实例的连接，并释放
    //    /// </summary>
    //    /// <param name="cmd"></param>
    //    public void TransEndError(SQLiteCommand cmd)
    //    {
    //        if (cmd == null || cmd.Connection == null || cmd.Transaction == null) return;
    //        try
    //        {
    //            cmd.Transaction.Rollback();
    //            if (cmd.Connection.State == ConnectionState.Open)
    //            {
    //                cmd.Connection.Close();
    //                cmd.Connection.Dispose();
    //            }
    //            cmd.Dispose();
    //        }
    //        catch (Exception ex)
    //        {
    //            log.Error(string.Format("TransEndError.Error\r\n{0}", ex));
    //            throw;
    //        }
    //    }

    //    /// <summary>
    //    /// 使用事务处理  Transact-SQL 语句列表
    //    /// </summary>
    //    /// <param name="sqlList"></param>
    //    /// <returns></returns>
    //    public bool TransExecuteNonQuery(List<string> sqlList)
    //    {
    //        bool error = true;
    //        try
    //        {
    //            using (SQLiteConnection con = new SQLiteConnection(_connString))
    //            {
    //                con.Open();
    //                SQLiteTransaction trans = con.BeginTransaction();
    //                using (SQLiteCommand cmd = new SQLiteCommand())
    //                {
    //                    cmd.Connection = con;
    //                    cmd.Transaction = trans;
    //                    try
    //                    {
    //                        for (int i = 0; i < sqlList.Count; i++)
    //                        {
    //                            cmd.CommandText = sqlList[i];
    //                            cmd.ExecuteNonQuery();
    //                        }
    //                        trans.Commit();
    //                    }
    //                    catch (Exception ex)
    //                    {
    //                        trans.Rollback();
    //                        error = false;
    //                        log.Error(string.Format("TransExecuteNonQuery.Execute.Error[{0}]\r\n{1}", cmd.CommandText, ex));
    //                        throw;
    //                    }
    //                    return true;
    //                }
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            if (error) log.Error(string.Format("TransExecuteNonQuery.Other.Error\r\n{0}", ex));
    //            throw;
    //        }
    //    }

    //    #region Dispose
    //    private bool disposed = false;

    //    /// <summary>
    //    /// 释放
    //    /// </summary>
    //    public void Dispose()
    //    {
    //        Dispose(true);
    //        GC.SuppressFinalize(this);
    //    }

    //    private void Dispose(bool disposing)
    //    {
    //        if (!disposed)
    //        {
    //            if (disposing)
    //            {
    //            }
    //        }
    //        disposed = true;
    //    }

    //    /// <summary>
    //    /// 析构
    //    /// </summary>
    //    ~SqLiteHelperDao()
    //    {
    //        Dispose(false);
    //    }
    //    #endregion
    //}
}
