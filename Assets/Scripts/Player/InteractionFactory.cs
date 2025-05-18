using UnityEngine;

/// <summary>
/// Базовый класс для всех типов взаимодействий с объектами
/// </summary>
public abstract class Interaction
{
    public abstract float PickupRange { get; }
    public abstract float ThrowStrength { get; }
    public abstract float HoldDistance { get; }
    public abstract float MaxGrabDistance { get; }
    public abstract bool FreezeRotation { get; }
    public abstract string InteractionType { get; }

    /// <summary>
    /// Вызывается при захвате объекта
    /// </summary>
    public virtual void OnGrab(Rigidbody rb)
    {
        if (rb == null) return;

        rb.useGravity = false;
        rb.linearDamping = 10f;
        rb.angularDamping = 5f;

        if (FreezeRotation)
        {
            rb.freezeRotation = true;
            rb.angularVelocity = Vector3.zero;
        }
    }

    /// <summary>
    /// Вызывается при отпускании объекта
    /// </summary>
    public virtual void OnRelease(Rigidbody rb)
    {
        if (rb == null) return;

        rb.useGravity = true;
        rb.linearDamping = 1f;
        rb.angularDamping = 0.05f;
        rb.freezeRotation = false;
    }

    /// <summary>
    /// Вызывается при броске объекта
    /// </summary>
    public virtual void OnThrow(Rigidbody rb, Vector3 direction)
    {
        if (rb == null) return;

        OnRelease(rb);
        rb.AddForce(direction.normalized * ThrowStrength, ForceMode.Impulse);
    }
}

/// <summary>
/// Взаимодействие с обычными объектами окружения
/// </summary>
public class ObjectInteraction : Interaction
{
    public override float PickupRange => 1.5f;
    public override float ThrowStrength => 10f;
    public override float HoldDistance => 0.5f;
    public override float MaxGrabDistance => 1.5f;
    public override bool FreezeRotation => true;
    public override string InteractionType => "Object";
}

/// <summary>
/// Взаимодействие с предметами, которые можно подбирать
/// </summary>
public class ItemInteraction : Interaction
{
    public override float PickupRange => 1.5f;
    public override float ThrowStrength => 45f;
    public override float HoldDistance => 1f;
    public override float MaxGrabDistance => 1.5f;
    public override bool FreezeRotation => true;
    public override string InteractionType => "Item";

    public override void OnGrab(Rigidbody rb)
    {
        base.OnGrab(rb);
        
    }
}

/// <summary>
/// Специальное взаимодействие с дверьми
/// </summary>
public class DoorInteraction : Interaction
{
    public override float PickupRange => 1.5f;
    public override float ThrowStrength => 5f;
    public override float HoldDistance => 1.2f;
    public override float MaxGrabDistance => 1.5f;
    public override bool FreezeRotation => false;
    public override string InteractionType => "Door";

    public override void OnGrab(Rigidbody rb)
    {
        if (rb == null) return;

        rb.useGravity = false;
        rb.freezeRotation = true; // Для дверей замораживаем вращение
        rb.centerOfMass = Vector3.zero;
    }
}

/// <summary>
/// Фабрика для создания конкретных реализаций взаимодействий
/// </summary>
public static class InteractionFactory
{
    private static Drag _cachedDragInstance;

    /// <summary>
    /// Создает конкретный тип взаимодействия на основе тега объекта
    /// </summary>
    public static Interaction CreateInteraction(string tag)
    {
        if (_cachedDragInstance == null)
        {
            _cachedDragInstance = Object.FindFirstObjectByType<Drag>();
            if (_cachedDragInstance == null)
            {
                Debug.LogError("Drag component not found in scene");
                return null;
            }
        }

        var tags = _cachedDragInstance.tags;

        return tag switch
        {
            _ when tag == tags.interactTag => new ObjectInteraction(),
            _ when tag == tags.itemTag => new ItemInteraction(),
            _ when tag == tags.doorTag => new DoorInteraction(),
            _ => null
        };
    }
}