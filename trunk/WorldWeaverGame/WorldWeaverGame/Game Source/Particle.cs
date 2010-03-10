using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace WorldWeaver
{
    class Particle
    {
        public enum Colours
        {
            Red,
            Orange,
            Yellow,
            Green,
            Blue,
            Purple,
            Silver,
            Shiny
        }


        #region Declarations

        Matrix orientation,
             translation,
             finalTransformation;

        private bool isCollected = false;
        private int charge;
        private Colours colour;

        //Subject to change
        private float collectionRadius = 0.0f;
        private float attractionRadius = 10000.0f;

        private BoundingSphere collectionSphere;
        private BoundingSphere attractionSphere;

        public const float e = 0.0000000000000000001602176487f; //elementary charge
        private const float MinimumAltitude = 350.0f;
        private const float MaximumAltitude = 9000.0f;
        
        private Vector3 position;
        public Vector3 Direction;

        // Max particle velocity.
        public float maxVelocity = 0.01f;

        #endregion


        #region Properties

        public bool IsCollected
        {
            get { return isCollected; }
            set { isCollected = value; }
        }

        //This particle's charge, getter, and setter
        public int Charge
        {
            get { return charge; }
        }

        //This particle's colour, getter, and setter
        public Colours Colour
        {
            get { return colour; }
        }

        //This particle's position; setter calculates new transformation matrix
        public Vector3 Position
        {
            get { return position; }
            set
            {
                position = value;
                translation = Matrix.CreateTranslation(position);
                finalTransformation = orientation * translation;

                collectionSphere.Center = position;
                attractionSphere.Center = position;
            }
        }

        public float CollectionRadius
        {
            get { return collectionRadius; }
        }

        public float AttractionRadius
        {
            get { return attractionRadius; }
        }

        public BoundingSphere CollectionSphere
        {
            get { return collectionSphere; }
        }

        public BoundingSphere AttractionSphere
        {
            get { return attractionSphere; }
        }

        #endregion


        #region Constructors

        //empty constructor
        public Particle(){}

        public Particle(int ch, Colours col, Vector3 pos)
        {
            charge = ch;
            colour = col;
            Position = pos;
            Direction = Vector3.Forward;
            orientation = Matrix.Identity;
            finalTransformation = orientation * translation;
            collectionSphere = new BoundingSphere(Position, collectionRadius);
            attractionSphere = new BoundingSphere(Position, attractionRadius);

        }

        #endregion

        #region Methods
        //These are based on the GhostArena3D examples in the book

        public int AssignModel()
        {
            switch (charge)
            {
                case 1:
                    return 0;
                case 2:
                    return 1;
                case 3:
                    return 2;
                case 4:
                    return 3;
                case -1:
                    return 4;
                case -2:
                    return 5;
                case -3:
                    return 6;
                case -4:
                    return 7;
                default:
                    return 0;
            }
        }

        public float CalculateVelocity(int playerCharge, Vector3 direction)
        {
            float newVelocity;
            //F = k_e(q_1*q_2/r*r) = Coulomb's Law; k_e is totally inaccurate (it's 9E9 IRL)
            newVelocity = -900*((playerCharge * charge) / (direction.Length() * direction.Length()));

            return newVelocity;
        }

        public void Move(Vector3 playerPosition, int playerCharge, BoundingSphere playerCollisionSphere)
        {
            if (AttractionSphere.Intersects(playerCollisionSphere))
            {
                Vector3 direction = (playerPosition - position);
                Vector3 vecToTarget = Vector3.Normalize(playerPosition - position);
                Vector3 axisToRotate = Vector3.Cross(direction, vecToTarget);

                double angle = Math.Acos((double)Vector3.Dot(direction, vecToTarget));

                // Avoid divide by zero. This case would occur if the cross product
                // of the vectors equal 0.
                if (axisToRotate == Vector3.Zero)
                {
                    axisToRotate = Vector3.UnitY;
                }
                else
                {
                    axisToRotate.Normalize();
                }

                //turn the particles to face the player
                orientation = Matrix.CreateFromAxisAngle(axisToRotate, (float)angle);
                finalTransformation = orientation * translation;

                //move the particles towards the player
                position += direction * CalculateVelocity(playerCharge, direction); //change to support velocity changes
                translation = Matrix.CreateTranslation(position);
                finalTransformation = orientation * translation;

                // Prevent particle from flying under the ground
                position.Y = Math.Max(Position.Y, MinimumAltitude);
                position.Y = Math.Min(Position.Y, MaximumAltitude);

                //move the centres of the collection and attraction radii with the particle model
                collectionSphere.Center = position;
                attractionSphere.Center = position;
            }
        }

        #endregion
    }
}
