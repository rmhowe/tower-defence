using UnityEngine;

namespace FIS {
    public class GameTile : MonoBehaviour {
        [SerializeField] Transform arrow;
        
        GameTile north, south, east, west;
        GameTile nextOnPath;
        int distance;

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
