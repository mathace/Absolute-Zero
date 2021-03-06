﻿using System;
using System.Text;

namespace AbsoluteZero {

    /// <summary>
    /// Provides methods and constants for bitwise operations. 
    /// </summary>
    static class Bit {

        /// <summary>
        /// The collection of bitboard files for a given square. File[s] gives the a 
        /// bitboard of the squares along the file of square s. 
        /// </summary>
        public static readonly UInt64[] File = new UInt64[64];

        /// <summary>
        /// The collection of bitboard ranks for a given square. Rank[s] gives the a 
        /// bitboard of the squares along the rank of square s. 
        /// </summary>
        public static readonly UInt64[] Rank = new UInt64[64];

        /// <summary>
        /// The collection of bitboard rays pointing to the north of a given square. 
        /// RayN[s] gives a bitboard of the ray of squares strictly to the north of 
        /// square s. 
        /// </summary>
        public static readonly UInt64[] RayN = new UInt64[64];

        /// <summary>
        /// The collection of bitboard rays pointing to the east of a given square. 
        /// RayN[s] gives a bitboard of the ray of squares strictly to the east of 
        /// square s. 
        /// </summary>
        public static readonly UInt64[] RayE = new UInt64[64];

        /// <summary>
        /// The collection of bitboard rays pointing to the south of a given square. 
        /// RayN[s] gives a bitboard of the ray of squares strictly to the south of 
        /// square s. 
        /// </summary>
        public static readonly UInt64[] RayS = new UInt64[64];

        /// <summary>
        /// The collection of bitboard rays pointing to the west of a given square. 
        /// RayN[s] gives a bitboard of the ray of squares strictly to the west of 
        /// square s. 
        /// </summary>
        public static readonly UInt64[] RayW = new UInt64[64];

        /// <summary>
        /// The collection of bitboard rays pointing to the northeast of a given 
        /// square. RayN[s] gives a bitboard of the ray of squares strictly to the 
        /// northeast of square s. 
        /// </summary>
        public static readonly UInt64[] RayNE = new UInt64[64];

        /// <summary>
        /// The collection of bitboard rays pointing to the northwest of a given 
        /// square. RayN[s] gives a bitboard of the ray of squares strictly to the 
        /// northwest of square s. 
        /// </summary>
        public static readonly UInt64[] RayNW = new UInt64[64];

        /// <summary>
        /// The collection of bitboard rays pointing to the southeast of a given 
        /// square. RayN[s] gives a bitboard of the ray of squares strictly to the 
        /// southeast of square s. 
        /// </summary>
        public static readonly UInt64[] RaySE = new UInt64[64];

        /// <summary>
        /// The collection of bitboard rays pointing to the southwest. RayN[s] gives 
        /// a bitboard of the ray of squares strictly to the southwest of square s. 
        /// </summary>
        public static readonly UInt64[] RaySW = new UInt64[64];

        /// <summary>
        /// The collection of horizontal and vertical bitboard rays extending from a 
        /// given square. Axes[s] gives a bitboard of the squares on the same rank 
        /// and file as square s but does not include s itself. 
        /// </summary>
        public static readonly UInt64[] Axes = new UInt64[64];

        /// <summary>
        /// The collection of diagonal bitboard rays extending from a given square. 
        /// Diagonals[s] gives a bitboard of the squares along the diagonals of 
        /// square s but does not include s itself. 
        /// </summary>
        public static readonly UInt64[] Diagonals = new UInt64[64];

        /// <summary>
        /// The bitboard of all light squares. 
        /// </summary>
        public const UInt64 LightSquares = 0xAA55AA55AA55AA55UL;

        /// <summary>
        /// The collection of indices for calculating the index of a single bit. 
        /// </summary>
        private static readonly Int32[] BitIndex = new Int32[64];

