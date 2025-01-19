using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Mono.Data.Sqlite;
using System.Linq;

public class FactionsEditor : MonoBehaviour
{
    [SerializeField] private TMP_Text inspectorText;         // Поле для отображения таблицы фракций
    [SerializeField] private TMP_Dropdown idDropdown;        // Выпадающий список для выбора фракции
    [SerializeField] private TMP_InputField nameInput;       // Поле для редактирования имени фракции
    [SerializeField] private TMP_InputField descriptionInput;// Поле для редактирования описания фракции
    [SerializeField] private Button saveButton;              // Кнопка сохранения изменений
    [SerializeField] private Button addButton;               // Кнопка добавления новой записи
    [SerializeField] private Button deleteButton;            // Кнопка удаления записи

    private string dbName = "URI=file:Factions.db";
    private List<Faction> factions = new List<Faction>();
    private int currentFactionIndex = 0;

    private void Start()
    {
        LoadFactionsFromDatabase();
        InitializeDropdown();
        ShowFactionsData();

        saveButton.onClick.AddListener(SaveChanges);
        addButton.onClick.AddListener(AddFaction);
        deleteButton.onClick.AddListener(DeleteFaction);
        idDropdown.onValueChanged.AddListener(OnIdDropdownValueChanged);
    }

    // Загрузка всех фракций из базы данных
    private void LoadFactionsFromDatabase()
    {
        factions.Clear();

        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT id, name, description FROM factions;";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        factions.Add(new Faction
                        {
                            ID = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Description = reader.GetString(2)
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
        foreach (var faction in factions)
        {
            idOptions.Add($"{faction.ID}");
        }

        idDropdown.ClearOptions();
        idDropdown.AddOptions(idOptions);

        if (idOptions.Count > 0)
        {
            idDropdown.value = 0;
            OnIdDropdownValueChanged(0); // Синхронизация с начальным значением
        }
    }

    // Отображение всех фракций в текстовом поле
    private void ShowFactionsData()
    {
        string table = "<mspace=0.7em><b>| ID  | Имя         | Описание          |</b>\n";
        table += "<b>|-----|-------------|------------------|</b>\n";

        foreach (var faction in factions)
        {
            table += $"| {faction.ID,-3} | {faction.Name,-11} | {faction.Description,-16} |\n";
        }

        table += "</mspace>";
        inspectorText.text = table;
    }

    // Обработка изменения выбора в выпадающем списке
    private void OnIdDropdownValueChanged(int index)
    {
        if (index < 0 || index >= factions.Count) return;

        currentFactionIndex = index;
    }

    // Сохранение изменений в базу данных
    private void SaveChanges()
    {
        if (factions.Count == 0 || currentFactionIndex >= factions.Count) return;

        var faction = factions[currentFactionIndex];
        string newName = string.IsNullOrWhiteSpace(nameInput.text) ? faction.Name : nameInput.text;
        string newDescription = string.IsNullOrWhiteSpace(descriptionInput.text) ? faction.Description : descriptionInput.text;

        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    UPDATE factions
                    SET name = @name, description = @description
                    WHERE id = @id;";
                command.Parameters.AddWithValue("@name", newName);
                command.Parameters.AddWithValue("@description", newDescription);
                command.Parameters.AddWithValue("@id", faction.ID);

                command.ExecuteNonQuery();
            }
        }

        // Обновляем локальные данные и интерфейс
        faction.Name = newName;
        faction.Description = newDescription;
        factions[currentFactionIndex] = faction;
        ShowFactionsData();
        Debug.Log($"Фракция ID {faction.ID} успешно обновлена.");
    }

    // Добавление новой фракции
    private void AddFaction()
    {
        string newName = nameInput.text.Trim();
        string newDescription = descriptionInput.text.Trim();

        if (string.IsNullOrEmpty(newName) || string.IsNullOrEmpty(newDescription))
        {
            Debug.LogError("Все поля должны быть заполнены для добавления новой фракции.");
            return;
        }

        // Находим максимальный ID среди всех существующих фракций
        int newId = factions.Count > 0 ? factions.Max(f => f.ID) + 1 : 1;

        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    INSERT INTO factions (id, name, description) VALUES (@id, @name, @description);";
                command.Parameters.AddWithValue("@id", newId);
                command.Parameters.AddWithValue("@name", newName);
                command.Parameters.AddWithValue("@description", newDescription);

                command.ExecuteNonQuery();
            }
        }

        // Добавляем новую фракцию в локальный список
        factions.Add(new Faction { ID = newId, Name = newName, Description = newDescription });
        InitializeDropdown();
        ShowFactionsData();
        Debug.Log($"Фракция \"{newName}\" успешно добавлена с ID {newId}.");
    }

    // Удаление текущей фракции
    private void DeleteFaction()
    {
        if (factions.Count == 0 || currentFactionIndex >= factions.Count) return;

        var faction = factions[currentFactionIndex];

        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "DELETE FROM factions WHERE id = @id;";
                command.Parameters.AddWithValue("@id", faction.ID);

                command.ExecuteNonQuery();
            }
        }

        // Удаляем фракцию из локального списка
        factions.RemoveAt(currentFactionIndex);
        InitializeDropdown();
        ShowFactionsData();

        if (factions.Count > 0)
            currentFactionIndex = 0;
        else
            currentFactionIndex = -1;

        Debug.Log($"Фракция ID {faction.ID} успешно удалена.");
    }

    // Вспомогательный класс для фракций
    private class Faction
    {
        public int ID;
        public string Name;
        public string Description;
    }
}
