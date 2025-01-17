using System;
using Mono.Data.Sqlite;
using UnityEngine;

public class FactionsDatabase : MonoBehaviour
{
    private string dbName = "URI=file:Factions.db";

    private void Start()
    {
        CreateTable();
        InsertFactionsIfNotExists();
        Debug.Log("Faction Database setup completed.");
    }

    // Создание таблицы
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
                    name TEXT NOT NULL UNIQUE,
                    description TEXT NOT NULL
                );";
                command.ExecuteNonQuery();
            }
        }
    }

    // Вставка фракций только при их отсутствии
    private void InsertFactionsIfNotExists()
    {
        InsertFactionIfNotExists("Крестоносцы", "Рыцарский орден, посвятивший себя борьбе за веру.");
        InsertFactionIfNotExists("Сарацины", "Мусульманская фракция, известная своей культурой и военной мощью.");
    }

    // Метод для добавления фракции при отсутствии в БД
    private void InsertFactionIfNotExists(string name, string description)
    {
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                // Проверка существования фракции
                command.CommandText = "SELECT COUNT(*) FROM factions WHERE name = @name;";
                command.Parameters.AddWithValue("@name", name);

                long count = (long)command.ExecuteScalar();

                // Если фракция отсутствует, добавляем её
                if (count == 0)
                {
                    command.CommandText = "INSERT INTO factions (name, description) VALUES (@name, @description);";
                    command.Parameters.AddWithValue("@description", description);
                    command.ExecuteNonQuery();

                    Debug.Log($"Фракция \"{name}\" добавлена в базу данных.");
                }
                else
                {
                    Debug.Log($"Фракция \"{name}\" уже существует в базе данных.");
                }
            }
        }
    }
}