        /// <summary>
        /// Initializes lookup tables. 
        /// </summary>
        static Bit() {

            for (Int32 square = 0; square < 64; square++) {

                // Initialize file and rank bitboard tables. 
                File[square] = LineFill(Position.File(square), 0, 1);
                Rank[square] = LineFill(Position.Rank(square) * 8, 1, 0);

                // Initialize ray tables. 
                RayN[square] = Bit.LineFill(square, 0, -1) ^ (1UL << square);
                RayE[square] = Bit.LineFill(square, 1, 0) ^ (1UL << square);
                RayS[square] = Bit.LineFill(square, 0, 1) ^ (1UL << square);
                RayW[square] = Bit.LineFill(square, -1, 0) ^ (1UL << square);
                RayNE[square] = Bit.LineFill(square, 1, -1) ^ (1UL << square);
                RayNW[square] = Bit.LineFill(square, -1, -1) ^ (1UL << square);
                RaySE[square] = Bit.LineFill(square, 1, 1) ^ (1UL << square);
                RaySW[square] = Bit.LineFill(square, -1, 1) ^ (1UL << square);
                Axes[square] = RayN[square] | RayE[square] | RayS[square] | RayW[square];
                Diagonals[square] = RayNE[square] | RayNW[square] | RaySE[square] | RaySW[square];
            }

            // Initialize bit index table. 
            for (Int32 i = 0; i < 64; i++)
                BitIndex[((1UL << i) * 0x07EDD5E59A4E28C2UL) >> 58] = i;

        }

        /// <summary>
        /// Removes and returns the index of the least significant set bit in the 
        /// given bitboard.  
        /// </summary>
        /// <param name="bitboard">The bitboard to pop.</param>
        /// <returns>The index of the least significant set bit.</returns>
        public static Int32 Pop(ref UInt64 bitboard) {
            UInt64 isolatedBit = bitboard & (0UL - bitboard);
            bitboard &= bitboard - 1;
            return BitIndex[(isolatedBit * 0x07EDD5E59A4E28C2UL) >> 58];
        }

        /// <summary>
        /// Returns the index of the bit in a bitboard with a single set bit. 
        /// </summary>
        /// <param name="bitboard">The bitboard to read.</param>
        /// <returns>The index of the single set bit.</returns>
        public static Int32 Read(UInt64 bitboard) {
            return BitIndex[(bitboard * 0x07EDD5E59A4E28C2UL) >> 58];
        }

        /// <summary>
        /// Returns the index of the least significant set bit in the given 
        /// bitboard.
        /// </summary>
        /// <param name="bitboard">The bitboard to scan.</param>
        /// <returns>The index of the least significant set bit.</returns>
        public static Int32 Scan(UInt64 bitboard) {
            return BitIndex[((bitboard & (0UL - bitboard)) * 0x07EDD5E59A4E28C2UL) >> 58];
        }

        /// <summary>
        /// Returns the index of the most significant set bit in the given bitboard.
        /// </summary>
        /// <param name="bitboard">The bitboard to scan.</param>
        /// <returns>The index of the most significant set bit.</returns>
        public static Int32 ScanReverse(UInt64 bitboard) {
            Int32 result = 0;
            if (bitboard > 0xFFFFFFFF) {
                bitboard >>= 32;
                result = 32;
            }
            if (bitboard > 0xFFFF) {
                bitboard >>= 16;
                result += 16;
            }
            if (bitboard > 0xFF) {
                bitboard >>= 8;
                result += 8;
            }
            if (bitboard > 0xF) {
                bitboard >>= 4;
                result += 4;
            }
            if (bitboard > 0x3) {
                bitboard >>= 2;
                result += 2;
            }
            if (bitboard > 0x1)
                result++;
            return result;
        }

        /// <summary>
        /// Returns the number of set bits in the given bitboard.
        /// </summary>
        /// <param name="bitboard">The bitboard to count.</param>
        /// <returns>The number of set bits.</returns>
        public static Int32 Count(UInt64 bitboard) {
            bitboard -= (bitboard >> 1) & 0x5555555555555555UL;
            bitboard = (bitboard & 0x3333333333333333UL) + ((bitboard >> 2) & 0x3333333333333333UL);
            return (Int32)(((bitboard + (bitboard >> 4) & 0xF0F0F0F0F0F0F0FUL) * 0x101010101010101UL) >> 56);
        }

