using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace WorldWeaver
{
    class Star : CelestialBody, IDrawableObject, ILoadableObject, IUpdatableObject
    {
        #region Declarations

        private ContentManager content;
        private int mySceneIndex;
        private Model model;
        private Matrix world;
        private ChaseCamera camera;
        //wb
        private Texture2D planetMap;
        private Texture2D planetMap_normal;
        private Texture2D planetMap_normal2;
        private Texture2D planetMap_normal3;
        private Texture2D clouds;
        private String curTechnique;
        private CustomEffects visualEffects = new CustomEffects();
        private Vector3 greyMapColorA;
        private Vector3 greyMapColorB;
        //
        #endregion

        #region Properties

        public int MySceneIndex
        {
            get { return mySceneIndex; }
            set { mySceneIndex = value; }
        }

        #endregion

        #region Constructor

        public Star(string name, Vector3 radius, Vector3 position, double r, MoleculePool pool, GraphicsDeviceManager graphics)
        {
            this.Name = name;
            this.Radius = radius;
            this.Position = position;
            this.R = r;
            this.mPool = pool;
            this.content = Globals.contentManager;
            this.camera = Globals.ChaseCamera;
            calculateEffectiveTemp();
            calculateMass();
            //wb
            visualEffects = new CustomEffects();
            CreateGreyMapColors(pool);
            //
        }

        #endregion

        #region Methods

        

        //Is this star a black hole?
        public bool IsBlackHole()
        {
            double srad = (2 * G * Mass) / (SPEEDOFLIGHT * SPEEDOFLIGHT); //Schwarzschild radius

            if (R < srad)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //What is the effective surface temperature of this star?
        public void calculateEffectiveTemp()
        {
            //temp range for stars is 2000K - 14000K
            double effTemp = 0;
            double colourIndexSum = 0;
            double colourAverage = 0;

            foreach(Particle p in mPool.Particles)
            {
                colourIndexSum += fixColourIndices(p);
            }

            colourAverage = colourIndexSum / mPool.Particles.Count;
            effTemp = 2000 * colourAverage; //average colour * 2000K

            EffectiveTemp = effTemp;
        }

        //helper function to adjust colour index values for black body temperature calculations
        //original ordering of the Colour enum would not have worked here
        private int fixColourIndices(Particle p)
        {
            int outI = 0;
            switch ((int)p.Colour)
            {
                case 0:
                    outI = 3; //red for temperature calc
                    break;
                case 1:
                    outI = 4;
                    break;
                case 2:
                    outI = 5;
                    break;
                case 3:
                    outI = 6;
                    break;
                case 4:
                    outI = 7; //blue
                    break;
                case 5:
                    outI = 2; //purple
                    break;
                case 6:
                    outI = 1; //silver
                    break;
                case 7:
                    outI = 0; //shiny
                    break;
            }

            return outI;
        }

        //calculates star mass based on the sum of the particle masses in this molecule pool
        public override void calculateMass()
        {
            foreach (Particle p in mPool.Particles)
            {
                Mass += p.AssignMass((int)p.Colour);
            }
        }

        public override void calculateGravityPoints()
        {
            GravityPoints = (int)R * 100;
        }

        #endregion

        #region Inherited Methods

        public void Update()
        {
        }

        //wb
        /* Switches between 2 colors depending on player.mPool.
		 * Once this system is plugged into planets and not stars
		 * this will only be called once upon planet creation, so
		 * it won't be an update method.
         */
        public void CreateGreyMapColors(MoleculePool mPool)
        {
            if (mPool.Particles.Count > 0 &&
                mPool.Particles.Count <= 4)
            {
                greyMapColorA = new Vector3(0, 1, 0);
                greyMapColorB = new Vector3(0, 0, 1);
            }
            else if (mPool.Particles.Count > 4 &&
                mPool.Particles.Count <= 8)
            {
                greyMapColorA = new Vector3(1, 0, 0);
                greyMapColorB = new Vector3(1.0f, 0.0f, 1.0f);
            }
            else if (mPool.Particles.Count > 8 &&
                mPool.Particles.Count <= 12)
            {
                greyMapColorA = new Vector3(1, 1, 0);
                greyMapColorB = new Vector3(0.0f, 1.0f, 1.0f);
            }
            else if (mPool.Particles.Count > 12 &&
                mPool.Particles.Count <= 16)
            {
                greyMapColorA = new Vector3(0.3f, 1, 0.3f);
                greyMapColorB = new Vector3(1.0f, 1.0f, 1.0f);
            }
            else
            {
                greyMapColorA = new Vector3(0, 0, 1.0f);
                greyMapColorB = new Vector3(0, 1.0f, 0);
            }
        }

        //When the scene graph calls each loadable object's LoadContent(), 
        //they each Content.Load their own models
        public void LoadContent()
        {
            //wb
            model = content.Load<Model>("..\\Content\\Models\\plus1");
            //model = content.Load<Model>("Models\\teapot");
            visualEffects.Phong = content.Load<Effect>(Globals.AssetList.PhongFXPath);
            planetMap = content.Load<Texture2D>("..\\Content\\Textures\\planetMap");
            planetMap_normal = content.Load<Texture2D>("..\\Content\\Textures\\planetMap_normal");
            planetMap_normal2 = content.Load<Texture2D>("..\\Content\\Textures\\planetMap_normal2");
            planetMap_normal3 = content.Load<Texture2D>("..\\Content\\Textures\\planetMap_normal3");
            clouds = content.Load<Texture2D>("..\\Content\\Textures\\clouds2");
            //end wb
            ReadyToRender = true;

        }

        public void UnloadContent()
        {
            //crap
        }
        //bool doOnce = true;
        public new void Draw(GameTime gameTime)
        {
            if (!Globals.gameplayScreenDestroyed)
            {
                //Particle's world matrix
                world = Matrix.CreateTranslation(Position);

                Matrix[] transforms = new Matrix[model.Bones.Count];
                model.CopyAbsoluteBoneTransformsTo(transforms);

                foreach (ModelMesh mesh in model.Meshes)
                {
                    //wallace brown 11/14/09
                    visualEffects.Set_Phong_Diffuse(new Vector3(1.0f, 1.0f, 1.0f), visualEffects.color_white);
                    visualEffects.Set_Phong_Ambient(visualEffects.color_white, new Vector4(0.1f, 0.1f, 0.1f, 1.0f));
                    visualEffects.Set_Phong_Specular(new Vector4(0.8f, 0.8f, 0.8f, 1.0f), visualEffects.color_white, 20.0f);

                    visualEffects.Set_IsGreymapped(true);
                    visualEffects.Set_GreyMapColors(greyMapColorA, greyMapColorB);

                    visualEffects.Set_Specials_Phong(false, false, false, true);
                    visualEffects.Update_Time(gameTime);
                    visualEffects.Update_Rotate('z',0.1f);
                    curTechnique = "Planet";
                    DrawModel_Phong(model, transforms, world, curTechnique);
                    curTechnique = "Stratosphere";
                    DrawModel_Phong(model, transforms, world, curTechnique);
                    //code End[]

                    mesh.Draw();
                }
                //spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred,
                //SaveStateMode.SaveState);
                //starTail.Draw(spriteBatch, gameTime);
                //spriteBatch.End();
            }
        }

        private void DrawModel_Phong(Model model, Matrix[] transform, Matrix world, string technique)
        {
            this.camera = Globals.ChaseCamera;


            Globals.sceneGraphManager.GraphicsDevice.VertexDeclaration = new VertexDeclaration(Globals.sceneGraphManager.GraphicsDevice, VertexPositionNormalTextureTangentBinormal.VertexElements);
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
                        visualEffects.Update_Phong(transform[mesh.ParentBone.Index] * world, camera.View, camera.Projection, camera.Position);

                        if (curTechnique.Equals("Planet"))
                        {
                            visualEffects.Set_IsGreymapped(true);
                            visualEffects.Set_GreyMapColors(greyMapColorA, greyMapColorB);
                            visualEffects.Phong.Parameters["gTex0"].SetValue(planetMap);
                            visualEffects.Phong.Parameters["gTexN"].SetValue(planetMap_normal3);
                            visualEffects.Phong.Parameters["gStratosphere"].SetValue(false);
                            visualEffects.Update_Rotate('z',0.1f);
                        }
                        if (curTechnique.Equals("Stratosphere"))
                        {
                            visualEffects.Set_IsGreymapped(false);
                            visualEffects.Phong.Parameters["gTexAnimated"].SetValue(clouds);
                            visualEffects.Phong.Parameters["gStratosphere"].SetValue(true);
                            visualEffects.Update_Rotate('z',0.2f);
                        }

                        Globals.sceneGraphManager.GraphicsDevice.Vertices[0].SetSource(mesh.VertexBuffer, part.StreamOffset, part.VertexStride);
                        Globals.sceneGraphManager.GraphicsDevice.Indices = mesh.IndexBuffer;
                        Globals.sceneGraphManager.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList,
                                                                      part.BaseVertex, 0, part.NumVertices,
                                                                      part.StartIndex, part.PrimitiveCount);
                    }
                }
                pass.End();
            }
            visualEffects.Phong.End();
        }

        #endregion
    }
}
