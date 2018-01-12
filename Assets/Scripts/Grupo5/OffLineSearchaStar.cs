using Assets.Scripts.SampleMind;
using Assets.Scripts.DataStructures;
using Assets.Scripts.Grupo5;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace Assets.Scripts.Grupo5
{
    /// <summary>
    /// This script will do an OffLine seach using the algorithm A*.
    /// It has his own private Stack of movements, and in order to use it in other scripts we can pass trhough parameters
    /// another Stack of movements.
    /// </summary>
    public class OffLineSearchAStar : AbstractPathMind
    {
        static  int                             numExpandedNodes = 0;
        private Stack<Locomotion.MoveDirection> movements        = null;
        private List<Node>                      nodesToExpand    = new List<Node>();
        private List<Node>                      expandedNodes    = new List<Node>();

        public bool DeepPreference = false;

         /// <summary>
         /// Search if the Node given by parameter is one of the objectives we are looking for.
         /// </summary>
         /// <param name="actualNode">Node will be checked.</param>
         /// <param name="objectives">Array of objectives we will compare with the Node given.</param>
         /// <param name="movementsToDo">Stack of Locomotion.MoveDirection needed to reach the objective.</param>
         /// <returns>bool: True If actualNode is an objective. False if not.</returns>
        private bool IsObjective(Node actualNode, CellInfo[] objectives, Stack<Locomotion.MoveDirection> movementsToDo)
        {
            //si ese nodo es goal, hemos acabado
            for (int i = 0; i < objectives.Length; i++)
            {
                if (actualNode.getCell() == objectives[i])
                {
                    numExpandedNodes--;

                    Node aux = actualNode;
                    do
                    {
                        movementsToDo.Push(aux.getMovement());
                        aux = aux.getFather();

                    } while (aux.getFather() != null);
                    print("Total number of Nodes expanded: " + numExpandedNodes);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Insert a List of Nodes into another List of Nodes by checking if they are already there or not.
        /// If the Node is inserted, it will be inserted by Distance to the objective. 
        /// </summary>
        /// <param name="sucessors">List of Nodes that will be checked and inserted.</param>
        /// <param name="nodesToExpand">List of Nodes that will be used to check if a sucessor is already expanded.</param>
        private void InsertSucessors(List<Node> sucessors, List<Node> nodesToExpand)
        {
            for (int i = 0; i < sucessors.Count; i++)
            {
                bool insertedNode = false;
                for (int j = 0; j < nodesToExpand.Count; j++)
                {
                    if (DeepPreference)
                    {
                        //As we know this is a square map and there is not diagonal moves, we check if the distance is >=,
                        //with this, we are not going to expand the same path 2 times, because it will do it in depth.
                        if (nodesToExpand[j].getDistance() >= sucessors[i].getDistance() && !insertedNode)
                        {
                            nodesToExpand.Insert(j, sucessors[i]);
                            insertedNode = true;
                        }
                    }
                    else
                    {
                        if (nodesToExpand[j].getDistance() > sucessors[i].getDistance() && !insertedNode)
                        {
                            nodesToExpand.Insert(j, sucessors[i]);
                            insertedNode = true;
                        }
                    }
                }
                if (!insertedNode)
                {
                    nodesToExpand.Add(sucessors[i]);
                }
            }
        }

        /// <summary>
        /// Algorithm A*. Explained in memory.
        /// </summary>
        /// <param name="nodesToExpand">List of Nodes we still want to expand.</param>
        /// <param name="boardInfo">Information of the Board.</param>
        /// <param name="objectives">Array of objectives we want to reach.</param>
        /// <param name="movements">Stack of movements we have to do in order to reach an objective.</param>
        /// <returns></returns>
        public bool AStar(List<Node> nodesToExpand, BoardInfo boardInfo, CellInfo[] objectives, Stack<Locomotion.MoveDirection> movements)
        {
            //While there is nodes to expand left
            while (nodesToExpand.Count > 0)
            {
                bool       isNodeObjective = false;
                List<Node> sucessors       = new List<Node>();
                Node       actualNode      = nodesToExpand[0];

                nodesToExpand.RemoveAt(0); //We get the first node out of the list
                numExpandedNodes++;

                isNodeObjective = IsObjective(actualNode, objectives, movements); //If it's goal we ended.
                if (isNodeObjective) return true;

                print("Node to expand: "           + actualNode.ToString());
                print("Number of expanded Nodes :" + numExpandedNodes);

                sucessors = actualNode.Expand(boardInfo, objectives, this.expandedNodes); //If it's not goal, we expand.

                InsertSucessors(sucessors, nodesToExpand);
            }
            Debug.Log("Solution not found. \n");
            return false;
        }

        public override void Repath()
        {
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
                Stack<Locomotion.MoveDirection>  movementsToDo = new Stack<Locomotion.MoveDirection>();
                Node                             firstNode     = new Node(null, currentPos, objectives);
                this.nodesToExpand.Add(firstNode);

                bool existsSolution = AStar(this.nodesToExpand, boardInfo, objectives, movementsToDo);
                if (!existsSolution)
                {
                    print("Goal not found. \n");
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
    }
}
