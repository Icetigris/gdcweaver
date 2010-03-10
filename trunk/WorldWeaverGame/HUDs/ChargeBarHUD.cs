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

            Position = new Rectangle((int)((Globals.hudManager.HudAreaWidth * 0.2f) - chargeBar.Width / 2),
                                          (int)((Globals.hudManager.HudAreaHeight - Globals.hudManager.HudAreaHeight * 0.1f) - chargeBar.Height / 2),
                                          chargeBar.Width,
                                          chargeBar.Height);
        }

        public override void UnloadContent()
        {
        }

        public override void Update()
        {
            //nothing lol
        }

        public override void Draw(GameTime gameTime)
        {
            Globals.hudManager.SpriteBatch.Draw(chargeBar, Position, Color.White);
        }
    }
}
