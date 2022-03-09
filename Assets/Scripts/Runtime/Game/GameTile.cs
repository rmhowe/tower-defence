using FIS.Runtime.Types;
using UnityEngine;

namespace FIS.Runtime.Game {
    public class GameTile : MonoBehaviour {
        [SerializeField] Transform arrow;
        
        static Quaternion northRotation = Quaternion.Euler(90f, 0f, 0f);
        static Quaternion southRotation = Quaternion.Euler(90f, 180f, 0f);
        static Quaternion eastRotation = Quaternion.Euler(90f, 90f, 0f);
        static Quaternion westRotation = Quaternion.Euler(90f, 270f, 0f);
        
        GameTile north, south, east, west;
        int distance;
        
        [HideInInspector] public bool IsAlternative;
        public bool IsPathSet => this.distance != int.MaxValue;
        public GameTile NextOnPath { get; private set; }
        public Vector3 ExitPoint { get; private set; }
        public Direction PathDirection { get; private set; }

        GameTileContent content;
        public GameTileContent Content {
            get => this.content;
            set {
                Debug.Assert(value != null, "Cannot assign null to content!");
                if (this.content != null) {
                    this.content.Recycle();
                }
                this.content = value;
                this.content.transform.localPosition = this.transform.localPosition;
            }
        }

        GameTile ExtendPath(GameTile neighbour, Direction direction) {
            Debug.Assert(this.IsPathSet, "No path!");
            if (neighbour == null || neighbour.IsPathSet) {
                return null;
            }
            neighbour.distance = this.distance + 1;
            neighbour.NextOnPath = this;
            neighbour.ExitPoint = neighbour.transform.localPosition - direction.GetHalfVector();
            neighbour.PathDirection = direction;
            return neighbour.Content.Type == GameTileContentType.Wall ? null : neighbour;
        }

        public GameTile ExtendPathNorth() => this.ExtendPath(this.north, Direction.North);
        public GameTile ExtendPathSouth() => this.ExtendPath(this.south, Direction.South);
        public GameTile ExtendPathEast() => this.ExtendPath(this.east, Direction.East);
        public GameTile ExtendPathWest() => this.ExtendPath(this.west, Direction.West);

        public void ClearPath() {
            this.distance = int.MaxValue;
            this.NextOnPath = null;
        }
        
        public void SetAsDestination() {
            this.distance = 0;
            this.NextOnPath = null;
            this.ExitPoint = this.transform.localPosition;
        }

        public void ShowPath() {
            if (this.distance == 0) {
                this.arrow!.gameObject.SetActive(false);
                return;
            }
            
            this.arrow.gameObject.SetActive(true);
            this.arrow.localRotation = this.NextOnPath == this.north ? GameTile.northRotation :
                this.NextOnPath == this.south ? GameTile.southRotation :
                this.NextOnPath == this.east ? GameTile.eastRotation :
                this.NextOnPath == this.west ? GameTile.westRotation :
                Quaternion.Euler(45f, 0f, 0f);
        }

        public void HidePath() {
            this.arrow.gameObject.SetActive(false);
        }

        public static void MakeEastWestNeighbours(GameTile east, GameTile west) {
            Debug.Assert(west.east == null && east.west == null, "Neighbours have been redefined");
            west.east = east;
            east.west = west;
        }
        
        public static void MakeNorthSouthNeighbours(GameTile north, GameTile south) {
            Debug.Assert(south.north == null && north.south == null, "Neighbours have been redefined");
            south.north = north;
            north.south = south;
        }
    }
}
