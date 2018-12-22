using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k_nearest_neighbors_algorithm
{
    class Fruit
    {
        public Fruit(double protein, double fats, double carbohydrates, int? isHealthy = null)
        {
            this.Protein = protein;
            this.Fats = fats;
            this.Carbohydrates = carbohydrates;
            if (isHealthy.HasValue)
            {
                this.isHealthy = isHealthy.Value;
            }
        }

        public double Protein { get; private set; }
        public double Fats { get; private set; }
        public double Carbohydrates { get; private set; }
        public int isHealthy { get; private set; } // 1 - вредный, 2 - полезный, 0 - нейтральный

        public double[] Method()
        {
            return new double[] { Protein, Fats, Carbohydrates };
        }

        public int SetListHealthy()
        {
            return isHealthy;
        }
    }
}
