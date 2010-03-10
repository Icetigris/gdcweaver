using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace WorldWeaver
{
    interface IDrawableObject : ISceneObject
    {
        void Draw(GameTime gameTime);
    }
}
