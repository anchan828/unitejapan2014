using UnityEngine;
using System.Collections;

public class Example : MonoBehaviour
{

    void OnGUI()
    {
        HogeManager.instance.globalInt = (int)GUILayout.HorizontalSlider(HogeManager.instance.globalInt, 0, 100);
        GUILayout.Label(HogeManager.instance.globalString);
        GUILayout.Label(HogeManager.instance.globalTexture);
    }
}
