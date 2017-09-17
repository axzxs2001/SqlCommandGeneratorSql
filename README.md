# SqlCommandGeneratorSql
A class library that generates SQL statements with SqlCommand to verify that the data is correct in SqlServer 


          var sqlGenerator = new SqlGenerator();

            #region select sql
            var selectCmd = new SqlCommand();
            selectCmd.CommandText = "select * from table1 where field1=@field1 and field2=@field2";
            selectCmd.Parameters.Add(new SqlParameter { ParameterName = "@field1", Value = "字段1", Size = 30 });
            selectCmd.Parameters.Add(new SqlParameter { ParameterName = "@field2", Value = 1 });
            Console.WriteLine(sqlGenerator.BuildSqlStatement(selectCmd));
            Console.WriteLine("-----------------------------------------------------------------------------");
            #endregion
            #region insert sql
            var insertCmd = new SqlCommand();
            insertCmd.CommandText = "insert into table1(field1,field2) value(@field1,@field2)";
            insertCmd.Parameters.Add(new SqlParameter { ParameterName = "@field1", Value = "字段1", Size = 30 });
            insertCmd.Parameters.Add(new SqlParameter { ParameterName = "@field2", Value = 1 });
            Console.WriteLine(sqlGenerator.BuildSqlStatement(insertCmd));
            Console.WriteLine("-----------------------------------------------------------------------------");
            #endregion
            #region stored procedure 
            var storeprocCmd = new SqlCommand();
            storeprocCmd.CommandText = "proc_query";
            storeprocCmd.CommandType = System.Data.CommandType.StoredProcedure;
            storeprocCmd.Parameters.Add(new SqlParameter { ParameterName = "@field1", Value = "字段1", Size = 30 });
            #endregion
            #region output parmeter
            var outPar = new SqlParameter();
            outPar.ParameterName = "@field2";
            outPar.SqlDbType = System.Data.SqlDbType.VarChar;
            outPar.Size = 50;
            outPar.Direction = System.Data.ParameterDirection.Output;
            storeprocCmd.Parameters.Add(outPar);

            Console.WriteLine(sqlGenerator.BuildSqlStatement(storeprocCmd));
            Console.WriteLine("-----------------------------------------------------------------------------");
            #endregion


