using System.Collections.Generic;

namespace FIS.Runtime.Game {
    [System.Serializable]
    public class EnemyCollection {
        List<Enemy> enemies = new();

        public void Add(Enemy enemy) {
            this.enemies.Add(enemy);
        }

        public void GameUpdate() {
            this.enemies.RemoveAll(enemy => {
                enemy.GameUpdate();
                return enemy.Destroyed;
            });
        }
    }
}
