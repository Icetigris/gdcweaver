using System;
using System.Collections.Generic;
using System.Text;

namespace WorldWeaver
{
    public class CountableColour : IComparable
    {
        #region Class Variables
        private int index;
        private Particle.Colours colour;
        #endregion

        #region Properties
        public int Index
        {
            get { return index; }
            set { index = value; }
        }

        public Particle.Colours Colour
        {
            get { return colour; }
            set { colour = value; }
        }
        #endregion

        #region Constructor
        public CountableColour(int index, Particle.Colours colour)
        {
            this.index = index;
            this.colour = colour;
        }
        #endregion

        #region IComparable Members

        public int CompareTo(object obj)
        {
            CountableColour inCC = (CountableColour)obj;
            int returnValue = 0;

            if (this.Index == inCC.Index)
            {
                returnValue = 0;
            }
            else if (this.Index < inCC.Index)
            {
                returnValue = -1;
            }
            else if (this.Index > inCC.Index)
            {
                returnValue = 1;
            }

            return returnValue;
        }

        #endregion
    }
}
