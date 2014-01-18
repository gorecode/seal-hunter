using UnityEngine;
using System.Collections;

public class LogCollisions : MonoBehaviour
{

		// Use this for initialization
		void Start ()
		{
	
		}

		void OnCollisionEnter2D (Collision2D collision)
		{
				Debug.Log ("OnCollisionEnter2D " + collision.gameObject.name);
		}

		void OnTriggerEnter2D (Collider2D collider)
		{
				Debug.Log ("OnTriggerEnter2D " + collider.gameObject.name);
		}

		// Update is called once per frame
		void Update ()
		{
	
		}
}
