using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GearHandle : MonoBehaviour
{
    [SerializeField] private Gear linkedGear;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private int mouseButton = 0;
    [SerializeField] private float raycastRange = 100f;
    bool grabButton = false;
    bool grabButtonClick = false;

    private bool isGrabbed = false;

    private void Update()
    {
        CheckGrab();
        HandleGrab();
    }

    private void HandleGrab()
    {
        if (!isGrabbed) return;

        float angle = CalculateAngle();

        if (linkedGear == null)
        {
            Debug.LogError("Missing reference to gear");
            return;
        }

        linkedGear.RotateGear(angle, linkedGear);
    }

    private float CalculateAngle()
    {
        Vector2 basePosition = Camera.main.WorldToScreenPoint(linkedGear.transform.position);
        float angle = -Mathf.Atan2(Input.mousePosition.x - basePosition.x, Input.mousePosition.y - basePosition.y);
        angle *= 180f / Mathf.PI;
        return angle;
    }

    private void CheckGrab()
    {
        Ray mouseRay = new(Camera.main.ScreenToWorldPoint(Input.mousePosition), Camera.main.transform.forward);
        grabButton = Mouse.current.leftButton.isPressed;
        grabButtonClick = Mouse.current.leftButton.wasPressedThisFrame;

        if (isGrabbed && !grabButton)
        {
            isGrabbed = false;
            linkedGear.Reset();
            return;
        }

        if (!linkedGear.CheckForBlockedChain(out Gear badOne))
        {
            if (grabButtonClick)
                Debug.LogError(string.Format("Gear {0} is blocking", badOne.name), badOne);
            return;
        }

        if (Physics.Raycast(mouseRay, raycastRange))
        {
            if (grabButton)
            {
                isGrabbed = true;
                return;
            }
            return;
        }
    }
}
