using UnityEngine;
using System.Collections;

public class SawScript : MonoBehaviour {
    void Update()
    {
        transform.Rotate(0, 0, 1200 * Time.deltaTime, Space.Self);
    }
}
