using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CurvePointControl : MonoBehaviour
{
    [Header("锁定X轴")]
    public bool m_isLockX = false;
    [Header("锁定Y轴")]
    public bool m_isLockY = true;
    [Header("锁定Z轴")]
    public bool m_isLockZ = false;

    [HideInInspector]
    public GameObject m_controlObject;
    [HideInInspector]
    public GameObject m_controlObject2;

    private Vector3 m_offsetPos1 = Vector3.zero;
    private Vector3 m_offsetPos2 = Vector3.zero;
    private LineRenderer m_lineRenderer;

    private BizerCurve m_curve = null;

    private void Awake()
    {
        m_curve = GetComponentInParent<BizerCurve>();
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

    void OnMouseDown()
    {
        //Debug.Log("CurvePointControl:OnMouseDown");
        if (!gameObject.tag.Equals("AnchorPoint")) return;
        OffsetPos();
    }

    public List<Vector3> OffsetPos()
    {
        List<Vector3> offsetPosList = new List<Vector3>();
        if (m_controlObject)
            m_offsetPos1 = m_controlObject.transform.position - transform.position;
        if (m_controlObject2)
            m_offsetPos2 = m_controlObject2.transform.position - transform.position;
        offsetPosList.Add(m_offsetPos1);
        offsetPosList.Add(m_offsetPos2);

        return offsetPosList;
    }

    void OnMouseDrag()
    {
        Vector3 pos0 = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, pos0.z);
        Vector3 mousePosInWorld = Camera.main.ScreenToWorldPoint(mousePos);
        Vector3 thisPos = mousePosInWorld;
        if (m_isLockX)
            thisPos.x = transform.position.x;
        if (m_isLockY)
            thisPos.y = transform.position.y;
        if (m_isLockZ)
            thisPos.z = transform.position.z;
        transform.position = thisPos;
        m_curve.UpdateLine(gameObject, m_offsetPos1, m_offsetPos2);
    }

    private void DrawControlLine()
    {
        if (!gameObject.tag.Equals("AnchorPoint") || (!m_controlObject && !m_controlObject2)) return;
        if (LineRenderer)
        {
            LineRenderer.positionCount = (m_controlObject && m_controlObject2) ? 3 : 2;
            if (m_controlObject && !m_controlObject2)
            {
                LineRenderer.SetPosition(0, m_controlObject.transform.position);
                LineRenderer.SetPosition(1, transform.position);
            }
            if (m_controlObject2 && !m_controlObject)
            {
                LineRenderer.SetPosition(0, transform.position);
                LineRenderer.SetPosition(1, m_controlObject2.transform.position);
            }
            if (m_controlObject && m_controlObject2)
            {
                LineRenderer.SetPosition(0, m_controlObject.transform.position);
                LineRenderer.SetPosition(1, transform.position);
                LineRenderer.SetPosition(2, m_controlObject2.transform.position);
            }
        }
    }

    void Update()
    {
        DrawControlLine();
    }

}
