using System;
using System.Collections;
using Cards.Environments;
using Game;
using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Door Settings")]
    [Tooltip("The door object to move")]
    public GameObject doorObject;

    [Tooltip("Vertical distance the door moves when opening")]
    public float openHeight = 3f;

    [Tooltip("Time (seconds) it takes for the door to fully open/close")]
    public float moveDuration = 1.0f;

    [Tooltip("If true, the door starts in the open position")]
    public bool startOpen = false;

    protected bool isOpen = false;
    protected bool isMoving = false;

    protected Vector3 closedPosition;
    protected Vector3 openPosition;
    
    public PersistentID id;

    private void Awake()
    {
        GlobalWorldManager.OnPreLoadNewScene += Init;
    }

    private void Init(CardEnv env)
    {
        GlobalWorldManager.OnPreLoadNewScene -= Init;
        if (doorObject == null)
            doorObject = gameObject;

        closedPosition = doorObject.transform.position;
        openPosition = closedPosition + Vector3.up * openHeight;
        
        if (id && GlobalState.instance.HasEvent($"DOOR_OPEN_{id.ID}")) startOpen = true;
        if (startOpen)
        {
            doorObject.transform.position = openPosition;
            isOpen = true;
        }
    }

    protected virtual void Update()
    {
        if (isMoving)
            return;

        if (CheckCondition())
        {
            Open();
        }
    }

    public virtual bool CheckCondition()
    {
        return false;
    }

    public virtual void Open()
    {
        if (isOpen || isMoving)
            return;
        if (id != null)
        {
            GlobalState.instance.AddEvent($"DOOR_OPEN_{id.ID}");
        }
        
        StartCoroutine(MoveDoor(closedPosition, openPosition, true));
    }

    public virtual void Close()
    {
        if (!isOpen || isMoving)
            return;

        StartCoroutine(MoveDoor(openPosition, closedPosition, false));
    }

    protected IEnumerator MoveDoor(Vector3 from, Vector3 to, bool opening)
    {
        isMoving = true;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(moveDuration, 0.0001f);
            doorObject.transform.position = Vector3.Lerp(from, to, t);
            yield return null;
        }

        doorObject.transform.position = to;
        isOpen = opening;
        isMoving = false;
    }

    public bool IsOpen() => isOpen;
    public bool IsMoving() => isMoving;
}
