﻿using System;
using PieceClass = AbsoluteZero.Piece;

namespace AbsoluteZero {

    /// <summary>
    /// Provides methods for move encoding and decoding. 
    /// </summary>
    static class Move {

        /// <summary>
        /// The value representing an invalid move.
        /// </summary>
        public const Int32 Invalid = 0;

        /// <summary>
        /// The amount the to square is shifted when encoding the move. 
        /// </summary>
        private const Int32 ToShift = 6;

        /// <summary>
        /// The amount the moving piece is shifted when encoding the move.
        /// </summary>
        private const Int32 PieceShift = ToShift + 6;

        /// <summary>
        /// The amount the captured piece is shifted when encoding the move.
        /// </summary>
        private const Int32 CaptureShift = PieceShift + 4;

        /// <summary>
        /// The amount the special piece is shifted when encoding the move.
        /// </summary>
        private const Int32 SpecialShift = CaptureShift + 4;

        /// <summary>
        /// The mask for extracting the unshifted square from a move.
        /// </summary>
        private const Int32 SquareMask = (1 << 6) - 1;

        /// <summary>
        /// The mask for extracting the unshifted square from a move.
        /// </summary>
        private const Int32 PieceMask = (1 << 4) - 1;

        /// <summary>
        /// The mask for extracting the shifted type of the captured piece from a 
        /// move. 
        /// </summary>
        private const Int32 TypeCaptureShifted = PieceClass.Mask << CaptureShift;

        /// <summary>
        /// The value of the captured empty piece exactly as it is represented in 
        /// the move. 
        /// </summary>
        private const Int32 EmptyCaptureShifted = PieceClass.Empty << CaptureShift;

        /// <summary>
        /// The mask for extracting the shifted type of the special piece from a move. 
        /// </summary>
        private const Int32 TypeSpecialShifted = PieceClass.Mask << SpecialShift;

        /// <summary>
        /// The value of the king special (castling) exactly as it is represented in 
        /// the move. 
        /// </summary>
        private const Int32 KingSpecialShifted = PieceClass.King << SpecialShift;

        /// <summary>
        /// The value of the pawn special (en passant) exactly as it is represented 
        /// in the move. 
        /// </summary>
        private const Int32 PawnSpecialShifted = PieceClass.Pawn << SpecialShift;

        /// <summary>
        /// The value of the queen special (promotion to queen) exactly as it is 
        /// represented in the move. 
        /// </summary>
        private const Int32 QueenSpecialShifted = PieceClass.Queen << SpecialShift;

        /// <summary>
        /// The mask for extracting the shifted type of the moving piece from a move. 
        /// </summary>
        private const Int32 TypePieceShifted = PieceClass.Mask << PieceShift;

        /// <summary>
        /// The value of the moving pawn exactly as it is represented in the move. 
        /// </summary>
        private const Int32 PawnPieceShifted = PieceClass.Pawn << PieceShift;

        /// <summary>
        /// Returns the pawn's move bitboard for the given square as the given 
        /// colour. 
        /// </summary>
        /// <param name="square">The square the pawn is on.</param>
        /// <param name="colour">The colour of the pawn.</param>
        /// <returns>The pawn's move bitboard.</returns>
        public static UInt64 Pawn(Int32 square, Int32 colour) {
            return 1UL << (square + 16 * colour - 8);
        }

        /// <summary>
        /// Returns a move encoded from the given parameters. 
        /// </summary>
        /// <param name="position">The position the move is to be played on.</param>
        /// <param name="from">The from square of the move.</param>
        /// <param name="to">The to square of the move.</param>
        /// <param name="special">The special piece of the move.</param>
        /// <returns>A move encoded from the given parameters.</returns>
        public static Int32 Create(Position position, Int32 from, Int32 to, Int32 special = PieceClass.Empty) {
            return from | (to << ToShift) | (position.Square[from] << PieceShift) | (position.Square[to] << CaptureShift) | (special << SpecialShift);
        }

