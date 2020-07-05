using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoMatch.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoMatch.Screens
{
    class GameScreen : IScreen
    {
        private readonly int _cellSizeInPx;
        private readonly int _betweenCellsInPx;
        private readonly int _cellRows;
        private readonly int _cellCols;
        private GameField gameField;
        GamePhase gamePhase;
        private readonly SpriteBatch _spriteBatch;
        private readonly SpriteFont _font;
        private Texture2D _ribbonTexture;
        private Vector2 _ribbonPosition;
        private System.Timers.Timer _timer;
        public int Time { get; private set; }


        public GameScreen(SpriteBatch spriteBatch, SpriteFont font)
        {
            _cellSizeInPx = 80;
            _betweenCellsInPx = 10;
            _cellRows = 8;
            _cellCols = 8;

            _spriteBatch = spriteBatch;
            _font = font;

            gamePhase = GamePhase.Interaction;

            _ribbonTexture = Textures.Images["ribbon"];
            _ribbonPosition = new Vector2(730, 0);

            gameField = new GameField(gamePhase, _cellSizeInPx, _betweenCellsInPx, _cellRows, _cellCols);


            Time = 0;
            _timer = new System.Timers.Timer();
            _timer.Interval = 1000;
            _timer.Start();
            _timer.Elapsed += (x, o) => Time++;
        }

        public void Click(int x, int y)
        {
            if ((_cellSizeInPx + _betweenCellsInPx) * _cellRows > x && (_cellSizeInPx + _betweenCellsInPx) * _cellCols > y)
                gameField.Click(x, y);
        }

        public GameStatus Update()
        {
            gameField.Update();
            if (Time >= 60)
               return GameStatus.GameOverScreen;
            return GameStatus.GameScreen;
        }

        public void Draw()
        {
            _spriteBatch.Begin();

            foreach (Cell cell in gameField.Field)
            {
                _spriteBatch.Draw(texture: cell.Texture,
                    position: cell.Position,
                    origin: new Vector2(cell.Texture.Width / 2, cell.Texture.Height / 2),
                    scale: new Vector2(cell.Scale),
                    color: Color.White * cell.Alpha,
                    layerDepth: 0);
            }

            foreach (Destroyer destroyer in gameField.Destroyers)
            {
                _spriteBatch.Draw(texture: destroyer.Texture1,
                                position: destroyer.Position1,
                                origin: new Vector2(destroyer.Texture1.Width / 2, destroyer.Texture1.Height / 2));

                _spriteBatch.Draw(texture: destroyer.Texture2,
                position: destroyer.Position2,
                origin: new Vector2(destroyer.Texture2.Width / 2, destroyer.Texture2.Height / 2));
            }

            _spriteBatch.Draw(_ribbonTexture, _ribbonPosition, Color.Black);

            string pointsText = $"Time: {Time}\n\nPoints: {gameField.Points}";
            Vector2 textWidth = _font.MeasureString(pointsText);
            Vector2 position = new Vector2(830 - textWidth.X - 10, 20);
            _spriteBatch.DrawString(_font, pointsText, position, Color.White);

            _spriteBatch.End();
        }
    }
}
