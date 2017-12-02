using Assets.Scripts.Grupo5;
using UnityEngine;


namespace Assets.Scripts.Grupo5
{

    public class Node
    {
        private Node father { get; set; }
        private CellInfo cell { get; set; }

        //Constructor de nodo, donde deberíamos pasarle el padre y los datos, que sería la informacion
        //de la celda en la que está el personaje
        public Node(Node father, CellInfo cell)
        {
            this.cell = cell;
            this.father = father;
        }

        //Método para expandir los nodos a partir del nodo en el que nos encontramos.
        public List<Node> Expand()
        {
            board = GetComponent<>
        }


    }


    
}