        /// <summary>
        /// Returns a move to be played on the given position that has the given 
        /// representation in coordinate notation. 
        /// </summary>
        /// <param name="position">The position the move is to be played on.</param>
        /// <param name="name">The representation of the move in coordinate notation.</param>
        /// <returns>A move that has the given representation in coordinate notation.</returns>
        public static Int32 Create(Position position, String name) {
            foreach (Int32 move in position.LegalMoves())
                if (name == Stringify.Move(move))
                    return move;
            return Invalid;
        }

        /// <summary>
        /// Returns the from square of the given move. 
        /// </summary>
        /// <param name="move">The move to decode.</param>
        /// <returns>The from square of the given move.</returns>
        public static Int32 From(Int32 move) {
            return move & SquareMask;
        }

        /// <summary>
        /// Returns the to square of the given move.
        /// </summary>
        /// <param name="move">The move to decode.</param>
        /// <returns>The to square of the given move.</returns>
        public static Int32 To(Int32 move) {
            return (move >> ToShift) & SquareMask;
        }

        /// <summary>
        /// Returns the moving piece of the given move.
        /// </summary>
        /// <param name="move">The move to decode.</param>
        /// <returns>The moving piece of the given move.</returns>
        public static Int32 Piece(Int32 move) {
            return (move >> PieceShift) & PieceMask;
        }

        /// <summary>
        /// Returns the captured piece of the given move.
        /// </summary>
        /// <param name="move">The move to decode.</param>
        /// <returns>The captured piece of the given move.</returns>
        public static Int32 Capture(Int32 move) {
            return (move >> CaptureShift) & PieceMask;
        }
        
        /// <summary>
        /// Returns the special piece of the given move. 
        /// </summary>
        /// <param name="move">The move to decode.</param>
        /// <returns>The special piece of the given move.</returns>
        public static Int32 Special(Int32 move) {
            return move >> SpecialShift;
        }

        /// <summary>
        /// Returns whether the given move captures an opposing piece.
        /// </summary>
        /// <param name="move">The move to decode.</param>
        /// <returns>Whether the given move captures an opposing piece.</returns>
        public static Boolean IsCapture(Int32 move) {
            return (move & TypeCaptureShifted) != EmptyCaptureShifted;
        }

        /// <summary>
        /// Returns whether the given move is a castling move. 
        /// </summary>
        /// <param name="move">The move to decode.</param>
        /// <returns>Whether the given move is a castling move.</returns>
        public static Boolean IsCastle(Int32 move) {
            return (move & TypeSpecialShifted) == KingSpecialShifted;
        }

        /// <summary>
        /// Returns whether the given move promotes a pawn.
        /// </summary>
        /// <param name="move">The move to decode.</param>
        /// <returns>Whether the given mode promotes a pawn.</returns>
        public static Boolean IsPromotion(Int32 move) {
            if ((move & TypePieceShifted) != PawnPieceShifted)
                return false;
            Int32 to = (move >> ToShift) & SquareMask;
            return (to - 8) * (to - 55) > 0;
        }

        /// <summary>
        /// Returns whether the given move is en passant. 
        /// </summary>
        /// <param name="move">The move to decode.</param>
        /// <returns>Whether the given mode is en passant.</returns>
        public static Boolean IsEnPassant(Int32 move) {
            return (move & TypeSpecialShifted) == PawnSpecialShifted;
        }

        /// <summary>
        /// Returns whether the given move is a pawn advance.
        /// </summary>
        /// <param name="move">The move to decode.</param>
        /// <returns>Whether the given move is a pawn advance.</returns>
        public static Boolean IsPawnAdvance(Int32 move) {
            return (move & TypePieceShifted) == PawnPieceShifted;
        }

        /// <summary>
        /// Returns whether the given move promotes a pawn to a queen. 
        /// </summary>
        /// <param name="move">The move to decode.</param>
        /// <returns>Whether the given mode promotes a pawn to a queen.</returns>
        public static Boolean IsQueenPromotion(Int32 move) {
            return (move & TypeSpecialShifted) == QueenSpecialShifted;
        }
    }
}
