using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace HelperClasses
{
    public static class SqlUtil
    {
        public static SqlCommand StoredProcedure(string StoredProcedureName, params SqlParameter[] parameters)
        {
            var cmd = new SqlCommand(StoredProcedureName);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddRange(parameters.ToArray());

            return cmd;
        }

        public static string SqlServerConnectionString(string server, string database)
        {
            var builder = new SqlConnectionStringBuilder();

            builder.DataSource = server;
            builder.InitialCatalog = database;
            builder.Add("Integrated Security", "SSPI");

            return builder.ConnectionString;
        }

        public static DataTable QueryData(string connectionString, SqlCommand cmd)
        {
            if (connectionString == null) throw new ArgumentNullException("connectionString");
            if (cmd == null) throw new ArgumentNullException("cmd");

            using (var cn = new SqlConnection(connectionString))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.Connection = cn;
                var table = new DataTable();
                da.Fill(table);

                return table;
            }
        }

        /// <summary>
        /// Used with SQL Server Table Valued Parameters. This adds a DataTable
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        public static SqlParameter TableValuedParameter(string parameterName, DataTable table) =>
            new SqlParameter()
            {
                ParameterName = parameterName,
                SqlValue = table,
                SqlDbType = SqlDbType.Structured,
            };
    }
}