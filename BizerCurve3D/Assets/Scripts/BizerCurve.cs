using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(LineRenderer))]
public class BizerCurve : MonoBehaviour
{
    public List<Transform> m_allPoints;
    private GameObject m_anchorPoint;
    private GameObject m_controlPoint;
    private GameObject m_pointParent;
    private LineRenderer m_lineRenderer;

    private int m_curveCount = 0;
    private int SEGMENT_COUNT = 60;//曲线取点个数（取点越多这个长度越趋向于精确）


    void Awake()
    {
        SetLine();
        if (null == m_anchorPoint)
            m_anchorPoint = Resources.Load("Prefabs/AnchorPoint") as GameObject;
        if (null == m_controlPoint)
            m_controlPoint = Resources.Load("Prefabs/ControlPoint") as GameObject;
    }

    #region 显示相关

    void SetLine()
    {
        if (null == m_lineRenderer)
            m_lineRenderer = GetComponent<LineRenderer>();
        m_lineRenderer.material = Resources.Load("Materials/Line") as Material;
        m_lineRenderer.startColor = Color.red;
        m_lineRenderer.endColor = Color.green;
        m_lineRenderer.widthMultiplier = 0.2f;
    }


    public List<Vector3> HiddenLine(bool isHidden = false)
    {
        m_pointParent.SetActive(isHidden);
        m_lineRenderer.enabled = isHidden;
        List<Vector3> pathPoints = new List<Vector3>();
        if (!isHidden)
        {
            for (int i = 0; i < m_lineRenderer.positionCount; i++)
            {
                pathPoints.Add(m_lineRenderer.GetPosition(i));
            }
        }
        return pathPoints;
    }

    private void DrawCurve()//画曲线
    {
        if (m_allPoints.Count < 4) return;
        m_curveCount = (int)m_allPoints.Count / 3;
        for (int j = 0; j < m_curveCount; j++)
        {
            for (int i = 1; i <= SEGMENT_COUNT; i++)
            {
                float t = (float)i / (float)SEGMENT_COUNT;
                int nodeIndex = j * 3;
                Vector3 pixel = CalculateCubicBezierPoint(t, m_allPoints[nodeIndex].position, m_allPoints[nodeIndex + 1].position, m_allPoints[nodeIndex + 2].position, m_allPoints[nodeIndex + 3].position);
                m_lineRenderer.positionCount = j * SEGMENT_COUNT + i;
                m_lineRenderer.SetPosition((j * SEGMENT_COUNT) + (i - 1), pixel);
            }
        }
    }

    #endregion

    #region 编辑相关
    private GameObject LoadPoint(GameObject pointPrefab, Vector3 pos)
    {
        if (pointPrefab == null)
        {
            Debug.LogError("The Prefab is Null!");
            return null;
        }
        if (null == m_pointParent)
        {
            m_pointParent = new GameObject("AllPoints");
            m_pointParent.transform.parent = this.transform;
        }

        GameObject pointClone = Instantiate(pointPrefab);
        pointClone.name = pointClone.name.Replace("(Clone)", "");
        pointClone.transform.SetParent(m_pointParent.transform);
        pointClone.transform.position = pos;

        return pointClone;
    }

    public void AddPoint(Vector3 anchorPointPos)
    {
        //初始化时m_allPoints添加了一个player
        if (m_allPoints.Count == 0)
        {
            GameObject anchorPoint0 = LoadPoint(m_anchorPoint, anchorPointPos);
            m_allPoints.Add(anchorPoint0.transform);
            return;
        }
        Transform lastPoint = m_allPoints[m_allPoints.Count - 1];
        GameObject controlPoint2 = LoadPoint(m_controlPoint, lastPoint.position + new Vector3(0, 0, -1));
        GameObject controlPoint = LoadPoint(m_controlPoint, anchorPointPos + new Vector3(0, 0, 1));
        GameObject anchorPoint = LoadPoint(m_anchorPoint, anchorPointPos);

        anchorPoint.GetComponent<CurvePointControl>().m_controlObject = controlPoint;
        lastPoint.GetComponent<CurvePointControl>().m_controlObject2 = controlPoint2;

        m_allPoints.Add(controlPoint2.transform);
        m_allPoints.Add(controlPoint.transform);
        m_allPoints.Add(anchorPoint.transform);

        DrawCurve();
    }
    public void DeletePoint(GameObject anchorPoint)
    {
        if (anchorPoint == null) return;
        CurvePointControl curvePoint = anchorPoint.GetComponent<CurvePointControl>();
        if (curvePoint && anchorPoint.tag.Equals("AnchorPoint"))
        {
            if (curvePoint.m_controlObject)
            {
                m_allPoints.Remove(curvePoint.m_controlObject.transform);
                Destroy(curvePoint.m_controlObject);
            }
            if (curvePoint.m_controlObject2)
            {
                m_allPoints.Remove(curvePoint.m_controlObject2.transform);
                Destroy(curvePoint.m_controlObject2);
            }
            if (m_allPoints.IndexOf(curvePoint.transform) == (m_allPoints.Count - 1))
            {//先判断删除的是最后一个元素再移除
                m_allPoints.Remove(curvePoint.transform);
                Transform lastPoint = m_allPoints[m_allPoints.Count - 2];
                GameObject lastPointCtrObject = lastPoint.GetComponent<CurvePointControl>().m_controlObject2;
                if (lastPointCtrObject)
                {
                    m_allPoints.Remove(lastPointCtrObject.transform);
                    Destroy(lastPointCtrObject);
                    lastPoint.GetComponent<CurvePointControl>().m_controlObject2 = null;
                }
            }
            else
            {
                m_allPoints.Remove(curvePoint.transform);
            }
            Destroy(anchorPoint);
            if (m_allPoints.Count == 1)
            {
                m_lineRenderer.positionCount = 0;
            }
        }

        DrawCurve();
    }

    public void UpdateLine(GameObject anchorPoint, Vector3 offsetPos1, Vector3 offsetPos2)
    {
        if (anchorPoint == null) return;
        if (anchorPoint.tag.Equals("AnchorPoint"))
        {
            CurvePointControl curvePoint = anchorPoint.GetComponent<CurvePointControl>();
            if (curvePoint)
            {
                if (curvePoint.m_controlObject)
                    curvePoint.m_controlObject.transform.position = anchorPoint.transform.position + offsetPos1;
                if (curvePoint.m_controlObject2)
                    curvePoint.m_controlObject2.transform.position = anchorPoint.transform.position + offsetPos2;
            }
        }
        DrawCurve();
    }

    #endregion

    //贝塞尔曲线公式：B(t)=P0*(1-t)^3 + 3*P1*t(1-t)^2 + 3*P2*t^2*(1-t) + P3*t^3 ,t属于[0,1].
    Vector3 CalculateCubicBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 p = uuu * p0;
        p += 3 * uu * t * p1;
        p += 3 * u * tt * p2;
        p += ttt * p3;

        return p;
    }
}
