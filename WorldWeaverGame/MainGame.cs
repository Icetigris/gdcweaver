#region File Description
//-----------------------------------------------------------------------------
// MainGame.cs
//
// World Weaver
// Elizabeth Baumel
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
#endregion

namespace WorldWeaver
{
    public class MainGame : Microsoft.Xna.Framework.Game
    {
      #region Declarations

        GraphicsDeviceManager graphics;

        ScreenManager screenManager;
        SceneGraphManager sceneGraphManager;
        HUDManager HudManager;

        AudioEngine audioEngine;
        WaveBank waveBank;
        SoundBank soundBank;

        AssetSettings settings;
        string xmlAssetList = "assetList";

        KeyboardState lastKeyboardState = new KeyboardState();
        GamePadState lastGamePadState = new GamePadState();
        KeyboardState currentKeyboardState = new KeyboardState();
        GamePadState currentGamePadState = new GamePadState();


        #endregion

        #region Initialization


        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;

            graphics.MinimumVertexShaderProfile = ShaderProfile.VS_1_1;
            graphics.MinimumPixelShaderProfile = ShaderProfile.PS_1_1;

            sceneGraphManager = new SceneGraphManager(this, graphics);
            HudManager = new HUDManager(this, graphics);
            screenManager = new ScreenManager(this, graphics);

            Globals.hudManager = HudManager;
            Globals.sceneGraphManager = sceneGraphManager;
                        
            Components.Add(sceneGraphManager);
            Components.Add(HudManager);
            Components.Add(screenManager);

            

            //Add moar managers here!

            

        }


        /// <summary>
        /// Initalize the game
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

        }


        /// <summary>
        /// Load graphics content.
        /// </summary>
        protected override void LoadContent()
        {
            //load the XML file into the ContentManager
            settings = Content.Load<AssetSettings>(xmlAssetList);

            //make asset list available to everything
            Globals.AssetList = settings;

            audioEngine = new AudioEngine(settings.audioEnginePath);
            waveBank = new WaveBank(audioEngine, settings.waveBankPath);
            soundBank = new SoundBank(audioEngine, settings.soundBankPath);
            Globals.musicSoundBank = soundBank;

//            screenManager.AddScreen(new BackgroundScreen());
//            screenManager.AddScreen(new MainMenuScreen());
            screenManager.AddScreen(new IntroScreen());
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Allows the game to run logic.
        /// </summary>
        protected override void Update(GameTime gameTime)
        {
            lastKeyboardState = currentKeyboardState;
            lastGamePadState = currentGamePadState;

            currentKeyboardState = Keyboard.GetState();
            currentGamePadState = GamePad.GetState(PlayerIndex.One);

            audioEngine.Update();

            base.Update(gameTime);
        }

        /// <summary>
        /// Draws the player and ground.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice device = graphics.GraphicsDevice;

            device.Clear(Color.MediumPurple);

            base.Draw(gameTime);
        }

        #endregion
    }


    #region Entry Point

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    static class Program
    {
        static void Main()
        {
            using (MainGame game = new MainGame())
            {
                game.Run();
            }
        }
    }

    #endregion
}