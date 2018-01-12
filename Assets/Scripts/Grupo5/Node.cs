using Assets.Scripts.DataStructures;
using Assets.Scripts.Grupo5;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace Assets.Scripts.Grupo5
{
    /// <summary>
    /// Node class.
    /// </summary>
    public class Node
    {
        private Node Father { get; set; }
        private CellInfo cell;
        private Locomotion.MoveDirection movement;

        private float      totalWalkCost = 0;
        private float      distance      = 0;
        private int        horizon       = 0;
        private static int numberNodes   = 0;
        private int        id            = 0;

        /// <summary>
        /// Node constructor.
        /// </summary>
        /// <param name="father">Father of the Node.</param>
        /// <param name="cell">Information of this Node.</param>
        /// <param name="goals">Objectives of this Node.</param>
        public Node(Node father, CellInfo cell, CellInfo[] goals)
        {
            this.id            = numberNodes;
            this.cell          = cell;
            this.Father        = father;
            this.movement      = GetDirection();
            this.totalWalkCost = CalculateWalkCost();
            this.distance      = CalculateDistance(goals);

            if (this.Father == null)
            {
                this.horizon = 1;
            }
            else
            {
                this.horizon = this.Father.horizon + 1;
            }
            numberNodes++;
        }

        /// <summary>
        /// Calculates the acumulative walk cost of a Node.
        /// </summary>
        /// <returns>Acumulative walk cost of a node.</returns>
        private float CalculateWalkCost()
        {
            if (this.Father == null)
            {
                return 0;
            }
            else
            {
                return (this.cell.WalkCost + this.Father.totalWalkCost);
            }  
        }

        /// <summary>
        /// This method expand the sucessors of a Node.
        /// </summary>
        /// <param name="boardInfo">Information of the Board.</param>
        /// <param name="goals">Objectives.</param>
        /// <param name="expandedNodes">Nodes that are already expanded.</param>
        /// <returns></returns>
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
                    if (this.Father == null) //si es el primer nodo
                    {
                        Node aux = new Node(this, cells[i], goals);                      
                        auxList.Add(aux);
                    }
                    else
                    {
                        if (!String.Equals(cells[i].CellId, this.Father.GetCell().CellId))
                        {
                            Node aux = new Node(this, cells[i], goals);
                            auxList.Add(aux);
                        }
                    }
                }
            }
            for (int i = 0; i < auxList.Count; i++)
            {
                bool found = false; 
                for (int j = 0; j < expandedNodes.Count; j++)
                {
                    if (expandedNodes[j].IsEqual(auxList[i]))
                        found = true;
                }
                if (!found)
                {
                    nodes.Add(auxList[i]);
                }
            }
            return nodes;
        }

        /// <summary>
        /// This method calculates Direction depending of the father.
        /// </summary>
        /// <returns>Direction needed to reach that Node.</returns>
        public Locomotion.MoveDirection GetDirection()
        {
            if (this.Father == null)
            {
                return Locomotion.MoveDirection.None;
            }
            else
            {
                int column = this.cell.ColumnId - Father.cell.ColumnId;
                int row    = this.cell.RowId    - Father.cell.RowId;

                if (column == 0)
                {
                    if (row > 0) return Locomotion.MoveDirection.Up;
                    else return  Locomotion.MoveDirection.Down;
                }
                else
                {
                    if (column > 0) return Locomotion.MoveDirection.Right;
                    else return Locomotion.MoveDirection.Left;
                }
            }
        }

        /// <summary>
        /// This method calculates Distance between a Node and the objective.
        /// </summary>
        /// <param name="goals"></param>
        /// <returns></returns>
        public float CalculateDistance(CellInfo[] goals)
        {
            int    column = Math.Abs(this.GetCell().ColumnId - goals[0].ColumnId);
            int    row    = Math.Abs(this.GetCell().RowId    - goals[0].RowId);
            float  dist   = column + row + this.totalWalkCost;
            return dist;
        }

        public float GetDistance()
        {
            return this.distance;
        }

        public override string ToString()
        {
            string text = "Node: " + this.id + ", position (" + this.cell.RowId + ", " + this.cell.ColumnId + "), with movement: " + this.movement + ". Distance: " + this.distance + "\n";
            return text;
        }

        public bool IsEqual(Node node)
        {
            if ((this.cell.ColumnId == node.cell.ColumnId) && (this.cell.RowId == node.cell.RowId))
                return true;
            else
                return false;
        }

        public Node GetFather()
        {
            return this.Father;
        }

        public int GetHorizon()
        {
            return this.horizon;
        }

        public void SetHorizon(int horizon)
        {
            this.horizon = horizon;
        }

        public CellInfo GetCell()
        {
            return cell;
        }

        public Locomotion.MoveDirection GetMovement()
        {
            return movement;
        }
    }
    
}