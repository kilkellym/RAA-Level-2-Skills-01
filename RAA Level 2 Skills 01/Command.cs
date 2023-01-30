#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Windows;

#endregion

namespace RAA_Level_2_Skills_01
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            // step 1: put any code needed for the form here

            // step 2: open form
            MyForm currentForm = new MyForm()
            {
                Width = 800,
                Height = 600,
                WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen,
                Topmost = true,
            };

            currentForm.ShowDialog();

            // step 4: get form data and do something
            if(currentForm.DialogResult == false)
            {
                return Result.Cancelled;
            }

            // do something
            List<string[]> dataList = new List<string[]>();
            string textboxresult = currentForm.GetTextBoxValue();

            string[] dataArray = System.IO.File.ReadAllLines(textboxresult);
                
            foreach(string data in dataArray)
            {
                string[] cellString = data.Split(',');
                dataList.Add(cellString);
            }

            // remove header row
            dataList.RemoveAt(0);

            bool checkBox1Value = currentForm.GetCheckbox1();

            string rabioButtonValue = currentForm.GetGroup1();

            //TaskDialog.Show("Test", "text box result is " + textboxresult);


            if(checkBox1Value == true)
            {
                TaskDialog.Show("Test", "Check box 1 was selected");
            }

            TaskDialog.Show("Test", rabioButtonValue);

            // go through csv data and do something

            Transaction transaction = new Transaction(doc);
            transaction.Start("Create level");

            foreach (string[] currentArray in dataList)
            {
                string text = currentArray[0];
                string number = currentArray[1];

                double actualNumber;
                bool convertNumber = double.TryParse(number, out actualNumber);

                if(convertNumber == false)
                {
                    continue;
                }

                // same code as TryParse
                double actualNumber2 = 0;
                try
                {
                    actualNumber2 = double.Parse(number);
                }
                catch (Exception)
                {
                    TaskDialog.Show("Error", "The item in the number column is not a number");
                }
                
                if(convertNumber == false)
                {
                    TaskDialog.Show("Error", "The item in the number column is not a number");
                }

                double metricConvert = actualNumber * 3.28084;
                Level currentLevel = Level.Create(doc, metricConvert);
                currentLevel.Name = text;

                ViewFamilyType planVFT = GetViewFamilyTypeByName(doc, "Floor Plan", ViewFamily.FloorPlan);
                ViewFamilyType ceilingPlanVFT = GetViewFamilyTypeByName(doc, "Ceiling Plan", ViewFamily.CeilingPlan);

                ViewPlan plan = ViewPlan.Create(doc, planVFT.Id, currentLevel.Id);
                ViewPlan ceilingPlan = ViewPlan.Create(doc, ceilingPlanVFT.Id, currentLevel.Id);

            }
            transaction.Commit();
            transaction.Dispose();

            return Result.Succeeded;
        }

        private ViewFamilyType GetViewFamilyTypeByName(Document doc, string typeName, ViewFamily viewFamily)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(ViewFamilyType));

            foreach(ViewFamilyType currentVFT in collector)
            {
                if(currentVFT.Name == typeName && currentVFT.ViewFamily == viewFamily)
                {
                    return currentVFT;
                }
            }

            return null;
        }

        public static String GetMethod()
        {
            var method = MethodBase.GetCurrentMethod().DeclaringType?.FullName;
            return method;
        }
    }
}

