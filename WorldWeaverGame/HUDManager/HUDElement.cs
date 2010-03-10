using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace WorldWeaver
{
    public abstract class HUDElement : IDrawableObject, ILoadableObject, IUpdatableObject
    {
        #region Variables

        private Rectangle rect;
        private string texturePath;
        private ContentManager content;
        private HUDManager hudmanager;
        private bool readyToRender;

        #endregion


        #region Properties

        public Rectangle Position
        {
            get { return rect; }
            set { rect = value; }
        }

        public string TexturePath
        {
            get { return texturePath; }
            set { texturePath = value; }
        }

        public ContentManager Content
        {
            get { return content; }
            set { content = value; }
        }

        public HUDManager HudManager
        {
            get { return hudmanager; }
            set { hudmanager = value; }
        }

        public bool ReadyToRender
        {
            get { return readyToRender; }
            set { readyToRender = value; }
        }

        #endregion


        #region Inherited Methods

        public virtual void LoadContent()
        {
            //We can't load here, this is abstract country.
        }

        public virtual void UnloadContent()
        {
            //Can't unload neither.
        }

        public virtual void Update()
        {
            //if I were Valve, and this were a commercial engine with scripting
            //I would have a scripting hook here
        }

        public virtual void Draw(GameTime gameTime)
        {
        }

        #endregion
    }
}
