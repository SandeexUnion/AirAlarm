using UnityEngine;
using System.Collections.Generic; // Добавляем для использования List

public class Inventory : MonoBehaviour
{
    private Drag playerDrag;
    private List<string> basicNames = new List<string>
    {
        "Cube",        // Куб
        "Sphere",      // Сфера
        "Capsule",     // Капсула
        "Cylinder",    // Цилиндр
        "Plane",       // Плоскость (большая, с 10x10 полигонов)
        "Quad",        // Квадрат (1x1 полигон, часто для UI или декалей)
        "Terrain",     // Ландшафт (Terrain)
        "Wind Zone",   // Зона ветра (для деревьев/травы)
        "3D Text",     // Устаревший 3D-текст (Legacy)
    };

    // Заменяем массив на List, который может динамически расширяться
    [SerializeField]
    private List<string> inventoryItems = new List<string>();

    void Start()
    {
        playerDrag = Object.FindFirstObjectByType<Drag>();
    }

    public void AddToInventoryNewItem(GameObject item)
    {
        if (item == null) return;

        inventoryItems.Add(CheckIfItemNameIsNotBasic(item).name);
        Debug.Log($"Предмет {CheckIfItemNameIsNotBasic(item).name} добавлен в инвентарь");
    }

    public void RemoveFromInventoryNewItem(GameObject item)
    {
        if (item == null) return;

        inventoryItems.Remove(item.name);
        Debug.Log($"Предмет {item.name} удален из инвентаря");
    }

    public string[] ShowInventory()
    {
        Debug.Log("Содержимое инвентаря выведено");
        return inventoryItems.ToArray();
    }

    public bool FindItemInInventory(GameObject item)
    {
        bool found = inventoryItems.Contains(item.name);
        Debug.Log(found ? $"Предмет {item.name} найден" : "Предмет не найден");
        return found;
    }

    private GameObject CheckIfItemNameIsNotBasic(GameObject item)
    {
        if (item == null) return null;

        if (basicNames.Contains(item.name))
        {
            if (item.transform.parent != null)
            {
                return item.transform.parent.gameObject;
            }
            return item; 
        }
        return item;
    }
}