#region Using Statements
using System;
using Microsoft.Xna.Framework;
#endregion
namespace WorldWeaver
{
    class BuildPlanetMenuScreen : MenuScreen
    {

        #region Variables

        Player p;
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public BuildPlanetMenuScreen(Player player)
            : base("Build Celestial Body Menu")
        {
            Globals.solarSystem = new SolarSystem();

            MenuEntries.Add(new MenuEntry("Build Star"));
            MenuEntries.Add(new MenuEntry("Build Planet"));
            MenuEntries.Add(new MenuEntry("Add Atmosphere"));
            MenuEntries.Add(new MenuEntry("Cancel"));
            p = Globals.Player;
            // Flag that there is no need for the game to transition
            // off when the pause menu is on top of it.
            IsPopup = true;
        }


        #endregion

        #region Handle Input


        /// <summary>
        /// Responds to user menu selections.
        /// </summary>
        protected override void OnSelectEntry(int entryIndex)
        {
            switch (entryIndex)
            {
                case 0:
                    /// Build star
                    if (p.mPool.Particles.Count != 0) //check if mPool is empty before trying to make stuff
                    {
                        Star s = new Star("Sun", Vector3.One, p.Position, 400.0, p.mPool, Globals.sceneGraphManager.GraphicsManager);
                        Globals.numStars++;
                        Globals.solarSystem.Add(s);
                        s.MySceneIndex = SceneGraphManager.SceneCount;
                        Console.WriteLine(s.Name + "'s index: " + s.MySceneIndex);
                        s.LoadContent();
                        SceneGraphManager.AddObject(s);
                        p.mPool.Particles.Clear();
                        Console.WriteLine("Black hole?: " + s.IsBlackHole() + "\n");
                        Console.WriteLine("Mass: " + s.Mass + "\n");
                        Console.WriteLine("Effective Temp: " + s.EffectiveTemp + "\n");
                    }
                    break;

                case 1:
                    //Build planet
                    if (Globals.solarSystem.SystemEmpty() && Globals.numStars > 0) // Requires at least 1 sun
                    {
                        Planet s = new Planet("Planet", Vector3.One, p.Position, 400.0, p.mPool, Globals.sceneGraphManager.GraphicsManager);
                        Globals.solarSystem.Add(s);
                        s.MySceneIndex = SceneGraphManager.SceneCount;
                        Console.WriteLine(s.Name + "'s index: " + s.MySceneIndex);
                        s.LoadContent();
                        SceneGraphManager.AddObject(s);
                        p.mPool.Particles.Clear();
                        Console.WriteLine("Mass: " + s.Mass + "\n");
                        //Console.WriteLine("Magnetic field: " + s.hasMagneticField + "\n");
                        Console.WriteLine("Effective Temp: " + s.EffectiveTemp + "\n");

                        // Add location to list of planet locations in player
                        p.addPlanetLocation(s.Position);

                    }
                    break;

                case 3:
                    // Go back to pause screen
                    ExitScreen();
                    ScreenManager.AddScreen(new PauseMenuScreen());
                    break;
            }
        }

        #endregion

        #region Draw


        /// <summary>
        /// Draws the pause menu screen. This darkens down the gameplay screen
        /// that is underneath us, and then chains to the base MenuScreen.Draw.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);

            base.Draw(gameTime);
        }


        #endregion
    }
}
