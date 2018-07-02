using System;

public struct Vector4Half
{
    public Half x;
    public Half y;
    public Half z;
    public Half w;

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
                case 2:
                    return z;
                case 3:
                    return w;
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
                case 2:
                    z = value;
                    break;
                case 3:
                    w = value;
                    break;
                default:
                    throw new System.IndexOutOfRangeException();
            } //switch
        } //set
    } //indexer
} //struct
