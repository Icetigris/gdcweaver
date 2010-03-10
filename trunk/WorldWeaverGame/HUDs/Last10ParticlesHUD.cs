using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace WorldWeaver
{
    public class Last10ParticlesHUD : HUDElement
    {
        private Texture2D last10InChain;
        private Queue<Particle> lastTen;
        private int TotalParticlesCollected;

        public Last10ParticlesHUD()
        {
            TexturePath = "Textures\\last10particlesinchain";
            lastTen = new Queue<Particle>();
            TotalParticlesCollected = 0;
        }

        public override void LoadContent()
        {
            /*
            if (Content == null)
            {
                Content = new ContentManager(HudManager.Game.Services, "Content");
            }
            */
            last10InChain = Globals.hudManager.Game.Content.Load<Texture2D>(TexturePath);

            Position = new Rectangle((int)((Globals.hudManager.HudAreaWidth - Globals.hudManager.HudAreaWidth * 0.1f) - last10InChain.Width / 2),
                                     (int)((Globals.hudManager.HudAreaHeight - Globals.hudManager.HudAreaHeight * 0.6f) - last10InChain.Height / 2),
                                     last10InChain.Width,
                                     last10InChain.Height);
        }

        public override void UnloadContent()
        {
        }

        public override void Update()
        {
            if (TotalParticlesCollected != Globals.Player.CurrentChain.totalParicles)
            {
                updateChain();
            }
        }

        public void updateChain()
        {
            if (Globals.Player.CurrentChain.totalParicles - TotalParticlesCollected <= Globals.Player.CurrentChain.Chain.Count)
            {
              //  Console.WriteLine("Total collected is " + Globals.Player.CurrentChain.Chain.Count);
                while (TotalParticlesCollected < Globals.Player.CurrentChain.totalParicles)
                {
                    lastTen.Enqueue(Globals.Player.CurrentChain.Chain[Globals.Player.CurrentChain.Chain.Count - (Globals.Player.CurrentChain.totalParicles - TotalParticlesCollected)]);

                    if (lastTen.Count > 10)
                    {
                        lastTen.Dequeue();

                    }

                    TotalParticlesCollected++;
                }

                for (int i = 0; i < lastTen.Count; i++)
                {
                    lastTen.ElementAt(i).Position = new Vector3(3000, ( (lastTen.Count - (i + 1) ) * -500) + 1700, -7000);
                }

              //  Console.WriteLine("ParticlesHUD # is " + lastTen.Count);
              //  Console.WriteLine("SECOND Chain found " + Globals.Player.CurrentChain.Chain.Count);
            }
            //for (int i = 0; i < 10 && i < Globals.Player.CurrentChain.Chain.Count; i++)
            //{
            //    if (lastTen[i] != null)
            //    {
            //        SceneGraphManager.RemoveObject(lastTen[i].MySceneIndex);
            //    }

            //    lastTen[i] = Globals.Player.CurrentChain.Chain[i];
            //    lastTen[i].MySceneIndex = SceneGraphManager.SceneCount;
            //    SceneGraphManager.AddObject(lastTen[i]);

            //    lastTen[i].IsVisible = true;

            //    lastTen[i].ParticleWorld = Matrix.CreateTranslation(0, 0, 0);
            //   // lastTen[i].ParticleWorld.Translation; // ParticleWorld.Translation = new Vector3(350, -400, -1500);

            //    Console.WriteLine("Collected Object Index is " + lastTen[i].MySceneIndex);
            //}

            //TotalParticlesCollected = Globals.Player.CurrentChain.totalParicles;
        }

        public override void Draw(GameTime gameTime)
        {
            Globals.hudManager.SpriteBatch.Draw(last10InChain, Position, Color.White);

            //I apologize for this horrible hack to get the particles to be drawn after the HUD
            Globals.hudManager.SpriteBatch.End();

            DrawParticles(gameTime);

            Globals.hudManager.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend,
                             SpriteSortMode.Immediate,
                             SaveStateMode.SaveState);
        }

        //yay crappy solution
        public void DrawParticles(GameTime gameTime)
        {
            foreach (Particle p in lastTen)
            {
                p.Draw(gameTime);
            }
        }
    }
}
