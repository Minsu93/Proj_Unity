using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
/// <summary>
/// 
/// </summary>
[ExecuteInEditMode]
public class Pointer : MonoBehaviour
{
    private void OnEnable()
    {
        if (!Application.isEditor)
        {
            Destroy(this);
        }
        SceneView.onSceneGUIDelegate += OnScene;
    }

    void OnScene(SceneView scene)
    {
        Event e = Event.current;

        if (e.type == EventType.MouseDown && e.button == 0)
        {
            Debug.Log("Mouse was pressed");

            Vector3 mousePos = e.mousePosition;
            float ppp = EditorGUIUtility.pixelsPerPoint;
            mousePos.y = scene.camera.pixelHeight - mousePos.y * ppp;
            mousePos.x *= ppp;

            Ray ray = scene.camera.ScreenPointToRay(mousePos);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                //Do something,
                if (hit.transform.TryGetComponent<AttachToPlanet>(out AttachToPlanet atp))
                {
                    atp.activate = false;
                    Debug.Log("activate false");
                }
            }
            e.Use();
        }
    }
}
#endif