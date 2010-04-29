using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace WorldWeaver
{
    /// <summary>
    /// Content Pipeline class for loading AssetSettings data from XNB format.
    /// </summary>
    class AssetSettingsReader : ContentTypeReader<AssetSettings>
    {
        protected override AssetSettings Read(ContentReader input, AssetSettings existingInstance)
        {
            AssetSettings settings = new AssetSettings();

            settings.skyboxModelPath = input.ReadString();
            settings.playerNEG1ModelPath = input.ReadString();
            settings.playerNEG2ModelPath = input.ReadString();
            settings.playerNEG3ModelPath = input.ReadString();
            settings.playerNEG4ModelPath = input.ReadString();
            settings.playerPOS1ModelPath = input.ReadString();
            settings.playerPOS2ModelPath = input.ReadString();
            settings.playerPOS3ModelPath = input.ReadString();
            settings.playerPOS4ModelPath = input.ReadString();

            settings.particlePlus1ModelPath = input.ReadString();
            settings.particlePlus2ModelPath = input.ReadString();
            settings.particlePlus3ModelPath = input.ReadString();
            settings.particlePlus4ModelPath = input.ReadString();

            settings.particleMinus1ModelPath = input.ReadString();
            settings.particleMinus2ModelPath = input.ReadString();
            settings.particleMinus3ModelPath = input.ReadString();
            settings.particleMinus4ModelPath = input.ReadString();

            settings.backgroundScreenTexturePath = input.ReadString();

            settings.audioEnginePath = input.ReadString();
            settings.waveBankPath = input.ReadString();
            settings.soundBankPath = input.ReadString();

            settings.titleThemeCueName = input.ReadString();
            settings.gameWonMusicCueName = input.ReadString();
            settings.gameStartMusicCueName = input.ReadString();
            settings.particleCollectSFXCueName = input.ReadString();

            // wallace Brown 11/01/09[]
            settings.PhongFXPath = input.ReadString();
            // end Code[]

            settings.gyro_needle = input.ReadString();
            settings.introLogo = input.ReadString();

            settings.spritefont = input.ReadString();

            return settings;
        }
    }
}
