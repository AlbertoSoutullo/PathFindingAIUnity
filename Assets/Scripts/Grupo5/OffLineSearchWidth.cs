using Assets.Scripts.SampleMind;
using Assets.Scripts.DataStructures;
using Assets.Scripts.Grupo5;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace Assets.Scripts.Grupo5
{
    /// <summary>
    /// This Script will perform a full Width Search.
    /// </summary>
    class OffLineSearchWidth : AbstractPathMind
    {
        static int numExpandedNodes = 0;
        private Stack<Locomotion.MoveDirection> movements = null;
        private List<Node> nodesToExpand = new List<Node>();
        private List<Node> expandedNodes = new List<Node>();

        /// <summary>
        /// Search if the Node given by parameter is one of the objectives we are looking for.
        /// </summary>
        /// <param name="actualNode">Node will be checked.</param>
        /// <param name="objectives">Array of objectives we will compare with the Node given.</param>
        /// <param name="movementsToDo">Stack of Locomotion.MoveDirection needed to reach the objective.</param>
        /// <returns>bool: True If actualNode is an objective. False if not.</returns>
        ///  private bool IsObjective(Node actualNode, CellInfo[] objectives, Stack<Locomotion.MoveDirection> movementsToDo)
        private bool IsObjective(Node actualNode, CellInfo[] objectives, Stack<Locomotion.MoveDirection> movementsToDo)
        {
            for (int i = 0; i<objectives.Length; i++)
            {
                if (actualNode.GetCell() == objectives[i])
                {
                    numExpandedNodes--;

                    Node aux = actualNode;
                    do
                    {
                        movementsToDo.Push(aux.GetMovement());
                        aux = aux.GetFather();

                    } while (aux.GetFather() != null);
                    Debug.Log("Total number of Nodes expanded: " + numExpandedNodes);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Algorithm BFS. Explained in memory.
        /// </summary>
        /// <param name="nodesToExpand">List of Nodes we still want to expand.</param>
        /// <param name="boardInfo">Information of the Board.</param>
        /// <param name="objectives">Array of objectives we want to reach.</param>
        /// <param name="movements">Stack of movements we have to do in order to reach an objective.</param>
        /// <returns></returns>
        public bool BFS(List<Node> nodesToExpand, BoardInfo boardInfo, CellInfo[] objectives, Stack<Locomotion.MoveDirection> movements)
        {
            //While there is nodes to expand left
            while (nodesToExpand.Count > 0)
            {
                bool isNodeObjective = false;
                List<Node> sucessors = new List<Node>();
                Node actualNode = nodesToExpand[0];

                nodesToExpand.RemoveAt(0); //We get the first node out of the list
                numExpandedNodes++;

                isNodeObjective = IsObjective(actualNode, objectives, movements); //If it's goal we ended.
                if (isNodeObjective) return true;

                Debug.Log("Node to expand: " + actualNode.ToString());
                Debug.Log("Number of expanded Nodes :" + numExpandedNodes);

                
                sucessors = actualNode.Expand(boardInfo, objectives, this.expandedNodes); //If it's not goal, we expand.

                for(int i = 0; i < sucessors.Count; i++)
                {
                    nodesToExpand.Add(sucessors[i]);
                }
                
                }
            Debug.Log("Solution not found. \n");
            return false;
        }


        /// <summary>
        /// This method will ask this script a movement by every Upgrade.
        /// </summary>
        /// <param name="boardInfo">Information of the Board.</param>
        /// <param name="currentPos">Actual position of our Character.</param>
        /// <param name="objectives">Array of objectives we want to reach.</param>
        /// <returns></returns>
        public override Locomotion.MoveDirection GetNextMove(BoardInfo boardInfo, CellInfo currentPos, CellInfo[] objectives)
        {
            if (this.movements == null)
            {
                Stack<Locomotion.MoveDirection> movementsToDo = new Stack<Locomotion.MoveDirection>();
                Node firstNode = new Node(null, currentPos, objectives);
                this.nodesToExpand.Add(firstNode);

                bool existsSolution = BFS(this.nodesToExpand, boardInfo, objectives, movementsToDo);
                if (!existsSolution)
                {
                    Debug.Log("Goal not found. \n");
                }
                else
                {
                    this.movements = movementsToDo;
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
