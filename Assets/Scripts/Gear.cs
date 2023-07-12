using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Gear : MonoBehaviour
{
    [SerializeField] protected List<Gear> LinkedGears;
    protected List<Gear> ValidGears = new List<Gear>();
    protected Vector3 eulerAngles;
    [SerializeField] protected float gearSize = 1f;

    public static float DefaultDistance = 8f;

    protected bool isOdd = true;
    protected bool beenChecked = false;
    protected Gear rotationCaller = null;

    public bool IsOdd { get => isOdd; set => isOdd = value; }
    public bool BeenChecked { get => beenChecked; set => beenChecked = value; }

    private void Start()
    {
        Init();
        ValidateGears();
    }

    private void ValidateGears()
    {
        if (LinkedGears.Count == 0) return;

        ValidGears = new List<Gear>();
        foreach (Gear g in LinkedGears)
        {
            if (CheckGearProximity(g))
            {
                ValidGears.Add(g);
            }
        }
    }


    public bool CheckForBlockedChain(out Gear badOne)
    {
        foreach (Gear g in ValidGears)
        {
            if (!g.gameObject.activeInHierarchy) continue;
            if (g.BeenChecked && g.IsOdd != !isOdd)
            {
                badOne = g;
                return false;
            }
            g.BeenChecked = true;
            g.IsOdd = !isOdd;
            if (!g.CheckForBlockedChain(out badOne))
            {
                return false;
            }
        }
        badOne = null;
        return true;
    }

    public void Reset()
    {
        beenChecked = false;
        isOdd = false;
        rotationCaller = null;
        if (ValidGears.Count == 0) return;
        ValidGears.ForEach(x => x.Reset());
    }

    private bool CheckGearProximity(Gear gearToCompare)
    {
        float distance = Vector3.Distance(gearToCompare.transform.position, transform.position);
        if (distance > Gear.DefaultDistance * (gearSize + gearToCompare.gearSize))
            return false;
        return true;
    }

    protected virtual void Init()
    {
        eulerAngles = transform.eulerAngles;
    }

    public virtual void RotateGear(float val, Gear newRotationCaller)
    {
        if (!gameObject.activeInHierarchy) return;
        if (IsThatAnotherCaller(newRotationCaller)) return;
        rotationCaller = newRotationCaller;

        eulerAngles.x = val;
        transform.eulerAngles = eulerAngles;

        if (ValidGears.Count == 0)
            return;

        ValidGears.ForEach(x => x.RotateGear(-val, this));
    }

    protected bool IsThatAnotherCaller(Gear newRotationCaller)
    {
        return rotationCaller != null && rotationCaller != newRotationCaller;
    }
}
