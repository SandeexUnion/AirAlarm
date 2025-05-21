using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// Класс для управления взаимодействиями с объектами (захват, бросок, использование)
/// </summary>
public class Drag : MonoBehaviour
{
    [Header("Основные компоненты")]
    [SerializeField] private PlayableDirector timeline;
    [SerializeField] private BlinkEffect blinkEffect;
    [SerializeField] private GameObject playerCam;
    [SerializeField] private CrosshairGUI crosshairGUI;

    [Header("Настройки управления")]
    [SerializeField] private string grabButton = "Grab";
    [SerializeField] private string throwButton = "Throw";
    [SerializeField] private string useButton = "Use";
    [SerializeField] private float reducedMouseSensitivity = 0.1f;
    [SerializeField] private float maxDistanceGrab = 1.5f;

    [Header("Теги для взаимодействий")]
    [SerializeField] public InteractionTags tags = new InteractionTags();

    private Inventory inventorySystem;
    private Interaction currentInteraction;
    private GameObject objectHeld;
    private bool isObjectHeld;
    private bool tryPickupObject;
    private bool IsDoorLocked;

    [System.Serializable]
    public class InteractionTags
    {
        public string interactTag = "Interact";
        public string itemTag = "InteractItem";
        public string doorTag = "Door";
        public string lockedDoorTag = "LockedDoor";
    }

    private void Start()
    {
        inventorySystem = GetComponent<Inventory>();
    }

    private void Update()
    {
        HandleInput();
    }

    #region Обработка ввода

    /// <summary>
    /// Обрабатывает все вводимые команды взаимодействия
    /// </summary>
    private void HandleInput()
    {
        HandleGrabInput();
        HandleThrowInput();
        HandleUseInput();
    }

    private void HandleGrabInput()
    {
        if (Input.GetButton(grabButton))
        {
            if (!isObjectHeld)
            {
                TryPickObject();
                tryPickupObject = true;
            }
            else
            {
                HoldObject();
            }
        }
        else if (isObjectHeld)
        {
            DropObject();
        }
    }

    private void HandleThrowInput()
    {
        if (Input.GetButton(throwButton) && isObjectHeld)
        {
            ThrowObject();
        }
    }

    private void HandleUseInput()
    {
        if (Input.GetButtonDown(useButton) && isObjectHeld)
        {
            Use();
        }
    }

    #endregion

    #region Взаимодействие с объектами

