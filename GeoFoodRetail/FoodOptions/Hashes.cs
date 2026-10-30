/** 
 * Copyright (c) 2024 Igor Shkabar. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

/// <summary>
/// To preserve all synonyms while eliminating a massive, unreadable `switch` statement, 
/// the best approach is to move the name variations into lookup collections (`HashSet<string>`).
/// Searching within a `HashSet` can be done in a case-insensitive manner (`StringComparison.OrdinalIgnoreCase`), 
/// which removes the need to account for different capitalizations (e.g., "BBQ", "bbq", and "Bbq" are no longer 
/// all required—keeping just one is sufficient). Here is how to implement this using clean C# code without losing 
/// any of your synonyms: Creating synonym lookup collections:
/// </summary>
/// 
namespace GeoFoodRetail.FoodOptions
{
    internal static class Hashes
    {
        // create lists of acceptable options (letter case no longer matters)
        internal static readonly HashSet<string> MeatKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Antelope Steaks", "Antelope", "Bacon", "Bacon Sliced", "BBQ meat", "BBQ", "B-B-Q meat", "B-B-Q",
            "Beef", "Beef - Organic", "Beef Stroganoff", "Bison Patties", "Bison", "Chicken - Free Range",
            "Chicken", "Duck Meat", "Duck", "Ham", "Kangaroo", "Kangaroo meat", "Lamb", "Liver",
            "Meat", "Pork", "Sausages", "Steaks", "Steak", "Turkey", "Veal"
        };

        internal static readonly HashSet<string> VeganKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Achacha Fruit", "Achacha", "Acorn squash", "Acorn", "Agave", "Ajowan Seed", "Allspice", "Almonds",
            "Annatto seed", "Aniseed", "Apple Cider Vinegar", "Apple Cider", "Cider", "Applesauce", "Alfalfa",
            "Adzuki Beans", "Apple", "Apples", "Apricot", "Arborio rice", "Avocado", "Artichoke", "Arugula",
            "Asian Greens", "Asian noodles", "Asian", "Baba ghanoush", "Baba", "Ghanoush", "Bamboo Shoots",
            "Bamboo", "Basil", "Basil - dried", "Basil - fresh", "Bean", "Butter Beans", "Beetroot", "Beet root", "Beet",
            "Borlotti Beans", "Borlotti", "Broccoli", "Broccolini", "Brussels Sprouts", "Brussels", "Sprouts",
            "Bush Tomato", "Cabbage", "Cabbages", "Camomile", "Camomiles", "Caper", "Capsicum", "Carrot",
            "Cauliflower", "Cavalo Nero", "Cavalo", "Celery", "Chilli Pepper", "Chilli", "Chinese Broccoli",
            "Chinese Cabbage", "Zucchini", "Cucumbers", "Cucumber", "Daikon", "Green Pepper", "Pepper",
            "Yellow pepper", "Red pepper", "Orange pepper", "Kale", "Kalettes", "Kohlrabi", "Lettuce",
            "Olives", "Olive", "Pea", "Peas", "Potatoes", "Potato", "Radish", "Salad", "Snowpeas",
            "Sweetcorn", "Tomato", "Tomatos", "Water spinach", "Spinach", "Greens", "Noodles",
            "Vegetables", "Vegetable", "Watercress"
        };

        internal static readonly HashSet<string> SeaKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Agar", "Agar Agar", "Ahi Tuna", "Ahi", "Albacore Tuna", "Anchovies", "Arame", "Barramundi",
            "Blue Eye Trevalla", "Blue Swimmer Crab", "Calamari", "Clams", "Crabs", "Farmed Prawns",
            "Hijiki", "Hiramasa Kingfish", "Hot smoked salmon", "Jewfish", "Ling", "Lobster", "Mackerel",
            "Mackerel Tinned", "Mahi mahi", "Mirror Dory", "Monkfish", "Morwong", "Mullet", "Mussels",
            "Nori", "Octopus", "Parrotfish", "Prawns", "Redfish", "Salmon", "Salmon - canned", "Sardines",
            "Scallops", "Silver Trevally", "Smoked Trout", "Spanner Crab", "Swordfish", "Threadfin Bream",
            "Trout", "Tuna", "Tuna - tinned", "Warehou", "Wild Alaska Salmon"
        };

        internal static readonly HashSet<string> LightKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Apple juice", "Butter", "Buttermilk", "Cacao", "Cofee", "Coconut milk", "Cottage cheese", "Cheesecake",
            "Curds", "Cream", "Fruit juices", "Fruit yoghurts", "Goat's Milk", "Green Tea", "Green tee",
            "Milk", "Milk Cow's - Reduced and Low Fat", "Low - fat sour cream", "Low-fat sour cream",
            "Muesli", "Oat milk", "Oatmeal", "Soy Milk", "Yoghurt - Sheeps Milk", "Tea", "Vegetable juices"
        };

        // Processing method
        internal static bool ProcessInput(string input, Action<string, string, string, string, string> updateUI)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                MessageBox.Show("The 'Food name' text box is empty!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            string category = DefineCategoryViaHashSet(input);

            // If it wasn't found in the grid, we attempt to identify it using the direct name from the class.
            if (category == null)
            {
                category = CompatibleFood.SetIndexByFoodName(input);
            }
            else
            {
                // Synchronizing the list index
                CompatibleFood.SetIndexByFoodName(input);
            }
            if (category == null)
            {
                MessageBox.Show("There may not be a set with the entered food name.\n\nPlease check your spelling or try singular form.", "Your choice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            // Compatibility calculation
            var fc = CompatibleFood.FoodCompatibility(category);
            int idx = CompatibleFood.Index;

            ShowStringsDependCategory(updateUI, category, fc, idx);
            return true;
        }

        private static void ShowStringsDependCategory(Action<string, string, string, string, string> updateUI, 
                                                    string category, 
                                                    (string Food1, string Food2, string Food3, string Food4, decimal ratio1, decimal ratio2, decimal ratio3, decimal ratio4) fc, 
                                                    int idx)
        {
            // Constructing the strings for display based on the category
            switch (category)
            {
                case "Meat":
                    string mainMeat = $"{fc.Food1} is compatible with {fc.Food2}, {fc.Food3} and {fc.Food4}. Ratio: {fc.Food1} - {fc.ratio1} gr., vegetables {fc.Food2} - {fc.ratio2} gr., side dish {fc.Food3} - {fc.ratio3} gr., spices {fc.Food4} - {fc.ratio4} gr.";
                    string sub1Meat = $"We can also combine {fc.Food1} with {fc.Food2}.";
                    string sub2Meat = $"... and with {fc.Food3} as a garnish.";
                    updateUI("Meat", fc.Food1, mainMeat, sub1Meat, sub2Meat);
                    break;

                case "Vegan":
                    string mainVegan = $"{fc.Food1} is compatible with {fc.Food2}, {fc.Food3} and {fc.Food4}. Ratio: {fc.Food1} {fc.ratio1} gr., {fc.Food2} {fc.ratio2} gr., {fc.Food3} {fc.ratio3} gr. and {fc.Food4} {fc.ratio4} gr.";
                    updateUI("Vegan", fc.Food1, mainVegan, null, null);
                    break;

                case "Sea":
                    string mainSea = $"{fc.Food1} is compatible with {fc.Food2} and {fc.Food3}. Ratio: {fc.Food1} {fc.ratio1} gr., vegetables {fc.Food2} {fc.ratio2} gr. and {fc.Food3} {fc.ratio3} gr.";
                    string nutSea = $"{fc.Food1} is very tasty to eat with {GetSafely(CompatibleFood.Nuts, idx)}.";
                    updateUI("Sea", fc.Food1, mainSea, nutSea, null);
                    break;

                case "Light":
                    string mainLight = $"{fc.Food1} may be bought in addition to {fc.Food2}, {fc.Food3} and {fc.Food4}. Ratio: {fc.Food1} {fc.ratio1} gr., {fc.Food2} {fc.ratio2} gr., {fc.Food3} {fc.ratio3} gr., {fc.Food4} {fc.ratio4} gr.";
                    string fruitLight = $"It's not bad to eat a serving of {GetSafely(CompatibleFood.Fruit, idx)}. If you don't drink raw milk.";
                    updateUI("Light", fc.Food1, mainLight, fruitLight, null);
                    break;
            }
        }

        private static string DefineCategoryViaHashSet(string input)
        {
            // Determining the category using a HashSet
            string category = null;
            if (MeatKeys.Contains(input))
                category = "Meat";
            else if (VeganKeys.Contains(input))
                category = "Vegan";
            else if (SeaKeys.Contains(input))
                category = "Sea";
            else if (LightKeys.Contains(input))
                category = "Light";
            return category;
        }

        private static string GetSafely(List<string> list, int index)
        {
            return (index >= 0 && index < list.Count) ? list[index] : "N/A";
        }
    }
}
