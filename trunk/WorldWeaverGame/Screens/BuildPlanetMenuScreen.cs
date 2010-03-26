#region Using Statements
using System;
using Microsoft.Xna.Framework;
#endregion
namespace WorldWeaver
{
    class BuildPlanetMenuScreen : MenuScreen
    {

        #region Variables

        SolarSystem solarSystem;
        Player p;
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public BuildPlanetMenuScreen(Player player)
            : base("Build Celestial Body Menu")
        {
            solarSystem = new SolarSystem();

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
                    // Build star
                    if (p.mPool.Particles.Count != 0) //check if mPool is empty before trying to make stuff
                    {
                        Star s = new Star("Sun", Vector3.One, p.Position, 1.0, p.mPool);
                        solarSystem.Add(s);
                        s.MySceneIndex = SceneGraphManager.SceneCount;
                        Console.WriteLine(s.Name + "'s index: " + s.MySceneIndex);
                        s.LoadContent();
                        SceneGraphManager.AddObject(s);
                        p.mPool.Particles.Clear();

                        // Calculate star data
                        s.calculateGravityPoints();
                        s.calculateMass();
                        //s.calculateEffectiveTemp();  <---- Why does this result in a temperature of NaN?

                        // What kind of star do we have?
                        Console.WriteLine("Black hole?: " + s.IsBlackHole());
                        Console.WriteLine("Mass: " + s.Mass);
                        Console.WriteLine("Effective Temp: " + s.EffectiveTemp);
                    }
                    break;

                case 1:
                    //Build planet
                    if (solarSystem.SystemEmpty() != true)
                    {
                        // Spawn planet
                        Planet planet = new Planet("Planet", Vector3.One, p.Position, 1.0, p.mPool);
                        solarSystem.Add(planet);
                        planet.MySceneIndex = SceneGraphManager.SceneCount;
                        Console.WriteLine(planet.Name + "'s index: " + planet.MySceneIndex);
                        planet.LoadContent();
                        SceneGraphManager.AddObject(planet);
                        Console.WriteLine("Magnetic field: " + planet.hasMagneticField() + "\n");

                        // Calculate planet data
                        planet.calculateGravityPoints();
                        planet.calculateMass();

                        // Checks to see what kind of planet we have.
                        Console.WriteLine("Mass: " + planet.Mass);
                        Console.WriteLine("Gravity: " + planet.GravityPoints);
                        Console.WriteLine("Radius: " + planet.R);

                        // Clear molecule pool LAST
                        p.mPool.Particles.Clear();

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
