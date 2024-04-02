using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorPointController : CurvePointController
{
    public ControlPointController m_controlPoint1;
    public ControlPointController m_controlPoint2;

    private LineRenderer m_lineRenderer;

    private BizerCurve m_curve = null;

    private void Awake()
    {
        m_curve = GetComponentInParent<BizerCurve>();
    }

    public ControlPointController CreateControlPoint(GameObject prefab, Vector3 pos, int index)
    {
        var pointPrefab = prefab;
        if (pointPrefab == null)
        {
            Debug.LogError("The Prefab is Null!");
            return null;
        }

        GameObject pointClone = Instantiate(pointPrefab);
        pointClone.name = pointClone.name.Replace("(Clone)", "");
        pointClone.transform.SetParent(this.transform);
        pointClone.transform.position = pos;

        var ctrl = pointClone.GetComponent<ControlPointController>();
        if (ctrl == null)
        {
            ctrl = pointClone.AddComponent<ControlPointController>();
        }
        if (index == 0)
            m_controlPoint1 = ctrl;
        if (index == 1)
            m_controlPoint2 = ctrl;
        return ctrl;
    }

    public LineRenderer LineRenderer
    {
        get
        {
            if (gameObject.tag.Equals("AnchorPoint") && !m_lineRenderer)
            {
                m_lineRenderer = gameObject.AddComponent<LineRenderer>();
                if (m_lineRenderer)
                {
                    m_lineRenderer.sortingOrder = 1;
                    m_lineRenderer.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended"));
                    m_lineRenderer.startColor = m_lineRenderer.endColor = Color.yellow;
                    m_lineRenderer.widthMultiplier = 0.03f;
                    m_lineRenderer.positionCount = 0;
                }
            }
            return m_lineRenderer;
        }
    }

    private void DrawControlLine()
    {
        if (!gameObject.tag.Equals("AnchorPoint") || (!m_controlPoint1 && !m_controlPoint2)) return;
        if (LineRenderer)
        {
            LineRenderer.positionCount = (m_controlPoint1 && m_controlPoint2) ? 3 : 2;
            if (m_controlPoint1 && !m_controlPoint2)
            {
                LineRenderer.SetPosition(0, m_controlPoint1.GetPos());
                LineRenderer.SetPosition(1, transform.position);
            }
            if (m_controlPoint2 && !m_controlPoint1)
            {
                LineRenderer.SetPosition(0, transform.position);
                LineRenderer.SetPosition(1, m_controlPoint2.GetPos());
            }
            if (m_controlPoint1 && m_controlPoint2)
            {
                LineRenderer.SetPosition(0, m_controlPoint1.GetPos());
                LineRenderer.SetPosition(1, this.GetPos());
                LineRenderer.SetPosition(2, m_controlPoint2.GetPos());
            }
        }
    }

    void Update()
    {
        DrawControlLine();
    }
}
