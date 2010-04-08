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
        Player p = Globals.Player;
        SolarSystem solarSystem = new SolarSystem();



        //loading extra icons, Kevin
        private Texture2D aIcon;
        private Texture2D pIcon;
        private Texture2D sIcon;

        private Color opacity = Color.White;
        private byte opaque = 0xff; //Color alpha parameter is in bytes -Elizabeth
        private byte greyedOut = 0x77;
        private byte transparent = 0x0;
        private bool canCreate = false;

        private string aIconPath;
        private string pIconPath;
        private string sIconPath;

        private Rectangle top;
        private Rectangle bottom;
        private Rectangle left;
        private Rectangle right;


        public PopUpCreationMenuHUD()
        {
            TexturePath = "Textures\\xboxControllerDPad";

            aIconPath = "Textures\\atmoicon";
            pIconPath = "Textures\\planeticon";
            sIconPath = "Textures\\staricon";

            //Kevin tries to add some other pretty icons in conjuction with the D-Pad
            //IconA = "Textures\\atmoicon";
            //IconP = "Textures\\planeticon";
            //IconS = "Textures\\staricon";
            //extra icons end here
        }

        public override void LoadContent()
        {
            if (Content == null)
            {
                Content = new ContentManager(HudManager.Game.Services, "Content");
            }

            opacity.A = transparent;
            pmenu = Globals.hudManager.Game.Content.Load<Texture2D>(TexturePath);
            aIcon = Globals.hudManager.Game.Content.Load<Texture2D>(aIconPath);
            pIcon = Globals.hudManager.Game.Content.Load<Texture2D>(pIconPath);
            sIcon = Globals.hudManager.Game.Content.Load<Texture2D>(sIconPath);

            //trying to load other icons
            //aIcon = Globals.hudManager.Game.Content.Load<Texture2D>(IconA);
            //pIcon = Globals.hudManager.Game.Content.Load<Texture2D>(IconP);
            //sIcon = Globals.hudManager.Game.Content.Load<Texture2D>(IconS);
            //end


            Position = new Rectangle((int)(((Globals.hudManager.HudAreaWidth * 0.2f) - pmenu.Width / 2)),
                                          (int)((Globals.hudManager.HudAreaHeight - Globals.hudManager.HudAreaHeight * 0.1f) - pmenu.Height * 2),
                                          pmenu.Width,
                                          pmenu.Height);

            top = new Rectangle((Position.X + pmenu.Width / 2) - sIcon.Width / 2, Position.Y - sIcon.Height / 2, sIcon.Width, sIcon.Height);
            left = new Rectangle((Position.X) - pIcon.Width / 2, Position.Y - pIcon.Height / 2 + pmenu.Height / 2, pIcon.Width, pIcon.Height);
            right = new Rectangle((Position.X) - aIcon.Width / 2 + pmenu.Width, Position.Y - aIcon.Height / 2 + pmenu.Height / 2, aIcon.Width, aIcon.Height);
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

                if ((canCreate == true && gamePadState.IsButtonDown(Buttons.DPadUp)))
                {
                    //Make Star

                    if (Globals.Player.mPool.Particles.Count != 0) //check if mPool is empty before trying to make stuff
                    {
                        Star s = new Star("Sun", Vector3.One, Globals.Player.Position, 1.0, Globals.Player.mPool, Globals.sceneGraphManager.GraphicsManager);
                        Globals.numStars++;
                        solarSystem.Add(s);
                        s.MySceneIndex = SceneGraphManager.SceneCount;
                        Console.WriteLine(s.Name + "'s index: " + s.MySceneIndex);
                        s.LoadContent();
                        SceneGraphManager.AddObject(s);
                        Globals.Player.mPool.Particles.Clear();
                        Console.WriteLine("Black hole?: " + s.IsBlackHole() + "\n");
                        Console.WriteLine("Mass: " + s.Mass + "\n");
                        Console.WriteLine("Effective Temp: " + s.EffectiveTemp + "\n");
                    }

                }
                else if ((canCreate == true && gamePadState.IsButtonDown(Buttons.DPadLeft)))
                {
                    if (!solarSystem.SystemEmpty())
                    {
                        // Spawn planet
                        Planet planet = new Planet("Planet", Vector3.One, Globals.Player.Position, 1.0, Globals.Player.mPool, Globals.sceneGraphManager.GraphicsManager);
                        solarSystem.Add(planet);
                        planet.MySceneIndex = SceneGraphManager.SceneCount;
                        Console.WriteLine(planet.Name + "'s index: " + planet.MySceneIndex);
                        planet.LoadContent();
                        SceneGraphManager.AddObject(planet);
                        Globals.Player.mPool.Particles.Clear();

                        // Checks to see what kind of planet we have.
                        Console.WriteLine("Mass: " + planet.Mass + "\n");
                        Console.WriteLine("Gravity: " + planet.GravityPoints + "\n");
                    }
                }

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
            Globals.hudManager.SpriteBatch.Draw(sIcon, top, opacity);
            Globals.hudManager.SpriteBatch.Draw(pIcon, left, opacity);
            Globals.hudManager.SpriteBatch.Draw(aIcon, right, opacity);

            //}
        }
    }
}