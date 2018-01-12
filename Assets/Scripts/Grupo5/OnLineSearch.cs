using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.DataStructures;
using Assets.Scripts.Grupo5;

namespace Assets.Scripts.Grupo5
{
    /// <summary>
    /// This Script will do an Horizon search. That search will depend on an horizon value given by parameter.
    /// </summary>
    class OnLineSearch : AbstractPathMind
    {
        static  int   numNodosExpandidos = 0;
        public  int   horizon            = 3;
        public  int   maxLoopMovements   = 20;
        private float alpha              = float.MaxValue;
        public  bool  exitLoops          = false;
        int[]         loopIterations     = new int[5] { 0, 0, 0, 0, 0 };

        private Stack<Locomotion.MoveDirection> movements = new Stack<Locomotion.MoveDirection>();

        /// <summary>
        /// This method will perform an Horizon Search.
        /// </summary>
        /// <param name="firstNode">First Node.</param>
        /// <param name="boardInfo">Information of the Board.</param>
        /// <param name="enemy">Array of enemies we want to hunt. </param>
        /// <param name="goals">Array of goals we want to reach. </param>
        /// <returns>Movement we want to do.</returns>
        private Node HorizonSearch(Node firstNode, BoardInfo boardInfo, CellInfo[] enemy, CellInfo[] goals)
        {
            bool       isGoal        = false;
            Node       bestNode      = null;
            Node       aux           = null;
            List<Node> nodesToExpand = new List<Node>();
            List<Node> expandedNodes = new List<Node>();

            alpha = float.MaxValue;

            firstNode.SetHorizon(1);
            nodesToExpand.Add(firstNode);

            while (nodesToExpand.Count > 0)
            {
                Node actualNode = nodesToExpand[0];
                print("Node to expand: " + actualNode.ToString());
                nodesToExpand.RemoveAt(0);
                numNodosExpandidos++;

                if (!(actualNode.GetDistance() >= alpha))
                {
                    if (actualNode.GetHorizon() < horizon)
                    {
                        List<Node> sucessors = new List<Node>();
                        sucessors = actualNode.Expand(boardInfo, enemy, expandedNodes);

                        for (int i = 0; i < sucessors.Count; i++)
                        {
                            if (sucessors[i].GetCell() == enemy[0])
                            {
                                aux = sucessors[i];
                                while (aux.GetFather() != firstNode)
                                {
                                    aux = aux.GetFather();
                                }
                                return aux;
                            }

                            isGoal = false;
                            for (int k = 0; k < goals.Length; k++)
                            {
                                if (goals[k] == sucessors[i].GetCell())
                                {
                                    isGoal = true;
                                }
                            }
                            if (!isGoal)
                            {
                                print(sucessors[i].ToString());
                                bool insertado = false;
                                for (int j = 0; j < nodesToExpand.Count; j++)
                                {
                                    if (nodesToExpand[j].GetDistance() >= sucessors[i].GetDistance() && !insertado)
                                    {
                                        nodesToExpand.Insert(j, sucessors[i]);
                                        insertado = true;
                                    }
                                }
                                if (!insertado)
                                {
                                    nodesToExpand.Add(sucessors[i]);
                                }
                                if (sucessors[i].GetHorizon() == horizon)
                                {
                                    if (sucessors[i].GetDistance() < alpha)
                                    {
                                        bestNode = sucessors[i];
                                        alpha = bestNode.GetDistance();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            aux = bestNode;
            while (aux.GetFather() != firstNode)
            {
                aux = aux.GetFather();
            }
            return aux;
        }

        public override void Repath()
        {
        }

        /// <summary>
        /// This method will extend the horizon value in order to avoid loops.
        /// </summary>
        /// <param name="node">Actual Node</param>
        private void ExtendHorizon(Node node)
        {
            if (node.GetMovement() == Locomotion.MoveDirection.Up)    loopIterations[0]++;
            if (node.GetMovement() == Locomotion.MoveDirection.Right) loopIterations[1]++;
            if (node.GetMovement() == Locomotion.MoveDirection.Down)  loopIterations[2]++;
            if (node.GetMovement() == Locomotion.MoveDirection.Left)  loopIterations[3]++;
            loopIterations[4]++;

            if (loopIterations[4] == maxLoopMovements)
            {
                int diferents = 0;
                for (int i = 0; i < loopIterations.Length - 1; i++)
                {
                    if (loopIterations[i] == 0) diferents++;
                }
                if (diferents == 2)
                {
                    horizon++;
                }
                Array.Clear(loopIterations, 0, loopIterations.Length);
            }
        }

        /// <summary>
        /// This method will ask this script a movement by every Upgrade.
        /// </summary>
        /// <param name="boardInfo">Information of the Board.</param>
        /// <param name="currentPos">Actual position of our Character.</param>
        /// <param name="objectives">Array of objectives we want to reach.</param>
        /// <returns>A movement.</returns>
        public override Locomotion.MoveDirection GetNextMove(BoardInfo boardInfo, CellInfo currentPos, CellInfo[] goals)
        {
            List<EnemyBehaviour> enemies = boardInfo.Enemies;
            if (enemies.Count == 0)
            {
                if (this.movements.Count == 0)
                {
                    bool encontrado  = false;
                    List<Node> nodes = new List<Node>();
                    Node currentNode = new Node(null, currentPos, goals);
                    nodes.Add(currentNode);
                    OffLineSearchAStar astar = new OffLineSearchAStar();
                    encontrado = astar.AStar(nodes, boardInfo, goals, this.movements);
                    if (!encontrado)
                    {
                        while (true)
                        {
                            return Locomotion.MoveDirection.None;
                        }
                    }
                }
                return this.movements.Pop(); 
            }
            else
            {
                CellInfo[] enemyInfo = new CellInfo[1];
                enemyInfo[0] = enemies[0].CurrentPosition();
                print("POSICION DEL ENEMIGO: " + enemyInfo[0].GetPosition);
                if (enemyInfo[0].Walkable == true)
                {
                    Node firstNode = new Node(null, currentPos, enemyInfo);

                    Node node = null;
                    node = HorizonSearch(firstNode, boardInfo, enemyInfo, goals);
                    if (node != null)
                    {
                        if (exitLoops)
                        {
                            ExtendHorizon(node);
                        }
                        return node.GetMovement();
                    }
                    else return Locomotion.MoveDirection.None;
                }
                else
                {
                    return Locomotion.MoveDirection.None;
                }
            }
        }
    }
}
