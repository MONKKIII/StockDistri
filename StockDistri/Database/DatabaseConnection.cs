using MySql.Data.MySqlClient;
using System.Data;
using System;

public class DatabaseConnection
{
    private MySqlConnection connection;
    private string server;
    private string database;
    private string uid;
    private string password;

    public DatabaseConnection()
    {
        Initialize();
    }

    private void Initialize()
    {
        server = "localhost";
        database = "stockbase";
        uid = "root";
        password = "";

        string connectionString;
        connectionString = $"SERVER={server};DATABASE={database};UID={uid};PASSWORD={password};";

        connection = new MySqlConnection(connectionString);
    }

    // Ouvrir la connexion à la base de données
    private bool OpenConnection()
    {
        try
        {
            connection.Open();
            return true;
        }
        catch (MySqlException ex)
        {
            // Gérer les erreurs de connexion
            return false;
        }
    }

    // Fermer la connexion à la base de données
    private bool CloseConnection()
    {
        try
        {
            connection.Close();
            return true;
        }
        catch (MySqlException ex)
        {
            // Gérer les erreurs de fermeture de connexion
            return false;
        }
    }

    // Exécuter une requête SQL
    public void ExecuteQuery(string query)
    {
        if (this.OpenConnection())
        {
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.ExecuteNonQuery();
            this.CloseConnection();
        }
    }

    // Récupérer des données à partir d'une requête SQL
    public DataTable ExecuteSelectQuery(string query)
    {
        DataTable dataTable = new DataTable();
        try
        {
            if (this.OpenConnection())
            {
                using (MySqlDataAdapter dataAdapter = new MySqlDataAdapter(query, connection))
                {
                    dataAdapter.Fill(dataTable);
                }
                this.CloseConnection();
            }
        }
        catch (Exception ex)
        {
            // Gérer les erreurs de récupération de données
        }
        return dataTable;
    }
}
