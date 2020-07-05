using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMatch.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoMatch
{
    class Destroyer
    {
        public Texture2D Texture1 { get; private set; }
        public Texture2D Texture2 { get; private set; }
        public Vector2 Position1 { get { return _position1; } }
        private Vector2 _startPosition1;
        private Vector2 _position1;
        private Vector2 _finalPosition1;
        public Vector2 Position2 { get { return _position2; } }
        private Vector2 _startPosition2;
        private Vector2 _position2;
        private Vector2 _finalPosition2;

        private BonusType _type;
        private float _speed;
        private int _step;
        public bool IsBusy
        {
            get
            {
                if (_position1 != _finalPosition1 || _position2 != _finalPosition2)
                    return true;
                return false;
            }
        }
        public bool IsDestroyed { get; set; }
        public event EventHandler StepCompleted;
        public event EventHandler ActionCompleted;

        public Destroyer(Vector2 startPosition, Vector2 finalPosition1, Vector2 finalPosition2, BonusType destroyerType, int step)
        {
            _type = destroyerType;
            _startPosition1 = startPosition;
            _startPosition2 = startPosition;

            _position1 = startPosition;
            _position2 = startPosition;
            _step = step;
            if (destroyerType == BonusType.HorDestroyer)
            {
                Texture1 = Textures.Images["leftdestroyer"];
                Texture2 = Textures.Images["rightdestroyer"];
            }
            else
            {
                Texture1 = Textures.Images["updestroyer"];
                Texture2 = Textures.Images["downdestroyer"];
            }

            _finalPosition1 = finalPosition1;
            _finalPosition2 = finalPosition2;


            _speed = 10f;

            IsDestroyed = false;
        }
        public void Update()
        {
            if (_type == BonusType.HorDestroyer)
            {
                if (_position1.X != _finalPosition1.X)
                {
                    Move(ref _position1.X, _finalPosition1.X);
                    if (Math.Abs(_position1.X - _startPosition1.X) > _step &&
                        Math.Abs(_position1.X - _startPosition1.X) % _step < _speed)
                    {
                        StepCompleted.Invoke(this, new EventArgs());
                    }
                }

                if (_position2.X != _finalPosition2.X)
                {
                    Move(ref _position2.X, _finalPosition2.X);
                    if (Math.Abs(_position2.X - _startPosition2.X) > _step &&
                        Math.Abs((_position2.X - _startPosition2.X)) % _step < _speed + 1)
                    {
                        StepCompleted.Invoke(this, new EventArgs());
                    }
                }
            }
            else
            {
                if (_position1.Y != _finalPosition1.Y)
                {
                    Move(ref _position1.Y, _finalPosition1.Y);
                    if (Math.Abs(_position1.Y - _startPosition1.Y) > _step &
                        Math.Abs(_position1.Y - _startPosition1.Y) % _step < _speed + 1)
                    {
                        StepCompleted.Invoke(this, new EventArgs());
                    }
                }

                if (_position2.Y != _finalPosition2.Y)
                {
                    Move(ref _position2.Y, _finalPosition2.Y);
                    if (Math.Abs(_position2.Y - _startPosition2.Y) > _step &&
                        Math.Abs(_position2.Y - _startPosition2.Y) % _step < _speed + 1)
                    {
                        StepCompleted.Invoke(this, new EventArgs());
                    }
                }
            }
        }
        private void Move(ref float p1, float p2)
        {
            if (p1 != p2)
            {
                int mult = (p1 < p2) ? 1 : -1;
                float inc = _speed * mult;
                if (Math.Abs(p1 - p2) <= _speed)
                {
                    p1 = p2;
                    IsDestroyed = true;
                    ActionCompleted.Invoke(this, new EventArgs());
                }
                else
                    p1 += inc;
            }
        }
    }
}
