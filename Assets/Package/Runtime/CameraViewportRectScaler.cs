using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TSKT
{
    [ExecuteInEditMode]
    public class CameraViewportRectScaler : MonoBehaviour
    {
        Camera _camera;
        Camera Camera => _camera ? _camera : _camera = GetComponent<Camera>();

        [SerializeField]
        bool scaleWithScreenSize = true;

        [SerializeField]
        UnityEngine.UI.CanvasScaler.ScreenMatchMode screenMatchMode = UnityEngine.UI.CanvasScaler.ScreenMatchMode.Expand;

        [SerializeField]
        [Range(0f, 1f)]
        float match = 0f;

        [SerializeField]
        Vector2Int referenceResolution = new Vector2Int(1920, 1080);

        [SerializeField]
        Vector2 minAnchorPosition = new Vector2(0.5f, 0.5f);

        [SerializeField]
        Vector2 maxAnchorPosition = new Vector2(0.5f, 0.5f);

        [SerializeField]
        int left = 960;

        [SerializeField]
        int right = 960;

        [SerializeField]
        int top = 540;

        [SerializeField]
        int bottom = 540;

        Vector2Int previousScreenSize;

        void Update()
        {
            if (previousScreenSize.x == Screen.width
                && previousScreenSize.y == Screen.height)
            {
#if !UNITY_EDITOR
                return;
#endif
            }
            previousScreenSize = new Vector2Int(Screen.width, Screen.height);

            var scale = GetScale();
            var referenceScreenSize = new Vector2(
                Screen.width / scale,
                Screen.height / scale);

            var xMinParent = -referenceScreenSize.x / 2;
            var xMaxParent = referenceScreenSize.x / 2;
            var yMinParent = -referenceScreenSize.y / 2;
            var yMaxParent = referenceScreenSize.y / 2;

            var xMinAnchor = Mathf.Lerp(xMinParent, xMaxParent, minAnchorPosition.x);
            var xMaxAnchor = Mathf.Lerp(xMinParent, xMaxParent, maxAnchorPosition.x);
            var yMinAnchor = Mathf.Lerp(yMinParent, yMaxParent, minAnchorPosition.y);
            var yMaxAnchor = Mathf.Lerp(yMinParent, yMaxParent, maxAnchorPosition.y);

            var xMin = xMinAnchor - left;
            var xMax = xMaxAnchor + right;
            var yMin = yMinAnchor - bottom;
            var yMax = yMaxAnchor + top;

            var rect = new Rect(
                xMin * scale + Screen.width / 2,
                yMin * scale + Screen.height / 2,
                (xMax - xMin) * scale,
                (yMax - yMin) * scale);
            Camera.pixelRect = rect;
        }

        float GetScale()
        {
            if (!scaleWithScreenSize)
            {
                return 1f;
            }

            var scaleByHeight = (float)Screen.height / referenceResolution.y;
            var scaleByWidth = (float)Screen.width / referenceResolution.x;

            var horizontalLongScreen = Screen.width * referenceResolution.y > referenceResolution.x * Screen.height;

            float factor;
            switch (screenMatchMode)
            {
                case UnityEngine.UI.CanvasScaler.ScreenMatchMode.Expand:
                    factor = horizontalLongScreen ? 1f : 0f;
                    break;
                case UnityEngine.UI.CanvasScaler.ScreenMatchMode.MatchWidthOrHeight:
                    factor = match;
                    break;
                case UnityEngine.UI.CanvasScaler.ScreenMatchMode.Shrink:
                    factor = horizontalLongScreen ? 0f : 1f;
                    break;
                default:
                    Debug.LogError("unknown math mode : " + screenMatchMode.ToString());
                    factor = 0f;
                    break;
            }

            return Mathf.Lerp(scaleByWidth, scaleByHeight, factor);
        }
    }
}
