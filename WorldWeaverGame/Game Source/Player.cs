#region File Description
//-----------------------------------------------------------------------------
// Player.cs
//
// World Weaver
// Elizabeth Baumel
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
#endregion

namespace WorldWeaver
{
    public class Player : SceneObject, IDrawableObject, ILoadableObject, IUpdatableObject
    {
        #region Declarations

        private int baseCharge, currentCharge;
        private int mySceneIndex;
        private bool playerVisible = false;
        public bool isRotating = false;
        private float timeSinceLastRotate = 0;
        private ContentManager content;
        private Model model;
        private GameTime gameTime;

        public ParticleChain CurrentChain;

        private MoleculePool moleculePool;

        public const float MinimumAltitude = 150.0f;
        public const float MaximumAltitude = 22000.0f;

        public const float MinimumX = -11000.0f;
        public const float MaximumX = 11000.0f;

        public const float MinimumZ = -11000.0f;
        public const float MaximumZ = 11000.0f;

        //public Vector3 Position;

        public Vector3 Direction;

        public Vector3 Up;

        private Vector3 right;

        public Vector3 Right
        {
            get { return right; }
        }

        private ChaseCamera camera;

        // Full speed at which ship can rotate; measured in radians per second.
        private const float RotationRate = 1.5f;

        // Mass of ship.
        private const float Mass = 1.0f;

        // Maximum force that can be applied along the ship's direction.
        private const float ThrustForce = 24000.0f;

        // Velocity scalar to approximate drag.
        private const float DragFactor = 0.97f;

        // Current ship velocity.
        public Vector3 Velocity;

        public int MySceneIndex
        {
            get { return mySceneIndex; }
            set { mySceneIndex = value; }
        }

        public int Charge
        {
            get
            {
                return baseCharge + CurrentChain.sumChainCharge();
            }
        }

        public int getBaseCharge
        {
            get
            {
                return baseCharge;
            }
        }

        // Ship world transform matrix.
        public override Matrix World
        {
            get { return world; }
        }
        private Matrix world;

        public BoundingSphere CollisionSphere
        {
            get { return collisionSphere; }
        }
        private BoundingSphere collisionSphere; //make more later for less crappy collision detection

        public MoleculePool mPool
        {
            get { return moleculePool; }
        }

        //wallace brown 11/14/09
        GraphicsDeviceManager graphics;
        CustomEffects visualEffects = new CustomEffects();
        Texture2D mTexture;
        //end Code[]

        // Add a list of location
        Vector3[] planetList = new Vector3[] {new Vector3(0,0,0)};  // This array must expand with every planet creation.

        #endregion

        #region Constructors

        //Empty constructor
        public Player()
        {
            
        }

        //Constructor for first person game (player model is not drawn)
        public Player(int charge)
        {
            baseCharge = charge;
            currentCharge = baseCharge;
            CurrentChain = new ParticleChain();
            moleculePool = new MoleculePool();

            Position = new Vector3(0, MinimumAltitude, 0);
            Direction = Vector3.Forward;
            Up = Vector3.Up;
            right = Vector3.Right;
            Velocity = Vector3.Zero;

            //collisionSphere's start centre is Position
            collisionSphere = new BoundingSphere(Position, 300.0f);

            

        }

        //Chase target player version of the constructor
        public Player(int charge, ChaseCamera camera, ContentManager content, GraphicsDeviceManager graphics)
        {
            baseCharge = charge;
            currentCharge = baseCharge;
            CurrentChain = new ParticleChain();
            moleculePool = new MoleculePool();

            Position = new Vector3(0, MinimumAltitude, 0);
            Direction = Vector3.Forward;
            Up = Vector3.Up;
            right = Vector3.Right;
            Velocity = Vector3.Zero;

            //collisionSphere's start centre is Position
            collisionSphere = new BoundingSphere(Position, 300.0f);
            this.camera = camera;
            this.content = content;//this.model = model;
            playerVisible = true;

            //wallace brown 11/15/09
            this.graphics = graphics;
            //end Code[]

        }

        #endregion

        #region Game Logic Methods

        //calculates overall charge of currentChain + player's baseCharge
        public int calculateCurrentCharge()
        {
            currentCharge = baseCharge + CurrentChain.sumChainCharge();

            return currentCharge;
        }

