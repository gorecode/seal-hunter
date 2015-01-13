using UnityEngine;
using UnityEngineExt;
using System.Collections;
using System.Collections.Generic;

public class SlaughterBackground : MonoBehaviour
{
    delegate void GameObjectDelegate(GameObject obj);
    
    public Texture2D backgroundTexture;
    public Transform backgroundObject;
    public Camera renderTextureCamera;

    private RenderTexture renderTexture;
    private Dictionary<GameObject, int> objectsToRenderLayersMap;
    private List<GameObject> objectsToRender = new List<GameObject>();

    void Start()
    {
        ServiceLocator.current.slaughterBackgroundController = this;

        objectsToRenderLayersMap = new Dictionary<GameObject, int>();
        
        renderTexture = new RenderTexture(640, 480, 24, RenderTextureFormat.ARGB32);
        renderTexture.anisoLevel = 1;
		renderTexture.generateMips = false;
		renderTexture.useMipMap = false;
		renderTexture.antiAliasing = 1;
        renderTexture.filterMode = FilterMode.Point;
        renderTexture.Create();

		// XXX: Workaround unity issue, without this trick CameraClearFlags.Nothing won't work.
		renderTextureCamera.clearFlags = CameraClearFlags.SolidColor;
        renderTextureCamera.targetTexture = renderTexture;
		renderTextureCamera.Render ();
		renderTextureCamera.clearFlags = CameraClearFlags.Nothing;
		renderTextureCamera.enabled = false;

		Graphics.Blit(backgroundTexture, renderTexture);

        backgroundObject.GetComponent<MeshRenderer>().material.mainTexture = renderTexture;
    }

    void OnEnable()
    {
        EventBus.OnBecomeDead += RenderToTextureLater;
    }
    
    void OnDisable()
    {
        EventBus.OnBecomeDead -= RenderToTextureLater;
    }
  
    public void RenderToTextureLater(GameObject go)
    {
        if (gameObject.activeSelf) objectsToRender.Add(go.Retain());
    }

    void Update()
    {
        int count = objectsToRender.Count;

        if (count > 0)
        {
            for (int i = 0; i < count; i++) {
                CallForEach(objectsToRender[i], SaveLayer);
                
				SetLayerRecursively(objectsToRender[i], Layers.ENEMY_CORPSES);
            }

			renderTextureCamera.enabled = true;
			renderTextureCamera.Render();
			renderTextureCamera.enabled = false;

			for (int i = 0; i < count; i++)
			{
                CallForEach(objectsToRender[i], RestoreLayer);

                objectsToRender[i].Release();
        	}

            objectsToRenderLayersMap.Clear();
            objectsToRender.Clear();
        }
    }

    void SaveLayer(GameObject obj)
    {
        objectsToRenderLayersMap.Add(obj, obj.layer);
    }
    
    void RestoreLayer(GameObject obj)
    {
        int layer;
        
        if (objectsToRenderLayersMap.TryGetValue(obj, out layer)) obj.layer = layer;
    }
    
	void SetLayerRecursively(GameObject obj, int newLayer)
	{
        if (null == obj) return;
        
        obj.layer = newLayer;
        
        foreach (Transform child in obj.transform)
        {
            if (null == child) continue;
            
            SetLayerRecursively(child.gameObject, newLayer);
        }
	}

    void CallForEach(GameObject obj, GameObjectDelegate func)
    {
        if (null == obj) return;
        
        func(obj);
        
        foreach (Transform child in obj.transform)
        {
            if (null == child) continue;
            
            CallForEach(child.gameObject, func);
        }
    }
}
