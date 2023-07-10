using System;

namespace MapGen.InputSystem
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }
        
        [SerializeField] private Camera sceneCamera;
        [SerializeField] private LayerMask placementLayerMask;
        [SerializeField] private int _maxDistance;
        
        private Vector3 _lastPosition;

        private void Awake()
        {
            Instance = this;
        }

        public bool GetSelectedMapPosition(out RaycastHit result)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = sceneCamera.nearClipPlane;
            Ray ray = sceneCamera.ScreenPointToRay(mousePos);
            
            if (Physics.Raycast(ray, out result, _maxDistance, placementLayerMask))
            {
                return true;
            }
            return false;
        }

        public bool IsGridSelected()
        {
            return Input.GetMouseButton(0);
        }
    }
}