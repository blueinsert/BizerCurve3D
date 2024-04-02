using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DM.Editor.View
{
    public class Test : MonoBehaviour
    {
        public GameObject m_player;
        public List<Vector3> m_pathPoints;

        // Use this for initialization
        void Start()
        {

        }

        IEnumerator Move()
        {
            if (m_pathPoints.Count == 0) yield break;
            int item = 1;
            while (true)
            {
                m_player.transform.LookAt(m_pathPoints[item]);
                m_player.transform.position = Vector3.Lerp(m_pathPoints[item - 1], m_pathPoints[item], 1f);
                item++;
                if (item >= m_pathPoints.Count)
                {
                    item = 1;
                    yield break;
                }
                yield return new WaitForEndOfFrame();
            }
        }

        void Update()
        {
            if (Input.GetKey(KeyCode.LeftControl) && (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1)))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (Input.GetMouseButtonUp(0) && hit.collider.tag.Equals("Terrain"))
                    {
                        Vector3 pointPos = new Vector3(hit.point.x, m_player.transform.position.y, hit.point.z);
                        DMDrawCurve.Instance.AddPoint(pointPos);
                    }
                    else if (Input.GetMouseButtonUp(1) && hit.collider.tag.Equals("AnchorPoint"))
                    {
                        DMDrawCurve.Instance.DeletePoint(hit.collider.gameObject);
                    }
                }
            }
            if (Input.GetKeyUp(KeyCode.A))
                m_pathPoints = DMDrawCurve.Instance.HiddenLine(false);
            else if (Input.GetKeyUp(KeyCode.Escape))
            {
                DMDrawCurve.Instance.HiddenLine(true);
                m_pathPoints.Clear();
            }
            if (Input.GetKeyUp(KeyCode.B))
            {
                StartCoroutine(Move());
            }
        }
    }
}