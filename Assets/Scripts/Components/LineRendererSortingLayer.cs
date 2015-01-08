using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class LineRendererSortingLayer : MonoBehaviour {
    public string sortingLayerName;
    public int sortingOrder;

	void Start ()
    {
        UpdateValues();
	}

    void Update()
    {
        if (Application.isPlaying) return;

        UpdateValues();
    }

    void UpdateValues()
    {
        LineRenderer lr = GetComponent<LineRenderer>();
        if (lr == null) return;
        if (!"".Equals(sortingLayerName)) lr.sortingLayerName = sortingLayerName;
        lr.sortingOrder = sortingOrder;
    }
}
