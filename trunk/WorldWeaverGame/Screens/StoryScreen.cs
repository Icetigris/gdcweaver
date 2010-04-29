using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace WorldWeaver
{
    class StoryScreen : GameScreen
    {
        private SpriteFont _titleFont;

        public StoryScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        public override void LoadContent()
        {
            _titleFont = ScreenManager.Game.Content.Load<SpriteFont>(Globals.AssetList.spritefont);

            base.LoadContent();
        }

        public override void HandleInput(InputState input)
        {
            if (input.MenuCancel)
            {
                ExitScreen();
            }
            base.HandleInput(input);
        }

        public override void Draw(GameTime gameTime)
        {
            Vector2 textPosition;

            //ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 0, 0);

            Color color = new Color(128, 0, 255, TransitionAlpha);

            ScreenManager.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend,
                                            SpriteSortMode.Immediate,
                                            SaveStateMode.SaveState);

            //draw title
            Vector2 titleSize = _titleFont.MeasureString("Story");
            textPosition = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2 - titleSize.X / 2, 5);

            ScreenManager.SpriteBatch.DrawString(_titleFont, "Story", textPosition + new Vector2(-50, 200), color);

            color = new Color(255, 255, 255, TransitionAlpha);

            //draw header
            textPosition = new Vector2(50, 100);
            ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, "It is moments after the Big Bang. \n\nParticles have formed and all is chaotic."
           +" \n\nIt is your job as a creation god to create stars and planets,\n especially in configurations that support life."
           +""
           ,textPosition + new Vector2(0, 150), color);

            
            ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, "Hit the B button or Esc key to go back", new Vector2(600, 700), Color.White);

            ScreenManager.SpriteBatch.End();

            if (TransitionPosition > 0)
                ScreenManager.FadeBackBufferToBlack(255 - TransitionAlpha);
        }
    }
}