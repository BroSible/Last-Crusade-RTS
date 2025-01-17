using System;
using Mono.Data.Sqlite;
using UnityEngine;

public class UnitDatabase : MonoBehaviour
{
    private string dbName = "URI=file:Units.db";

    [SerializeField] private UnitDatabaseSO unitDatabaseSO;

    private void Start()
    {
        CreateTable();
        InsertUnitsIfNotExists();
        Debug.Log("Unit Database load completed.");
    }

    // Создание таблицы с уникальным именем юнита
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
                    name TEXT NOT NULL UNIQUE,
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

    // Вставка юнитов только если их нет в базе
    private void InsertUnitsIfNotExists()
    {
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                foreach (var unit in unitDatabaseSO.units)
                {
                    // Проверка существования юнита по имени
                    command.CommandText = "SELECT COUNT(*) FROM units WHERE name = @name;";
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@name", unit.Name);

                    long count = (long)command.ExecuteScalar();

                    // Вставка юнита, если его нет в БД
                    if (count == 0)
                    {
                        command.CommandText = @"
                        INSERT INTO units (name, description, maxHealth, unitDamage, speedUnit, attackingDistance, stopAttackingDistance) 
                        VALUES (@name, @description, @maxHealth, @unitDamage, @speedUnit, @attackingDistance, @stopAttackingDistance);";

                        command.Parameters.AddWithValue("@description", unit.description);
                        command.Parameters.AddWithValue("@maxHealth", unit.maxHealth);
                        command.Parameters.AddWithValue("@unitDamage", unit.unitDamage);
                        command.Parameters.AddWithValue("@speedUnit", unit.speedUnit);
                        command.Parameters.AddWithValue("@attackingDistance", unit.attackingDistance);
                        command.Parameters.AddWithValue("@stopAttackingDistance", unit.stopAttackingDistance);

                        command.ExecuteNonQuery();

                        Debug.Log($"Юнит \"{unit.Name}\" добавлен в базу данных.");
                    }
                    else
                    {
                        Debug.Log($"Юнит \"{unit.Name}\" уже существует в базе данных.");
                    }
                }
            }
        }
    }
}
