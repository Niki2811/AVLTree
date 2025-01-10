using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AVLTree_Kursova
{
    public partial class Form1 : Form
    {

        private AVLTree tree; //Дефинираме AVL дървото

        public Form1()
        {
            tree = new AVLTree(); //Инициализиране на дървото
            InitializeComponent();
        }

        public class Node //Node e възел, който представлява един елемент от дървото
        {
            public int Key { get; set; } //Цяло число, стойността за конкретния възел.
                                         //Основна стойност, която се съхранява в дървото

            public int Height { get; set; }  //Цяло число, което показва височината на възела.
                                             //Най-голямото число от въведените е върха на дървото.

            public Node Left { get; set; }   //Указател към левия клон на дървото за този възел

            public Node Right { get; set; }  //Указател към десния клон на дървото за този възел

            public Node(int key) //Конструкторът създава нов възел в дървото
            {                    //Приема ключа на възела и задава начална височина 1,
                                 //защото няма създадени клони
                Key = key;       //Left & Right = null
                Height = 1;

            }

        }

        public class AVLTree
        {
            private Node Root; //Това е началната точка на дървото. От него започват всички операции.

            public Node GetRoot
            {
                get { return Root; }
            }

            private int Height(Node node)
            {
                if (node == null)
                {
                    return 0;
                }
                return node.Height;
            }

            private int GetBalance(Node node)
            {
                if (node == null)
                {
                    return 0;  // Ако възелът е null, балансът е 0
                }
                else
                {
                    int leftHeight = Height(node.Left);   // Височина на лявото поддърво
                    int rightHeight = Height(node.Right); // Височина на дясното поддърво

                    return leftHeight - rightHeight;  // Разликата между височините е балансът
                }
            }


            private Node RotateRight(Node y) //Извършва дясн завъртане на дървото около възел 'y'.
            {
                Node x = y.Left;
                Node T2 = x.Right; //T2 конкретно указва второто поддърво, което е свързано с въртенето.
                                   //Временен storage за част от дървото, който се мести от една позиция в друга.

                x.Right = y;
                y.Left = T2;

                y.Height = Math.Max(Height(y.Left), Height(y.Right)) + 1; //Тези редове актуализират височината на възлите y и x след завъртане. 
                x.Height = Math.Max(Height(x.Left), Height(x.Right)) + 1; //Височината на един възел се изчислява като 1 плюс максималната височина на двата клона.
                                                                          //AVL дървото, трябва да остане балансирано след всяка операция.               
                return x; //Методът връща x, защото след извършването на ротацията, x става новият корен на клона.
            }

            private Node RotateLeft(Node x)
            {
                Node y = x.Right;
                Node T2 = y.Left;

                y.Left = x;
                y.Left = x;
                x.Right = T2;

                x.Height = Math.Max(Height(x.Left), Height(x.Right)) + 1; //Идентично като за RotateRight, но се връща 'y' и след ротацията, 'y' е новият корен на клона.
                y.Height = Math.Max(Height(y.Left), Height(y.Right)) + 1;

                return y;

            }

            public void Delete(int key)
            {
                Root = DeleteNode(Root, key);
            }

            private Node GetMinValueNode(Node node)
            {
                Node current = node;

                while (current.Left != null)
                {
                    current = current.Left;
                }
                return current;
            }

            public void Insert(int key) //Вмъква нова стойност в дървото
            {
                Root = InsertNode(Root, key);//Започва от Root възела и намира подходящата позиция за новия възел
                                             //Проверява дали дървото остава балансирано след вмъкването. 
                                             //При баланс > 1 извършва дясно завъртане
                                             //При баланс < -1 извършва ляво завъртане
                                             //Ако проблемът е смесен, прави комбинирани завъртания.
            }

            private Node InsertNode(Node node, int key) //Тук добавяме ключ в дървото и целта е чрез него то да остане балансирано
            {
                if (node == null)       //Ако възелът е null, тогава сме до мястото където трябва да се създаде нов възел с key
                    return new Node(key);//Връща новият възел и се връзва с родителския

                if (key < node.Key)                        //Проверява дали key е по-малко или по-голямо от текущия възел (node.Key)   
                    node.Left = InsertNode(node.Left, key);//Ако key < node.Key, извиква рекурсивно метода за лявото поддърво.
                else if (key > node.Key)                   //Ако key > node.Key, извиква рекурсивно метода за дясното поддърво.
                    node.Right = InsertNode(node.Right, key);//Ако key == node.Key, не прави нищо (AVL дървото не позволява дубликати).
                else
                    return node;

                node.Height = 1 + Math.Max(Height(node.Left), Height(node.Right)); //Актуализиране на височината. 

                int balance = GetBalance(node); //Балансът е разликата във височините на лявото и дясното поддърво

                //Ако балансът е нарушен, методът извършва една от четирите възможни ротации.
                if (balance > 1 && key < node.Left.Key) //лявото поддърво е по-високо и се прави дясно завъртане
                    return RotateRight(node);

                if (balance < -1 && key > node.Right.Key)//Дясното поддърво е по-високо и се правя лява ротация
                    return RotateLeft(node);

                if (balance > 1 && key > node.Left.Key)//Лявото поддърво е по-високо, но новия key е в дясното поддърво на лявото дете
                {                                      //Прави се лява ротация на лявото дете и дясна ротация на текущия възел
                    node.Left = RotateLeft(node.Left);
                    return RotateRight(node);
                }

                if (balance < -1 && key < node.Right.Key)//Дясното поддърво е по-високo, но новия key е в лявото поддърво на дясното дете
                {                                        //Прави се дясна ротация на дясното дете и лява ротация на текущия възел.
                    node.Right = RotateRight(node.Right);
                    return RotateLeft(node);
                }

                return node; //Връща възел
            }

            private Node DeleteNode(Node node, int key)
            {
                if (node == null)
                    return null; 
                
                if (key < node.Key)
                    node.Left = DeleteNode(node.Left, key);
                else if (key > node.Key)
                    node.Right = DeleteNode(node.Right, key);
                else
                {
                    
                    if (node.Left == null || node.Right == null)
                    {
                        
                        Node temp = node.Left != null ? node.Left : node.Right;

                        
                        if (temp == null)
                        {
                            temp = node;
                            node = null;
                        }
                        else
                            node = temp; 

                        temp = null; 
                    }
                    else
                    {
                        
                        Node temp = GetMinValueNode(node.Right); 

                        node.Key = temp.Key; 

                        
                        node.Right = DeleteNode(node.Right, temp.Key);
                    }
                }

                if (node == null)
                    return null; 
                node.Height = Math.Max(Height(node.Left), Height(node.Right)) + 1;
                
                int balance = GetBalance(node);

                if (balance > 1 && GetBalance(node.Left) >= 0)
                    return RotateRight(node);
                if (balance > 1 && GetBalance(node.Left) < 0)
                {
                    node.Left = RotateLeft(node.Left);
                    return RotateRight(node);
                }            
                if (balance < -1 && GetBalance(node.Right) <= 0)
                    return RotateLeft(node);
                if (balance < -1 && GetBalance(node.Right) > 0)
                {
                    node.Right = RotateRight(node.Right);
                    return RotateLeft(node);
                }
                return node;
            }
       
            private Node SearchNode(Node node, int key) //Методът търси възел с даден ключ в дървото.
                                                        //Ако намери такъм възел, го връща. Ако ключът не съществува, връща null
            {
                if (node == null || node.Key == key) //Ако текущият възел е null, значи сме стигнали до празен възел – ключът не е намерен
                    return node;                     //Ако текущият възел съдържа ключа, връщаме този възел.
                                                     //Ако ключът е по-малък от текущия възел, търсим в лявото поддърво. Ако е по-голям, търсим в
                if (key < node.Key)                  //дясното поддърво
                    return SearchNode(node.Left, key);
                else
                    return SearchNode(node.Right, key);
            }

            public bool Search(int key)//Проверява дали дадена стойност съществува в дървото.
            {                          //Започва от Root и сравнява.
                                       //Ако ключът е по-малък, търси в левия клон
                                       //Ако е по-голям, търси в десния клон
                                       //Ако намери възела(node) връща истина. Ако не, лъжа.
                return SearchNode(Root, key) != null;
            }

        }

        //Добавяне на стойност в AVL дървото 
        private void btnInsert_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtInput.Text, out int key))//Проверява дали въведеният текст в txtInput (текстовото поле) може да бъде преобразуван в цяло число (key).//Ако е успешно, методът продължава; ако не, показва съобщение за грешка.
            {
                tree.Insert(key);                        //Извиква метода за вмъкване в AVL дървото, добавяйки нов възел с ключа key
                txtInput.Clear();                        //lstOutput.Items.Add($"Вмъкната стойност: {key}"); //Print на добавената стойност
                                                         //Изчиства текстовото поле след обработката. Така се разбира, че стойността е вече приета в списъка.
                if (!listBoxNumbers.Items.Contains(key)) //Иначе трябва да се трие ръчно от потребителя.
                {
                    listBoxNumbers.Items.Add(key);
                }
            }
            else
            {
                MessageBox.Show("Въведете цяло число.", "Грешка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtInput.Text, out int key)) //Проверява дали текстът в txtInput може да се преобразува в цяло число key
            {
                bool found = tree.Search(key); //Ако ключът е валиден, търси конкретен ключ в дървото
                MessageBox.Show(found ? $"Стойността {key} е намерена." : $"Стойността {key} не е намерена.");
                txtInput.Clear();
            }
            else
            {
                MessageBox.Show("Въведете цяло число.", "Грешка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PrintTreeToTreeView(Node node, TreeNode treeNode)
        {
            if (node != null)
            {
                TreeNode newNode = new TreeNode(node.Key.ToString());
                //TreeNode е клас за представяне на възел в TreeView

                if (treeNode == null)
                {
                    treeViewOutput.Nodes.Add(newNode); // Добавяме новия възел като коренов възел
                }
                else
                {
                    treeNode.Nodes.Add(newNode); // Добавяме дъщерен възел(newNode) към родителския възел
                }

                PrintTreeToTreeView(node.Left, newNode); // За лявото поддърво
                PrintTreeToTreeView(node.Right, newNode); // За дясното поддърво
            }
        }

       
        public void VisualizeTreeInTreeView()
        {
            treeViewOutput.Nodes.Clear(); 
            PrintTreeToTreeView(tree.GetRoot, null);
        }
         
        private void btnPrint_Click(object sender, EventArgs e)
        {
            treeViewOutput.Nodes.Clear();
            VisualizeTreeInTreeView();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtInput.Text, out int key)) // Проверява дали въведеният текст в txtInput може да бъде преобразуван в цяло число (key)
            {
                tree.Delete(key);           // Извиква метода за изтриване на стойността от AVL дървото
                VisualizeTreeInTreeView(); // Обновява визуализацията на дървото
                txtInput.Clear();           // Изчиства текстовото поле
            }
            else
            {
                MessageBox.Show("Въведете цяло число.", "Грешка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    //Ротациите са така измислени, че да запазват основното свойство на
    //двоичното дърво, а именно - за всеки връх стойностите във всички върхове от
    //лявото му поддърво да са по-малки от него, а стойностите от дясното му
    //поддърво да са по-големи или равни на него.
}
