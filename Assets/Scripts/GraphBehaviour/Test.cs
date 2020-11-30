// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text;
using UnityEngine;
using UnityEngine.Profiling;

#if WINDOWS_UWP
using Windows.System;
#endif

namespace Microsoft.MixedReality.Toolkit.Diagnostics
{
    /// <summary>
    /// 
    /// ABOUT: The VisualProfiler provides a drop in, single file, solution for viewing 
    /// your Windows Mixed Reality Unity application's frame rate and memory usage. Missed 
    /// frames are displayed over time to visually find problem areas. Memory is reported 
    /// as current, peak and max usage in a bar graph. 
    /// 
    /// USAGE: To use this profiler simply add this script as a component of any GameObject in 
    /// your Unity scene. The profiler is initially enabled (toggle-able via the initiallyActive 
    /// property), but can be toggled via the enabled/disable voice commands keywords.
    /// 
    /// </summary>
    public class Test : MonoBehaviour
    {
        private static readonly int maxStringLength = 32;
        private static readonly int maxTargetFrameRate = 120;
        private static readonly int maxFrameTimings = 128;
        private static readonly int frameRange = 30;
        public Vector3 defaultWindowPosition = new Vector3(0.0f, 0.0f, 2.0f);
        public Vector2 defaultWindowRotation = new Vector2(0f, 0f);
        public Vector3 defaultWindowScale = new Vector3(0.00125f, 0.00125f, 0.0f);
        //public Vector2 cursorViewOffset = new Vector2(20.0f, 30.0f);
        private static readonly Vector3[] backgroundScales = { new Vector3(1.0f, 1.0f, 0.0f), new Vector3(1.0f, 0.5f, 0.0f), new Vector3(1.0f, 0.25f, 0.0f) };
        private static readonly Vector3[] backgroundOffsets = { new Vector3(0.0f, 0.0f, 2.0f), new Vector3(0.0f, 0.25f, 2.0f), new Vector3(0.0f, 0.375f, 2.0f) };

        public Transform WindowParent { get; set; } = null;

        [Header("Profiler Settings")]
        [SerializeField, Tooltip("Is the profiler currently visible.")]
        private bool isVisible = false;

        public bool IsVisible
        {
            get { return isVisible; }
            set { isVisible = value; }
        }


        [Header("Window Settings")]
        [SerializeField, Tooltip("What part of the view port to anchor the window to.")]
        private TextAnchor windowAnchor = TextAnchor.MiddleCenter;

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

        [SerializeField, Tooltip("The offset from the view port center applied based on the window anchor selection.")]
        private Vector2 cursorViewOffset = new Vector2(20.0f, 30.0f);

        public Vector2 CursorViewOffset
        {
            get { return windowOffset; }
            set { windowOffset = value; }
        }

        [SerializeField, Range(0.1f, 5.0f), Tooltip("Use to scale the window size up or down, can simulate a zooming effect.")]
        private float windowScale = 0.25f;

        public float WindowScale
        {
            get { return windowScale; }
            set { windowScale = Mathf.Clamp(value, 0.1f, 5.0f); }
        }

        [SerializeField, Range(0.0f, 50.0f), Tooltip("How quickly to interpolate the window towards its target position and rotation.")]
        private float windowFollowSpeed = 1.0f;

        public float WindowFollowSpeed
        {
            get { return windowFollowSpeed; }
            set { windowFollowSpeed = Mathf.Abs(value); }
        }

        [Header("UI Settings")]
        [SerializeField, Range(0, 3), Tooltip("How many decimal places to display on numeric strings.")]
        private int displayedDecimalDigits = 1;


        [SerializeField, Tooltip("The color of the window backplate.")]
        private Color baseColor = new Color(80 / 256.0f, 80 / 256.0f, 80 / 256.0f, 1.0f);

        private Transform window;
        private Transform background;
        private TextMesh cpuFrameRateText;
        private TextMesh gpuFrameRateText;
        private Transform memoryStats;
        private TextMesh usedMemoryText;
        private TextMesh peakMemoryText;
        private TextMesh limitMemoryText;
        private Transform usedAnchor;
        private Transform peakAnchor;
        private Quaternion windowHorizontalRotation;
        private Quaternion windowHorizontalRotationInverse;
        private Quaternion windowVerticalRotation;
        private Quaternion windowVerticalRotationInverse;

