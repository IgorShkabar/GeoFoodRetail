/**
 * Copyright (c) 2024-26 Igor Shkabar. All rights reserved.
 * Menu items: New, Open
*/

using GeoFoodRetail.MainDashboard;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GeoFoodRetail
{
    internal class NewOpenSafeCloseExit
    {
        static string fn;
        static bool textChanged;

        public NewOpenSafeCloseExit()
        {
            fn = string.Empty;
            textChanged = false;
        }

        static string Text { get; set; }

        internal static void NewDoc()
        {
            var dr = DialogResult.Yes;

            switch (dr)
            {
                case DialogResult.Yes:
                    try
                    {
                        using (var sf = new SaveFileDialog())
                        {
                            sf.Filter = "Text files (*.txt)|Plain text (*.txt)|Text in RTF format (*.rtf)|Portable Document File (*.pdf)|Word document 1997-2003 (*.doc)|Word document (*.docx)|Word document with macros support (*.docm)|Word template (*.dot)|Word template with macros support (*.dotm)|XPS document (*.xps)|XML document (*.xml)|Web page (*.htm;*.html)|All files (*.*)|*.*";
                            sf.Title = "New File";                             

                            if (sf.ShowDialog() == DialogResult.OK)
                            {
                                try
                                {
                                    File.Create(sf.FileName).Close(); // create the new file                                    
                                    string currentfilePath = sf.FileName; // store the path                                    
                                }
                                catch (System.Exception ex)
                                {
                                    MessageBox.Show($"Error creating file: {ex.Message}", "Error creating the file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                    }
                    catch (InvalidOperationException)
                    {
                        MessageBox.Show($"There is no WINWORD.EXE process, so Notepad.exe will execute.");
                    }
                    catch (System.IndexOutOfRangeException)
                    {
                        MessageBox.Show($"Probably, the necessary Office Packet does not installed in your Operation System.");
                    }
                    catch (System.ArgumentNullException)
                    {
                        MessageBox.Show($"The notepad file will be created, instead the Microsoft Office Word.");
                    }
                    catch (System.IO.FileNotFoundException)
                    {
                        dr = DialogResult.OK;
                    }
                    break;
                case DialogResult.Cancel:
                    break;
            }
        }

        internal static string OpenFileAndGetContent()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                openFileDialog.Filter = "Text files(*.txt)|Plain text (*.txt)|Text in RTF format (*.rtf)|Portable Document File (*.pdf)|Word document 1997-2003 (*.doc)|Word document (*.docx)|Word document with macros support (*.docm)|Word template (*.dot)|Word template with macros support (*.dotm)|XPS document (*.xps)|XML document (*.xml)|Web page (*.htm;*.html)|All files (*.*)|*.*";
                openFileDialog.Title = "Open File";
                openFileDialog.Multiselect = true;
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {                      
                    string filePath = openFileDialog.FileName;                    
                    return File.ReadAllText(filePath); // Returns the text of the opened file
                }
            }
            return string.Empty;
        }

        // The method accepts a list of forms or controls that need to be saved
        internal static void SaveAll(IEnumerable<Form> openDocuments)
        {
            foreach (Form doc in openDocuments)
            {                
                SaveDocument(doc);
            }
            MessageBox.Show("All changes have been successfully saved!", "Save All", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private static void SaveDocument(Form doc)
        {
            // This is where the basic logic for saving a specific document resides.
            // For example, locating a text field on a form and writing its content to a file:
            Control richBox = doc.Controls.Find("richTextBox1", true).FirstOrDefault();
            if (richBox is RichTextBox rtb)
            {
                File.WriteAllText(doc.Text + ".txt", rtb.Text);
            }
            MessageBox.Show($"Document '{doc.Text}' has been saved.");
        }

        internal static void CloseDoc(Form currentDoc)
        {
            Form openDocuments = currentDoc;

            // A check could be added here: has the file been saved?
            bool isDocUnsaved = true;
            if(isDocUnsaved)
            {
                DialogResult result = MessageBox.Show("You have unsaved changes. Save before closing?",
                    "Closing the document", MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Warning);

                switch(result)
                {
                    case DialogResult.Yes:
                        SaveDocument(openDocuments);
                        openDocuments.Close();
                        break;
                    case DialogResult.No:
                        openDocuments.Close();
                        break;
                    default:
                        openDocuments.Close();
                        break;
                }
            }
        }

        internal static void ExitApplication()
        {            
            Application.Exit();
        }
    }
}
