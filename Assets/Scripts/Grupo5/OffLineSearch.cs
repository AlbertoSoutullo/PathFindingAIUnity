using Assets.Scripts.SampleMind;
using Assets.Scripts.DataStructures;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace Assets.Scripts.Grupo5
{
    public class OffLineSearch : AbstractPathMind
    {

        private Stack<Locomotion.MoveDirection> movements = null;


        /*Creamos*/
        private bool aStarAlgorithm(Node actualNode, BoardInfo boardInfo, CellInfo[] goals)
        {
            List<Node> decendents = new List<Node>();

            for (int i = 0; i < goals.Length; i++)
            {
                if (actualNode.getCell() == goals[i])
                {
                    this.movements.Push(actualNode.getMovement());
                    return true;
                }
            }

            decendents = actualNode.Expand(boardInfo);
            //Ordenamos
            decendents = sortDecendents(decendents,goals);

            if (aStarAlgorithm())

            
        }

        public override void Repath()
        {

        }


        public override Locomotion.MoveDirection GetNextMove(BoardInfo boardInfo, CellInfo currentPos, CellInfo[] goals)
        {

            if (this.movements == null)
            {
                Node firstNode = new Node(null, currentPos);
                this.movements = new Stack<Locomotion.MoveDirection>();
                this.movements = aStarAlgorithm(firstNode, boardInfo, goals);
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

        //Metodo para ordenar el array de nodos.
        private List<Node> sortDecendents(List<Node> Nodes, CellInfo[] goal)

        {
            int[] numeros = new int[Nodes.Count];
            for(int i=0; i< Nodes.Count; i++)
            {
                int column = Math.Abs(Nodes[i].getCell().ColumnId - goal[0].ColumnId);
                int row = Math.Abs(Nodes[i].getCell().RowId - goal[0].RowId);
                int dist = column + row;
                numeros[i] = dist;
            }

            
            return null;
        }
    }
}