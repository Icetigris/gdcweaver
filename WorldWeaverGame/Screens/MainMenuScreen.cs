#region Using Statements
using System;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace WorldWeaver
{
    class MainMenuScreen : MenuScreen
    {
        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        /// 
        private Cue titleCue;

        public MainMenuScreen()
            : base("")
        {
            titleCue = Globals.musicSoundBank.GetCue(Globals.AssetList.titleThemeCueName);
            titleCue.Play();
            MenuEntries.Add(new MenuEntry("Play Game"));
            MenuEntries.Add(new MenuEntry("Story"));
            MenuEntries.Add(new MenuEntry("Controls"));
            MenuEntries.Add(new MenuEntry("Quit"));
        }

        public override void LoadContent()
        {
            base.LoadContent();
        }

        /// <summary>
        /// Responds to user menu selections.
        /// </summary>
        protected override void OnSelectEntry(int entryIndex)
        {
            switch (entryIndex)
            {
                case 0:

                    // Play the game.
                    LoadingScreen.Load(ScreenManager, true, new GameplayScreen());
                    Globals.gameplayScreenDestroyed = false;
                    titleCue.Stop(AudioStopOptions.AsAuthored);
                    break;
                case 1:
                    //Show story screen
                    ScreenManager.AddScreen(new StoryScreen());
                    break;

                case 2:
                    //Show controls
                    ScreenManager.AddScreen(new ControlsScreen());
                    break;

                case 3:
                    // Exit the game.
                    OnCancel();
                    break;
            }
        }

        void StartGame(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new GameplayScreen());
        }

        /// <summary>
        /// When the user cancels the main menu, ask if they want to quit the game.
        /// </summary>
        protected override void OnCancel()
        {
            const string message = "Are you sure you want to quit?";

            MessageBoxScreen messageBox = new MessageBoxScreen(message);

            messageBox.Accepted += ExitMessageBoxAccepted;

            ScreenManager.AddScreen(messageBox);
        }


        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to quit" message box.
        /// </summary>
        void ExitMessageBoxAccepted(object sender, EventArgs e)
        {
            ScreenManager.Game.Exit();
        }

    }
}