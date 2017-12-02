using Assets.Scripts.DataStructures;
using UnityEngine;

namespace Assets.Scripts.Grupo5
{
    public class OffLineSearch : AbstractPathMind
    {


        public override void Repath()
        {

        }


        public override Locomotion.MoveDirection GetNextMove(BoardInfo boardInfo, CellInfo currentPos, CellInfo[] goals)
        {
            return Locomotion.MoveDirection.Right;
           
        }
    }
}