using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitDatabaseDisplay : MonoBehaviour
{
    [SerializeField] private UnitDatabaseSO unitDatabaseSO;  // Ссылка на ScriptableObject
    [SerializeField] private TMP_Text inspectorText;         // Текстовое поле для отображения
    [SerializeField] private Button saveButton;              // Кнопка для сохранения изменений
    [SerializeField] private TMP_Dropdown idDropdown;        // Выпадающий список для выбора ID

    // Поля ввода для редактирования данных
    [SerializeField] private TMP_InputField nameInput;               // Поле для изменения имени
    [SerializeField] private TMP_InputField descriptionInput;         // Поле для изменения описания
    [SerializeField] private TMP_InputField maxHealthInput;           // Поле для изменения здоровья
    [SerializeField] private TMP_InputField unitDamageInput;          // Поле для изменения урона
    [SerializeField] private TMP_InputField speedUnitInput;           // Поле для изменения скорости
    [SerializeField] private TMP_InputField attackingDistanceInput;   // Поле для изменения дистанции атаки
    [SerializeField] private TMP_InputField stopAttackingDistanceInput; // Поле для изменения дистанции остановки атаки

    private int currentUnitIndex = 0; // Индекс текущего выбранного юнита

    private void Start()
    {
        if (unitDatabaseSO != null && unitDatabaseSO.units.Count > 0)
        {
            InitializeDropdown();
            ShowUnitData();  // Показываем таблицу сразу при старте
            LoadDataIntoFields();  // Загружаем данные для редактирования первого юнита
        }

        if (saveButton != null)
        {
            saveButton.onClick.AddListener(SaveChanges);
        }

        if (idDropdown != null)
        {
            idDropdown.onValueChanged.AddListener(OnIdDropdownValueChanged);
        }
    }

    // Инициализация выпадающего списка для выбора ID
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

    // Выбор ID из выпадающего списка
    private void OnIdDropdownValueChanged(int index)
    {
        currentUnitIndex = index;
        LoadDataIntoFields();  // Загружаем данные для выбранного юнита
    }

    // Отображение данных юнитов в текстовом поле
    private void ShowUnitData()
    {
        if (unitDatabaseSO == null || unitDatabaseSO.units == null || unitDatabaseSO.units.Count == 0)
        {
            inspectorText.text = "Нет данных о юнитах.";
            return;
        }

        string data = "";

        data += $"<mspace=0.7em><b>| ID  | Имя        | Здоровье | Урон  | Скорость | Дист. Атаки | Стоп Атака |</b>\n";
        data += $"<b>|-----|------------|----------|-------|----------|-------------|------------|</b>\n";

        foreach (var unit in unitDatabaseSO.units)
        {
            data += $"| {unit.ID,-3} | {unit.Name,-10} | {unit.maxHealth,8} | {unit.unitDamage,5} | {unit.speedUnit,8} | {unit.attackingDistance,11:F1} | {unit.stopAttackingDistance,10:F1} |\n";
        }

        data += "</mspace>";
        inspectorText.text = data;
    }

    // Загружает данные из ScriptableObject в InputFields для редактирования
    private void LoadDataIntoFields()
    {
        if (unitDatabaseSO == null || unitDatabaseSO.units.Count == 0 || currentUnitIndex >= unitDatabaseSO.units.Count)
            return;

        var unit = unitDatabaseSO.units[currentUnitIndex];  // Выбираем текущий юнит по индексу

        // Оставляем поля ввода пустыми, без присваивания значений
        // Например, если вы хотите оставить их пустыми, просто пропустите присваивание:
        // nameInput.text = unit.Name; // Не нужно это присваивать, если не хотите заполнять.
        // Или наоборот, если хотите, чтобы юнит не изменял поля, это тоже будет возможно.
    }


    // Сохранение изменений из InputFields в ScriptableObject
    private void SaveChanges()
    {
        if (unitDatabaseSO == null || unitDatabaseSO.units.Count == 0 || currentUnitIndex >= unitDatabaseSO.units.Count)
            return;

        var unit = unitDatabaseSO.units[currentUnitIndex];  // Выбираем текущий юнит по индексу

        // Сохраняем только те данные, которые не пустые
        unit.Name = !string.IsNullOrEmpty(nameInput.text) ? nameInput.text : unit.Name;
        unit.description = !string.IsNullOrEmpty(descriptionInput.text) ? descriptionInput.text : unit.description;
        
        if (int.TryParse(maxHealthInput.text, out int maxHealth) && !string.IsNullOrEmpty(maxHealthInput.text))
            unit.maxHealth = maxHealth;
        
        if (int.TryParse(unitDamageInput.text, out int unitDamage) && !string.IsNullOrEmpty(unitDamageInput.text))
            unit.unitDamage = unitDamage;
        
        if (int.TryParse(speedUnitInput.text, out int speedUnit) && !string.IsNullOrEmpty(speedUnitInput.text))
            unit.speedUnit = speedUnit;
        
        if (float.TryParse(attackingDistanceInput.text, out float attackingDistance) && !string.IsNullOrEmpty(attackingDistanceInput.text))
            unit.attackingDistance = attackingDistance;
        
        if (float.TryParse(stopAttackingDistanceInput.text, out float stopAttackingDistance) && !string.IsNullOrEmpty(stopAttackingDistanceInput.text))
            unit.stopAttackingDistance = stopAttackingDistance;

        ShowUnitData();  // Показываем обновленную таблицу после сохранения изменений
        Debug.Log("Изменения успешно сохранены.");
    }
}
