using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Resize : MonoBehaviour
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
        this.windowMaximized = false;
        this.windowMinimized = true;
        //initialize with the starting position
        resetRectPosition = windowGraph.localPosition;
        Debug.LogWarning(resetRectPosition);
 
        /*
        windowGraph.Find("maximizeBtn").GetComponent<Button>().onClick.AddListener(delegate
        {
            this.maximize();
        });
        windowGraph.Find("minimizeBtn").GetComponent<Button>().onClick.AddListener(delegate
        {
            this.minimize();
        });
        */
        this.ShowTools(false);
    }

    /*
     * Avoided for perfomance reasons
     * 
    void OnMouseDrag()
    {
        Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance);
        Vector3 objPosition = Camera.main.ScreenToWorldPoint(mousePosition);

        transform.position = objPosition;
    }
    */

    private Vector3 screenPoint;
    private Vector3 offset;

    void OnMouseDown()
    {
        screenPoint = Camera.main.WorldToScreenPoint(transform.position);
        offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
    }
    void OnMouseDrag()
    {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
        transform.position = curPosition;
    }

private void Update()
    {
        if (this.windowMaximized)
        {
            windowGraph.GetComponent<Button>().onClick.AddListener(delegate
            {
                this.minimize();
            });
        }
        else if (this.windowMinimized)
        {
            windowGraph.GetComponent<Button>().onClick.AddListener(delegate
            {
                this.maximize();
            });
        }
    }

    private void ShowTools(bool show)
    {
        //windowGraph.Find("barChartBtn").GetComponent<RectTransform>().gameObject.SetActive(show);
        //windowGraph.Find("lineGraphBtn").GetComponent<RectTransform>().gameObject.SetActive(show);
        //windowGraph.Find("decreaseVisibleAmountBtn").GetComponent<RectTransform>().gameObject.SetActive(show);
        //windowGraph.Find("increaseVisibleAmountBtn").GetComponent<RectTransform>().gameObject.SetActive(show);
        //windowGraph.Find("celciusBtn").GetComponent<RectTransform>().gameObject.SetActive(show);
        //windowGraph.Find("fahrenheitBtn").GetComponent<RectTransform>().gameObject.SetActive(show);
        windowGraph.Find("Dropdown").GetComponent<RectTransform>().gameObject.SetActive(show);

    }

    private void maximize()
    {
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

