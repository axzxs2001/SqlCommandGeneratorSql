using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace SqlCommandGeneratorSql.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var sqlGenerator = new SqlGenerator();

            #region 查询sql语句
            var selectCmd = new SqlCommand();
            selectCmd.CommandText = "select * from table1 where field1=@field1 and field2=@field2";
            selectCmd.Parameters.Add(new SqlParameter { ParameterName = "@field1", Value = "字段1", Size = 30 });
            selectCmd.Parameters.Add(new SqlParameter { ParameterName = "@field2", Value = 1 });
            Console.WriteLine(sqlGenerator.BuildSqlStatement(selectCmd));
            Console.WriteLine("-----------------------------------------------------------------------------");
            #endregion
            #region 添加sql语句
            var insertCmd = new SqlCommand();
            insertCmd.CommandText = "insert into table1(field1,field2) value(@field1,@field2)";
            insertCmd.Parameters.Add(new SqlParameter { ParameterName = "@field1", Value = "字段1", Size = 30 });
            insertCmd.Parameters.Add(new SqlParameter { ParameterName = "@field2", Value = 1 });
            Console.WriteLine(sqlGenerator.BuildSqlStatement(insertCmd));
            Console.WriteLine("-----------------------------------------------------------------------------");
            #endregion
            #region 存储过程
            var storeprocCmd = new SqlCommand();
            storeprocCmd.CommandText = "proc_query";
            storeprocCmd.CommandType = System.Data.CommandType.StoredProcedure;
            storeprocCmd.Parameters.Add(new SqlParameter { ParameterName = "@field1", Value = "字段1", Size = 30 });
            #endregion
            #region 出参
            var outPar = new SqlParameter();
            outPar.ParameterName = "@field2";
            outPar.SqlDbType = System.Data.SqlDbType.VarChar;
            outPar.Size = 50;
            outPar.Direction = System.Data.ParameterDirection.Output;
            storeprocCmd.Parameters.Add(outPar);

            Console.WriteLine(sqlGenerator.BuildSqlStatement(storeprocCmd));
            Console.WriteLine("-----------------------------------------------------------------------------");
            #endregion

        }
    }
}
