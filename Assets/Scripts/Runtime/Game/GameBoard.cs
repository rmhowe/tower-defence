using System.Collections.Generic;
using FIS.Runtime.ScriptableObjects;
using FIS.Runtime.Types;
using UnityEngine;

namespace FIS.Runtime.Game {
    public class GameBoard : MonoBehaviour {
        [SerializeField] Transform ground;
        [SerializeField] GameTile tilePrefab;
        [SerializeField] Texture2D gridTexture;

        Vector2Int size;
        GameTile[] tiles;
        Queue<GameTile> searchFrontier = new();
        List<GameTile> spawnPoints = new();
        GameTileContentFactory contentFactory;
        bool pathsValid;

        public int SpawnPointCount => this.spawnPoints.Count;
        public GameTile GetSpawnPoint(int index) => this.spawnPoints[index];

        bool showPaths = false;
        public bool ShowPaths {
            get => this.showPaths;
            set {
                this.showPaths = value;
                if (this.showPaths) {
                    foreach (var tile in this.tiles) {
                        tile.ShowPath();
                    }
                } else {
                    foreach (var tile in this.tiles) {
                        tile.HidePath();
                    }
                }
            }
        }

        bool showGrid = false;
        public bool ShowGrid {
            get => this.showGrid;
            set {
                this.showGrid = value;
                var mat = this.ground.GetComponent<MeshRenderer>().material;
                if (this.showGrid) {
                    mat.mainTexture = this.gridTexture;
                    mat.mainTextureScale = this.size;
                } else {
                    mat.mainTexture = null;
                }
            }
        }

        void SetPaths() {
            this.pathsValid = false;
            foreach (var tile in this.tiles) {
                if (tile.Content.Type == GameTileContentType.Destination) {
                    tile.SetAsDestination();
                    this.searchFrontier.Enqueue(tile);
                } else {
                    tile.ClearPath();
                }
            }

            if (this.searchFrontier.Count == 0) {
                return;
            }

            while (this.searchFrontier.Count > 0) {
                var tile = this.searchFrontier.Dequeue();
                if (tile != null) {
                    if (tile.IsAlternative) {
                        this.searchFrontier.Enqueue(tile.ExtendPathNorth());
                        this.searchFrontier.Enqueue(tile.ExtendPathSouth());
                        this.searchFrontier.Enqueue(tile.ExtendPathEast());
                        this.searchFrontier.Enqueue(tile.ExtendPathWest());
                    } else {
                        this.searchFrontier.Enqueue(tile.ExtendPathWest());
                        this.searchFrontier.Enqueue(tile.ExtendPathEast());
                        this.searchFrontier.Enqueue(tile.ExtendPathSouth());
                        this.searchFrontier.Enqueue(tile.ExtendPathNorth());
                    }
                }
            }

            foreach (var tile in this.tiles) {
                if (!tile.IsPathSet) {
                    return;
                }
            }

            if (this.showPaths) {
                foreach (var tile in this.tiles) {
                    tile.ShowPath();
                }
            }
            
            this.pathsValid = true;
        }

        public void Initialise(Vector2Int boardSize, GameTileContentFactory tileContentFactory) {
            this.size = boardSize;
            this.contentFactory = tileContentFactory;
            this.ground.localScale = new Vector3(boardSize.x, boardSize.y, 1f);

            this.tiles = new GameTile[this.size.x * this.size.y];
            Vector2 offset = new((this.size.x - 1) * 0.5f, (this.size.y - 1) * 0.5f);
            int i = 0;
            for (int y = 0; y < this.size.y; y++) {
                for (int x = 0; x < this.size.x; x++) {
                    GameTile tile = Object.Instantiate(this.tilePrefab!, this.transform, false);
                    tile.transform.localPosition = new Vector3(x - offset.x, 0f, y - offset.y);
                    tile.IsAlternative = x % 2 == 0;
                    if (y % 2 == 0) {
                        tile.IsAlternative = !tile.IsAlternative;
                    }
                    tile.Content = this.contentFactory.Get(GameTileContentType.Empty);
                    this.tiles[i] = tile;

                    if (x > 0) {
                        GameTile.MakeEastWestNeighbours(this.tiles[i], this.tiles[i-1]);
                    }
                    if (y > 0) {
                        GameTile.MakeNorthSouthNeighbours(this.tiles[i], this.tiles[i-this.size.x]);
                    }
                    
                    i++;
                }
            }
            
            this.ToggleDestination(this.tiles[this.tiles.Length / 2]);
            this.ToggleSpawnPoint(this.tiles[0]);
        }

        public GameTile GetTile(Ray ray) {
            if (Physics.Raycast(ray, out RaycastHit hit)) {
                int x = (int)(hit.point.x + this.size.x * 0.5f);
                int y = (int)(hit.point.z + this.size.y * 0.5f);
                if (x >= 0 && x < this.size.x && y >= 0 && y < this.size.y) {
                    return this.tiles?[x + y * this.size.x];
                }
            }
            return null;
        }

        public void ToggleDestination(GameTile tile) {
            switch (tile.Content.Type) {
                case GameTileContentType.Destination:
                    tile.Content = this.contentFactory.Get(GameTileContentType.Empty);
                    this.SetPaths();
                    if (!this.pathsValid) {
                        tile.Content = this.contentFactory.Get(GameTileContentType.Destination);
                        this.SetPaths();
                    }
                    return;
                case GameTileContentType.Empty:
                    tile.Content = this.contentFactory.Get(GameTileContentType.Destination);
                    this.SetPaths();
                    return;
            }
        }

        public void ToggleWall(GameTile tile) {
            switch (tile.Content.Type) {
                case GameTileContentType.Wall:
                    tile.Content = this.contentFactory.Get(GameTileContentType.Empty);
                    this.SetPaths();
                    return;
                case GameTileContentType.Empty:
                    tile.Content = this.contentFactory.Get(GameTileContentType.Wall);
                    this.SetPaths();
                    if (!this.pathsValid) {
                        tile.Content = this.contentFactory.Get(GameTileContentType.Empty);
                        this.SetPaths();
                    }
                    return;
            }
        }
        
        public void ToggleSpawnPoint(GameTile tile) {
            switch (tile.Content.Type) {
                case GameTileContentType.SpawnPoint:
                    if (this.spawnPoints.Count > 1) {
                        this.spawnPoints.Remove(tile);
                        tile.Content = this.contentFactory.Get(GameTileContentType.Empty);
                    }
                    return;
                case GameTileContentType.Empty:
                    tile.Content = this.contentFactory.Get(GameTileContentType.SpawnPoint);
                    this.spawnPoints.Add(tile);
                    return;
            }
        }
    }
}
