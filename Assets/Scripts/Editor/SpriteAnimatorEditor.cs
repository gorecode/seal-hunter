using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections;
using System.IO;

[CustomEditor(typeof(SpriteAnimator))]
public class SpriteAnimationEditor : Editor {
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Create Animations"))
        {
            CreateAnimations();
        }
    }

    public void CreateAnimations()
    {
        SpriteAnimator myScript = (SpriteAnimator)target;
        Animation myAnimation = myScript.gameObject.animation;

        Object[] selection = Selection.GetFiltered(typeof(Texture2D), SelectionMode.Unfiltered);

        SpriteList[] sheets = new SpriteList[selection.Length];

        for (int i = 0; i < selection.Length; i++)
        {
            Object texture = selection[i];

            sheets[i] = new SpriteList();
            sheets[i].pivots = new Vector3[1];
            sheets[i].pivots[0] = Vector3.zero;

            string textureAssetPath = AssetDatabase.GetAssetPath(texture);

            Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(textureAssetPath).OfType<Sprite>().ToArray();

            sheets[i].sprites = sprites;
            sheets[i].name = texture.name;

            AnimationClip clip = new AnimationClip();
            
            clip.name = texture.name;
            clip.frameRate = sprites.Length + 1;
            clip.wrapMode = WrapMode.Once;
            
            float frameDuration = 1.0f / clip.frameRate;
            
            AnimationCurve indexCurve = AnimationCurve.Linear(0.0f, 0.0f, (clip.frameRate - 1) * frameDuration, sprites.Length - 1);
            AnimationCurve sheetCurve = AnimationCurve.Linear(0.0f, i, clip.frameRate * frameDuration, i);

            indexCurve.AddKey(clip.frameRate * frameDuration, sprites.Length - 1);

            clip.SetCurve("", typeof(SpriteAnimator), "index", indexCurve);
            clip.SetCurve("", typeof(SpriteAnimator), "sheet", sheetCurve);
            
            AnimationUtility.SetAnimationType(clip, ModelImporterAnimationType.Legacy);

            string prefabFolder = Directory.GetParent(textureAssetPath).ToString();
            string clipFile = prefabFolder + "/" + clip.name + ".anim";

            AssetDatabase.CreateAsset(clip, clipFile);

            myAnimation.AddClip(clip, clip.name);
        }

        AssetDatabase.SaveAssets();

        myScript.sheets = sheets;
    }
}
