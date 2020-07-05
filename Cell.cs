using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMatch.Enums;

namespace MonoMatch
{
    class Cell
    {
        private float speed;

        //public bool IsSelected { get; set; }
        private CellType _cellType;
        public CellType CellType
        {
            get
            {
                return _cellType;
            }
            set
            {
                _cellType = value;
                Texture = Textures.Cells[(value, this.BonusType)];
            }
        }
        public bool IsBusy
        {
            get
            {
                if (Position != newPosition || Alpha != newAlpha)
                    return true;
                return false;
            }
        }
        public float Alpha { get; set; }
        private float alphaSpeed;
        private float newAlpha;
        public bool IsDestroyed { get; set; }
        private BonusType _bonusType;
        public BonusType BonusType
        {
            get
            {
                return _bonusType;
            }
            set
            {
                _bonusType = value;
                Texture = Textures.Cells[(this.CellType, value)];
            }
        }

        public event EventHandler ActionCompleted;

        public Texture2D Texture { get; private set; }
        private Vector2 _position;
        public Vector2 Position
        {
            get
            {
                return _position;
            }
            private set
            {
                _position = value;
            }
        }

        private Vector2 newPosition;
        private Vector2 oldPosition;

        public bool IsSelected { get; set; }
        public float Scale { get; private set; }
        private float scaleSpeed;
        private float maxScale;
        private float minScale;
        public Cell(CellType cellType, Vector2 position)
        {
            CellType = cellType;
            BonusType = BonusType.None;
            Position = position;
            newPosition = position;
            oldPosition = position;

            speed = 5f;
            Scale = 1f;
            Alpha = 1f;
            newAlpha = 1f;
            alphaSpeed = 0.4f;

            maxScale = 1.15f;
            minScale = 0.9f;
            scaleSpeed = 0.03f;

            IsDestroyed = false;
        }

        public void Update()
        {
            if (Position != newPosition)
            {
                Move(ref _position.X, newPosition.X);
                Move(ref _position.Y, newPosition.Y);
            }

            if (Alpha != newAlpha)
                UpdateAlpha();


            if (IsSelected)
                UpdateScale();
            else
                Scale = 1f;
        }

        private void UpdateAlpha()
        {
            if (Alpha - alphaSpeed > newAlpha)
                Alpha -= alphaSpeed;
            else
                Alpha = newAlpha;
            if (Alpha == 0)
            {
                IsDestroyed = true;
                ActionCompleted.Invoke(this, new EventArgs());
            }
        }

        private void UpdateScale()
        {
            if ((scaleSpeed > 0 && Scale + scaleSpeed > maxScale) ||
                (scaleSpeed < 0 && Scale + scaleSpeed < minScale))
                scaleSpeed *= -1;
            Scale += scaleSpeed;
        }

        private void Move(ref float p1, float p2)
        {
            if (p1 != p2)
            {
                int mult = (p1 < p2) ? 1 : -1;
                float inc = speed * mult;
                if (Math.Abs(p1 - p2) <= speed)
                {
                    _position = newPosition;
                    ActionCompleted.Invoke(this, new EventArgs());
                }
                else
                    p1 += inc;
            }
        }
        public void MoveTo(Vector2 position)
        {
            oldPosition = Position;
            newPosition = position;
        }
        public void MoveBack()
        {
            newPosition = oldPosition;
        }

        public void Destroy()
        {
            newAlpha = 0;
        }
        public void DestroyByBomb()
        {
            Alpha = 1.5f;
            alphaSpeed = 0.04f;
            newAlpha = 0;
        }
    }
}
