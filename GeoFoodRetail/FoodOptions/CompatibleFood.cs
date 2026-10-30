/** 
 * Copyright (c) 2024-26 Igor Shkabar. All rights reserved.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GeoFoodRetail
{
    internal class CompatibleFood 
    {
        // constants & fields:
        private const decimal Veg = 0.5m;
        private const decimal Spaice = 0.05m;
        private const decimal Grain = 0.25m;
        private const decimal Oil = 0.08m;
        private const decimal Light = 0.65m;
        private const decimal Delta = 0.05m;
                
        private decimal _massVegetables;
        private decimal _massLightFood;
        private decimal _massSeafood;
        private decimal _massSpices;
        private decimal _massGrains;
        private decimal _massMeat;
        private decimal _massOil;     
        private string _vegetable;
        private string _meatDiffer;
        private string _selectedFood = "Water"; // Set the default base value to 100.0m

        private static readonly Random _random = new Random();
        
        // Property controls access & data validness
        internal decimal MassFood { get; set; }
        internal static int Index { get; private set; } // Food index

        [System.ComponentModel.Browsable(false)]
        internal static int Count { get; }

        internal static string Food1Name { get; set; }
        internal static string Food2Name { get; set; }
        internal static string Food3Name { get; set; }
        internal static string Food4Name { get; set; }
        internal static decimal Ratio1Volume { get; set; }
        internal static decimal Ratio2Volume { get; set; }
        internal static decimal Ratio3Volume { get; set; }
        internal static decimal Ratio4Volume { get; set; }

        internal static List<string> Meat { get; } = new List<string>() { "Antelope Steaks", "Bacon", "Bacon Sliced", "BBQ meat", "Beef", "Beef - Organic", "Bison Patties", "Chicken - Free Range", "Duck Meat", "Ham", "Kangaroo meat", "Lamb", "Liver", "Pork", "Sausages", "Turkey", "Veal" };

        internal static List<string> Meal { get; } = new List<string>() { "Agave Syrup","Amaranth","Arborio rice","Barley","Barley Pearl","Basmati rice","Bay Leaves","Beer Miller Lite","Black Rice","Black tahini","Blue Cheese","Bocconcini","Bok Choy","Bran","Bread","Bread - Rye","Bread - Sour Dough",
            "Bread - Spelt","Bread - Wholemeal","Brie Cheese","Brown Rice","Brown rice vinegar","Buckwheat","Buckwheat Noodles (Soba)","Bulghur","Chocolate - Dark","Cous Cous","Dashi","Eggs","Green Chicken Curry","Honey","Mountain Bread","Mushrooms - Brown","Mushroom - Flat",
            "Panetonne","Pasta","Rye","Sake","Shoyu","Soy Beans","Spelt","Sunflower Seeds","Tofu","Water","Wheat","White rice","Wild Rice","Wine","Zagnut" };

        internal static List<string> Vegetables { get; } = new List<string>() { "Alfalfa","Adzuki Beans","Artichoke","Arugula","Asian Greens","Asian noodles","Asparagus","Aubergine","Avocado","Baba ghanoush", "Bamboo Shoots", "Apple Cider Vinegar", "Basil - dried", "Basil - fresh","Bean", "Butter Beans",
            "Beans","Beans Snake","Bean Sprouts","Beans Green","Beans","Bean Sprouts","Beetroot","Borlotti Beans","Broccoli","Broccolini","Brussels Sprouts","Bush Tomato","Cabbage","Camomile","Caper","Capsicum","Carrot","Cauliflower","Cavalo Nero",
            "Celery","Chilli Pepper","Chinese Broccoli","Chinese Cabbage","Zucchini","Cucumbers","Daikon","Green Pepper","Kale","Kalettes","Kohlrabi","Lettuce","Olives","Pea","Peas","Potatoes","Radish","Salad","Snowpeas","Sweetcorn","Tomato","Water spinach","Watercress"    };

        internal static List<string> SeaFood { get; } = new List<string>() { "Agar Agar","Anchovies","Albacore Tuna","Arame","Barramundi","Blue Eye Trevalla", "Blue Swimmer Crab","Calamari","Clams","Crabs","Farmed Prawns","Hijiki","Hiramasa Kingfish","Hot smoked salmon","Jewfish","Ling","Lobster",
            "Mackerel","Mackerel - Tinned","Mahi mahi","Mirror Dory","Monkfish","Morwong","Mullet","Mussels","Nori","Octopus","Parrotfish","Prawns","Redfish","Salmon","Salmon - canned","Sardines","Scallops","Silver Trevally","Smoked Trout","Spanner Crab","Swordfish",
            "Threadfin Bream","Trout","Tuna","Tuna - tinned","Warehou","Wild Alaska Salmon"   };

        internal static List<string> Nuts { get; } = new List<string>() { "Almonds", "Candle Nut", "Cashews", "Chestnut", "Coconut", "Hazelnut", "LSA", "Macadamia Nut", "Nut", "Peanuts", "Pecan Nut", "Pine Nut", "Pistachio Nut", "Slivered Almonds", "Walnut" };
               
        internal static List<string> LightFood { get; } = new List<string>() { "Apple juice","Butter","Buttermilk","Cacao","Coffee","Coconut milk","Cottage cheese","Cheesecake","Curds","Cream","Fruit juices","Fruit yoghurts","Goat's Milk","Green Tea","Milk","Milk Cow's - Reduced and Low Fat","Low-fat sour cream","Muesli","Oat milk","Oatmeal","Soy Milk","Yoghurt - Sheeps Milk",
            "Tea","Vegetable juices" };

        internal static List<string> Fruit { get; } = new List<string>() { "Achacha","Apples","Apples - Cox's Orange Pippin","Apples - Jazz","Apples - Red Fuji","Apples - Bonza","Apples - Golden Delicious","Apples - Jonathan","Apples - Sundowner","Apple Juice Concentrate","Annatto seed","Apples - Braeburn","Apples - Granny Smith","Apples - Red Delicious","Apples Royal Gala",
            "Apricots","Banana","Barberry","Blood oranges","Cantaloupe","Carambola","Cherries","Cherry tomatoes","Cumquat","Currants","Dates","Figs","Fruit salad","Grapefruit","Grapes","Grapes - red seedless","Guava","Honeydew melon","Lemon","Lychees","Mace","Mandarins","Mango","Mangosteens","Melon","Melon - Piel de Sapo",
            "Nectarines","Peaches","Pineapple","Pomegranate","Plump","Strawberries","Tamarillo","Tangelo","Watermelon","Yello"  };

        internal static List<string> Spices { get; } = new List<string>() { "Apple Cider Vinegar","Ajowan Seed","Amaranth", "Asafoetida", "Balsamic Vinegar","Caraway Seed","Cardamom","Cassia bark","Chervil","Chinese Five Spice","Cinnamon","Cloves","Coriander Seed","Cumin","Fish Sauce",
            "Galangal","Garam Masala","Garlic","Ginger","Harissa","Horseradish","Lemon Myrtle","Lemongrass","Malt vinegar","Marjoram","Miso","Mustard","Paprika","Rosemary","Turmeric","Vinegar","Zatar"  };

        internal static List<string> Cheeses { get; } = new List<string>() { "Blue Cheese","Camembert","Cheddar","Corn Tortilla","Cottage Cheese","Cottage Cheese - low fat","Cream Cheese","Edam","Feta","Fromage Blanc","Goat's Cheese","Gruyere","Haloumi","Mascarpone","Merchant stock","MUM",
            "Mozzarella","Nicole","Parmesan cheese","Pecorino","Provolone","Quark","Ricotta","Taleggio cheese","Tom Yum"    };

        internal static List<string> Oils { get; } = new List<string>() { "Almond oil", "Avocado Oil","Camellia Tea Oil","Canola Oil","Corn Oil","Corn Syrup","Extra Virgin Olive Oil","Flaxseed Oil","Grape Seed Oil","Kecap Manis","Macadamia oil","Margarine","Olive Oil","Safflower Oil","Sesame Oil",
            "Sunflower Oil","Tea Oil","Vegetable Oil" };

        /// <summary>
        /// I find the minimum size among all the lists so that the index never goes out of bounds.
        /// </summary>
        /// <param name="ml"></param>

        internal static void Combine(string ml)
        {  
            int minCount = Math.Min(Meat.Count,
                            Math.Min(Vegetables.Count,
                            Math.Min(Meal.Count,
                            Math.Min(SeaFood.Count, LightFood.Count))));

            if (minCount == 0) return;

            // generate an index strictly within the minimum size limit.
            Index = _random.Next(minCount);

            var o = new CompatibleFood();
            o._selectedFood = Meal[Index];
            o._vegetable = Vegetables[Index];
            o._meatDiffer = Meat[Index];

            // A loop using the safe limit minCount instead of Vegetables.Count
            int loopIndex = 0;
            while (loopIndex < minCount)
            {
                for (loopIndex = 0; loopIndex < minCount; loopIndex++)
                {
                    Equals(Meat, Vegetables);
                    Equals(o._selectedFood, Meal);
                    Equals(Cheeses, Oils);
                    Equals(SeaFood, Vegetables);
                    Equals(LightFood, Fruit);
                    Equals(Nuts, Fruit);
                }
            }
        }

        // A helper method for finding an index in a list, ignoring case
        private static int FindIndexIgnoreCase(List<string> list, string foodName)
        {
            return list.FindIndex(x => string.Equals(x, foodName, StringComparison.OrdinalIgnoreCase));
        }

        // Product search and setting the correct index
        internal static string SetIndexByFoodName(string foodName)
        {
            int i = FindIndexIgnoreCase(Meat, foodName);
            if (i != -1) { Index = i; return "Meat"; }

            i = FindIndexIgnoreCase(Vegetables, foodName);
            if (i != -1) { Index = i; return "Vegan"; }

            i = FindIndexIgnoreCase(SeaFood, foodName);
            if (i != -1) { Index = i; return "Sea"; }

            i = FindIndexIgnoreCase(LightFood, foodName);
            if (i != -1) { Index = i; return "Light"; }
            return null; // Not found
        }

        // i is the abbrv. of index
        private static string GetElementSafely(List<string> list, int i)
        {
            if (list == null || list.Count == 0)
                return string.Empty;

            return list[Math.Abs(i) % list.Count];
        }

        /// The Extracted Tuple for the FoodCompatibility(category) Tuple

        private static (string Food1, string Food2, string Food3, string Food4,
            decimal ratio1, decimal ratio2, decimal ratio3, decimal ratio4)
            ForFoodCompatibility(string category, string meat, string vegan, string sea, string light, string meal, string spice, string oil, CompatibleFood o)
        {
            switch (category)
            {
                case "Meat":
                    o._massVegetables = o.MassFood * Veg;
                    o._massSpices = o.MassFood * Spaice;
                    o._massGrains = o.MassFood * Grain;
                    o._massMeat = o.MassFood - (o._massVegetables + o._massGrains + o._massSpices);
                    return (meat, vegan, meal, spice, o._massMeat, o._massVegetables, o._massGrains, o._massSpices);

                case "Vegan":
                    o._massOil = o.MassFood * Oil;
                    o._massSpices = o.MassFood * Spaice;
                    o._massGrains = o.MassFood * Grain;
                    o._massVegetables = o.MassFood - (o._massOil + o._massSpices + o._massGrains);
                    return (vegan, meal, oil, spice, o._massVegetables, o._massGrains, o._massOil, o._massSpices);

                case "Sea":
                    o._massVegetables = o.MassFood * Veg;
                    o._massOil = o.MassFood * Oil;
                    o._massSpices = o.MassFood * Spaice;
                    o._massGrains = o.MassFood * Grain - Delta;
                    o._massSeafood = o.MassFood - (o._massVegetables + o._massSpices + o._massOil);
                    return (sea, vegan, meal, spice, o._massSeafood, o._massVegetables, o._massGrains, o._massSpices);

                default: // Light food
                    o._massLightFood = o.MassFood * Light;
                    o._massVegetables = o.MassFood * Veg;
                    o._massGrains = o.MassFood * Grain;
                    o._massOil = o.MassFood - (o._massLightFood + o._massVegetables + o._massGrains);
                    return (light, vegan, meal, oil, o._massLightFood, o._massVegetables, o._massGrains, o._massOil);
            }
        }
        // Tuple: Compatibility of food

        internal static (string Food1, string Food2, string Food3, string Food4,
            decimal ratio1, decimal ratio2, decimal ratio3, decimal ratio4) FoodCompatibility(string category)
        {
            string meat = GetElementSafely(Meat, Index);
            string vegan = GetElementSafely(Vegetables, Index);
            string sea = GetElementSafely(SeaFood, Index);
            string light = GetElementSafely(LightFood, Index);
            string meal = GetElementSafely(Meal, Index);
            string spice = GetElementSafely(Spices, Index);
            string oil = GetElementSafely(Oils, Index);

            // To get decimal value from textBox2
            var o = new CompatibleFood
            {
                MassFood = FoodPurchasingOptionsForm.MassFoodfrUserInput
            };

            return ForFoodCompatibility(category, meat, vegan, sea, light, meal, spice, oil, o);
        }        
    }
}
