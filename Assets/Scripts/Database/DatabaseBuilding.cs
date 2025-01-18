using System;
using Mono.Data.Sqlite;
using UnityEngine;

public class DatabaseBuilding : MonoBehaviour
{
    private string dbName = "URI=file:Buildings.db";

    public ObjectsDatabseSO objectsDatabase;

    private void Start()
    {
        CreateTable();
        InsertObjectsIfNotExists();
        Debug.Log("Building Database setup completed.");
    }

    // Создание таблицы для зданий
    private void CreateTable()
    {
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                CREATE TABLE IF NOT EXISTS ObjectData (
                    ID INTEGER PRIMARY KEY,
                    Name TEXT NOT NULL UNIQUE,
                    Description TEXT,
                    SizeX INTEGER,
                    SizeY INTEGER,
                    Cost INTEGER
                );";
                command.ExecuteNonQuery();
            }
        }
    }

    // Вставка объектов, если их нет в БД
    private void InsertObjectsIfNotExists()
    {
        foreach (var obj in objectsDatabase.objectsData)
        {
            int totalCost = 0;
            foreach (var req in obj.requirements)
            {
                totalCost += req.amount;
            }

            InsertObjectIfNotExists(obj.ID, obj.Name, obj.description, obj.Size.x, obj.Size.y, totalCost);
        }
    }

    // Добавление объекта в БД при отсутствии
    private void InsertObjectIfNotExists(int id, string name, string description, int sizeX, int sizeY, int cost)
    {
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                // Проверка существования объекта
                command.CommandText = "SELECT COUNT(*) FROM ObjectData WHERE Name = @name;";
                command.Parameters.AddWithValue("@name", name);

                long count = (long)command.ExecuteScalar();

                // Если объекта нет, добавляем его
                if (count == 0)
                {
                    command.CommandText = @"
                        INSERT INTO ObjectData (ID, Name, Description, SizeX, SizeY, Cost)
                        VALUES (@id, @name, @description, @sizeX, @sizeY, @cost);";
                    command.Parameters.AddWithValue("@id", id);
                    command.Parameters.AddWithValue("@description", description);
                    command.Parameters.AddWithValue("@sizeX", sizeX);
                    command.Parameters.AddWithValue("@sizeY", sizeY);
                    command.Parameters.AddWithValue("@cost", cost);

                    command.ExecuteNonQuery();
                    Debug.Log($"Объект \"{name}\" добавлен в базу данных.");
                }
                else
                {
                    Debug.Log($"Объект \"{name}\" уже существует в базе данных.");
                }
            }
        }
    }
}
