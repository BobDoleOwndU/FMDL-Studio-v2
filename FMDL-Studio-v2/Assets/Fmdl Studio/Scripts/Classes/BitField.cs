using System;
using UnityEngine;

namespace FmdlStudio
{
    public struct BitField
    {
        private const int MAX_BIT_COUNT = 7; //Size of variable - 1;
        public byte value;

        public bool GetBit(int idx)
        {
            if (idx > MAX_BIT_COUNT || idx < 0)
            {
                throw new IndexOutOfRangeException();
            } //if

            byte temp = (byte)(this.value << MAX_BIT_COUNT - idx);

            return (temp >> MAX_BIT_COUNT == 1);
        } //GetBit

        public void SetBit(int idx, bool value)
        {
            if (idx > MAX_BIT_COUNT || idx < 0)
            {
                throw new IndexOutOfRangeException();
            } //if

            if (value)
            {
                this.value |= (byte)(1 << idx);
            } //if
            else if (GetBit(idx))
            {
                this.value ^= (byte)(1 << idx);
            } //else
        } //SetBit
    } //class
} //namespace