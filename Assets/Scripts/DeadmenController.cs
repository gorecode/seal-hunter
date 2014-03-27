using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;

public class DeadmenController : MonoBehaviour
{
    private RenderTexture renderTexture;

    public Texture2D backgroundTexture;
    public Transform backgroundObject;
    public Camera renderTextureCamera;

    private List<GameObject> objectsToRender = new List<GameObject>();

    void Start()
    {
        renderTexture = new RenderTexture(640, 480, 24, RenderTextureFormat.ARGB32);
        renderTexture.anisoLevel = 0;
        renderTexture.filterMode = FilterMode.Point;
        renderTexture.Create();

        backgroundObject.GetComponent<MeshRenderer>().material.mainTexture = backgroundTexture;
        int oldLayer = backgroundObject.gameObject.layer;
        backgroundObject.gameObject.layer = Layers.ENEMY_CORPSES;

        renderTextureCamera.targetTexture = renderTexture;
        renderTextureCamera.gameObject.SetActive(true);
        renderTextureCamera.clearFlags = CameraClearFlags.SolidColor;
        renderTextureCamera.Render();
        renderTextureCamera.gameObject.SetActive(false);
        renderTextureCamera.clearFlags = CameraClearFlags.Nothing;

        EventBus.EnemyDied.Subscribe(OnEnemyDied);

        backgroundObject.gameObject.layer = oldLayer;
        backgroundObject.GetComponent<MeshRenderer>().material.mainTexture = renderTexture;
    }

    private void OnEnemyDied(GameObject enemy)
    {
        objectsToRender.Add(enemy);
    }

    void Update()
    {
        if (objectsToRender.Count > 0)
        {
            Debug.Log("Render dead objects of count " + objectsToRender.Count);

            for (int i = 0; i < objectsToRender.Count; i++)
            {
                GameObject go = objectsToRender[i];
                while (go.transform.parent != null) go = go.transform.parent.gameObject;
                SetLayerRecursively(go, LayerMask.NameToLayer("Enemy Corpses"));
                renderTextureCamera.gameObject.SetActive(true);
                renderTextureCamera.Render();
                renderTextureCamera.gameObject.SetActive(false);
                GameObjects.DestroyRoot(go);
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
