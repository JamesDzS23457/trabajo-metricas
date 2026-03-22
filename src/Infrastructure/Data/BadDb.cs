using System;
using System.Data;
using Microsoft.Data.Sqlite;

namespace Infrastructure.Data;


public static class BadDb
{
    public const string ConnectionString = "Data Source=Tienda.db";

    public static void Initialize()
    {
        using (var connection = new SqliteConnection(ConnectionString))
        {
            connection.Open();
            var command  = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Orders (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    CustomerName TEXT,
                    ProductName TEXT,
                    Quantity INTEGER,
                    UnitPrice DECIMAL
                );";
            command.ExecuteNonQuery();
        }
    }

    public static int ExecuteNonQueryUnsafe(string sql)
    {
        using (var conn = new SqliteConnection(ConnectionString))
        {
            conn.Open();
            var cmd = new SqliteCommand(sql, conn);
            return cmd.ExecuteNonQuery();
        }
    }

    public static IDataReader ExecuteReaderUnsafe(string sql)
    {
        var conn = new SqliteConnection(ConnectionString);
        conn.Open();
        var cmd = new SqliteCommand(sql, conn);
        return cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
    }
}
