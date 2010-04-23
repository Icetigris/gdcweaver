using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace AssetPipeline
{
    /// <summary>
    /// Outputs a test XML file when the AssetPipeline is run as an executable
    /// </summary>
    public class AssetSettingsContent
    {
        public string skyboxModelPath = "Models\\purplenebula";
        public string playerModelPath = "Models\\athena";

        public string particlePlus1ModelPath = "Models\\plus1";
        public string particlePlus2ModelPath = "Models\\plus2";
        public string particlePlus3ModelPath = "Models\\plus3";
        public string particlePlus4ModelPath = "Models\\plus4";
        public string particleMinus1ModelPath = "Models\\-1";
        public string particleMinus2ModelPath = "Models\\-2";
        public string particleMinus3ModelPath = "Models\\-3";
        public string particleMinus4ModelPath = "Models\\-4";

        public string backgroundScreenTexturePath = "Models\\Checker";

        public string audioEnginePath = "Content\\Sound\\xactProject.xgs";
        public string waveBankPath = "Content\\Sound\\myWaveBank.xwb";
        public string soundBankPath = "Content\\Sound\\mySoundBank.xsb";

        public string titleThemeCueName = "SoA-Title";
        public string gameWonMusicCueName = "SOA-SkiesInTheAlbatross";
        public string gameStartMusicCueName = "LR_ArcaneMysteries";
        public string particleCollectSFXCueName = "Sonic Ring";

        //wallace brown 11/01/09[]
        public string PhongFXPath = "Effects\\Phong";
        //end Code[]

        public string gyro_needle = "Models\\gyro_hook";
        public string introLogo = "Images\\UMBCGDCLogoscreen";

        //Prints everything that is in this class
        public override string ToString()
        {
            string allTheCrapInThisGame = "";

            allTheCrapInThisGame += skyboxModelPath + "\n";
            allTheCrapInThisGame += playerModelPath + "\n";
            allTheCrapInThisGame += particlePlus1ModelPath + "\n";
            allTheCrapInThisGame += particlePlus2ModelPath + "\n";
            allTheCrapInThisGame += particlePlus3ModelPath + "\n";
            allTheCrapInThisGame += particlePlus4ModelPath + "\n";
            allTheCrapInThisGame += particleMinus1ModelPath + "\n";
            allTheCrapInThisGame += particleMinus2ModelPath + "\n";
            allTheCrapInThisGame += particleMinus3ModelPath + "\n";
            allTheCrapInThisGame += particleMinus4ModelPath + "\n";
            allTheCrapInThisGame += backgroundScreenTexturePath + "\n";
            allTheCrapInThisGame += audioEnginePath + "\n";
            allTheCrapInThisGame += waveBankPath + "\n";
            allTheCrapInThisGame += soundBankPath + "\n";
            allTheCrapInThisGame += titleThemeCueName + "\n";
            allTheCrapInThisGame += gameWonMusicCueName + "\n";
            allTheCrapInThisGame += gameStartMusicCueName + "\n";
            allTheCrapInThisGame += particleCollectSFXCueName + "\n";

            //wallace brown 11/01/09[]
            allTheCrapInThisGame += PhongFXPath + "\n";
            //end Code[]

            gyro_needle += gyro_needle + "\n";
            introLogo += introLogo + "\n";

            return allTheCrapInThisGame;
        }
    }
}
