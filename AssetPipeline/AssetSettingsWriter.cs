using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace AssetPipeline
{
    /// <summary>
    /// Content Pipeline class for saving AssetSettings data into XNB format.
    /// </summary>
    [ContentTypeWriter]
    public class AssetSettingsWriter : ContentTypeWriter<AssetSettingsContent>
    {
        protected override void Write(ContentWriter output, AssetSettingsContent value)
        {
            output.Write(value.skyboxModelPath);
            output.Write(value.playerNEG1ModelPath);
            output.Write(value.playerNEG2ModelPath);
            output.Write(value.playerNEG3ModelPath);
            output.Write(value.playerNEG4ModelPath);
            output.Write(value.playerPOS1ModelPath);
            output.Write(value.playerPOS2ModelPath);
            output.Write(value.playerPOS3ModelPath);
            output.Write(value.playerPOS4ModelPath);
            output.Write(value.particlePlus1ModelPath);
            output.Write(value.particlePlus2ModelPath);
            output.Write(value.particlePlus3ModelPath);
            output.Write(value.particlePlus4ModelPath);
            output.Write(value.particleMinus1ModelPath);
            output.Write(value.particleMinus2ModelPath);
            output.Write(value.particleMinus3ModelPath);
            output.Write(value.particleMinus4ModelPath);
            output.Write(value.backgroundScreenTexturePath);
            output.Write(value.audioEnginePath);
            output.Write(value.waveBankPath);
            output.Write(value.soundBankPath);
            output.Write(value.titleThemeCueName);
            output.Write(value.gameWonMusicCueName);
            output.Write(value.gameStartMusicCueName);
            output.Write(value.particleCollectSFXCueName);
            //wallace brown 11/01/09[]
            output.Write(value.PhongFXPath);
            //end Code[]
            output.Write(value.gyro_needle);
            output.Write(value.introLogo);
        }

        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return "WorldWeaver.AssetSettings, WorldWeaverGame";
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "WorldWeaver.AssetSettingsReader, WorldWeaverGame";
        }
    }
}
