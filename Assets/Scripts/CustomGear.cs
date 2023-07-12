using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomGear : Gear
{
    [SerializeField] private float angleModifier = 1f;

    public override void RotateGear(float val, Gear newRotationCaller)
    {
        if (!gameObject.activeInHierarchy) return;
        if (IsThatAnotherCaller(newRotationCaller)) return;
        rotationCaller = newRotationCaller;

        eulerAngles.x = val*angleModifier;
        transform.eulerAngles = eulerAngles;

        if (ValidGears.Count == 0)
            return;

        ValidGears.ForEach(x => x.RotateGear(-val,this));
    }
}
