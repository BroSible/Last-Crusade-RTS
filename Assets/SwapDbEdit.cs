using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mono.Data.Sqlite;

public class SwapDbEdit : MonoBehaviour
{
    public GameObject unitDb;
    public GameObject usersDb;
    public GameObject factionsDb;
    public GameObject gameStatsDb;
    public GameObject buildingsDb;

    [SerializeField] private TMP_Text inspectorText;
    [SerializeField] private TMP_Dropdown unitIdDropdown;
    [SerializeField] private TMP_Dropdown userIdDropdown;
    [SerializeField] private TMP_Dropdown factionIdDropdown;
    [SerializeField] private TMP_Dropdown gameStatsIdDropdown;
    [SerializeField] private TMP_Dropdown buildingIdDropdown;

    private string dbNameUnit = "URI=file:Units.db";
    private string dbNameUsers = "URI=file:Users.db";
    private string dbNameFactions = "URI=file:Factions.db";
    private string dbNameGameStats = "URI=file:GameStats.db";
    private string dbNameBuildings = "URI=file:Buildings.db";

    public void unitDbEnable()
    {
        unitDb.SetActive(true);
        usersDb.SetActive(false);
        factionsDb.SetActive(false);
        gameStatsDb.SetActive(false);
        buildingsDb.SetActive(false);
        LoadUnitData();
        LoadIdsToDropdown(dbNameUnit, "units", unitIdDropdown);
    }

    public void usersDbEnable()
    {
        unitDb.SetActive(false);
        usersDb.SetActive(true);
        factionsDb.SetActive(false);
        gameStatsDb.SetActive(false);
        buildingsDb.SetActive(false);
        LoadUserData();
        LoadIdsToDropdown(dbNameUsers, "users", userIdDropdown);
    }

    public void factionsDbEnable()
    {
        unitDb.SetActive(false);
        usersDb.SetActive(false);
        factionsDb.SetActive(true);
        gameStatsDb.SetActive(false);
        buildingsDb.SetActive(false);
        LoadFactionsData();
        LoadIdsToDropdown(dbNameFactions, "factions", factionIdDropdown);
    }

    public void gameStatsDbEnable()
    {
        unitDb.SetActive(false);
        usersDb.SetActive(false);
        factionsDb.SetActive(false);
        gameStatsDb.SetActive(true);
        buildingsDb.SetActive(false);
        LoadGameStatsData();
        LoadIdsToDropdown(dbNameGameStats, "GameStatistics", gameStatsIdDropdown);
    }

    public void buildingsDbEnable()
    {
        unitDb.SetActive(false);
        usersDb.SetActive(false);
        factionsDb.SetActive(false);
        gameStatsDb.SetActive(false);
        buildingsDb.SetActive(true);
        LoadBuildingData();
        LoadIdsToDropdown(dbNameBuildings, "ObjectData", buildingIdDropdown);
    }

    private void LoadIdsToDropdown(string dbName, string tableName, TMP_Dropdown dropdown)
    {
        List<string> ids = new List<string>();
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"SELECT id FROM {tableName};";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ids.Add(reader["id"].ToString());
                    }
                }
            }
        }
        dropdown.ClearOptions();
        dropdown.AddOptions(ids);
    }

    private void LoadUnitData()
    {
        using (var connection = new SqliteConnection(dbNameUnit))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM units;";
                using (var reader = command.ExecuteReader())
                {
                    string data = "<mspace=0.7em><b>| ID  | Имя        | Здоровье | Урон  | Скорость | Дист. Атаки | Стоп Атака |</b>\n";
                    data += "<b>|-----|------------|----------|-------|----------|-------------|------------|</b>\n";

                    while (reader.Read())
                    {
                        data += $"| {reader["id"],-3} | {reader["name"],-10} | {reader["maxHealth"],8} | {reader["unitDamage"],5} | {reader["speedUnit"],8} | {reader["attackingDistance"],11:F1} | {reader["stopAttackingDistance"],10:F1} |\n";
                    }
                    data += "</mspace>";
                    inspectorText.text = data;
                }
            }
        }
    }

