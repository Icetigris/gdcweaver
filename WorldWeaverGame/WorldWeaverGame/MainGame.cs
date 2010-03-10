#region File Description
//-----------------------------------------------------------------------------
// MainGame.cs
//
// World Weaver
// Elizabeth Baumel
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
#endregion

namespace WorldWeaver
{
    public class MainGame : Microsoft.Xna.Framework.Game
    {
      #region Declarations

        GraphicsDeviceManager graphics;

        SpriteBatch spriteBatch;
        SpriteFont spriteFont;


        KeyboardState lastKeyboardState = new KeyboardState();
        GamePadState lastGamePadState = new GamePadState();
        KeyboardState currentKeyboardState = new KeyboardState();
        GamePadState currentGamePadState = new GamePadState();

        
        ChaseCamera camera;

        #region Player variables
        Player player = new Player(-1);

        Model characterModel;
        #endregion

        Model groundModel;

        #region Particle variables
        public enum ParticleModels
        { 
            Plus1,
            Plus2,
            Plus3,
            Plus4,
            Minus1,
            Minus2,
            Minus3,
            Minus4
        }

        Model[] particleModels = new Model[8];
        List<Particle> particlesInSpace;
        List<Particle> particlesToDelete;
        #endregion

        bool cameraSpringEnabled = true;

        #endregion

        #region Initialization


        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 853;
            graphics.PreferredBackBufferHeight = 480;


            // Create the chase camera
            camera = new ChaseCamera();

            // Set the camera offsets
            camera.DesiredPositionOffset = new Vector3(0.0f, 50.0f, 1000.0f);
            camera.LookAtOffset = new Vector3(0.0f, 150.0f, 0.0f);

            // Set camera perspective
            camera.NearPlaneDistance = 10.0f;
            camera.FarPlaneDistance = 100000.0f;

            //TODO: Set any other camera invariants here such as field of view
        }


        /// <summary>
        /// Initalize the game
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();


            // Set the camera aspect ratio
            // This must be done after the class to base.Initalize() which will
            // initialize the graphics device.
            camera.AspectRatio = (float)graphics.GraphicsDevice.Viewport.Width /
                graphics.GraphicsDevice.Viewport.Height;

            
            // Perform an inital reset on the camera so that it starts at the resting
            // position. If we don't do this, the camera will start at the origin and
            // race across the world to get behind the chased object.
            // This is performed here because the aspect ratio is needed by Reset.
            UpdateCameraChaseTarget();
            camera.Reset();

