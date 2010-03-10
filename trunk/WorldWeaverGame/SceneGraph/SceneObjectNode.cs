using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace WorldWeaver
{
    public class SceneObjectNode : Node
    {
        private SceneObject _sceneObject;
        public SceneObject SceneObject
        {
            get { return _sceneObject; }
            set { _sceneObject = value; }
        }

        public SceneObjectNode(SceneObject newObject)
        {
            _sceneObject = newObject;
        }

        public override void Update()
        {
            if (SceneObject is IUpdatableObject && Globals.gameplayScreenHasFocus)
            {
                ((IUpdatableObject)SceneObject).Update();
            }
        }

        public override void LoadContent()
        {
            if (SceneObject is ILoadableObject)
            {
                ((ILoadableObject)SceneObject).LoadContent();
                
            }
        }

        public override void UnloadContent()
        {
            if (SceneObject is ILoadableObject)
            {
                ((ILoadableObject)SceneObject).UnloadContent();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (SceneObject is IDrawableObject && SceneObject.ReadyToRender)
            {
                ((IDrawableObject)SceneObject).Draw(gameTime);
                //SceneObject.Draw(gameTime);
                //Console.WriteLine("SceneObjectNode Draw()");
            }
        }
    }
}
