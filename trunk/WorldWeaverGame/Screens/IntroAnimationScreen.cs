using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Graphics;

namespace WorldWeaver
{
    class IntroAnimationScreen : MenuScreen
    {
        private Video introVid;
        private VideoPlayer player;

        ContentManager content;
        
        double timeInSeconds;
        bool fadingOut;
        bool doOnce = true;

        public IntroAnimationScreen()
            : base("IntroAnimation")
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            timeInSeconds = 0;
            fadingOut = false;

            player = new VideoPlayer();
        }

        public override void LoadContent()
        {
            if (content == null)
            {
                content = new ContentManager(ScreenManager.Game.Services, "Content");
            }

            introVid = content.Load<Video>("Video\\WWtest");
            

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
            timeInSeconds = introVid.Duration.TotalSeconds;
        }

        protected override void OnSelectEntry(int entryIndex)
        {
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

            if ((timeInSeconds > introVid.Duration.TotalSeconds && !fadingOut) || gamePadState.IsButtonDown(Buttons.Start)
                                                                  || keyboardState.IsKeyDown(Keys.Enter))
            {
                LoadingScreen.Load(ScreenManager, false, new BackgroundScreen(),
                                   new MainMenuScreen());
                fadingOut = true;
                player.Stop();
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

            //spriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.BackToFront, SaveStateMode.SaveState);
            if (doOnce)
            {
                player.Play(introVid);
                doOnce = false;
            }
            //spriteBatch.End();
        }
    }
}
