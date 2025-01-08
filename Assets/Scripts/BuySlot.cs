using UnityEngine;
using UnityEngine.UI;

public class BuySlot : MonoBehaviour
{
    public bool isAvailable;

    public BuyingSystem buyingSystem;

    public int databaseItemId;

    private Button button;
    private Image buttonImage;
    private Color originalColor;
    private Color unavailableColor = new Color(0.5f, 0.5f, 0.5f, 1f); // Серый цвет

    void Start()
    {
        button = GetComponent<Button>();
        buttonImage = GetComponent<Image>();

        if (buttonImage != null)
        {
            originalColor = buttonImage.color; // Сохраняем исходный цвет кнопки
        }

        UpdateAvailabilityUI();
    }

    public void ClickedOnSlot()
    {
        if (isAvailable)
        {
            buyingSystem.placementSystem.StartPlacement(databaseItemId);
        }
    }

    private void UpdateAvailabilityUI()
    {
        if (isAvailable)
        {
            if (button != null)
            {
                button.interactable = true; // Разрешаем взаимодействие
            }

            if (buttonImage != null)
            {
                buttonImage.color = originalColor; // Восстанавливаем оригинальный цвет
            }
        }
        else
        {
            if (button != null)
            {
                button.interactable = false; // Запрещаем взаимодействие
            }

            if (buttonImage != null)
            {
                buttonImage.color = unavailableColor; // Устанавливаем серый цвет
            }
        }
    }
}
