
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Window_Movement : MonoBehaviour
{
    private const float distance = 2.0f;

    private static readonly Vector2 defaultWindowRotation = new Vector2(10.0f, 20.0f);
    private static readonly Vector3 defaultWindowScale = new Vector3(0.2f, 0.04f, 1.0f);
    private static readonly Vector3[] backgroundScales = { new Vector3(1.0f, 1.0f, 1.0f), new Vector3(1.0f, 0.5f, 1.0f), new Vector3(1.0f, 0.25f, 1.0f) };
    private static readonly Vector3[] backgroundOffsets = { new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.25f, 0.0f), new Vector3(0.0f, 0.375f, 0.0f) };

    public Transform window;
    //public Transform background;
    public Transform cameraTransform;
    
    private Transform usedAnchor;
    private Transform peakAnchor;
    private Quaternion windowHorizontalRotation;
    private Quaternion windowHorizontalRotationInverse;
    private Quaternion windowVerticalRotation;
    private Quaternion windowVerticalRotationInverse;

    public Transform WindowParent { get; set; } = null;

    [Header("Transform Settings")]
    [SerializeField, Tooltip("Is the transform currently visible.")]
    private bool isVisible = false;

    [Header("Window Settings")]
    [SerializeField, Tooltip("What part of the view port to anchor the window to.")]
    private TextAnchor windowAnchor = TextAnchor.LowerCenter;

    public TextAnchor WindowAnchor
    {
        get { return windowAnchor; }
        set { windowAnchor = value; }
    }

    [SerializeField, Tooltip("The offset from the view port center applied based on the window anchor selection.")]
    private Vector2 windowOffset = new Vector2(0.1f, 0.1f);

    public Vector2 WindowOffset
    {
        get { return windowOffset; }
        set { windowOffset = value; }
    }

    [SerializeField, Range(0.5f, 5.0f), Tooltip("Use to scale the window size up or down, can simulate a zooming effect.")]
    private float windowScale = 1.0f;

    public float WindowScale
    {
        get { return windowScale; }
        set { windowScale = Mathf.Clamp(value, 0.5f, 5.0f); }
    }

    [SerializeField, Range(0.0f, 100.0f), Tooltip("How quickly to interpolate the window towards its target position and rotation.")]
    private float windowFollowSpeed = 5.0f;

    public float WindowFollowSpeed
    {
        get { return windowFollowSpeed; }
        set { windowFollowSpeed = Mathf.Abs(value); }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        if (window == null)
        {
            return;
        }

        // Update window transformation.
        

        if (isVisible && cameraTransform != null)
        {
            float t = Time.deltaTime * windowFollowSpeed;
            window.position = Vector3.Lerp(window.position, CalculateWindowPosition(cameraTransform), t);
            window.rotation = Quaternion.Slerp(window.rotation, CalculateWindowRotation(cameraTransform), t);
            window.localScale = defaultWindowScale * windowScale;
            //CalculateBackgroundSize();
        }

        // Update visibility state.
        window.gameObject.SetActive(isVisible);
    }

   /*
    private void CalculateBackgroundSize()
    {
        if (isVisible && cameraTransform != null)
        {
            background.localPosition = backgroundOffsets[0];
            background.localScale = backgroundScales[0];
        }
        else
        {
            background.localPosition = backgroundOffsets[2];
            background.localScale = backgroundScales[2];
        }
    }
    */

    private Vector3 CalculateWindowPosition(Transform cameraTransform)
    {
        float windowDistance = Mathf.Max(16.0f / Camera.main.fieldOfView, Camera.main.nearClipPlane + 0.25f);
        Vector3 position = cameraTransform.position + (cameraTransform.forward * windowDistance);
        Vector3 horizontalOffset = cameraTransform.right * windowOffset.x;
        Vector3 verticalOffset = cameraTransform.up * windowOffset.y;

        switch (windowAnchor)
        {
            case TextAnchor.UpperLeft: position += verticalOffset - horizontalOffset; break;
            case TextAnchor.UpperCenter: position += verticalOffset; break;
            case TextAnchor.UpperRight: position += verticalOffset + horizontalOffset; break;
            case TextAnchor.MiddleLeft: position -= horizontalOffset; break;
            case TextAnchor.MiddleRight: position += horizontalOffset; break;
            case TextAnchor.LowerLeft: position -= verticalOffset + horizontalOffset; break;
            case TextAnchor.LowerCenter: position -= verticalOffset; break;
            case TextAnchor.LowerRight: position -= verticalOffset - horizontalOffset; break;
        }

        return position;
    }

    private Quaternion CalculateWindowRotation(Transform cameraTransform)
    {
        Quaternion rotation = cameraTransform.rotation;

        switch (windowAnchor)
        {
            case TextAnchor.UpperLeft: rotation *= windowHorizontalRotationInverse * windowVerticalRotationInverse; break;
            case TextAnchor.UpperCenter: rotation *= windowHorizontalRotationInverse; break;
            case TextAnchor.UpperRight: rotation *= windowHorizontalRotationInverse * windowVerticalRotation; break;
            case TextAnchor.MiddleLeft: rotation *= windowVerticalRotationInverse; break;
            case TextAnchor.MiddleRight: rotation *= windowVerticalRotation; break;
            case TextAnchor.LowerLeft: rotation *= windowHorizontalRotation * windowVerticalRotationInverse; break;
            case TextAnchor.LowerCenter: rotation *= windowHorizontalRotation; break;
            case TextAnchor.LowerRight: rotation *= windowHorizontalRotation * windowVerticalRotation; break;
        }

        return rotation;
    }
}
