using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMatch.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoMatch.Screens
{
    class StartScreen: IScreen
    {
        private readonly SpriteBatch _spriteBatch;
        private readonly SpriteFont _font;
        private readonly Texture2D _startScreenTexture;
        private readonly Vector2 _startScreenPosition;
        private GameStatus _nextScreen;

        public StartScreen(SpriteBatch spriteBatch, SpriteFont font)
        {
            _spriteBatch = spriteBatch;
            _font = font;

            _startScreenTexture = Textures.Images["startscreen"];
            _startScreenPosition = new Vector2(0, 0);
            _nextScreen = GameStatus.StartScreen;
        }

        public GameStatus Update()
        {
            return _nextScreen;
        }

        public void Draw()
        {
            _spriteBatch.Begin();
            _spriteBatch.Draw(_startScreenTexture, _startScreenPosition, Color.White);
            _spriteBatch.End();
        }
        public void Click(int x, int y)
        {
            //if (x > 0 && x < 300 && y > 0 && y < 300)
                _nextScreen = GameStatus.GameScreen;
        }
    }
}
