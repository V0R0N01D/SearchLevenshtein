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

namespace Searc
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            using (var dataBase = new BaseDateEntities())
            {
                // запрос данных из бд
                var quertyView = from bace in dataBase.Product
                                 join ProductTypes in dataBase.ProductType on bace.ProductTypeID equals ProductTypes.ID
                                 select new
                                 {
                                     titleProduct = bace.Title,
                                     productTypes = ProductTypes.Title,
                                     articleNumber = bace.ArticleNumber,
                                     personCount = bace.ProductionPersonCount,
                                     workshopNum = bace.ProductionWorkshopNumber,
                                     costAgent = bace.MinCostForAgent
                                 };

                // передача данных в ListView
                LVCar.ItemsSource = quertyView.ToList();
            }
        }

        // метод который работает когда строка поиска изменяется
        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            using (var dataBase = new BaseDateEntities())
            {
                // запрос данных из бд
                var quertyViewChange = from bace in dataBase.Product
                                       join ProductTypes in dataBase.ProductType on bace.ProductTypeID equals ProductTypes.ID
                                       select new
                                       {
                                           titleProduct = bace.Title,
                                           productTypes = ProductTypes.Title,
                                           articleNumber = bace.ArticleNumber,
                                           personCount = bace.ProductionPersonCount,
                                           workshopNum = bace.ProductionWorkshopNumber,
                                           costAgent = bace.MinCostForAgent
                                       };

                // если строка пустая то выводятся все данные
                if (string.IsNullOrWhiteSpace(Search.Text) == true)
                {
                    LVCar.ItemsSource = quertyViewChange.ToList();
                }
                // если в строке что-то написано, то сортировка по этому слову
                else
                {
                    var Seh = quertyViewChange.ToList();
                    foreach(var s in quertyViewChange.ToList())
                    {
                        // если расстояние левенштейна больше 2 то элемент удаляется
                        if(LevenshteinDistance(s.productTypes.ToUpper(), Search.Text.ToUpper()) > 2)
                        {
                            Seh.Remove(s);
                        }
                    }
                    LVCar.ItemsSource = Seh;
                }
            }
        }

        // вычисление минимального числа
        static int Minimum(int a, int b, int c)
        {
            if (a > b)
            {
                a = b;
            }

            if (a > c)
            {
                a = c;
            }

            return a;
        }

        // вычисление расстояния левенштейна
        static int LevenshteinDistance(string firstWord, string secondWord)
        {
            var n = firstWord.Length + 1;
            var m = secondWord.Length + 1;
            var matrixD = new int[n, m];

            const int deletionCost = 1;
            const int insertionCost = 1;

            for (var i = 0; i < n; i++)
            {
                matrixD[i, 0] = i;
            }

            for (var j = 0; j < m; j++)
            {
                matrixD[0, j] = j;
            }

            for (var i = 1; i < n; i++)
            {
                for (var j = 1; j < m; j++)
                {
                    var substitutionCost = firstWord[i - 1] == secondWord[j - 1] ? 0 : 1;

                    matrixD[i, j] = Minimum(matrixD[i - 1, j] + deletionCost,
                                            matrixD[i, j - 1] + insertionCost,
                                            matrixD[i - 1, j - 1] + substitutionCost);
                }
            }

            return matrixD[n - 1, m - 1];
        }
    }
}