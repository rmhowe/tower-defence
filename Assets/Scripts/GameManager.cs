using FIS.ScriptableObjects;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FIS {
    public class GameManager : MonoBehaviour {
        [SerializeField] Vector2Int boardSize = new(11, 11);
        [SerializeField] GameBoard board;
        [SerializeField] GameTileContentFactory tileContentFactory;
        [SerializeField] Camera mainCamera;

        PlayerControls controls;

        void Awake() {
            this.controls = new PlayerControls();
            this.board.Initialise(this.boardSize, this.tileContentFactory);
        }

        void HandleClick(InputAction.CallbackContext context) {
            Debug.Log($"{context.started} {context.performed} {context.canceled}");
            Ray ray = this.mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            GameTile tile = this.board.GetTile(ray);
            if (tile != null) {
                this.board.ToggleDestination(tile);
            }
        }

        void OnEnable() {
            this.controls.UI.Click.performed += this.HandleClick; 
            this.controls.UI.Click.Enable();
        }
        
        void OnDisable() {
            this.controls.UI.Click.performed -= this.HandleClick;
            this.controls.UI.Click.Disable();
        }
    }
}
