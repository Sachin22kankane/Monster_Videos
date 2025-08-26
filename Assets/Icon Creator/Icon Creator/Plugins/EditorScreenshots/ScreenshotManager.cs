using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif
[ExecuteInEditMode]
public class ScreenshotManager : MonoBehaviour {
#if UNITY_EDITOR
    public int printIndex;
    public int[] editorScreenSizeIndices;
    public string screenshotsFolderName = "Screenshots";
    [TextArea (2, 4)] public string path;

    private void Start () {
        path = (Application.dataPath.Replace ("/Assets", "")) + "/" + screenshotsFolderName;
        if (Directory.Exists (path) == false) {
            Directory.CreateDirectory (path);
            print ("Created folder at (" + path + ")");
        }

        for (int i = 0; i < editorScreenSizeIndices.Length; i++) {
            string s = GameViewUtils.GetViewDimentions (editorScreenSizeIndices[i]);
            if (Directory.Exists (path + "/" + s) == false) {
                Directory.CreateDirectory (path + "/" + s);
            }
        }
    }

    void PrintSizesTillIndex () {
        for (int i = 0; i <= printIndex; i++) {
            print (i + ". " + GameViewUtils.GetViewDimentions (i));
        }
    }


    public void EditorScreenshot () {
        StartCoroutine (EditorScreenshotRoutine ());
    }

    IEnumerator EditorScreenshotRoutine () {
        float temp = Time.timeScale;
        Time.timeScale = 0.05f;
        path = (Application.dataPath.Replace ("/Assets", "")) + "/" + screenshotsFolderName;
        if (Directory.Exists (path) == false) {
            Directory.CreateDirectory (path);
            yield return null;
        }
        for (int i = 0; i < editorScreenSizeIndices.Length; i++) {
            if (Directory.Exists (path + "/" + Screen.width + "x" + Screen.height) == false) {
                Directory.CreateDirectory (path + "/" + Screen.width + "x" + Screen.height);
                yield return null;
            }
        }
        // TimeController.instance.isTakingScreenShot = true;
        for (int i = 0; i < editorScreenSizeIndices.Length; i++) {
            yield return null;
            GameViewUtils.SetSize (editorScreenSizeIndices[i]);
            yield return null;
            // print(Screen.width + ", " + Screen.height);
            ScreenCapture.CaptureScreenshot (screenshotsFolderName + "/" + Screen.width + "x" + Screen.height + "/" + "Screenshot (" + Screen.width + "x" + Screen.height + ")_ " + System.DateTime.Now.ToString ("dd-mm-yyyy hh-mm-ss") + ".png", 1);
            yield return null;
        }
        // TimeController.instance.isTakingScreenShot = false;
        Time.timeScale = temp;
    }
    private void Update () {
        if (Input.GetKeyDown (KeyCode.C)) {
            EditorScreenshot ();
        }
    }
    private void OnValidate () {
        path = (Application.dataPath.Replace ("/Assets", "")) + "/" + screenshotsFolderName;
    }
#endif
}