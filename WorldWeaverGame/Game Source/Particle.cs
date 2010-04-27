using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace WorldWeaver
{
    //These are the atom things that you pick up
    public class Particle : SceneObject, IDrawableObject, ILoadableObject, IUpdatableObject
    {
        public enum Colours
        {
            Red,
            Orange,
            Yellow,
            Green,
            Blue,
            Purple,
            Silver,
            Shiny
        }

        #region Declarations

        private ContentManager content;
        private Model model;
        private Cue collectSound;

        private ChaseCamera chaseCamera;
        private HudCamera hudCamera;
        private Player player;
        private Matrix world;
        private Vector3 targetColour;

        private Random randomNumberGenerator;

        // wallace brown
        private CustomEffects visualEffects = new CustomEffects();
        private Texture2D mTexture;
        GraphicsDeviceManager graphics;
        private Vector3 glowColor;
        private ParticleEffect_Emmiter orbital;
        private ChaseCamera camera;

        ///Models loaded once only for flexible use
        private static bool models_loaded = false;
        private static Model plus_1;
        private static Model plus_2;
        private static Model plus_3;
        private static Model plus_4;
        private static Model minus_1;
        private static Model minus_2;
        private static Model minus_3;
        private static Model minus_4;

        // end code

        Matrix orientation,
             translation,
             finalTransformation;

        private int mySceneIndex;
        private bool isCollected = false;
        private int charge;
        private Colours colour;

        //Subject to change
        private float collectionRadius = 0.0f;
        private float attractionRadius = 11000.0f;

        private BoundingSphere collectionSphere;
        private BoundingSphere attractionSphere;

        public const float e = 0.0000000000000000001602176487f; //elementary charge
        public const float MinimumAltitude = 150.0f;
        public const float MaximumAltitude = 22000.0f;

        public const float MinimumX = -11000.0f;
        public const float MaximumX = 11000.0f;

        public const float MinimumZ = -11000.0f;
        public const float MaximumZ = 11000.0f;
        
        //private Vector3 position;
        public Vector3 Direction;

        // Max particle velocity.
        public float maxVelocity = 0.01f;

        #endregion

        #region Properties

        public int MySceneIndex
        {
            get { return mySceneIndex; }
            set { mySceneIndex = value; }
        }

        public bool IsCollected
        {
            get { return isCollected; }
            set { isCollected = value; }
        }

        //This particle's charge, getter, and setter
        public int Charge
        {
            get { return charge; }
        }

        //This particle's colour, getter, and setter
        public Colours Colour
        {
            get { return colour; }
        }

        //This particle's position; setter calculates new transformation matrix
        /*
        public Vector3 Position
        {
            get { return position; }
            set
            {
                position = value;
                translation = Matrix.CreateTranslation(position);
                finalTransformation = orientation * translation;

                collectionSphere.Center = position;
                attractionSphere.Center = position;
            }
        }
        */
        public float CollectionRadius
        {
            get { return collectionRadius; }
        }

        public float AttractionRadius
        {
            get { return attractionRadius; }
        }

        public BoundingSphere CollectionSphere
        {
            get { return collectionSphere; }
        }

        public BoundingSphere AttractionSphere
        {
            get { return attractionSphere; }
        }

        #endregion

        #region Constructors

        //empty constructor
        public Particle(){}

        public Particle(Random rng, int sceneIndex, ContentManager content)//(int ch, Colours col, Vector3 pos, ContentManager content)
        {
            randomNumberGenerator = rng;
            mySceneIndex = sceneIndex;
            charge = generateRandomCharge();
            colour = generateRandomColour();
            Position = generateRandomPosition();
            Direction = Vector3.Forward;
            orientation = Matrix.Identity;
            finalTransformation = orientation * translation;
            collectionSphere = new BoundingSphere(Position, collectionRadius);
            attractionSphere = new BoundingSphere(Position, attractionRadius);
            this.content = content;
            //wb
            orbital = new ParticleEffect_Emmiter(ParticleEffect_Behavior.orbit, 25.0f,
                this.Position, 10);
            //end
        }

        public Particle(Random rng, int sceneIndex, ContentManager content, GraphicsDeviceManager graphics)
        {
            randomNumberGenerator = rng;
            mySceneIndex = sceneIndex;
            charge = generateRandomCharge();
            colour = generateRandomColour();
            Position = generateRandomPosition();
            Direction = Vector3.Forward;
            orientation = Matrix.Identity;
            finalTransformation = orientation * translation;
            collectionSphere = new BoundingSphere(Position, collectionRadius);
            attractionSphere = new BoundingSphere(Position, attractionRadius);
            this.content = content;
            this.graphics = graphics;
        }  

        #endregion

        #region Public Methods

        public Vector3 AssignColor(int particleEnum)
        {
            Color outC = Color.White;
            switch (particleEnum)
            {
                case 0:
                    outC = Color.Red;
                    break;
                case 1:
                    outC = Color.DarkOrange;
                    break;
                case 2:
                    outC = Color.Yellow;
                    break;
                case 3:
                    outC = Color.Green;
                    break;
                case 4:
                    outC = Color.Blue;
                    break;
                case 5:
                    outC = Color.MediumPurple;
                    break;
                case 6:
                    outC = Color.Gray;
                    break;
                case 7:
                    outC = Color.White;
                    break;
            }
            return outC.ToVector3();
        }

        public static Vector3 convertColor(int particleEnum)
        {
            Color outC = Color.White;
            switch (particleEnum)
            {
                case 0:
                    outC = Color.Red;
                    break;
                case 1:
                    outC = Color.DarkOrange;
                    break;
                case 2:
                    outC = Color.Yellow;
                    break;
                case 3:
                    outC = Color.Green;
                    break;
                case 4:
                    outC = Color.Blue;
                    break;
                case 5:
                    outC = Color.MediumPurple;
                    break;
                case 6:
                    outC = Color.Gray;
                    break;
                case 7:
                    outC = Color.White;
                    break;
            }
            return outC.ToVector3();
        }

        public int AssignMass(int particleEnum)
        {
            int outMass = 0; //grams?
            switch (particleEnum)
            {
                case 0:
                    outMass = 1;
                    break;
                case 1:
                    outMass = 2;
                    break;
                case 2:
                    outMass = 8;
                    break;
                case 3:
                    outMass = 16;
                    break;
                case 4:
                    outMass = 32;
                    break;
                case 5:
                    outMass = 64;
                    break;
                case 6:
                    outMass = 128;
                    break;
                case 7:
                    outMass = 0;
                    break;
            }
            return outMass;
        }

        public float CalculateVelocity(int playerCharge, Vector3 direction)
        {
            float newVelocity;
            //F = k_e(q_1*q_2/r*r) = Coulomb's Law; k_e is totally inaccurate (it's 9E9 IRL)
            newVelocity = -900 *((playerCharge * charge) / (direction.Length() * direction.Length()));

            // Joey Breeden Dec 4, 2009 <-- I fiddled with the above equation to lessen the numbers and lower
            // the particle speed, as was specified in the task list.  Original number was -1800

            return newVelocity;
        }

        public void Move(Vector3 playerPosition, int playerCharge, BoundingSphere playerCollisionSphere)
        {
            if (AttractionSphere.Intersects(playerCollisionSphere))
            {
                Vector3 direction = (playerPosition - Position);
                Vector3 vecToTarget = Vector3.Normalize(playerPosition - Position);
                Vector3 axisToRotate = Vector3.Cross(direction, vecToTarget);

                double angle = Math.Acos((double)Vector3.Dot(direction, vecToTarget));

                // Avoid divide by zero. This case would occur if the cross product
                // of the vectors equal 0.
                if (axisToRotate == Vector3.Zero)
                {
                    axisToRotate = Vector3.UnitY;
                }
                else
                {
                    axisToRotate.Normalize();
                }

                //turn the particles to face the player
                orientation = Matrix.CreateFromAxisAngle(axisToRotate, (float)angle);
                finalTransformation = orientation * translation;

                //move the particles towards the player
                Vector3 newPosition = Position + (direction * CalculateVelocity(playerCharge, direction)); //change to support velocity changes
                Position = new Vector3(newPosition.X, newPosition.Y, newPosition.Z);
                translation = Matrix.CreateTranslation(Position);
                finalTransformation = orientation * translation;
                /*
                // Prevent particle from flying under the ground
                position.Y = Math.Max(Position.Y, MinimumAltitude);
                position.Y = Math.Min(Position.Y, MaximumAltitude);

                // Prevent particle from flying out to nowhere in the x direction
                position.X = Math.Max(Position.X, MinimumX);
                position.X = Math.Min(Position.X, MaximumX);

                // Prevent particle from flying out to nowhere in the z direction
                position.Z = Math.Max(Position.Z, MinimumZ);
                position.Z = Math.Min(Position.Z, MaximumZ);
                */

                Vector3 temp = Position;

                //SHITTY SOLUTION IS SHITTY
                // Prevent particles from flying under the ground
                if (!isCollected)
                {
                    temp.Y = Math.Max(Position.Y, MinimumAltitude);
                    Position = new Vector3(temp.X, temp.Y, temp.Z);

                    temp.Y = Math.Min(Position.Y, MaximumAltitude);
                    Position = new Vector3(temp.X, temp.Y, temp.Z);

                    // Prevent particles from flying out to nowhere in the x direction
                    temp.X = Math.Max(Position.X, MinimumX);
                    Position = new Vector3(temp.X, temp.Y, temp.Z);

                    temp.X = Math.Min(Position.X, MaximumX);
                    Position = new Vector3(temp.X, temp.Y, temp.Z);

                    // Prevent particles from flying out to nowhere in the z direction
                    temp.Z = Math.Max(Position.Z, MinimumZ);
                    Position = new Vector3(temp.X, temp.Y, temp.Z);

                    temp.Z = Math.Min(Position.Z, MaximumZ);
                    Position = new Vector3(temp.X, temp.Y, temp.Z);
                }

                //move the centres of the collection and attraction radii with the particle model
                collectionSphere.Center = Position;
                attractionSphere.Center = Position;
            }
        }

        public override string ToString()
        {
            string theParticle = "";

            theParticle += "Charge: " + Charge + ", Colour: " + Colour + ", Position: " + Position + "\n";

            return theParticle;
        }

        #endregion

        #region Private Methods

        //Gives this particle a random charge upon creation
        private int generateRandomCharge()
        {
            int randomIndex = randomNumberGenerator.Next(0, 7);//randCharge.Next(0, 7);
            int aCharge = 0;

            switch (randomIndex)
            {
                case 0:
                    aCharge = 1;
                    break;
                case 1:
                    aCharge = 2;
                    break;
                case 2:
                    aCharge = 3;
                    break;
                case 3:
                    aCharge = 4;
                    break;
                case 4:
                    aCharge = -1;
                    break;
                case 5:
                    aCharge = -2;
                    break;
                case 6:
                    aCharge = -3;
                    break;
                case 7:
                    aCharge = -4;
                    break;
            }

            return aCharge;
        }

        //Gives this particle a random colour upon creation
        private Particle.Colours generateRandomColour()
        {
            Particle.Colours aColour = Particle.Colours.Silver;
            int randomIndex = randomNumberGenerator.Next(0, 6);//randColour.Next(0, 6);

            switch (randomIndex)
            {
                case 0:
                    aColour = Particle.Colours.Red;
                    break;
                case 1:
                    aColour = Particle.Colours.Orange;
                    break;
                case 2:
                    aColour = Particle.Colours.Yellow;
                    break;
                case 3:
                    aColour = Particle.Colours.Green;
                    break;
                case 4:
                    aColour = Particle.Colours.Blue;
                    break;
                case 5:
                    aColour = Particle.Colours.Purple;
                    break;
                case 6:
                    aColour = Particle.Colours.Silver;
                    break;
            }

            return aColour;
        }

        //Gives this particle a random starting position upon creation
        private Vector3 generateRandomPosition()
        {
            double randDoubleX = randomNumberGenerator.NextDouble();//randPosition.NextDouble();
            double randDoubleY = randomNumberGenerator.NextDouble();//randPosition.NextDouble();
            double randDoubleZ = randomNumberGenerator.NextDouble();//randPosition.NextDouble();

            float randX = (float)((2 * Particle.MaximumX * randDoubleX) + Particle.MinimumX);
            float randY = (float)(Math.Abs(Particle.MaximumAltitude + Particle.MinimumAltitude) * randDoubleY - Particle.MinimumAltitude);
            float randZ = (float)((2 * Particle.MaximumZ * randDoubleZ) + Particle.MinimumZ);

            return new Vector3(randX, randY, randZ);
        }

        #endregion

        #region Inherited Methods

        //particles need the player's position, charge, collision sphere so that they can calculate
        //their attraction physics and whether or not they are collected
        public void Update()
        {
            Move(player.Position, player.Charge, player.CollisionSphere);
            if (Globals.Player.CollisionSphere.Intersects(collectionSphere) && !isCollected)
            {
                isCollected = true;

                Globals.Player.CurrentChain.addParticle(this);
                //play sound
                collectSound.Play();
                SceneGraphManager.RemoveObject(mySceneIndex);
                //Console.WriteLine("Cool collect bro");

                // Create new PopUpChargeData object
                //PopUpChargeData particleData = new PopUpChargeData();
                //Globals.hudManager.AddElement(new PopUpChargeData(this.Charge));
                PopUpChargeData.setMessage(this.Charge);
                
            }
        }

        #region Loading
        public void LoadContent()
        {
            if (!Particle.models_loaded)
            {
                LoadModels();
                Particle.models_loaded = true;

            }
            model = LoadParticleFromCharge(charge);
            model = CloneModelEffect(model, charge);

            #region old code
            /*
            switch (charge)
            {
                case 1:
                    model = content.Load<Model>(Globals.AssetList.particlePlus1ModelPath);
                    break;
                case 2:
                    model = content.Load<Model>(Globals.AssetList.particlePlus2ModelPath);
                    break;
                case 3:
                    model = content.Load<Model>(Globals.AssetList.particlePlus3ModelPath);
                    break;
                case 4:
                    model = content.Load<Model>(Globals.AssetList.particlePlus4ModelPath);
                    break;
                case -1:
                    model = content.Load<Model>(Globals.AssetList.particleMinus1ModelPath);
                    break;
                case -2:
                    model = content.Load<Model>(Globals.AssetList.particleMinus2ModelPath);
                    break;
                case -3:
                    model = content.Load<Model>(Globals.AssetList.particleMinus3ModelPath);
                    break;
                case -4:
                    model = content.Load<Model>(Globals.AssetList.particleMinus4ModelPath);
                    break;
            }
            */
            #endregion
           
            //wb
            visualEffects.Phong = content.Load<Effect>(Globals.AssetList.PhongFXPath);
            //mTexture = content.Load<Texture2D>("Models\\Checker");
            // end Code
            ReadyToRender = true;
            collectSound = Globals.musicSoundBank.GetCue(Globals.AssetList.particleCollectSFXCueName);
        }

        public void LoadModels()
        {
            ///Currently the particle models are loaded multiple times. After research
            ///I've found that it is more efficient and flexible to load the models once
            ///and clone them unpon reuse.

            Effect customPhong = content.Load<Effect>(Globals.AssetList.PhongFXPath);

            Particle.plus_1 = content.Load<Model>(Globals.AssetList.particlePlus1ModelPath);
            CustomEffects.ChangeEffectUsedByModel(Particle.plus_1, customPhong);
            Particle.plus_2 = content.Load<Model>(Globals.AssetList.particlePlus2ModelPath);
            CustomEffects.ChangeEffectUsedByModel(Particle.plus_2, customPhong);
            Particle.plus_3 = content.Load<Model>(Globals.AssetList.particlePlus3ModelPath);
            CustomEffects.ChangeEffectUsedByModel(Particle.plus_3, customPhong);
            Particle.plus_4 = content.Load<Model>(Globals.AssetList.particlePlus4ModelPath);
            CustomEffects.ChangeEffectUsedByModel(Particle.plus_4, customPhong);
            Particle.minus_1 = content.Load<Model>(Globals.AssetList.particleMinus1ModelPath);
            CustomEffects.ChangeEffectUsedByModel(Particle.minus_1, customPhong);
            Particle.minus_2 = content.Load<Model>(Globals.AssetList.particleMinus2ModelPath);
            CustomEffects.ChangeEffectUsedByModel(Particle.minus_2, customPhong);
            Particle.minus_3 = content.Load<Model>(Globals.AssetList.particleMinus3ModelPath);
            CustomEffects.ChangeEffectUsedByModel(Particle.minus_3, customPhong);
            Particle.minus_4 = content.Load<Model>(Globals.AssetList.particleMinus4ModelPath);
            CustomEffects.ChangeEffectUsedByModel(Particle.minus_4, customPhong);
        }
       
        public Model LoadParticleFromCharge(int charge)
        {
            Model model;
            switch (charge)
            {
                case 1:
                    model = content.Load<Model>(Globals.AssetList.particlePlus1ModelPath);
                    break;
                case 2:
                    model = content.Load<Model>(Globals.AssetList.particlePlus2ModelPath);
                    break;
                case 3:
                    model = content.Load<Model>(Globals.AssetList.particlePlus3ModelPath);
                    break;
                case 4:
                    model = content.Load<Model>(Globals.AssetList.particlePlus4ModelPath);
                    break;
                case -1:
                    model = content.Load<Model>(Globals.AssetList.particleMinus1ModelPath);
                    break;
                case -2:
                    model = content.Load<Model>(Globals.AssetList.particleMinus2ModelPath);
                    break;
                case -3:
                    model = content.Load<Model>(Globals.AssetList.particleMinus3ModelPath);
                    break;
                case -4:
                    model = content.Load<Model>(Globals.AssetList.particleMinus4ModelPath);
                    break;
                default: 
                    model = content.Load<Model>(Globals.AssetList.particlePlus1ModelPath);
                    break;
            }
            return (model);
        }

        public Model CloneModelEffect(Model newModel, int charge)
        {
            switch (charge)
            {
                case 1:
                    for (int v = 0; v < newModel.Meshes[v].MeshParts.Count - 1; v++)
                    {
                        newModel.Meshes[v].MeshParts[v].Effect = Particle.plus_1.Meshes[v].Effects[0].Clone(graphics.GraphicsDevice);
                    }
                    break;
                case 2:
                    for (int v = 0; v < newModel.Meshes[v].MeshParts.Count; v++)
                    {
                        newModel.Meshes[v].MeshParts[v].Effect = Particle.plus_2.Meshes[v].Effects[0].Clone(graphics.GraphicsDevice);
                    }
                    break;
                case 3:
                    for (int v = 0; v < newModel.Meshes[v].MeshParts.Count; v++)
                    {
                        newModel.Meshes[v].MeshParts[v].Effect = Particle.plus_3.Meshes[v].Effects[0].Clone(graphics.GraphicsDevice);
                    }
                    break;
                case 4:
                    for (int v = 0; v < newModel.Meshes[v].MeshParts.Count; v++)
                    {
                        newModel.Meshes[v].MeshParts[v].Effect = Particle.plus_4.Meshes[v].Effects[0].Clone(graphics.GraphicsDevice);
                    }
                    break;
                case -1:
                    for (int v = 0; v < newModel.Meshes[v].MeshParts.Count; v++)
                    {
                        newModel.Meshes[v].MeshParts[v].Effect = Particle.minus_1.Meshes[v].Effects[0].Clone(graphics.GraphicsDevice);
                    }
                    break;
                case -2:
                    for (int v = 0; v < newModel.Meshes[v].MeshParts.Count; v++)
                    {
                        newModel.Meshes[v].MeshParts[v].Effect = Particle.minus_2.Meshes[v].Effects[0].Clone(graphics.GraphicsDevice);
                    }
                    break;
                case -3:
                    for (int v = 0; v < newModel.Meshes[v].MeshParts.Count; v++)
                    {
                        newModel.Meshes[v].MeshParts[v].Effect = Particle.minus_3.Meshes[v].Effects[0].Clone(graphics.GraphicsDevice);
                    }
                    break;
                case -4:
                    for (int v = 0; v < newModel.Meshes[v].MeshParts.Count; v++)
                    {
                        newModel.Meshes[v].MeshParts[v].Effect = Particle.minus_4.Meshes[v].Effects[0].Clone(graphics.GraphicsDevice);
                    }
                    break;
                default:
                    for (int v = 0; v < newModel.Meshes[v].MeshParts.Count; v++)
                    {
                        newModel.Meshes[v].MeshParts[v].Effect = Particle.plus_1.Meshes[v].Effects[0].Clone(graphics.GraphicsDevice);
                    }
                    break;
            }
            return (newModel);
        }

        #endregion

        public void UnloadContent()
        {
            Particle.models_loaded = false;
        }

        //particles need the view and projection matrices from the current camera in order to draw themselves
        //world matrix is calculated from Position
        public new void Draw(GameTime gameTime)//Model model, Matrix world, Vector3 targetColour, ChaseCamera camera)
        {
            this.player = Globals.Player;

            visualEffects.Init_Phong();
            #region draw
            if (!isCollected)
            {
                //Particle's world matrix
                world = Matrix.CreateTranslation(Position);

                //Make target colour for particle
                targetColour = AssignColor((int)colour);

                Matrix[] transforms = new Matrix[model.Bones.Count];
                model.CopyAbsoluteBoneTransformsTo(transforms);

                //wallace brown 11/12/09[]
                visualEffects.Set_Phong_Diffuse(targetColour, visualEffects.color_white);
                visualEffects.Set_Phong_Ambient(new Vector4(targetColour.X, targetColour.Y, targetColour.Z, 1.0f), new Vector4(0.2f, 0.2f, 0.2f, 1.0f));
                visualEffects.Set_Phong_Specular(new Vector4(0.8f, 0.8f, 0.8f, 1.0f), visualEffects.color_white, 8.0f);
                visualEffects.Set_IsGreymapped(false);

                if (charge == -1)
                {
                    visualEffects.Set_Specials_Phong(false, false, true, true);
                    visualEffects.Update_Time(gameTime);
                    visualEffects.Update_Glow(3.0f, 1.0f);
                    visualEffects.Update_Rotate('y',0.5f);
                    glowColor = new Vector3(0.1f, 0.0f, 0.1f);
                    DrawModel_Phong(model, transforms, world, "Particles_MultipleLights");
                }
                else if(charge < 0 && charge != -1)
                {
                    visualEffects.Set_Specials_Phong(false, false, true, true);
                    visualEffects.Update_Time(gameTime);
                    visualEffects.Update_Glow(3.2f, 0.1f);
                    if (targetColour == Color.Red.ToVector3() ||
                        targetColour == Color.Yellow.ToVector3())
                    {
                        visualEffects.Update_Rotate('x',0.2f);
                    }
                    else if (targetColour == Color.Blue.ToVector3() ||
                        targetColour == Color.Green.ToVector3())
                    {
                        visualEffects.Update_Rotate('x',0.1f);
                    }
                    glowColor = new Vector3(0.1f, 0.0f, 0.1f);
                    DrawModel_Phong(model, transforms, world, "Particles_MultipleLights");
                }
                else if (charge > 0)
                {
                    visualEffects.Set_Specials_Phong(false, false, true, true);
                    visualEffects.Update_Time(gameTime);
                    visualEffects.Update_Glow(3.2f, 0.1f);
                    if (targetColour == Color.Red.ToVector3() ||
                        targetColour == Color.Yellow.ToVector3())
                    {
                        visualEffects.Update_Rotate('x', 0.2f);
                    }
                    else if (targetColour == Color.Blue.ToVector3() ||
                        targetColour == Color.Green.ToVector3())
                    {
                        visualEffects.Update_Rotate('x', 0.1f);
                    }
                    glowColor = new Vector3(1.0f, 1.0f, 1.0f);
                    DrawModel_Phong(model, transforms, world, "Particles_MultipleLights");
                }
                else
                {
                    visualEffects.Set_Specials_Phong(false, false, false, false);
                    DrawModel_Phong(model, transforms, world, "Main");
                }

                BoundingSphereRenderer.Render(collectionSphere, 
                    Globals.sceneGraphManager.GraphicsDevice, 
                    Globals.ChaseCamera.View, 
                    Globals.ChaseCamera.Projection, 
                    Globals.DEBUG, 200);
            }
            #endregion

        }

        #region Draw Methods

        private void DrawModel_Phong(Model model, Matrix[] transform,
            Matrix world, string technique)
        {
            this.camera = Globals.ChaseCamera;

            // Set suitable renderstates for drawing a 3D model.
            RenderState renderState = graphics.GraphicsDevice.RenderState;

            renderState.AlphaBlendEnable = false;
            renderState.AlphaTestEnable = false;
            renderState.DepthBufferEnable = true;

            // Look up the bone transform matrices.
            Matrix[] transforms = new Matrix[model.Bones.Count];

            model.CopyAbsoluteBoneTransformsTo(transforms);

            // Draw the model.
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {
                    // Specify which effect technique to use.
                    effect.CurrentTechnique = effect.Techniques[technique];

                    Matrix localWorld = transforms[mesh.ParentBone.Index] * world;

                    effect.Parameters["gWorld"].SetValue(localWorld);
                    effect.Parameters["gWIT"].SetValue(Matrix.Invert(Matrix.Transpose(localWorld)));
                    effect.Parameters["gWInv"].SetValue(Matrix.Invert(localWorld));
                    effect.Parameters["gWVP"].SetValue(localWorld * camera.View * camera.Projection);
                    effect.Parameters["gEyePosW"].SetValue(camera.Position);




                    effect.Parameters["gNumLights"].SetValue(Globals.numLights);

                    String parameter;
                    for (int v = 0; v < Globals.numLights; v++)
                    {
                        parameter = "gLightPos_multiple_" + (v + 1);
                        effect.Parameters[parameter].SetValue(Globals.lights[v]._position);
                        parameter = "gDiffuseMtrl_multiple_" + (v + 1);
                        effect.Parameters[parameter].SetValue(Globals.lights[v]._diffuse_material);
                        parameter = "gDiffuseLight_multiple_" + (v + 1);
                        effect.Parameters[parameter].SetValue(Globals.lights[v]._diffuse_light);
                        parameter = "gSpecularMtrl_multiple_" + (v + 1);
                        effect.Parameters[parameter].SetValue(Globals.lights[v]._specular_material);
                        parameter = "gSpecularLight_multiple_" + (v + 1);
                        effect.Parameters[parameter].SetValue(Globals.lights[v]._specular_light);
                    }









                    //effect.Parameters["gLightVecW"].SetValue(new Vector3(0.0f, 0.0f, -1.0f));
                    //effect.Parameters["gDiffuseMtrl"].SetValue(new Vector4(targetColour.X, targetColour.Y, targetColour.Z, 1.0f));
                    //effect.Parameters["gDiffuseLight"].SetValue(new Vector4(1.0f));
                    effect.Parameters["gAmbientMtrl"].SetValue(new Vector4(targetColour.X, targetColour.Y, targetColour.Z, 1.0f));
                    effect.Parameters["gAmbientLight"].SetValue(new Vector4(0.4f));
                    //effect.Parameters["gSpecularMtrl"].SetValue(new Vector4(0.8f, 0.8f, 0.8f, 1.0f));
                    //effect.Parameters["gSpecularLight"].SetValue(Color.White.ToVector4());
                    effect.Parameters["gSpecularPower"].SetValue(15.0f);

                    effect.Parameters["gRotate"].SetValue(visualEffects.useRotate);
                    effect.Parameters["gRotSpeed"].SetValue(visualEffects.gRotSpeed);
                    effect.Parameters["gRotAxis"].SetValue(visualEffects.gRotAxis);

                    effect.Parameters["gGlowColor"].SetValue(glowColor);
                    effect.Parameters["gInflation"].SetValue(visualEffects.gInflation);
                    effect.Parameters["gGlowExp"].SetValue(visualEffects.gGlowExp);

                    effect.Parameters["gTime"].SetValue(visualEffects.gTime);

                    effect.CommitChanges();
                }

                mesh.Draw();
            }

            #region oldcode
            //    Globals.sceneGraphManager.GraphicsDevice.VertexDeclaration = new VertexDeclaration(Globals.sceneGraphManager.GraphicsDevice, VertexPositionNormalTextureTangentBinormal.VertexElements);
        //    visualEffects.Phong.CurrentTechnique = visualEffects.Phong.Techniques[technique];

        //    visualEffects.Phong.Begin();

        //    foreach (EffectPass pass in visualEffects.Phong.CurrentTechnique.Passes)
        //    {
        //        // Begin current pass
        //        pass.Begin();
        //        foreach (ModelMesh mesh in model.Meshes)
        //        {
        //            foreach (ModelMeshPart part in mesh.MeshParts)
        //            {
        //                part.Effect = visualEffects.Phong;
        //                visualEffects.Update_Phong(transform[mesh.ParentBone.Index] * world, Globals.ChaseCamera.View, Globals.ChaseCamera.Projection, Globals.ChaseCamera.Position);
        //                visualEffects.Phong.Parameters["gTex0"].SetValue(mTexture);
        //                visualEffects.Phong.Parameters["gGlowColor"].SetValue(glowColor);

        //                graphics.GraphicsDevice.Vertices[0].SetSource(mesh.VertexBuffer, part.StreamOffset, part.VertexStride);
        //                graphics.GraphicsDevice.Indices = mesh.IndexBuffer;
        //                graphics.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList,
        //                                                              part.BaseVertex, 0, part.NumVertices,
        //                                                              part.StartIndex, part.PrimitiveCount);
        //            }
        //        }
        //        pass.End();
        //    }
            //    visualEffects.Phong.End();
            #endregion
        }
 
        #endregion
        
        #endregion
    }
}
