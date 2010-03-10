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

        #endregion

        #region Properties

        public int MySceneIndex
        {
            get { return mySceneIndex; }
            set { mySceneIndex = value; }
        }

        #endregion

        #region Constructor

        public Star(string name, Vector3 radius, Vector3 position, double r, MoleculePool pool)
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

        //When the scene graph calls each loadable object's LoadContent(), 
        //they each Content.Load their own models
        public void LoadContent()
        {
            model = content.Load<Model>("Models\\teapot");
            ReadyToRender = true;
        }

        public void UnloadContent()
        {
            //crap
        }
        bool doOnce = true;
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
