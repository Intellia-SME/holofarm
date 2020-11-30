using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Resize_Graph : MonoBehaviour
{
    //private RectTransform GraphCanvas;
    public RectTransform windowGraph;

    private Canvas gridCenter;
    private Canvas current;

    private GameObject tempGameObject;

    private Vector3 resetRectPosition;

    private bool windowMaximized, windowMinimized;

    // Start is called before the first frame update
    void Start()
    {
        //GraphCanvas = GameObject.Find("GraphCanvas").GetComponent<RectTransform>();
        gridCenter = GameObject.Find("Center").GetComponent<Canvas>();
        //current = GameObject.Find("Graph").GetComponent<Canvas>();
        //windowGraph = GameObject.Find("Window_Graph").GetComponent<RectTransform>();

        //initialize with the starting position
        resetRectPosition = windowGraph.localPosition;
        Debug.LogWarning(resetRectPosition);

        this.windowMaximized = false;
        this.windowMinimized = true;
        /*
        windowGraph.Find("maximizeBtn").GetComponent<Button>().onClick.AddListener(delegate
        {
            this.maximize();
        });
        windowGraph.Find("minimizeBtn").GetComponent<Button>().onClick.AddListener(delegate
        {
            this.minimize();
        });
        
        windowGraph.GetComponent<Button>().onClick.AddListener(delegate
        {
            this.maximize();
            Debug.LogWarning("window clicked!");
        });
        /*
        windowGraph.GetComponent<Button>().onClick.AddListener(delegate
        {
            this.minimize();
        });
        */
        this.ShowTools(false);
    }

    private void Update()
    {
        if (this.windowMaximized)
        {
            windowGraph.GetComponent<Button>().onClick.AddListener(delegate
            {
                this.minimize();
            });
        }else if (this.windowMinimized)
        {
            windowGraph.GetComponent<Button>().onClick.AddListener(delegate
            {
                this.maximize();
            });
        }
    }

    private void ShowTools(bool show)
    {
        windowGraph.Find("barChartBtn").GetComponent<RectTransform>().gameObject.SetActive(show);
        windowGraph.Find("lineGraphBtn").GetComponent<RectTransform>().gameObject.SetActive(show);
        windowGraph.Find("decreaseVisibleAmountBtn").GetComponent<RectTransform>().gameObject.SetActive(show);
        windowGraph.Find("increaseVisibleAmountBtn").GetComponent<RectTransform>().gameObject.SetActive(show);
        //windowGraph.Find("euroBtn").GetComponent<RectTransform>().gameObject.SetActive(show);
        //windowGraph.Find("dollarBtn").GetComponent<RectTransform>().gameObject.SetActive(show);

    }

    private void maximize()
    {
        Debug.LogWarning("maximize");
        this.windowMaximized = true;
        this.windowMinimized = false;

        transform.parent.GetComponent<UIBehaviour>().maximizeBehaviour(transform);

        //resetRectPosition = current.transform.localPosition;

        //Debug.LogError(resetRectPosition);

        windowGraph.position = gridCenter.GetComponent<RectTransform>().transform.position;
        windowGraph.localScale = new Vector3(2, 2, 1);

        this.ShowTools(true);
    }

    private void minimize()
    {

        this.windowMaximized = false;
        this.windowMinimized = true;

        transform.parent.GetComponent<UIBehaviour>().minimizeBehaviour(transform);

        //windowGraph = GameObject.Find("Window_Graph").GetComponent<RectTransform>();
        //graphContainer = initialRect;
        //graphContainer.sizeDelta = Vector2.zero;

        //current.enabled = true;
        //Debug.LogError(initRect);

        //current = GameObject.Find("Graph").GetComponent<Canvas>();
        Debug.LogWarning(resetRectPosition);
        windowGraph.localPosition = resetRectPosition;
        windowGraph.localScale = new Vector3(1, 1, 1);

        this.ShowTools(false);
        Destroy(tempGameObject);
        gridCenter.enabled = false;
    }
}


