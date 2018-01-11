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
        static int numNodosExpandidos=0;

        //Lista de Movimientos que va a devolver nuestro algoritmo
        public Stack<Locomotion.MoveDirection> movements = null;
        private List<Node> nodesToExpand = null;
        private List<Node> expandedNodes = new List<Node>();


        public bool aStar(List<Node> nodesToExpand, BoardInfo boardInfo, CellInfo[] goals)
        {
            //variablle para ver si ya se ha insertado el nodo en el
            bool insertado = false;
            while (nodesToExpand.Count > 0)
            {
                //Pillamos el nodo que toca expandir, y lo sacamos de la lista
                Node actualNode = nodesToExpand[0];
                nodesToExpand.RemoveAt(0);
                numNodosExpandidos++;
                
                //si ese nodo es goal, hemos acabado
                for (int i = 0; i < goals.Length; i++)
                {
                    if (actualNode.getCell() == goals[i])
                    {
                        numNodosExpandidos--;

                        Node aux = actualNode;
                        do {
                            this.movements.Push(aux.getMovement());
                            aux = aux.getFather();

                        } while (aux.getFather() != null);
                        print("Numero de nodos expandidos al final: " + numNodosExpandidos);
                        return true;
                    }
                }
                print("Node to expand: " + actualNode.ToString());
                print("Numero de nodos expandidos " + numNodosExpandidos);

                //Expandimos los sucesores de este nodo (expand ya hace que esos sucesores apunten al padre)
                List<Node> sucessors = new List<Node>();
                sucessors = actualNode.Expand(boardInfo, goals, this.expandedNodes);
                //sucessors.Sort(compareNodesByDistance);


                for (int i = 0; i < sucessors.Count ; i++)
                {
                    insertado = false;
                    for (int j = 0; j < nodesToExpand.Count; j++)
                    {
                        //Con el igual lo que hace es priorizar un camino ya que sabemos que al ser una parrilla, no vamos
                        //a expandir dos veces lo mismo al avanzar en diagonal, ya que arriba derecha es igual que derecha arriba
                        if (nodesToExpand[j].getDistance() >= sucessors[i].getDistance() && !insertado)
                        {
                            nodesToExpand.Insert(j, sucessors[i]);
                            insertado = true;
                        }
                    }
                    if (!insertado)
                    {
                        nodesToExpand.Add(sucessors[i]);
                    }
                }

  
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
                this.nodesToExpand = new List<Node>();
                Node firstNode = new Node(null, currentPos, goals);
                this.nodesToExpand.Add(firstNode);

                bool encontrado = aStar(this.nodesToExpand, boardInfo, goals);
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
