using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Drawing;
using MySql.Data.MySqlClient;
using System.Xml.Linq;

public interface IDatabaseAccess
{
    public void ReadFromDB();
    public void InsertToDB(string username, string name, string surname);
    public void InsertToHistory(string username, int requests);
    public bool CheckUserExists(string username);
}
public class DatabaseAccess : IDatabaseAccess
{
    // public static string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=Users;Integrated Security=True";
    public static string connectionString = @"Data Source=MASECHABAK-OZOW\SQLEXPRESS;Initial Catalog=Users;Integrated Security=True";
    public static SqlConnection connection = new SqlConnection(connectionString);
    public static SqlDataReader dataReader;
    public static SqlDataAdapter adapter = new SqlDataAdapter();

    public DatabaseAccess() {
      
        
    }

    public void ReadFromDB()
    {
        connection.Open();
        SqlCommand command;
        String sql, Output = "";
        sql = "SELECT * FROM user_t JOIN user_hist ON user_hist.username = user_t.username;";
        command = new SqlCommand(sql, connection);
        dataReader= command.ExecuteReader();
        while (dataReader.Read())
        {
            Output = Output +dataReader.GetValue(0) + " - " + dataReader.GetValue(1); 
        }

       // Console.WriteLine(Output);
        dataReader.Close();
        command.Dispose();
        connection.Close();
    }
    public void InsertToDB(string username, string name, string surname)
    {
        connection.Open();
        SqlCommand command;
        String sql = "";
        sql = $"INSERT INTO user_t(name, surname, username) VALUES ('{name}','{surname}', '{username}');";
        command = new SqlCommand(sql, connection);
        adapter.InsertCommand = new SqlCommand(sql, connection);
        adapter.InsertCommand.ExecuteNonQuery();
        Console.WriteLine("A new user has been added!");
        command.Dispose();
        connection.Close();
        connection.Open();
        sql = $"INSERT INTO user_hist(requests, username) VALUES ( '1','{username}');";
        command = new SqlCommand(sql, connection);
        adapter.InsertCommand = new SqlCommand(sql, connection);
        adapter.InsertCommand.ExecuteNonQuery();
        command.Dispose();
        connection.Close();
    }

   
    public void InsertToHistory(string username, int requests) {

        connection.Open();
        SqlCommand command;
        String output = "";
        String sql = "";
        sql = $"SELECT requests FROM user_hist WHERE user_hist.username = '{username}';";
        command = new SqlCommand(sql, connection);
        dataReader = command.ExecuteReader();
        while (dataReader.Read())
        {
            output = output + dataReader.GetValue(0);
        }
        //command.Dispose();
        connection.Close();
        connection.Open();

        int currentRequest = Convert.ToInt32(output);
        requests = currentRequest + 1;

        sql = $"UPDATE user_hist SET requests = '{requests}' WHERE user_hist.username = '{username}';";
        command = new SqlCommand(sql, connection);
        adapter.UpdateCommand = new SqlCommand(sql, connection);
        adapter.UpdateCommand.ExecuteNonQuery();
        dataReader.Close();
        command.Dispose();
        connection.Close();

    }

    public bool CheckUserExists(string username) {
        connection.Open();
        SqlCommand command;
        string sql = "";
        sql = $"SELECT COUNT(*) from user_t where username like '{username}';";
        command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@username", username);
        int reqCount = (int)command.ExecuteScalar();
        command.Dispose();
        connection.Close();


        if (reqCount > 0)
        {
            //user exists
            return true;
        }
        else
        {
            //user dne
            return false;
        }

      
    }

}


