using System.Collections.Generic;
using UnityEngine;

namespace Manager
{
    public class PoolManager : MonoBehaviour
    {
        private readonly Dictionary<
            GameObject,
            Queue<GameObject>>
            prefabPools = new();

        private readonly Dictionary<
            GameObject,
            GameObject>
            instanceToPrefab = new();

        private Transform poolRoot;

        public void Init()
        {
            poolRoot = transform;
        }

        public GameObject Spawn(
            GameObject prefab)
        {
            if (!prefabPools.TryGetValue(
                    prefab,
                    out Queue<GameObject> pool))
            {
                pool = new Queue<GameObject>();

                prefabPools.Add(
                    prefab,
                    pool
                );
            }

            GameObject instance;

            if (pool.Count > 0)
            {
                instance = pool.Dequeue();
            }
            else
            {
                instance = CreateInstance(prefab);
            }

            instance.SetActive(true);

            return instance;
        }

        public GameObject Spawn(
            GameObject prefab,
            Vector3 position,
            Quaternion rotation)
        {
            GameObject instance =
                Spawn(prefab);

            instance.transform.SetPositionAndRotation(
                position,
                rotation
            );

            return instance;
        }

        public void Despawn(
            GameObject instance)
        {
            if (!instanceToPrefab.TryGetValue(
                    instance,
                    out GameObject prefab))
            {
                Debug.LogWarning(
                    $"{instance.name} is not a pooled instance."
                );

                return;
            }

            instance.SetActive(false);

            instance.transform.SetParent(
                poolRoot
            );

            prefabPools[prefab].Enqueue(
                instance
            );
        }

        private GameObject CreateInstance(
            GameObject prefab)
        {
            GameObject instance =
                Instantiate(
                    prefab,
                    poolRoot
                );

            instanceToPrefab.Add(
                instance,
                prefab
            );

            return instance;
        }  
    }
}
