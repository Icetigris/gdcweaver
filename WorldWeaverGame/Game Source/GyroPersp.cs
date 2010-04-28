#region File Description
//-----------------------------------------------------------------------------
// GyroPersp.cs
//
// World Weaver
// Joseph Breeden
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
#endregion

namespace WorldWeaver
{
    public class GyroPersp : SceneObject, IDrawableObject, ILoadableObject, IUpdatableObject
    {
        #region Declarations
        

        // Create model variable
        private int mySceneIndex;
        private ContentManager content;
        private Model model, gyro_needle;

        //public Vector3 Position;
        public Vector3 Direction;
        public Vector3 Up;
        private Vector3 right;
        public Vector3 Right
        {
            get { return right; }
        }
        
        // Scene Index get set code
        public int MySceneIndex
        {
            get { return mySceneIndex; }
            set { mySceneIndex = value; }
        }

        // Stuff relative to the new shaders.
        GraphicsDeviceManager graphics;
        CustomEffects visualEffects = new CustomEffects();
        Texture2D mTexture;
        private HudCamera camera;

        Matrix world = Matrix.Identity;

        #endregion

        #region Constructors
        // Region contains constructors

        //========================
        // DEFAULT CONSTRUCTOR
        //========================
        public GyroPersp()
        {
            
        }

        //========================
        // CONSTRUCTOR (Vector3, ChaseCamera)
        //========================
        public GyroPersp(GraphicsDeviceManager graphics)
        {
            this.content = Globals.contentManager;
            this.graphics = graphics;
        }

        #endregion

        #region Methods

        //========================
        // public void LoadContent()
        //========================
        public void LoadContent()
        {

            // Loads data from the scenegraph manager?
            //gyro = Globals.hudManager.Game.Content.Load<Model>(modelPath);

            //Position = new Rectangle((int)((Globals.hudManager.HudAreaWidth * 0.2f) / 2 + 50),
            //(int)((Globals.hudManager.HudAreaHeight - Globals.hudManager.HudAreaHeight * 0.1f) / 2),
            //(int)((Globals.hudManager.HudAreaWidth * 0.2f) / 2 + 50),
            //(int)((Globals.hudManager.HudAreaHeight - Globals.hudManager.HudAreaHeight * 0.1f) / 2));

            model = content.Load<Model>(Globals.AssetList.playerNEG1ModelPath);
            gyro_needle = content.Load<Model>(Globals.AssetList.gyro_needle);
            mTexture = content.Load<Texture2D>("Models\\testTex2");
            visualEffects.Phong = content.Load<Effect>(Globals.AssetList.PhongFXPath);
            ReadyToRender = true;
        }

        //========================
        // public void UnloadContent()
        //========================
        public void UnloadContent()
        {
            // Required method for the scenegraph manager
        }

        //========================
        // public void Update()
        //========================
        public void Update()
        {
            // Updates the Gyro's direction with each frame.
            world = Matrix.Identity;

            world.Translation = new Vector3(230,-230,-800);
            
            world.Forward = Globals.Player.Direction;
            world.Up = Globals.Player.Up;
            world.Right = Globals.Player.Right;
        }


        #region Draw Methods

        public new void Draw(GameTime gameTime)
        {
          //  this.Scale = new Vector3(0.1f, 0.1f, 0.1f);

            //wallace brown 11/14/09
            visualEffects.Init_Phong();
            //code End[]
            if (!Globals.gameplayScreenDestroyed)
            {
                Matrix[] transforms = new Matrix[gyro_needle.Bones.Count];
                gyro_needle.CopyAbsoluteBoneTransformsTo(transforms);

                foreach (ModelMesh mesh in gyro_needle.Meshes)
                //wallace brown 11/14/09
                visualEffects.Set_Specials_Phong(false, false, false, false);
                visualEffects.Set_Phong_Diffuse(new Vector3(1.0f, 1.0f, 1.0f), visualEffects.color_white);
                visualEffects.Set_Phong_Ambient(visualEffects.color_white, new Vector4(0.1f, 0.1f, 0.1f, 1.0f));
                visualEffects.Set_Phong_Specular(new Vector4(0.8f, 0.8f, 0.8f, 1.0f), visualEffects.color_white, 20.0f);

                DrawModel_Phong(gyro_needle, transforms, world, "Main");
                //code End[]
            }
        }

        private void DrawModel_Phong(Model model, Matrix[] transform, Matrix world, string technique)
        {
            this.camera = Globals.hudCamera;

            graphics.GraphicsDevice.VertexDeclaration = new VertexDeclaration(graphics.GraphicsDevice, VertexPositionNormalTextureTangentBinormal.VertexElements);
            visualEffects.Phong.CurrentTechnique = visualEffects.Phong.Techniques[technique];

            visualEffects.Phong.Begin();

            foreach (EffectPass pass in visualEffects.Phong.CurrentTechnique.Passes)
            {
                // Begin current pass
                pass.Begin();

                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        part.Effect = visualEffects.Phong;
                        visualEffects.Update_Phong(transform[mesh.ParentBone.Index] * world, camera.View /*camera.View*/, camera.Projection, camera.Position/*camera.Position*/);
                        visualEffects.Phong.Parameters["gTex0"].SetValue(mTexture);

                        graphics.GraphicsDevice.Vertices[0].SetSource(mesh.VertexBuffer, part.StreamOffset, part.VertexStride);
                        graphics.GraphicsDevice.Indices = mesh.IndexBuffer;
                        graphics.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList,
                                                                      part.BaseVertex, 0, part.NumVertices,
                                                                      part.StartIndex, part.PrimitiveCount);
                    }
                    //mesh.Draw();
                }
                pass.End();
            }
            visualEffects.Phong.End();
        }

        private void DrawModel_Phong_Special(Model model, Matrix[] transform, Matrix world, string technique, GameTime time)
        {
            this.camera = Globals.hudCamera;

            graphics.GraphicsDevice.VertexDeclaration = new VertexDeclaration(graphics.GraphicsDevice, VertexPositionNormalTextureTangentBinormal.VertexElements);
            visualEffects.Phong.CurrentTechnique = visualEffects.Phong.Techniques[technique];

            visualEffects.Phong.Begin();

            foreach (EffectPass pass in visualEffects.Phong.CurrentTechnique.Passes)
            {
                // Begin current pass
                pass.Begin();

                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        part.Effect = visualEffects.Phong;
                        visualEffects.Update_Phong(transform[mesh.ParentBone.Index] * world, camera.View/*camera.View*/, camera.Projection, camera.Position/*camera.Position*/);

                        graphics.GraphicsDevice.Vertices[0].SetSource(mesh.VertexBuffer, part.StreamOffset, part.VertexStride);
                        graphics.GraphicsDevice.Indices = mesh.IndexBuffer;
                        graphics.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList,
                                                                      part.BaseVertex, 0, part.NumVertices,
                                                                      part.StartIndex, part.PrimitiveCount);
                    }
                }
                pass.End();
            }
            visualEffects.Phong.End();
        }

        #endregion

        #endregion
    }
}
