using UnityEngine;
using System.Collections.Generic; // ��������� ��� ������������� List

public class Inventory : MonoBehaviour
{
    private Drag playerDrag;
    private List<string> basicNames = new List<string>
    {
        "Cube",        // ���
        "Sphere",      // �����
        "Capsule",     // �������
        "Cylinder",    // �������
        "Plane",       // ��������� (�������, � 10x10 ���������)
        "Quad",        // ������� (1x1 �������, ����� ��� UI ��� �������)
        "Terrain",     // �������� (Terrain)
        "Wind Zone",   // ���� ����� (��� ��������/�����)
        "3D Text",     // ���������� 3D-����� (Legacy)
    };

    // �������� ������ �� List, ������� ����� ����������� �����������
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
        Debug.Log($"������� {CheckIfItemNameIsNotBasic(item).name} �������� � ���������");
    }

    public void RemoveFromInventoryNewItem(GameObject item)
    {
        if (item == null) return;

        inventoryItems.Remove(item.name);
        Debug.Log($"������� {item.name} ������ �� ���������");
    }

    public string[] ShowInventory()
    {
        Debug.Log("���������� ��������� ��������");
        return inventoryItems.ToArray();
    }

    public bool FindItemInInventory(GameObject item)
    {
        bool found = inventoryItems.Contains(item.name);
        Debug.Log(found ? $"������� {item.name} ������" : "������� �� ������");
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