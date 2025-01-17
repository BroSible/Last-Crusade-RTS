using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;

public class GameStats : MonoBehaviour
{
    private int unitCount = 0;
    private int buildingCount = 0;

    // Предыдущее состояние для сравнения
    private int prevUnitCount = -1;
    private int prevBuildingCount = -1;

    private string dbPath;

    void Start()
    {
        dbPath = "URI=file:" + Application.dataPath + "/../GameStats.db";
        Debug.Log("Database Path: " + dbPath);
        CreateTable();
    }

    void Update()
    {
        // Подсчёт только активных юнитов и зданий
        int currentUnitCount = CountUnits();
        int currentBuildingCount = CountBuildings();

        // Проверка изменений
        if (currentUnitCount != prevUnitCount || currentBuildingCount != prevBuildingCount)
        {
            unitCount = currentUnitCount;
            buildingCount = currentBuildingCount;

            Debug.Log($"Units: {unitCount}");
            Debug.Log($"Buildings: {buildingCount}");

            SaveStatsToDatabase();

            // Обновляем предыдущее состояние
            prevUnitCount = unitCount;
            prevBuildingCount = buildingCount;
        }
    }

    // Подсчёт живых юнитов
    private int CountUnits()
    {
        int count = 0;
        foreach (Unit unit in FindObjectsOfType<Unit>())
        {
            if (!IsLayerAttackable(unit.gameObject) && unit.gameObject.activeInHierarchy && unit.CompareTag("Unit"))
            {
                count++;
            }
        }
        return count;
    }

    // Подсчёт живых зданий
    private int CountBuildings()
    {
        int count = 0;
        foreach (Constructable building in FindObjectsOfType<Constructable>())
        {
            if (!IsLayerAttackable(building.gameObject) && building.gameObject.activeInHierarchy)
            {
                count++;
            }
        }
        return count;
    }

    private bool IsLayerAttackable(GameObject obj)
    {
        return obj.layer == LayerMask.NameToLayer("Attackable");
    }

    private void CreateTable()
    {
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            string createTableQuery = "CREATE TABLE IF NOT EXISTS GameStatistics (" +
                                      "id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                                      "unitCount INTEGER, " +
                                      "buildingCount INTEGER, " +
                                      "timestamp DATETIME DEFAULT CURRENT_TIMESTAMP)";
            using (var command = new SqliteCommand(createTableQuery, connection))
            {
                command.ExecuteNonQuery();
            }
        }
    }

    private void SaveStatsToDatabase()
    {
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            string insertQuery = "INSERT INTO GameStatistics (unitCount, buildingCount) " +
                                 "VALUES (@unitCount, @buildingCount)";

            using (var command = new SqliteCommand(insertQuery, connection))
            {
                command.Parameters.AddWithValue("@unitCount", unitCount);
                command.Parameters.AddWithValue("@buildingCount", buildingCount);

                command.ExecuteNonQuery();
            }
        }
    }
}
