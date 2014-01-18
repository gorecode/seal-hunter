using UnityEngine;
using System.Collections;

public class DeathPool : MonoBehaviour
{
		// Use this for initialization
		void Start ()
		{
	
		}

		void OnTriggerEnter2D (Collider2D collider)
		{
				Transform t = collider.transform;
				while (t.parent != null) {
						t = t.parent;
				}
				Destroy (t.gameObject);
		}

		// Update is called once per frame
		void Update ()
		{
	
		}
}
