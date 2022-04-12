/*
    Filename: Form1.cs
    Purpose:  Setup of GUI and functions
    Author:   Ivon Harris
    Date:     04/05/2022
    Version:  1.17
    License:  GNU
    Notes:    rescaped from .NET version 5 (Previously VS 2019)
              Functions to complete
              Open      --- display read in class data to dataGridView table        --- ReadClassData()      --- done
                                                                                    --- DisplayClassData()   --- done
Save      --- save class data and overite to currentFilePath                        --- SaveClassData()      --- done
Save as   --- Save as   --- save as class data to a file/location chosen by user    --- SaveClassData()      --- done
Exit      --- Exit      --- Exit application                                        --- Exit()               --- done

              Functions to complete (buttons)
Clear     --- Clear     --- Clears the classroom fields                             --- ClearClassData()     --- done
              Save      --- save changes (same as menu item)                        --- SaveCladdData()      --- done
              Sort      --- Sort the classroms list alphabetically
              Find      --- Enter name into search field to search student
              Save RAF  --- Save files as .dat                                      --- 
              Exit      --- Exit application                                        --- Exit()               --- done

              TESTING   --- Current data not being read by displayData
*/
using System.Text;

namespace ClassroomManager
{
    public partial class Form1 : Form
    {
        // rows and columns for Data Frid View table
        public const int TOTAL_ROWS = 19;
        public const int TOTAL_COLS = 10;
        //list data structure of Cell objects
        private List<Cell> cellList;
        //current path of current file chosen
        private string currentFilePath;
        // list of students 
        private List<Cell> cellNames;
        // string holds student names (ToString)
        string value = "";
        // sting array for studentList[]
        string[] array;
        // currentrecord
        private int currentRecord;

        List<string> studentNames = new List<string>();

        public Form1()
        {

            InitializeComponent();

            //add columns to the Data Grid View
            for (int column = 0; column < TOTAL_COLS; column++)
            {
                cellDataGridView.Columns.Add("Column", string.Format((column).ToString()));
            }
            // add rows to the Data Grid View
            for (int row = 0; row < TOTAL_ROWS - 1; row++)
            {
                cellDataGridView.Rows.Add();
            }
            // set row header width(for row header numbers)
            cellDataGridView.RowHeadersWidth = 60;
            // set row header numbers (start at 0)
            for (int count = 0; count < (cellDataGridView.Rows.Count - 1); count++)
            {
                cellDataGridView.Rows[count].HeaderCell.Value = string.Format((count).ToString(), "0");
            }
            // change font and font size for each cell
            cellDataGridView.DefaultCellStyle.Font = new Font("Arial", 10);

            // initialise cellList
            cellList = new List<Cell>();

            //initialise currentFilePath
            currentFilePath = "";
        }
        private void ReadClassData()
        {

            if (cellList.Count > 0)
            {
                // reset list and remove all items
                cellList.Clear();
                cellList = new List<Cell>();
            }
            // read external file
            try
            {
                if (File.Exists(currentFilePath))
                {
                    // read using StreamReader (read line by line)
                    using (StreamReader streamReader = new StreamReader(currentFilePath))
                    {
                        int counter = 0;
                        string line = "";
                        //var testing = "";
                        // loop line by line
                        while ((line = streamReader.ReadLine()) != null)
                        {
                            //testing += line + "\n";
                            //split line by comma delimiter
                            string[] lineArray = line.Split(',');
                            // check if counter is < 4
                            if (counter < 4)
                            {
                                string title = lineArray[0];
                                string info = lineArray[1];
                                switch (counter)
                                {
                                    case 0: teacherTextBox.Text = info; break;
                                    case 1: classTextBox.Text = info; break;
                                    case 2: roomTextBox.Text = info; break;
                                    case 3: dateTextBox.Text = info; break;
                                }
                            }
                            else
                            {
                                // Grid Data view content
                                // get the 4 values for each cell read in
                                int cols = Int32.Parse(lineArray[0]);
                                int rows = Int32.Parse(lineArray[1]);
                                string cellValue = lineArray[2];
                                string cellColour = "NA";

                                if (cellValue.Equals("BKGRND FILL"))
                                {
                                    cellColour = lineArray[3];
                                }
                                //add new instance to cellList
                                cellList.Add(new Cell(rows, cols, cellValue, cellColour));

                            }
                            counter++;
                        }// end while
                        streamReader.Close();
                        //MessageBox.Show(testing);
                    }// end of streamReader
                }//end if (File.exists)

                string testOutput = "";
                for (int i = 0; i < cellList.Count; i++)
                {
                    testOutput += cellList[i].ToString() + "\n";
                }
                MessageBox.Show(testOutput);
            }// end try block a
            catch (Exception a)
            {
                MessageBox.Show("Error encountered at :  ", "ERROR" + a.Message);

            }

        }


