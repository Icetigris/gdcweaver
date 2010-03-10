using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WorldWeaver
{
    public class SceneObject : ISceneObject
    {
        //Is this object ready to be rendered?
        private bool _readyToRender = false;
        public bool ReadyToRender
        {
            get { return _readyToRender; }
            set { _readyToRender = value; }
        }

        //Name of object in the scene
        private string objectName;
        public string ObjectName
        {
            get { return objectName; }
            set { objectName = value; }
        }

        //3D Model
        //************should probably have filepath here rather than actual model*********************
        

        //World matrix
        private Matrix _world = Matrix.Identity;

        //THIS IS VIRTUAL BECAUSE PLAYER IS OVERRIDING IT.  Wasn't before, and I have no idea how it
        //was even compiling.
        public virtual Matrix World
        {
            get { return _world; }
            set { _world = value; }
        }

        //Object's position
        private Vector3 _position = Vector3.Zero;
        public Vector3 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        //Object's rotation in Quaternion form
        private Quaternion _rotation = Quaternion.Identity;
        public Quaternion Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }

        //Object's scale
        private Vector3 _scale = Vector3.One;
        public Vector3 Scale
        {
            get { return _scale; }
            set { _scale = value; }
        }

        public void Draw(GameTime gameTime)
        {
            if (this is IDrawableObject)
            {
                //draw object
                //******THIS IS THE BOTTOM OF THE DRAW CALL HIERARCHY******
                //******YOU ACTUALLY DRAW SHIT HERE************************
                /*Matrix[] transforms = new Matrix[model.Bones.Count];
                model.CopyAbsoluteBoneTransformsTo(transforms);

                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.PreferPerPixelLighting = true;
                        effect.World = transforms[mesh.ParentBone.Index] * _world;

                        // Use the matrices provided by the chase camera
                        effect.View = _chaseCamera.View;
                        effect.Projection = _chaseCamera.Projection;
                    }

                    mesh.Draw();
                }
                Console.WriteLine("SceneObject Draw()");*/
            }
        }
    }
}
