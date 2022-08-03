using MySqlConnector;

namespace kalCasino.Database;

internal static class Db
{
    private const string ConStr = @"server=localhost;userid=root;password=4AnAl1PEnetRator7;database=polniykal";

    public static async Task<long> GetValueLong(string sqlCommand)
    {
        try
        {
            await using var conn = new MySqlConnection(ConStr);
            await conn.OpenAsync();
            await using var command = new MySqlCommand
            {
                CommandText = sqlCommand,
                Connection = conn
            };
            await using var reader = await command.ExecuteReaderAsync();
        
            await reader.ReadAsync();
            var result = reader.GetInt64(0);

            return result;
        }
        catch (MySqlException sqlException)
        {
            Console.WriteLine(sqlException);
            throw;
        }
    }
    
    public static async Task<bool> GetValueBool(string sqlCommand)
    {
        try
        {
            await using var conn = new MySqlConnection(ConStr);
            await conn.OpenAsync();
            await using var command = new MySqlCommand
            {
                CommandText = sqlCommand,
                Connection = conn
            };
            await using var reader = await command.ExecuteReaderAsync();
        
            await reader.ReadAsync();
            var result = reader.GetBoolean(0);

            return result;
        }
        catch (MySqlException sqlException)
        {
            Console.WriteLine(sqlException);
            throw;
        }
    }
    
    public static async Task Do(string sqlCommand)
    {
        try
        {
            await using var conn = new MySqlConnection(ConStr);
            await conn.OpenAsync();
            await using var command = new MySqlCommand
            {
                CommandText = sqlCommand,
                Connection = conn
            };

            await command.ExecuteNonQueryAsync();
        }
        catch (MySqlException sqlException)
        {
            Console.WriteLine(sqlException);
            throw;
        }
    }
}