        //Adds chain to Molecule Pool and clears the current chain
        public void dumpChain()
        {
            if (calculateCurrentCharge() == 0)
            {
                //put all the particles in currentChain into moleculePool
                moleculePool.appendChainToPool(CurrentChain);

                //clear currentChain
                CurrentChain.clearChain();
                //Console.WriteLine("chain dumped\n");
                //reset overall charge
                currentCharge = baseCharge;
            }
        }

        // Expands the planet array
        private void expandPlanetList()
        {
            // Create new array that is +1 original array's length
            Vector3[] temp = new Vector3[planetList.Length + 1];

            // Loop through temp array and copy data from original
            for (int i = 0; i < planetList.Length; i++)
            {
                temp[i] = planetList[i];                
            }

            // Create new planetList, will be recovered from temp
            planetList = new Vector3[temp.Length];

            // Re-loop through array and re copy data
            for (int i = 0; i < planetList.Length; i++)
            {
                planetList[i] = temp[i];
            }
        }

        // Adds planet locations to the location array
        public void addPlanetLocation(Vector3 location)
        {
            // Expand the planetlist
            expandPlanetList();

            // Add planet to last index
            planetList[planetList.Length - 1] = location;

        }


        #endregion

        #region Player and Chase camera update method

        public void GetPlayerRequirements(GameTime gameTime)
        {
            this.gameTime = gameTime;
        }

        private float thrustAmount;
        private const float BRAKE_CONST = .5f;  //1=instant stop, 0=no effect
        private bool quickTurning = false;

        //180 turn touch up
        private bool hasTurnedLeft = false;

        private short turnLeft = 0;
        private const short TURN_180_TURNS = 16;
        private const float piInc = (float)(Math.PI) / 512f;
        private  Vector2[] ROT_ARRAY = {
                                             new Vector2(piInc,0f),
                                             new Vector2(2f*piInc,0f),
                                             new Vector2(4f*piInc,0f),
                                             new Vector2(8f*piInc,0f),
                                             new Vector2(16f*piInc,0f),
                                             new Vector2(32f*piInc,0f),
                                             new Vector2(64f*piInc,0f),
                                             new Vector2(129f*piInc,0f),
                                             new Vector2(129f*piInc,0f),
                                             new Vector2(64f*piInc,0f),
                                             new Vector2(32f*piInc,0f),
                                             new Vector2(16f*piInc,0f),
                                             new Vector2(8f*piInc,0f),
                                             new Vector2(4f*piInc,0f),
                                             new Vector2(2f*piInc,0f),
                                             new Vector2(piInc,0f),   
                                            };



        // Applies a simple rotation to the ship and animates position based
        // on simple linear motion physics.
        // From Chase Camera example from XNA site
        public void Update()
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds; //stays

            //KeyboardState keyboardState = Keyboard.GetState(); //stays
            //GamePadState gamePadState = GamePad.GetState(PlayerIndex.One); //stays

            if (quickTurning)
            {
                //DO TURN THING
                timeSinceLastRotate += elapsed;
                float timeToRotate = 0.17f / ROT_ARRAY.Length;

                while (timeSinceLastRotate > timeToRotate)
                {
                    timeSinceLastRotate -= timeToRotate;

                    Vector2 rotationAmount = ROT_ARRAY[turnLeft - 1];

                    if (!hasTurnedLeft)
                    {
                        rotationAmount = (ROT_ARRAY[turnLeft - 1]) * -1;
                    }

                    // Correct the X axis steering when the ship is upside down
                    if (Up.Y < 0)
                    {
                        rotationAmount.X = -rotationAmount.X;
                    }

                    //float[] rotArray = generateRot180Array();
                    //pi/2 in the first and second halves of the array; from there, 1/4 of what's left goes in the first
                    //half, 3/4 in the second half

                    turnLeft--;
                    if (turnLeft <= 0)
                    {
                        quickTurning = false;
                        timeSinceLastRotate = 0;
                    }

                    // Create rotation matrix from rotation amount
                    Matrix rotationMatrix =
                        Matrix.CreateFromAxisAngle(Right, rotationAmount.Y) *
                        Matrix.CreateRotationY(rotationAmount.X);

                    // Rotate orientation vectors
                    Direction = Vector3.TransformNormal(Direction, rotationMatrix);
                    Up = Vector3.TransformNormal(Up, rotationMatrix);

                    // Re-normalize orientation vectors
                    // Without this, the matrix transformations may introduce small rounding
                    // errors which add up over time and could destabilize the ship.
                    Direction.Normalize();
                    Up.Normalize();

                    // Re-calculate Right
                    right = Vector3.Cross(Direction, Up);

                    // The same instability may cause the 3 orientation vectors may
                    // also diverge. Either the Up or Direction vector needs to be
                    // re-computed with a cross product to ensure orthagonality
                    Up = Vector3.Cross(Right, Direction);
                }
            }
            else
            {
                getUpdateUserInput(elapsed);
            }

