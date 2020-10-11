using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TSKT
{
    public class CaptureScreenshotSample : MonoBehaviour
    {
        IEnumerator Start()
        {
            Texture2D texture = null;
            yield return TextureUtil.CaptureScreenshot(100, 100, _ => texture = _);
            var bytes = texture.EncodeToJPG();
            System.IO.File.WriteAllBytes("ss.jpg", bytes);
        }
    }
}
