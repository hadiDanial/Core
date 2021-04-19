using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core
{
    public class OnClickActivateEvent : MonoBehaviour
    {
        [SerializeField] GameEvent gameEvent;

        [SerializeField]
        private Camera gameCamera;
        private static InputAction click;

        void Awake()
        {
            gameCamera = Camera.main;
            if (click == null)
            {
                click = new InputAction(binding: "<Mouse>/leftButton");
            }
            click.performed += ctx =>
            {
                RaycastHit hit;
                Vector3 coor = Mouse.current.position.ReadValue();
                if (Physics.Raycast(gameCamera.ScreenPointToRay(coor), out hit))
                {
                    if(hit.transform == transform)
                    Activate();
                }
            };
            click.Enable();
        }


        [ContextMenu("Activate", false, 0)]
        private void Activate()
        {
            Debug.Log("CLICK!");
            gameEvent?.Invoke();
        }
    }
}
