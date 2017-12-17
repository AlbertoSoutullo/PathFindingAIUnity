using Assets.Scripts.DataStructures;
using Assets.Scripts.Grupo5;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace Assets.Scripts.Grupo5
{

    public class Node
    {
        //En los nodos guardamos la dirección del padre, la información de la celda, y el movimiento que
        //deberíamos tomar para llegar a goal.
        //Esta bíen planteado tener que cambiar la clase nodo de la práctica 1 a la 2?
        private Node father { get; set; }
        private CellInfo cell;
        private Locomotion.MoveDirection movement;
        private int distance;
        private static int numberNodes = 0;

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
        public Node(Node father, CellInfo cell, CellInfo[] goals)
        {
            numberNodes++;
            this.cell = cell;
            this.father = father;
            this.movement = getDirection();
            this.distance = calculateDistance(goals);
            Debug.Log("Node " + numberNodes + "created. \n");
        }

        //Método para expandir los nodos a partir del nodo en el que nos encontramos.
        public List<Node> Expand(BoardInfo boardInfo, CellInfo[] goals)
        {
            List<CellInfo> cells = new List<CellInfo>();
            List<Node> nodes = new List<Node>();

            for (int i = 0; i < 4; i++)
            {
                cells.Add(this.cell.WalkableNeighbours(boardInfo)[i]);
                if (cells[i] != father.getCell())
                {
                    Node aux = new Node(this, cells[i], goals);
                    nodes.Add(aux);
                }
            }
            return nodes;
        }

        public Locomotion.MoveDirection getDirection()
        {
            //Si el padre es null, osea, si es el primer nodo, no tendrá movimiento.
            if (this.father == null)
            {
                return Locomotion.MoveDirection.None;
            }
            else
            {
                int column = this.cell.ColumnId - father.cell.ColumnId;
                int row = this.cell.RowId - father.cell.RowId;

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

        //Calculamos el coste de este nodo para poder usarlo en la comparación del sort.
        public int calculateDistance(CellInfo[] goals)
        {
            int column = Math.Abs(this.getCell().ColumnId - goals[0].ColumnId);
            int row = Math.Abs(this.getCell().RowId - goals[0].RowId);
            int dist = column + row;

            return dist;
        }

        public int getDistance()
        {
            return this.distance;
        }

    }
    
}