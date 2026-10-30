/** 
 * Copyright (c) 2024 Igor Shkabar. All rights reserved.
 * The main Form of the Solution form for working with meal data.
 * To ensure that changes to `listView1` are saved permanently 
 * (so that the data does not disappear when the program is restarted), 
 * you need to write the current list back to the text file (`foods.txt`) 
 * every time an item is added or removed.
 */
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
using GeoFoodRetail.Utils; // FoodParserUtility namespace
using System.Drawing;

namespace GeoFoodRetail
{
    internal partial class MainForm
    {
        //  The quick list in memory to store lines from the foods.txt file
        private List<string> _foodCache = new List<string>();
        

        // Heart VirtualMode: вызывается автоматически, когда элемент становится виден на экране
        private void listView1_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            /// The method is called ONLY for rows currently visible on the screen
            if (e.ItemIndex >= 0 && e.ItemIndex < _foodCache.Count)
            {
                // Create an element on the fly only for the visible row
                e.Item = new ListViewItem(_foodCache[e.ItemIndex]);
            }
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {            
            if (string.IsNullOrWhiteSpace(textBox5.Text))
            {
                MessageBox.Show("Please enter a food item name in the Food text box first.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var dialogResult2 = MessageBox.Show("Are you sure you want to add this item?","Approving",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (dialogResult2 == DialogResult.Yes)
            {
                listView1.BeginUpdate();
                try
                {                    
                    AddUniqueItem(textBox5.Text);
                    PacketingProcessing();
                }
                finally
                {                    
                    listView1.EndUpdate();
                }
            }
        }

        private void listView1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            
        }

        private void PacketingProcessing()
        {            
            if (listView1.CheckedItems.Count > 0)
            {                
                listView1.BeginUpdate();

                foreach(ListViewItem li in listView1.CheckedItems)
                {
                    li.SubItems[listView1.CheckedItems.Count].Text = "Is processed";                    
                    li.BackColor = Color.LightGreen;
                }                
                listView1.EndUpdate();
            }
        }

        // Method for safely adding an element
        private void AddUniqueItem(string text)
        {
            // Check whether an element with this text already exists
            bool isDuplicate = listView1.Items.Cast<ListViewItem>()
                .Any(item => item.Text.Equals(text, StringComparison.OrdinalIgnoreCase));

            if (!isDuplicate)
            {
                listView1.Items.Add(new ListViewItem(text));
                
                textBox5.Clear();                
                SaveFoodsToFile();
            }
            else
            {
                MessageBox.Show("This item already exists!", "Dupplicate", MessageBoxButtons.OK, MessageBoxIcon.Warning);                
            }
        }

        private void RemoveMealItem()
        {
            if (listView1.CheckedItems.Count > 0)
            {
                // SAVE THE CURRENT STATE TO THE STACK BEFORE DELETION
                var currentState = listView1.Items.Cast<ListViewItem>()
                                           .Select(item => item.Text)
                                           .ToList();
                _undoStack.Push(currentState);

                // Freezing the interface update for maximum speed
                listView1.BeginUpdate();

                // Remove items from the end of the CheckedItems collection 
                // so that the indices do not shift during removal
                for (int i = listView1.CheckedItems.Count - 1; i >= 0; i--)
                {
                    ListViewItem itemToRemove = listView1.CheckedItems[i];

                    // Safely remove the element object itself from the general list
                    listView1.Items.Remove(itemToRemove);
                }
                // Unfreeze the interface
                listView1.EndUpdate();                
                SaveFoodsToFile();
            }
            else { MessageBox.Show("Please select a row to remove."); }
        }

        
        private void SaveFoodsToFile()
        {
            try
            {
                string filePath = 
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "updated_meal.txt");

                // Let's ensure that the Resources folder exists
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                // Collect the text from all elements in listView1 into a list
                var lines = listView1.Items.Cast<ListViewItem>()
                                           .Select(item => item.Text)
                                           .ToList();
                File.WriteAllLines(filePath, lines, System.Text.Encoding.UTF8);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void LoadFoodsFromFile()
        {
            string filePath = 
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Meal.txt");

            if (File.Exists(filePath))
            {
                listView1.Items.Clear();

                var lines = File.ReadAllLines(filePath, System.Text.Encoding.UTF8);
                

                listView1.BeginUpdate();
                foreach (var line in lines)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        listView1.Items.Add(new ListViewItem(line.Trim()));
                    }
                }
                listView1.EndUpdate();
            }
        }
    }
}
