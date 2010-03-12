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
        public const int TOTAL_PARTICLES_TO_DISPLAY = 10;
        public const int CURVE_SCALE = 15;
        public const float TOTAL_SECONDS_TO_ANIMATE = 0.3f;
        private Texture2D last10InChain;
        private Queue<Particle> lastTen;
        private List<Particle> particlesToAnimate;
        private int TotalParticlesCollected;
        private Texture2D twoDparticle;
        private string twoDparticleString;
        private float[] calculatedCurve;
        private Vector2[] targetLocation;

        public Last10ParticlesHUD()
        {
            TexturePath = "Textures\\last10particlesinchain";
            twoDparticleString = "Textures\\largeparticle";
            lastTen = new Queue<Particle>();
            particlesToAnimate = new List<Particle>();
            TotalParticlesCollected = 0;
            calculatedCurve = new float[TOTAL_PARTICLES_TO_DISPLAY];
            this.CalculateCurve();
            targetLocation = new Vector2[TOTAL_PARTICLES_TO_DISPLAY];
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
            twoDparticle = Globals.hudManager.Game.Content.Load<Texture2D>(twoDparticleString);

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
            updateChain();
        }

        public void updateChain()
        {
            if (TotalParticlesCollected != Globals.Player.CurrentChain.Chain.Count)
            {
                List<Particle> tempParticleList = lastTen.ToList();
                foreach (Particle p in tempParticleList)
                {
                    particlesToAnimate.Add(p);
                }

                int numNewParticles = Math.Max(0, Globals.Player.CurrentChain.Chain.Count - TotalParticlesCollected);

                TotalParticlesCollected = 0;
                lastTen.Clear();

                int arrayOffset = Math.Max(0, Globals.Player.CurrentChain.Chain.Count - TOTAL_PARTICLES_TO_DISPLAY);

                while (TotalParticlesCollected < Globals.Player.CurrentChain.Chain.Count && TotalParticlesCollected < TOTAL_PARTICLES_TO_DISPLAY)
                {

                    lastTen.Enqueue(Globals.Player.CurrentChain.Chain[TotalParticlesCollected + arrayOffset]);

                    if (TotalParticlesCollected <= TotalParticlesCollected - numNewParticles)
                    {
                        lastTen.ElementAt(TotalParticlesCollected).Position = new Vector3(this.calculateEndLocation(0), 0);
                    }

                    TotalParticlesCollected++;
                }

                for (int i = 0; i < lastTen.Count; i++)
                {
                    targetLocation[i] = calculateEndLocation(lastTen.Count - (i + 1));
                }

                TotalParticlesCollected = Globals.Player.CurrentChain.Chain.Count;

                //do a compare to see what particles are no longer in there
                foreach (Particle p in lastTen)
                {
                    particlesToAnimate.Remove(p);
                }
            }


        }

        private Vector2 calculateEndLocation(float index)
        {
            int i = (int)index;
            int haxyMultiplier = (i > 8) ? -5 : 1;
            return new Vector2(this.Position.X + 50 + (haxyMultiplier * (calculatedCurve[i] * CURVE_SCALE)), this.Position.Y + 120 + (i * twoDparticle.Height));
        }

        public override void Draw(GameTime gameTime)
        {
            Globals.hudManager.SpriteBatch.Draw(last10InChain, Position, Color.White);
            DrawParticles(gameTime);

        }

        //yay crappy solution
        public void DrawParticles(GameTime gameTime)
        {
            //Update code here because of easy access to gameTime
            for (int i = 0; i < lastTen.Count; i++)
            {
                lastTen.ElementAt(i).Position += new Vector3((float)((targetLocation[i].X - lastTen.ElementAt(i).Position.X) * gameTime.ElapsedGameTime.TotalSeconds / TOTAL_SECONDS_TO_ANIMATE), (float)((targetLocation[i].Y - lastTen.ElementAt(i).Position.Y) * gameTime.ElapsedGameTime.TotalSeconds / TOTAL_SECONDS_TO_ANIMATE), 0);
                //lastTen.ElementAt(i).Position = new Vector3(targetLocation[i], 0);
            }

            //TODO: find out how to grab the screen width
            int screenWidth = 1024;
            for (int i = 0; i < particlesToAnimate.Count; i++)
            {
                particlesToAnimate[i].Position += new Vector3((float)(screenWidth * (gameTime.ElapsedGameTime.TotalSeconds / TOTAL_SECONDS_TO_ANIMATE)), 0, 0);

                if (particlesToAnimate[i].Position.X > screenWidth)
                {
                    particlesToAnimate.RemoveAt(i);
                    i--;
                }
            }

            foreach (Particle p in particlesToAnimate)
            {
                Globals.hudManager.SpriteBatch.Draw(twoDparticle, new Vector2(p.Position.X, p.Position.Y), this.convertParticleColorTo(p));
            }

            foreach (Particle p in lastTen)
            {
                Globals.hudManager.SpriteBatch.Draw(twoDparticle, new Vector2(p.Position.X, p.Position.Y), this.convertParticleColorTo(p));
            }
        }

        public Color convertParticleColorTo(Particle p)
        {
            Color aColour = Color.White;

            switch (p.Colour)
            {
                case Particle.Colours.Red:
                    aColour = Color.Red;
                    break;
                case Particle.Colours.Orange:
                    aColour = Color.Orange;
                    break;
                case Particle.Colours.Yellow:
                    aColour = Color.Yellow;
                    break;
                case Particle.Colours.Green:
                    aColour = Color.Green;
                    break;
                case Particle.Colours.Blue:
                    aColour = Color.Blue;
                    break;
                case Particle.Colours.Purple:
                    aColour = Color.Purple;
                    break;
                case Particle.Colours.Silver:
                    aColour = Color.Silver;
                    break;
            }

            return aColour;
        }

        private void CalculateCurve()
        {
            float increment = (float)(3.5 * Math.PI) / TOTAL_PARTICLES_TO_DISPLAY;
            float increments = 0;

            for (int i = 0; i < TOTAL_PARTICLES_TO_DISPLAY; i++)
            {
                this.calculatedCurve[i] = (float)Math.Sin(increments);
                increments += increment;
            }
        }
    }
}
