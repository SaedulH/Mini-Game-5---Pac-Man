using System;
using UnityEngine;

namespace CoreSystem
{
    [Serializable]
    public class WallRules
    {
        public bool IsBoundary;
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
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public Mesh Mesh { get; private set; }
        [field: SerializeField] public bool FlipX { get; private set; }
        [field: SerializeField] public bool FlipY { get; private set; }
        [field: SerializeField] public WallRules[] WallRules { get; private set; }

        [field: Header("Wall Rules")]
        [field: SerializeField] public bool IsBoundary { get; private set; }
        [field: SerializeField] public bool HasWallTop { get; private set; }
        [field: SerializeField] public bool HasWallTopLeft { get; private set; }
        [field: SerializeField] public bool HasWallTopRight { get; private set; }
        [field: SerializeField] public bool HasWallBottom { get; private set; }
        [field: SerializeField] public bool HasWallBottomLeft { get; private set; }
        [field: SerializeField] public bool HasWallBottomRight { get; private set; }
        [field: SerializeField] public bool HasWallLeft { get; private set; }
        [field: SerializeField] public bool HasWallRight { get; private set; }

        public bool Matches(bool isBoundary, bool hasTop, bool hasTopLeft, bool hasTopRight, bool hasBottom, bool hasBottomLeft, bool hasBottomRight, bool hasLeft, bool hasRight)
        {
            foreach (var rule in WallRules)
            {
                bool matches = true;
                matches &= rule.IsBoundary == isBoundary;
                matches &= rule.HasWallTop == hasTop;
                matches &= rule.HasWallTopLeft == hasTopLeft;
                matches &= rule.HasWallTopRight == hasTopRight;
                matches &= rule.HasWallBottom == hasBottom;
                matches &= rule.HasWallBottomLeft == hasBottomLeft;
                matches &= rule.HasWallBottomRight == hasBottomRight;
                matches &= rule.HasWallLeft == hasLeft;
                matches &= rule.HasWallRight == hasRight;
                if (matches)
                {
                    return true;
                }
            }

            return false;
        }
    }
}