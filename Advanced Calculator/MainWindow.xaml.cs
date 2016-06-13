using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections;
using System.Diagnostics;
using System.Media;
using Microsoft.Win32;

namespace Advancedcalculator
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 
    
    public class container //представляет собой контейнер типизированных списков для чисел и операторов
    {
        public container()
        {
            
            tempoperand.Add("");
            operands.Add(0);
            operators.Add(' ');
            names.Add("");
        }
        public char[] basicoperators = new char[] { '+', '-', '/', '*'};//представляет массив который содержит базовые операторы
        public List<double> operands = new List<double>();//типизированный список чисел которые мы вводим.
        public List<string> tempoperand = new List<string>();//временный список чисел который мы потом конвертируем в operands
        public List<char> operators = new List<char>();//список математических операторов
        public List<int> brecketindicators = new List<int>();
        public List<string> names = new List<string>();
          
    }
    public partial class MainWindow : Window
    {
        private MediaPlayer mediaPlayer = new MediaPlayer();
        public MainWindow()
        {
            InitializeComponent();
            variables.Add(new container());
            basicmath.Add('+', (i, j) => i + j);
            basicmath.Add('-', (i, j) => i - j);
            basicmath.Add('*', (i, j) => i * j);
            basicmath.Add('/', (i, j) => i / j);
            
        }
       
        Dictionary<char, Func<double, double, double>> basicmath = new Dictionary<char, Func<double, double, double>>();
        int i = 0;
        int k=1;
        double israd = 1;
         
        List<container> variables = new List<container>();//список типа conteiner.

       
        private void dodigit<T>(object sender, T e)//метод который добаляет нажатый символ на экран,а потом записывает в tempoperand.
        {
            if (Display.Text.Length > 0)
            {
                if (((Char.IsPunctuation(Convert.ToChar(e)) && variables[k - 1].tempoperand[i].Contains(",") == false) || Char.IsDigit(Convert.ToChar(e))) && Display.Text[Display.Text.Length - 1] != ')')
                {
                    Display.Text += e;
                    variables[k - 1].tempoperand[i] += e.ToString();
                }
            }
            else
            {
                if((Char.IsPunctuation(Convert.ToChar(e)))==false)
                {
                Display.Text += e;
                variables[k - 1].tempoperand[i] += e.ToString();
                }
            }
           
        }
        private void dooperator<T>(object sender, T e)//метод который добаляет нажатый оператор на экран.
        {
            if (Display.Text.Length > 0)
            {
                if (variables[k - 1].basicoperators.Contains(Display.Text[Display.Text.Length - 1]) == false && Display.Text[Display.Text.Length-1]!='(')
                {
                    Display.Text += e;
                    variables[k - 1].operands.Add(0);//Создаём новый пустой элемент списка
                    variables[k - 1].operators.Add(' ');//Создаём новый пустой элемент списка
                    variables[k - 1].tempoperand.Add("");//Создаём новый пустой элемент списка
                    variables[k - 1].names.Add("");
                    variables[k - 1].operands[i] = Convert.ToDouble(variables[k - 1].tempoperand[i]);
                    variables[k - 1].operators[i] = Convert.ToChar(e);
                    i = i + 1;
                }
            }
        }
        
        private void doequal<T>(object sender, T e)
        {
            if (Display.Text.Length > 0 && variables[k - 1].basicoperators.Contains(Display.Text[Display.Text.Length - 1]) == false && Display.Text[Display.Text.Length - 1] != '(')
            {
                variables[k - 1].operands[i] = Convert.ToDouble(variables[k - 1].tempoperand[i]);
                variables[k - 1].operands[0] = calculator(variables[k - 1].operands, variables[k - 1].operators);
                if (k == 1)
                    Display.Text = Convert.ToString(variables[k - 1].operands[0]);
                variables[k - 1].operators.Clear();
                variables[k - 1].tempoperand.Clear();
                variables[k - 1].tempoperand.Add("");
                variables[k - 1].tempoperand[0] = Convert.ToString(variables[k - 1].operands[0]);
            }

        }
        private void doclear<T>(object sender,T e)
        {
            Display.Text="";

            variables.Clear();
            variables.Add(new container());
            k = 1;
            i = 0;
        }
        private void doreg<T>(object sender,T e)
        {

            if (Display.Text.Length > 0)
            {
                if (Char.IsDigit(Convert.ToChar(Display.Text[Display.Text.Length - 1])))
                {
                    if (variables[k - 1].tempoperand[i].Contains("-") == false)
                    {
                        variables[k - 1].tempoperand[i] = variables[k - 1].tempoperand[i].Remove(variables[k - 1].tempoperand[i].Length - 1);
                        Display.Text = Display.Text.Remove(Display.Text.Length - 1);
                    }

                }
            }
        
        }
        private void doopenbracket<T>(object sender, T e)
        {
            if (Display.Text.Length > 0)
            {
                if (Char.IsDigit(Convert.ToChar(Display.Text[Display.Text.Length - 1])) && Display.Text[Display.Text.Length - 1] != ')')
                {


                }
                else
                {
                    variables[0].brecketindicators.Add(i);
                    i = 0;
                    Display.Text += "(";
                    variables.Add(new container());
                    k = k + 1;
                }
            }
            else
            {
                variables[0].brecketindicators.Add(i);
                i = 0;
                Display.Text += "(";
                variables.Add(new container());
                k = k + 1;
            }
        }
        private void doclosebracket<T>(object sender, T e)
        {
            if ((Display.Text.Count(x => x == '(') - Display.Text.Count(x => x == ')')) >= 1 && Display.Text[Display.Text.Length - 1] != '(' && variables[0].basicoperators.Contains(Display.Text[Display.Text.Length - 1])==false)
            {
            variables[k - 1].names.Add("");
            Display.Text += e;
            doequal(sender, e);
            i = variables[0].brecketindicators[variables.Count-2];

            switch (variables[k - 1].names[i])
            {
                case "Sin": dosinus(sender, e); break;
                case "Cos": docosinus(sender, e); break;
                case "√": calcsqrt(sender, e); break;
            }

            variables[k - 2].operands[i] = variables[k - 1].operands[0];
            variables[k - 2].tempoperand[i] = Convert.ToString(variables[k - 1].operands[0]);
           

            variables[k - 1].tempoperand.Clear();
            variables[k - 1].tempoperand.Add("");
            k = k - 1;
            }
       
        }
        private void dochangesign<T>(object sender, T e)
        {
            if (Display.Text.Length > 0)
            {
                if (variables[k - 1].basicoperators.Contains(Display.Text[Display.Text.Length - 1]) == false && Display.Text[Display.Text.Length - 1] != '(' && Display.Text[Display.Text.Length - 1] != ')')
                {
                    if (variables[k - 1].tempoperand[i].Contains("-"))
                    {
                        variables[k - 1].tempoperand[i] = variables[k - 1].tempoperand[i].Replace("-", "");
                        Display.Text = Display.Text.Remove(Display.Text.Length - variables[k - 1].tempoperand[i].Length - 3);
                        Display.Text += variables[k - 1].tempoperand[i];
                    }
                    else
                    {
                        Display.Text = Display.Text.Remove(Display.Text.Length - variables[k - 1].tempoperand[i].Length);
                        variables[k - 1].tempoperand[i] = variables[k - 1].tempoperand[i].Insert(0, "-");
                        Display.Text += "(" + variables[k - 1].tempoperand[i] + ")";
                    }
                }
            }
        }
        private void tricalc<T>(object sender, T e)
        {
              switch(e.ToString())
              {
                  case "Sin": dosinus(sender, e); break;
                  case "Cos": docosinus(sender, e); break;
              }
        }
        private void displaytrig<T>(object sender, T e)
        {
           
            if (Display.Text.Length > 0)
            {
                if ((Char.IsDigit(Display.Text[Display.Text.Length - 1]) || variables[k - 1].tempoperand[variables[k - 1].tempoperand.Count-1].Contains("-")))
                {
                    if (variables[k - 1].tempoperand[variables[k - 1].tempoperand.Count - 1].Contains("-"))
                    {
                        Display.Text = Display.Text.Insert(Display.Text.Length - variables[k - 1].tempoperand[i].Length-1, e.ToString() + "(");
                    }
                    else
                    {
                        Display.Text = Display.Text.Insert(Display.Text.Length - variables[k - 1].tempoperand[i].Length, e.ToString() + "(");
                    }
                   
                    Display.Text += ")";
                    variables[k - 1].operands[i] = Convert.ToDouble(variables[k - 1].tempoperand[i]);

                    tricalc(sender,e);
                }
                else
                {
                    Display.Text += e;

                    doopenbracket(sender, "(");
                }
            }
            else
            {
                Display.Text += e;

                doopenbracket(sender, "(");
            }
            variables[k - 1].names[i] = e.ToString();
         
           
           
        }
        private void dosinus<T>(object sender, T e)
        {

            variables[k - 1].tempoperand[i] = Convert.ToString(Math.Round(Math.Sin(variables[k - 1].operands[i] * israd), 3));
            variables[k - 1].operands[i] = Convert.ToDouble(variables[k - 1].tempoperand[i]);
        }
        private void docosinus<T>(object sender, T e)
        {
            variables[k - 1].tempoperand[i] = Convert.ToString(Math.Round(Math.Cos(variables[k - 1].operands[i] * israd), 3));
            variables[k - 1].operands[i] = Convert.ToDouble(variables[k - 1].tempoperand[i]);
        }
        private void calcsqrt<T>(object sender, T e)
        {
            variables[k - 1].tempoperand[i] = Convert.ToString(Math.Round(Math.Sqrt(variables[k - 1].operands[i]), 3));
            variables[k - 1].operands[i] = Convert.ToDouble(variables[k - 1].tempoperand[i]);
        }
        private void dosqrt<T>(object sender, T e)
        {
            
            if (Display.Text.Length > 0  )
            {
                if (Char.IsDigit(Display.Text[Display.Text.Length - 1]))
                {
                    Display.Text = Display.Text.Insert(Display.Text.Length - variables[k - 1].tempoperand[i].Length, e.ToString() + "(");
                    Display.Text += ")";
                    variables[k - 1].operands[i] = Convert.ToDouble(variables[k - 1].tempoperand[i]);
                    calcsqrt(sender,e);
                }
                else
                {
                    Display.Text += e;

                    doopenbracket(sender, "(");
                }
            }
            else
            {

                Display.Text += e;

                doopenbracket(sender, "(");
            }
            variables[k - 1].names[i] = e.ToString();
         
        }
        
        private void Button_Click_digit(object sender, RoutedEventArgs e)//обработчик нажатия цифры или запятой
        {
            dodigit(sender, (e.OriginalSource as Button).Content);
        }

        
        private void Someoperator(object sender, RoutedEventArgs e)//обработчик нажатия кнопки оператора +-*/
        {
            dooperator(sender,(e.OriginalSource as Button).Content);
        }

        private void Equal(object sender, RoutedEventArgs e)
        {
            doequal(sender,"=");
        }

        private void Clear(object sender, RoutedEventArgs e)
        {
            doclear(sender, e);
        }

       
        private double calculator(List<double> a,List<char> b)
        {
            for (int l=0;l<i;l++)
            {
               if (b[l]=='/' || b[l]=='*')
               {
                   a[l] = basicmath[b[l]].Invoke(a[l],a[l+1]);
                   b.RemoveAt(l);
                   for (int j = l + 1; j < i ; j++ )
                   {
                       a[j] = a[j + 1];
                   }
                   a.RemoveAt(i);
                   i = i-1;
                   
                   l = -1;
               }
            }
            
            for (int l = 0; l < i; l++)
            {
                a[l] = basicmath[b[l]].Invoke(a[l], a[l + 1]);
                a.RemoveAt(l+1);
                b.RemoveAt(l);
                i = i - 1;
                l = -1;
                
            }
            return a[0];
        }
        private void Reg(object sender, RoutedEventArgs e)
        {
            doreg(sender,e);
        }

        private void openbracket(object sender, RoutedEventArgs e)
        {
            doopenbracket(sender, (e.OriginalSource as Button).Content);
        }

        private void closebracket(object sender, RoutedEventArgs e)
        {
            doclosebracket(sender, (e.OriginalSource as Button).Content);

        }
        private void plusminus(object sender, RoutedEventArgs e)
        {
            dochangesign(sender, e);
        }
        private void sinus(object sender, RoutedEventArgs e)
        {
            displaytrig(sender, (e.OriginalSource as Button).Content);
        }
        private void sqrt(object sender, RoutedEventArgs e)
        {
            dosqrt(sender, (e.OriginalSource as Button).Content);
        }
        private void iskeypressed(object sender, KeyEventArgs e)
        {
           
            if ((Keyboard.Modifiers == ModifierKeys.Shift) && (e.Key == Key.OemPlus))
            {
                dooperator(sender, "+");
                return;
            }
            if ((Keyboard.Modifiers == ModifierKeys.Shift) && (e.Key == Key.D9))
            {
                doopenbracket(sender, "(");
                return;
            }
            if ((Keyboard.Modifiers == ModifierKeys.Shift) && (e.Key == Key.D0))
            {
                doclosebracket(sender, ")");
                return;
            }
            if ((Keyboard.Modifiers == ModifierKeys.Shift) && (e.Key == Key.D8))
            {
                dooperator(sender, "*");
                return;
            }
            var ch = (char)KeyInterop.VirtualKeyFromKey(e.Key);//преоразует клавишу System.Windows.Input.Key в windows 32 key.
            if (Char.IsDigit(ch))
            {
                dodigit(sender, ch);
            }
            switch (e.Key)
            {
                case Key.OemPlus: doequal(sender, e); break;
                case Key.OemMinus: dooperator(sender,"-"); break;
                case Key.Divide: dooperator(sender, "/"); break;
                case Key.Return: doequal(sender, e); break;
                case Key.Back: doreg(sender, e); break;
                case Key.OemComma :case Key.OemPeriod: dodigit(sender, ","); break;
                case Key.Escape: doclear(sender, e); break;
                case Key.OemQuestion: dooperator(sender, "/"); break;
                case Key.S: displaytrig(sender, "Sin"); break;

            }
            
        }
        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            if (Deg.IsChecked == true)
            {
                israd = 0.0174532925;
            }
            else
            {
                israd = 1;
            }
        }

    }
}
