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
        private float distance = 0;
        private float totalWalkCost;
        private int horizon = 0;
        private static int numberNodes = 0;
        private int id;

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
            this.id = numberNodes;
            this.cell = cell;
            this.father = father;
            this.movement = getDirection();
            this.totalWalkCost = calculateWalkCost();
            this.distance = calculateDistance(goals);
            if (this.father == null)
            {
                this.horizon = 1;
            }
            else
            {
                this.horizon = this.father.horizon + 1;
            }
            
            numberNodes++;
        }

        private float calculateWalkCost()
        {
            if (this.father == null)
            {
                return 0;
            }
            else
            {
                return (this.cell.WalkCost + this.father.totalWalkCost);
            }  
        }

        //Método para expandir los nodos a partir del nodo en el que nos encontramos.
        public List<Node> Expand(BoardInfo boardInfo, CellInfo[] goals, List<Node> expandedNodes)
        {
            List<CellInfo> cells = new List<CellInfo>();
            List<Node> nodes     = new List<Node>();
            List<Node> auxList   = new List<Node>();

            cells.AddRange(this.cell.WalkableNeighbours(boardInfo));
            
            for (int i = 0; i < cells.Count; i++)
            {
                if (cells[i] != null)
                {
                    if (this.father == null) //si es el primer nodo
                    {
                        Node aux = new Node(this, cells[i], goals);                      
                        //Debug.Log(this.ToString());
                        auxList.Add(aux);

                    }
                    else
                    {
                        if (!String.Equals(cells[i].CellId, this.father.getCell().CellId))
                        {
                            Node aux = new Node(this, cells[i], goals);
                            //Debug.Log(this.ToString());
                            auxList.Add(aux);
                        }
                    }
                }
            }
            for (int i = 0; i < auxList.Count; i++)
            {
                bool found = false; //flag para mirar si los sucesores no han sido metidos ya en la lista de nodos expandidos.
                for (int j = 0; j < expandedNodes.Count; j++)
                {
                    if (expandedNodes[j].isEqual(auxList[i]))
                        found = true;
                }
                if (!found)
                {
                    nodes.Add(auxList[i]);
                    expandedNodes.Add(auxList[i]);
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
                    if (column > 0) return Locomotion.MoveDirection.Right;
                    else return Locomotion.MoveDirection.Left;
                }
            }
        }

        //Calculamos el coste de este nodo para poder usarlo en la comparación del sort.
        public float calculateDistance(CellInfo[] goals)
        {
            int column = Math.Abs(this.getCell().ColumnId - goals[0].ColumnId);
            int row = Math.Abs(this.getCell().RowId - goals[0].RowId);
            float dist = column + row;
            dist += this.totalWalkCost;
            return dist;
        }

        public float getDistance()
        {
            return this.distance;
        }

        public override string ToString()
        {
            string text = "Node: " + this.id + ", position (" + this.cell.RowId + ", " + this.cell.ColumnId + "), with movement: " + this.movement + ". Distance: " + this.distance + "\n";
            return text;
        }

        public bool isEqual(Node node)
        {
            if ((this.cell.ColumnId == node.cell.ColumnId) && (this.cell.RowId == node.cell.RowId))
                return true;
            else
                return false;
        }

        public Node getFather()
        {
            return this.father;
        }

        public int GetHorizon()
        {
            return this.horizon;
        }

        public void SetHorizon(int horizon)
        {
            this.horizon = horizon;
        }
    }
    
}