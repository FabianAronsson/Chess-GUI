using Chess.PieceFactory;

namespace Chess
{
    public class MoveModel
    {
        public MoveModel(Rook rook, int y, int x)
        {
            //Values are set up to start checking the northern direction first.
            TempY = y; //The current y value 
            TempX = x; //The current x value 
            YOffset = -1; //By how much in the y direction TempY should change
            XOffset = 0; //By how much in the x direction TempX should change
            ActiveDirection = y; //Which direction should change currently
            IsPositiveOperation = false; //Whether or not greater than or less than should be used
            MaxValue = -1; //The maximum value on the board in z direction
            DirValue = -1; //In what direction and how much it should change
            N = -1; //An constant that changes specific values, like TempY and TempX
            XrayY = YOffset; //How much the piece's XrayY should increase with, aka where the piece is xraying the opposite king.
            XrayX = XOffset; //- || -

        }
        public MoveModel(Bishop bishop, int y, int x)
        {
            //Values are set up looking at the northwestern way first.
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
            XrayY = YOffset;
            XrayX = XOffset;
        }

        public MoveModel(Knight knight, int y, int x)
        {
            //Values are set up to start checking the northern direction first.
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
            //Values are setup to check lateral directions first
            if (isBlack)
            {
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

        public int XrayX { get; set; }
        public int XrayY { get; set; }
    }
}