    /// <summary>
    /// Пытается подобрать объект перед игроком
    /// </summary>
    private void TryPickObject()
    {
        Ray playerAim = playerCam.GetComponent<Camera>().ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(playerAim, out RaycastHit hit, maxDistanceGrab))
        {
            HandleSpecialCases(hit);
            HandleRegularObjects(hit);
        }
    }

    private void HandleSpecialCases(RaycastHit hit)
    {
        // Обработка кровати (специальный случай)
        if (hit.collider.gameObject.name == "Old_wood_bed" && hit.collider.CompareTag(tags.interactTag))
        {
            HandleBedInteraction(hit);
            blinkEffect.CloseEyes = true;
            blinkEffect.Blink();
            return;
        }

        // Для дверей - особые условия
        if (hit.collider.CompareTag(tags.doorTag) || hit.collider.CompareTag(tags.lockedDoorTag))
        {
            HandleDoorInteraction(hit);
        }
    }

    private void HandleRegularObjects(RaycastHit hit)
    {
        if (hit.collider.CompareTag(tags.interactTag) || hit.collider.CompareTag(tags.itemTag))
        {
            objectHeld = hit.collider.gameObject;
            currentInteraction = InteractionFactory.CreateInteraction(hit.collider.tag);

            if (currentInteraction != null)
            {
                isObjectHeld = true;
                currentInteraction.OnGrab(objectHeld.GetComponent<Rigidbody>());
            }
        }
    }

    private void HandleDoorInteraction(RaycastHit hit)
    {
        if (!tryPickupObject) return;

        float distanceToPlayer = Vector3.Distance(hit.point, playerCam.transform.position);
        if (distanceToPlayer > 1.5f) return;

        // Проверяем, есть ли у объекта тег "LockedDoor" (или другой признак блокировки)
        if (hit.collider.CompareTag("LockedDoor"))
        {

            crosshairGUI.isLookingAtLockedDoor = true;
            // Проверяем наличие ключа в инвентаре
            // Предполагаем, что ключ имеет имя "Key" или другое уникальное имя
            bool hasKey = inventorySystem.HasItem(GameObject.Find("Key"));
            Debug.Log(hasKey);
           // Или inventorySystem.inventoryItems.Contains("Key")

            if (!hasKey)
            {
                Debug.Log("Дверь заблокирована, нужен ключ!");
                return;
            }
            else
            {
                Debug.Log("Дверь открыта ключом!");
                hit.collider.tag = tags.doorTag; // Меняем тег на обычную дверь
            }
        }

        objectHeld = hit.collider.gameObject;
        currentInteraction = new DoorInteraction();
        isObjectHeld = true;
        currentInteraction.OnGrab(objectHeld.GetComponent<Rigidbody>());
    }

    private void HandleBedInteraction(RaycastHit hit)
    {
        crosshairGUI.isLookingAtInteractable = true;
        if (blinkEffect != null) blinkEffect.CloseEyes = true;
        hit.collider.gameObject.tag = "Untagged";
        StartCoroutine(WaitAndExecute());
    }

    private IEnumerator WaitAndExecute()
    {
        yield return new WaitForSeconds(4f);
        if (blinkEffect != null) blinkEffect.CloseEyes = false;
        if (timeline != null) timeline.Play();
        GetComponent<AudioManager>()?.PlayNextTrack();
    }

    /// <summary>
    /// Удерживает объект перед игроком
    /// </summary>
    private void HoldObject()
    {
        if (currentInteraction == null) return;

        Ray playerAim = playerCam.GetComponent<Camera>().ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Vector3 nextPos = playerCam.transform.position + playerAim.direction * currentInteraction.HoldDistance;
        Vector3 currPos = objectHeld.transform.position;

        objectHeld.GetComponent<Rigidbody>().linearVelocity = (nextPos - currPos) * 10;

        if (Vector3.Distance(objectHeld.transform.position, playerCam.transform.position) > currentInteraction.MaxGrabDistance)
        {
            DropObject();
        }
    }

    /// <summary>
    /// Бросает удерживаемый объект
    /// </summary>
    private void ThrowObject()
    {
        if (!isObjectHeld) return;

        isObjectHeld = false;
        currentInteraction?.OnThrow(objectHeld.GetComponent<Rigidbody>(), playerCam.transform.forward);
        objectHeld = null;
        currentInteraction = null;
    }

    /// <summary>
    /// Использует удерживаемый объект
    /// </summary>
    private void Use()
    {
        if (!isObjectHeld || objectHeld == null) return;

        if (objectHeld.CompareTag(tags.itemTag) || objectHeld.CompareTag(tags.interactTag))
        {
            inventorySystem.AddToInventory(objectHeld);
            isObjectHeld = false;

            objectHeld.SendMessage("UseObject", SendMessageOptions.DontRequireReceiver);
            Destroy(objectHeld);

            objectHeld = null;
            currentInteraction = null;
            Debug.Log("Предмет добавлен в инвентарь");
        }
    }

    /// <summary>
    /// Отпускает удерживаемый объект
    /// </summary>
    private void DropObject()
    {
        if (!isObjectHeld) return;

        isObjectHeld = false;
        tryPickupObject = false;
        currentInteraction?.OnRelease(objectHeld.GetComponent<Rigidbody>());
        objectHeld = null;
        currentInteraction = null;
    }

    #endregion
}