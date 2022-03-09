using UnityEngine;
using UnityEngine.SceneManagement;

namespace FIS.Runtime.ScriptableObjects {
    public abstract class GameObjectFactory : ScriptableObject {
        Scene scene;
        
        protected T CreateGameObjectInstance<T>(T prefab) where T : MonoBehaviour {
            if (!this.scene.isLoaded) {
                if (Application.isEditor) {
                    this.scene = SceneManager.GetSceneByName(this.name);
                    if (!this.scene.isLoaded) {
                        this.scene = SceneManager.CreateScene(this.name);
                    }
                }
                else {
                    this.scene = SceneManager.CreateScene(this.name);
                }
            }
            T instance = Object.Instantiate(prefab);
            SceneManager.MoveGameObjectToScene(instance.gameObject, this.scene);
            return instance;
        }
    }
}
