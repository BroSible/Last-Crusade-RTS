using System.Collections.Generic;
using Mono.Data.Sqlite;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitDatabaseDisplay : MonoBehaviour
{
    [SerializeField] private UnitDatabaseSO unitDatabaseSO;
    [SerializeField] private TMP_Text inspectorText;
    [SerializeField] private Button saveButton;
    [SerializeField] private TMP_Dropdown idDropdown;

    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_InputField descriptionInput;
    [SerializeField] private TMP_InputField maxHealthInput;
    [SerializeField] private TMP_InputField unitDamageInput;
    [SerializeField] private TMP_InputField speedUnitInput;
    [SerializeField] private TMP_InputField attackingDistanceInput;
    [SerializeField] private TMP_InputField stopAttackingDistanceInput;

    private string dbNameUnits = "URI=file:Units.db"; // Путь к вашей базе данных
    private int currentUnitIndex = 0;

    private void Start()
    {
        LoadDataFromDatabase();
        InitializeDropdown();

        if (saveButton != null)
        {
            saveButton.onClick.AddListener(SaveChanges);
        }

        if (idDropdown != null)
        {
            idDropdown.onValueChanged.AddListener(OnIdDropdownValueChanged);
        }
    }

    private void InitializeDropdown()
    {
        List<string> idOptions = new List<string>();

        // Заполняем список доступных ID
        foreach (var unit in unitDatabaseSO.units)
        {
            idOptions.Add(unit.ID.ToString());
        }

        idDropdown.ClearOptions();
        idDropdown.AddOptions(idOptions);
    }

    private void OnIdDropdownValueChanged(int index)
    {
        currentUnitIndex = index;
    }

    private void LoadDataFromDatabase()
    {
        using (var connection = new SqliteConnection(dbNameUnits))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT ID, Name, Description, MaxHealth, UnitDamage, SpeedUnit, AttackingDistance, StopAttackingDistance FROM units;";
                using (var reader = command.ExecuteReader())
                {
                    int idIndex = reader.GetOrdinal("ID");
                    int nameIndex = reader.GetOrdinal("Name");
                    int descriptionIndex = reader.GetOrdinal("Description");
                    int maxHealthIndex = reader.GetOrdinal("MaxHealth");
                    int unitDamageIndex = reader.GetOrdinal("UnitDamage");
                    int speedUnitIndex = reader.GetOrdinal("SpeedUnit");
                    int attackingDistanceIndex = reader.GetOrdinal("AttackingDistance");
                    int stopAttackingDistanceIndex = reader.GetOrdinal("StopAttackingDistance");

                    string data = "<mspace=0.7em><b>| ID  | Имя        | Здоровье | Урон  | Скорость | Дист. Атаки | Стоп Атака |</b>\n";
                    data += "<b>|-----|------------|----------|-------|----------|-------------|------------|</b>\n";

                    while (reader.Read())
                    {
                        string id = reader.IsDBNull(idIndex) ? "N/A" : reader.GetInt32(idIndex).ToString();
                        string name = reader.IsDBNull(nameIndex) ? "N/A" : reader.GetString(nameIndex);
                        string description = reader.IsDBNull(descriptionIndex) ? "N/A" : reader.GetString(descriptionIndex);
                        string maxHealth = reader.IsDBNull(maxHealthIndex) ? "N/A" : reader.GetInt32(maxHealthIndex).ToString();
                        string unitDamage = reader.IsDBNull(unitDamageIndex) ? "N/A" : reader.GetInt32(unitDamageIndex).ToString();
                        string speedUnit = reader.IsDBNull(speedUnitIndex) ? "N/A" : reader.GetInt32(speedUnitIndex).ToString();
                        string attackingDistance = reader.IsDBNull(attackingDistanceIndex) ? "N/A" : reader.GetFloat(attackingDistanceIndex).ToString();
                        string stopAttackingDistance = reader.IsDBNull(stopAttackingDistanceIndex) ? "N/A" : reader.GetFloat(stopAttackingDistanceIndex).ToString();

                        data += $"| {reader["id"],-3} | {reader["name"],-10} | {reader["maxHealth"],8} | {reader["unitDamage"],5} | {reader["speedUnit"],8} | {reader["attackingDistance"],11:F1} | {reader["stopAttackingDistance"],10:F1} |\n";
                    }
                    data += "</mspace>";
                    inspectorText.text = data;
                }
            }
        }
    }

    private void SaveChanges()
    {
        if (unitDatabaseSO == null || unitDatabaseSO.units.Count == 0 || currentUnitIndex >= unitDatabaseSO.units.Count)
            return;

        var unit = unitDatabaseSO.units[currentUnitIndex];

        unit.Name = !string.IsNullOrEmpty(nameInput.text) ? nameInput.text : unit.Name;
        unit.description = !string.IsNullOrEmpty(descriptionInput.text) ? descriptionInput.text : unit.description;

        if (int.TryParse(maxHealthInput.text, out int maxHealth))
            unit.maxHealth = maxHealth;

        if (int.TryParse(unitDamageInput.text, out int unitDamage))
            unit.unitDamage = unitDamage;

        if (int.TryParse(speedUnitInput.text, out int speedUnit))
            unit.speedUnit = speedUnit;

        if (float.TryParse(attackingDistanceInput.text, out float attackingDistance))
            unit.attackingDistance = attackingDistance;

        if (float.TryParse(stopAttackingDistanceInput.text, out float stopAttackingDistance))
            unit.stopAttackingDistance = stopAttackingDistance;

        SaveToDatabase(unit);
        LoadDataFromDatabase();  // Перезагружаем данные после сохранения изменений
        Debug.Log("Изменения успешно сохранены.");
    }

    private void SaveToDatabase(UnitData unit)
    {
        using (var connection = new SqliteConnection(dbNameUnits))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                // Подготавливаем SQL-запрос для обновления данных юнита в базе данных
                command.CommandText = @"
                    UPDATE units
                    SET Name = @name,
                        Description = @description,
                        MaxHealth = @maxHealth,
                        UnitDamage = @unitDamage,
                        SpeedUnit = @speedUnit,
                        AttackingDistance = @attackingDistance,
                        StopAttackingDistance = @stopAttackingDistance
                    WHERE ID = @id;";

                // Добавляем параметры в запрос для предотвращения SQL-инъекций
                command.Parameters.AddWithValue("@id", unit.ID);
                command.Parameters.AddWithValue("@name", unit.Name);
                command.Parameters.AddWithValue("@description", unit.description);
                command.Parameters.AddWithValue("@maxHealth", unit.maxHealth);
                command.Parameters.AddWithValue("@unitDamage", unit.unitDamage);
                command.Parameters.AddWithValue("@speedUnit", unit.speedUnit);
                command.Parameters.AddWithValue("@attackingDistance", unit.attackingDistance);
                command.Parameters.AddWithValue("@stopAttackingDistance", unit.stopAttackingDistance);

                // Выполняем запрос
                command.ExecuteNonQuery();
            }
        }
    }
}
