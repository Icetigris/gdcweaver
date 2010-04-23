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

        //all this may not be necessary....

        Player player;
        GraphicsDeviceManager graphics;
        ChaseCamera camera;
        bool cameraSpringEnabled = true;
        Skybox skybox;
        ContentManager Content = null;
        //
        
        

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

                    Globals.godcharge = -1;
                    LoadingScreen.Load(ScreenManager, true, new GameplayScreen());
                    Globals.gameplayScreenDestroyed = false;
                    titleCue.Stop(AudioStopOptions.AsAuthored); 

                    
                    break;

                case 1:

                    //gonna try to change some specs here...
        //            if (Content == null)
        //            {
        //                Content = new ContentManager(ScreenManager.Game.Services, "Content");
        //                Globals.contentManager = Content;
        //            }
        //            graphics = ScreenManager.GraphicsManager;

                    //Create chase camera
        //            camera = new ChaseCamera(graphics);
        //            Globals.ChaseCamera = camera;

        //            Globals.hudCamera = new HudCamera(new Vector3(0, 0, 0), Matrix.CreateLookAt(new Vector3(0, 0, 0), new Vector3(0, 0, -1), new Vector3(0, 1, 0))/*camera.View*/, Globals.ChaseCamera.Projection);
        //            player = player = new Player(-3, camera, Content, graphics);
        //            Globals.Player = player;

        //            player.MySceneIndex = SceneGraphManager.SceneCount;
        //            SceneGraphManager.AddObject(player);

                    Globals.godcharge = 3;
                    LoadingScreen.Load(ScreenManager, true, new GameplayScreen());
                    Globals.gameplayScreenDestroyed = false;
                    titleCue.Stop(AudioStopOptions.AsAuthored); 
                    break;

                case 2:

                    Globals.godcharge = -2;
                    LoadingScreen.Load(ScreenManager, true, new GameplayScreen());
                    Globals.gameplayScreenDestroyed = false;
                    titleCue.Stop(AudioStopOptions.AsAuthored);
                    break;

                case 3:

                    Globals.godcharge = -3;
                    LoadingScreen.Load(ScreenManager, true, new GameplayScreen());
                    Globals.gameplayScreenDestroyed = false;
                    titleCue.Stop(AudioStopOptions.AsAuthored);
                    break;

                case 4:

                    Globals.godcharge = 2;
                    LoadingScreen.Load(ScreenManager, true, new GameplayScreen());
                    Globals.gameplayScreenDestroyed = false;
                    titleCue.Stop(AudioStopOptions.AsAuthored);
                    break;

                
                case 5:

                    Globals.godcharge = -4;
                    LoadingScreen.Load(ScreenManager, true, new GameplayScreen());
                    Globals.gameplayScreenDestroyed = false;
                    titleCue.Stop(AudioStopOptions.AsAuthored);
                    break;

                case 6:

                    Globals.godcharge = 1;
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
