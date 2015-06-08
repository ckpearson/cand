using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace CandidateAssessment.Data
{
    public static class DatabaseAccess
    {
        public static async Task<DbConnection> GetConnection(string server, string database)
        {
            if (string.IsNullOrEmpty(server)) throw new ArgumentNullException("server");
            if (string.IsNullOrEmpty(database)) throw new ArgumentNullException("database");

            var builder = new SqlConnectionStringBuilder();
            builder.DataSource = server;
            builder.InitialCatalog = database;
            builder.IntegratedSecurity = true;
            var conn = builder.ToString();
            var connection = new SqlConnection(conn);
            await connection.OpenAsync();
            return connection;
        }

        public static Task<int> InsertInto<T>(DbConnection connection, string table, IEnumerable<T> items, params string[] parameters)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (string.IsNullOrEmpty(table)) throw new ArgumentNullException("table");
            if (items == null) throw new ArgumentNullException("items");
            if (parameters == null) throw new ArgumentNullException("parameters");

            var query = string.Format("INSERT INTO {0} VALUES ({1})",
                table, string.Join(",", parameters.Select(p => "@" + p)));
            return connection.ExecuteAsync(query, items);
        }

        public static async Task<IEnumerable<string>> GetTableNames(DbConnection connection)
        {
            if (connection == null) throw new ArgumentNullException("connection");

            using (var reader = await connection.ExecuteReaderAsync("SELECT * FROM Sys.Tables"))
            {
                var tables = new List<string>();
                while (reader.Read())
                {
                    tables.Add(reader.GetString(0));
                }
                return tables; 
            }
        }

        public static async Task<IEnumerable<string>> GetColumnNames(DbConnection connection, string table)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (string.IsNullOrEmpty(table)) throw new ArgumentNullException("table");

            using (var reader = await connection.ExecuteReaderAsync("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='" + table + "'"))
            {
                var cols = new List<string>();
                while (reader.Read())
                {
                    cols.Add(reader.GetString(0));
                }
                return cols; 
            }
        }
    }
}