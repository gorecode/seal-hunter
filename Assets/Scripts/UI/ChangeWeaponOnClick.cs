using UnityEngine;
using System.Collections;

public class ChangeWeaponOnClick : MonoBehaviour {
    public AudioClip sound;

    Transform highlight;

    void Awake()
    {
        highlight = transform.FindChild("Highlight");
    }

    public void OnClick()
    {
        GameObject hunter = GameObject.Find("Guns");

        for (int i = 0; i < hunter.transform.childCount; i++)
        {
            Transform child = hunter.transform.GetChild(i);
            if (child.GetComponent<Gun>() == null) continue;
            child.gameObject.SetActive(child.gameObject.name.Equals(gameObject.name));
        }

        Transform parent = transform.parent;

        for (int i = 0; i < parent.childCount; i++)
        {
            ChangeWeaponOnClick buttonScript = parent.GetChild(i).GetComponent<ChangeWeaponOnClick>();
            if (buttonScript == null) continue;
            buttonScript.highlight.gameObject.SetActive(buttonScript == this);
        }

        if (sound != null) AudioCenter.PlayClipAtMainCamera(sound);
    }
}
