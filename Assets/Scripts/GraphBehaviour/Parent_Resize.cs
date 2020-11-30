using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Parent_Resize : MonoBehaviour
{
    private Transform[] childTranforms;
    private Button[,] resizeButtons;
    private Vector3[] resetRectPosition;
    private Canvas gridCenter;

    // Start is called before the first frame update
    void Start()
    {

        gridCenter = GameObject.Find("Center").GetComponent<Canvas>();


        int children = transform.childCount;
        childTranforms = new Transform[children];
        resetRectPosition = new Vector3[children];
        //resizeButtons = new Button[children][2];
        for (int i = 0; i < children; ++i)
        {
            print("Foreach loop: " + transform.GetChild(i));
            childTranforms[i] = transform.GetChild(i);

            //Debug.LogError(childTranforms[i].name);

            if(childTranforms[i].childCount == 0)
            {
                continue;
            }
            //initialize with the starting position
            resetRectPosition[i] = childTranforms[i].localPosition;
            Debug.LogWarning(resetRectPosition[i]);
        }



    }

    private void maximize(int child_id)
    {
        childTranforms[child_id].position = gridCenter.GetComponent<RectTransform>().transform.position;
        childTranforms[child_id].localScale = new Vector3(2, 2, 1);

    }

    private void minimize(int child_id)
    {
        Debug.LogWarning(resetRectPosition);
        childTranforms[child_id].localPosition = resetRectPosition[child_id];
        childTranforms[child_id].localScale = new Vector3(1, 1, 1);
        gridCenter.enabled = false;
    }
}
