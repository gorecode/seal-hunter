using UnityEngine;
using System.Collections;

public class ValrusMouth : MonoBehaviour {
    private Valrus valrus;

    void Awake()
    {
        valrus = transform.parent.GetComponent<Valrus>();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.transform == transform.parent) return;

        Component c = collider.gameObject.GetComponent(typeof(ITouchable));

        if (c != null)
        {
            Creature creature = c as Creature;

            creature.OnTouch();

            if (creature.GetCurrentState() == Creature.State.Dying)
            {
                valrus.CalmDown();
            }
        }
    }
}
