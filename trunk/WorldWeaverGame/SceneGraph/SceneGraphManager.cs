using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace WorldWeaver
{
    public class SceneGraphManager : DrawableGameComponent
    {
        private GraphicsDeviceManager graphics;
        public GraphicsDeviceManager GraphicsManager
        {
            get { return graphics; }
        }

        private static int initSceneCount = 0;
        public static int SceneCount
        {
            get { return initSceneCount; }
        }

        //root node of the scene graph
        private static Node _root;
        public static Node Root
        {
            get { return _root; }
        }

        //create the actual SceneGraphManager
        public SceneGraphManager(Game game, GraphicsDeviceManager gdmanager)
            : base(game)
        {
            graphics = gdmanager;
            _root = new Node();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _root.Update();
        }

        //Draw all the objects in the scene graph
        public override void Draw(GameTime gameTime)
        {
            //Console.WriteLine("SceneGraphManager Draw()");
            //graphics.GraphicsDevice.RenderState.DepthBufferEnable = true;
            base.Draw(gameTime);
            _root.Draw(gameTime);
            GC.Collect();
        }

        //Load content
        protected override void LoadContent()
        {
            base.LoadContent();
            _root.LoadContent();
        }

        //Unload content
        protected override void UnloadContent()
        {
            base.UnloadContent();

            _root.UnloadContent();
        }

        public override string ToString()
        {
            return base.ToString();
        }

        //Add a node to the root node
        public static void AddObject(SceneObject newObject)
        {
            SceneObjectNode node = new SceneObjectNode(newObject);
            node.MySceneIndex = initSceneCount;
            initSceneCount++;
            _root.AddNode(node);
        }

        public static void RemoveObject(int initialSceneIndex)
        {
            Node tempNode = _root.NodeChildren.Find(delegate(Node n) { return n.MySceneIndex == initialSceneIndex; });
            _root.RemoveNode(tempNode);
        }

        public static void EmptyGraph()
        {
            _root.DestroyChildren();
            initSceneCount = 0;
        }
    }
}
