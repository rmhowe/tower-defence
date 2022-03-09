using FIS.Runtime.ScriptableObjects;
using FIS.Runtime.Types;
using UnityEngine;

namespace FIS.Runtime.Game {
    public class GameTileContent : MonoBehaviour {
        public GameTileContentType Type;

        GameTileContentFactory originFactory;
        public GameTileContentFactory OriginFactory {
            get => this.originFactory;
            set {
                Debug.Assert(this.originFactory == null, "Redefined origin factory!");
                this.originFactory = value;
            }
        }

        public void Recycle() {
            this.originFactory.Reclaim(this);
        }
    }
}
