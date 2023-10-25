using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

//[ExecuteInEditMode]     //이것때문에 Play 모드가 아니어도 실행되는거였음.
public class NodeAttach : MonoBehaviour
{
    public SpriteShapeController spriteShapeController;
    public int index;
    public bool useNormals = false;
    [Header("Offset")]
    public float yOffset = 0.0f;
    public bool localOffset = false;
    private Spline spline;
    private int lastSpritePointCount;
    private Vector3 lastPosition;

    void Awake()
    {
        if(spriteShapeController != null)
            spline = spriteShapeController.spline;
    }

    /*
    void Update()
    {
        if (EditorApplication.isPlaying)
            this.enabled = false;

        if (!EditorApplication.isPlaying)      //에디터가 플레이 중이 아니다
        {
            //spriteShapeController = GetNearestPlanet().GetComponent<SpriteShapeController>();

            if (spriteShapeController == null)
                return;


            spline = spriteShapeController.spline;              //Spline Controller에서 Spline을 가져온다.

            if ((spline.GetPointCount() != 0) && (lastSpritePointCount != 0))       //현재 Spline 및 지난 Spline 포인트의 개수가 0이 아니다. 
            {
                index = Mathf.Clamp(index, 0, spline.GetPointCount() - 1);      //Index의 최대 제한. 
                if (spline.GetPointCount() != lastSpritePointCount)     //spine의 수가 달라졌다면
                {
                    if (spline.GetPosition(index) != lastPosition)      //lastPosition과 현재 Spline 위치가 다르다면
                    {
                        index += spline.GetPointCount() - lastSpritePointCount;     // 인덱스를 현재 위치에맞게 조절한다. 
                    }
                }


                if ((index <= spline.GetPointCount() - 1) && (index >= 0))          //위치 Normal에 맞게 회전시킨다
                {
                    if (useNormals)
                    {
                        if (spline.GetTangentMode(index) != ShapeTangentMode.Linear)
                        {
                            Vector3 lt = Vector3.Normalize(spline.GetLeftTangent(index) - spline.GetRightTangent(index));
                            float a = Angle(Vector3.left, lt);

                            transform.rotation = Quaternion.Euler(0, 0, a);
                        }
                    }
                    else
                    {
                        transform.rotation = Quaternion.Euler(0, 0, 0);
                    }

                    Vector3 offsetVector;
                    if (localOffset)            //y축으로 yOffset만큼 이동시키기. 로컬 방향으로 가는지 그냥 월드 방향으로 갈지 지정할 수 있다.
                    {
                        offsetVector = (Vector3)Rotate(Vector2.up, transform.localEulerAngles.z) * yOffset;
                    }
                    else
                    {
                        offsetVector = Vector2.up * yOffset;
                    }

                    transform.position = spriteShapeController.transform.position + spline.GetPosition(index) + offsetVector;   //최종 위치 설정
                    lastPosition = spline.GetPosition(index);
                }
            }
        }
        lastSpritePointCount = spline.GetPointCount();
    }

    */

    private float Angle(Vector3 a, Vector3 b)
    {
        float dot = Vector3.Dot(a, b);
        float det = (a.x * b.y) - (b.x * a.y);
        return Mathf.Atan2(det, dot) * Mathf.Rad2Deg;
    }

    private Vector2 Rotate(Vector2 v, float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad;
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);
        float tx = v.x;
        float ty = v.y;
        return new Vector2(cos * tx - sin * ty, sin * tx + cos * ty);
    }


   
}