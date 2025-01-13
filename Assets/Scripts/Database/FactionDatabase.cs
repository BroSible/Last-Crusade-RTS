using System;
using Mono.Data.Sqlite;
using UnityEngine;

public class FactionsDatabase : MonoBehaviour
{
    private string dbName = "URI=file:Factions.db";

    private void Start()
    {
        CreateTable();
        InsertFactions();
        Debug.Log("Database setup completed.");
    }

    private void CreateTable()
    {
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                CREATE TABLE IF NOT EXISTS factions (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    name TEXT NOT NULL,
                    description TEXT NOT NULL
                );";
                command.ExecuteNonQuery();
            }
        }
    }

    private void InsertFactions()
    {
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                // Insert "Крестоносцы"
                command.CommandText = "INSERT INTO factions (name, description) VALUES (@name, @description);";
                command.Parameters.AddWithValue("@name", "Крестоносцы");
                command.Parameters.AddWithValue("@description", "Рыцарский орден, посвятивший себя борьбе за веру.");
                command.ExecuteNonQuery();

                // Insert "Сарацины"
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@name", "Сарацины");
                command.Parameters.AddWithValue("@description", "Мусульманская фракция, известная своей культурой и военной мощью.");
                command.ExecuteNonQuery();
            }
        }
    }
}
