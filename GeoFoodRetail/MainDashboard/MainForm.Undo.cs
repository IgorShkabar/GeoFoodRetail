/**
 * Copyright (c) 2024-26 Igor Shkabar. All rights reserved.
 * Yes, implement an Undo feature (to reverse a deletion).
*/
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GeoFoodRetail
{
    internal partial class MainForm
    {
        // Add a history stack to your form class
        // Stack for storing previous states of the list (to implement Undo)
        private Stack<List<string>> _undoStack = new Stack<List<string>>();

        /** Creating an Undo method
         Now, let’s write a method that will revert the list to its previously saved state: */

        private void UndoLastDeletion()
        {
            if (_undoStack.Count > 0)
            {
                // Retrieve the last saved state from the stack
                var previousState = _undoStack.Pop();

                listView1.BeginUpdate();
                listView1.Items.Clear();

                // Return the items back to the ListView
                foreach (var text in previousState)
                {
                    listView1.Items.Add(new ListViewItem(text));
                }

                listView1.EndUpdate();

                // Overwrite the foods.txt file with the current, restored list
                SaveFoodsToFile();
                MessageBox.Show("Deletion cancelled!", "Undo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Nothing to cancel.", "Undo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // find an meal item from the ListView component
        private void FindMeal(string searchTerm)
        {
            /* Since your search uses .Equals(), you should ensure searchTerm 
             * is not null to prevent potential issues, 
             * or handle empty strings like this     */
            searchTerm = textBox5.Text?.Trim() ?? string.Empty;

            ListViewItem foundItem = listView1.Items.Cast<ListViewItem>()
                .FirstOrDefault(item =>
                item.Text.Equals(searchTerm, System.StringComparison.OrdinalIgnoreCase) ||
                (item.SubItems.Count > 1 && item.SubItems[1].Text.Equals(searchTerm, System.StringComparison.OrdinalIgnoreCase))
                );

            if (foundItem != null)
            {
                foundItem.Selected = true;
                listView1.TopItem = foundItem; // Scrolls to and highlights the item
            }           
        }

        private void SelectAllItems()
        {
            listView1.BeginUpdate();
            try
            {                
                foreach (ListViewItem lv in listView1.Items)
                {
                    lv.Selected = true;                                        
                }                
                textBox5.SelectAll();

                // Set focus if FullRowSelect is enabled
                if (listView1.FullRowSelect == true)
                {
                    listView1.Select();
                }
            }
            finally
            {
                listView1.EndUpdate();
            }
        }

        private void CopyFoodItem()
        {
            // Check whether there are any selected elements at all
            if (listView1.SelectedItems.Count == 0) return;

            var sb = new System.Text.StringBuilder();

            foreach (ListViewItem item in listView1.SelectedItems)
            {
                // Assemble text from all columns of this row, separated by tabs
                var subItems = new List<string>();
                foreach (ListViewItem.ListViewSubItem subItem in item.SubItems)
                {
                    subItems.Add(subItem.Text);
                }
                sb.AppendLine(string.Join("\t", subItems));
            }
            // Place the finished text into the clipboard
            Clipboard.SetText(sb.ToString());
        }

        private void PasteContent()
        {            
            if (!Clipboard.ContainsText()) return;

            string clipboardText = Clipboard.GetText();

            // Split the text into individual lines
            string[] lines = clipboardText.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                // Split the string into columns (using the tab character as a delimiter)
                string[] columns = line.Split('\t');

                if (columns.Length > 0)
                {
                    // Create a new ListView item. The first element is the main column, and the rest are subitems (SubItems).
                    ListViewItem newItem = new ListViewItem(columns[0]);

                    for (int i = 1; i < columns.Length; i++)
                    {
                        newItem.SubItems.Add(columns[i]);
                    }                    
                    listView1.Items.Add(newItem);
                }
            }
        }
        
        private void DeselectListViewItems()
        {
            listView1.BeginUpdate();
            try
            {
                listView1.SelectedItems.Clear();
                if (listView1.FocusedItem != null)
                {
                    listView1.FocusedItem.Focused = false;
                }                
            }
            finally
            {
                listView1.EndUpdate();
            }
        }

        private void findFoodItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (textBox5.Text != string.Empty) FindMeal(textBox5.Text);
            else
                toolTip1.SetToolTip(textBox5, "Type a food name in the `Food Name` text field.");
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UndoLastDeletion();
        }

        private void selectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectAllItems();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopyFoodItem();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PasteContent();
        }

        private void deselectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeselectListViewItems();
        }
    }
}
