using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Mono.Data.Sqlite;

public class GameStatsEdit : MonoBehaviour
{
    [SerializeField] private TMP_Text inspectorText;      // Поле для отображения статистики
    [SerializeField] private TMP_Dropdown idDropdown;     // Выпадающий список для выбора записи
    [SerializeField] private TMP_InputField unitCountInput; // Поле для редактирования количества юнитов
    [SerializeField] private TMP_InputField buildingCountInput; // Поле для редактирования количества зданий
    [SerializeField] private Button saveButton;           // Кнопка сохранения изменений
    [SerializeField] private Button addButton;            // Кнопка добавления новой записи
    [SerializeField] private Button deleteButton;         // Кнопка удаления записи

    private string dbName = "URI=file:GameStats.db";
    private List<GameStatsRecord> gameStats = new List<GameStatsRecord>();
    private int currentStatsIndex = 0;

    private void Start()
    {
        LoadGameStatsFromDatabase();
        InitializeDropdown();
        ShowGameStatsData();

        saveButton.onClick.AddListener(SaveChanges);
        addButton.onClick.AddListener(AddStatsRecord);
        deleteButton.onClick.AddListener(DeleteStatsRecord);
        idDropdown.onValueChanged.AddListener(OnIdDropdownValueChanged);
    }

    // Загрузка всех записей статистики из базы данных
    private void LoadGameStatsFromDatabase()
    {
        gameStats.Clear();

        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                // Обновляем запрос, чтобы выбрать также timestamp
                command.CommandText = "SELECT id, unitCount, buildingCount, timestamp FROM GameStatistics;";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        gameStats.Add(new GameStatsRecord
                        {
                            ID = reader.GetInt32(0),
                            UnitCount = reader.GetInt32(1),
                            BuildingCount = reader.GetInt32(2),
                            Timestamp = reader.IsDBNull(3) ? "N/A" : reader.GetString(3) // Загружаем Timestamp
                        });
                    }
                }
            }
        }
    }

    // Инициализация выпадающего списка
    private void InitializeDropdown()
    {
        List<string> idOptions = new List<string>();
        foreach (var stats in gameStats)
        {
            idOptions.Add($"{stats.ID}");
        }

        idDropdown.ClearOptions();
        idDropdown.AddOptions(idOptions);

        if (idOptions.Count > 0)
        {
            idDropdown.value = 0;
            OnIdDropdownValueChanged(0); // Синхронизация с начальным значением
        }
    }

    // Отображение всех записей статистики в текстовом поле
    private void ShowGameStatsData()
    {
        // Заголовок таблицы с добавленным столбцом для Timestamp
        string table = "<mspace=0.7em><b>| ID  | Юниты  | Здания  | Timestamp           |</b>\n";
        table += "<b>|-----|--------|---------|---------------------|</b>\n";

        // Добавляем строки с данными, включая Timestamp
        foreach (var stats in gameStats)
        {
            table += $"| {stats.ID,-3} | {stats.UnitCount,-6} | {stats.BuildingCount,-8} | {stats.Timestamp,-19} |\n";
        }

        table += "</mspace>";
        inspectorText.text = table;
    }

    // Обработка изменения выбора в выпадающем списке
    private void OnIdDropdownValueChanged(int index)
    {
        if (index < 0 || index >= gameStats.Count) return;

        currentStatsIndex = index;
        unitCountInput.text = gameStats[currentStatsIndex].UnitCount.ToString();
        buildingCountInput.text = gameStats[currentStatsIndex].BuildingCount.ToString();
    }

    // Сохранение изменений в базу данных
    private void SaveChanges()
    {
        if (gameStats.Count == 0 || currentStatsIndex >= gameStats.Count) return;

        var stats = gameStats[currentStatsIndex];
        int newUnitCount = int.Parse(unitCountInput.text);
        int newBuildingCount = int.Parse(buildingCountInput.text);

        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    UPDATE GameStatistics
                    SET unitCount = @unitCount, buildingCount = @buildingCount
                    WHERE id = @id;";
                command.Parameters.AddWithValue("@unitCount", newUnitCount);
                command.Parameters.AddWithValue("@buildingCount", newBuildingCount);
                command.Parameters.AddWithValue("@id", stats.ID);

                command.ExecuteNonQuery();
            }
        }

        // Обновляем локальные данные и интерфейс
        stats.UnitCount = newUnitCount;
        stats.BuildingCount = newBuildingCount;
        gameStats[currentStatsIndex] = stats;
        ShowGameStatsData();
        Debug.Log($"Статистика ID {stats.ID} успешно обновлена.");
    }

    // Добавление новой записи статистики
    private void AddStatsRecord()
    {
        int newUnitCount = int.Parse(unitCountInput.text);
        int newBuildingCount = int.Parse(buildingCountInput.text);

        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    INSERT INTO GameStatistics (unitCount, buildingCount) VALUES (@unitCount, @buildingCount);
                    SELECT last_insert_rowid();";
                command.Parameters.AddWithValue("@unitCount", newUnitCount);
                command.Parameters.AddWithValue("@buildingCount", newBuildingCount);

                int newId = (int)(long)command.ExecuteScalar();

                // Добавляем новую запись в локальный список
                gameStats.Add(new GameStatsRecord { ID = newId, UnitCount = newUnitCount, BuildingCount = newBuildingCount, Timestamp = "N/A" });
                InitializeDropdown();
                ShowGameStatsData();
                Debug.Log($"Запись статистики успешно добавлена с ID {newId}.");
            }
        }
    }

    // Удаление текущей записи статистики
    private void DeleteStatsRecord()
    {
        if (gameStats.Count == 0 || currentStatsIndex >= gameStats.Count) return;

        var stats = gameStats[currentStatsIndex];

        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "DELETE FROM GameStatistics WHERE id = @id;";
                command.Parameters.AddWithValue("@id", stats.ID);

                command.ExecuteNonQuery();
            }
        }

        // Удаляем запись из локального списка
        gameStats.RemoveAt(currentStatsIndex);
        InitializeDropdown();
        ShowGameStatsData();

        if (gameStats.Count > 0)
            currentStatsIndex = 0;
        else
            currentStatsIndex = -1;

        Debug.Log($"Запись статистики ID {stats.ID} успешно удалена.");
    }

    // Вспомогательный класс для статистики игры
    private class GameStatsRecord
    {
        public int ID;
        public int UnitCount;
        public int BuildingCount;
        public string Timestamp;  // Добавлено поле для Timestamp
    }
}
