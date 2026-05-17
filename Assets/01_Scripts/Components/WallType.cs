using System;
using UnityEngine;
using Utilities;

namespace CoreSystem
{
    [Serializable]
    public class WallRules
    {
        public BoundaryType boundaryType;
        public bool HasWallTop;
        public bool HasWallTopLeft;
        public bool HasWallTopRight;
        public bool HasWallBottom;
        public bool HasWallBottomLeft;
        public bool HasWallBottomRight;
        public bool HasWallLeft;
        public bool HasWallRight;
    }

    [CreateAssetMenu(fileName = "Wall Type", menuName = "Levels/WallType")]
    public class WallType : ScriptableObject
    {
        [field: Header("Wall Data")]
        [field: SerializeField] public WallNodeType Description { get; private set; }
        [field: SerializeField] public Mesh Mesh { get; private set; }
        [field: SerializeField, Range(0, 360)] public float YRotation { get; private set; }
        [field: SerializeField] public WallRules[] WallRules { get; private set; }

        public bool Matches(BoundaryType boundaryType, bool hasTop, bool hasTopLeft, bool hasTopRight, bool hasBottom, bool hasBottomLeft, bool hasBottomRight, bool hasLeft, bool hasRight)
        {
            foreach (var rule in WallRules)
            {
                bool matches = true;
                matches &= rule.HasWallTop == hasTop;
                matches &= rule.HasWallTopLeft == hasTopLeft;
                matches &= rule.HasWallTopRight == hasTopRight;
                matches &= rule.HasWallBottom == hasBottom;
                matches &= rule.HasWallBottomLeft == hasBottomLeft;
                matches &= rule.HasWallBottomRight == hasBottomRight;
                matches &= rule.HasWallLeft == hasLeft;
                matches &= rule.HasWallRight == hasRight;
                matches &= rule.boundaryType == boundaryType;

                if (matches)
                {
                    return true;
                }
            }

            return false;
        }
    }
}