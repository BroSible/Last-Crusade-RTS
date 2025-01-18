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
        ResetAndInsertUnits();
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

    // Удаление всех существующих данных и вставка новых
    private void ResetAndInsertUnits()
    {
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                // Удаляем все данные из таблицы
                command.CommandText = "DELETE FROM units;";
                command.ExecuteNonQuery();
                Debug.Log("Все юниты были удалены из базы данных.");

                // Сброс автоинкремента для id
                command.CommandText = "UPDATE SQLITE_SEQUENCE SET SEQ=0 WHERE NAME='units';";
                command.ExecuteNonQuery();
                Debug.Log("Счетчик автоинкремента был сброшен.");

                // Вставка новых юнитов
                foreach (var unit in unitDatabaseSO.units)
                {
                    command.CommandText = @"
                    INSERT INTO units (name, description, maxHealth, unitDamage, speedUnit, attackingDistance, stopAttackingDistance) 
                    VALUES (@name, @description, @maxHealth, @unitDamage, @speedUnit, @attackingDistance, @stopAttackingDistance);";

                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@name", unit.Name);
                    command.Parameters.AddWithValue("@description", unit.description);
                    command.Parameters.AddWithValue("@maxHealth", unit.maxHealth);
                    command.Parameters.AddWithValue("@unitDamage", unit.unitDamage);
                    command.Parameters.AddWithValue("@speedUnit", unit.speedUnit);
                    command.Parameters.AddWithValue("@attackingDistance", unit.attackingDistance);
                    command.Parameters.AddWithValue("@stopAttackingDistance", unit.stopAttackingDistance);

                    command.ExecuteNonQuery();
                    Debug.Log($"Юнит \"{unit.Name}\" добавлен в базу данных.");
                }
            }
        }
    }
}
