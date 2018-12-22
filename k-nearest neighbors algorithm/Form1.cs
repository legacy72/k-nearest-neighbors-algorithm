using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace k_nearest_neighbors_algorithm
{
    public partial class Form1 : Form
    {
        static List<Tuple<double[], int>> listOfFruits = new List<Tuple<double[], int>>();
        static List<double[]> trainingSet = new List<double[]>();
        static List<int> trainigSetHealthy = new List<int>();
        static Bitmap bmp;
        static Graphics g;

        public Form1()
        {
            InitializeComponent();

            FillListOfFruits();
            Training();

            bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g = Graphics.FromImage(bmp);
            Draw(bmp, g, 250, 10, "black");

            FillPictureBox(bmp);
        }

        void FillPictureBox(Bitmap bmp)
        {
            pictureBox1.Image = bmp;
        }

        // инициализация объектов класса
        void FillListOfFruits()
        {
            listOfFruits.Add(new Tuple<double[], int>(new double[] { 6.6, 35.8, 46.8 }, 1));
            listOfFruits.Add(new Tuple<double[], int>(new double[] { 1.2, 0.1, 8.1 }, 2));
            listOfFruits.Add(new Tuple<double[], int>(new double[] { 4.1, 8.3, 1.2 }, 2));
            listOfFruits.Add(new Tuple<double[], int>(new double[] { 3.4, 30.2, 64.7 }, 1));
            listOfFruits.Add(new Tuple<double[], int>(new double[] { 4.7, 9.3, 3.4 }, 2));
            listOfFruits.Add(new Tuple<double[], int>(new double[] { 1.7, 50, 40.8 }, 1));
            listOfFruits.Add(new Tuple<double[], int>(new double[] { 11.2, 18.7, 2.9 }, 0));
            listOfFruits.Add(new Tuple<double[], int>(new double[] { 12, 11.2, 1.4 }, 0));
        }

        // создание тренировочных данных
        void Training()
        {
            foreach (var fruit in listOfFruits)
            {
                Fruit f = new Fruit(fruit.Item1[0], fruit.Item1[1], fruit.Item1[2], fruit.Item2);

                trainingSet.Add(f.Method());
                trainigSetHealthy.Add(f.SetListHealthy());
            }
        }

        // расчет эвклидового пространства
        double EuclideanDistance(double[] a, double[] b)
        {
            double res = 0;
            for (int i = 0; i < a.Length; i++)
            {
                res += Math.Pow(a[i] - b[i], 2);
            }

            return Math.Sqrt(res);
        }

        // Лист расстояний от классифицируемого продукта
        private List<double> GetListOfDistances(double[] newFruitParams)
        {
            double distance = 0.0;

            List<double> distances = new List<double>();
            g.Clear(Color.White);
            Draw(bmp, g, 250, 10, "black");

            for (int i = 0; i < trainingSet.Count; i++)
            {
                distance = EuclideanDistance(newFruitParams, trainingSet[i]);

                float k = 2.5f;
                try
                {
                    if (trainigSetHealthy[i] == 0)
                        Draw(bmp, g, 250, 10 + ((float)distance * k), "blue");
                    else if (trainigSetHealthy[i] == 1)
                        Draw(bmp, g, 250 + ((float)distance * k), 10, "red");
                    else if (trainigSetHealthy[i] == 2)
                        Draw(bmp, g, 250 - ((float)distance * k), 10, "green");
                    
                    FillPictureBox(bmp);
                }
                catch {; }
                distances.Add(distance);
            }

            return distances;
        }

        // индексы ближайщих k соседей
        List<int> GetIndexesOfNearestNeighbors(List<double> distances, int k)
        {
            List<int> indexes = new List<int>();
            for (int i = 0; i < k; i++)
            {
                int index = distances.IndexOf(distances.Min());
                indexes.Add(index);
                distances[index] = 99999;
            }

            return indexes;
        }

        // определение к каким классам относятся соседи
        List<int> GetClassesOfNeighbors(List<int> indexesOfNearestNeighbors)
        {
            List<int> classificators = new List<int>();
            foreach (var ind in indexesOfNearestNeighbors)
            {
                classificators.Add(listOfFruits[ind].Item2);
            }

            return classificators;
        }

        // классификация
        int Classify(List<int> classificators)
        {
            int health = 0;
            int notHealth = 0;
            int neutral = 0;

            foreach (var c in classificators)
            {
                if (c == 1)
                {
                    notHealth++;
                }
                else if (c == 2)
                {
                    health++;
                }
                else
                {
                    neutral++;
                }

            }
            if (neutral == Math.Max(Math.Max(health, notHealth), neutral))
                return 0;
            if (notHealth == Math.Max(Math.Max(health, notHealth), neutral))
                return 1;
            if (health == Math.Max(Math.Max(health, notHealth), neutral))
                return 2;
            return 0;
        }

        string Test(double protein, double fats, double carbohydrates)
        {
            Fruit newFruit = new Fruit(protein, fats, carbohydrates); // создаем экземпляр нового фрукта, который нужно классифицировать 
            double[] newFruitParams = newFruit.Method(); // переводим его БЖУ в массив

            List<double> distances = GetListOfDistances(newFruitParams);

            int k = 3; // количество соседей

            List<int> indexesOfNearestNeighbors = GetIndexesOfNearestNeighbors(distances, k); // индексы ближайших соседей

            List<int> classificators = GetClassesOfNeighbors(indexesOfNearestNeighbors);

            int isHealthy = Classify(classificators);

            if (isHealthy == 0)
                return "Нейтральный";
            if (isHealthy == 1)
                return "Вредный";
            if (isHealthy == 2)
                return "Полезный";
            return "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                TextBox pr = (TextBox)proteinTB;
                TextBox fats = (TextBox)fatsTB;
                TextBox carb = (TextBox)carbohydratesTB;
                string res = Test(Convert.ToDouble(pr.Text), Convert.ToDouble(fats.Text), Convert.ToDouble(carb.Text));
                label4.Text = res;
            }
            catch { }
        }

        private void Draw(Bitmap bmp, Graphics g, float x, float y, string color)
        {
            SolidBrush brush;
            if (color == "red")
                brush = new SolidBrush(Color.Red);
            else if (color == "blue")
                brush = new SolidBrush(Color.Blue);
            else if (color == "green")
                brush = new SolidBrush(Color.Green);
            else
                brush = new SolidBrush(Color.Black);
            g.FillRectangle(brush, y, x, 4, 4);
        }
    }
}