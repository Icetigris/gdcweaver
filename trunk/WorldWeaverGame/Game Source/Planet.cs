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

namespace WorldWeaver
{
    class Planet : CelestialBody, IDrawableObject, ILoadableObject, IUpdatableObject
    {
        //has an atmosphere
        //core composition (does it have a magnetosphere?)


        #region Declarations

        private ContentManager content;
        private int mySceneIndex;
        private Model model;
        private Matrix world;
        private ChaseCamera camera;

        #endregion

        #region Constructor

        public Planet(string name, Vector3 radius, Vector3 position, double r, MoleculePool pool)
        {
            this.Name = name;
            this.Radius = radius;
            this.Position = position;
            this.R = r;
            this.mPool = pool;

            // More constructor data, prevents null variables
            this.content = Globals.contentManager;
            this.camera = Globals.ChaseCamera;
        }

        #endregion

        #region Methods

        //if mass of the planet is at least 30% due to silver particles, you get a magnetic field
        public void hasMagneticField()
        {
            /// Get total quantity of particles so we can do the math
            //int numParticles = mPool.Particles;

            foreach (Particle p in mPool.Particles)
            {

            }
        }

        //calculate mass of planet from sum of particle masses
        public override void calculateMass()
        {
            foreach (Particle p in mPool.Particles)
            {
                Mass += p.AssignMass((int)p.Colour);
            }
        }

        //calculate gravity points for the planet (for moons and stuff)
        public override void calculateGravityPoints()
        {
            GravityPoints = (int)R * 2;
        }

        #endregion

        #region Draw Methods

        public void Update()
        {
        }

        //When the scene graph calls each loadable object's LoadContent(), 
        //they each Content.Load their own models
        public void LoadContent()
        {
            model = content.Load<Model>("Models\\donutplanet");
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
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.PreferPerPixelLighting = true;

                        effect.World = transforms[mesh.ParentBone.Index] * world;

                        // Use the matrices provided by the chase camera
                        effect.View = camera.View;
                        effect.Projection = camera.Projection;
                    }

                    mesh.Draw();
                }
            }
        }

        #endregion



    }
}
