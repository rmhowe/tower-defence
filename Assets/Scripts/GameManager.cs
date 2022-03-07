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
            this.board.ShowGrid = true;
        }

        void PlaceWall(InputAction.CallbackContext context) {
            Ray ray = this.mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            GameTile tile = this.board.GetTile(ray);
            if (tile != null) {
                this.board.ToggleWall(tile);
            }
        }
        
        void PlaceDestination(InputAction.CallbackContext context) {
            Ray ray = this.mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            GameTile tile = this.board.GetTile(ray);
            if (tile != null) {
                this.board.ToggleDestination(tile);
            }
        }
        
        void TogglePathVisibility(InputAction.CallbackContext context) {
            this.board.ShowPaths = !this.board.ShowPaths;
        }
        
        void ToggleGridVisibility(InputAction.CallbackContext context) {
            this.board.ShowGrid = !this.board.ShowGrid;
        }

        void OnEnable() {
            this.controls.Player.PlaceWall.performed += this.PlaceWall;
            this.controls.Player.PlaceDestination.performed += this.PlaceDestination;
            this.controls.Player.TogglePathVisibility.performed += this.TogglePathVisibility;
            this.controls.Player.ToggleGridVisibility.performed += this.ToggleGridVisibility;
            this.controls.Player.Enable();
        }
        
        void OnDisable() {
            this.controls.Player.PlaceWall.performed -= this.PlaceWall;
            this.controls.Player.PlaceDestination.performed -= this.PlaceDestination;
            this.controls.Player.TogglePathVisibility.performed -= this.TogglePathVisibility;
            this.controls.Player.ToggleGridVisibility.performed -= this.ToggleGridVisibility;
            this.controls.Player.Disable();
        }
    }
}
