using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SlaughterLayerRenderer : MonoBehaviour
{
    private RenderTexture renderTexture;

    public Texture2D backgroundTexture;
    public Transform backgroundObject;
    public Camera renderTextureCamera;

    private List<GameObject> objectsToRender = new List<GameObject>();

    void Start()
    {
        renderTexture = new RenderTexture(640, 480, 24, RenderTextureFormat.ARGB32);
        renderTexture.anisoLevel = 1;
		renderTexture.generateMips = false;
		renderTexture.useMipMap = false;
		renderTexture.antiAliasing = 1;
        renderTexture.filterMode = FilterMode.Point;
        renderTexture.Create();

		// XXX: Workaround unity issue, with non working CameraClearFlags.Nothing flag.
		renderTextureCamera.clearFlags = CameraClearFlags.SolidColor;
        renderTextureCamera.targetTexture = renderTexture;
		renderTextureCamera.Render ();
		renderTextureCamera.clearFlags = CameraClearFlags.Nothing;
		renderTextureCamera.enabled = false;

		Graphics.Blit (backgroundTexture, renderTexture);

        backgroundObject.GetComponent<MeshRenderer>().material.mainTexture = renderTexture;

		EventBus.OnBecomeDead += OnEnemyDied;
    }

    private void OnEnemyDied(GameObject enemy)
    {
        objectsToRender.Add(enemy);
    }

    void Update()
    {
        if (objectsToRender.Count > 0)
        {
			Debug.Log("Render " + objectsToRender.Count + " dead enemies");

            for (int i = 0; i < objectsToRender.Count; i++)
				SetLayerRecursively(objectsToRender[i], Layers.ENEMY_CORPSES);

			renderTextureCamera.enabled = true;
			renderTextureCamera.Render();
			renderTextureCamera.enabled = false;

			for (int i = 0; i < objectsToRender.Count; i++)
			{
				SetLayerRecursively(objectsToRender[i], Layers.ENEMY);

				Creature2 creatureScript = objectsToRender[i].GetComponentInChildren(typeof(Creature2)) as Creature2;

				creatureScript.Advance(Creature2.State.Recycled);
			}

            objectsToRender.Clear();
        }
    }

	void SetLayerRecursively(GameObject obj, int newLayer)
	{
		if (null == obj)
		{
			return;
		}
		
		obj.layer = newLayer;
		
		foreach (Transform child in obj.transform)
		{
			if (null == child)
			{
				continue;
			}
			
			SetLayerRecursively(child.gameObject, newLayer);
		}
	}
}
