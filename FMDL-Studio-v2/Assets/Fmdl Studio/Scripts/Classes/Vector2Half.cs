using System;

namespace FmdlStudio.Scripts.Classes
{
    public struct Vector2Half
    {
        public Half x;
        public Half y;

        public Vector2Half(Half x, Half y)
        {
            this.x = x;
            this.y = y;
        } //constructor

        public Half this[int i]
        {
            get
            {
                switch (i)
                {
                    case 0:
                        return x;
                    case 1:
                        return y;
                    default:
                        throw new System.IndexOutOfRangeException();
                } //switch
            } //get
            set
            {
                switch (i)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    default:
                        throw new System.IndexOutOfRangeException();
                } //switch
            } //set
        } //indexer
    } //struct
} //namespace