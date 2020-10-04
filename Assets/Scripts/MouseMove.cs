using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMove : MonoBehaviour
{
    bool mouseDrag = false;

    void Update()
    {
        if(mouseDrag)
        {
            Vector3 pz = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pz.z = 0;
            gameObject.transform.position = pz;
        }
    }

    private void OnMouseUp()
    {
        mouseDrag = false;
    }

    private void OnMouseDown()
    {
        mouseDrag = true;
    }
}
