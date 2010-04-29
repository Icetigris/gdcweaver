using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace WorldWeaver
{
    public class ChargeBarHUD : HUDElement
    {
        private Texture2D chargeBar;
        private int charge;
        private String chargeString = "";
        SpriteFont spriteFont;

        public ChargeBarHUD()
        {
            TexturePath = "Textures\\chargebar";
        }

        public override void LoadContent()
        {
            /*
            if (Content == null)
            {
                Content = new ContentManager(HudManager.Game.Services, "Content");
            }
            */
            chargeBar = Globals.hudManager.Game.Content.Load<Texture2D>(TexturePath);
            spriteFont = Content.Load<SpriteFont>(Globals.AssetList.spritefont);

            Position = new Rectangle((int)((Globals.hudManager.HudAreaWidth * 0.2f) - chargeBar.Width / 2),
                                          (int)((Globals.hudManager.HudAreaHeight - Globals.hudManager.HudAreaHeight * 0.1f) - chargeBar.Height / 2),
                                          chargeBar.Width,
                                          chargeBar.Height);
            
            charge = Globals.Player.calculateCurrentCharge();
            if (charge > 0)
            {
                chargeString = "+";
                chargeString += "" + charge;
            }
            else
            {
                chargeString = "" + charge;
            }
        }

        public override void UnloadContent()
        {
        }

        public override void Update()
        {
            chargeString = "";
            charge = Globals.Player.calculateCurrentCharge();
            if (charge > 0)
            {
                chargeString = "+" + charge;
            }
            else
            {
                chargeString = "" + charge;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Globals.hudManager.SpriteBatch.Draw(chargeBar, Position, Color.White);
            Globals.hudManager.SpriteBatch.DrawString(spriteFont, chargeString, (new Vector2(Position.Left + Position.Right/13, Position.Bottom - Position.Bottom / 13)), Color.White);

        }
    }
}
