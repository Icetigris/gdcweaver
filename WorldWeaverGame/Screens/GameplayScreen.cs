using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using System.Text;

namespace WorldWeaver
{
    public class GameplayScreen : GameScreen
    {
        #region Variables

        GraphicsDeviceManager graphics;

        ContentManager Content = null;
        SpriteBatch spriteBatch;
        SpriteFont spriteFont;
        //wb
        Cue gameWonMusic = Globals.musicSoundBank.GetCue(Globals.AssetList.gameWonMusicCueName);
        Cue gameStartMusic = Globals.musicSoundBank.GetCue(Globals.AssetList.gameStartMusicCueName);
        bool havePlayedCleansedSong;
        ParticleEffect_Emmiter orbit_x, orbit_y, orbit_z;
        //end wb

        KeyboardState lastKeyboardState = new KeyboardState();
        GamePadState lastGamePadState = new GamePadState();
        KeyboardState currentKeyboardState = new KeyboardState();
        GamePadState currentGamePadState = new GamePadState();

        Random randomNumberGenerator = new Random();




        #region Player variables
        Player player;
        #endregion

        #region Camera and Sky variables

        ChaseCamera camera;
        bool cameraSpringEnabled = true;
        Skybox skybox;

        #endregion

        #endregion

        #region Initialization


        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.0);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            gameStartMusic.Play();

            AddInitialWorldLight();
            
