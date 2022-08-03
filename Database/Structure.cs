using MySqlConnector;

namespace kalCasino.Database;

public class Structure
{
    public static void CreateStructure()
        {
            using var con = new MySqlConnection(@"server=localhost;userid=root;password=4AnAl1PEnetRator7;database=polniykal");
            con.Open();
            using var cmd = new MySqlCommand();
            cmd.Connection = con;

            cmd.CommandText = "DROP TABLE IF EXISTS userBalances";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE userBalances(id BIGINT(10) PRIMARY KEY,
        balance BIGINT DEFAULT 0)";
            cmd.ExecuteNonQuery();

            Console.WriteLine("Table balances created");
        }
}