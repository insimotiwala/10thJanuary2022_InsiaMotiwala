using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelloWorld2 : MonoBehaviour
{
    private int count = 0; //global variable

    // Start is called before the first frame update
    private void Start()
    {
        Debug.Log("Hello Everyone!");
    }

    // Update is called once per frame
    private void Update()
    {
        Debug.Log("Update: " + count);
        int inc = 2; //local
        count = count + inc;
    }
}