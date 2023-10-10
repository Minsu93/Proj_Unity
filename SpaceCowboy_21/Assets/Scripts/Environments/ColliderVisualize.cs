using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]

public class ColliderVisualize : MonoBehaviour
{
    public bool showLine = true;
    public Color lineColor = Color.blue;

    private PolygonCollider2D[] thisColliders;
    private List<Vector2[]> pointsList;
    private List<Vector3> _tList;

    void OnDrawGizmos()
    {

        if (!showLine)
        {
            return;
        }

        pointsList = new List<Vector2[]>();
        _tList = new List<Vector3>();

        Gizmos.matrix = transform.localToWorldMatrix;


        thisColliders = gameObject.GetComponents<PolygonCollider2D>();
        foreach (PolygonCollider2D c in thisColliders)
        {

            Vector2[] pointWithOffset = new Vector2[c.points.Length];

            for (int i = 0; i < c.points.Length; i++)
            {
                pointWithOffset[i] = c.points[i] + c.offset;
            }

            pointsList.Add(pointWithOffset);
            _tList.Add(c.transform.position);
        }

        for (int j = 0; j < pointsList.Count; j++)
        {

            Vector2[] points = pointsList[j];
            Vector3 _t = _tList[j];

            Gizmos.color = lineColor;

            for (int i = 0; i < points.Length - 1; i++)
            {
                Gizmos.DrawLine(new Vector3(points[i].x + _t.x, points[i].y + _t.y), new Vector3(points[i + 1].x + _t.x, points[i + 1].y + _t.y));
            }

            Gizmos.DrawLine(new Vector3(points[points.Length - 1].x + _t.x, points[points.Length - 1].y + _t.y), new Vector3(points[0].x + _t.x, points[0].y + _t.y));

        }



    }
}
