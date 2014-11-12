using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SlaughterLayerRenderer : MonoBehaviour
{
    private RenderTexture renderTexture;

    public Texture2D staticBackgroundTexture;
    public Transform dynamicBackgroundObject;
    public Camera renderTextureCamera;

    private List<GameObject> objectsToRender = new List<GameObject>();

    void Start()
    {
        renderTexture = new RenderTexture(640, 480, 24, RenderTextureFormat.ARGB32);
        renderTexture.anisoLevel = 0;
        renderTexture.filterMode = FilterMode.Point;
        renderTexture.Create();

        dynamicBackgroundObject.GetComponent<MeshRenderer>().material.mainTexture = staticBackgroundTexture;
        int oldLayer = dynamicBackgroundObject.gameObject.layer;
        dynamicBackgroundObject.gameObject.layer = Layers.ENEMY_CORPSES;

        renderTextureCamera.targetTexture = renderTexture;
        renderTextureCamera.gameObject.SetActive(true);
        renderTextureCamera.clearFlags = CameraClearFlags.SolidColor;
        renderTextureCamera.Render();
        renderTextureCamera.gameObject.SetActive(false);
        renderTextureCamera.clearFlags = CameraClearFlags.Nothing;

        EventBus.OnDeath += OnEnemyDied;

        dynamicBackgroundObject.gameObject.layer = oldLayer;
        dynamicBackgroundObject.GetComponent<MeshRenderer>().material.mainTexture = renderTexture;
    }

    private void OnEnemyDied(GameObject enemy)
    {
        objectsToRender.Add(enemy);
    }

    void Update()
    {
        if (objectsToRender.Count > 0)
        {
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
