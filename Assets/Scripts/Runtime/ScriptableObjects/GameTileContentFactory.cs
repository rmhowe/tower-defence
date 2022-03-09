using FIS.Runtime.Game;
using FIS.Runtime.Types;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FIS.Runtime.ScriptableObjects {
    [CreateAssetMenu(fileName = "GameTileContentFactory", menuName = "Tower Defence/Factories/Game Tile Content Factory")]
    public class GameTileContentFactory : GameObjectFactory {
        [SerializeField] GameTileContent destinationPrefab;
        [SerializeField] GameTileContent emptyPrefab;
        [SerializeField] GameTileContent wallPrefab;
        [SerializeField] GameTileContent spawnPointPrefab;
        
        public void Reclaim(GameTileContent content) {
            Debug.Assert(content.OriginFactory == this, "Wrong factory reclaimed!");
            Object.Destroy(content.gameObject);
        }

        GameTileContent Get(GameTileContent prefab) {
            GameTileContent instance = this.CreateGameObjectInstance(prefab);
            instance.OriginFactory = this;
            return instance;
        }

        public GameTileContent Get(GameTileContentType type) {
            switch (type) {
                case GameTileContentType.Destination:
                    return this.Get(this.destinationPrefab);
                case GameTileContentType.Empty:
                    return this.Get(this.emptyPrefab);
                case GameTileContentType.Wall:
                    return this.Get(this.wallPrefab);
                case GameTileContentType.SpawnPoint:
                    return this.Get(this.spawnPointPrefab);
                default:
                    Debug.Log($"Unsupported type: {type}");
                    return null;
            }
        }
    }
}
