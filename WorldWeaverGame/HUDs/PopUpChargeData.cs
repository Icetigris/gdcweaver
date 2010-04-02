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

        //spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
        //spriteFont = Content.Load<SpriteFont>("Fonts\\Arial");

        GraphicsDeviceManager graphics;
        ContentManager Content = null;
        SpriteBatch spriteBatch;// = new SpriteBatch(graphics.GraphicsDevice);
        SpriteFont spriteFont;// = Content.Load<SpriteFont>("Fonts\\Arial");
        public static string message = "";
        Texture2D gradientTexture;
        int charge;
        bool collected = true;

        Vector2 stringSize;

        //Boolean graphicsInitialized = false;
        //Player player;

        //===============================
        // CONSTRUCTOR (default)
        //===============================
        public PopUpChargeData()
        {
            
        }

        /*
        //===============================
        // CONSTRUCTOR
        //===============================
        public PopUpChargeData(bool isCollected, int pCharge)
        {
            collected = isCollected;
            charge = pCharge;

        }
         * */

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
        }

        /*
        public String Message
        {
            get { return message; }
            set
            {
                message = value;
                stringSize = spriteFont.MeasureString(message);
            }
        }
        */


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

            // Default text message so something prints
            //Message = "Particle collected!";      

            //gradientTexture = content.Load<Texture2D>("Images/gradient");
        }


        /*
        // Pop-up text message when the player picks up a particle.  Called everytime a particle is collected.
        // How do we make it vanish?  -Joey        
        private void checkForCollision(BoundingSphere playerCollisionSphere)
        {

            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred,
                SaveStateMode.SaveState);

            

            //string text = "Position: (" + (int)player.Position.X + ", " + (int)player.Position.Y + ", " + (int)player.Position.Z + ")";
            // Draw the string twice to create a drop shadow, first colored black
            // and offset one pixel to the bottom right, then again in white at the
            // intended position. This makes text easier to read over the background.
            //spriteBatch.DrawString(spriteFont, message, new Vector2(65, 65), Color.Black);
            //spriteBatch.DrawString(spriteFont, text, new Vector2(64, 64), Color.White);

            spriteBatch.End();
        }
        */

        //===============================
        // Draw()
        //===============================
        public override void Draw(GameTime gameTime)
        {
            //checkForCollision(player.CollisionSphere);

            // Draw message, we'll calculate time later
            
            //if (collected == true)
            //{                
                spriteBatch.DrawString(spriteFont, message, new Vector2(Globals.hudManager.HudAreaWidth / 2
                    + Globals.hudManager.hudAreaX - stringSize.X / 2, Globals.hudManager.HudAreaHeight / 2 + Globals.hudManager.hudAreaY), Color.White);
                
            //}
            //collected = false;



        }

        





        

    }
}
