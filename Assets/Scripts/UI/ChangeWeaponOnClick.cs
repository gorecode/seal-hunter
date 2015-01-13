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
        Hunter hunter = GameObject.Find("Hunter").GetComponentInChildren<Hunter>();
        hunter.SetActiveGunByName(gameObject.name);

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
