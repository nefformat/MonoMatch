using Microsoft.Xna.Framework.Graphics;
using MonoMatch.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoMatch
{
    static class Textures
    {
        public static Dictionary<(CellType, BonusType), Texture2D> Cells { get; private set; }
        public static Dictionary<string, Texture2D> Images { get; private set; }

        static Textures()
        {
            Cells = new Dictionary<(CellType, BonusType), Texture2D>();
            Images = new Dictionary<string, Texture2D>();
        }
    }
}