            // Calculate force from thrust amount
            Vector3 force = Direction * thrustAmount * ThrustForce;


            // Apply acceleration
            Vector3 acceleration = force / Mass;
            Velocity += acceleration * elapsed;

            // Apply psuedo drag
            Velocity *= DragFactor;

            // Apply velocity
            Position += Velocity * elapsed;


            Vector3 temp = Position;
            
            //SHITTY SOLUTION IS SHITTY
            //>LOL REDUNDANCY IS REDUNDANT

            // Prevent player from flying under the ground
            temp.Y = Math.Max(Position.Y, MinimumAltitude);
            Position = new Vector3(temp.X, temp.Y, temp.Z);

            temp.Y = Math.Min(Position.Y, MaximumAltitude);
            Position = new Vector3(temp.X, temp.Y, temp.Z);

            // Prevent player from flying out to nowhere in the x direction
            temp.X = Math.Max(Position.X, MinimumX);
            Position = new Vector3(temp.X, temp.Y, temp.Z);

            temp.X = Math.Min(Position.X, MaximumX);
            Position = new Vector3(temp.X, temp.Y, temp.Z);

            // Prevent player from flying out to nowhere in the z direction
            temp.Z = Math.Max(Position.Z, MinimumZ);
            Position = new Vector3(temp.X, temp.Y, temp.Z);

            temp.Z = Math.Min(Position.Z, MaximumZ);
            Position = new Vector3(temp.X, temp.Y, temp.Z);

            // Reconstruct the ship's world matrix
            world = Matrix.Identity;
            world.Forward = Direction;
            world.Up = Up;
            world.Right = right;
            world.Translation = Position;
            collisionSphere.Center = Position;

            //see if the player's charge has been neutralised and if so, dump chain into molecule pool
            //Sorry for commenting this out - didn't see a check to see if player's charge was neutral and it was dumping the particles alot
            dumpChain();
        }

        private void getUpdateUserInput(float elapsed)
        {
            KeyboardState keyboardState = Keyboard.GetState(); //stays
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One); //stays

            // Determine rotation amount from input
            Vector2 rotationAmount = -gamePadState.ThumbSticks.Left;
            if (keyboardState.IsKeyDown(Keys.Left))
            {
                rotationAmount.X = 1.0f;
                isRotating = true;
                hasTurnedLeft = true;
            }

            if (keyboardState.IsKeyDown(Keys.Right))
            {
                rotationAmount.X = -1.0f;
                isRotating = true;
                hasTurnedLeft = false;
            }

            if (keyboardState.IsKeyDown(Keys.Up))
            {
                rotationAmount.Y = -1.0f;
                isRotating = true;
            }

            if (keyboardState.IsKeyDown(Keys.Down))
            {
                rotationAmount.Y = 1.0f;
                isRotating = true;
            }

            if (rotationAmount.X > 0)
            {
                hasTurnedLeft = true;
            }
            else if(rotationAmount.X < 0)
            {
                hasTurnedLeft = false;
            }

           
            //THIS IS KEVIN'S ATTEMPT AND IMPLEMENTING THE 180 TURN
            if (keyboardState.IsKeyDown(Keys.F))
            {

                //rotationAmount.Y = (float)Math.PI;//180.0f * rotationAmount * RotationRate * elapsed;
                //rotationAmount.X = 180.0f * elapsed * 25;//180.0f;
                quickTurning = true;
                turnLeft = TURN_180_TURNS;
                timeSinceLastRotate = 0;
                //Matrix.CreateRotationX(rotationAmount.X);
                // Vector3.Lerp(Direction, Position, 180.0f);
                // Vector3.Transform(Position, Matrix.CreateRotationX(rotationAmount.X), Vector3.Backward);
                // Quaternion matrixdealie = Quaternion.CreateFromRotationMatrix(World);
                // Matrix.CreateRotationX(matrixdealie);
                //Matrix rotationMatrix



                //Position.X = Direction.Y + (float)Math.PI;
                //Up.Normalize();

                //Matrix.CreateRotationY((float)Math.PI);
            }

