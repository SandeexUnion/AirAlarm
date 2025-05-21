using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// ������� ��������� ��� ���������� ���������� ���������
/// </summary>
public class Inventory : MonoBehaviour
{
    [Header("��������� ���������")]
    [Tooltip("������� ����� ��������, ������� �� ������ ����������� � ���������")]
    [SerializeField]
    private List<string> basicObjectNames = new List<string>
    {
        "Cube",        // ��������: ���
        "Sphere",      // ��������: �����
        "Capsule",     // ��������: �������
        "Cylinder",    // ��������: �������
        "Plane",       // ��������: ���������
        "Quad",        // ��������: �������
        "Terrain",     // ��������� ���������
        "Wind Zone",   // ��������� �����
        "3D Text"      // ���������� 3D �����
    };

    [Tooltip("�������� � ���������")]
    [SerializeField] private List<string> inventoryItems = new List<string>();

    private Drag playerDrag;

    private void Start()
    {
        playerDrag = FindObjectOfType<Drag>();
        if (playerDrag == null)
        {
            Debug.LogWarning("�� ������ ��������� Drag � �����");
        }
    }

    /// <summary>
    /// ��������� ������� � ��������� ����� ��������
    /// </summary>
    public void AddToInventory(GameObject item)
    {
        if (item == null)
        {
            Debug.LogWarning("������� �������� null ������ � ���������");
            return;
        }

        GameObject validItem = GetValidItemName(item);
        if (validItem == null) return;

        string itemName = validItem.name;
        if (!inventoryItems.Contains(itemName))
        {
            inventoryItems.Add(itemName);
            Debug.Log($"������� '{itemName}' �������� � ���������. ����� ���������: {inventoryItems.Count}");
        }
        else
        {
            Debug.Log($"������� '{itemName}' ��� ���� � ���������");
        }
    }

    /// <summary>
    /// ������� ������� �� ���������
    /// </summary>
    public void RemoveFromInventory(GameObject item)
    {
        if (item == null)
        {
            Debug.LogWarning("������� ������� null ������ �� ���������");
            return;
        }

        if (inventoryItems.Remove(item.name))
        {
            Debug.Log($"������� '{item.name}' ������ �� ���������. �������� ���������: {inventoryItems.Count}");
        }
        else
        {
            Debug.Log($"������� '{item.name}' �� ������ � ���������");
        }
    }

    /// <summary>
    /// ���������� ����� ������ ��������� � ���������
    /// </summary>
    public IReadOnlyList<string> GetInventoryItems()
    {
        Debug.Log($"��������� ���������� ���������. ����� ���������: {inventoryItems.Count}");
        return inventoryItems.AsReadOnly();
    }

    /// <summary>
    /// ��������� ������� �������� � ���������
    /// </summary>
    public bool HasItem(GameObject item)
    {
        //if (item == null) return false;
        

        bool hasItem = inventoryItems.Contains(item.name);
        Debug.Log("s" + hasItem == null);
        Debug.Log(hasItem ?
            $"������� '{item.name}' ������ � ���������" :
            $"������� '{item.name}' ����������� � ���������");

        return hasItem;
    }

    /// <summary>
    /// ���������, �������� �� ��� �������� ������� (�� ����������� � ���������)
    /// </summary>
    private GameObject GetValidItemName(GameObject item)
    {
        if (item == null) return null;

        // ���� ��� ������� ������, ��������� ��� ��������
        if (basicObjectNames.Contains(item.name))
        {
            return item.transform.parent != null ? item.transform.parent.gameObject : item;
        }

        return item;
    }

    /// <summary>
    /// ������� ��������� ���������
    /// </summary>
    public void ClearInventory()
    {
        Debug.Log($"��������� ������. ������� {inventoryItems.Count} ���������");
        inventoryItems.Clear();
    }

    /// <summary>
    /// ���������, ���� �� ���������
    /// </summary>
    public bool IsEmpty()
    {
        return inventoryItems.Count == 0;
    }
}