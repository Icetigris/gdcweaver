using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace WorldWeaver
{
    public class HUDManager : DrawableGameComponent
    {
        #region Variables
        GraphicsDeviceManager gdManager;
        private static ContentManager content;
        private SpriteBatch spriteBatch;
        private bool isInitialized = false;

        Rectangle hudArea = new Rectangle(1, 1, 1, 1);
        private int hudAreaWidth;
        private int hudAreaHeight;
        private static List<HUDElement> hudElements = new List<HUDElement>();
        #endregion

        #region Properties

        public static ContentManager Content
        {
            get { return content; }
            set { content = value; }
        }

        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
        }

        public int HudAreaWidth
        {
            get { return hudAreaWidth; }
        }

        public int HudAreaHeight
        {
            get { return hudAreaHeight; }
        }

        public static List<HUDElement> HudElements
        {
            get { return hudElements; }
        }

        #endregion

        #region Constructor

        public HUDManager(Game game, GraphicsDeviceManager graphicsDeviceManager)
            : base(game)
        {
            gdManager = graphicsDeviceManager;
        }

        #endregion


        #region Inherited Methods

        public override void Initialize()
        {
            base.Initialize();

            isInitialized = true;
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            GetTitleSafeArea(20f);
            hudAreaWidth = hudArea.Width;
            hudAreaHeight = hudArea.Height;

            //Load the content for the HUDManager
            content = Game.Content;
            spriteBatch = new SpriteBatch(gdManager.GraphicsDevice);
            foreach (HUDElement e in hudElements)
            {
                e.LoadContent();
                e.ReadyToRender = true;
            }
        }

        protected override void UnloadContent()
        {
            foreach (HUDElement e in hudElements)
            {
                e.UnloadContent();
            }
            DestroyHUD();
        }

        public override void Update(GameTime gameTime)
        {
            foreach(HUDElement e in hudElements)
            {
                e.Update();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            SpriteBatch.Begin(SpriteBlendMode.AlphaBlend,
                                         SpriteSortMode.Immediate,
                                         SaveStateMode.SaveState);

            foreach (HUDElement e in hudElements)
            {
                e.Draw(gameTime);
            }

            SpriteBatch.End();
        }

        #endregion


        #region Public Methods

        public void GetTitleSafeArea(float percent)
        {
            hudArea.X = gdManager.GraphicsDevice.Viewport.X;
            hudArea.Y = gdManager.GraphicsDevice.Viewport.Y;
            hudArea.Width = gdManager.GraphicsDevice.Viewport.Width;
            hudArea.Height = gdManager.GraphicsDevice.Viewport.Height;
        }

        public void AddElement(HUDElement element)
        {
            hudElements.Add(element);
            if(isInitialized)
            {
                element.HudManager = this;
                element.Content = content;
                element.LoadContent();
                element.ReadyToRender = true;
            }
        }

        public void RemoveElement(HUDElement element)
        {
            hudElements.Remove(element);
        }

        public void DestroyHUD()
        {
            hudElements.Clear();
        }

        #endregion


    }
}
