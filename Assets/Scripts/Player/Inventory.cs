using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Система инвентаря для управления коллекцией предметов
/// </summary>
public class Inventory : MonoBehaviour
{
    [Header("Настройки инвентаря")]
    [Tooltip("Базовые имена объектов, которые не должны добавляться в инвентарь")]
    [SerializeField]
    private List<string> basicObjectNames = new List<string>
    {
        "Cube",        // Примитив: Куб
        "Sphere",      // Примитив: Сфера
        "Capsule",     // Примитив: Капсула
        "Cylinder",    // Примитив: Цилиндр
        "Plane",       // Примитив: Плоскость
        "Quad",        // Примитив: Квадрат
        "Terrain",     // Компонент ландшафта
        "Wind Zone",   // Компонент ветра
        "3D Text"      // Устаревший 3D текст
    };

    [Tooltip("Предметы в инвентаре")]
    [SerializeField] private List<string> inventoryItems = new List<string>();

    private Drag playerDrag;

    private void Start()
    {
        playerDrag = FindObjectOfType<Drag>();
        if (playerDrag == null)
        {
            Debug.LogWarning("Не найден компонент Drag в сцене");
        }
    }

    /// <summary>
    /// Добавляет предмет в инвентарь после проверки
    /// </summary>
    public void AddToInventory(GameObject item)
    {
        if (item == null)
        {
            Debug.LogWarning("Попытка добавить null объект в инвентарь");
            return;
        }

        GameObject validItem = GetValidItemName(item);
        if (validItem == null) return;

        string itemName = validItem.name;
        if (!inventoryItems.Contains(itemName))
        {
            inventoryItems.Add(itemName);
            Debug.Log($"Предмет '{itemName}' добавлен в инвентарь. Всего предметов: {inventoryItems.Count}");
        }
        else
        {
            Debug.Log($"Предмет '{itemName}' уже есть в инвентаре");
        }
    }

    /// <summary>
    /// Удаляет предмет из инвентаря
    /// </summary>
    public void RemoveFromInventory(GameObject item)
    {
        if (item == null)
        {
            Debug.LogWarning("Попытка удалить null объект из инвентаря");
            return;
        }

        if (inventoryItems.Remove(item.name))
        {
            Debug.Log($"Предмет '{item.name}' удален из инвентаря. Осталось предметов: {inventoryItems.Count}");
        }
        else
        {
            Debug.Log($"Предмет '{item.name}' не найден в инвентаре");
        }
    }

    /// <summary>
    /// Возвращает копию списка предметов в инвентаре
    /// </summary>
    public IReadOnlyList<string> GetInventoryItems()
    {
        Debug.Log($"Запрошено содержимое инвентаря. Всего предметов: {inventoryItems.Count}");
        return inventoryItems.AsReadOnly();
    }

    /// <summary>
    /// Проверяет наличие предмета в инвентаре
    /// </summary>
    public bool HasItem(GameObject item)
    {
        //if (item == null) return false;
        

        bool hasItem = inventoryItems.Contains(item.name);
        Debug.Log("s" + hasItem == null);
        Debug.Log(hasItem ?
            $"Предмет '{item.name}' найден в инвентаре" :
            $"Предмет '{item.name}' отсутствует в инвентаре");

        return hasItem;
    }

    /// <summary>
    /// Проверяет, является ли имя предмета базовым (не добавляемым в инвентарь)
    /// </summary>
    private GameObject GetValidItemName(GameObject item)
    {
        if (item == null) return null;

        // Если это базовый объект, проверяем его родителя
        if (basicObjectNames.Contains(item.name))
        {
            return item.transform.parent != null ? item.transform.parent.gameObject : item;
        }

        return item;
    }

    /// <summary>
    /// Очищает инвентарь полностью
    /// </summary>
    public void ClearInventory()
    {
        Debug.Log($"Инвентарь очищен. Удалено {inventoryItems.Count} предметов");
        inventoryItems.Clear();
    }

    /// <summary>
    /// Проверяет, пуст ли инвентарь
    /// </summary>
    public bool IsEmpty()
    {
        return inventoryItems.Count == 0;
    }
}