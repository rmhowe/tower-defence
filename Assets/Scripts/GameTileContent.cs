#nullable enable
using FIS.ScriptableObjects;
using UnityEngine;

namespace FIS {
    public class GameTileContent : MonoBehaviour {
        public enum GameTileContentType {
            Empty,
            Destination
        }
        public GameTileContentType Type;

        GameTileContentFactory? originFactory;
        public GameTileContentFactory? OriginFactory {
            get => this.originFactory;
            set {
                Debug.Assert(this.originFactory == null, "Redefined origin factory!");
                this.originFactory = value;
            }
        }

        public void Recycle() {
            this.originFactory?.Reclaim(this);
        }
    }
}
