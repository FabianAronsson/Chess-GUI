using Chess.PieceFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    public class MoveModel
    {
        public MoveModel(Rook rook, int y, int x)
        {
            //values are set up to start checking the northern direction first, it then follows the switch statement in order for each direction
            TempY = y;
            TempX = x;
            YOffset = -1;
            XOffset = 0;
            ActiveDirection = y;
            IsPositiveOperation = false;
            MaxValue = -1;
            DirValue = -1;
            N = -1;

        }
        public MoveModel(Bishop bishop, int y, int x)
        {
            //the values are set up looking at the northwestern way, afterwards it follows the switch statement in each direction for a bishop
             TempY = y;
             TempX = x;
             YOffset = -1;
             XOffset = -1;
             ActiveDirectionY = y;
             ActiveDirectionX = x;
             MaxValueX = -1;
             MaxValueY = -1;
             DirValueY = -1;
             DirValueX = -1;
             NY = -1;
             NX = -1;
        }

        public MoveModel(Knight knight, int y, int x)
        {
            //values are set up to start checking the northern direction first, it then follows the switch statement in order for each direction
            TempY = y;
            TempX = x;
            YOffset = -2;
            XOffset = -1;
            MaxValueY = -1;
            MaxValueX = -1;
            NewMaxValueY = -1;
            NewMaxValueX = 8;
            NY = 1;
            NX = -1;
        }

        public MoveModel(Pawn pawn, int y, int x, bool isBlack)
        {
            if (isBlack)
            {//check diagonal moves, not working properly currently.
                TempY = y;
                TempX = x;
                YOffset = 1;
                XOffset = 0;
                MaxValueY = 8;
                MaxValueX = 8;
                DirValueY = 1;
                DirValueX = 0;
                NY = 1;
                NX = 1;
                ActiveDirectionY = y;
                ActiveDirectionX = x;
                CanDoubleMove = true;
            }
            else
            {
                TempY = y;
                TempX = x;
                YOffset = -1;
                XOffset = 0;
                MaxValueY = -1;
                MaxValueX = 8;
                DirValueY = -1;
                DirValueX = 0;
                NY = -1;
                NX = 1;
                ActiveDirectionY = y;
                ActiveDirectionX = x;
                CanDoubleMove = true;
            }
            
        }

        public int TempY { get; set; }
        public int TempX { get; set; }
        public int YOffset { get; set; }
        public int XOffset { get; set; }
        public int ActiveDirection { get; set; }
        public int ActiveDirectionY { get; set; }
        public int ActiveDirectionX { get; set; }
        public bool IsPositiveOperation { get; set; }
        public int MaxValue { get; set; }
        public int MaxValueY { get; set; }
        public int MaxValueX { get; set; }
        public int NewMaxValueY { get; set; }
        public int NewMaxValueX { get; set; }
        public int DirValue { get; set; }
        public int DirValueY { get; set; }
        public int DirValueX { get; set; }
        public int N { get; set; }
        public int NY { get; set; }
        public int NX { get; set; }
        public bool CanDoubleMove { get; set; }

    }
}
