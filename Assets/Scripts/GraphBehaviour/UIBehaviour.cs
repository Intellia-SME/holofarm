using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBehaviour : MonoBehaviour
{
    private Transform[] childTranforms;
    private int children = 0;

    // Start is called before the first frame update
    void Start()
    {
        children = transform.childCount;
        childTranforms = new Transform[children];
        for (int i = 0; i < children; ++i)
        {
            print("Foreach loop: " + transform.GetChild(i));
            childTranforms[i] = transform.GetChild(i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void maximizeBehaviour(Transform caller)
    {
        for (int i = 0; i < children; ++i)
        {
            if(childTranforms[i] == caller)
            {
                continue;
            }
            else
            {
                childTranforms[i].GetComponent<Canvas>().enabled = false;
            }
        }
    }

    public void minimizeBehaviour(Transform caller)
    {
        for (int i = 0; i < children; ++i)
        {
            if (childTranforms[i] == caller)
            {
                continue;
            }
            else
            {
                childTranforms[i].GetComponent<Canvas>().enabled = true;
            }
        }
    }

}
