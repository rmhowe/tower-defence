using System.Collections.Generic;
using FIS.ScriptableObjects;
using UnityEngine;

namespace FIS {
    public class GameBoard : MonoBehaviour {
        [SerializeField] Transform ground;
        [SerializeField] GameTile tilePrefab;

        Vector2Int size;
        GameTile[] tiles;
        Queue<GameTile> searchFrontier = new();
        GameTileContentFactory contentFactory;

        bool SetPaths() {
            foreach (GameTile tile in this.tiles) {
                if (tile.Content.Type == GameTileContent.GameTileContentType.Destination) {
                    tile.SetAsDestination();
                    this.searchFrontier.Enqueue(tile);
                } else {
                    tile.ClearPath();
                }
            }

            if (this.searchFrontier.Count == 0) {
                return false;
            }

            while (this.searchFrontier.Count > 0) {
                GameTile tile = this.searchFrontier.Dequeue();
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

            foreach (GameTile tile in this.tiles) {
                tile.ShowPath();
            }
            return true;
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
                    tile.Content = this.contentFactory.Get(GameTileContent.GameTileContentType.Empty);
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
            if (tile.Content.Type == GameTileContent.GameTileContentType.Destination) {
                tile.Content = this.contentFactory.Get(GameTileContent.GameTileContentType.Empty);
                if (!this.SetPaths()) {
                    tile.Content = this.contentFactory.Get(GameTileContent.GameTileContentType.Destination);
                    this.SetPaths();
                }
            } else {
                tile.Content = this.contentFactory.Get(GameTileContent.GameTileContentType.Destination);
                this.SetPaths();
            }
        }
    }
}
