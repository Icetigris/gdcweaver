using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace WorldWeaver
{
    public class ChargeBarSliderHUD : HUDElement
    {
        #region Variables
        private Texture2D chargeSlider;
        private CrosshairHUD chargeBar;
        private Rectangle originalPosition;
        #endregion

        #region Properties

        public CrosshairHUD ChargeBar
        {
            get { return chargeBar; }
            set { chargeBar = value; }
        }

        #endregion

        #region Constructor

        public ChargeBarSliderHUD(CrosshairHUD chargeBar)
        {
            TexturePath = "Textures\\chargebarmarker";
            this.chargeBar = chargeBar;
        }

        #endregion

        #region Public Methods

        public override void LoadContent()
        {
            chargeSlider = Globals.hudManager.Game.Content.Load<Texture2D>(TexturePath);

            Position = new Rectangle((int)((chargeBar.Position.Width * 0.6f) - chargeSlider.Width / 2),
                                 (int)((Globals.hudManager.HudAreaHeight - Globals.hudManager.HudAreaHeight * 0.132f) - chargeSlider.Height / 2),
                                 chargeSlider.Width,
                                 chargeSlider.Height);
            originalPosition = Position;
        }

        public override void UnloadContent()
        {
        }

        public override void Update()
        {
            Position = new Rectangle(originalPosition.X + Globals.Player.Charge * 5, originalPosition.Y, originalPosition.Width, originalPosition.Height);
        }

        public override void Draw(GameTime gameTime)
        {
            Globals.hudManager.SpriteBatch.Draw(chargeSlider, Position, Color.White);
            //Console.WriteLine("Look ma, I'm drawing!");
        }

        #endregion
    }
}
