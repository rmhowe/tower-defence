using System.Collections.Generic;
using UnityEngine;

namespace FIS.Runtime.Types {
    public enum Direction {
        North,
        East,
        South,
        West
    }

    public enum DirectionChange {
        None,
        TurnRight,
        TurnLeft,
        TurnAround
    }

    public static class DirectionExtensions {
        static readonly Dictionary<Direction, Quaternion> rotations = new() {
            {Direction.North, Quaternion.identity},
            {Direction.East, Quaternion.Euler(0f, 90f, 0f)},
            {Direction.South, Quaternion.Euler(0f, 180f, 0f)},
            {Direction.West, Quaternion.Euler(0f, 270f, 0f)}
        };
        
        static readonly Dictionary<Direction, Vector3> halfVectors = new() {
            {Direction.North, Vector3.forward * 0.5f},
            {Direction.East, Vector3.right * 0.5f},
            {Direction.South, Vector3.back * 0.5f},
            {Direction.West, Vector3.left * 0.5f}
        };

        public static Quaternion GetRotation(this Direction direction) {
            return DirectionExtensions.rotations[direction];
        }

        public static float GetAngle(this Direction direction) {
            return (float) direction * 90f;
        }

        public static Vector3 GetHalfVector(this Direction direction) {
            return DirectionExtensions.halfVectors[direction];
        }
        
        public static DirectionChange GetDirectionChange(this Direction from, Direction to) {
            if (from == to) {
                return DirectionChange.None;
            } else if (from + 1 == to || from - 3 == to) {
                return DirectionChange.TurnRight;
            } else if (from - 1 == to || from + 3 == to) {
                return DirectionChange.TurnLeft;
            }
            return DirectionChange.TurnAround;
        }
    }
}
