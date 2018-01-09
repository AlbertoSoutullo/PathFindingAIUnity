using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.DataStructures;

namespace Assets.Scripts.Grupo5
{
    class OnLineSearch : AbstractPathMind
    {
        static int numNodosExpandidos = 0;
        public int horizon = 3;
        private int alpha = int.MaxValue;

        private Stack<Locomotion.MoveDirection> movements = null;

        private bool OnLineSearch(Node firstNode, BoardInfo boardInfo, CellInfo[] goals)
        {
            int horizonAux = 0;
            List<Node> nodesToExpand = new List<Node>();
            nodesToExpand.Add(firstNode);

            while (horizonAux < 3)
            {

            }
        }


        public override Locomotion.MoveDirection GetNextMove(BoardInfo boardInfo, CellInfo currentPos, CellInfo[] goals)
        {
            if (this.movements == null)
            {
                Node firstNode = new Node(null, currentPos, goals);

                bool encontrado = OnLineSearch(firstNode, boardInfo, goals);
                if (!encontrado)
                {
                    print("Goal not found. \n");
                }
                //Mirar si no ha encontrado goal.
                else
                {

                }
            }
            if (this.movements.Count == 0)
            {
                return Locomotion.MoveDirection.None;
            }
            else
            {
                return this.movements.Pop();
            }
        }

        public override void Repath()
        {
            
        }
    }
}
