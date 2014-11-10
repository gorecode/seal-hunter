using UnityEngine;
using System.Collections;

public class FollowObject : MonoBehaviour {
    public Transform target;

    public void Update()
    {
        Camera.main.transform.position = new Vector3(target.position.x, target.position.y, Camera.main.transform.position.z);
    }
}