private void LoadUserData()
{
    using (var connection = new SqliteConnection(dbNameUsers))
    {
        connection.Open();
        using (var command = connection.CreateCommand())
        {
            command.CommandText = "SELECT * FROM users;";
            using (var reader = command.ExecuteReader())
            {
                string data = "<mspace=0.7em><b>| ID  | Username   | Password   |</b>\n";
                data += "<b>|-----|------------|------------|</b>\n";

                // Проверка наличия столбцов
                int idIndex = reader.GetOrdinal("id");
                int usernameIndex = reader.GetOrdinal("username");
                int passwordIndex = reader.GetOrdinal("password");

                while (reader.Read())
                {
                    // Используем индексы столбцов для извлечения значений
                    string id = reader.IsDBNull(idIndex) ? "N/A" : reader.GetString(idIndex);
                    string username = reader.IsDBNull(usernameIndex) ? "N/A" : reader.GetString(usernameIndex);
                    string password = reader.IsDBNull(passwordIndex) ? "N/A" : reader.GetString(passwordIndex);

                    data += $"| {id,-3} | {username,-10} | {password,-10} |\n";
                }
                data += "</mspace>";
                inspectorText.text = data;
            }
        }
    }
}


    private void LoadFactionsData()
    {
        using (var connection = new SqliteConnection(dbNameFactions))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM factions;";
                using (var reader = command.ExecuteReader())
                {
                    string data = "<mspace=0.7em><b>| ID  | Название   | Описание             |</b>\n";
                    data += "<b>|-----|------------|---------------------|</b>\n";

                    while (reader.Read())
                    {
                        data += $"| {reader["id"],-3} | {reader["name"],-10} | {reader["description"],-19} |\n";
                    }
                    data += "</mspace>";
                    inspectorText.text = data;
                }
            }
        }
    }

    private void LoadGameStatsData()
    {
        using (var connection = new SqliteConnection(dbNameGameStats))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                // Запрос на выборку всех данных из таблицы GameStatistics
                command.CommandText = "SELECT * FROM GameStatistics;";

                using (var reader = command.ExecuteReader())
                {
                    // Проверка на наличие столбцов и их индексы
                    int idIndex = reader.GetOrdinal("id");
                    int unitCountIndex = reader.GetOrdinal("unitCount");
                    int buildingCountIndex = reader.GetOrdinal("buildingCount");
                    int timestampIndex = reader.GetOrdinal("timestamp");

                    string data = "<mspace=0.7em><b>| ID  | UnitCount | BuildingCount | Timestamp           |</b>\n";
                    data += "<b>|-----|-----------|---------------|---------------------|</b>\n";

                    // Чтение строк из базы данных
                    while (reader.Read())
                    {
                        // Извлечение данных из каждого столбца
                        string id = reader.IsDBNull(idIndex) ? "N/A" : reader.GetInt32(idIndex).ToString();
                        string unitCount = reader.IsDBNull(unitCountIndex) ? "N/A" : reader.GetInt32(unitCountIndex).ToString();
                        string buildingCount = reader.IsDBNull(buildingCountIndex) ? "N/A" : reader.GetInt32(buildingCountIndex).ToString();
                        string timestamp = reader.IsDBNull(timestampIndex) ? "N/A" : reader.GetString(timestampIndex);

                        // Форматирование строки для отображения
                        data += $"| {id,-3} | {unitCount,-9} | {buildingCount,-13} | {timestamp,-19} |\n";
                    }

                    data += "</mspace>";
                    inspectorText.text = data;  // Отображение данных в UI
                }
            }
        }
    }




    private void LoadBuildingData()
    {
        using (var connection = new SqliteConnection(dbNameBuildings))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                // Запрос на выборку всех данных из таблицы ObjectData
                command.CommandText = "SELECT * FROM ObjectData;";

                using (var reader = command.ExecuteReader())
                {
                    // Получаем индексы столбцов для каждой строки
                    int idIndex = reader.GetOrdinal("ID");
                    int nameIndex = reader.GetOrdinal("Name");
                    int descriptionIndex = reader.GetOrdinal("Description");
                    int sizeXIndex = reader.GetOrdinal("SizeX");
                    int sizeYIndex = reader.GetOrdinal("SizeY");
                    int costIndex = reader.GetOrdinal("Cost");

                    // Если хотя бы один из столбцов не найден, выводим ошибку
                    if (idIndex == -1 || nameIndex == -1 || sizeXIndex == -1 || sizeYIndex == -1 || costIndex == -1)
                    {
                        Debug.LogError("One or more required columns are missing in the database.");
                        return;
                    }

                    // Переменная для отображения данных в UI
                    string data = "<mspace=0.7em><b>| ID  | Name       | Description    | SizeX | SizeY | Cost |</b>\n";
                    data += "<b>|-----|------------|----------------|-------|-------|------|</b>\n";

                    // Чтение строк из базы данных
                    while (reader.Read())
                    {
                        // Извлечение данных с проверкой на DBNull
                        string id = reader.IsDBNull(idIndex) ? "N/A" : reader.GetInt32(idIndex).ToString();
                        string name = reader.IsDBNull(nameIndex) ? "N/A" : reader.GetString(nameIndex);
                        string description = reader.IsDBNull(descriptionIndex) ? "N/A" : reader.GetString(descriptionIndex);
                        string sizeX = reader.IsDBNull(sizeXIndex) ? "N/A" : reader.GetInt32(sizeXIndex).ToString();
                        string sizeY = reader.IsDBNull(sizeYIndex) ? "N/A" : reader.GetInt32(sizeYIndex).ToString();
                        string cost = reader.IsDBNull(costIndex) ? "N/A" : reader.GetInt32(costIndex).ToString();

                        // Форматирование строки для отображения
                        data += $"| {id,-3} | {name,-10} | {description,-15} | {sizeX,-5} | {sizeY,-5} | {cost,-4} |\n";
                    }

                    data += "</mspace>";
                    inspectorText.text = data;
                }
            }
        }
    }

}
