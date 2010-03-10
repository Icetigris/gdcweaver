using System;
using System.Collections.Generic;
using System.Text;

namespace WorldWeaver
{
    interface ILoadableObject : ISceneObject
    {
        void LoadContent();
        void UnloadContent();
    }
}
