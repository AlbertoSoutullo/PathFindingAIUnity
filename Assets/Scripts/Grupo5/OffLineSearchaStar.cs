using Assets.Scripts.SampleMind;
using Assets.Scripts.DataStructures;
using Assets.Scripts.Grupo5;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace Assets.Scripts.Grupo5
{
    public class OffLineSearchaStar : AbstractPathMind
    {

        //Lista de Movimientos que va a devolver nuestro algoritmo
        private Stack<Locomotion.MoveDirection> movements = null;
        private Queue<Node> nodesQueue = null;
        private List<Node> expandedNodes = new List<Node>();


        private bool aStar(Queue<Node> nodesToExpand, BoardInfo boardInfo, CellInfo[] goals)
        {
            while (nodesToExpand.Count > 0)
            {
                //Pillamos el nodo que toca expandir, y lo sacamos de la lista
                Node actualNode = nodesToExpand.Dequeue();
                print(actualNode.ToString());
                this.expandedNodes.Add(actualNode);
                print(actualNode.ToString());
                //si ese nodo es goal, hemos acabado
                for (int i = 0; i < goals.Length; i++)
                {
                    if (actualNode.getCell() == goals[i])
                    {
                        Node aux = actualNode;
                        do {
                            print(aux.ToString());
                            this.movements.Push(aux.getMovement());
                            aux = aux.getFather();

                        } while (aux.getFather() != null);
                        return true;
                    }
                }

                //Expandimos los sucesores de este nodo (expand ya hace que esos sucesores apunten al padre)
                List<Node> sucessors = new List<Node>();
                sucessors = actualNode.Expand(boardInfo, goals);

                sucessors.Sort(compareNodesByDistance);

                //Como los hemos ordenado de menor a mayor distancia, y al estar trabajando con una pila, los vamos a meter al revés
                for (int i = sucessors.Count -1; i >= 0 ; i--)
                {
                    bool found = false; //flag para mirar si los sucesores no han sido metidos ya en la lista de nodos expandidos.
                    for (int j = 0; j < this.expandedNodes.Count; j++)
                    {
                        if (this.expandedNodes[j].isEqual(sucessors[i]))
                            found = true;
                    }
                    if (!found)
                        nodesToExpand.Enqueue(sucessors[i]);
                    
                }

                //Por que no funciona esto
                /*
                foreach (Node node in sucessors)
                {
                    nodesToExpand.Push(node);
                }
                */
  
            }
            Debug.Log("Solution not found. \n");
            return false;
        }

        public override void Repath()
        {

        }


        public override Locomotion.MoveDirection GetNextMove(BoardInfo boardInfo, CellInfo currentPos, CellInfo[] goals)
        {
            if (this.movements == null)
            {
                this.movements = new Stack<Locomotion.MoveDirection>();
                this.nodesQueue = new Queue<Node>();
                Node firstNode = new Node(null, currentPos, goals);
                this.nodesQueue.Enqueue(firstNode);

                bool encontrado = aStar(this.nodesQueue, boardInfo, goals);
                if (!encontrado)
                {
                    print("Goal not found. \n");
                }
                //Mirar si no ha encontrado goal.
                //aStarAlgorithm(firstNode, boardInfo, goals);
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

        //Método comparatorio que usaremos en el sort.
        private int compareNodesByDistance(Node x, Node y)
        {
            if (x == null)
            {
                if (y == null)
                {
                    return 0; //Si ambos son null, ambos son iguales
                }
                else
                {
                    return -1; //Si x es null pero y no, y es mas grande
                }
            }
            else //Si x no es null
            {
                if (y == null)  // e y es null, x es mas grande
                {
                    return 1;
                }
                else //Si ni x ni y son null, los comparamos, primero por coste, despues por distancia
                {
                    if (x.getCell().WalkCost == y.getCell().WalkCost) //Si el coste es el mismo (como debería ser en esta práctica)
                    {
                        if (x.getDistance() >= y.getDistance()) //si X está mas lejos que y, x es mayor
                        {
                            return 1;
                        }
                        else
                        {
                            return -1; //Si no, y es mayor.
                        }
                    }
                    else
                    {
                        if (x.getCell().WalkCost > y.getCell().WalkCost) //Si el coste de X es mayor, x es mayor.
                        {
                            return 1;
                        }
                        else
                        {
                            return -1;
                        }
                    }

                }

            }
        }
    }
}
