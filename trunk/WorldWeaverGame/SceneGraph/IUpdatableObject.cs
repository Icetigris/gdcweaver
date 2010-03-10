using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace WorldWeaver
{
    interface IUpdatableObject : ISceneObject
    {
        void Update();
    }
}