            if (gamePadState.IsButtonDown(Buttons.Y))
            {
                quickTurning = true;
                timeSinceLastRotate = 0;
                turnLeft = TURN_180_TURNS;
            }

            //should bring up the pop up menu...

            if (gamePadState.IsButtonDown(Buttons.RightShoulder))
            {
                
            }

            // Scale rotation amount to radians per second
            rotationAmount = rotationAmount * RotationRate * elapsed;

            // Correct the X axis steering when the ship is upside down
            if (Up.Y < 0)
            {
                rotationAmount.X = -rotationAmount.X;
            }


            // Create rotation matrix from rotation amount
            Matrix rotationMatrix =
                Matrix.CreateFromAxisAngle(Right, rotationAmount.Y) *
                Matrix.CreateRotationY(rotationAmount.X);

            // Rotate orientation vectors
            Direction = Vector3.TransformNormal(Direction, rotationMatrix);
            Up = Vector3.TransformNormal(Up, rotationMatrix);

            // Re-normalize orientation vectors
            // Without this, the matrix transformations may introduce small rounding
            // errors which add up over time and could destabilize the ship.
            Direction.Normalize();
            Up.Normalize();

            // Re-calculate Right
            right = Vector3.Cross(Direction, Up);

            // The same instability may cause the 3 orientation vectors may
            // also diverge. Either the Up or Direction vector needs to be
            // re-computed with a cross product to ensure orthagonality
            Up = Vector3.Cross(Right, Direction);


            // Determine thrust amount from input
            thrustAmount = gamePadState.Triggers.Right;

            //cross 2 vectors to get orthogonal third, use the right hand rule
            Vector3 Forward = Vector3.Cross(Up,Right);
            //<math>This is the projection of the velocity vector onto the forward vector.</math>
            //<english> This is how fast we're going forwards.</english>
            float forwardSpeed = Vector3.Dot(Forward, Velocity) / Forward.LengthSquared(); 

             if (keyboardState.IsKeyDown(Keys.Space))
            {
                thrustAmount = 1.0f;
            }
            //brake code - brakes = negative acceleration by definition
             if (keyboardState.IsKeyDown(Keys.LeftShift) && forwardSpeed > 0f || (gamePadState.IsButtonDown(Buttons.LeftTrigger) && forwardSpeed > 0f))
            {
                thrustAmount -= BRAKE_CONST;    //allows for slowed movement when holding brake and gas at same time
                //thrustAmount = -1.0f;
            }
            if (keyboardState.IsKeyDown(Keys.LeftShift) || (gamePadState.IsButtonDown(Buttons.LeftTrigger)))
            {
                // thrustAmount -= 0.001f; //maybe, set it a .001 for the sake of being able to move
                thrustAmount -= BRAKE_CONST;
            }


