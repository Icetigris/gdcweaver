#region Using Statements
using System;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace WorldWeaver
{
    class SelectCharacterScreen : MenuScreen 
    {

        private Cue titleCue;
        

        public SelectCharacterScreen(Cue TitleCue)
            : base("")
        {
            this.titleCue = TitleCue;
            

            MenuEntries.Add(new MenuEntry("Athena"));
            MenuEntries.Add(new MenuEntry("Ananse"));
            MenuEntries.Add(new MenuEntry("Beira"));
            MenuEntries.Add(new MenuEntry("Papatunaku"));
            MenuEntries.Add(new MenuEntry("Magatamaryu"));
            MenuEntries.Add(new MenuEntry("Saraswati"));
            MenuEntries.Add(new MenuEntry("Tezcatlipoca"));
            MenuEntries.Add(new MenuEntry("Return to Main Menu"));
        }

        public override void LoadContent()
        {
            base.LoadContent();
        }

        protected override void OnSelectEntry(int entryIndex)
        {
            base.OnSelectEntry(entryIndex);

            switch (entryIndex)
            {
                case 0:

                    // Play the game.

                    //insert Athena Specifications here...

                    LoadingScreen.Load(ScreenManager, true, new GameplayScreen());
                    Globals.gameplayScreenDestroyed = false;
                    titleCue.Stop(AudioStopOptions.AsAuthored); 

                    
                    break;

                case 7:

                    //Go back
                    ExitScreen();
                    break;
            }
        }
    }
        
}
