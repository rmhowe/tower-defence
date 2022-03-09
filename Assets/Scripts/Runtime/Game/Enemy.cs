using FIS.Runtime.ScriptableObjects;
using FIS.Runtime.Types;
using UnityEngine;

namespace FIS.Runtime.Game {
    public class Enemy : MonoBehaviour {
        [SerializeField] Transform model;
        
        EnemyFactory originFactory;
        GameTile tileFrom, tileTo;
        Vector3 positionFrom, positionTo;
        float progress;
        Direction direction;
        DirectionChange directionChange;
        float directionAngleFrom, directionAngleTo;

        [HideInInspector] public bool Destroyed = false;

        public EnemyFactory OriginFactory {
            get => this.originFactory;
            set {
                Debug.Assert(this.originFactory == null, "Redefined origin factory!");
                this.originFactory = value;
            }
        }

        void PrepareIntro() {
            this.positionFrom = this.tileFrom.transform.localPosition;
            this.positionTo = this.tileFrom.ExitPoint;
            this.direction = this.tileFrom.PathDirection;
            this.directionChange = DirectionChange.None;
            this.directionAngleFrom = this.direction.GetAngle();
            this.directionAngleTo = this.direction.GetAngle();
            this.transform.localRotation = this.direction.GetRotation();
        }

        public void SpawnOn(GameTile tile) {
            Debug.Assert(tile.NextOnPath != null, "Nowhere to go!", this);
            this.tileFrom = tile;
            this.tileTo = tile.NextOnPath;
            this.progress = 0f;
            this.PrepareIntro();
        }

        void PrepareForward() {
            this.transform.localRotation = this.direction.GetRotation();
            this.directionAngleTo = this.direction.GetAngle();
            this.model.localPosition = Vector3.zero;
        }

        void PrepareTurnRight() {
            this.directionAngleTo = this.directionAngleFrom + 90f;
            this.model.localPosition = new Vector3(-0.5f, 0f, 0f);
            this.transform.localPosition = this.positionFrom + this.direction.GetHalfVector();
        }

        void PrepareTurnLeft() {
            this.directionAngleTo = this.directionAngleFrom - 90f;
            this.model.localPosition = new Vector3(0.5f, 0f, 0f);
            this.transform.localPosition = this.positionFrom + this.direction.GetHalfVector();
        }

        void PrepareTurnAround() {
            this.directionAngleTo = this.directionAngleFrom + 180f;
            this.model.localPosition = Vector3.zero;
            this.transform.localPosition = this.positionFrom;
        }
        
        void PrepareNextState() {
            this.positionFrom = this.positionTo;
            this.positionTo = this.tileFrom.ExitPoint;
            this.directionChange = this.direction.GetDirectionChange(this.tileFrom.PathDirection);
            this.direction = this.tileFrom.PathDirection;
            this.directionAngleFrom = this.directionAngleTo;
            switch (this.directionChange) {
                case DirectionChange.None:
                    this.PrepareForward();
                    break;
                case DirectionChange.TurnRight:
                    this.PrepareTurnRight();
                    break;
                case DirectionChange.TurnLeft:
                    this.PrepareTurnLeft();
                    break;
                default:
                    this.PrepareTurnAround();
                    break;
            }
        }

        public void GameUpdate() {
            this.progress += Time.deltaTime;
            if (this.progress >= 1f) {
                this.tileFrom = this.tileTo;
                this.tileTo = this.tileTo.NextOnPath;
                if (this.tileTo == null) {
                    this.originFactory.Reclaim(this);
                    this.Destroyed = true;
                    return;
                }

                this.progress -= 1f;
                this.PrepareNextState();
            }

            if (this.directionChange == DirectionChange.None) {
                this.transform.localPosition = Vector3.LerpUnclamped(this.positionFrom, this.positionTo, this.progress);
            } else {
                float angle = Mathf.LerpUnclamped(this.directionAngleFrom, this.directionAngleTo, this.progress);
                this.transform.localRotation = Quaternion.Euler(0f, angle, 0f);
            }
        }
    }
}
