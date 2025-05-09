using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class Drag : MonoBehaviour
{
    public PlayableDirector timeline;
    public BlinkEffect bf;
    public GameObject playerCam;
    public string GrabButton = "Grab";
    public string ThrowButton = "Throw";
    public string UseButton = "Use";
    public float reducedMouseSensitivity = 0.1f;
    public CrosshairGUI crosshairGUI; 

    private Inventory inventorySystem;
    private Interaction currentInteraction;
    private GameObject objectHeld;
    private bool isObjectHeld;
    private bool tryPickupObject;
    private float maxDistanceGrab = 1.5f;
    [System.Serializable]
    public class InteractionTags
    {
        public string interactTag = "Interact";
        public string itemTag = "InteractItem";
        public string doorTag = "Door";
    }

    public InteractionTags tags = new InteractionTags();

    private void Start()
    {
        inventorySystem = GetComponent<Inventory>();
    }
    private void HandleInput()
    {
        if (Input.GetButton(GrabButton))
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

        if (Input.GetButton(ThrowButton) && isObjectHeld)
        {
            ThrowObject();
        }

        if (Input.GetButtonDown(UseButton) && isObjectHeld)
        {

            Use();
        }
    }
    void Update()
    {
        HandleInput();
    }

    private void TryPickObject()
    {
        Ray playerAim = playerCam.GetComponent<Camera>().ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(playerAim, out hit, maxDistanceGrab))
        {
            // Обработка кровати (специальный случай)
            if (hit.collider.gameObject.name == "Old_wood_bed" && hit.collider.CompareTag(tags.interactTag))
            {
                HandleBedInteraction(hit);
                bf.CloseEyes = true;
                bf.Blink();
                return;
            }

            // Для дверей - особые условия
            if (hit.collider.CompareTag(tags.doorTag) && tryPickupObject)
            {
                float distanceToPlayer = Vector3.Distance(hit.point, playerCam.transform.position);
                if (distanceToPlayer > 1.5f) return; // Максимальная дистанция для дверей

                objectHeld = hit.collider.gameObject;
                currentInteraction = new DoorInteraction(); // Создаем специальное взаимодействие для двери

                isObjectHeld = true;
                currentInteraction.OnGrab(objectHeld.GetComponent<Rigidbody>());
                return;
            }

            // Обычные объекты
            if (hit.collider.CompareTag(tags.interactTag) ||
                hit.collider.CompareTag(tags.itemTag))
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
    }

    private void HandleBedInteraction(RaycastHit hit)
    {
        crosshairGUI.isLookingAtInteractable = true;
        if (bf != null) bf.CloseEyes = true;
        hit.collider.gameObject.tag = "Untagged";
        StartCoroutine(WaitAndExecute());
    }

    private IEnumerator WaitAndExecute()
    {
        yield return new WaitForSeconds(4f);
        if (bf != null) bf.CloseEyes = false;
        if (timeline != null) timeline.Play();
        if (GetComponent<AudioManager>() != null)
            GetComponent<AudioManager>().PlayNextTrack();
    }

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

    private void DropObject()
    {
        if (!isObjectHeld) return;

        isObjectHeld = false;
        tryPickupObject = false;
        currentInteraction?.OnRelease(objectHeld.GetComponent<Rigidbody>());
        objectHeld = null;
        currentInteraction = null;
    }

    private void ThrowObject()
    {
        if (!isObjectHeld) return;

        isObjectHeld = false;
        currentInteraction?.OnThrow(objectHeld.GetComponent<Rigidbody>(), playerCam.transform.forward);
        objectHeld = null;
        currentInteraction = null;
    }

    private void Use()
    {
        // Убираем лишние вызовы TryPickObject
        if (!isObjectHeld || objectHeld == null) return;

        // Проверяем, можно ли добавить в инвентарь
        if (objectHeld.CompareTag(tags.itemTag) || objectHeld.CompareTag(tags.interactTag))
        {
            inventorySystem.AddToInventoryNewItem(objectHeld);
            isObjectHeld = false;

            // Вызываем методы объекта перед уничтожением
            objectHeld.SendMessage("UseObject", SendMessageOptions.DontRequireReceiver);
            Destroy(objectHeld);

            objectHeld = null;
            currentInteraction = null;
            Debug.Log("Предмет добавлен в инвентарь");
        }
    }
}