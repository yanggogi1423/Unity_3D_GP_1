using UnityEngine;

public class MyGizmos : MonoBehaviour
{
    public Color _color = Color.red;
    public float _radius = 0.8f;

    void OnDrawGizmos()
    {
        Gizmos.color = _color;
        Gizmos.DrawSphere(transform.position, _radius);
    }
}
