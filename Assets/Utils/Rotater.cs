using UnityEngine;

public class Rotater : MonoBehaviour
{
    public Vector3 axis;
    public float amount;
    void Update()
    {
        transform.Rotate(axis, Time.deltaTime * amount);
    }
}
