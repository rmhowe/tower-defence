using UnityEngine;

namespace FIS {
    public class GameBoard : MonoBehaviour {
        [SerializeField] Transform ground;
        [SerializeField] GameTile tilePrefab;

        Vector2Int size;
        GameTile[] tiles;

        public void Initialise(Vector2Int boardSize) {
            this.size = boardSize;
            this.ground.localScale = new Vector3(boardSize.x, boardSize.y, 1f);

            this.tiles = new GameTile[this.size.x * this.size.y];
            Vector2 offset = new((this.size.x - 1) * 0.5f, (this.size.y - 1) * 0.5f);
            int i = 0;
            for (int y = 0; y < this.size.y; y++) {
                for (int x = 0; x < this.size.x; x++) {
                    GameTile tile = Object.Instantiate(this.tilePrefab, this.transform, false);
                    tile.transform.localPosition = new Vector3(x - offset.x, 0f, y - offset.y);
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
        }
    }
}
