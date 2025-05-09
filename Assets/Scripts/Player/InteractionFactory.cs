using UnityEngine;

public abstract class Interaction
{
    public abstract float PickupRange { get; }
    public abstract float ThrowStrength { get; }
    public abstract float HoldDistance { get; }
    public abstract float MaxGrabDistance { get; }
    public abstract bool FreezeRotation { get; }

    public virtual void OnGrab(Rigidbody rb)
    {
        rb.useGravity = false;
        if (FreezeRotation) rb.freezeRotation = true;
    }

    public virtual void OnRelease(Rigidbody rb)
    {
        rb.useGravity = true;
        rb.freezeRotation = false;
    }

    public virtual void OnThrow(Rigidbody rb, Vector3 direction)
    {
        rb.AddForce(direction * ThrowStrength);
    }
}

public class ObjectInteraction : Interaction
{
    public override float PickupRange => 1.5f;
    public override float ThrowStrength => 10f;
    public override float HoldDistance => 0.5f;
    public override float MaxGrabDistance => 1.5f;
    public override bool FreezeRotation => true;
}

public class ItemInteraction : Interaction
{
    public override float PickupRange => 1.5f;
    public override float ThrowStrength => 45f;
    public override float HoldDistance => 1f;
    public override float MaxGrabDistance => 1.5f;
    public override bool FreezeRotation => true;
}

public class DoorInteraction : Interaction
{
    public override float PickupRange => 1.5f;
    public override float ThrowStrength => 5f;
    public override float HoldDistance => 1.2f;
    public override float MaxGrabDistance => 1.5f;
    public override bool FreezeRotation => false;

    public override void OnGrab(Rigidbody rb)
    {
        rb.useGravity = false;
        rb.freezeRotation = true; // Для дверей все же лучше заморозить вращение
    }
}

public static class InteractionFactory
{
    public static Interaction CreateInteraction(string tag)
    {
        Drag dragInstance = GameObject.FindObjectOfType<Drag>();
        if (dragInstance == null) return null;

        if (tag == dragInstance.tags.interactTag)
            return new ObjectInteraction();
        if (tag == dragInstance.tags.itemTag)
            return new ItemInteraction();
        if (tag == dragInstance.tags.doorTag)
            return new DoorInteraction();

        return null;
    }
}