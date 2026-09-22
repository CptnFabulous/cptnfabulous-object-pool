using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CptnFabulous.ObjectPool
{
    public static class ObjectPool
    {
        // Keeps track of different pools for different prefabs
        static Dictionary<Component, IndividualObjectPool> dictionary = new Dictionary<Component, IndividualObjectPool>();

        public static IReadOnlyDictionary<Component, IndividualObjectPool> activePools => dictionary;

        /// <summary>
        /// Registers a pool for a prefab, and requests a copy (or creates one if all are currently being used). Object is parented to its pool parent by default.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prefab">The prefab you want to spawn a copy of.</param>
        /// <param name="activeByDefault">Does the object spawn as active or inactive?</param>
        /// <param name="maxPrefabs">Sets how many prefabs can spawn at a time before existing ones start being re-assigned. Zero or less means an unlimited number.</param>
        /// <returns></returns>
        public static T RequestObject<T>(T prefab) where T : Component
        {
            // Only this function needs to have a generic type, because each prefab is separated by being dictionary keys anyway.
            // I tried setting it up to use completely generic types, but this meant having to declare the type each time ObjectPool is referenced.

            // Don't do anything if there's no prefab specified
            if (prefab == null) return null;

            // Ensure an object pool is present for this prefab (create one if it hasn't already been created)
            TryCreateObjectPool(prefab);

            // Request the desired object from that pool.
            return dictionary[prefab].RequestObject() as T;
        }








        /// <summary>
        /// Creates an object pool for a prefab type. This is called automatically when a prefab is requested, but can be called beforehand to set unique properties for a prefab type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prefab"></param>
        /// <param name="activeByDefault"></param>
        /// <param name="maxPrefabs"></param>
        /// <param name="disableUponDismissal"></param>
        /// <returns></returns>
        public static bool TryCreateObjectPool<T>(T prefab, Transform parent = null, bool activeByDefault = true, int maxPrefabs = 0, bool disableUponDismissal = true) where T : Component
        {
            // Don't do anything if there's no prefab specified
            if (prefab == null) return false;

            // While we're managing the dictionary, delete any pools from it whose original prefabs have been destroyed
            ClearDictionaryElements(dictionary, (c) => c == null);

            // Check if a pool already exists for this prefab
            if (dictionary.ContainsKey(prefab)) return false;

            // Create the pool and add it to the dictionary
            IndividualObjectPool newPool = new IndividualObjectPool(prefab, parent, maxPrefabs, activeByDefault, disableUponDismissal);
            dictionary.Add(prefab, newPool);
            return true;
        }


        /// <summary>
        /// Returns an object to the pool it was spawned from. Destroys the object if it was not created from a pool.
        /// </summary>
        /// <param name="toDismiss">The component whose GameObject you want to get rid of.</param>
        public static void DismissObject(Component toDismiss)
        {
            // Don't proceed if there's nothing to dismiss
            if (toDismiss == null) return;

            // Iterate through the pools to see if it's part of one of them.
            foreach (IndividualObjectPool pool in dictionary.Values)
            {
                // Try returning the object to the pool
                bool dismissalSuccessful = pool.TryReturnObject(toDismiss);
                // If successful, end this function
                if (dismissalSuccessful) return;
            }

            // If none of the pools accepted it, just destroy it since we still need to get rid of it
            Object.Destroy(toDismiss.gameObject);
        }

        public static void ClearDictionaryElements<TKey, TValue>(Dictionary<TKey, TValue> dictionary, System.Func<TKey, bool> criteria)
        {
            for (int i = dictionary.Count - 1; i >= 0; i--)
            {
                TKey key = dictionary.Keys.ElementAt(i);
                if (criteria.Invoke(key)) dictionary.Remove(key);
            }
        }
    }
}