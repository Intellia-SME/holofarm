using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VideoResize : MonoBehaviour
{
    //private RectTransform GraphCanvas;
    public RectTransform videoWindow;

    private Canvas gridCenter;

    private Vector3 resetRectPosition;


    private bool windowMaximized, windowMinimized;

    // Start is called before the first frame update
    void Start()
    {
        //GraphCanvas = GameObject.Find("GraphCanvas").GetComponent<RectTransform>();
        //current = GameObject.Find("Graph").GetComponent<Canvas>();
        //videoWindow = GameObject.Find("Video_Graph").GetComponent<RectTransform>();
        this.windowMaximized = false;
        this.windowMinimized = true;
        //initialize with the starting position
        resetRectPosition = videoWindow.localPosition;
        //Debug.LogError(resetRectPosition);


        /*
        minimizeBtn.onClick.AddListener(delegate
        {
            this.minimize();
        });

        maximizeBtn.onClick.AddListener(delegate
        {
            this.maximize();
        });

        
        videoWindow.Find("minimizeBtn").GetComponent<RectTransform>().gameObject.SetActive(false);

        videoWindow.Find("maximizeBtn").GetComponent<Button>().onClick.AddListener(delegate
        {
            this.maximize();
        });
        videoWindow.Find("minimizeBtn").GetComponent<Button>().onClick.AddListener(delegate
        {
            this.minimize();
        });
        */

        gridCenter = GameObject.Find("Center").GetComponent<Canvas>();

        this.ShowTools(false);
    }

    private void Update()
    {
        if (this.windowMaximized)
        {
            videoWindow.GetComponent<Button>().onClick.AddListener(delegate
            {
                this.minimize();
            });
        }
        else if (this.windowMinimized)
        {
            videoWindow.GetComponent<Button>().onClick.AddListener(delegate
            {
                this.maximize();
            });
        }
    }

    private void ShowTools(bool show)
    {


    }

    private void maximize()
    {
        this.windowMaximized = true;
        this.windowMinimized = false;

        transform.parent.GetComponent<UIBehaviour>().maximizeBehaviour(transform);

        //videoWindow.Find("maximizeBtn").GetComponent<RectTransform>().gameObject.SetActive(false);
        //videoWindow.Find("minimizeBtn").GetComponent<RectTransform>().gameObject.SetActive(true);

        //resetRectPosition = current.transform.localPosition;

        //Debug.LogError(resetRectPosition);

        videoWindow.position = gridCenter.GetComponent<RectTransform>().transform.position;
        videoWindow.localScale = new Vector3(2, 2, 1);

        //this.ShowTools(true);
    }

    private void minimize()
    {

        this.windowMaximized = false;
        this.windowMinimized = true;

        transform.parent.GetComponent<UIBehaviour>().minimizeBehaviour(transform);

        //videoWindow.Find("maximizeBtn").GetComponent<RectTransform>().gameObject.SetActive(true);
        //videoWindow.Find("minimizeBtn").GetComponent<RectTransform>().gameObject.SetActive(false);
        //windowGraph = GameObject.Find("Window_Graph").GetComponent<RectTransform>();
        //graphContainer = initialRect;
        //graphContainer.sizeDelta = Vector2.zero;

        //current.enabled = true;
        //Debug.LogError(initRect);

        //current = GameObject.Find("Graph").GetComponent<Canvas>();
        Debug.LogWarning(resetRectPosition);
        videoWindow.localPosition = resetRectPosition;
        videoWindow.localScale = new Vector3(1, 1, 1);

        //this.ShowTools(false);
        gridCenter.enabled = false;
    }
}
