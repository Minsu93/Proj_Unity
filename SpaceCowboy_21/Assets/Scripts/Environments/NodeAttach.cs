using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

//[ExecuteInEditMode]     //�̰Ͷ����� Play ��尡 �ƴϾ ����Ǵ°ſ���.
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

        if (!EditorApplication.isPlaying)      //�����Ͱ� �÷��� ���� �ƴϴ�
        {
            //spriteShapeController = GetNearestPlanet().GetComponent<SpriteShapeController>();

            if (spriteShapeController == null)
                return;


            spline = spriteShapeController.spline;              //Spline Controller���� Spline�� �����´�.

            if ((spline.GetPointCount() != 0) && (lastSpritePointCount != 0))       //���� Spline �� ���� Spline ����Ʈ�� ������ 0�� �ƴϴ�. 
            {
                index = Mathf.Clamp(index, 0, spline.GetPointCount() - 1);      //Index�� �ִ� ����. 
                if (spline.GetPointCount() != lastSpritePointCount)     //spine�� ���� �޶����ٸ�
                {
                    if (spline.GetPosition(index) != lastPosition)      //lastPosition�� ���� Spline ��ġ�� �ٸ��ٸ�
                    {
                        index += spline.GetPointCount() - lastSpritePointCount;     // �ε����� ���� ��ġ���°� �����Ѵ�. 
                    }
                }


                if ((index <= spline.GetPointCount() - 1) && (index >= 0))          //��ġ Normal�� �°� ȸ����Ų��
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
                    if (localOffset)            //y������ yOffset��ŭ �̵���Ű��. ���� �������� ������ �׳� ���� �������� ���� ������ �� �ִ�.
                    {
                        offsetVector = (Vector3)Rotate(Vector2.up, transform.localEulerAngles.z) * yOffset;
                    }
                    else
                    {
                        offsetVector = Vector2.up * yOffset;
                    }

                    transform.position = spriteShapeController.transform.position + spline.GetPosition(index) + offsetVector;   //���� ��ġ ����
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