using MonoMatch.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoMatch.Screens
{
    interface IScreen
    {
        void Draw();
        GameStatus Update();
        void Click(int x, int y);
    }
}
