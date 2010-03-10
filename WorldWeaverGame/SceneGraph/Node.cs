using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace WorldWeaver
{
    public class Node
    {
        private int mySceneIndex;
        public int MySceneIndex
        {
            get { return mySceneIndex; }
            set { mySceneIndex = value; }
        }

        //list of children of this node
        protected NodeList _nodeList;

        //returns list of children of this node
        public NodeList NodeChildren
        {
            get { return _nodeList; }
        }

        //constructor
        public Node()
        {
            _nodeList = new NodeList();
        }

        public void AddNode(Node node)
        {
            _nodeList.Add(node);
        }

        public void RemoveNode(Node node)
        {
            _nodeList.Remove(node);
        }

        public void DestroyChildren()
        {
            _nodeList.Clear();
        }

        public virtual void Update()
        {
            _nodeList.ForEach(
            delegate(Node n)
            {
                n.Update();
            });
        }

        public virtual void LoadContent()
        {
            _nodeList.ForEach(
            delegate(Node n)
            {
                n.LoadContent();
            });
        }

        public virtual void UnloadContent()
        {
            _nodeList.ForEach(
            delegate(Node n)
            {
                n.UnloadContent();
            });
        }

        public virtual void Draw(GameTime gameTime)
        {
            //Console.WriteLine("Node Draw() outer");
            _nodeList.ForEach(
            delegate(Node n)
            {
                n.Draw(gameTime);
                //Console.WriteLine("Node Draw() inner");
            });
        }
    }
}
