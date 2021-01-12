using System.Collections;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;
using Tekla.Structures.Model.UI;
using TSM = Tekla.Structures.Model;
using TSMUI = Tekla.Structures.Model.UI;

namespace Tekla.Technology.Akit.UserScript
{
    static class Script
    {
        public static void Run(Tekla.Technology.Akit.IScript akit)
        {
            try
            {                
                Model model = new Model();
                TSMUI.ModelObjectSelector modelObjectSelector = new TSMUI.ModelObjectSelector();
                ModelObjectEnumerator objectsToCopyEnum = modelObjectSelector.GetSelectedObjects();

                ArrayList objectsToCopyArray = new ArrayList();

                while (objectsToCopyEnum.MoveNext())
                    objectsToCopyArray.Add(objectsToCopyEnum.Current);

                Beam sourceBeam = PickBeam();

                ArrayList modelObjectSelectorArray = new ArrayList();
                modelObjectSelectorArray.Add(sourceBeam);
                modelObjectSelector.Select(modelObjectSelectorArray);
                modelObjectSelectorArray.Clear();

                CopyToAnotherBeamByEndPoint(objectsToCopyArray, sourceBeam);
            }
            catch { }
        }

        public static Beam PickBeam()
        {
            Picker picker = new Picker();

            Beam pickedBeam = null;

            while (pickedBeam is null)
            {
                Part pickedPart = (Part)picker.PickObject(Picker.PickObjectEnum.PICK_ONE_PART, "Please pick a beam");

                if (pickedPart is Beam)
                    pickedBeam = (Beam)pickedPart;
            }

            return pickedBeam;
        }

        public static void CopyToAnotherBeamByEndPoint(ArrayList objectsToCopyArray, Beam sourceBeam)
        {
            try
            {
                Model model = new Model();

                Beam destinationBeam = PickBeam();

                CoordinateSystem sourceCoordinateSystem = sourceBeam.GetCoordinateSystem();
                sourceCoordinateSystem.Origin = sourceBeam.EndPoint;

                CoordinateSystem destinationCoordinateSystem = destinationBeam.GetCoordinateSystem();
                destinationCoordinateSystem.Origin = destinationBeam.EndPoint;

                foreach (ModelObject objectToCopy in objectsToCopyArray)
                    TSM.Operations.Operation.CopyObject(objectToCopy, sourceCoordinateSystem, destinationCoordinateSystem);

                model.CommitChanges();

                CopyToAnotherBeamByEndPoint(objectsToCopyArray, sourceBeam);
            }
            catch { }
        }
    }
}
