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

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
    }

    class Word
    {
        public string word; //Almacena la palabra
        public int length; //Alamacena la longitud de la palabra
        public List<bool> validFlags; //Almacena las banderas para saber si se puede usar una letra
        public bool orientation; //true para Horizontal, false para vertical
        public bool used;
        public Tuple<int, int> position;
        public int id; //Se almacena el numero de palabra para las pistas

        public Word(string _word)
        {
            length = _word.Length;
            char[] indexedWord = new char[length + 2];
            indexedWord[0] = ' ';
            indexedWord[length + 1] = '.';
            for (int i = 0; i < length; i++) indexedWord[i + 1] = _word[i];
            length += 2; //Se ajusta el largo de la palabra tomando en cuenta que se agrego el id
            word = new String(indexedWord);
            validFlags = new List<bool>();
            for (int i = 0; i < length; i++)
            {
                validFlags.Add(true);
            }
            validFlags[0] = false; //Quitamos la usabilidad de el caracter id
            validFlags[length - 1] = false;
        }
        //Regresa un booleano que indica si la palabra tiene caracteres usables
        public bool IsUsable()
        {
            foreach (bool flag in validFlags)
            {
                if (flag)
                    return true;
            }
            return false;
        }

        //Regresa un lista con los caracteres usables de la palabra
        public List<char> GetLetters()
        {
            List<char> letters = new List<char>();
            for (int i = 0; i < length; i++)
            {
                if (validFlags[i])
                    letters.Add(word[i]);
            }
            return letters;
        }

        public void AppendID(int ID)
        {
            char[] tempWord = word.ToArray();
            tempWord[0] = Char.Parse(ID.ToString());
            word = new String(tempWord);
            id = ID;
        }
    };

    class Grid
    {
        public int height; // ↓
        public int width; // →
        public List<List<char>> grid;
        public Grid(int h, int w)
        {
            height = h;
            width = w;
            grid = new List<List<char>>();
            //Se incializa el grid y se llena de espacios vacios
            for (int i = 0; i < height; i++)
            {
                grid.Add(new List<char>());
                for (int j = 0; j < width; j++)
                {
                    grid[i].Add(' ');
                }
            }
            height--;
            width--; //Para que funcione con index 0
        }

        public void DrawWord(Word firstWord)
        {
            Random rnd = new Random();
            firstWord.AppendID(1);
            firstWord.orientation = Convert.ToBoolean(rnd.Next(2));
            int mitad = Convert.ToInt32(firstWord.length / 2);
            int x = !firstWord.orientation ? (width / 2) : (width / 2 - mitad);
            int y = firstWord.orientation ? (height / 2) : (height / 2 - mitad);
            firstWord.position = new Tuple<int, int>(x, y);
            for (int i = 0; i < firstWord.length; i++)
            {
                grid[x][y] = firstWord.word[i];
                if (firstWord.orientation) x++;
                else y++;
            }
        }

        //Dibuja una nueva palabra en el grid, regresa true si se pudo insertar
        public bool DrawWord(Word parentWord, Word newWord, int parentIndex, int newIndex, ref int contID)
        {
            int x = parentWord.position.Item1;//Obtenemos la posicion de la primera letra
            int y = parentWord.position.Item2;// de la palabra padre
            newWord.orientation = !parentWord.orientation; //Ponemos una orientación inversa a la del padre

            if (parentWord.orientation)
            {
                x += parentIndex;//Se posiciona en la letra del cruce
                y -= newIndex; //Se posiciona en donde se va a escribir la nueva palabra

                //La palabra cabe dentro del crucigrama?
                if (y < 0 || x > width)
                    return false;
                if (y + newWord.length > height)
                    return false;

            }
            else
            {
                y += parentIndex; //Se posiciona x en la letra del cruce
                x -= newIndex; //Se posiciona en donde se va a escribir la nueva palabra

                //La palabra cabe dentro del crucigrama?
                if (y > height || x < 0)
                    return false;
                if (x + newWord.length > width)
                    return false;

            }

            //La palabra no se cruza con otra palabra?
            int x1 = x;
            int y1 = y;
            if (newWord.orientation)
                x1--;
            else
                y1--;

            for (int i = -1; i <= newWord.length; i++)
            {
                if (!(x1 < 0 || y1 < 0))
                {
                    if (grid[x1][y1] != ' ')
                    {
                        if (i != -1 && i < newWord.length)
                        {

                            if (grid[x1][y1] == newWord.word[newIndex] && i == newIndex /*|| grid[x1][y1] == newWord.word[i] && newWord.validFlags[i]*/) //Esto es un funcion que permite                                                                    
                            {                                                                                                                            //el cruce entre varias palabras si la letra lo permite
                                if (newWord.orientation)
                                    x1++;
                                else
                                    y1++;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            if (grid[x1][y1] == newWord.word[newIndex])
                            {
                                if (newWord.orientation)
                                    x1++;
                                else
                                    y1++;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        if (newWord.orientation)
                            x1++;
                        else
                            y1++;
                    }
                }
                else
                {
                    if (newWord.orientation)
                        x1++;
                    else
                        y1++;
                }

            }

            //Tras pasar todas las validaciones se dibuja la palabra en el grid
            //Se asigna la posicion x,y a la palabra a insertar
            newWord.position = new Tuple<int, int>(x, y);
            contID++;
            newWord.AppendID(contID);
            for (int i = 0; i < newWord.length; i++)
            {
                grid[x][y] = newWord.word[i];
                if (newWord.orientation)
                    x++;
                else
                    y++;
            }

            //Activar las banderas de los caracteres utilizados y de los aledaños

            parentWord.validFlags[parentIndex] = false;
            parentWord.validFlags[parentIndex - 1] = false;
            if (parentIndex != parentWord.length - 1)
                parentWord.validFlags[parentIndex + 1] = false;
            newWord.validFlags[newIndex] = false;
            newWord.validFlags[newIndex - 1] = false;
            if (newIndex != newWord.length - 1) //Si es el ultimo caracter se omite la bandera +1 para no
                newWord.validFlags[newIndex + 1] = false; //salir del rango de la lista

            return true; //Se retorna un true indicando que si se pudo insertar la palabra
        }
    };

    class Crossword
    {
        public List<Word> words; //Lista de palabras a utlizar 
        List<string> clues; //Pistas de las palabras
        int level;
        public int contID;
        Grid grid;
        Crossword()
        {
            grid = new Grid(20, 20);
            words = new List<Word>();
            contID = 1;
        }
        void GetWords() { }// Recupera las palabras de una base de datos o archivo
        void DrawCrossword() { } //Dibuja el crucigrama
        void DrawClues() { } //Dibuja las preguntas

        public void ConstructionAlgorithm(int nWords) //Algoritmo para seleccionar palabras y sus cruces
        {
            words.Sort((Word x, Word y) => y.length - x.length); //Ordenamos el array por tamaños
            Random rnd = new Random();
            Word firstWord = words[0];
            grid.DrawWord(firstWord); //Dibuja la palabra en el centro del grid
            Word secondWord;
            words[0].used = true;
            bool allInserted = false;
            //Se debe crear un ciclo que recorra a todas las palabras y que asigne a la ultima palabra
            //el estatus de padre, pero que si el ciclo para asiganrle falle, pueda seleccionar otra
            //palabra sin problema


            //Que pasa si una palabra no coincide con niguna otra palabra en ningun caracter?
            // Se crea un contador que cuenta cuantas veces se trata de insertar una palabra, si
            //supera el numero de palabras *2, se cambia la palabra padre por una de las anteriormente
            //insertadas buscando que encaje ahi
            int tryCounter = 0;
            while (!allInserted && contID <= nWords)
            {
                if (tryCounter > words.Count * 2)
                {
                    do
                    {
                        firstWord = words[rnd.Next(0, words.Count)];
                    } while (!(firstWord.used && firstWord.IsUsable()));
                    tryCounter = 0;
                }
                do
                {
                    secondWord = words[rnd.Next(0, words.Count)];
                } while (secondWord.used || firstWord.Equals(secondWord));


                bool inserted = false; //Esta bandera nos dice si se pudo dibujar la palabra en el crucigrama
                for (int i = 1; i < firstWord.word.Length; i++) //Empieza de uno para que ingnore el id
                {
                    if (!inserted) //El ciclo ya no se ejecuta si la palabra fue insertada
                        for (int j = 1; j < secondWord.word.Length; j++)
                        {
                            if (!inserted)
                                if (firstWord.word[i] == secondWord.word[j]) //Checa si hay match
                                {
                                    if (firstWord.validFlags[i] && secondWord.validFlags[j]) //Checa si se puede insertar
                                    {
                                        inserted = grid.DrawWord(firstWord, secondWord, i, j, ref contID);//Intenta dibujar la palabra
                                                                                                          //Si la puede dibujar, se rompe el ciclo, si no, sigue buscando otro match
                                    }
                                }
                        }
                    else
                    {
                        secondWord.used = true; //Activamos la flag used para saber que ya la usamos
                        break; //Y se termina el ciclo
                    }
                }
                if (inserted)
                {
                    firstWord = secondWord;
                    tryCounter = 0;
                }
                //Significa que la palabra se inserto satisfactoriamente y se reinicia el tryCounter
                else tryCounter++;//Si inserted es false, significa que el ciclo va a volver a correrse pero la primera
                //palabra va a permanecer igual y va a buscar otra palabra al azar
                //El tryCounter se aumenta para ver cuantas veces se intenta insertar esa palabrai

                //Cambia la flag allInserted para ver si se terminar el ciclo
                allInserted = true;
                foreach (var word in words)
                {
                    if (!word.used) allInserted = false;
                }
            }
        }





    }
}
