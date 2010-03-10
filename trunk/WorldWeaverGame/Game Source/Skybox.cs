using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;


namespace WorldWeaver
{
    class Skybox : SceneObject, IDrawableObject, ILoadableObject, IUpdatableObject
    {
        #region Variables

        private Matrix world = Matrix.Identity;
        private ContentManager content;
        private string modelPath;
        private Model model;
        private ChaseCamera camera;
        private int mySceneIndex;

        //Wallace brown 11/18/09
        //private CustomEffects visualEffects = new CustomEffects();
        //private Texture2D mTexture;
        GraphicsDeviceManager graphics;
        //end Code[]

        #endregion

        #region Properties

        public string ModelPath
        {
            get { return modelPath; }
            set { modelPath = value; }
        }

        public int MySceneIndex
        {
            get { return mySceneIndex; }
            set { mySceneIndex = value; }
        }

        #endregion

        #region Constructors

        public Skybox(ChaseCamera camera, GraphicsDeviceManager graphics)//, GraphicsDeviceManager gdm)
        {
            this.content = Globals.contentManager;
            this.camera = camera;
            this.graphics = graphics;
        }

        #endregion

        #region Inherited Methods

        public void LoadContent()
        {
            modelPath = Globals.AssetList.skyboxModelPath;
            model = content.Load<Model>(modelPath);
            ReadyToRender = true;
        }

        public void UnloadContent()
        {
        }

        public void Update()
        {
            Position = Globals.Player.Position;
        }

        public new void Draw(GameTime gameTime)
        {
            //if (!Globals.gameplayScreenDestroyed && Globals.gameplayScreenHasFocus)
            //{
                world = Matrix.CreateTranslation(Position);

                Matrix[] transforms = new Matrix[model.Bones.Count];
                model.CopyAbsoluteBoneTransformsTo(transforms);
          
                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.PreferPerPixelLighting = true;
                        effect.World = transforms[mesh.ParentBone.Index] * world;
                        // Use the matrices provided by the chase camera
                        effect.View = camera.View;
                        effect.Projection = camera.Projection;
                        graphics.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
                    }
                    mesh.Draw();
                    //Console.WriteLine("RenderState.DepthBufferEnable: " + graphicsManager.GraphicsDevice.RenderState.DepthBufferEnable);
                }
            //}
        }

        #region Draw Methods

        #endregion

        #endregion
    }
}
