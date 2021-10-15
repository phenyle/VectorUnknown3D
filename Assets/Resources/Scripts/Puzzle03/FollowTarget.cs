using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Transform target;

    private void LateUpdate()
    {
        transform.position = target.transform.position;
    }
}
