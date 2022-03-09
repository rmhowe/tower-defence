using FIS.Runtime.Game;
using UnityEngine;

namespace FIS.Runtime.ScriptableObjects {
    [CreateAssetMenu(fileName = "EnemyFactory", menuName = "Tower Defence/Factories/Enemy Factory")]
    public class EnemyFactory : GameObjectFactory {
        [SerializeField] Enemy prefab;

        public Enemy Get() {
            Enemy instance = this.CreateGameObjectInstance(this.prefab);
            instance.OriginFactory = this;
            return instance;
        }

        public void Reclaim(Enemy enemy) {
            Debug.Assert(enemy.OriginFactory == this, "Wrong factory reclaimed!");
            Object.Destroy(enemy.gameObject);
        }
    }
}
