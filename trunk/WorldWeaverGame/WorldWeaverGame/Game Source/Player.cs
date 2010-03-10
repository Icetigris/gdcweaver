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
using Microsoft.Xna.Framework.Input;
#endregion

namespace WorldWeaver
{
    class Player
    {
        #region Declarations

        private int baseCharge, currentCharge;

        public bool isRotating = false;

        public ParticleChain CurrentChain;

        private MoleculePool moleculePool;

        private const float MinimumAltitude = 350.0f;
        private const float MaximumAltitude = 9000.0f;

        public Vector3 Position;

        public Vector3 Direction;

        public Vector3 Up;

        private Vector3 right;

        public Vector3 Right
        {
            get { return right; }
        }

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

        public int Charge
        {
            get
            {
                return baseCharge + CurrentChain.sumChainCharge();
            }
        }

        // Ship world transform matrix.
        public Matrix World
        {
            get { return world; }
        }
        private Matrix world;

        public BoundingSphere CollisionSphere
        {
            get { return collisionSphere; }
        }
        private BoundingSphere collisionSphere; //make more later for less crappy collision detection

        #endregion

        #region Constructors

        //Empty constructor
        public Player()
        {
        }

        //Constructor to which you pass parameters
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

        #endregion

        #region Methods

        //calculates overall charge of currentChain + player's baseCharge
        public int calculateCurrentCharge()
        {
            currentCharge = baseCharge + CurrentChain.sumChainCharge();

            return currentCharge;
        }

        //Adds chain to Molecule Pool and clears the current chain
        public void dumpChain()
        {
            if (currentCharge == 0)
            {
                //put all the particles in currentChain into moleculePool
                moleculePool.appendChainToPool(CurrentChain);

                //clear currentChain
                CurrentChain.clearChain();
            }
        }
        #endregion

        #region Player update method
        // Applies a simple rotation to the ship and animates position based
        // on simple linear motion physics.
        // From Chase Camera example from XNA site

        //TODO: PLEASE FIX THIS, ITSA HORRIBLE PROGRAMMING PRACTICE
        private float thrustAmount;
        private float brakeAmount; //adding brake experimentation
        private bool quickTurning = false;
        private short turnLeft = 0;
        private const short TURN_180_TURNS = 16;
        private const float piInc = (float)(Math.PI)/512f;
        private Vector2[] ROT_ARRAY = {
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


        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds; //stays


            //KeyboardState keyboardState = Keyboard.GetState(); //stays
            //GamePadState gamePadState = GamePad.GetState(PlayerIndex.One); //stays

            if (quickTurning)
            {
                //DO TURN THING
                Vector2 rotationAmount = ROT_ARRAY[turnLeft-1];

                //float[] rotArray = generateRot180Array();
                //pi/2 in the first and second halves of the array; from there, 1/4 of what's left goes in the first
                //half, 3/4 in the second half



                turnLeft--;
                if (turnLeft == 0)
                {
                    quickTurning = false;
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


            // Prevent player from flying under the ground
            Position.Y = Math.Max(Position.Y, MinimumAltitude);
            Position.Y = Math.Min(Position.Y, MaximumAltitude);


            // Reconstruct the ship's world matrix
            world = Matrix.Identity;
            world.Forward = Direction;
            world.Up = Up;
            world.Right = right;
            world.Translation = Position;
            collisionSphere.Center = Position;
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
            }

            if (keyboardState.IsKeyDown(Keys.Right))
            {
                rotationAmount.X = -1.0f;
                isRotating = true;
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

            //THIS IS KEVIN'S ATTEMPT AND IMPLEMENTING THE 180 TURN
            if (keyboardState.IsKeyDown(Keys.F))
            {

                //rotationAmount.Y = (float)Math.PI;//180.0f * rotationAmount * RotationRate * elapsed;
                //rotationAmount.X = 180.0f * elapsed * 25;//180.0f;
                quickTurning = true;
                turnLeft = TURN_180_TURNS;
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

            if (gamePadState.IsButtonDown(Buttons.A))
            {
                quickTurning = true;
                turnLeft = TURN_180_TURNS;
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
            //brakeAmount = gamePadState.Triggers.Left;
           

            if (keyboardState.IsKeyDown(Keys.Space))
            {
                thrustAmount = 1.0f;
            }
            if (keyboardState.IsKeyDown(Keys.LeftShift) || (gamePadState.IsButtonDown(Buttons.LeftTrigger)))
            {
                thrustAmount = 0.001f; //maybe, set it a .001 for the sake of being able to move
            }

        }
        
        #endregion
    }
}
