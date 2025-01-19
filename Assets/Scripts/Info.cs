using UnityEngine;
using UnityEngine.UI;
using Mono.Data.Sqlite;
using System.Collections.Generic;
using TMPro;

public class Info : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject factionPanel;      // Панель на Canvas
    public GameObject adminPanel;
    public TMP_Text factionText;         // Текстовый компонент для отображения данных

    private string dbName = "URI=file:Factions.db";

    private bool isDataLoaded = false;   // Флаг для отслеживания загрузки данных

    private void Start()
    {
        factionPanel.SetActive(false);  // Отключаем панель при запуске
    }

    // Метод для отображения фракций
    public void ShowFactions()
    {
        ResetDataLoadedFlag();  // Сбрасываем флаг, чтобы подгрузить свежие данные

        if (!isDataLoaded)
        {
            List<string> factionsInfo = LoadFactionsFromDatabase();

            if (factionsInfo.Count > 0)
            {
                factionText.text = string.Join("\n\n", factionsInfo);
            }
            else
            {
                factionText.text = "Нет доступных данных о фракциях.";
            }

            isDataLoaded = true;
        }

        factionPanel.SetActive(true);
    }

    // Метод для скрытия панели
    public void HideFactionPanel()
    {
        factionPanel.SetActive(false);
    }

    public void ResetDataLoadedFlag()
    {
        isDataLoaded = false;  // Сбрасываем флаг, чтобы при следующем запросе данные подгрузились снова
    }


    public void HideAdminPanel()
    {
        adminPanel.SetActive(false);
    }

    public void EnableAdminPanel()
    {
        adminPanel.SetActive(true);
    }

    // Загрузка фракций из БД
    private List<string> LoadFactionsFromDatabase()
    {
        List<string> factionsList = new List<string>();

        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT name, description FROM factions;";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string name = reader.GetString(0);
                        string description = reader.GetString(1);
                        factionsList.Add($"<b>{name}</b>\n{description}");
                    }
                }
            }
        }

        return factionsList;
    }

    
}
