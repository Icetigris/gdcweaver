﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorldWeaver
{
    public class AssetSettings
    {
        public string skyboxModelPath;
        public string playerNEG1ModelPath;
        public string playerNEG2ModelPath;
        public string playerNEG3ModelPath;
        public string playerNEG4ModelPath;
        public string playerPOS1ModelPath;
        public string playerPOS2ModelPath;
        public string playerPOS3ModelPath;
        public string playerPOS4ModelPath;

        public string particlePlus1ModelPath;
        public string particlePlus2ModelPath;
        public string particlePlus3ModelPath;
        public string particlePlus4ModelPath;
        public string particleMinus1ModelPath;
        public string particleMinus2ModelPath;
        public string particleMinus3ModelPath;
        public string particleMinus4ModelPath;

        public string backgroundScreenTexturePath;

        public string audioEnginePath;
        public string waveBankPath;
        public string soundBankPath;

        public string titleThemeCueName;
        public string gameWonMusicCueName;
        public string gameStartMusicCueName;
        public string particleCollectSFXCueName;
        public string gyro_needle;
        public string introLogo;

        //wallace brown 11/01/09
        public string PhongFXPath;
        //end Code[]

        //Prints everything that is in this class
        public override string ToString()
        {
            string allTheCrapInThisGame = "";

            allTheCrapInThisGame += skyboxModelPath + "\n";
            allTheCrapInThisGame += playerNEG1ModelPath + "\n";
            allTheCrapInThisGame += playerNEG2ModelPath + "\n";
            allTheCrapInThisGame += playerNEG3ModelPath + "\n";
            allTheCrapInThisGame += playerNEG4ModelPath + "\n";
            allTheCrapInThisGame += playerPOS1ModelPath + "\n";
            allTheCrapInThisGame += playerPOS2ModelPath + "\n";
            allTheCrapInThisGame += playerPOS3ModelPath + "\n";
            allTheCrapInThisGame += playerPOS4ModelPath + "\n";
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

            //wallace brown 11/01/09
            allTheCrapInThisGame += PhongFXPath + "\n";
            //end Code[]
            allTheCrapInThisGame += gyro_needle + "\n";
            allTheCrapInThisGame += introLogo + "\n";

            return allTheCrapInThisGame;
        }
    }
}
