#region Using Statements
using System;
using Microsoft.Xna.Framework;
#endregion

namespace WorldWeaver
{
    /// <summary>
    /// The pause menu comes up over the top of the game,
    /// giving the player options to resume or quit.
    /// </summary>
    class PauseMenuScreen : MenuScreen
    {

        
        Player p;
        SolarSystem s;

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public PauseMenuScreen()
            : base("Pause Menu")
        {
            MenuEntries.Add(new MenuEntry("Resume Game"));
            MenuEntries.Add(new MenuEntry("Build Celestial Body"));
            MenuEntries.Add(new MenuEntry("Quit Game"));

            // Flag that there is no need for the game to transition
            // off when the pause menu is on top of it.
            IsPopup = true;
        }

        public PauseMenuScreen(Player player)
            : base("Pause Menu")
        {
            MenuEntries.Add(new MenuEntry("Resume Game"));
            MenuEntries.Add(new MenuEntry("Build Celestial Body"));
            MenuEntries.Add(new MenuEntry("Quit Game"));

            p = player;
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
                    // Resume the game.
                    ExitScreen();
                    break;

                case 1:
                    //Build planet
                    ScreenManager.AddScreen(new BuildPlanetMenuScreen(p));
                    ExitScreen();
                    break;

                case 2:
                    // Quit the game, after a confirmation message box.
                    const string message = "Are you sure you want to quit this game?";
                    Console.WriteLine(message);
                    MessageBoxScreen messageBox = new MessageBoxScreen(message);

                    messageBox.Accepted += QuitMessageBoxAccepted;


                    

                    ScreenManager.AddScreen(messageBox);

                    
                    

                    break;
            }
        }


        /// <summary>
        /// When the user cancels the pause menu, resume the game.
        /// </summary>
        protected override void OnCancel()
        {
            ExitScreen();
        }


        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to quit" message box. This uses the loading screen to
        /// transition from the game back to the main menu screen.
        /// </summary>
        void QuitMessageBoxAccepted(object sender, EventArgs e)
        {
            //I'm gonna try to eliminate everything in the previous solar system to fix the "creation after quit bug"--Kevin
            Globals.Player.mPool.Particles.Clear();
            Globals.numStars = 0;
           // s.SystemEmpty();
            //not going the way I thought....
           

            LoadingScreen.Load(ScreenManager, true, new BackgroundScreen(),
                               new MainMenuScreen());

           
        }


        #endregion

        #region Draw


        /// <summary>
        /// Draws the pause menu screen. This darkens down the gameplay screen
        /// that is underneath us, and then chains to the base MenuScreen.Draw.
        /// </summary>
        /// 
        //original code:
        public override void Draw(GameTime gameTime)
        {
            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);

            base.Draw(gameTime);
        }


        #endregion
    }
}
