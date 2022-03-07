using UnityEngine;

namespace FIS {
    public class GameManager : MonoBehaviour {
        [SerializeField] Vector2Int boardSize = new(11, 11);
        [SerializeField] GameBoard board;

        void Awake() {
            this.board.Initialise(this.boardSize);
        }
    }
}
