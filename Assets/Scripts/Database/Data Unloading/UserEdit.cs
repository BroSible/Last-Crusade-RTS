using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Mono.Data.Sqlite;

public class UserEdit : MonoBehaviour
{
    [SerializeField] private TMP_Text inspectorText;      // Поле для отображения статистики пользователей
    [SerializeField] private TMP_Dropdown idDropdown;     // Выпадающий список для выбора записи
    [SerializeField] private TMP_InputField usernameInput; // Поле для редактирования имени пользователя
    [SerializeField] private TMP_InputField passwordInput; // Поле для редактирования пароля
    [SerializeField] private Button saveButton;           // Кнопка сохранения изменений
    [SerializeField] private Button addButton;            // Кнопка добавления новой записи
    [SerializeField] private Button deleteButton;         // Кнопка удаления записи

    private string dbName = "URI=file:Users.db";
    private List<UserRecord> users = new List<UserRecord>();
    private int currentUserIndex = 0;

    private void Start()
    {
        LoadUsersFromDatabase();
        InitializeDropdown();
        ShowUsersData();

        saveButton.onClick.AddListener(SaveChanges);
        addButton.onClick.AddListener(AddUserRecord);
        deleteButton.onClick.AddListener(DeleteUserRecord);
        idDropdown.onValueChanged.AddListener(OnIdDropdownValueChanged);
    }

    // Загрузка всех пользователей из базы данных
    private void LoadUsersFromDatabase()
    {
        users.Clear();

        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT id, username, password FROM users;";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new UserRecord
                        {
                            ID = reader.GetInt32(0),
                            Username = reader.GetString(1),
                            Password = reader.GetString(2)
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
        foreach (var user in users)
        {
            idOptions.Add($"{user.ID}");
        }

        idDropdown.ClearOptions();
        idDropdown.AddOptions(idOptions);

        if (idOptions.Count > 0)
        {
            idDropdown.value = 0;
            OnIdDropdownValueChanged(0); // Синхронизация с начальным значением
        }
    }

    // Отображение всех записей пользователей в текстовом поле
    private void ShowUsersData()
    {
        string table = "<mspace=0.7em><b>| ID  | Username  | Password   |</b>\n";
        table += "<b>|-----|-----------|------------|</b>\n";

        foreach (var user in users)
        {
            table += $"| {user.ID,-3} | {user.Username,-9} | {user.Password,-10} |\n";
        }

        table += "</mspace>";
        inspectorText.text = table;
    }

    // Обработка изменения выбора в выпадающем списке
    private void OnIdDropdownValueChanged(int index)
    {
        if (index < 0 || index >= users.Count) return;

        currentUserIndex = index;

        var user = users[currentUserIndex];
        usernameInput.text = user.Username;
        passwordInput.text = user.Password; // показываем текущий пароль
    }

    // Сохранение изменений в базу данных
    private void SaveChanges()
    {
        if (users.Count == 0 || currentUserIndex >= users.Count) return;

        var user = users[currentUserIndex];
        string newUsername = usernameInput.text;
        string newPassword = passwordInput.text;

        if (string.IsNullOrEmpty(newUsername) || string.IsNullOrEmpty(newPassword))
        {
            Debug.LogWarning("Поля не могут быть пустыми.");
            return;
        }

        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    UPDATE users
                    SET username = @username, password = @password
                    WHERE id = @id;";
                command.Parameters.AddWithValue("@username", newUsername);
                command.Parameters.AddWithValue("@password", newPassword);
                command.Parameters.AddWithValue("@id", user.ID);

                command.ExecuteNonQuery();
            }
        }

        // Обновляем локальные данные и интерфейс
        user.Username = newUsername;
        user.Password = newPassword;
        users[currentUserIndex] = user;
        ShowUsersData();
        Debug.Log($"Пользователь ID {user.ID} успешно обновлен.");
    }

    // Добавление нового пользователя
    private void AddUserRecord()
    {
        string newUsername = usernameInput.text;
        string newPassword = passwordInput.text;

        if (string.IsNullOrEmpty(newUsername) || string.IsNullOrEmpty(newPassword))
        {
            Debug.LogWarning("Поля не могут быть пустыми.");
            return;
        }

        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    INSERT INTO users (username, password) VALUES (@username, @password);
                    SELECT last_insert_rowid();";
                command.Parameters.AddWithValue("@username", newUsername);
                command.Parameters.AddWithValue("@password", newPassword);

                int newId = (int)(long)command.ExecuteScalar();

                // Добавляем нового пользователя в локальный список
                users.Add(new UserRecord { ID = newId, Username = newUsername, Password = newPassword });
                InitializeDropdown();
                ShowUsersData();
                Debug.Log($"Пользователь успешно добавлен с ID {newId}.");
            }
        }
    }

    // Удаление текущего пользователя
    private void DeleteUserRecord()
    {
        if (users.Count == 0 || currentUserIndex >= users.Count) return;

        var user = users[currentUserIndex];

        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "DELETE FROM users WHERE id = @id;";
                command.Parameters.AddWithValue("@id", user.ID);

                command.ExecuteNonQuery();
            }
        }

        // Удаляем пользователя из локального списка
        users.RemoveAt(currentUserIndex);
        InitializeDropdown();
        ShowUsersData();

        if (users.Count > 0)
            currentUserIndex = 0;
        else
            currentUserIndex = -1;

        Debug.Log($"Пользователь ID {user.ID} успешно удален.");
    }

    // Вспомогательный класс для пользователя
    private class UserRecord
    {
        public int ID;
        public string Username;
        public string Password;
    }
}
