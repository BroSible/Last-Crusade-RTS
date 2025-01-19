using System.Collections.Generic;
using Mono.Data.Sqlite;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingDatabaseDisplay : MonoBehaviour
{
    [SerializeField] private ObjectsDatabseSO objectsDatabaseSO;
    [SerializeField] private TMP_Text inspectorText;
    [SerializeField] private Button saveButton;
    [SerializeField] private TMP_Dropdown idDropdown;

    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_InputField descriptionInput;
    [SerializeField] private TMP_InputField sizeXInput;
    [SerializeField] private TMP_InputField sizeYInput;
    [SerializeField] private TMP_InputField costInput;

    private string dbNameBuildings = "URI=file:Buildings.db";
    private int currentBuildingIndex = 0;

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
            idDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        }
    }

    private void InitializeDropdown()
    {
        List<string> idOptions = new List<string>();
        
        // Заполняем список доступных ID
        foreach (var unit in objectsDatabaseSO.objectsData)
        {
            idOptions.Add(unit.ID.ToString());
        }

        idDropdown.ClearOptions();
        idDropdown.AddOptions(idOptions);
    }

    private void OnDropdownValueChanged(int index)
    {
        currentBuildingIndex = index;
    }

    private void LoadDataFromDatabase()
    {
        using (var connection = new SqliteConnection(dbNameBuildings))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT ID, Name, Description, SizeX, SizeY, Cost FROM ObjectData;";
                using (var reader = command.ExecuteReader())
                {
                    int idIndex = reader.GetOrdinal("ID");
                    int nameIndex = reader.GetOrdinal("Name");
                    int descriptionIndex = reader.GetOrdinal("Description");
                    int sizeXIndex = reader.GetOrdinal("SizeX");
                    int sizeYIndex = reader.GetOrdinal("SizeY");
                    int costIndex = reader.GetOrdinal("Cost");

                    string data = "<mspace=0.7em><b>| ID  | Name       | Description      | SizeX | SizeY | Cost |</b>\n";
                    data += "<b>|-----|------------|-----------------|-------|-------|------|</b>\n";

                    while (reader.Read())
                    {
                        string id = reader.IsDBNull(idIndex) ? "N/A" : reader.GetInt32(idIndex).ToString();
                        string name = reader.IsDBNull(nameIndex) ? "N/A" : reader.GetString(nameIndex);
                        string description = reader.IsDBNull(descriptionIndex) ? "N/A" : reader.GetString(descriptionIndex);
                        string sizeX = reader.IsDBNull(sizeXIndex) ? "N/A" : reader.GetInt32(sizeXIndex).ToString();
                        string sizeY = reader.IsDBNull(sizeYIndex) ? "N/A" : reader.GetInt32(sizeYIndex).ToString();
                        string cost = reader.IsDBNull(costIndex) ? "N/A" : reader.GetInt32(costIndex).ToString();

                        data += $"| {id,-3} | {name,-10} | {description,-15} | {sizeX,-5} | {sizeY,-5} | {cost,-4} |\n";
                    }
                    data += "</mspace>";
                    inspectorText.text = data;
                }
            }
        }
    }


    private void SaveChanges()
    {
        if (objectsDatabaseSO == null || objectsDatabaseSO.objectsData.Count == 0 || currentBuildingIndex >= objectsDatabaseSO.objectsData.Count)
            return;

        var building = objectsDatabaseSO.objectsData[currentBuildingIndex];

        building.Name = !string.IsNullOrEmpty(nameInput.text) ? nameInput.text : building.Name;
        building.description = !string.IsNullOrEmpty(descriptionInput.text) ? descriptionInput.text : building.description;

        if (int.TryParse(sizeXInput.text, out int sizeX))
            building.Size = new Vector2Int(sizeX, building.Size.y);

        if (int.TryParse(sizeYInput.text, out int sizeY))
            building.Size = new Vector2Int(building.Size.x, sizeY);

        if (int.TryParse(costInput.text, out int cost))
        {
            if (building.requirements.Count > 0)
            {
                building.requirements[0].amount = cost;
            }
        }

        SaveToDatabase(building);
        LoadDataFromDatabase();
        //InitializeDropdown();
        Debug.Log("Изменения успешно сохранены.");
    }

    private void SaveToDatabase(ObjectData building)
    {
        using (var connection = new SqliteConnection(dbNameBuildings))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    UPDATE ObjectData
                    SET Name = @name,
                        Description = @description,
                        SizeX = @sizeX,
                        SizeY = @sizeY,
                        Cost = @cost
                    WHERE ID = @id;";

                command.Parameters.AddWithValue("@id", building.ID);
                command.Parameters.AddWithValue("@name", building.Name);
                command.Parameters.AddWithValue("@description", building.description);
                command.Parameters.AddWithValue("@sizeX", building.Size.x);
                command.Parameters.AddWithValue("@sizeY", building.Size.y);

                int totalCost = 0;
                foreach (var req in building.requirements)
                {
                    totalCost += req.amount;
                }
                command.Parameters.AddWithValue("@cost", totalCost);

                command.ExecuteNonQuery();
            }
        }
    }
}
