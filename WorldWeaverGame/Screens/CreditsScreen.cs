using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace WorldWeaver
{
    class CreditsScreen : GameScreen
    {
        private SpriteFont _titleFont;
        String credits1, credits2;

        public CreditsScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            credits1 = "Project Lead\n Elizabeth Baumel\n\n";
            credits1 += "Programming\n Joey Breeden\n Wallace Brown\n Ben Harris\n Kevin Markey\n Abdul Natafgi\n Matt Song\n\n";
            credits2 = "Art\n Denise Rockwell\n Isaac Sohn\n Kevin Durant\n\n";
            credits2+= "Sound\n David Carls\n Chuck Miller";
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

            Color color = new Color(128, 0, 255, TransitionAlpha);

            ScreenManager.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend,
                                            SpriteSortMode.Immediate,
                                            SaveStateMode.SaveState);

            //draw title
            Vector2 titleSize = _titleFont.MeasureString("Credits");
            textPosition = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2 - titleSize.X / 2, 5);

            ScreenManager.SpriteBatch.DrawString(_titleFont, "Credits", textPosition + new Vector2(-50, 200), color);

            color = new Color(255, 255, 255, TransitionAlpha);

            //draw header
            textPosition = new Vector2(50, 100);
            
            ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, credits1, textPosition + new Vector2(0, 150), color);
            ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, credits2, textPosition + new Vector2(300, 150), color);


            ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, "Hit the B button or Esc key to go back", new Vector2(600, 700), Color.White);

            ScreenManager.SpriteBatch.End();

            if (TransitionPosition > 0)
                ScreenManager.FadeBackBufferToBlack(255 - TransitionAlpha);
        }
    }
}