        private Matrix4x4[] frameInfoMatrices;
        private Vector4[] frameInfoColors;
        private MaterialPropertyBlock frameInfoPropertyBlock;
        private int colorID;
        private int parentMatrixID;
        private int frameCount;
        private System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        private FrameTiming[] frameTimings = new FrameTiming[maxFrameTimings];
        private string[] cpuFrameRateStrings;
        private string[] gpuFrameRateStrings;
        private char[] stringBuffer = new char[maxStringLength];

        private ulong memoryUsage;
        private ulong peakMemoryUsage;
        private ulong limitMemoryUsage;

        // Rendering resources.
        [SerializeField, HideInInspector]
        private Material defaultMaterial;
        [SerializeField, HideInInspector]
        private Material defaultInstancedMaterial;
        private Material backgroundMaterial;
        private Material foregroundMaterial;
        private Material textMaterial;
        private Mesh quadMesh;

        private void Reset()
        {
            if (defaultMaterial == null)
            {
                defaultMaterial = new Material(Shader.Find("Hidden/Internal-Colored"));
                defaultMaterial.SetFloat("_ZWrite", 0.0f);
                defaultMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Disabled);
                defaultMaterial.renderQueue = 5000;
            }

            if (defaultInstancedMaterial == null)
            {
                Shader defaultInstancedShader = Shader.Find("Hidden/Instanced-Colored");

                if (defaultInstancedShader != null)
                {
                    defaultInstancedMaterial = new Material(defaultInstancedShader);
                    defaultInstancedMaterial.enableInstancing = true;
                    defaultInstancedMaterial.SetFloat("_ZWrite", 0.0f);
                    defaultInstancedMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Disabled);
                    defaultInstancedMaterial.renderQueue = 5000;
                }
                else
                {
                    Debug.LogWarning("A shader supporting instancing could not be found for the VisualProfiler, falling back to traditional rendering. This may impact performance.");
                }
            }

            if (Application.isPlaying)
            {
                backgroundMaterial = new Material(defaultMaterial);
                foregroundMaterial = new Material(defaultMaterial);
                defaultMaterial.renderQueue = foregroundMaterial.renderQueue - 1;
                backgroundMaterial.renderQueue = defaultMaterial.renderQueue - 1;

                MeshRenderer meshRenderer = new GameObject().AddComponent<TextMesh>().GetComponent<MeshRenderer>();
                textMaterial = new Material(meshRenderer.sharedMaterial);
                textMaterial.renderQueue = defaultMaterial.renderQueue;
                Destroy(meshRenderer.gameObject);

                MeshFilter quadMeshFilter = GameObject.CreatePrimitive(PrimitiveType.Quad).GetComponent<MeshFilter>();

                if (defaultInstancedMaterial != null)
                {
                    // Create a quad mesh with artificially large bounds to disable culling for instanced rendering.
                    // TODO: Use shared mesh with normal bounds once Unity allows for more control over instance culling.
                    quadMesh = quadMeshFilter.mesh;
                    quadMesh.bounds = new Bounds(Vector3.zero, Vector3.one * float.MaxValue);
                }
                else
                {
                    quadMesh = quadMeshFilter.sharedMesh;
                }

                Destroy(quadMeshFilter.gameObject);
            }

