using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Mono.Data.Sqlite;

public class SwapDbEdit : MonoBehaviour
{
    public GameObject unitDb;
    public GameObject usersDb;
    public GameObject factionsDb;
    public GameObject gameStatsDb;
    public GameObject buildingsDb;

    [SerializeField] private TMP_Text inspectorText;         // Текстовое поле для отображения данных базы
    private string dbNameUnit = "URI=file:Units.db";  // Название базы данных юнитов
    private string dbNameUsers = "URI=file:Users.db";  // Название базы данных пользователей
    private string dbNameFactions = "URI=file:Factions.db";  // Название базы данных фракций
    private string dbNameGameStats = "URI=file:GameStats.db";  // Название базы данных игровых статистик
    private string dbNameBuildings = "URI=file:Buildings.db";  // Название базы данных зданий

    // Методы для переключения между базами данных
    public void unitDbEnable()
    {
        unitDb.SetActive(true);
        usersDb.SetActive(false);
        factionsDb.SetActive(false);
        gameStatsDb.SetActive(false);
        buildingsDb.SetActive(false);
        LoadUnitData();
    }

    public void usersDbEnable()
    {
        unitDb.SetActive(false);
        usersDb.SetActive(true);
        factionsDb.SetActive(false);
        gameStatsDb.SetActive(false);
        buildingsDb.SetActive(false);
        LoadUserData();
    }

    public void factionsDbEnable()
    {
        unitDb.SetActive(false);
        usersDb.SetActive(false);
        factionsDb.SetActive(true);
        gameStatsDb.SetActive(false);
        buildingsDb.SetActive(false);
        LoadFactionsData();
    }

    public void gameStatsDbEnable()
    {
        unitDb.SetActive(false);
        usersDb.SetActive(false);
        factionsDb.SetActive(false);
        gameStatsDb.SetActive(true);
        buildingsDb.SetActive(false);
        LoadGameStatsData();
    }

    public void buildingsDbEnable()
    {
        unitDb.SetActive(false);
        usersDb.SetActive(false);
        factionsDb.SetActive(false);
        gameStatsDb.SetActive(false);
        buildingsDb.SetActive(true);
        LoadBuildingData();
    }

    // Метод для подгрузки данных юнитов из базы данных
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
                    string data = "";
                    data += $"<mspace=0.7em><b>| ID  | Имя        | Здоровье | Урон  | Скорость | Дист. Атаки | Стоп Атака |</b>\n";
                    data += $"<b>|-----|------------|----------|-------|----------|-------------|------------|</b>\n";

                    while (reader.Read())
                    {
                        data += $"| {reader["id"],-3} | {reader["name"],-10} | {reader["maxHealth"],8} | {reader["unitDamage"],5} | {reader["speedUnit"],8} | {reader["attackingDistance"],11:F1} | {reader["stopAttackingDistance"],10:F1} |\n";
                    }
                    data += "</mspace>";
                    inspectorText.text = data;  // Показываем таблицу данных в текстовом поле
                }
            }
        }
    }

    // Метод для подгрузки данных пользователей из базы данных
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
                    string data = "";
                    data += $"<mspace=0.7em><b>| ID  | Username   | Password  |</b>\n";
                    data += $"<b>|-----|------------|-----------|</b>\n";

                    while (reader.Read())
                    {
                        data += $"| {reader["id"],-3} | {reader["username"],-10} | {reader["password"],-9} |\n";
                    }
                    data += "</mspace>";
                    inspectorText.text = data;  // Показываем таблицу данных в текстовом поле
                }
            }
        }
    }

    // Метод для подгрузки данных фракций из базы данных
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
                    string data = "";
                    data += $"<mspace=0.7em><b>| ID  | Название    | Описание    |</b>\n";
                    data += $"<b>|-----|------------|-------------|</b>\n";

                    while (reader.Read())
                    {
                        data += $"| {reader["id"],-3} | {reader["name"],-10} | {reader["description"],-15} |\n";
                    }
                    data += "</mspace>";
                    inspectorText.text = data;  // Показываем таблицу данных в текстовом поле
                }
            }
        }
    }

    // Метод для подгрузки данных игровых статистик из базы данных
    private void LoadGameStatsData()
    {
        using (var connection = new SqliteConnection(dbNameGameStats))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM game_stats;";
                using (var reader = command.ExecuteReader())
                {
                    string data = "";
                    data += $"<mspace=0.7em><b>| ID  | Статистика | Значение   |</b>\n";
                    data += $"<b>|-----|------------|------------|</b>\n";

                    while (reader.Read())
                    {
                        data += $"| {reader["id"],-3} | {reader["stat_name"],-12} | {reader["value"],-9} |\n";
                    }
                    data += "</mspace>";
                    inspectorText.text = data;  // Показываем таблицу данных в текстовом поле
                }
            }
        }
    }

    // Метод для подгрузки данных зданий из базы данных
    private void LoadBuildingData()
    {
        using (var connection = new SqliteConnection(dbNameBuildings))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM ObjectData;";
                using (var reader = command.ExecuteReader())
                {
                    string data = "";
                    data += $"<mspace=0.7em><b>| ID  | Name       | Description   | SizeX | SizeY | Cost |</b>\n";
                    data += $"<b>|-----|------------|---------------|-------|-------|------|</b>\n";

                    while (reader.Read())
                    {
                        data += $"| {reader["ID"],-3} | {reader["Name"],-10} | {reader["Description"],-15} | {reader["SizeX"],5} | {reader["SizeY"],5} | {reader["Cost"],5} |\n";
                    }
                    data += "</mspace>";
                    inspectorText.text = data;  // Показываем таблицу данных в текстовом поле
                }
            }
        }
    }
}
