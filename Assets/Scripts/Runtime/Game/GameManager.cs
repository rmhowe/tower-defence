using FIS.Runtime.ScriptableObjects;
using FIS.Runtime.Types;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FIS.Runtime.Game {
    public class GameManager : MonoBehaviour {
        [SerializeField] Vector2Int boardSize = new(11, 11);
        [SerializeField] GameBoard board;
        [SerializeField] GameTileContentFactory tileContentFactory;
        [SerializeField] EnemyFactory enemyFactory;
        [SerializeField, Range(0.1f, 10f)] float enemySpawnSpeed = 1f;
        [SerializeField] Camera mainCamera;

        PlayerControls controls;
        float spawnProgress = 1f;
        EnemyCollection enemies = new();

        void Awake() {
            this.controls = new PlayerControls();
            this.board.Initialise(this.boardSize, this.tileContentFactory);
            this.board.ShowGrid = true;
        }

        void Update() {
            this.spawnProgress += this.enemySpawnSpeed * Time.deltaTime;
            while (this.spawnProgress >= 1f) {
                this.spawnProgress -= 1f;
                this.SpawnEnemy();
            }
            this.enemies.GameUpdate();
        }

        void SpawnEnemy() {
            GameTile spawnPoint = this.board.GetSpawnPoint(Random.Range(0, this.board.SpawnPointCount));
            Enemy enemy = this.enemyFactory.Get();
            enemy.SpawnOn(spawnPoint);
            this.enemies.Add(enemy);
        }

        void PlaceTile(GameTileContentType type) {
            Ray ray = this.mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            GameTile tile = this.board.GetTile(ray);
            if (tile != null) {
                switch (type) {
                    case GameTileContentType.Destination:
                        this.board.ToggleDestination(tile);
                        return;
                    case GameTileContentType.Wall:
                        this.board.ToggleWall(tile);
                        return;
                    case GameTileContentType.SpawnPoint:
                        this.board.ToggleSpawnPoint(tile);
                        return;
                }
            }
        }

        void PlaceWall(InputAction.CallbackContext context) {
            this.PlaceTile(GameTileContentType.Wall);
        }
        
        void PlaceDestination(InputAction.CallbackContext context) {
            this.PlaceTile(GameTileContentType.Destination);
        }
        
        void PlaceSpawnPoint(InputAction.CallbackContext context) {
            this.PlaceTile(GameTileContentType.SpawnPoint);
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
            this.controls.Player.PlaceSpawnPoint.performed += this.PlaceSpawnPoint;
            this.controls.Player.TogglePathVisibility.performed += this.TogglePathVisibility;
            this.controls.Player.ToggleGridVisibility.performed += this.ToggleGridVisibility;
            this.controls.Player.Enable();
        }
        
        void OnDisable() {
            this.controls.Player.PlaceWall.performed -= this.PlaceWall;
            this.controls.Player.PlaceDestination.performed -= this.PlaceDestination;
            this.controls.Player.PlaceSpawnPoint.performed -= this.PlaceSpawnPoint;
            this.controls.Player.TogglePathVisibility.performed -= this.TogglePathVisibility;
            this.controls.Player.ToggleGridVisibility.performed -= this.ToggleGridVisibility;
            this.controls.Player.Disable();
        }
    }
}
