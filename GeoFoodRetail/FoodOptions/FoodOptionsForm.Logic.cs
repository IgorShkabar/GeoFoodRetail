/** 
 * Copyright (c) 2024-26 Igor Shkabar. All rights reserved.
 * Selecting various menu options, as well as the quantity and proportional balance of food.
 * Using the CheckIndexes tuple.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GeoFoodRetail.FoodOptions;
using static GeoFoodRetail.CompatibleFood;

namespace GeoFoodRetail
{
    internal partial class FoodPurchasingOptionsForm
    {
        public void SaveIndexesToCheck()
        {
            var (Food1, Food2, Food3, Food4, ratio1, ratio2, ratio3, ratio4) = FoodCompatibility(FoodNameTextBox.Text);
            Food1 = FoodNameTextBox.Text;

            CheckIndexes(Food1, Food2, Food3, Food4, ratio1, ratio2, ratio3, ratio4);
        }

        private void CheckIndexes(string Food1, string Food2, string Food3, string Food4, decimal ratio1, decimal ratio2, decimal ratio3, decimal ratio4)
        {
            // Safe indices for verification
            int idx = Index;

            if ((FoodNameTextBox.Text != SeaFood[idx % SeaFood.Count]) || (FoodNameTextBox.Text != Vegetables[idx % Vegetables.Count]) ||
                    (FoodNameTextBox.Text != LightFood[idx % LightFood.Count]) || (FoodNameTextBox.Text != Meal[idx % Meal.Count]) || (FoodNameTextBox.Text != Cheeses[idx % Cheeses.Count]) ||
                    (FoodNameTextBox.Text != Oils[idx % Oils.Count]) || (FoodNameTextBox.Text != Spices[idx % Spices.Count]) || (FoodNameTextBox.Text != Nuts[idx % Nuts.Count]) ||
                    (FoodNameTextBox.Text != Fruit[idx % Fruit.Count]) && (FoodNameTextBox.Text != null))
            {
                FoodNameTextBox.Text = Meat[idx % Meat.Count];

                label11.Text = $"{Food1} is compatible with {Food2}, {Food3} and {Food4}. In the ratio: {Food1} - {ratio1} gr., vegetables {Food2} - {ratio2} gr., side dish {Food3} - {ratio3} gr., and spices {Food4} {ratio4}.";
                Combine(Meal[idx % Meal.Count]);
                label12.Visible = true;
                label13.Visible = true;
                label12.Text = $"We can also combine {Food1} with {Vegetables[idx % Vegetables.Count]}.";
                label13.Text = $"... and with {Meal[idx % Meal.Count]} as a garnish.";
                return;
            }
            else if (FoodNameTextBox.Text != Meat[idx % Meat.Count] || (FoodNameTextBox.Text != SeaFood[idx % SeaFood.Count]) ||
                    (FoodNameTextBox.Text != LightFood[idx % LightFood.Count]) || (FoodNameTextBox.Text != Meal[idx % Meal.Count]) || (FoodNameTextBox.Text != Cheeses[idx % Cheeses.Count]) ||
                    (FoodNameTextBox.Text != Oils[idx % Oils.Count]) || (FoodNameTextBox.Text != Spices[idx % Spices.Count]) || (FoodNameTextBox.Text != Nuts[idx % Nuts.Count]) ||
                    (FoodNameTextBox.Text != Fruit[idx % Fruit.Count]) && (FoodNameTextBox.Text != null))
            {
                FoodNameTextBox.Text = Vegetables[idx % Vegetables.Count];

                label11.Enabled = false;

                label12.Text = $"{Food1} is compatible with {Food2}, {Food3} and {Food4}. In the ratio: {Food1} {ratio1} gr., vegetables {Food2} {ratio2} gr., {Food3} {ratio3} gr. and {Food4} {ratio4}.";
                Combine(Vegetables[idx % Vegetables.Count]);
                return;
            }
            else if (FoodNameTextBox.Text == SeaFood[idx % SeaFood.Count])
            {
                FoodNameTextBox.Text = SeaFood[idx % SeaFood.Count];

                label11.Text = $"{Food1} is compatible with {Food2} and {Food3}. In the ratio: {Food1} {ratio1} gr., vegetables {Food2} {ratio2} and {Food3} {ratio3}.";
                Combine(SeaFood[idx % SeaFood.Count]);
                label12.Visible = true;
                label12.Text = $"{Food1} is very tasty to eat with {Nuts[idx % Nuts.Count]}.";
                return;
            }
            else if (FoodNameTextBox.Text == LightFood[idx % LightFood.Count])
            {
                FoodNameTextBox.Text = LightFood[idx % LightFood.Count];

                label11.Text = $"{Food1} may be bought to addition to {Food2}, {Food3} and {Food4}. In the ratio: {Food1} {ratio1} gr., {Food2} {ratio2} gr., {Food3} {ratio3}gr., {Food4} {ratio4}.";
                Combine(LightFood[idx % LightFood.Count]);
                label12.Visible = true;
                label12.Text = $"It's not bad to eat a serving of {Fruit[idx % Fruit.Count]}.";
                return;
            }
            else
            {
                if (FoodNameTextBox.Text == "")
                {
                    MessageBox.Show("The 'Food name' text box is empty!");
                    FoodNameTextBox.Focus();
                }
                else if (FoodNameTextBox.Text != Meat[idx % Meat.Count] || (FoodNameTextBox.Text != SeaFood[idx % SeaFood.Count]) || (FoodNameTextBox.Text != Vegetables[idx % Vegetables.Count]) ||
                    (FoodNameTextBox.Text != LightFood[idx % LightFood.Count]) || (FoodNameTextBox.Text != Meal[idx % Meal.Count]) || (FoodNameTextBox.Text != Cheeses[idx % Cheeses.Count]) ||
                    (FoodNameTextBox.Text != Oils[idx % Oils.Count]) || (FoodNameTextBox.Text != Spices[idx % Spices.Count]) || (FoodNameTextBox.Text != Nuts[idx % Nuts.Count]) ||
                    (FoodNameTextBox.Text != Fruit[idx % Fruit.Count]) && (FoodNameTextBox.Text != null))
                {
                    MessageBox.Show("There may not be a set with the entered food name.\n\nPlease check your spelling or enter something like the name of the food items in the main form.\nTry entering the name of the food in the singular.\n\n*There can be an infinite number of food combinations, so the program cannot reflect absolutely all of your entered food names in accordance with your choice.",
                    "Your choice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        // obtain a combined food product
        private static void GetCombineFood()
        {
            string m = Vegetables[Index];
            Combine(m);
        }

        private void ProcessLabelsUpdate()
        {
            // Starting processing with a GUI update delegate
            bool success = Hashes.ProcessInput(FoodNameTextBox.Text.Trim(), (category, correctName, mainText, extra1, extra2) =>
            {
                // Correcting the name in textBox1 to the canonical version from the list
                FoodNameTextBox.Text = correctName;

                // Update the necessary Labels based on the category, WITHOUT clearing the others
                switch (category)
                {
                    case "Meat":
                        label5.Text = mainText;
                        label3.Text = extra1;
                        label4.Text = extra2;
                        label3.Visible = label4.Visible = true;
                        break;

                    case "Vegan":
                        label7.Text = mainText;
                        break;

                    case "Sea":
                        label6.Text = mainText;
                        label9.Text = extra1;
                        label9.Visible = true;
                        break;

                    case "Light":
                        label8.Text = mainLightText(mainText);
                        label10.Text = extra1;
                        label10.Visible = true;
                        break;
                }
                SetBordersForActiveLabels();
            });
        }
    }
}
