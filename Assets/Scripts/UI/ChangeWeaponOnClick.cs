using UnityEngine;
using System.Collections;

public class ChangeWeaponOnClick : MonoBehaviour {
    public void OnClick()
    {
        GameObject hunter = GameObject.Find("Guns");

        for (int i = 0; i < hunter.transform.childCount; i++)
        {
            Transform child = hunter.transform.GetChild(i);
            child.gameObject.SetActive(child.gameObject.name.Equals(gameObject.name));
        }
    }
}
