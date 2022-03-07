using UnityEngine;

namespace FIS {
    public class GameTile : MonoBehaviour {
        [SerializeField] Transform arrow;
        
        static Quaternion northRotation = Quaternion.Euler(90f, 0f, 0f);
        static Quaternion southRotation = Quaternion.Euler(90f, 180f, 0f);
        static Quaternion eastRotation = Quaternion.Euler(90f, 90f, 0f);
        static Quaternion westRotation = Quaternion.Euler(90f, 270f, 0f);
        
        GameTile north, south, east, west;
        GameTile nextOnPath;
        int distance;
        
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
        public bool IsPathSet => this.distance != int.MaxValue;
        public bool IsAlternative;

        GameTile ExtendPath(GameTile neighbour) {
            Debug.Assert(this.IsPathSet, "No path!");
            if (neighbour == null || neighbour.IsPathSet) {
                return null;
            }
            neighbour.distance = this.distance + 1;
            neighbour.nextOnPath = this;
            return neighbour;
        }

        public GameTile ExtendPathNorth() => this.ExtendPath(this.north);
        public GameTile ExtendPathSouth() => this.ExtendPath(this.south);
        public GameTile ExtendPathEast() => this.ExtendPath(this.east);
        public GameTile ExtendPathWest() => this.ExtendPath(this.west);

        public void ClearPath() {
            this.distance = int.MaxValue;
            this.nextOnPath = null;
        }
        
        public void SetAsDestination() {
            this.distance = 0;
            this.nextOnPath = null;
        }

        public void ShowPath() {
            if (this.distance == 0) {
                this.arrow!.gameObject.SetActive(false);
                return;
            }
            
            this.arrow!.gameObject.SetActive(true);
            this.arrow!.localRotation = this.nextOnPath == this.north ? GameTile.northRotation :
                this.nextOnPath == this.south ? GameTile.southRotation :
                this.nextOnPath == this.east ? GameTile.eastRotation :
                this.nextOnPath == this.west ? GameTile.westRotation :
                Quaternion.Euler(45f, 0f, 0f);
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
