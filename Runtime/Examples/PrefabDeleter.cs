using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CptnFabulous.ObjectPool
{
    public class PrefabDeleter : MonoBehaviour
    {
        public Camera camera;
        public LayerMask raycastMask = ~0;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray clickRay = camera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(clickRay, out RaycastHit rh, camera.farClipPlane, raycastMask))
                {
                    ObjectPool.DismissObject(rh.collider.transform);
                }
            }
        }
    }
}
