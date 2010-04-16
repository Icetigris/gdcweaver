using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace WorldWeaver
{
    public class PopUpChargeData : HUDElement
    {
        // Variable List
        // Constants
        private const float FADE_VALUE = 1.0f;
        private const float FADE_DECREMENT = 0.4f;

        GraphicsDeviceManager graphics;
        ContentManager Content = null;
        SpriteBatch spriteBatch;// = new SpriteBatch(graphics.GraphicsDevice);
        SpriteFont spriteFont;// = Content.Load<SpriteFont>("Fonts\\Arial");
        public static string message = "";
        Texture2D gradientTexture;
        int charge;
        bool collected = true;

        // Time manager
        private static float elapsedTime;

        // Transparency Color data
        private static float fadeAlpha = 1.0f;
        private static float fadeCol = FADE_VALUE;
        Color fade = new Color(1.0f, 1.0f, 1.0f, fadeAlpha);

        Vector2 stringSize;

        //Boolean graphicsInitialized = false;
        //Player player;

        //===============================
        // CONSTRUCTOR (default)
        //===============================
        public PopUpChargeData()
        {
            
        }
        

        // Static method
        public static void setMessage(int i)
        {
            if (i < 0)
            {
                message = "" + i;
            }
            else if (i > 0)
            {
                message = "+" + i;
            }

            // Reset all data
            fadeAlpha = 1.0f;
            //Need more coffeee
            elapsedTime = 0;
        }


        //===============================
        // LoadContent()
        //===============================
        public override void LoadContent()
        {
            ContentManager content = Globals.hudManager.Game.Content;
            Content = new ContentManager(Globals.hudManager.Game.Services, "Content");
            Globals.contentManager = Content;

            spriteBatch = Globals.hudManager.SpriteBatch;
            spriteFont = Content.Load<SpriteFont>("Fonts\\Arial");

        }


        //===============================
        // Draw()
        //===============================
        public override void Draw(GameTime gameTime)
        {
            // Check to see if message is != "", if so, begin counting timer
            if (message.CompareTo("") != 0)
                elapsedTime += gameTime.ElapsedRealTime.Milliseconds;

            // Check to see if 2.5 seconds has elapsed before we remove the text
            if (elapsedTime < 2500.0f)
            {
                spriteBatch.DrawString(spriteFont, message, new Vector2(Globals.hudManager.HudAreaWidth / 2
                + Globals.hudManager.hudAreaX - stringSize.X / 2, Globals.hudManager.HudAreaHeight / 2 + Globals.hudManager.hudAreaY), fade);

                // Degrade the color values
                fadeAlpha -= FADE_DECREMENT * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        
                // Create new color
                fade = new Color(fadeCol, fadeCol, fadeCol, fadeAlpha);

            }
            else
            {
                message = "";

                spriteBatch.DrawString(spriteFont, "", new Vector2(Globals.hudManager.HudAreaWidth / 2
                + Globals.hudManager.hudAreaX - stringSize.X / 2, Globals.hudManager.HudAreaHeight / 2 + Globals.hudManager.hudAreaY), fade);
                elapsedTime = 0.0f;

                // Reset fade value
                fadeAlpha = 1.0f;
                
            }
                     
            
         }
        

    }
}
