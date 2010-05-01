using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
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
        public static SolarSystem solarSystem;
        public static Last10ParticlesHUD last10ParticlesHUD;
        public static GameTime gameTime;
        public static float titleDown = 260;
        public static float bodyTextDown = 150;

        //wb
        public static Dictionary<Model, Model> convertedModels = new Dictionary<Model, Model>();
        public static int maxLights = 3;
        public static int numLights = 0;
        public static Lights[] lights;
        public static LinkedList<Star> lightSource;
        //end wb

        //km
        public static int godcharge;
    }
}
