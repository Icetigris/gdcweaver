using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace WorldWeaver
{
    class IntroScreen : MenuScreen
    {
        public const double INTRO_SCREEN_TIME = 3.0;

        ContentManager content;
        Texture2D introTexture;
        double timeInSeconds;

        public IntroScreen() : base("Intro")
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            timeInSeconds = 0;
        }

        public override void LoadContent()
        {
            if (content == null)
            {
                content = new ContentManager(ScreenManager.Game.Services, "Content");
            }

            introTexture = content.Load<Texture2D>(Globals.AssetList.introLogo) ;
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
            timeInSeconds += gameTime.ElapsedGameTime.TotalSeconds;

            if (timeInSeconds > INTRO_SCREEN_TIME)
            {
                LoadingScreen.Load(ScreenManager, true, new BackgroundScreen(),
                                   new MainMenuScreen());
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

            spriteBatch.Draw(introTexture, fullscreen,
                             new Color(fade, fade, fade));

            spriteBatch.End();
        }
    }
}
