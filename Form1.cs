using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Crucigrama_PA
{

    class Word
    {
        //Hay que dejar un espacio para el numero identificador
        public string word; //Almacena la palabra
        public int length; //Alamacena la longitud de la palabra
        public List<bool> validFlags; //Almacena las banderas para saber si se puede usar una letra
        public bool orientation; //true para Horizontal, false para vertical
        public bool used;
        public Tuple<int, int> position;
        public int id; //Se almacena el numero de palabra para las pistas
        public bool IsUsable() //Regresa un booleano que indica si la palabra tiene caracteres usables
        {
            foreach (bool flag in validFlags)
            {
                if (flag) return true;
            }
            return false;
        }
        List<char> GetLetters()
        {
            List<char> letters = new List<char>();
            for (int i = 0; i < length; i++)
            {
                if (validFlags[i])
                    letters.Add(word[i]);
            }
            return letters;
        }
    };

    class Grid
    {
        int height; // ↓
        int width; // →
        List<List<char>> grid;
        //Como dibujar palabras que no se dan por el inicio?
        //Se debe mandar el index de la letra a cruzar, es decir desde donde se debe
        //dibujar la palabra
        //Se dibuja la parte posterior a ese indice
        //Despues de dibuja la parte anterior a ese indice
        //Deberia caber utilizando las validaciones que hicimos en otra funcion

        Grid()
        {
            height = 20;
            width = 20;
            grid = new List<List<char>>();
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    grid[i][j] = '  ';
                }
            }
        }


        bool DrawWord(Word parentWord, Word newWord, int parentIndex, int  newIndex)
        {
            int x = parentWord.position.Item1;//Obtenemos la posicion de la primera letra
            int y = parentWord.position.Item2;// de la palabra padre
            Tuple<int, int> intersection = new Tuple<int,int>(x, y);
            newWord.orientation = !parentWord.orientation; //Ponemos una orientación inversa
            if (parentWord.orientation) //Ajustamos la posicion de x y basados en donde se cruzan 
            {                           // y en donde comenzaremos a dibujar la nueva palabra
                x += parentIndex;
                //Guarda el cruce
                intersection = new Tuple<int, int>(x, y);
                y -= newIndex;

                //Validaciones
                //1. La palabra cabe dentro del crucigrama
                if (y<0 || x > width) return false;
                if (y + newWord.length > height) return false;
                //Validaciones
                //2. La palabra no se cruza con otra palabra
                int y2 = y;
                for (int i = 0; i < newWord.length; i++)
                {
                    if ( grid[x][y] != ' ')
                    {
                        return false;
                    }
                    y2++;
                }
                
            }
            else
            {

                y += parentIndex;
                intersection = new Tuple<int, int>(x, y);
                x -= newIndex;

                //Validaciones
                //1. La palabra cabe dentro del crucigrama
                if (y < 0 || x > width) return false;
                if (x + newWord.length > width) return false;
                //Validaciones
                //2. La palabra no se cruza con otra palabra
                int x2 = x;
                for (int i = 0; i < newWord.length; i++)
                {
                    if (grid[x2][y] != ' ')
                    {
                        return false;
                    }
                    x2++;
                }
            }

            //Validaciones
            //1. La palabra cabe dentro del crucigrama

            //2. La palabra no se cruza con otra palabra
            for (int i = 0;  i < newWord.length;  i++)
            {   
                grid[x][y] = newWord.word[i];
                if (newWord.orientation) x++;
                else y++;
            }
            
            //Activar las banderas de los caracteres utilizados y de los aledaños
            parentWord.validFlags[parentIndex] = false;
            parentWord.validFlags[parentIndex + 1] = false;
            parentWord.validFlags[parentIndex - 1] = false;
            newWord.validFlags[newIndex] = false;
            newWord.validFlags[newIndex + 1] = false;
            newWord.validFlags[newIndex - 1] = false;

           
            return true;
        }
    };



    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
    }
}
