using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Door Settings")]
    [Tooltip("The door object to move")]
    public GameObject doorObject;

    [Tooltip("Time (seconds) it takes for the door to fully open/close")]
    public float moveDuration = 1.0f;

    protected bool isOpen = false;
    protected bool isMoving = false;

    protected float doorHeight = 0f;
    protected Vector3 closedPosition;
    protected Vector3 openPosition;

    protected virtual void Awake()
    {
        if (doorObject == null)
            doorObject = gameObject;

        closedPosition = doorObject.transform.position;

        Renderer renderer = doorObject.GetComponent<Renderer>();
        if (renderer != null)
        {
            doorHeight = renderer.bounds.size.y;
        }
        else
        {
            Debug.LogWarning($"{name}: Door has no Renderer, height set to 0.");
        }

        openPosition = closedPosition + Vector3.up * doorHeight;
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

    public bool IsOpen()
    {
        return isOpen;
    }

    public bool IsMoving()
    {
        return isMoving;
    }
}