using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace SqlCommandGeneratorSql
{
    /// <summary>
    /// 本类生成执行的SQL语句
    /// </summary>
    public class SqlGenerator
    {
        /// <summary>
        /// 生成SQL语句
        /// </summary>
        /// <param name="SqlCommand">SqlCommand</param>
        /// <returns></returns>
        public virtual string BuildSqlStatement(SqlCommand sqlCommand)
        {
            var sql = sqlCommand.CommandText;
            if (string.IsNullOrEmpty(sql))
            {
                throw new Exception("此SqlCommand没有CommandText");
            }
            var commandType = sqlCommand.CommandType;
            var parameters = new SqlParameter[sqlCommand.Parameters.Count];
            for (int i = 0; i < parameters.Length; i++)
            {
                parameters[i] = sqlCommand.Parameters[i];
            }
            if (sql == null)
            {
                throw new ArgumentNullException("sql");
            }
            var safeParameters = parameters;
            if (safeParameters != null)
            {
                safeParameters = safeParameters.Where(p => p != null).ToArray();
            }
            switch (commandType)
            {
                case CommandType.StoredProcedure:
                    return CreateExecutableStoredProcedureStatement(sql, safeParameters);
                case CommandType.Text:
                    return CreateExecutableQueryStatement(sql, safeParameters);
                default:
                    throw new NotSupportedException($"这种command type {commandType} 不被支持.");
            }
        }
        /// <summary>
        /// 生成存储过程
        /// </summary>
        /// <param name="storedProcedureName">存储过程名称</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        private string CreateExecutableStoredProcedureStatement(string storedProcedureName, IEnumerable<SqlParameter> parameters)
        {
            var sqlParameters = new List<string>();
            var printParameters = new List<string>();
            var spString = new StringBuilder();
            if (parameters != null)
            {
                foreach (SqlParameter sqlParameter in parameters)
                {
                    if (sqlParameter.Direction != ParameterDirection.Input)
                    {
                        spString.AppendLine(GetParameterDeclaration(sqlParameter));
                        string param = $"{sqlParameter.ParameterName} output";
                        sqlParameters.Add(param);
                        printParameters.Add($"print {sqlParameter.ParameterName}");
                    }
                    else
                    {
                        spString.AppendLine($"{GetParameterDeclaration(sqlParameter)}= {GetParameterValue(sqlParameter)}");
                        string param = $"{sqlParameter.ParameterName}";
                        sqlParameters.Add(param);
                    }
                }
            }
            spString.AppendLine($"EXEC {storedProcedureName} {string.Join(", ", sqlParameters)}");
            spString.AppendLine(string.Join("\r\n", printParameters));
            return spString.ToString();
        }
        /// <summary>
        /// 生成SQL语句
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        private string CreateExecutableQueryStatement(string sql, IEnumerable<SqlParameter> parameters)
        {
            var sqlString = new StringBuilder();
            if (parameters != null)
            {
                foreach (var dbParameter in parameters)
                {
                    sqlString.Append(CreateParameterText(dbParameter));
                }
            }
            sqlString.Append(sql);
            return sqlString.ToString();
        }
        /// <summary>
        /// 生成定义语句
        /// </summary>
        /// <param name="dbParameter"></param>
        /// <returns></returns>
        protected string CreateParameterText(SqlParameter dbParameter)
        {
            return $"{GetParameterDeclaration(dbParameter)}={GetParameterValue(dbParameter)}{Environment.NewLine}";
        }
        /// <summary>
        /// 生成定义参数
        /// </summary>
        /// <param name="dbParameter">参数</param>
        /// <returns></returns>
        protected virtual string GetParameterDeclaration(SqlParameter dbParameter)
        {
            if (dbParameter == null)
            {
                throw new ArgumentNullException("dbParameter");
            }
            var declareBuilder = new StringBuilder();
            switch (dbParameter.SqlDbType)
            {
                case SqlDbType.NText:
                case SqlDbType.Text:
                    declareBuilder.Append($"declare {dbParameter.ParameterName} nvarchar(max)");
                    break;
                default:
                    declareBuilder.Append($"declare {dbParameter.ParameterName} {dbParameter.SqlDbType}");
                    if (dbParameter.Size > 0)
                    {
                        if (dbParameter.Precision > 0)
                        {
                            declareBuilder.Append($"({ dbParameter.Size},{dbParameter.Precision})");
                        }
                        else
                        {
                            declareBuilder.Append($"({dbParameter.Size})");
                        }
                    }
                    break;
            }
            return declareBuilder.ToString();
        }
        /// <summary>
        /// 生成值 
        /// </summary>
        /// <param name="sqlParameter">参数</param>
        /// <returns></returns>
        protected virtual string GetParameterValue(SqlParameter sqlParameter)
        {
            if (sqlParameter.Direction != ParameterDirection.Input)
            {
                return null;
            }
            if (sqlParameter == null)
            {
                throw new ArgumentNullException("sqlParameter");
            }
            string retval;
            if (sqlParameter.Value == DBNull.Value)
            {
                return "null";
            }
            switch (sqlParameter.SqlDbType)
            {
                case SqlDbType.NText:
                case SqlDbType.NVarChar:
                    retval = $"N'{sqlParameter.Value.ToString().ReplaceSingleQuote()}'";
                    break;
                case SqlDbType.Char:
                case SqlDbType.NChar:
                case SqlDbType.Text:
                case SqlDbType.VarChar:
                case SqlDbType.Xml:
                case SqlDbType.Time:
                case SqlDbType.UniqueIdentifier:
                    retval = $"'{sqlParameter.Value.ToString().ReplaceSingleQuote()}'";
                    break;
                case SqlDbType.Date:
                case SqlDbType.DateTime:
                case SqlDbType.DateTime2:
                case SqlDbType.DateTimeOffset:
                    var dateTime = ((DateTime)sqlParameter.Value).ToString("yyyy-MM-dd HH:mm:ss:fff");
                    retval = $"convert(datetime,'{dateTime}', 121)";
                    break;
                case SqlDbType.Bit:
                    retval = (Boolean.Parse(sqlParameter.Value.ToString())) ? "1" : "0";
                    break;
                case SqlDbType.Decimal:
                    retval = ((decimal)sqlParameter.Value).ToString();
                    break;
                case SqlDbType.Image:
                case SqlDbType.Binary:
                case SqlDbType.VarBinary:
                    retval = " -- 不支持ImageBinary --";
                    break;
                default:
                    retval = sqlParameter.Value.ToString().ReplaceSingleQuote();
                    break;
            }
            return retval;
        }
    }

}
