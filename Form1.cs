using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Crossword_PA
{
    public partial class Form2 : Form
    {
        public Crossword c = new Crossword();
        public Form2()
        {
            InitializeComponent();
            c.GetWords();
            switch (Program.level)
            {
                case 1:
                    c.ConstructionAlgorithm(5);
                    break;
                case 2:
                    c.ConstructionAlgorithm(Program.rnd.Next(7, 20));
                    break;
                case 3:
                default:
                    c.ConstructionAlgorithm(Program.rnd.Next(10, 20));
                    break;
            }

            DrawCrossword();
            DrawClues();

        }

        void DrawCrossword()
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            for (int j = 0; j < c.grid.grid[0].Count(); j++)
            {
                DataGridViewTextBoxColumn a = new DataGridViewTextBoxColumn();
                a.FillWeight = 16;
                a.MaxInputLength = 1;
                a.DefaultCellStyle.BackColor = Color.White;
                a.Width = dataGridView1.Width / c.grid.grid[0].Count();
                dataGridView1.Columns.Add(a);
            }

            for (int i = 0; i < c.grid.grid.Count(); i++)
            {
                DataGridViewRow b = new DataGridViewRow();
                b.Height = dataGridView1.Height / c.grid.grid.Count();
                dataGridView1.Rows.Add(b);
            }

            for (int i = 0; i < c.grid.grid.Count; i++)
            {
                for (int j = 0; j < c.grid.grid[i].Count; j++)
                {
                    DrawCell(j, i, c.grid.grid[i][j]);
                }
            }

            for (int i = 0; i < c.words.Count; i++)
            {
                if (c.words[i].id > 0)
                {
                    DrawCell(c.words[i].position.Item2, c.words[i].position.Item1, c.words[i].id);
                }
            }

        }

        private void DrawClues()
        {
            int yPositionH = 68;
            int yPositionV = 295;
            for (int i = 0; i < c.sortedClues.Count(); i++)
            {
                Word correspondiente = c.words.Find((Word x) =>
                {
                    if (x.id == i + 1) return true;
                    else return false;
                });
                Label clueHolder = new Label();
                clueHolder.Width = 500;
                if (!correspondiente.orientation)
                {
                    yPositionH += 21;
                    clueHolder.Location = new Point(500, yPositionH);
                }
                else
                {
                    yPositionV += 21;
                    clueHolder.Location = new Point(500, yPositionV);
                }

                clueHolder.Text = $"{correspondiente.id.ToString()}. {c.sortedClues[i]}";
                this.Controls.Add(clueHolder);
            }
        }

        private void DrawCell(int x, int y, char car)
        {
            DataGridViewCell c = dataGridView1[x, y];
            if (car == ' ')
            {
                c.Style.BackColor = Color.Black;
                c.Value = car;
                c.ReadOnly = true;
            }
        }
        private void DrawCell(int x, int y, int index)
        {
            DataGridViewCell c = dataGridView1[x, y];
            c.Style.Font = new Font("Arial", 8.5F, GraphicsUnit.Pixel);
            c.Style.BackColor = Color.SlateGray;
            c.Value = index;
            c.ReadOnly = true;
        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form1 form1 = new Form1();
            form1.Show();
        }

        private void dificultadBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void revisarBtn_Click(object sender, EventArgs e)
        {
            bool correct = true;
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                for (int j = 0; j < dataGridView1.ColumnCount; j++)
                {
                    char cell = Convert.ToChar(dataGridView1.Rows[i].Cells[j].Value);
                    char s = c.grid.grid[i][j];
                    if (cell != ' ' && cell != s && s != '#')
                        correct = false;
                }
            }
            if (correct)
            {
                MessageBox.Show("Felicidades!!!! Todo está correcto :)");
                Close();
            }
            else
                MessageBox.Show("Algo esta mal, intenta de Nuevo :(");
        }
    }

    public class Word
    {
        public char[] word; //Almacena la palabra
        public int length; //Alamacena la longitud de la palabra
        public List<bool> validFlags; //Almacena las banderas para saber si se puede usar una letra
        public bool orientation; //true para Horizontal, false para vertical
        public bool used;
        public Tuple<int, int> position;
        public int id; //Se almacena el numero de palabra para las pistas

        public Word(string _word)
        {
            _word = _word.ToLower();
            length = _word.Length;
            word = new char[length + 2];
            word[0] = '#';
            word[length - 1] = '.';
            for (int i = 0; i < length; i++) word[i + 1] = _word[i];
            length += 2; ; //Se ajusta el largo de la palabra tomando en cuenta que se agrego el id
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
    };

    public class Grid
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
            firstWord.id = 1;
            firstWord.orientation = Convert.ToBoolean(Program.rnd.Next(2));
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
                if (!(x1 < 0 || y1 < 0) && grid[x1][y1] != ' ')
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

            //Tras pasar todas las validaciones se dibuja la palabra en el grid
            //Se asigna la posicion x,y a la palabra a insertar
            newWord.position = new Tuple<int, int>(x, y);
            contID++;
            newWord.id = contID;
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

    public class Crossword
    {
        public List<Word> words;
        public List<string> clues; //Pistas de las palabras
        public List<string> sortedClues;
        public int contID;
        public Grid grid;
        public Crossword()
        {
            grid = new Grid(30, 30);
            words = new List<Word>();
            contID = 1;
            clues = new List<string>();
            sortedClues = new List<string>();
        }
        public void GetWords() // Recupera las palabras de una base de datos o archivo
        {
            try
            {
                StreamReader wordFile;

                wordFile = File.OpenText("palabras.txt");
                for (int i = 1; i <= 40; i++)
                {
                    if (i % 2 == 1)
                    {
                        Word a = new Word(wordFile.ReadLine()); //Falta try-catch                              !!!!!
                        words.Add(a);
                    }
                    else
                    {
                        clues.Add(wordFile.ReadLine());
                    }
                }
                wordFile.Close();
            }
            catch(FileNotFoundException e)
            {
                MessageBox.Show("No se encontró el archivo, utilizando palabras por defecto");
                string[] palabras = new string[] {
                    "Mintonette",
                    "Nombre original del Voleibol.",
                    "ymca",
                    "Siglas de la universidad donde se inventa el Voleibol.",
                    "libero",
                    "Posicion en voleibol cuya especialidad es la defensa.",
                    "remate",
                    "En voleibol, movimiento de ataque mas comun.",
                    "set",
                    "Conjunto de 25 puntos en voleibol.",
                    "saque",
                    "Movimiento que da inicio a un punto o rally.",
                    "bloquear",
                    "Accion de parar un ataque del equipo contrario por encima de la red.",
                    "linea",
                    "El arbitro de _____ es el encargado de determinar si un punto cae dentro de la cancha.",
                    "fivb",
                    "Siglas de la organizacion mundial encargada de regular las normas del Voleibol.",
                    "zaguero",
                    "Se le llama ataque de _______ al que se realiza detras de la linea de 3 metros.",
                    "playa",
                    "El voleibol de _____ se juega sobre arena y con dos jugadores.",
                    "antenas",
                    "(pl.) Varillas de 80 cm a cada lado de la red y sirven como delimitacion de la zona de juego sobre la red.",
                    "rotacion",
                    "Movimiento en sentido de las manecillas del reloj que se realiza cuando se arrebata el saque al contrario.",
                    "red",
                    "Se encuentra exactamente en el centro de la cancha dividiendo las areas de cada equipo.",
                    "tres",
                    "Numero maximo de toques que puede dar un equipo antes de pasar el balon.",
                    "sentado",
                    "El voleibol _______ es una variante con popularidad entre los deportes para discapacitados.",
                    "punto",
                    "Asi se le llama a una anotacion de un equipo.",
                    "tiempos",
                    "Cada equipo puede solicitar hasta dos _______ de descanso de 30 segundos en cada set.",
                    "muerta",
                    "Cuando un punto es incierto y se repite, se le conoce como bola ______.",
                    "cambio",
                    "(sus.)Se le llama ______ a sacar a un jugador en cancha y reemplazarlo por otro. Se tienen 6 maximo por set"
                };
                for (int i = 1; i <= 40; i++)
                {
                    if (i % 2 == 1)
                    {
                        Word a = new Word(palabras[i]); //Falta try-catch                              !!!!!
                        words.Add(a);
                    }
                    else
                    {
                        clues.Add(palabras[i]);
                    }
                }

            }
        }

        public void ConstructionAlgorithm(int nWords) //Algoritmo para seleccionar palabras y sus cruces
        {
            //words.Sort((Word x, Word y) => y.length - x.length); //Ordenamos el array por 
            Word firstWord = words[0];
            foreach (var word in words)
            {
                if (word.length > firstWord.length)
                    firstWord = word;
            }
            grid.DrawWord(firstWord); //Dibuja la palabra en el centro del grid

            int posOrig = words.IndexOf(firstWord);
            sortedClues.Add(clues[posOrig]);

            Word secondWord;
            firstWord.used = true;
            bool allInserted = false;
            //Se debe crear un ciclo que recorra a todas las palabras y que asigne a la ultima palabra
            //el estatus de padre, pero que si el ciclo para asiganarle falle, pueda seleccionar otra
            //palabra sin problema
            //Que pasa si una palabra no coincide con niguna otra palabra en ningun caracter?
            // Se crea un contador que cuenta cuantas veces se trata de insertar una palabra, si
            //supera el numero de palabras *2, se cambia la palabra padre por una de las anteriormente
            //insertadas buscando que encaje ahi
            int tryCounter = 0;
            int worstCaseCounter = 0;
            while (!allInserted && contID <= nWords -1)
            {
                if (tryCounter > words.Count * 2)
                {
                    if (tryCounter < 60) {
                        do
                        {
                            firstWord = words[Program.rnd.Next(0, words.Count)];
                        } while (!(firstWord.used && firstWord.IsUsable()));
                        tryCounter = 0;
                        worstCaseCounter++;
                    }
                }

                if (worstCaseCounter > 10)
                { 
                    //Durante la fase de pruebas hay algunas veces en las que las palabras se acomodan 
                    //de una manera en la que no entra ninguna otra
                    //esto es el ultimo recurso, si no encuentra ningun espacio
                    //pone todo como empezó y vuelve a llamar a la función
                    for (int i = 0; i < words.Count(); i++)
                    {
                        words[i].used = false;
                        for (int j = 1; j < words[i].validFlags.Count() - 1; j++)
                        {
                            words[i].validFlags[j] = true;
                        }
                        words[i].id = 0;
                    }

                    for (int i = 0; i < grid.grid.Count(); i++)
                    {
                        for (int j = 0; j < grid.grid[i].Count(); j++)
                        {
                            grid.grid[i][j] = ' ';
                        }
                    }
                    sortedClues.Clear();
                    contID = 1;
                    ConstructionAlgorithm(nWords);
                    return;
                }


                do
                {
                    secondWord = words[Program.rnd.Next(0, words.Count)];
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

                    posOrig = words.IndexOf(secondWord);
                    sortedClues.Add(clues[posOrig]);
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

            for (int i = 0; i < words.Count(); i++)
            {
                if (words[i].used)
                {
                    int x = words[i].position.Item1 + (words[i].orientation ? words[i].length - 1 : 0);
                    int y = words[i].position.Item2 + (!words[i].orientation ? words[i].length - 1 : 0);
                    grid.grid[x][y] = ' ';
                }
            }
        }
    }
}
