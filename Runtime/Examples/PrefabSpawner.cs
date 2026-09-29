using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CptnFabulous.ObjectPool
{
    public class PrefabSpawner : MonoBehaviour
    {
        public Transform prefab;
        public Transform spawnPoint;
        public Bounds spawnPointDevianceZone;

        private void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                Vector3 min = spawnPointDevianceZone.min;
                Vector3 max = spawnPointDevianceZone.max;
                Vector3 spawnPosition = new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z));
                spawnPosition = spawnPoint.TransformPoint(spawnPosition);
                
                ObjectPool.RequestObject(prefab, null, spawnPosition, Quaternion.identity, true);
            }
        }
        private void OnDrawGizmos()
        {
            Gizmos.matrix = spawnPoint.localToWorldMatrix;
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(spawnPointDevianceZone.center, spawnPointDevianceZone.size);
        }
    }
}
