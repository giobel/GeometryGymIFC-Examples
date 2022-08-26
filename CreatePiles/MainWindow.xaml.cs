using GeometryGym.Ifc;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;

namespace CreatePiles
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {


            DatabaseIfc db = new DatabaseIfc(ModelView.Ifc4DesignTransfer);

            IfcBuilding building = new IfcBuilding(db, "Station") { };
            IfcProject project = new IfcProject(building, "IfcProject", IfcUnitAssignment.Length.Metre) { };

            List<Pile> piles = new List<Pile>();

            using (var reader = new StreamReader(this.inputCsvFile.Text))
            {
                string headerLine = reader.ReadLine();
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    if ( line != "")
                    {
                        var values = line.Split(',');

                        piles.Add(new Pile(values[0], int.Parse(values[1]), double.Parse(values[2]), double.Parse(values[3]), double.Parse(values[4]), double.Parse(values[5])));
                    }
                    
                }
            }

            foreach (Pile item in piles)
            { 
                IfcExtrudedAreaSolid extrudedAreaSolid = new IfcExtrudedAreaSolid(new IfcCircleProfileDef(db, $"D{item.Diameter}", item.Diameter*0.0005), item.Length);
                IfcProductDefinitionShape productDefinitionShape = new IfcProductDefinitionShape(new IfcShapeRepresentation(extrudedAreaSolid));
                IfcObjectPlacement placement = new IfcLocalPlacement(
                                                    new IfcAxis2Placement3D(
                                                        new IfcCartesianPoint(db, item.Easting, item.Northings, item.CutOffLevel)
                                                        )
                                                    );
                IfcPile pile = new IfcPile(building, placement, productDefinitionShape);
                pile.Name = item.Name;
                pile.Description = $"A bored {item.Diameter} dia pile";
                pile.ConstructionType = IfcPileConstructionEnum.BORED;
            }
 
            db.WriteFile(this.outputFolder.Text);

            System.Diagnostics.Process.Start(this.outputFolder.Text);
        }
    }

    internal class Pile
    {
        public string Name { get; set; }
        public int Diameter { get; set; }
        public double Easting { get; set; }
        public double Northings { get; set; }
        public double CutOffLevel { get; set; }
        public double Length { get; set; }


        public Pile(string Name, int Diameter, double Easting, double Northern, double CutOffLevel, double Length)
        {
            this.Name = Name;
            this.Diameter = Diameter;
            this.Easting = Easting;
            this.Northings = Northern;
            this.CutOffLevel = CutOffLevel;
            this.Length = Length;
        }

    }
}
