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
                Width = 500,
                Height = 450,
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
            string textboxresult = currentForm.GetTextBoxValue();

            bool checkBox1Value = currentForm.GetCheckbox1();

            string rabioButtonValue = currentForm.GetGroup1();

            TaskDialog.Show("Test", "text box result is " + textboxresult);

            if(checkBox1Value == true)
            {
                TaskDialog.Show("Test", "Check box 1 was selected");
            }

            TaskDialog.Show("Test", rabioButtonValue);


            return Result.Succeeded;
        }

        public static String GetMethod()
        {
            var method = MethodBase.GetCurrentMethod().DeclaringType?.FullName;
            return method;
        }
    }
}

