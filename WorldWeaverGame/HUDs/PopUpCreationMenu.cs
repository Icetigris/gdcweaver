using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace WorldWeaver
  
    //This will eventually become the pop up menu for easy planet creation...
{
    public class PopUpCreationMenuHUD : HUDElement
    {
        private Texture2D pmenu;
        private Color opacity = Color.White;
        private byte opaque = 0xff; //Color alpha parameter is in bytes -Elizabeth
        private byte greyedOut = 0x77;
        private byte transparent = 0x0;
        private bool canCreate = false;

        public PopUpCreationMenuHUD()
        {
            TexturePath = "Textures\\xboxControllerDPad";
        }

        public override void LoadContent()
        {
            if (Content == null)
            {
                Content = new ContentManager(HudManager.Game.Services, "Content");
            }
            
            opacity.A = transparent;
            pmenu = Globals.hudManager.Game.Content.Load<Texture2D>(TexturePath);

            Position = new Rectangle((int)(((Globals.hudManager.HudAreaWidth * 0.2f) - pmenu.Width / 2)),
                                          (int)((Globals.hudManager.HudAreaHeight - Globals.hudManager.HudAreaHeight * 0.1f) - pmenu.Height * 2),
                                          pmenu.Width,
                                          pmenu.Height);
        }

        public override void UnloadContent()
        {
        }

        public override void Update()
        {
            KeyboardState keyboardState = Keyboard.GetState(); //stays
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One); //stays

            //if the player is pressing the menu button AND there are particles in their molecule pool
            if ((keyboardState.IsKeyDown(Keys.P) || gamePadState.IsButtonDown(Buttons.RightShoulder)) 
                && Globals.Player.mPool.Particles.Count > 0)
            {
                //the D-pad is opaque and they can create CelestialBodies
                opacity.A = opaque;
                canCreate = true;
            }
            else if ((keyboardState.IsKeyDown(Keys.P) || gamePadState.IsButtonDown(Buttons.RightShoulder))
                     && Globals.Player.mPool.Particles.Count <= 0)
            {
                //you can make planets
                opacity.A = greyedOut;
                    
            }
            else
            {
                opacity.A = transparent;
            }

        }

        //KeyboardState keyboardState = Keyboard.GetState(); //stays
        //GamePadState gamePadState = GamePad.GetState(PlayerIndex.One); //stays

        public override void Draw(GameTime gameTime)
        {

            //if (gamePadState.IsButtonDown(Buttons.RightShoulder))
            //{

            Globals.hudManager.SpriteBatch.Draw(pmenu, Position, opacity);
            //}
        }
    }
}