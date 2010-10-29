#region Using Statements
using System;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

#endregion

namespace WorldWeaver
{
    class SelectCharacterScreen : MenuScreen
    {

        private Cue titleCue;

        private MainMenuScreen parent;       
        

        public SelectCharacterScreen(MainMenuScreen mainScreen)
            : base("")
        {
            parent = mainScreen;
            

            MenuEntries.Add(new MenuEntry("Athena"));
            MenuEntries.Add(new MenuEntry("Beira"));
            MenuEntries.Add(new MenuEntry("Papatunaku"));
            MenuEntries.Add(new MenuEntry("Saraswati"));
            MenuEntries.Add(new MenuEntry("Tezcatlipoca"));
            MenuEntries.Add(new MenuEntry("Magatamaryu"));
            MenuEntries.Add(new MenuEntry("Ananse"));
            MenuEntries.Add(new MenuEntry("Na'Ngasohu"));
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

                    Globals.godcharge = -1;
                    LoadingScreen.Load(ScreenManager, true, new GameplayScreen());
                    Globals.gameplayScreenDestroyed = false;
                    parent.TitleCue.Stop(AudioStopOptions.AsAuthored); 

                    
                    break;

                case 1:
                    Globals.godcharge = -2;
                    LoadingScreen.Load(ScreenManager, true, new GameplayScreen());
                    Globals.gameplayScreenDestroyed = false;
                    parent.TitleCue.Stop(AudioStopOptions.AsAuthored); 
                    break;

                case 2:

                    Globals.godcharge = -3;
                    LoadingScreen.Load(ScreenManager, true, new GameplayScreen());
                    Globals.gameplayScreenDestroyed = false;
                    parent.TitleCue.Stop(AudioStopOptions.AsAuthored);
                    break;

                case 3:

                    Globals.godcharge = -4;
                    LoadingScreen.Load(ScreenManager, true, new GameplayScreen());
                    Globals.gameplayScreenDestroyed = false;
                    parent.TitleCue.Stop(AudioStopOptions.AsAuthored);
                    break;

                case 4:

                    Globals.godcharge = 1;
                    LoadingScreen.Load(ScreenManager, true, new GameplayScreen());
                    Globals.gameplayScreenDestroyed = false;
                    parent.TitleCue.Stop(AudioStopOptions.AsAuthored);
                    break;

                
                case 5:

                    Globals.godcharge = 2;
                    LoadingScreen.Load(ScreenManager, true, new GameplayScreen());
                    Globals.gameplayScreenDestroyed = false;
                    parent.TitleCue.Stop(AudioStopOptions.AsAuthored);
                    break;

                case 6:

                    Globals.godcharge = 3;
                    LoadingScreen.Load(ScreenManager, true, new GameplayScreen());
                    Globals.gameplayScreenDestroyed = false;
                    parent.TitleCue.Stop(AudioStopOptions.AsAuthored);
                    break;

                case 7:

                    Globals.godcharge = 4;
                    LoadingScreen.Load(ScreenManager, true, new GameplayScreen());
                    Globals.gameplayScreenDestroyed = false;
                    parent.TitleCue.Stop(AudioStopOptions.AsAuthored);
                    break;

                case 8:

                    //Go back
                    ExitScreen();
                    break;
            }
        }
    }
        
}
