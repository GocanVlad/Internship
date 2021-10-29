using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe
{
    class Squares
    {
        public Player Owner { get; internal set; }

        public static implicit operator Squares(Square v)
        {
            throw new NotImplementedException();
        }

        public enum Player { None = 0, Circles, Crosses}
        public struct Square
        {
            public Player Owner { get; }
            public Square(Player owner)
            {
                this.Owner = owner;
            }

            public override string ToString()
            {
                switch(Owner)
                {
                    case Player.None:
                        return ".";
                    case Player.Crosses:
                        return "X";
                    case Player.Circles:
                        return "O";
                    default:
                        throw new Exception("Invalid state");
                }
            }
        }
    }
}
