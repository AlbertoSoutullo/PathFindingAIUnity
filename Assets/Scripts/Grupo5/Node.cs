using Assets.Scripts.DataStructures;
using Assets.Scripts.Grupo5;
using UnityEngine;
using System.Collections.Generic;


namespace Assets.Scripts.Grupo5
{

    public class Node
    {
        private Node father { get; set; }
        private CellInfo cell;
        private Locomotion.MoveDirection movement;

        public CellInfo getCell()
        {
            return cell;
        }

        public Locomotion.MoveDirection getMovement()
        {
            return movement;
        }


        //Constructor de nodo, donde deberíamos pasarle el padre y los datos, que sería la informacion
        //de la celda en la que está el personaje
        public Node(Node father, CellInfo cell)
        {
            this.cell = cell;
            this.father = father;
            this.movement = getDirection();
        }

        //Método para expandir los nodos a partir del nodo en el que nos encontramos.
        public List<Node> Expand(BoardInfo boardInfo)
        {
            List<CellInfo> cells = new List<CellInfo>();
            List<Node> nodes = new List<Node>();

            for (int i = 0; i < 4; i++)
            {
                cells.Add(this.cell.WalkableNeighbours(boardInfo)[i]);
                if (cells[i] != father.getCell())
                {

                    Node aux = new Node(this, cells[i]);
                    nodes.Add(aux);
                }
            }
            return nodes;
        }

        public Locomotion.MoveDirection  getDirection()
        {
            int column = this.cell.ColumnId - father.cell.ColumnId;
            int row    = this.cell.RowId - father.cell.RowId;

            if (column == 0)
            {
                if (row > 0) return Locomotion.MoveDirection.Up;
                else return Locomotion.MoveDirection.Down;
            }
            else 
            {
                if (column > 0) return Locomotion.MoveDirection.Left;
                else return Locomotion.MoveDirection.Right;
            }

        }

    }
    
}