#region File Description
//-----------------------------------------------------------------------------
// Planet.cs
//
// World Weaver
// Elizabeth Baumel
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace WorldWeaver
{
    class Planet : CelestialBody, IDrawableObject, ILoadableObject, IUpdatableObject
    {
        #region Declarations

        private ContentManager content;
        private int mySceneIndex;
        private Model model;
        private Matrix world;
        private ChaseCamera camera;
        //wb
        private Texture2D planetMap;
        private TextureCube planetMapCube;
        private Texture2D planetMap_normal;
        private TextureCube planetMapCube_normal;
        private Texture2D clouds;
        private String curTechnique;
        private CustomEffects visualEffects = new CustomEffects();
        private Vector3 greyMapColorA;
        private Vector3 greyMapColorB;
        private Player player;
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

        public Planet(string name, Vector3 radius, Vector3 position, double r, MoleculePool pool, GraphicsDeviceManager graphics)
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

            CollisionSphere = new BoundingSphere(this.Position, (float)this.R);
            isVisible = true;
        }

        #endregion

        #region Methods

        //if mass of the planet is at least 30% due to silver particles, you get a magnetic field
        public void hasMagneticField()
        {
            // Get total quantity of particles so we can do the math
            //int numParticles = mPool.Particles;

            foreach (Particle p in mPool.Particles)
            {
                
            }
        }

        //What is the effective surface temperature of this planet?
        public void calculateEffectiveTemp()
        {
            //temp for a planet depends on:
            //D  = distance from a Planet 
            //L = luminosity
            //A = surface area of planet
            //sigma = Stefan�Boltzmann constant = 5.6704E10^-8 Wm^-2 K^-4
            //here's an expression:
            //effTemp = ((L(1 - A)) / (16 * PI * sigma * D^2))^1/4

            //Earth's effTemp is about 43�F/6�C
            //so within normal range, but on a chilly day
            //double effTemp = 0;
            //double colourIndexSum = 0;
            //double colourAverage = 0;       

            //NEED TO RECOMPUTE colourAverage TO ACCOMODATE PARTICLE COLOURS' EFFECTS ON
            //INTERNAL TEMP OF PLANET

            EffectiveTemp = 1; //TEMPORARY SHIT VALUE
        }

        //calculates Planet mass based on the sum of the particle masses in this molecule pool
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
            player = Globals.Player;
            CollidePlayer(this.CollisionSphere, this.player);
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

                Vector3 temp = new Vector3(newPlayerPosition.X, newPlayerPosition.Y, newPlayerPosition.Z);
                //Vector3 distanceFromBorder;

                //distanceFromBorder.Y = Math.Min(Player.MaximumAltitude - newPlayerPosition.Y, newPlayerPosition.Y - Player.MinimumAltitude);
                //distanceFromBorder.X = Math.Min(Player.MaximumX - newPlayerPosition.X, newPlayerPosition.X - Player.MinimumX);
                //distanceFromBorder.Z = Math.Min(Player.MaximumZ - newPlayerPosition.Z, newPlayerPosition.Z - Player.MinimumZ);

                //if (distanceFromBorder.Y < 10)
                {
                    // Prevent player from flying under the ground
                    temp.Y = Math.Max(newPlayerPosition.Y, Player.MinimumAltitude + (float)(R * 0f));
                    temp.Y = Math.Min(newPlayerPosition.Y, Player.MaximumAltitude - (float)R * 0f);
                }
                //if (distanceFromBorder.X < 10)
                {
                    // Prevent player from flying out to nowhere in the x direction
                    temp.X = Math.Max(newPlayerPosition.X, Player.MinimumX + (float)R * 0f);
                    temp.X = Math.Min(newPlayerPosition.X, Player.MaximumX - (float)R * 0f);
                }
                //if (distanceFromBorder.Z < 10)
                {
                    // Prevent player from flying out to nowhere in the z direction
                    temp.Z = Math.Max(newPlayerPosition.Z, Player.MinimumZ + (float)R * 0f);
                    temp.Z = Math.Min(newPlayerPosition.Z, Player.MaximumZ - (float)R * 0f);
                }

                player.Position = temp;
            }
        }

        private void CollideParticle(BoundingSphere boundingSphere, Particle particle)
        {
            if (boundingSphere.Intersects(particle.CollectionSphere))
            {
                Vector3 newParticlePosition = new Vector3();
                float distance = boundingSphere.Radius + player.CollisionSphere.Radius;

                //is this faster than just doing (Math.Abs(Center) - Math.Abs(Position)) * (Center < 0) ? -1:1)
                //Probably not?, this sorta works though?
                Vector3 normalizedDifference = Vector3.Normalize(boundingSphere.Center - particle.Position); //the normalized distance
                Vector3 normalizedDifferencePlusOne = normalizedDifference + new Vector3(1); //make it all positive
                Vector3 normalizedPositiveDifference = normalizedDifferencePlusOne / new Vector3(2); //scale it from 0 to 1
                Vector3 normalDifference = Vector3.One - normalizedPositiveDifference; //find the distance we need to add to the player
                Vector3 differenceOneHalfScale = (normalDifference - new Vector3(0.5f)); //scale it from -.5 to .5
                Vector3 scaled = differenceOneHalfScale * new Vector3(2); //scale it from -1 to 1

                newParticlePosition.X = boundingSphere.Center.X + (scaled.X * distance);
                newParticlePosition.Y = boundingSphere.Center.Y + (scaled.Y * distance);
                newParticlePosition.Z = boundingSphere.Center.Z + (scaled.Z * distance);

                particle.Position = newParticlePosition;
            }
        }

        /// <summary>
        /// Manages the colors of the greymaps currently used to texture
        /// the planets. At this time supports only 2 colors at a time.
        /// </summary>
        /// <param name="mPool"></param>
        public void CreateGreyMapColors(MoleculePool mPool)
        {
            //ELIZABETH TIEM:
            //make the greyMap A and B colours dependent on the 2 most predominant colours in the mPool
            greyMapColorA = Particle.convertColor((int)mPool.getColourFrequencies()[0]);
            greyMapColorB = Particle.convertColor((int)mPool.getColourFrequencies()[1]);
        }

        //When the scene graph calls each loadable object's LoadContent(), 
        //they each Content.Load their own models
        public void LoadContent()
        {
            //wb
            model = content.Load<Model>("..\\Content\\Models\\sphere");
            //model = content.Load<Model>("Models\\teapot");
            visualEffects.Phong = content.Load<Effect>(Globals.AssetList.PhongFXPath);
            planetMap = content.Load<Texture2D>("..\\Content\\Textures\\planet_cubeMap");
            planetMapCube = content.Load<TextureCube>("..\\Content\\Textures\\testCubeMap");
            planetMapCube_normal = content.Load<TextureCube>("..\\Content\\Textures\\normalCube");
            planetMap_normal = content.Load<Texture2D>("..\\Content\\Textures\\planet_normalCubeMap");
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
            if (!Globals.gameplayScreenDestroyed && isVisible)
            {
                //Particle's world matrix
                world = Matrix.Identity * Matrix.CreateScale((float)this.R / 400) * Matrix.CreateTranslation(Position);

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
                //PlanetTail.Draw(spriteBatch, gameTime);
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
                            visualEffects.Phong.Parameters["gNumLights"].SetValue(Globals.numLights);

                            String parameter;
                            for (int v = 0; v < Globals.numLights; v++)
                            {
                                parameter = "gLightPos_multiple_" + (v + 1);
                                visualEffects.Phong.Parameters[parameter].SetValue(Globals.lights[v]._position);
                                parameter = "gDiffuseMtrl_multiple_" + (v + 1);
                                visualEffects.Phong.Parameters[parameter].SetValue(Globals.lights[v]._diffuse_material);
                                parameter = "gDiffuseLight_multiple_" + (v + 1);
                                visualEffects.Phong.Parameters[parameter].SetValue(Globals.lights[v]._diffuse_light);
                                parameter = "gSpecularMtrl_multiple_" + (v + 1);
                                visualEffects.Phong.Parameters[parameter].SetValue(Globals.lights[v]._specular_material);
                                parameter = "gSpecularLight_multiple_" + (v + 1);
                                visualEffects.Phong.Parameters[parameter].SetValue(Globals.lights[v]._specular_light);
                            }


                            visualEffects.Set_IsGreymapped(true);
                            visualEffects.Set_GreyMapColors(greyMapColorA, greyMapColorB);
                            visualEffects.Phong.Parameters["gTex0"].SetValue(planetMapCube);
                            visualEffects.Phong.Parameters["gTexN"].SetValue(planetMapCube_normal);
                            visualEffects.Phong.Parameters["gStratosphere"].SetValue(false);
                            visualEffects.Update_Rotate('z', 0.1f);
                        }
                        if (curTechnique.Equals("Stratosphere"))
                        {
                            visualEffects.Set_IsGreymapped(false);
                            visualEffects.Phong.Parameters["gTexAnimated"].SetValue(clouds);
                            visualEffects.Phong.Parameters["gStratosphere"].SetValue(true);
                            visualEffects.Update_Rotate('z', 0.2f);
                        }

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
