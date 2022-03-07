using UnityEngine;
using UnityEngine.SceneManagement;

namespace FIS.ScriptableObjects {
    [CreateAssetMenu(menuName = "Tower Defence/Tiles/Content Factory")]
    public class GameTileContentFactory : ScriptableObject {
        [SerializeField] GameTileContent destinationPrefab;
        [SerializeField] GameTileContent emptyPrefab;
        [SerializeField] GameTileContent wallPrefab;
        
        Scene contentScene;
        
        public void Reclaim(GameTileContent content) {
            Debug.Assert(content.OriginFactory == this, "Wrong factory reclaimed!");
            Object.Destroy(content.gameObject);
        }

        GameTileContent Get(GameTileContent prefab) {
            GameTileContent instance = Object.Instantiate(prefab);
            instance.OriginFactory = this;
            this.MoveToFactoryScene(instance.gameObject);
            return instance;
        }

        void MoveToFactoryScene(GameObject go) {
            if (!this.contentScene.isLoaded) {
                if (Application.isEditor) {
                    this.contentScene = SceneManager.GetSceneByName(this.name);
                    if (!this.contentScene.isLoaded) {
                        this.contentScene = SceneManager.CreateScene(this.name);
                    }
                } else {
                    this.contentScene = SceneManager.CreateScene(this.name);
                }
            }
            SceneManager.MoveGameObjectToScene(go, this.contentScene);
        }

        public GameTileContent Get(GameTileContent.GameTileContentType type) {
            switch (type) {
                case GameTileContent.GameTileContentType.Destination:
                    return this.Get(this.destinationPrefab);
                case GameTileContent.GameTileContentType.Empty:
                    return this.Get(this.emptyPrefab);
                case GameTileContent.GameTileContentType.Wall:
                    return this.Get(this.wallPrefab);
                default:
                    Debug.Log($"Unsupported type: {type}");
                    return null;
            }
        }
    }
}