        // event handler for the open menu item

        /*
         * METHOD:      DisplayClassData()
         * PURPOSE:     Display classroom data stored in cellList to dataGridView
         * Inputs:      none
         * Outputs:     void
        */
        private void DisplayClassData()
        {
            if (cellList.Count > 0)
            {
                //display and setup data in the dataGridView
                DataGridViewCellStyle style = new DataGridViewCellStyle();
                // back colour light blue (fill)
                style.BackColor = Color.LightBlue;
                // for colour black (text)
                style.ForeColor = Color.Black;
                // loop through cellList
                for (int i = 0; i < cellList.Count; i++)
                {
                    // if cellValue = BKGRND FILL
                    // then setup the cell with the style colour
                    if (cellList[i].CellValue.Equals("BKGRND FILL"))
                    {
                        cellDataGridView.Rows[cellList[i].Row].Cells[cellList[i].Col].Style = style;
                    }
                    else
                    {
                        cellDataGridView.Rows[cellList[i].Row].Cells[cellList[i].Col].Value = cellList[i].CellValue;
                    }
                }
            }
            else
            {
                MessageBox.Show("Error message: No data for this class", "Error");
            }

        }

        /*
         * METHOD:      SaveClassData()
         * PURPOSE:     Saves class data to external .csv file (save or save as)
         * Inputs:      String save type
         *              "save" is to save to the currentFilePath (over-writes existing file)
         *              "save as"is to save the file to a different file path
         * Outputs:     void
        */
        private void SaveClassData(string saveType)
        {
            // check if there is any data in the cellList first
            if (cellList.Count == 0)
            {
                return;
            }
            // check if there is a currentFilePath
            if (currentFilePath.Length <= 0)
            {
                return;
            }

            // get file content to save
            // firt we get teacher, class, room and date details
            string fileContent = "Teacher:," + teacherTextBox.Text + "\r\n";
            fileContent += "Class:," + classTextBox.Text + "\r\n";
            fileContent += "Room:," + roomTextBox.Text + "\r\n";
            fileContent += "Date:," + dateTextBox.Text + "\r\n";

            //loop through each row and column of dataGridView
            for (int row = 0; row < TOTAL_ROWS; row++)
            {
                for (int col = 0; col < TOTAL_COLS; col++)
                {
                    if (cellDataGridView.Rows[row].Cells[col].Value != null)
                    {
                        string cellValue = cellDataGridView.Rows[row].Cells[col].Value.ToString();
                        if (!String.IsNullOrEmpty(cellValue))
                        {
                            fileContent += col + "," + row + "," + cellValue + "\r\n";
                        }
                    }
                    else if (cellDataGridView.Rows[row].Cells[col].HasStyle)
                    {
                        fileContent += col + "," + row + "," + "BKGRND FILL,blue\r\n";
                    }
                }//End of Inner for loop
            }//end of outer for loop
             //MessageBox.Show(fileContent);
            DialogResult = MessageBox.Show(null, "you are about to save the class data to the following directory: "
                           + currentFilePath + "\n\n Do you wish to continue?", "Save Class", MessageBoxButtons.OKCancel);
            if (DialogResult == DialogResult.OK)
            {
                // proceed with save
                if (saveType.Equals("save"))
                {
                    try
                    {
                        File.WriteAllText(currentFilePath, fileContent);
                        MessageBox.Show("Class data saved to: " + currentFilePath, "FILE SAVE SUCCESFUL!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Could not save data to: " + currentFilePath, "FILE SAVE FAILED");
                        Console.WriteLine(ex.StackTrace);
                    }
                }
                else if (saveType.Equals("save as"))
                {
                    //to specified file path user choice
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.InitialDirectory = @"C:\Users\Audii\source\repos\ClassroomManager\bin\Debug\net6.0-windows\test.csv";
                    saveFileDialog.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";
                    saveFileDialog.FilterIndex = 2;
                    saveFileDialog.RestoreDirectory = true;
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        currentFilePath = saveFileDialog.FileName;
                        try
                        {
                            File.WriteAllText(currentFilePath, fileContent);
                            MessageBox.Show("Class data saved to: " + currentFilePath, "FILE SAVE SUCCESFUL!");
                        }
                        catch (Exception ey)
                        {
                            MessageBox.Show("Could not save data to: " + currentFilePath, "FILE SAVE FAILED");
                            Console.WriteLine(ey.StackTrace);
                        }
                    }
                }
                else
                {
                    if (saveType.Equals("save raf"))
                    {
                        SaveFileDialog saveFileDialog = new SaveFileDialog();
                        saveFileDialog.InitialDirectory = @"C:\Users\Audii\source\repos\ClassroomManager\bin\Debug\net6.0-windows\test.csv";
                        saveFileDialog.Filter = "dat files (*.dat)|*.dat|All files (*.*)|*.*";
                        saveFileDialog.FilterIndex = 2;
                        saveFileDialog.RestoreDirectory = true;
                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            currentFilePath = saveFileDialog.FileName;
                            try
                            {

                                rafSave(fileContent);
                                MessageBox.Show(fileContent);
                                //File.W(currentFilePath, fileContent);
                                //MessageBox.Show("Class data saved to: " + currentFilePath, "FILE SAVE SUCCESFUL!");
                            }
                            catch (Exception ez)
                            {
                                MessageBox.Show("Could not save data to: " + currentFilePath, "FILE SAVE FAILED");
                                Console.WriteLine(ez.StackTrace);
                            }
                        }
                    }
                }
            }
        }

        /*
         * METHOD:      Exit()
         * PURPOSE:     Exits the application
         * Inputs:      none
         * Outputs:     void
        */
        private void Exit()
        {
            //Exit the application
            if (System.Windows.Forms.Application.MessageLoop)
            {
                //Winforms app
                System.Windows.Forms.Application.Exit();
            }
            else
            {
                //Console app
                System.Environment.Exit(1);
            }
        }

        /*
        * METHOD:      ClearClassData()
        * PURPOSE:     Clear all relevant cells
        * Inputs:      none
        * Outputs:     void
       */
        private void ClearClassData()
        {
            // check if there is any data in the cellList first
            if (cellList.Count == 0)
            {
                return;
            }
            // check if there is a currentFilePath
            if (currentFilePath.Length <= 0)
            {
                return;
            }
            // clear Header titles first
            teacherTextBox.Text = "";
            classTextBox.Text = "";
            roomTextBox.Text = "";
            dateTextBox.Text = "";
            // Clear classroom names from tiles only
            if (cellList.Count > 0)
            {
                //display and setup data in the dataGridView
                DataGridViewCellStyle style = new DataGridViewCellStyle();
                // back colour light blue (fill)
                style.BackColor = Color.LightBlue;
                // for colour black (text)
                style.ForeColor = Color.Black;
                // string to reserve front desk
                string frontDesk = "Front Desk";
                // loop through cellList
                for (int i = 0; i < cellList.Count; i++)
                {
                    // if cellValue = BKGRND FILL
                    // then setup the cell with the style colour
                    // then replace cells will "" value
                    if (cellList[i].CellValue.Equals("BKGRND FILL"))
                    {
                        cellDataGridView.Rows[cellList[i].Row].Cells[cellList[i].Col].Style = style;
                    }

                    else if (cellList[i].CellValue.Equals("Front Desk"))
                    {
                        cellDataGridView.Rows[cellList[i].Row].Cells[cellList[i].Col].Value = frontDesk;
                    }
                    else
                    {
                        cellDataGridView.Rows[cellList[i].Row].Cells[cellList[i].Col].Value = "";
                    }
                }
            }
        }

        /*
        * METHOD:      saveRaf()
        * PURPOSE:     Save class data as a random access file (binary format)
        * Inputs:      takes a string and converts to binary for save (ToString.Acii Int 32)
        * Outputs:     void
       */
        static string rafSave(string data)
        {
            StringBuilder rafFile = new StringBuilder();

            foreach (char c in data.ToCharArray())
            {
                rafFile.Append(Convert.ToString(c, 2).PadLeft(8, '0'));
            }
            return rafFile.ToString();

        }

        /*
        * METHOD:      SortNames()
        * PURPOSE:     Sort the student names by alphabetical order
        * Inputs:      none
        * Outputs:     void
        */

        private void SortNames()
        {
            //loop through each row and column of dataGridView\
            foreach (DataGridViewRow row in cellDataGridView.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.Value.ToString() != "" && cell.Value.ToString() != null && cell.Value.ToString() != "BKGRND FILL" && cell.Value.ToString() != "Front Desk")
                    {
                        value = cell.Value.ToString();
                        studentNames.Add(value);
                        Console.WriteLine(studentNames);
                    }
                }
            }
            //convert the list to an array 
            array = studentNames.ToArray();
            //sort the array of students
            studentNames.Sort();
            string testOutput = "";
            for (int i = 0; i < studentNames.Count; i++)
            {
                testOutput += studentNames[i].ToString() + "\n";
            }
            MessageBox.Show(testOutput);
        }

        /*
        * METHOD:      findName()
        * PURPOSE:     find the student by name in list
        * Inputs:      none
        * Outputs:     void
        */

        private void findName()
        {
            if (String.IsNullOrEmpty(findNameTextBox.Text))
            {
                MessageBox.Show("You need to search a name here, cannot be empty", "ERROR!");
                return;
            }
            else
            {
                // 
                string nameSearch = findNameTextBox.Text.Trim();
                // boolean for foundStatus
                bool foundStatus = false;
                // linear search through the list

                for (int i = 0; i < cellList.Count; i++)
                {
                    //check if name exists in the list
                    if (nameSearch.Equals(cellList[i].CellValue))
                    {
                        MessageBox.Show(nameSearch + "  FOUND\n" + " Located at highlighted seat");
                        // change the foundstatus to true
                        foundStatus = true;
                        // break the loop
                        break;
                        // if found, change currentRecord to the index
                        currentRecord = i;



                    }// end of record search
                } // end of for loop
                // check if not found
                if (!foundStatus)
                {
                    // inform user that the record was not found
                    MessageBox.Show(nameSearch + "  could not be found please check the spelling and try again");
                }
            }

        }// end of button event handler



        /*
        * METHOD:      openToolStripMenuItem1_Click(object sender, EventArgs e)
        * PURPOSE:     event handler for the open menu item (opens a file from the currentFilePath)
        * Inputs:      object sender (the button object)
        *              EventArgs e (parameters of the object button)
        * Outputs:     void
        */
        private void openToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = @"C:\Users\Audii\source\repos\ClassroomManager\bin\Debug\net6.0-windows\test.csv";
            openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // get the path of specified file
                currentFilePath = openFileDialog.FileName;
                //  This just tests the path == MessageBox.Show(currentFilePath);

                //external file operations
                ReadClassData();
                // display the class data
                DisplayClassData();

            }
        }

        /*
        * METHOD:      saveToolStripMenuItem_Click(object sender, EventArgs e)
        * PURPOSE:     event handler for the open menu item (saves [currentFile]to the currentFilePath & overwrites)
        * Inputs:      object sender (the menu item File.Save button object)
        *              EventArgs e (parameters of the object button)
        * Outputs:     void
        */
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveClassData("save");
        }

        /*
        * METHOD:      saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        * PURPOSE:     event handler for the opens dialog and allows user to choose save path
        * Inputs:      object sender (the menu item File.Open object)
        *              EventArgs e (parameters of the object button)
        * Outputs:     void
        */
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveClassData("save as");
        }

        /*
        * METHOD:      exitToolStripMenuItem_Click(object sender, EventArgs e)
        * PURPOSE:     event handler for exiting the application
        * Inputs:      object sender (the menu item from File.Exit)
        *              EventArgs e (parameters of the object button)
        * Outputs:     void
        */
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Exit();
        }

        /*
        * METHOD:      clearBtn_Click(object sender, EventArgs e)
        * PURPOSE:     event handler for clearing the desks and people in desks
        * Inputs:      event handler for  (the clearBtn object)
        *              EventArgs e (parameters of the object button)
        * Outputs:     void
        */
        private void clearBtn_Click(object sender, EventArgs e)
        {
            //Confirm clearing classroom (seats only)
            //EventArgs e(parameters of the object button)
            ClearClassData();
        }

        /*
        * METHOD:      saveBtn_Click(object sender, EventArgs e)
        * PURPOSE:     event handler for saving the current classroom using SaveClassData ("save")
        * Inputs:      event handler for  (the saveBtn object)
        *              EventArgs e (parameters of the object button)
        * Outputs:     void
        */
        private void saveBtn_Click(object sender, EventArgs e)
        {
            SaveClassData("save");
        }

        /*
         * METHOD:      sortBtn_Click(object sender, EventArgs e)
         * PURPOSE:     event handler for Exiting application
         * Inputs:      event handler for  (the exitBtn exit object)
         *              EventArgs e (parameters of the object button)
         * Outputs:     void
         */
        private void sortBtn_Click(object sender, EventArgs e)
        {
            SortNames();
        }

        /*
        * METHOD:      exitBtn_Click(object sender, EventArgs e)
        * PURPOSE:     event handler for Exiting application
        * Inputs:      event handler for  (the exitBtn exit object)
        *              EventArgs e (parameters of the object button)
        * Outputs:     void
        */
        private void exitBtn_Click(object sender, EventArgs e)
        {
            Exit();
        }

        /*
        * METHOD:      openBtn_Click(object sender, EventArgs e)
        * PURPOSE:     event handler for the open button  (opens a file from the currentFilePath)
        * Inputs:      object sender (the button object)
        *              EventArgs e (parameters of the object button)
        * Outputs:     void
        */
        private void openBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = @"C:\Users\Audii\source\repos\ClassroomManager\bin\Debug\net6.0-windows\test.csv";
            openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // get the path of specified file
                currentFilePath = openFileDialog.FileName;
                //  This just tests the path == MessageBox.Show(currentFilePath);

                //external file operations
                ReadClassData();
                // display the class data
                DisplayClassData();

            }
        }

        private void rafBtn_Click(object sender, EventArgs e)
        {
            SaveClassData("save raf");
        }

        private void findBtn_Click(object sender, EventArgs e)
        {
            findName();
        }
    }
}