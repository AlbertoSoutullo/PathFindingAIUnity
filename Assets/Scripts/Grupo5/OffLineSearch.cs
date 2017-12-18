using Assets.Scripts.SampleMind;
using Assets.Scripts.DataStructures;
using Assets.Scripts.Grupo5;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace Assets.Scripts.Grupo5
{
    public class OffLineSearch : AbstractPathMind
    {

        //Lista de Movimientos que va a devolver nuestro algoritmo
        private Stack<Locomotion.MoveDirection> movements = null;
        private List<Node> nodes = null; //abierta



        private bool test(List<Node> nodesToExpand, BoardInfo boardInfo, CellInfo[] goals)
        {
            while (nodesToExpand.Count > 0)
            {
                //Pillamos el nodo que toca expandir, y lo sacamos de la lista
                Node actualNode = nodesToExpand[0];
                this.movements.Push(actualNode.getMovement());
                nodesToExpand.RemoveAt(0);
                print(actualNode.ToString());
                //si ese nodo es goal, hemos acabado
                for (int i = 0; i < goals.Length; i++)
                {
                    if (actualNode.getCell() == goals[i])
                    {
                        return true;
                    }
                }

                //Expandimos los sucesores de este nodo (expand ya hace que esos sucesores apunten al padre)
                List<Node> sucessors = new List<Node>();
                sucessors = actualNode.Expand(boardInfo, goals);
                print("Sucesores :" + sucessors.Count);
                //Añadimos los sucesores a los nodos que tenemos que espandir
                nodesToExpand.AddRange(sucessors);
                print("Nodos a expandir: " + nodesToExpand.Count);
                //Reordenamos los nodos
                nodesToExpand.Sort(compareNodesByDistance);
                
            }
            Debug.Log("Solution not found. \n");
            return false;

        }

        //NO SE USA
        //Metodo A* recursivo, donde le pasamos el nodo actual, la información del tablero y los goals.
        //private bool aStarAlgorithm(Node actualNode, BoardInfo boardInfo, CellInfo[] goals)
        //{
        //    //Lista para guardar los descendientes
        //    List<Node> decendents = new List<Node>();

        //    //Primero miramos si ya estamos en una salida, y si lo estamos no devolvemos movimientos.
        //    for (int i = 0; i < goals.Length; i++)
        //    {
        //        if (actualNode.getCell() == goals[i])
        //        {
        //            this.movements.Push(actualNode.getMovement());
        //            return true;
        //        }
        //    }

        //    //Sacamos los descendientes
        //    decendents = actualNode.Expand(boardInfo);
        //    //Ordenamos
        //    decendents = sortDecendents(decendents, goals);

        //    //
        //}

        public override void Repath()
        {

        }


        public override Locomotion.MoveDirection GetNextMove(BoardInfo boardInfo, CellInfo currentPos, CellInfo[] goals)
        {
            if (this.movements == null)
            {
                this.movements = new Stack<Locomotion.MoveDirection>();
                this.nodes = new List<Node>();
                Node firstNode = new Node(null, currentPos, goals);
                this.nodes.Add(firstNode);

                bool encontrado = test(this.nodes, boardInfo, goals);
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

        ////NO SE USA
        ////Metodo para ordenar el array de nodos. (Heurística)
        //private List<Node> sortDecendents(List<Node> Nodes, CellInfo[] goal)
        //{
        //    int[] numeros = new int[Nodes.Count];
        //    for(int i=0; i< Nodes.Count; i++)
        //    {
        //        int column = Math.Abs(Nodes[i].getCell().ColumnId - goal[0].ColumnId);
        //        int row = Math.Abs(Nodes[i].getCell().RowId - goal[0].RowId);
        //        int dist = column + row;
        //        numeros[i] = dist;
        //    }  
        //    return null;
        //}

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

/*
public class BusquedaAmplitud
{
    public Queue<Nodo> Abiertos { get; set; }
    public BusquedaAmplitud()
    {
        Abiertos = new Queue<Nodo>();
    }

    public Nodo Buscar(Estado inicio, Estado meta)
    {
        inicio.Accion = "Inicio";
        Nodo inicial = new Nodo(inicio, null);

        Abiertos.Enqueue(inicial);
        while (Abiertos.Count > 0)
        {
            Nodo actual = Abiertos.Dequeue();
            if (EsMeta(actual, meta))
            {
                return actual;

            }
            foreach (var nodo in actual.Expandir())
            {
                Abiertos.Enqueue(nodo);
            }
        }
        return null;
    }

    public bool EsMeta(Nodo actual, Estado meta)
    {
        return actual.Estado.EsMeta(meta);
    }
}
*/