using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace WorldWeaver
{
    public class HudCamera
    {

        // CONSTRUCTOR
        public HudCamera(Vector3 position, Matrix view, Matrix projection)
        {
            this.chasePosition = position;
            this.view = view;
            this.projection = projection;
        }

        private Vector3 chasePosition; // Position of object being chased.


        #region Matrix properties

        /// <summary>
        /// View transform matrix.
        /// </summary>
        public Matrix View
        {
            get { return view; }
        }
        private Matrix view;

        /// <summary>
        /// Projecton transform matrix.
        /// </summary>
        public Matrix Projection
        {
            get { return projection; }
        }
        private Matrix projection;


        // Return Position
        public Vector3 Position
        {
            get { return chasePosition; }

        }

        #endregion


    }
}
