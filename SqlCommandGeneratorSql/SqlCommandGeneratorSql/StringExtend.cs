using System;
using System.Collections.Generic;
using System.Text;

namespace SqlCommandGeneratorSql
{
    public static class StringExtend
    {
        public static string ReplaceSingleQuote(this string sql)
        {
            if (sql == null)
            {
                throw new ArgumentNullException("sql");
            }
            return sql.Replace("'", "''");
        }
    }
}