        /// <summary>
        /// Returns the number of set bits in the given bitboard. For a bitboard 
        /// with very few set bits this may be faster than Bit.Count(). 
        /// </summary>
        /// <param name="bitboard">The bitboard to count.</param>
        /// <returns>The number of set bits.</returns>
        public static Int32 CountSparse(UInt64 bitboard) {
            Int32 count = 0;
            while (bitboard != 0) {
                count++;
                bitboard &= bitboard - 1;
            }
            return count;
        }

        /// <summary>
        /// Returns a bitboard that gives the result of performing a floodfill from 
        /// a given index for a given distance. 
        /// </summary>
        /// <param name="index">The index to floodfill from.</param>
        /// <param name="distance">The distance to floodfill.</param>
        /// <returns>A bitboard that is the result of the floodfill.</returns>
        public static UInt64 FloodFill(Int32 index, Int32 distance) {
            if (distance < 0 || index < 0 || index > 63)
                return 0;
            UInt64 bitboard = 1UL << index;
            bitboard |= FloodFill(index + 8, distance - 1);
            bitboard |= FloodFill(index - 8, distance - 1);
            if (Math.Floor(index / 8F) == Math.Floor((index + 1) / 8F))
                bitboard |= FloodFill(index + 1, distance - 1);
            if (Math.Floor(index / 8F) == Math.Floor((index - 1) / 8F))
                bitboard |= FloodFill(index - 1, distance - 1);
            return bitboard;
        }

        /// <summary>
        /// Returns a bitboard that has set bits along a given line.  
        /// </summary>
        /// <param name="index">A point on the line.</param>
        /// <param name="dx">The x component of the line's direction vector.</param>
        /// <param name="dy">The y component of the line's direction vector.</param>
        /// <returns>The bitboard that is the result of the line fill.</returns>
        public static UInt64 LineFill(Int32 index, Int32 dx, Int32 dy) {
            if (index < 0 || index > 63)
                return 0;
            UInt64 bitboard = 1UL << index;
            if (Math.Floor(index / 8F) == Math.Floor((index + dx) / 8F))
                bitboard |= LineFill(index + dx + dy * 8, dx, dy);
            return bitboard;

        }

        /// <summary>
        /// Returns a string giving the binary representation of the move.
        /// </summary>
        /// <param name="x">The move to convert.</param>
        /// <returns>The binary representation of the move.</returns>
        public static String ToString(Int32 move) {
            Char[] sequence = new Char[32];
            for (Int32 i = sequence.Length - 1; i >= 0; i--) {
                sequence[i] = (Char)((move & 1) + 48);
                move >>= 1;
            }
            return new String(sequence);
        }

        /// <summary>
        /// Returns a string giving the binary representation of the bitboard.
        /// </summary>
        /// <param name="bitboard">The bitboard to convert.</param>
        /// <returns>The binary representation of the bitboard.</returns>
        public static String ToString(UInt64 bitboard) {
            Char[] chars = new Char[64];
            for (Int32 i = chars.Length - 1; i >= 0; i--) {
                chars[i] = (Char)((bitboard & 1) + 48);
                bitboard >>= 1;
            }
            return new String(chars);
        }

        /// <summary>
        /// Returns a string giving the binary representation of the bitboard with 
        /// appropriate line terminating characters. The result is a 8 by 8 matrix. 
        /// </summary>
        /// <param name="bitboard">The bitboard to convert</param>
        /// <returns>The binary representation of the bitboard in a matrix format.</returns>
        public static String ToMatrix(UInt64 bitboard) {
            StringBuilder sb = new StringBuilder(78);
            Int32 file = 0;
            for (Int32 i = 0; i < 71; i++)
                if (++file > 8) {
                    sb.Append(Environment.NewLine);
                    file = 0;
                } else {
                    sb.Append(bitboard & 1);
                    bitboard >>= 1;
                }
            return sb.ToString();

        }
    }
}
