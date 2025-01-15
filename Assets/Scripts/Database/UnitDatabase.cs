using System;
using Mono.Data.Sqlite;
using UnityEngine;

public class UnitDatabase: MonoBehaviour
{
    private string dbName = "URI=file:Units.db";

    [SerializeField] private UnitDatabaseSO unitDatabaseSO;

    private void Start()
    {
        CreateTable();
        InsertUnits();
        Debug.Log("Database setup completed.");
    }

    // Метод для создания таблицы юнитов
    private void CreateTable()
    {
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                CREATE TABLE IF NOT EXISTS units (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    name TEXT NOT NULL,
                    description TEXT NOT NULL,
                    maxHealth INTEGER NOT NULL,
                    unitDamage INTEGER NOT NULL,
                    speedUnit INTEGER NOT NULL,
                    attackingDistance REAL NOT NULL,
                    stopAttackingDistance REAL NOT NULL
                );";
                command.ExecuteNonQuery();
            }
        }
    }

    // Метод для вставки данных из UnitDatabaseSO в базу данных
    private void InsertUnits()
    {
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                foreach (var unit in unitDatabaseSO.units)
                {
                    // Вставляем каждый юнит
                    command.CommandText = "INSERT INTO units (name, description, maxHealth, unitDamage, speedUnit, attackingDistance, stopAttackingDistance) VALUES (@name, @description, @maxHealth, @unitDamage, @speedUnit, @attackingDistance, @stopAttackingDistance);";
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@name", unit.Name);
                    command.Parameters.AddWithValue("@description", unit.description);
                    command.Parameters.AddWithValue("@maxHealth", unit.maxHealth);
                    command.Parameters.AddWithValue("@unitDamage", unit.unitDamage);
                    command.Parameters.AddWithValue("@speedUnit", unit.speedUnit);
                    command.Parameters.AddWithValue("@attackingDistance", unit.attackingDistance);
                    command.Parameters.AddWithValue("@stopAttackingDistance", unit.stopAttackingDistance);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
