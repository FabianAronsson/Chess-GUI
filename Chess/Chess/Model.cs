using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    public class Model
    {
        private int srcX;

        public int XSrcCoordinate
        {
            get { return srcX; }
            set { srcX = value; }
        }

        private int srcY;

        public int YSrcCoordinate
        {
            get { return srcY; }
            set { srcY = value; }
        }

        private int destX;

        public int DestinationX
        {
            get { return destX; }
            set { destX = value; }
        }
        private int destY;

        public int DestinationY
        {
            get { return destY; }
            set { destY = value; }
        }

        private string FEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        public string FENotation
        {
            get { return FEN; }
            set { FEN = value; }
        }


    }
}
