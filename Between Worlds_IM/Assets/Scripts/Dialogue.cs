using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour
{
    public GameObject Balloon;
    public string Text;

    private GameObject _curBalloon;

    private void OnTriggerEnter(Collider col)
    {
        if (col.tag != "Player") { return; }
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null) { return; }

        _curBalloon = Instantiate(Balloon); //create text balloon
        _curBalloon.GetComponent<RectTransform>().position = new Vector2(Screen.width / 2.0f, 15); //position text balloon
        _curBalloon.GetComponent<Text>().text = Text; //update text in balloon
        _curBalloon.transform.parent = canvas.transform; //add balloon under Canvas
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.tag != "Player") { return; }
        if (_curBalloon == null) { return; }
        Destroy(_curBalloon);
    }
}