            //
        }

        public void AddInitialWorldLight()
        {
            Globals.lights = new Lights[Globals.maxLights];
            Lights.AddLight(new Lights(
                new Vector3(0.0f, 100.0f, 0.0f),
                new Vector4(0.4f),
                new Vector4(0.4f),
                Color.White.ToVector4(),
                new Vector4(0.8f, 0.8f, 0.8f, 1.0f),
                Color.White.ToVector4())
                );

            Globals.lightSource = new LinkedList<Star>();
        }

        public override void LoadContent()
        {
            if (Content == null)
            {
                Content = new ContentManager(ScreenManager.Game.Services, "Content");
                Globals.contentManager = Content;
            }
             graphics = ScreenManager.GraphicsManager;
            
            //Create chase camera
            camera = new ChaseCamera(graphics);            
            Globals.ChaseCamera = camera;

            Globals.hudCamera = new HudCamera(new Vector3(0, 0, 0), Matrix.CreateLookAt(new Vector3(0, 0, 0), new Vector3(0, 0, -1), new Vector3(0, 1, 0))/*camera.View*/, Globals.ChaseCamera.Projection);

            Globals.last10ParticlesHUD = new Last10ParticlesHUD();

            //Load models, fonts
            InitializeGraphics();

            skybox = new Skybox(camera, graphics);

            skybox.MySceneIndex = SceneGraphManager.SceneCount;
            SceneGraphManager.AddObject(skybox);


            //move played here to try to fix update order problem
            //Matt Song
            //wb
            Globals.cleansedGalaxy = false;
            havePlayedCleansedSong = false;
            Globals.numStars = 0;
            player = new Player(Globals.godcharge, camera, Content, graphics);
            Globals.Player = player;

            //REMEMBER TO UNCOMMENT SOON
            player.MySceneIndex = SceneGraphManager.SceneCount;
            SceneGraphManager.AddObject(player);

            //wb
            Vector3 temp = new Vector3(player.Position.X, player.Position.Y - 150.0f,
                player.Position.Z);
            orbit_x = new ParticleEffect_Emmiter(ParticleEffect_Behavior.fall, 25.0f,
                    temp, 60, 'x');
            orbit_y = new ParticleEffect_Emmiter(ParticleEffect_Behavior.fall, 25.0f,
                    temp, 60, 'y');
            orbit_z = new ParticleEffect_Emmiter(ParticleEffect_Behavior.fall, 25.0f,
                    temp, 60, 'z');

            SceneGraphManager.AddObject(orbit_x);
            SceneGraphManager.AddObject(orbit_y);
            SceneGraphManager.AddObject(orbit_z);
            //end


            Globals.gyro = new GyroPersp(graphics);
            Globals.gyro.MySceneIndex = SceneGraphManager.SceneCount;
            Console.WriteLine("Gyroscope's index: " + Globals.gyro.MySceneIndex);
            SceneGraphManager.AddObject(Globals.gyro);

            

            

            // Pre-calculate the HUD element positions.
            ChargeBarHUD chargeBar = new ChargeBarHUD();
            Globals.hudManager.AddElement(chargeBar);
            Globals.hudManager.AddElement(new PopUpCreationMenuHUD());
            Globals.hudManager.AddElement(new ChargeBarSliderHUD(chargeBar));
            Globals.hudManager.AddElement(Globals.last10ParticlesHUD);
            Globals.hudManager.AddElement(new PopUpChargeData());

            List<HUDElement> hudElement = HUDManager.HudElements;

            foreach (HUDElement hudE in hudElement)
            {
                if (hudE.GetType() == typeof(Last10ParticlesHUD) )
                {
                    ((Last10ParticlesHUD)(hudE)).Clear();
                }
            }

            Console.WriteLine("Skybox's index: " + skybox.MySceneIndex);
            Console.WriteLine("Player's index: " + player.MySceneIndex);

            Globals.solarSystem = new SolarSystem();

            //Star s = new Star("herp", Vector3.One, Globals.Player.Position, 1.0, Globals.Player.mPool);
            //s.MySceneIndex = SceneGraphManager.SceneCount;
            //Console.WriteLine(s.Name + "'s index: " + s.MySceneIndex);
            //SceneGraphManager.AddObject(s);
            SceneGraphManager.Root.LoadContent();
            ScreenManager.Game.ResetElapsedTime();
           
        }

        public void InitializeGraphics()
        {
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
            spriteFont = Content.Load<SpriteFont>(Globals.AssetList.spritefont);

            for (int i = 0; i <= 100; i++)
            {
                //wb
                Particle p = new Particle(randomNumberGenerator, SceneGraphManager.SceneCount, Content, graphics);
                //end Code[]
                p.MySceneIndex = SceneGraphManager.SceneCount;
                SceneGraphManager.AddObject(p);
                Console.WriteLine("Particle #" + i + "'s index: " + p.MySceneIndex);
            }
            #region PARTICLE DEBUG SHIT
            /*
            particlesInSpace.Add(new Particle(1, Particle.Colours.Red, new Vector3(20.0f, 400f, 10f)));
            particlesInSpace.Add(new Particle(2, Particle.Colours.Orange, new Vector3(200.0f, 400f, 90f)));
            particlesInSpace.Add(new Particle(3, Particle.Colours.Yellow, new Vector3(120.0f, 1400f, 100f)));
            particlesInSpace.Add(new Particle(4, Particle.Colours.Green, new Vector3(-90.0f, 4400f, -80f)));
            particlesInSpace.Add(new Particle(-1, Particle.Colours.Blue, new Vector3(-520.0f, 500f, -410f)));
            particlesInSpace.Add(new Particle(-2, Particle.Colours.Purple, new Vector3(-50.0f, 700f, -310f)));
            particlesInSpace.Add(new Particle(-3, Particle.Colours.Silver, new Vector3(-220.0f, 600f, 210f)));
            particlesInSpace.Add(new Particle(-4, Particle.Colours.Shiny, new Vector3(100.0f, 900f, 110f)));
            */
            #endregion
        }

        protected Rectangle GetTitleSafeArea(float percent)
        {
            Rectangle retval = new Rectangle(graphics.GraphicsDevice.Viewport.X,
                graphics.GraphicsDevice.Viewport.Y,
                graphics.GraphicsDevice.Viewport.Width,
                graphics.GraphicsDevice.Viewport.Height);
            /*
#if XBOX
        // Find Title Safe area of Xbox 360.
        float border = (1 - percent) / 2;
        retval.X = (int)(border * retval.Width);
        retval.Y = (int)(border * retval.Height);
        retval.Width = (int)(percent * retval.Width);
        retval.Height = (int)(percent * retval.Height);
        return retval;            
#else
            return retval;
#endif
             */
            return retval;
        }


        /*public void InitializeHUD()
        {
            hudRect = GetTitleSafeArea(20f);

            int w = hudRect.Width;
            int h = hudRect.Height;

            last10InChain = Content.Load<Texture2D>("Textures\\last10particlesinchain");
            chargeBar = Content.Load<Texture2D>("Textures\\chargebar");
            chargeSlider = Content.Load<Texture2D>("Textures\\chargebarmarker");
            particleSprite = Content.Load<Texture2D>("Textures\\largeparticle");

            last10Rect = new Rectangle((int)((w - w*0.1f) - last10InChain.Width / 2),
                                          (int)((h - h*0.6f) - last10InChain.Height / 2),
                                          last10InChain.Width,
                                          last10InChain.Height);

            chargeBarRect = new Rectangle((int)((w * 0.2f) - chargeBar.Width / 2),
                                          (int)((h - h*0.1f) - chargeBar.Height / 2),
                                          chargeBar.Width,
                                          chargeBar.Height);

            chargeSliderRect = new Rectangle((int)((chargeBarRect.Width * 0.6f) - chargeSlider.Width / 2),
                                          (int)((h - h * 0.132f) - chargeSlider.Height / 2),
                                          chargeSlider.Width,
                                          chargeSlider.Height);

            chargeTextPos = new Vector2(w - (int)(w * 0.93f), (int)(h - h * 0.1f));
        }*/

        public override void UnloadContent()
        {
            gameWonMusic.Stop(AudioStopOptions.AsAuthored);
            gameStartMusic.Stop(AudioStopOptions.AsAuthored);
            Globals.gameplayScreenDestroyed = true;
            SceneGraphManager.Root.UnloadContent();
            SceneGraphManager.EmptyGraph();
            DestroyLights();
            Content.Unload();
        }

        private void DestroyLights()
        {
            Globals.lights = null;
            Globals.numLights = 0;
        }


        #endregion

        #region Update


        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                    bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            //if pause screen is up, don't have scene graph draw shit
            Globals.gameplayScreenHasFocus = IsActive;

            if (IsActive)
            {
                lastKeyboardState = currentKeyboardState;
                lastGamePadState = currentGamePadState;

                currentKeyboardState = Keyboard.GetState();
                currentGamePadState = GamePad.GetState(PlayerIndex.One);

                // Pressing the Back button or Home key toggles the spring behavior on and off
                if (lastKeyboardState.IsKeyUp(Keys.Home) &&
                    (currentKeyboardState.IsKeyDown(Keys.Home)) ||
                    (lastGamePadState.Buttons.Back == ButtonState.Released &&
                    currentGamePadState.Buttons.Back == ButtonState.Pressed))
                {
                    cameraSpringEnabled = !cameraSpringEnabled;
                }

                // Update the player
                player.GetPlayerRequirements(gameTime);

                //wb
                if (Globals.gameTime == null)
                {
                    Globals.gameTime = gameTime;
                }
                UpdateStageCleansed();
                Vector3 t = new Vector3(player.Position.X + 30.0f, player.Position.Y - 50.0f, player.Position.Z - 140.0f);
                orbit_x.UpdatePosition(t);
                orbit_y.UpdatePosition(t);
                orbit_z.UpdatePosition(t);
                //

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
            }
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

        //wb
        private void UpdateStageCleansed()
        {
            if (Globals.numStars > 0)
            {
                Globals.cleansedGalaxy = true;
            }
            if (Globals.cleansedGalaxy && !havePlayedCleansedSong)
            {
                havePlayedCleansedSong = true;
                gameStartMusic.Stop(AudioStopOptions.AsAuthored);
                gameWonMusic.Play();
            }
        }
        //end wb

        #endregion

        #region Draw

        public override void Draw(GameTime gameTime)
        {
            // Must reset these states or else the render will not be
            // correct.  The sprite rendering is at fault in the
            // Game State Management sample.
            ScreenManager.GraphicsDevice.RenderState.DepthBufferEnable = true;
            ScreenManager.GraphicsDevice.RenderState.AlphaBlendEnable = false;
            ScreenManager.GraphicsDevice.RenderState.AlphaTestEnable = false;
            ScreenManager.GraphicsDevice.SamplerStates[0].AddressU = TextureAddressMode.Wrap;
            ScreenManager.GraphicsDevice.SamplerStates[0].AddressV = TextureAddressMode.Wrap;
            ScreenManager.GraphicsDevice.SamplerStates[0].AddressW = TextureAddressMode.Wrap;
            ScreenManager.GraphicsDevice.SamplerStates[0].MagFilter = TextureFilter.Anisotropic;
            ScreenManager.GraphicsDevice.SamplerStates[0].MinFilter = TextureFilter.Anisotropic;
            ScreenManager.GraphicsDevice.SamplerStates[0].MipFilter = TextureFilter.Linear;            

            player.GetPlayerRequirements(gameTime);

            DrawOverlayText();

        }
        /*
        private void DrawHUD()
        {
            ScreenManager.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend,
                                         SpriteSortMode.Immediate,
                                         SaveStateMode.SaveState);
            
            int w = ScreenManager.GraphicsDevice.Viewport.Width;
            int h = ScreenManager.GraphicsDevice.Viewport.Height;

            string displayString = "";

            if (player.Charge > 0)
            {
                displayString += "+" + player.Charge;
            }
            else if (player.Charge < 0)
            {
                displayString += "" + player.Charge;
            }
            else
            {
                displayString = "0";
            }

            spriteFont.MeasureString(displayString);
            
            ScreenManager.SpriteBatch.Draw(last10InChain, last10Rect, Color.White);
            ScreenManager.SpriteBatch.Draw(chargeBar, chargeBarRect, Color.White);
            ScreenManager.SpriteBatch.Draw(chargeSlider, updateChargeSliderPos(chargeSliderRect), Color.White);

            ScreenManager.SpriteBatch.DrawString(spriteFont, displayString,
                                                 chargeTextPos, Color.White);

            ScreenManager.SpriteBatch.End();
        }
        */
        private Rectangle updateChargeSliderPos(Rectangle old)
        {
            Rectangle chargeSliderPos = new Rectangle(old.X + player.Charge * 5, old.Y, old.Width, old.Height);


            return chargeSliderPos;
        }

        /// <summary>
        /// Displays an overlay showing what the controls are,
        /// and which settings are currently selected.
        /// </summary>
        /// 
        private void DrawOverlayText()
        {
            if (Globals.DEBUG)
            {
                spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred,
                    SaveStateMode.SaveState);

                string text = "Right Trigger or Spacebar = thrust\n" +
                              "Left Thumb Stick or Arrow keys = steer\n" +
                              "A key or L button = toggle camera spring (" + (cameraSpringEnabled ?
                                  "on" : "off") + ")" + player.CurrentChain + "\nPlayer's total charge: " + player.Charge;


                //string text = "Position: (" + (int)player.Position.X + ", " + (int)player.Position.Y + ", " + (int)player.Position.Z + ")";
                // Draw the string twice to create a drop shadow, first colored black
                // and offset one pixel to the bottom right, then again in white at the
                // intended position. This makes text easier to read over the background.
                spriteBatch.DrawString(spriteFont, text, new Vector2(65, 65), Color.Black);
                spriteBatch.DrawString(spriteFont, text, new Vector2(64, 64), Color.White);

                spriteBatch.End();
            }
        }

        #endregion

        #region Menu Input Handling


        public override void HandleInput(InputState input)
        {
            //if player pauses, bring up pause screen
            if (input.PauseGame)
            {
                // If they pressed pause, bring up the pause menu screen.
                ScreenManager.AddScreen(new PauseMenuScreen(player));
            }
        }

        #endregion
    }
}