            particlesInSpace = new List<Particle>();
            particlesToDelete = new List<Particle>();
            particlesInSpace.Add(new Particle(1, Particle.Colours.Red, new Vector3(20.0f, 400f, 10f)));
            particlesInSpace.Add(new Particle(2, Particle.Colours.Orange, new Vector3(200.0f, 400f, 90f)));
            particlesInSpace.Add(new Particle(3, Particle.Colours.Yellow, new Vector3(120.0f, 1400f, 100f)));
            particlesInSpace.Add(new Particle(4, Particle.Colours.Green, new Vector3(-90.0f, 4400f, -80f)));
            particlesInSpace.Add(new Particle(-1, Particle.Colours.Blue, new Vector3(-520.0f, 500f, -410f)));
            particlesInSpace.Add(new Particle(-2, Particle.Colours.Purple, new Vector3(-50.0f, 700f, -310f)));
            particlesInSpace.Add(new Particle(-3, Particle.Colours.Silver, new Vector3(-220.0f, 600f, 210f)));
            particlesInSpace.Add(new Particle(-4, Particle.Colours.Shiny, new Vector3(100.0f, 900f, 110f)));
        }


        /// <summary>
        /// Load graphics content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
            spriteFont = Content.Load<SpriteFont>("Fonts\\Arial");

            characterModel = Content.Load<Model>("Models\\athena");

            particleModels[(int)ParticleModels.Plus1] = Content.Load<Model>("Models\\+1");
            particleModels[(int)ParticleModels.Plus2] = Content.Load<Model>("Models\\+2");
            particleModels[(int)ParticleModels.Plus3] = Content.Load<Model>("Models\\+3");
            particleModels[(int)ParticleModels.Plus4] = Content.Load<Model>("Models\\+4");
            particleModels[(int)ParticleModels.Minus1] = Content.Load<Model>("Models\\-1");
            particleModels[(int)ParticleModels.Minus2] = Content.Load<Model>("Models\\-2");
            particleModels[(int)ParticleModels.Minus3] = Content.Load<Model>("Models\\-3");
            particleModels[(int)ParticleModels.Minus4] = Content.Load<Model>("Models\\-4");

            groundModel = Content.Load<Model>("Models\\Ground");
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Allows the game to run logic.
        /// </summary>
        protected override void Update(GameTime gameTime)
        {
            lastKeyboardState = currentKeyboardState;
            lastGamePadState = currentGamePadState;

            currentKeyboardState = Keyboard.GetState();
            currentGamePadState = GamePad.GetState(PlayerIndex.One);


            // Exit when the Escape key or Back button is pressed
            if (currentKeyboardState.IsKeyDown(Keys.Escape) ||
                currentGamePadState.Buttons.Back == ButtonState.Pressed)
            {
                Exit();
            }

            // Pressing the A button or key toggles the spring behavior on and off
            if (lastKeyboardState.IsKeyUp(Keys.A) &&
                (currentKeyboardState.IsKeyDown(Keys.A)) ||
                (lastGamePadState.Buttons.LeftShoulder == ButtonState.Released &&
                currentGamePadState.Buttons.LeftShoulder == ButtonState.Pressed))
            {
                cameraSpringEnabled = !cameraSpringEnabled;
            }

            // Reset the player on R key or right thumb stick clicked
           /* if (currentKeyboardState.IsKeyDown(Keys.R) ||
                currentGamePadState.Buttons.RightStick == ButtonState.Pressed)
            {
                player.Reset();
                camera.Reset();
            }
            */
            // Update the player
            player.Update(gameTime);
            foreach (Particle particle in particlesInSpace)
            {
                particle.Move(player.Position, player.Charge, player.CollisionSphere);//, 0.001f);
                if (player.CollisionSphere.Intersects(particle.CollectionSphere))
                {
                    player.CurrentChain.addParticle(particle);
                    particle.IsCollected = true;
                    particlesToDelete.Add(particle);
                }
            }

            foreach(Particle particle in particlesToDelete)
            {
                particlesInSpace.Remove(particle);
            }
            particlesToDelete.Clear();
            // Update the camera to chase the new target
            UpdateCameraChaseTarget();

            // The chase camera's update behavior is the springs, but we can
            // use the Reset method to have a locked, spring-less camera
            if (cameraSpringEnabled)
            {
                camera.Update(gameTime);
            }
            else
            {
                camera.Reset();
            }


            base.Update(gameTime);
        }

        /// <summary>
        /// Update the values to be chased by the camera
        /// </summary>
        private void UpdateCameraChaseTarget()
        {            
            camera.ChasePosition = player.Position;
            camera.ChaseDirection = player.Direction;
            camera.Up = player.Up;
        }


        /// <summary>
        /// Draws the player and ground.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice device = graphics.GraphicsDevice;

            device.Clear(Color.MediumPurple);

            DrawModel(characterModel, player.World);
            
            foreach (Particle particle in particlesInSpace)
            {
                if (particle.IsCollected == false)
                {
                    //particle.OrientationToTarget(player.Position);
                    DrawModel(particleModels[particle.AssignModel()], Matrix.CreateTranslation(particle.Position));
                }
            }
            
            //DrawModel(plus1, Matrix.Identity);
            DrawModel(groundModel, Matrix.Identity);

            DrawOverlayText();

            base.Draw(gameTime);
        }


        /// <summary>
        /// Simple model drawing method. The interesting part here is that
        /// the view and projection matrices are taken from the camera object.
        /// </summary>        
        private void DrawModel(Model model, Matrix world)
        {
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


        /// <summary>
        /// Displays an overlay showing what the controls are,
        /// and which settings are currently selected.
        /// </summary>
        private void DrawOverlayText()
        {
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred,
                SaveStateMode.SaveState);

            string text = "Right Trigger or Spacebar = thrust\n" +
                          "Left Thumb Stick or Arrow keys = steer\n" +
                          "A key or L button = toggle camera spring (" + (cameraSpringEnabled ?
                              "on" : "off") + ")" + player.CurrentChain + "\nPlayer's total charge: " + player.Charge;

            // Draw the string twice to create a drop shadow, first colored black
            // and offset one pixel to the bottom right, then again in white at the
            // intended position. This makes text easier to read over the background.
            spriteBatch.DrawString(spriteFont, text, new Vector2(65, 65), Color.Black);
            spriteBatch.DrawString(spriteFont, text, new Vector2(64, 64), Color.White);

            spriteBatch.End();
        }


        #endregion
    }


    #region Entry Point

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    static class Program
    {
        static void Main()
        {
            using (MainGame game = new MainGame())
            {
                game.Run();
            }
        }
    }

    #endregion
}