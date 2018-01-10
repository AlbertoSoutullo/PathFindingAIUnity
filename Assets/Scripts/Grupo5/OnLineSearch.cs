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
        private float alpha = float.MaxValue;

        private Stack<Locomotion.MoveDirection> movements = null;
        private List<Node> expandedNodes = new List<Node>();

        private bool onLineSearch(Node firstNode, BoardInfo boardInfo, CellInfo[] goals)
        {
            Node bestNode = null;
            List<Node> nodesToExpand = new List<Node>();
            List<Node> nodesForChoice = new List<Node>();

            firstNode.SetHorizon(1);
            nodesToExpand.Add(firstNode);

            while (nodesToExpand.Count > 0)
            {
                //Pillamos el nodo que toca expandir, y lo sacamos de la lista
                Node actualNode = nodesToExpand[0];
                print("Node to expand: " + actualNode.ToString());
                nodesToExpand.RemoveAt(0);
                numNodosExpandidos++;

                if (!(actualNode.getDistance() > alpha))
                {
                    //Si no hemos llegado al horizonte, expandimos.
                    if (actualNode.GetHorizon() < 3)
                    {
                        //Expandimos los sucesores de este nodo (expand ya hace que esos sucesores apunten al padre)
                        List<Node> sucessors = new List<Node>();
                        sucessors = actualNode.Expand(boardInfo, goals, this.expandedNodes);
                        print("Sucesores : " + sucessors.Count);
                        for (int i = 0; i < sucessors.Count; i++)
                        {
                            print(sucessors[i].ToString());
                            bool insertado = false;
                            for (int j = 0; j < nodesToExpand.Count; j++)
                            {
                                //Con el igual lo que hace es priorizar un camino ya que sabemos que al ser una parrilla, no vamos
                                //a expandir dos veces lo mismo al avanzar en diagonal, ya que arriba derecha es igual que derecha arriba
                                if (nodesToExpand[j].getDistance() >= sucessors[i].getDistance() && !insertado && sucessors[i].GetHorizon() < 3)
                                {
                                    nodesToExpand.Insert(j, sucessors[i]);
                                    print("Nodo: " + sucessors[i].ToString() + "se compara con "+ nodesToExpand[j].ToString() + ". Insertado");
                                    insertado = true;
                                }
                            }
                            if (!insertado && sucessors[i].GetHorizon() < 3)
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
                                    nodesForChoice.Add(bestNode);
                                    print("Nuevo alpha: " + alpha + "de " + bestNode.ToString());
                                }
                            }
                        }
                    }
                }
            }
            Node aux = bestNode;
            int backSteps = 0;
            do
            {
                movements.Push(aux.getMovement());
                aux = aux.getFather();
                backSteps++;

            } while ((aux.getFather() != null) && (backSteps < 3) );
            return true;
        }

        /*Método para ordenar enemigos
        List<EnemyBehaviour> orderEnemies(List<EnemyBehaviour> enemies)
        {

        }*/

        public override void Repath()
        {
            movements = new Stack<Locomotion.MoveDirection>();
            expandedNodes = new List<Node>();

        }

        public override Locomotion.MoveDirection GetNextMove(BoardInfo boardInfo, CellInfo currentPos, CellInfo[] goals)
        {
            if (this.movements == null)
            {
                List<EnemyBehaviour> enemies = boardInfo.Enemies;
                CellInfo[] enemyInfo = new CellInfo[3];
                enemyInfo[0] = enemies[0].CurrentPosition();

                print(enemyInfo[0].GetPosition);
                Node firstNode = new Node(null, currentPos, enemyInfo);
                this.movements = new Stack<Locomotion.MoveDirection>();

                bool encontrado = onLineSearch(firstNode, boardInfo, enemyInfo);
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
                Locomotion.MoveDirection moveAux = this.movements.Pop();
                Repath();
                print(moveAux);
                return moveAux;
            }
            
        }


    }
}
