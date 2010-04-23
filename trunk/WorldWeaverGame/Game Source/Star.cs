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
        private TextureCube starCubeMap_Low;
        private TextureCube starCubeMap_Mid;
        private TextureCube starCubeMap_Top;
        private TextureCube starCubeMap_Normal;
        private CustomEffects visualEffects = new CustomEffects();
        private Vector3 greyMapColorA;
        private Vector3 greyMapColorB;
        private Vector3 greyMapColorC;
        private BoundingSphere collisionSphere;
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

            collisionSphere = new BoundingSphere(Position, (float)R);
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
            CollidePlayer(this.collisionSphere, Globals.Player);
        }

        public void Update(GameTime gameTime)
        {
            
        }

        private void CollidePlayer(BoundingSphere boundingSphere, Player player)
        {
            if (boundingSphere.Intersects(player.CollisionSphere))
            {
                Vector3 newPlayerPosition = new Vector3();

                float distance = boundingSphere.Radius + player.CollisionSphere.Radius;

                //is this faster than just doing (Math.Abs(Center) - Math.Abs(Position)) * (Center < 0) ? -1:1)
                //Probably not?, this sorta works though?
                Vector3 normalizedDifference = Vector3.Normalize(boundingSphere.Center - player.Position); //the normalized distance
                Vector3 normalizedDifferencePlusOne = normalizedDifference + new Vector3(1); //make it all positive
                Vector3 normalizedPositiveDifference = normalizedDifferencePlusOne / new Vector3(2); //scale it from 0 to 1
                Vector3 normalDifference = Vector3.One - normalizedPositiveDifference; //find the distance we need to add to the player
                Vector3 differenceOneHalfScale = (normalDifference - new Vector3(0.5f)); //scale it from -.5 to .5
                Vector3 scaled = differenceOneHalfScale * new Vector3(2); //scale it from -1 to 1

                newPlayerPosition.X = boundingSphere.Center.X + (scaled.X * distance);
                newPlayerPosition.Y = boundingSphere.Center.Y + (scaled.Y * distance);
                newPlayerPosition.Z = boundingSphere.Center.Z + (scaled.Z * distance);

                player.Position = newPlayerPosition;
            }
        }

      
        public void CreateGreyMapColors(MoleculePool mPool)
        {
            //if (mPool.Particles.Count > 0 &&
            //    mPool.Particles.Count <= 4)
            //{
            //    greyMapColorA = new Vector3(0, 1, 0);
            //    greyMapColorB = new Vector3(0, 0, 1);
            //}
            //else if (mPool.Particles.Count > 4 &&
            //    mPool.Particles.Count <= 8)
            //{
            //    greyMapColorA = new Vector3(1, 0, 0);
            //    greyMapColorB = new Vector3(1.0f, 0.0f, 1.0f);
            //}
            //else if (mPool.Particles.Count > 8 &&
            //    mPool.Particles.Count <= 12)
            //{
            //    greyMapColorA = new Vector3(1, 1, 0);
            //    greyMapColorB = new Vector3(0.0f, 1.0f, 1.0f);
            //}
            //else if (mPool.Particles.Count > 12 &&
            //    mPool.Particles.Count <= 16)
            //{
            //    greyMapColorA = new Vector3(0.3f, 1, 0.3f);
            //    greyMapColorB = new Vector3(1.0f, 1.0f, 1.0f);
            //}
            //else
            //{
            //    greyMapColorA = new Vector3(0, 0, 1.0f);
            //    greyMapColorB = new Vector3(0, 1.0f, 0);
            //}

            //ELIZABETH TIEM:
            //make the greyMap A and B colours dependent on the 2 most predominant colours in the mPool
            //greyMapColorA = Particle.convertColor((int)mPool.getColourFrequencies()[0]);
            //greyMapColorB = Particle.convertColor((int)mPool.getColourFrequencies()[1]);

            greyMapColorA = new Vector3(1, 1, 1); 
            greyMapColorB = new Vector3(1, 0.49f, 0.14f);
            greyMapColorC = new Vector3(0.80f,0,0);
        }

        //When the scene graph calls each loadable object's LoadContent(), 
        //they each Content.Load their own models
        public void LoadContent()
        {
            //wb
            model = content.Load<Model>("..\\Content\\Models\\sphere");
            //model = content.Load<Model>("Models\\teapot");
            visualEffects.Phong = content.Load<Effect>(Globals.AssetList.PhongFXPath);
            starCubeMap_Mid = content.Load<TextureCube>("..\\Content\\Textures\\grey_cube_mid");
            starCubeMap_Top = content.Load<TextureCube>("..\\Content\\Textures\\top_5");
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
                world = Matrix.CreateTranslation(Position);

                Matrix[] transforms = new Matrix[model.Bones.Count];
                model.CopyAbsoluteBoneTransformsTo(transforms);

                foreach (ModelMesh mesh in model.Meshes)
                {
                    //wallace brown 11/14/09
                    visualEffects.Set_Phong_Diffuse(new Vector3(1.0f, 1.0f, 1.0f), visualEffects.color_white);
                    visualEffects.Set_Phong_Ambient(visualEffects.color_white, new Vector4(0.2f, 0.2f, 0.2f, 1.0f));
                    visualEffects.Set_Phong_Specular(new Vector4(0.8f, 0.8f, 0.8f, 1.0f), visualEffects.color_white, 20.0f);

                    visualEffects.Set_IsGreymapped(true);
                    visualEffects.Set_GreyMapColors(greyMapColorA, greyMapColorB, greyMapColorC);

                    visualEffects.Set_Specials_Phong(false, false, false, true);
                    visualEffects.Update_Time(gameTime);
                    visualEffects.Update_Rotate('z',0.1f);
                    DrawModel_Phong(model, transforms, world, "Star");
                    //code End[]

                    mesh.Draw();
                }
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

                        visualEffects.Phong.Parameters["gTex1"].SetValue(starCubeMap_Mid);
                        visualEffects.Phong.Parameters["gTex2"].SetValue(starCubeMap_Top);
                        visualEffects.Phong.Parameters["gStratosphere"].SetValue(false);
                        visualEffects.Update_Rotate('z', 0.1f);

                        visualEffects.Phong.CommitChanges();
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
