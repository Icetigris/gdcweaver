using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace WorldWeaver
{
    class IntroAnimationScreen : MenuScreen
    {
        public const double INTRO_SCREEN_TIME = 3.3;
        public const double ONE_OVER_20 = 0.05;
        public const int FRAMECOUNT = 160;

        private const int SIZE = 20;

        private double leftoverTime = 0.0;
        private int frameCounter = 0;
        private int frameIndex = 0;
        private double elapsedTime = 0.0;
        private int firstRun = 0;

        ContentManager content;
        Texture2D[] frames = new Texture2D[SIZE]; //300 frame animation, only hold 1 sec at a time
        Texture2D[] backBuffer = new Texture2D[SIZE];
        Texture2D[] activeFrameBuffer;
        Texture2D[] activeBackBuffer;
        double timeInSeconds;
        bool fadingOut;

        public IntroAnimationScreen()
            : base("IntroAnimation")
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            timeInSeconds = 0;
            fadingOut = false;

            //BDH
            activeFrameBuffer = frames;
            activeBackBuffer = backBuffer;
        }

        public override void LoadContent()
        {
            if (content == null)
            {
                content = new ContentManager(ScreenManager.Game.Services, "Content");
            }

            //load first frames
            LoadFirstFrames();

        }

        /// <summary>
        /// Unloads graphics content for this screen.
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();
        }

        protected override void OnCancel()
        {
            timeInSeconds = INTRO_SCREEN_TIME;
        }

        protected override void OnSelectEntry(int entryIndex)
        {
        }

        private void LoadFirstFrames()
        {
            String filenumber = "";
            for (int i = 0; i < frames.Length; i++)
            {
                filenumber = "Video\\bigbang0";

                if (frameIndex > FRAMECOUNT)
                {
                    frameIndex = FRAMECOUNT;
                }

                //account for bigbang0000.jpg naming scheme
                if (frameIndex < 10)
                {
                    filenumber += "00";
                    filenumber += frameIndex;
                }
                else if (frameIndex < 100)
                {
                    filenumber += "0";
                    filenumber += frameIndex;
                }
                else
                {
                    filenumber += frameIndex;
                }

                activeFrameBuffer[i] = content.Load<Texture2D>(filenumber);
                activeFrameBuffer[i].Name = filenumber;
                filenumber = "";
                frameIndex++;
            }
        }

        private void swapBuffers()
        {
            //temp = old
            //old = new
            //new = temp
            if (activeFrameBuffer == frames)
            {
                activeFrameBuffer = backBuffer;
                activeBackBuffer = frames;
            }
            else
            {
                activeFrameBuffer = frames;
                activeBackBuffer = backBuffer;
            }
        }

        private void LoadNextFrame()
        {
            string filenumber = "Video\\bigbang0";

            if (frameIndex > FRAMECOUNT)
            {
                frameIndex = FRAMECOUNT;
            }

            //account for bigbang0000.jpg naming scheme
            if (frameIndex < 10)
            {
                filenumber += "00";
                filenumber += frameIndex;
            }
            else if (frameIndex < 100)
            {
                filenumber += "0";
                filenumber += frameIndex;
            }
            else
            {
                filenumber += frameIndex;
            }

            activeBackBuffer[frameCounter] = content.Load<Texture2D>(filenumber);
            activeBackBuffer[frameCounter].Name = filenumber;
            frameIndex++;
            filenumber = "";
        }

        /// <summary>
        /// Updates the background screen. Unlike most screens, this should not
        /// transition off even if it has been covered by another screen: it is
        /// supposed to be covered, after all! This overload forces the
        /// coveredByOtherScreen parameter to false in order to stop the base
        /// Update method wanting to transition off.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);

            timeInSeconds += gameTime.ElapsedRealTime.TotalSeconds;

            if ((timeInSeconds > INTRO_SCREEN_TIME && !fadingOut) || gamePadState.IsButtonDown(Buttons.Start)
                                                                  || keyboardState.IsKeyDown(Keys.Enter))
            {
                LoadingScreen.Load(ScreenManager, false, new BackgroundScreen(),
                                   new MainMenuScreen());
                fadingOut = true;
            }

            base.Update(gameTime, otherScreenHasFocus, false);
        }


        /// <summary>
        /// Draws the background screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);
            byte fade = TransitionAlpha;

            spriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.BackToFront, SaveStateMode.SaveState);

            //update current frame
            elapsedTime = gameTime.ElapsedRealTime.TotalSeconds;
            elapsedTime += leftoverTime;
            if (firstRun == 0)
            {
                firstRun = 1;
            }
            else if (firstRun == 1)
            {
                firstRun = 2;
            }
            else if (elapsedTime > ONE_OVER_20)
            {
                //advance current frame
                frameCounter++;
                if (frameCounter >= frames.Length)
                {
                    frameCounter = 0;
                    swapBuffers();
                }
            }
            leftoverTime = elapsedTime;

            spriteBatch.Draw(activeFrameBuffer[frameCounter], fullscreen,
                             new Color(fade, fade, fade));
            LoadNextFrame();
            Console.WriteLine(activeFrameBuffer[frameCounter].Name);


            spriteBatch.End();
        }
    }
}