            stopwatch.Reset();
            stopwatch.Start();
        }

        private void Start()
        {
            Reset();
            BuildWindow();
        }

        private void OnDestroy()
        {
            if (window != null)
            {
                Destroy(window.gameObject);
            }
        }

        private void LateUpdate()
        {
            if (window == null)
            {
                return;
            }

            // Update window transformation.
            Transform cameraTransform = Camera.main ? Camera.main.transform : null;

            if (isVisible && cameraTransform != null)
            {
                if (Mathf.Abs(cameraTransform.localRotation.x) > (CursorViewOffset.x + Mathf.Abs(window.localRotation.x)) 
                    || Mathf.Abs(cameraTransform.localRotation.y) > (CursorViewOffset.y + Mathf.Abs(window.localRotation.y)))
                {
                    float t = Time.deltaTime * windowFollowSpeed;
                    window.position = Vector3.Lerp(window.position, CalculateWindowPosition(cameraTransform), t);
                    window.rotation = Quaternion.Slerp(window.rotation, CalculateWindowRotation(cameraTransform), t);
                    window.localScale = defaultWindowScale * windowScale;
                    CalculateBackgroundSize();
                }

            }
            // Update visibility state.
            window.gameObject.SetActive(isVisible);
        }

        private Vector3 CalculateWindowPosition(Transform cameraTransform)
        {
            float windowDistance = Mathf.Max(16.0f / Camera.main.fieldOfView, Camera.main.nearClipPlane + 0.25f);
            Vector3 position = cameraTransform.position + (cameraTransform.forward * windowDistance);
            Vector3 horizontalOffset = cameraTransform.right * windowOffset.x;
            Vector3 verticalOffset = cameraTransform.up * windowOffset.y;

            switch (windowAnchor)
            {
                //case TextAnchor.UpperLeft: position += verticalOffset - horizontalOffset; break;
                //case TextAnchor.UpperCenter: position += verticalOffset; break;
                //case TextAnchor.UpperRight: position += verticalOffset + horizontalOffset; break;
                //case TextAnchor.MiddleLeft: position -= horizontalOffset; break;
                case TextAnchor.MiddleRight: position += horizontalOffset; break;
                //case TextAnchor.LowerLeft: position -= verticalOffset + horizontalOffset; break;
                //case TextAnchor.LowerCenter: position -= verticalOffset; break;
                //case TextAnchor.LowerRight: position -= verticalOffset - horizontalOffset; break;
            }

            return position;
        }

        private Quaternion CalculateWindowRotation(Transform cameraTransform)
        {
            Quaternion rotation = cameraTransform.rotation;

            switch (windowAnchor)
            {
                //case TextAnchor.UpperLeft: rotation *= windowHorizontalRotationInverse * windowVerticalRotationInverse; break;
                //case TextAnchor.UpperCenter: rotation *= windowHorizontalRotationInverse; break;
                //case TextAnchor.UpperRight: rotation *= windowHorizontalRotationInverse * windowVerticalRotation; break;
                //case TextAnchor.MiddleLeft: rotation *= windowVerticalRotationInverse; break;
                case TextAnchor.MiddleRight: rotation *= windowVerticalRotation; break;
                //case TextAnchor.LowerLeft: rotation *= windowHorizontalRotation * windowVerticalRotationInverse; break;
                //case TextAnchor.LowerCenter: rotation *= windowHorizontalRotation; break;
                //case TextAnchor.LowerRight: rotation *= windowHorizontalRotation * windowVerticalRotation; break;
            }

            return rotation;
        }


        private void CalculateBackgroundSize()
        {
            background.localPosition = backgroundOffsets[2];
            background.localScale = backgroundScales[2];
        }

        private void BuildWindow()
        {
            // Initialize property block state.
            //colorID = Shader.PropertyToID("_Color");
            //parentMatrixID = Shader.PropertyToID("_ParentLocalToWorldMatrix");

            // Build the window root.
            {

                //window = new GameObject("VisualProfiler").transform;
                window = GameObject.Find("AppContent").GetComponent<Transform>();
                window.parent = WindowParent;
                window.localPosition = defaultWindowPosition;
                defaultWindowScale = window.transform.localScale;
                windowHorizontalRotation = Quaternion.AngleAxis(defaultWindowRotation.y, Vector3.right);
                windowHorizontalRotationInverse = Quaternion.Inverse(windowHorizontalRotation);
                windowVerticalRotation = Quaternion.AngleAxis(defaultWindowRotation.x, Vector3.up);
                windowVerticalRotationInverse = Quaternion.Inverse(windowVerticalRotation);
            }

            // Build the window background.
            {
                background = CreateQuad("Background", window).transform;
                InitializeRenderer(background.gameObject, backgroundMaterial, colorID, baseColor);
                CalculateBackgroundSize();
            }

            window.gameObject.SetActive(isVisible);
        }

 

        private static Transform CreateAnchor(string name, Transform parent)
        {
            Transform anchor = new GameObject(name).transform;
            anchor.parent = parent;
            anchor.localScale = Vector3.one;
            anchor.localPosition = new Vector3(-0.5f, 0.0f, 0.0f);

            return anchor;
        }

        private static GameObject CreateQuad(string name, Transform parent)
        {
            GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            Destroy(quad.GetComponent<Collider>());
            quad.name = name;
            quad.transform.parent = parent;

            return quad;
        }

        private static Renderer InitializeRenderer(GameObject obj, Material material, int colorID, Color color)
        {
            Renderer renderer = obj.GetComponent<Renderer>();
            renderer.sharedMaterial = material;

            MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor(colorID, color);
            renderer.SetPropertyBlock(propertyBlock);

            OptimizeRenderer(renderer);

            return renderer;
        }

        private static void OptimizeRenderer(Renderer renderer)
        {
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = false;
            renderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
            renderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            renderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
            renderer.allowOcclusionWhenDynamic = false;
        }
    }
}

