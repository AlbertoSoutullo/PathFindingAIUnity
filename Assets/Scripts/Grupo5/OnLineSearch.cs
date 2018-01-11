using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.DataStructures;
using Assets.Scripts.Grupo5;

namespace Assets.Scripts.Grupo5
{
    class OnLineSearch : AbstractPathMind
    {
        static int numNodosExpandidos = 0;
        public int horizon = 3;
        public int maxLoopMovements = 20;
        private float alpha = float.MaxValue;
        int[] loopIterations = new int[5] { 0, 0, 0, 0 , 0};
        public bool exitLoops = false;

        private Stack<Locomotion.MoveDirection> movements = new Stack<Locomotion.MoveDirection>();


        private Node onLineSearch(Node firstNode, BoardInfo boardInfo, CellInfo[] enemy, CellInfo[] goals)
        {
            bool isGoal = false;
            Node bestNode = null;
            List<Node> nodesToExpand = new List<Node>();
            List<Node> expandedNodes = new List<Node>();

            alpha = float.MaxValue;

            firstNode.SetHorizon(1);
            nodesToExpand.Add(firstNode);

            while (nodesToExpand.Count > 0)
            {
                //Pillamos el nodo que toca expandir, y lo sacamos de la lista
                Node actualNode = nodesToExpand[0];
                print("Node to expand: " + actualNode.ToString());
                nodesToExpand.RemoveAt(0);
                numNodosExpandidos++;

                if (actualNode.getCell() == enemy[0])
                {
                    return actualNode;
                }

                if (!(actualNode.getDistance() >= alpha))
                {
                    //Si no hemos llegado al horizonte, expandimos.
                    if (actualNode.GetHorizon() < horizon)
                    {
                        //Expandimos los sucesores de este nodo (expand ya hace que esos sucesores apunten al padre)
                        List<Node> sucessors = new List<Node>();
                        sucessors = actualNode.Expand(boardInfo, enemy, expandedNodes);
                        print("Sucesores : " + sucessors.Count);

                        for (int i = 0; i < sucessors.Count; i++)
                        {
                            if (sucessors[i].getCell() == enemy[0])
                            {
                                return actualNode;
                            }
                            isGoal = false;
                            for (int k = 0; k < goals.Length; k++)
                            {
                                if (goals[k] == sucessors[i].getCell())
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
                                    //Con el igual lo que hace es priorizar un camino ya que sabemos que al ser una parrilla, no vamos
                                    //a expandir dos veces lo mismo al avanzar en diagonal, ya que arriba derecha es igual que derecha arriba
                                    if (nodesToExpand[j].getDistance() >= sucessors[i].getDistance() && !insertado)
                                    {
                                        nodesToExpand.Insert(j, sucessors[i]);
                                        print("Nodo: " + sucessors[i].ToString() + "se compara con " + nodesToExpand[j].ToString() + ". Insertado");
                                        insertado = true;
                                    }
                                }
                                if (!insertado)
                                {
                                    nodesToExpand.Add(sucessors[i]);
                                    print("Nodo: " + sucessors[i].ToString() + " insertado al final.");
                                }
                                if (sucessors[i].GetHorizon() == horizon)
                                {
                                    if (sucessors[i].getDistance() < alpha)
                                    {
                                        bestNode = sucessors[i];
                                        alpha = bestNode.getDistance();
                                        //nodesForChoice.Add(bestNode);
                                        print("Nuevo alpha: " + alpha + "de " + bestNode.ToString());
                                    }
                                }
                            }
                        }
                    }
                }
            }
            Node aux = bestNode;
            while (aux.getFather() != firstNode)
            {
                aux = aux.getFather();
            }
            return aux;
        }

        /*Método para ordenar enemigos
        List<EnemyBehaviour> orderEnemies(List<EnemyBehaviour> enemies)
        {

        }*/

        public override void Repath()
        {

            

        }

        public override Locomotion.MoveDirection GetNextMove(BoardInfo boardInfo, CellInfo currentPos, CellInfo[] goals)
        {
            List<EnemyBehaviour> enemies = boardInfo.Enemies;

            if (enemies.Count == 0)
            {
                if (this.movements.Count == 0)
                {
                    bool encontrado = false;
                    List<Node> nodes = new List<Node>();
                    Node currentNode = new Node(null, currentPos, goals);
                    nodes.Add(currentNode);
                    OffLineSearchaStar astar = new OffLineSearchaStar();
                    encontrado = astar.aStar(nodes, boardInfo, goals, this.movements);
                }
                return this.movements.Pop(); ;
            }
            else
            {
                CellInfo[] enemyInfo = new CellInfo[1];
                enemyInfo[0] = enemies[0].CurrentPosition();
                print("Caminable " + enemyInfo[0].Walkable);
                if (enemyInfo[0].Walkable == true)
                {
                    Node firstNode = new Node(null, currentPos, enemyInfo);

                    Node node = null;
                    node = onLineSearch(firstNode, boardInfo, enemyInfo, goals);
                    if (node != null)
                    {
                        print("holahola" + node.ToString());
                        /*while (node.getFather().getFather().getFather() != null)
                        {
                            node = node.getFather();
                        }
                        */
                        print(node.ToString());

                        if (exitLoops)
                        {
                            if (node.getMovement() == Locomotion.MoveDirection.Up) loopIterations[0]++;
                            if (node.getMovement() == Locomotion.MoveDirection.Right) loopIterations[1]++;
                            if (node.getMovement() == Locomotion.MoveDirection.Down) loopIterations[2]++;
                            if (node.getMovement() == Locomotion.MoveDirection.Left) loopIterations[3]++;
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
                        

                        return node.getMovement();
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
