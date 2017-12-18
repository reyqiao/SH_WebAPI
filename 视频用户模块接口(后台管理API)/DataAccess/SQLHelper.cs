using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Collections;



namespace DataAccess
{

    /// <summary>
    /// The SqlHelper class is intended to encapsulate high performance, 
    /// scalable best practices for common uses of SqlClient.
    /// </summary>
    public abstract class SqlHelper
    {

        private static readonly string ConnectionString = ConfigurationManager.ConnectionStrings["DB_Live"].ConnectionString;

      

        private static int commandTimeout = 30;

        /// <summary>
        /// ���ʱʱ��
        /// </summary>
        public static int CommandTimeout
        {
            get { return commandTimeout; }
            set { commandTimeout = value; }
        }


        // Hashtable to store cached parameters
        private static Hashtable parmCache = Hashtable.Synchronized(new Hashtable());

        #region ExecuteNonQuery

        /// <summary>
        /// Executes the non query.
        /// </summary>
        /// <param name="cmdText">The CMD text.</param>
        /// <param name="commandParameters">The command parameters.</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string cmdText, params SqlParameter[] commandParameters)
        {
            return ExecuteNonQuery(CommandType.Text, cmdText, commandParameters);
        }

        /// <summary>
        /// Executes the non query.
        /// </summary>
        /// <param name="cmdType">Type of the CMD.</param>
        /// <param name="cmdText">The CMD text.</param>
        /// <param name="commandParameters">The command parameters.</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                cmd.CommandTimeout = CommandTimeout;

                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);

                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
        }

        /// <summary>
        /// Executes the non query.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="cmdType">Type of the CMD.</param>
        /// <param name="cmdText">The CMD text.</param>
        /// <param name="commandParameters">The command parameters.</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {

            SqlCommand cmd = new SqlCommand();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                cmd.CommandTimeout = CommandTimeout;
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
        }

        /// <summary>
        /// Executes the non query.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="cmdType">Type of the CMD.</param>
        /// <param name="cmdText">The CMD text.</param>
        /// <param name="commandParameters">The command parameters.</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(SqlConnection connection, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {

            SqlCommand cmd = new SqlCommand();
            cmd.CommandTimeout = CommandTimeout;
            PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }

        /// <summary>
        /// Executes the non query.
        /// </summary>
        /// <param name="trans">The trans.</param>
        /// <param name="cmdType">Type of the CMD.</param>
        /// <param name="cmdText">The CMD text.</param>
        /// <param name="commandParameters">The command parameters.</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(SqlTransaction trans, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandTimeout = CommandTimeout;
            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, commandParameters);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }

        #endregion


        #region ExecuteReader

        /// <summary>
        /// Executes the reader.
        /// </summary>
        /// <param name="cmdType">Type of the CMD.</param>
        /// <param name="cmdText">The CMD text.</param>
        /// <param name="commandParameters">The command parameters.</param>
        /// <returns></returns>
        public static SqlDataReader ExecuteReader(CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandTimeout = CommandTimeout;
            SqlConnection conn = new SqlConnection(ConnectionString);

            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);

                SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return rdr;

            }
            catch (Exception exc)
            {
                conn.Close();
                throw exc;
            }
        }

        /// <summary>
        /// Executes the reader.
        /// </summary>
        /// <param name="cmdText">The CMD text.</param>
        /// <param name="commandParameters">The command parameters.</param>
        /// <returns></returns>
        public static SqlDataReader ExecuteReader(string cmdText, params SqlParameter[] commandParameters)
        {
            return ExecuteReader(CommandType.Text, cmdText, commandParameters);
        }

        /// <summary>
        /// Executes the reader.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="cmdType">Type of the CMD.</param>
        /// <param name="cmdText">The CMD text.</param>
        /// <param name="commandParameters">The command parameters.</param>
        /// <returns></returns>
        public static SqlDataReader ExecuteReader(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandTimeout = CommandTimeout;
            SqlConnection conn = new SqlConnection(connectionString);
            // we use a try/catch here because if the method throws an exception we want to 
            // close the connection throw code, because no datareader will exist, hence the 
            // commandBehaviour.CloseConnection will not work
            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return rdr;
            }
            catch
            {
                conn.Close();
                throw;
            }
        }

        /// <summary>
        /// Executes the reader.
        /// </summary>
        /// <param name="conn">���ݿ�����</param>
        /// <param name="cmdType">��������</param>
        /// <param name="cmdText">��������</param>
        /// <param name="commandParameters">�������</param>
        /// <returns>reader</returns>
        public static SqlDataReader ExecuteReader(SqlConnection conn, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();

            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);

                SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return rdr;
            }
            catch (Exception exc)
            {
                conn.Close();
                throw exc;
            }
        }

        /// <summary>
        /// ִ�����ݶ�ȡ��(��DataReader�ر�ʱ����ص����ݿ����Ӳ��ر�)
        /// </summary>
        /// <param name="conn">���ݿ�����</param>
        /// <param name="cmdType">��������</param>
        /// <param name="cmdText">��������</param>
        /// <param name="commandParameters">�������</param>
        /// <returns>reader</returns>
        public static SqlDataReader ExecuteReaderWithoutClosingConnection(SqlConnection conn, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();

            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);

                SqlDataReader rdr = cmd.ExecuteReader();
                cmd.Parameters.Clear();
                return rdr;
            }
            catch (Exception exc)
            {
                conn.Close();
                throw exc;
            }
        }

        #endregion

        #region ExecuteDataSet
        /// <summary>
        /// Executes the data set.
        /// </summary>
        /// <param name="cmdText">The CMD text.</param>
        /// <param name="commandParameters">The command parameters.</param>
        /// <returns></returns>
        public static DataSet ExecuteDataSet(string cmdText, params SqlParameter[] commandParameters)
        {
            return ExecuteDataSet(CommandType.Text, cmdText, commandParameters);
        }

        /// <summary>
        /// Executes the data set.
        /// </summary>
        /// <param name="cmdType">Type of the CMD.</param>
        /// <param name="cmdText">The CMD text.</param>
        /// <param name="commandParameters">The command parameters.</param>
        /// <returns></returns>
        public static DataSet ExecuteDataSet(CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandTimeout = CommandTimeout;
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {

                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                SqlDataAdapter oda = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                oda.Fill(ds);
                cmd.Parameters.Clear();
                return ds;
            }
        }

        public static DataSet ExecuteDataSetEx(string cmdText, string _connectionString)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandTimeout = CommandTimeout;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                PrepareCommand(cmd, conn, null, CommandType.Text, cmdText, null);
                SqlDataAdapter oda = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                oda.Fill(ds);
                cmd.Parameters.Clear();
                return ds;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmdText">sql</param>
        /// <param name="PageSize">��ҳ����</param>
        /// <param name="CurrentPageIndex">����</param>
        /// <returns></returns>
        public static DataSet ExecuteDataSet(CommandType cmdType, string cmdText, int PageSize, int CurrentPageIndex)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandTimeout = CommandTimeout;
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, null);
                SqlDataAdapter oda = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                oda.Fill(ds, PageSize * (CurrentPageIndex - 1), PageSize, "test");
                //  oda.Fill(ds);
                cmd.Parameters.Clear();
                return ds;
            }
        }


        #endregion

        /// <summary>
        /// Executes the scalar.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="cmdType">Type of the CMD.</param>
        /// <param name="cmdText">The CMD text.</param>
        /// <param name="commandParameters">The command parameters.</param>
        /// <returns></returns>
        public static object ExecuteScalar(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandTimeout = CommandTimeout;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return val;
            }
        }


        #region Procedure

        private static SqlCommand BuildIntCommand(SqlConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            SqlCommand command = BuildQueryCommand(connection, storedProcName, parameters);
            command.Parameters.Add(new SqlParameter("ReturnValue", SqlDbType.Int, 4, ParameterDirection.ReturnValue, false, 0, 0, string.Empty, DataRowVersion.Default, null));
            return command;
        }

        private static SqlCommand BuildQueryCommand(SqlConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            SqlCommand command = new SqlCommand(storedProcName, connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            foreach (SqlParameter parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }
            return command;
        }

        public static SqlDataReader RunProcedure(string storedProcName, IDataParameter[] parameters)
        {
            SqlDataReader reader2;
            SqlConnection connection = new SqlConnection(ConnectionString);
            try
            {
                connection.Open();
                SqlCommand command = BuildQueryCommand(connection, storedProcName, parameters);
                command.CommandType = CommandType.StoredProcedure;
                reader2 = command.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (SqlException exception)
            {
                throw new Exception(exception.Message);
            }
            finally
            {
                connection.Close();
            }
            return reader2;
        }

        public static int RunProcedure(string storedProcName, IDataParameter[] parameters, out int rowsAffected)
        {
            int num2;
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = BuildIntCommand(connection, storedProcName, parameters);
                    rowsAffected = command.ExecuteNonQuery();
                    int num = (int)command.Parameters["ReturnValue"].Value;
                    num2 = num;
                }
                catch (SqlException exception)
                {
                    throw new Exception(exception.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
            return num2;
        }

        public static DataSet RunProcedure(string storedProcName, IDataParameter[] parameters, string tableName)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                DataSet dataSet = new DataSet();
                connection.Open();
                new SqlDataAdapter { SelectCommand = BuildQueryCommand(connection, storedProcName, parameters) }.Fill(dataSet, tableName);
                connection.Close();
                return dataSet;
            }
        }

        #endregion



        #region ExecuteScalar

        /// <summary>
        /// Executes the scalar.
        /// </summary>
        /// <param name="cmdType">Type of the CMD.</param>
        /// <param name="cmdText">The CMD text.</param>
        /// <param name="commandParameters">The command parameters.</param>
        /// <returns></returns>
        public static object ExecuteScalar(CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandTimeout = CommandTimeout;
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {

                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return val;
            }
        }

        /// <summary>
        /// Executes the scalar.
        /// </summary>
        /// <param name="cmdText">The CMD text.</param>
        /// <param name="commandParameters">The command parameters.</param>
        /// <returns></returns>
        public static object ExecuteScalar(string cmdText, params SqlParameter[] commandParameters)
        {
            return ExecuteScalar(CommandType.Text, cmdText, commandParameters);
        }

        /// <summary>
        /// Executes the scalar.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="commandParameters">The command parameters.</param>
        /// <returns></returns>
        public static object ExecuteScalar(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (transaction == null)
                throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null)
                throw new ArgumentException("The transaction was rollbacked	or commited, please	provide	an open	transaction.", "transaction");

            // Create a	command	and	prepare	it for execution
            SqlCommand cmd = new SqlCommand();
            cmd.CommandTimeout = CommandTimeout;
            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters);

            // Execute the command & return	the	results
            object retval = cmd.ExecuteScalar();

            // Detach the SqlParameters	from the command object, so	they can be	used again
            cmd.Parameters.Clear();
            return retval;
        }

        ///// <summary>
        ///// Executes the scalar.
        ///// </summary>
        ///// <param name="connectionString">The connection string.</param>
        ///// <param name="cmdType">Type of the CMD.</param>
        ///// <param name="cmdText">The CMD text.</param>
        ///// <param name="commandParameters">The command parameters.</param>
        ///// <returns></returns>
        //public static object ExecuteScalar(SqlConnection connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        //{
        //    SqlCommand cmd = new SqlCommand();

        //    PrepareCommand(cmd, connectionString, null, cmdType, cmdText, commandParameters);
        //    object val = cmd.ExecuteScalar();
        //    cmd.Parameters.Clear();
        //    return val;
        //}

        #endregion

        #region ���ɲ���
        /// <summary>
        /// ����in��Ԥ�������
        /// </summary>
        /// <param name="ParamName"></param>
        /// <param name="DbType"></param>
        /// <param name="Size"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static SqlParameter MakeInParam(string ParamName, SqlDbType DbType, int Size, object Value)
        {
            return MakeParam(ParamName, DbType, Size, ParameterDirection.Input, Value);
        }

        /// <summary>
        /// ����in��Ԥ�������
        /// </summary>
        /// <param name="ParamName"></param>
        /// <param name="DbType"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static SqlParameter MakeInParam(string ParamName, SqlDbType DbType, object Value)
        {
            return MakeParam(ParamName, DbType, 0, ParameterDirection.Input, Value);
        }

        /// <summary>
        /// ����out��Ԥ�������
        /// </summary>
        /// <param name="ParamName"></param>
        /// <param name="DbType"></param>
        /// <param name="Size"></param>
        /// <returns></returns>
        public static SqlParameter MakeOutParam(string ParamName, SqlDbType DbType, int Size)
        {
            return MakeParam(ParamName, DbType, Size, ParameterDirection.Output, null);
        }

        /// <summary>
        /// ����out��Ԥ�������
        /// </summary>
        /// <param name="ParamName"></param>
        /// <param name="DbType"></param>
        /// <returns></returns>
        public static SqlParameter MakeOutParam(string ParamName, SqlDbType DbType)
        {
            return MakeParam(ParamName, DbType, 0, ParameterDirection.Output, null);
        }

        public static SqlParameter MakeReturnParam(SqlDbType DbType)
        {
            return MakeParam("@returnvalue", DbType, 0, ParameterDirection.ReturnValue, null);
        }

        /// <summary>
        /// ����Ԥ�������
        /// </summary>
        /// <param name="ParamName"></param>
        /// <param name="DbType"></param>
        /// <param name="Size"></param>
        /// <param name="Direction">һ����������Ϊout��return��Ԥ�������</param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static SqlParameter MakeParam(string ParamName, SqlDbType DbType, Int32 Size, ParameterDirection Direction, object Value)
        {
            SqlParameter param;

            if (Size > 0)
                param = new SqlParameter(ParamName,DbType, Size);
            else
                param = new SqlParameter(ParamName,DbType);

            param.Direction = Direction;
            if (!(Direction == ParameterDirection.Output && Value == null) && !(Direction == ParameterDirection.ReturnValue && Value == null))
                param.Value = Value;

            return param;
        }

        #endregion ���ɲ�������


        #region MyRegion


        public static DataSet GetList(string Table, string PkField, string Fields, string StrWhere, string Sort, int PageSize, int CurrentPage, out int RecordCount)
        {
            /*
             @iRecordCount INT OUTPUT, --�ܼ�¼��
             @iPageCurr INT, --��ǰҳ
             @iPageSize INT,--ÿҳ��¼��
             @sPkey NVARCHAR(50), --����
             @sField NVARCHAR(1000), --�����ֶ�'*'
             @sTable NVARCHAR(100), --�����ͼ��
             @sCondition NVARCHAR(1000),--����
             @sOrder NVARCHAR(100) --����֧�ֶ��ֶ��� ID Desc,Time Desc
            */

            try
            {

                System.Data.SqlClient.SqlParameter sqlOutParmar = new SqlParameter("@iRecordCount ", SqlDbType.Int);
                sqlOutParmar.Direction = ParameterDirection.Output;
                SqlParameter[] parameters = {
                    sqlOutParmar,
                    new SqlParameter("@iPageCurr", SqlDbType.Int),
                    new SqlParameter("@iPageSize", SqlDbType.Int),
                    new SqlParameter("@sPkey", SqlDbType.NVarChar,100),
                    new SqlParameter("@sField", SqlDbType.NVarChar,4000),
					new SqlParameter("@sTable", SqlDbType.NVarChar,100),
                    new SqlParameter("@sCondition", SqlDbType.NVarChar,4000),
                    new SqlParameter("@sOrder ", SqlDbType.NVarChar,400),
				};
                parameters[1].Value = CurrentPage;
                parameters[2].Value = PageSize;
                parameters[3].Value = PkField;
                parameters[4].Value = Fields;
                parameters[5].Value = Table;
                parameters[6].Value = StrWhere;
                parameters[7].Value = Sort;

                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("UP_GetRecordFromPage", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    foreach (SqlParameter parm in parameters)
                        cmd.Parameters.Add(parm);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataSet dst = new DataSet();
                    da.Fill(dst);
                    RecordCount = int.Parse(sqlOutParmar.Value.ToString());
                    return dst;
                }

            }
            catch (Exception ex)
            {
                RecordCount = 0;
                throw new Exception(ex.ToString());
            }


        }


        /// <summary>
        /// ���ط�ҳ��¼
        /// </summary>
        /// <param name="TableName">������</param>
        /// <param name="Fields">���ֶ�(a,b,c,d)</param>
        /// <param name="SqlWhere">��ѯ����������Ҫ��where</param>
        /// <param name="OrderField">�����ֶ�(ID Asc,Name Desc)</param>
        /// <param name="PageSize">ÿҳ��¼��</param>
        /// <param name="CurrentPage">��ǰҳ��</param>
        /// <param name="RecordCount">��¼����</param>
        /// <returns></returns>
        public static DataSet GetList(string TableName, string Fields, string SqlWhere, string OrderField, int PageSize, int CurrentPage, out int RecordCount)
        {
            try
            {

                System.Data.SqlClient.SqlParameter sqlOutParmar = new SqlParameter("@TotalRecord ", SqlDbType.Int);
                sqlOutParmar.Direction = ParameterDirection.Output;
                SqlParameter[] parameters = {
                    new SqlParameter("@TableName", SqlDbType.NVarChar,100),
                    new SqlParameter("@Fields", SqlDbType.NVarChar,4000),
                    new SqlParameter("@OrderField", SqlDbType.NVarChar,4000),
                    new SqlParameter("@SqlWhere", SqlDbType.NVarChar,4000),
					new SqlParameter("@PageSize", SqlDbType.Int),
                    new SqlParameter("@PageIndex", SqlDbType.Int),
                    sqlOutParmar
				};
                parameters[0].Value = TableName;
                parameters[1].Value = Fields;
                parameters[2].Value = OrderField;
                parameters[3].Value = SqlWhere;
                parameters[4].Value = PageSize;
                parameters[5].Value = CurrentPage;


                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("UP_GetRecordByPage", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    foreach (SqlParameter parm in parameters)
                        cmd.Parameters.Add(parm);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataSet dst = new DataSet();
                    da.Fill(dst);
                    RecordCount = int.Parse(sqlOutParmar.Value.ToString());
                    return dst;
                }
            }
            catch (Exception ex)
            {
                RecordCount = 0;
                throw new Exception(ex.ToString());
            }
        }


        /// <summary>
        /// ����ָ�������ļ�¼
        /// </summary>
        /// <param name="ReturnCount">��������</param>
        /// <param name="Table">��������ͼ��</param>
        /// <param name="Fields">�����ֶμ���</param>
        /// <param name="StrWhere">������Ҫ��Where</param>
        /// <param name="Sort">����Ҫ��Order By</param>
        /// <returns></returns>
        public static DataSet GetList(int ReturnCount, string Table, string Fields, string StrWhere, string Sort)
        {
            try
            {


                SqlParameter[] parameters = {
                    new SqlParameter("@ReturnCount", SqlDbType.Int,4),
                    new SqlParameter("@Table", SqlDbType.NVarChar,1000),
                    new SqlParameter("@Fields", SqlDbType.NVarChar,4000),
                    new SqlParameter("@StrWhere", SqlDbType.NVarChar,4000),
                    new SqlParameter("@Sort", SqlDbType.NVarChar,400)
				};
                parameters[0].Value = ReturnCount;
                parameters[1].Value = Table;
                parameters[2].Value = Fields;
                parameters[3].Value = StrWhere;
                parameters[4].Value = Sort;

                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("UP_GetRecordByTop", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    foreach (SqlParameter parm in parameters)
                        cmd.Parameters.Add(parm);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataSet dst = new DataSet();
                    da.Fill(dst);
                    return dst;
                }


            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        #endregion


        /// <summary>
        /// Caches the parameters.
        /// </summary>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="commandParameters">The command parameters.</param>
        public static void CacheParameters(string cacheKey, params SqlParameter[] commandParameters)
        {
            parmCache[cacheKey] = commandParameters;
        }

        /// <summary>
        /// Gets the cached parameters.
        /// </summary>
        /// <param name="cacheKey">The cache key.</param>
        /// <returns></returns>
        public static SqlParameter[] GetCachedParameters(string cacheKey)
        {
            SqlParameter[] cachedParms = (SqlParameter[])parmCache[cacheKey];

            if (cachedParms == null)
                return null;

            SqlParameter[] clonedParms = new SqlParameter[cachedParms.Length];

            for (int i = 0, j = cachedParms.Length; i < j; i++)
                clonedParms[i] = (SqlParameter)((ICloneable)cachedParms[i]).Clone();

            return clonedParms;
        }

        /// <summary>
        /// Oras the bit.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// <returns></returns>
        public static string SqlBit(bool value)
        {
            if (value)
                return "Y";
            else
                return "N";
        }

        /// <summary>
        /// Oras the bool.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static bool SqlBool(string value)
        {
            if (value.Equals("Y"))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Prepares the command.
        /// </summary>
        /// <param name="cmd">The CMD.</param>
        /// <param name="conn">The conn.</param>
        /// <param name="trans">The trans.</param>
        /// <param name="cmdType">Type of the CMD.</param>
        /// <param name="cmdText">The CMD text.</param>
        /// <param name="cmdParms">The CMD parms.</param>
        private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, SqlParameter[] cmdParms)
        {

            if (conn.State != ConnectionState.Open)
                conn.Open();

            cmd.Connection = conn;
            cmd.CommandText = cmdText;

            if (trans != null)
                cmd.Transaction = trans;

            cmd.CommandType = cmdType;

            if (cmdParms != null)
            {
                foreach (SqlParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }

        #region IDataBase ��Ա

        /// <summary>
        /// ������ݿ�����
        /// </summary>
        /// <returns>���ݿ�����</returns>
        public static System.Data.Common.DbConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        #endregion
    }
}