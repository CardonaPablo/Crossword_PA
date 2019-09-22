
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
        public char[] word; //Almacena la palabra
        public int length; //Alamacena la longitud de la palabra
        public List<bool> validFlags; //Almacena las banderas para saber si se puede usar una letra
        public bool orientation; //true para Horizontal, false para vertical
        public bool used;
        public Tuple<int, int> position;
        public int id; //Se almacena el numero de palabra para las pistas

        Word(string _word)
        {
            length = _word.Length;
            char[] indexedWord = new char[ length + 1];
            indexedWord[0] = ' ';
            for (int i = 0; i < length; i++)
            {
                indexedWord[i + 1] = word[i];
            }
            length++; //Se ajusta el largo de la palabra tomando en cuenta que se agrego el index
        }

        public bool IsUsable() //Regresa un booleano que indica si la palabra tiene caracteres usables
        {
            foreach (bool flag in validFlags)
            {
                if (flag) return true;
            }
            return false;
        }
        List<char> GetLetters() //Regresa un lista con los caracteres usables de la palabra
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
        Grid(int h, int w)
        {
            height = h;
            width = w;
            grid = new List<List<char>>();
            //Se incializa el grid y se llena de espacios vacios
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    grid[i][j] = ' ';
                }
            }
        }


        bool DrawWord(Word parentWord, Word newWord, int parentIndex, int  newIndex) //Dibuja una nueva palabra en el grid, regresa true si se pudo insertar
        {
            int x = parentWord.position.Item1;//Obtenemos la posicion de la primera letra
            int y = parentWord.position.Item2;// de la palabra padre
            newWord.orientation = !parentWord.orientation; //Ponemos una orientación inversa a la del padre

            if (parentWord.orientation)
            {                           
                x += parentIndex;//Se posiciona en la letra del cruce
                y -= newIndex; //Se posiciona en donde se va a escribir la nueva palabra
                //Validaciones
                //1. La palabra cabe dentro del crucigrama
                if (y<0 || x > width) return false;
                if (y + newWord.length > height) return false;
                //2. La palabra no se cruza con otra palabra
                for (int i = y-1 ; i < newWord.length +1 ; i++)
                {                                               
                    //Que pasa si 'i' se sale del grid?
                    if ( i < 0 || i > height) continue;
                    if ( grid[x][i] != ' ')
                    {
                        return false;
                    }
                }
            }
            else
            {
                y += parentIndex; //Se posiciona x en la letra del cruce
                x -= newIndex; //Se posiciona en donde se va a escribir la nueva palabra

                //Validaciones
                //1. La palabra cabe dentro del crucigrama
                if (y < 0 || x > width) return false;
                if (x + newWord.length > width) return false;
                //Validaciones
                //2. La palabra no se cruza con otra palabra
                for (int i = x-1; i < newWord.length +1; i++)
                {
                    //Que pasa si 'i' se sale del grid? Se ignora en las ultimas posiciones
                    if (i < 0 || i > width) continue;
                    if (grid[i][y] != ' ')
                    {
                        return false;
                    }
                }
            }

            //Tras pasar todas las validaciones se dibuja la palabra en el grid
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
            //Se retorna un true indicando que si se pudo insertar la palabra
            return true;
        }
    };

    class Crossword
    {
        List<Word> words; //Lista de palabras a utlizar 
        List<string> clues; //Pistas de las palabras
        int level;
        Grid grid;
        Crossword()
        {
           /* Por el momento no se utiliza el constructor */
        }
        void getWords(){};// Recupera las palabras de una base de datos o archivo
        void DrawCrossword() { } //Dibuja el crucigrama
        void DrawClues(){ } //Dibuja las preguntas
        bool constructionAlgorithm() { return true} //Algoritmo para seleccionar palabras y sus cruces
    };



    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
    }
}

