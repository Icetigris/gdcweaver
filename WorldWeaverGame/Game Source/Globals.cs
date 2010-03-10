using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace WorldWeaver
{
    public class Globals
    {
        public static bool gameplayScreenDestroyed;
        public static bool gameplayScreenHasFocus;
        public static HUDManager hudManager;
        public static Player Player;
        public static ChaseCamera ChaseCamera;
        public static AssetSettings AssetList;
        public static SoundBank musicSoundBank;
        public static ContentManager contentManager;
        public static SceneGraphManager sceneGraphManager;
        public static GyroPersp gyro;
        public static HudCamera hudCamera;
    }
}
