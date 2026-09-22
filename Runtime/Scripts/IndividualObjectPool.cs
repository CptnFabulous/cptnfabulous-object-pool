using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CptnFabulous.ObjectPool
{
    public class IndividualObjectPool
    {
        public Component originalPrefab { get; private set; }
        public int maxPrefabs { get; private set; }
        public bool activeByDefault { get; private set; }
        public bool disableUponDismissal { get; private set; }
        public Transform poolParent { get; private set; }

        List<Component> active = new List<Component>();
        Queue<Component> standby = new Queue<Component>();

        public bool unlimited => maxPrefabs <= 0;

        public IndividualObjectPool(Component prefab, Transform parent, int maxPrefabs, bool activeByDefault, bool disableUponDismissal)
        {
            this.originalPrefab = prefab;

            // Set up pool parent
            if (parent != null)
            {
                this.poolParent = parent;
            }
            else
            {
                this.poolParent = new GameObject($"Object Pool Parent ({originalPrefab})").transform;
                Object.DontDestroyOnLoad(poolParent);
            }

            // Set additional values (maybe I should put these in the constructor)
            this.maxPrefabs = maxPrefabs;
            this.activeByDefault = activeByDefault;
            this.disableUponDismissal = disableUponDismissal;
        }

        public Component RequestObject()
        {
            // Clear entries for accidentally-destroyed objects
            active.RemoveAll((x) => x == null);

            // Try dequeueing an existing value (if it's there).
            Component final = null;
            while (standby.TryDequeue(out final))
            {
                // If a value is found, check if it's null.
                if (final != null) break;
                // If null, try again until a not-null value is found, or there are no more entries
            }
            // This loop helps purge the standby queue of accidental null values.

            // If none are on standby, see if we can spawn a new one, or if the max count has been reached.
            if (final == null)
            {
                if (unlimited || active.Count < maxPrefabs)
                {
                    final = Object.Instantiate(originalPrefab, poolParent);
                }
                else
                {
                    // If limit has been reached, 'deactivate' the oldest already-active one and re-use it
                    final = active[0];
                    active.RemoveAt(0);
                }
            }

            // Add the value to the list so we know what order it was spawned in
            active.Add(final);
            final.gameObject.SetActive(activeByDefault);
            return final;
        }
        public bool TryReturnObject(Component toDismiss)
        {
            // Ignore if this object isn't part of the pool
            if (active.Contains(toDismiss) == false) return false;

            // Remove from active list, add to standby queue
            active.Remove(toDismiss);
            standby.Enqueue(toDismiss);

            // Disable object and shuffle it back in with the pool parent
            if (disableUponDismissal) toDismiss.gameObject.SetActive(false);
            toDismiss.transform.SetParent(poolParent);

            return true;
        }
    }
}