            // Focus Player, locks onto nearest planet
            if (keyboardState.IsKeyDown(Keys.Q) || gamePadState.IsButtonDown(Buttons.LeftShoulder))
            {
                // Now use arrow keys to lock on to a planet in the planetList.
                // + = i++
                // - = i--
                // Create cycle instance variable
                int i = 0;  // Bottom of planetList

                if (keyboardState.IsKeyDown(Keys.Add) || gamePadState.IsButtonDown(Buttons.DPadRight))
                {
                    if (i < planetList.Length - 1)
                    {
                        i++;
                        this.Direction = planetList[i++];
                    }
                    else
                    {
                        i = 0;
                        this.Direction = planetList[i];
                    }
                }
                else if (keyboardState.IsKeyDown(Keys.Subtract) || gamePadState.IsButtonDown(Buttons.DPadLeft))
                {
                    if (i > 0)
                    {
                        i--;
                        this.Direction = planetList[i];
                    }
                    else
                    {
                        i = planetList.Length - 1;
                        this.Direction = planetList[i];
                    }
                }
                Console.WriteLine(i);

            }


        }

        #endregion

        #region Content Loaders and Drawing
        //When the scene graph calls each loadable object's LoadContent(), 
        //they each Content.Load their own models
        public void LoadContent()
        {
            visualEffects = new CustomEffects();
           // model = content.Load<Model>(Globals.AssetList.playerModelPath); //might need to uncomment soon
            

            //different model, different player
            // It works!

            switch(Globals.godcharge)
            {

                case -1:

                    model = content.Load<Model>(Globals.AssetList.playerModelPath);
                    break;

                case 3:
                    model = content.Load<Model>("Models//teapot");
                    break;

                case -2:
                    model = content.Load<Model>("Models//teapot");
                    break;

                case -3:
                    model = content.Load<Model>("Models//teapot");
                    break;
                case 2:
                    model = content.Load<Model>("Models//teapot");
                    break;

                case -4:
                    model = content.Load<Model>("Models//teapot");
                    break;

                case 1:
                    model = content.Load<Model>("Models//teapot");
                    break;
                    
                
            }

            visualEffects.Phong = content.Load<Effect>(Globals.AssetList.PhongFXPath);
            mTexture = content.Load<Texture2D>("Images\\whattrex");
            Effect customPhong = content.Load<Effect>(Globals.AssetList.PhongFXPath);
            CustomEffects.ChangeEffectUsedByModel(model, customPhong);
            ReadyToRender = true;
        }

        public void UnloadContent()
        {
            //crap
        }

        public new void Draw(GameTime gameTime)
        {
            //wallace brown 11/14/09
            visualEffects.Init_Phong();
            //code End[]
            if (!Globals.gameplayScreenDestroyed)
            {
                if (playerVisible)
                {
                    Matrix[] transforms = new Matrix[model.Bones.Count];
                    model.CopyAbsoluteBoneTransformsTo(transforms);

                    foreach (ModelMesh mesh in model.Meshes)
                    {
                        //wallace brown
                        visualEffects.Set_Phong_Diffuse(new Vector3(1.0f, 1.0f, 1.0f), visualEffects.color_white);
                        visualEffects.Set_Phong_Ambient(visualEffects.color_white, new Vector4(0.1f, 0.1f, 0.1f, 1.0f));
                        visualEffects.Set_Phong_Specular(new Vector4(0.8f, 0.8f, 0.8f, 1.0f), visualEffects.color_white, 20.0f);
                        visualEffects.Set_Specials_Phong(false, false, false, false);

                        DrawModel_Phong(model, transforms, world, "Main_MultipleLights");
                        //code End[]
                        BoundingSphereRenderer.Render(collisionSphere, Globals.sceneGraphManager.GraphicsDevice, Globals.ChaseCamera.View, Globals.ChaseCamera.Projection, Globals.DEBUG);
                    }
                }
            }
        }

        private void DrawModel_Phong(Model model, Matrix[] transform, Matrix world, string technique)
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

                    //effect.Parameters["gLightVecW"].SetValue(new Vector3(0.0f, -1.0f, 0.0f));
                    //effect.Parameters["gDiffuseMtrl"].SetValue(new Vector4(1.0f));
                    //effect.Parameters["gDiffuseLight"].SetValue(Color.White.ToVector4());
                    effect.Parameters["gAmbientMtrl"].SetValue(Color.White.ToVector4());
                    effect.Parameters["gAmbientLight"].SetValue(new Vector4(0.1f));
                    //effect.Parameters["gSpecularMtrl"].SetValue(new Vector4(0.8f, 0.8f, 0.8f, 1.0f));
                    //effect.Parameters["gSpecularLight"].SetValue(Color.White.ToVector4());
                    effect.Parameters["gSpecularPower"].SetValue(20.0f);

                    effect.CommitChanges();
                }

                mesh.Draw();
            }

        }

        private void DrawModel_Phong_Special(Model model, Matrix[] transform, Matrix world, string technique, GameTime time)
        {
            this.camera = Globals.ChaseCamera;

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
                        visualEffects.Update_Phong(transform[mesh.ParentBone.Index] * world, camera.View, camera.Projection, camera.Position);
                        //visualEffects.Phong.Parameters["gTex0"].SetValue(mTexture);

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
    }
}
