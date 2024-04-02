﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DM.Editor.View
{
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

        private Vector3 offsetPos1 = Vector3.zero;
        private Vector3 offsetPos2 = Vector3.zero;
        private LineRenderer lineRenderer;

        void Start()
        {
            if (gameObject.tag.Equals("AnchorPoint") && !lineRenderer)
                lineRenderer = gameObject.AddComponent<LineRenderer>();
            if (lineRenderer)
            {
                lineRenderer.sortingOrder = 1;
                lineRenderer.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended"));
                lineRenderer.startColor = lineRenderer.endColor = Color.yellow;
                lineRenderer.widthMultiplier = 0.03f;
                lineRenderer.positionCount = 0;
            }
        }

        void OnMouseDown()
        {
            if (!gameObject.tag.Equals("AnchorPoint")) return;
            OffsetPos();
        }

        public List<Vector3> OffsetPos()
        {
            List<Vector3> offsetPosList = new List<Vector3>();
            if (m_controlObject)
                offsetPos1 = m_controlObject.transform.position - transform.position;
            if (m_controlObject2)
                offsetPos2 = m_controlObject2.transform.position - transform.position;
            offsetPosList.Add(offsetPos1);
            offsetPosList.Add(offsetPos2);

            return offsetPosList;
        }

        void OnMouseDrag()
        {
            //if (gameObject.tag.Equals("AnchorPoint")) return;
            Vector3 pos0 = Camera.main.WorldToScreenPoint(transform.position);
            Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, pos0.z);
            Vector3 mousePosInWorld= Camera.main.ScreenToWorldPoint(mousePos);
            Vector3 thisPos = mousePosInWorld;
            if (m_isLockX)
                thisPos.x = transform.position.x;
            if (m_isLockY)
                thisPos.y = transform.position.y;
            if (m_isLockZ)
                thisPos.z = transform.position.z;
            transform.position = thisPos;
            DMDrawCurve.Instance.UpdateLine(gameObject, offsetPos1, offsetPos2);   
        }      

        private void DrawControlLine()
        {
            if (!gameObject.tag.Equals("AnchorPoint") || (!m_controlObject && !m_controlObject2)) return;
            if (lineRenderer)
            {
                lineRenderer.positionCount = (m_controlObject && m_controlObject2) ? 3 : 2;
                if (m_controlObject && !m_controlObject2)
                {
                    lineRenderer.SetPosition(0, m_controlObject.transform.position);
                    lineRenderer.SetPosition(1, transform.position);
                }
                if (m_controlObject2 && !m_controlObject)
                {
                    lineRenderer.SetPosition(0, transform.position);
                    lineRenderer.SetPosition(1, m_controlObject2.transform.position);
                }
                if (m_controlObject && m_controlObject2)
                {
                    lineRenderer.SetPosition(0, m_controlObject.transform.position);
                    lineRenderer.SetPosition(1, transform.position);
                    lineRenderer.SetPosition(2, m_controlObject2.transform.position);
                }
            }
        }

        void Update()
        {
            DrawControlLine();
        }

    }
}