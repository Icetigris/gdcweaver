using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace WorldWeaver
{
    public class Globals
    {
        public static bool gameplayScreenDestroyed;
        public static bool gameplayScreenHasFocus;
        public static bool cleansedGalaxy;
        public static int numStars;
        public static HUDManager hudManager;
        public static Player Player;
        public static ChaseCamera ChaseCamera;
        public static AssetSettings AssetList;
        public static SoundBank musicSoundBank;
        public static ContentManager contentManager;
        public static SceneGraphManager sceneGraphManager;
        public static GyroPersp gyro;
        public static HudCamera hudCamera;
        public static bool DEBUG = false;

        public static Last10ParticlesHUD last10ParticlesHUD;
        //km
        public static int godcharge;
    }
}
