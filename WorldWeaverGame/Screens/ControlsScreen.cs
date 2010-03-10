using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace WorldWeaver
{
    class ControlsScreen : GameScreen
    {
        private SpriteFont _titleFont;

        public ControlsScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        public override void LoadContent()
        {
            _titleFont = ScreenManager.Game.Content.Load<SpriteFont>("Fonts/Arial");

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
            Vector2 titleSize = _titleFont.MeasureString("Controls");
            textPosition = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2 - titleSize.X / 2, 5);

            ScreenManager.SpriteBatch.DrawString(_titleFont, "Controls", textPosition + new Vector2(-50, 200), color);

            color = new Color(255, 255, 255, TransitionAlpha);

            //draw header
            textPosition = new Vector2(50, 100);
            ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, "Thrust: R trigger OR Spacebar", textPosition + new Vector2(0, 250), color);
            ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, "Directions: Left analog stick OR arrow buttons", textPosition + new Vector2(0, 300), color);
            ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, "Confirm: A OR Enter", textPosition + new Vector2(0, 350), color);
            ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, "Cancel: B OR Esc key", textPosition + new Vector2(0, 400), color);
            ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, "Pause: Start OR Esc Key", textPosition + new Vector2(0, 450), color);
            ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, "Toggle Camera Spring: Back button OR A key", textPosition + new Vector2(0, 500), color);

            
            ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, "Hit the B button or Esc key to go back", new Vector2(600, 700), Color.White);

            ScreenManager.SpriteBatch.End();

            if (TransitionPosition > 0)
                ScreenManager.FadeBackBufferToBlack(255 - TransitionAlpha);
        }
    }
}
