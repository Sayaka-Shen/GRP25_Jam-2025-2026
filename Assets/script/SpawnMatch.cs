using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Alumette;
using Random = UnityEngine.Random;

public class SpawnMatch : MonoBehaviour
{
    [Header("Parameter")]
    [SerializeField] private GameObject m_Prefab;
    [SerializeField] private BoxCollider m_spawnableArea;
    [SerializeField] int m_row;
    [SerializeField] int m_col;
    [SerializeField] private int percentOrignalAlumette;
    [SerializeField] private float m_timeBeforeSpawning;
    private GameObject m_prefabInstantiate;
    private float m_timer = 0;
    private HashSet<Vector2Int> m_usedGridIndex = new HashSet<Vector2Int>();

    private void Start()
    {
        ResetUsedGridIndex();
    }

    private void Update()
    {
        if(!GameManager.Instance.HasGameEnd && !ZoneManager.Instance.MaxObstacleSpawnedReach)
        {
            m_timer += Time.deltaTime;

            float randomEnumID = Random.Range(0, 100);

            if (m_timer > m_timeBeforeSpawning)
            {
                Vector3 matchPos = PickRandomPoint(m_spawnableArea.bounds);
                if (matchPos != Vector3.zero)
                {
                    m_prefabInstantiate = Instantiate(m_Prefab, matchPos, m_Prefab.transform.rotation);
                    ZoneManager.Instance.NbObstacleSpawn++;

                    if (m_prefabInstantiate != null && m_prefabInstantiate.TryGetComponent<Alumette>(out Alumette alumette))
                    {
                        if (randomEnumID < percentOrignalAlumette)
                        {
                            alumette.AlumetteType = AlumetteState.BaseState;
                        }
                        else
                        {
                            alumette.AlumetteType = (AlumetteState)Random.Range(0, 7);
                        }
                    }

                    m_timer = 0;
                }
            }
        }
    }

    private Vector3 PickRandomPoint(Bounds bounds)
    {
        float boundsXDist = Mathf.Abs(bounds.min.x - bounds.max.x);
        float boundsZDist = Mathf.Abs(bounds.min.z - bounds.max.z);

        // m_col * m_row =  max essaie de la boucle au cas ou 
        for (int i = 0; i < m_col * m_row; i++)
        {
            int colIndex = Random.Range(0, m_col + 1);
            int rowIndex = Random.Range(0, m_row + 1);
            Vector2Int gridIndex = new Vector2Int(colIndex, rowIndex);

            if (!m_usedGridIndex.Contains(gridIndex))
            {
                float x = bounds.min.x + (boundsXDist / m_col) * colIndex;
                float z = bounds.min.z + (boundsZDist / m_row) * rowIndex;
                m_usedGridIndex.Add(gridIndex);

                return new Vector3(x, 10, z);
            }
        }

        return Vector3.zero;
    }

    private void ResetUsedGridIndex()
    {
        m_usedGridIndex.Clear();
    }

    // Visual for the segment of the grid
    private void OnDrawGizmosSelected()
    {
        Bounds b = m_spawnableArea.bounds;
        var bXDis = Mathf.Abs(b.min.x - b.max.x);
        var bZDis = Mathf.Abs(b.min.z - b.max.z);

        for (int i = 0; i < m_row + 1; i++)
        {
            for (int j = 0; j < m_col + 1; j++)
            {
                var x = b.min.x + (bXDis / m_col) * j;
                var z = b.min.z + (bZDis / m_row) * i;

                Gizmos.DrawWireSphere(new Vector3(x, 0, z), 0.5f);
            }
        }
    }

}
