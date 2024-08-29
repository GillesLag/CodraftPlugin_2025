using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CodraftPlugin_Library
{
    public static class ElementSettings
    {
        const float feetToMm = 304.8f;
        const float radsToDegrees = 57.2958f;

        /// <summary>
        /// Set the parameters for a pipe element.
        /// </summary>
        /// <param name="pipe">A pipe element from the document.</param>
        public static void SetCodraftParametersPipe(Pipe pipe, JObject parameterConfiguration)
        {
            // Pipe parameters
            string startBoven = (string)parameterConfiguration["parameters"]["pipe"]["property_1"];
            string eindeBoven = (string)parameterConfiguration["parameters"]["pipe"]["property_2"];
            string startCenter = (string)parameterConfiguration["parameters"]["pipe"]["property_3"];
            string eindeCenter = (string)parameterConfiguration["parameters"]["pipe"]["property_4"];
            string startOnder = (string)parameterConfiguration["parameters"]["pipe"]["property_5"];
            string eindeOnder = (string)parameterConfiguration["parameters"]["pipe"]["property_6"];
            double startOffset = pipe.get_Parameter(BuiltInParameter.RBS_START_OFFSET_PARAM).AsDouble();
            double eindeOffset = pipe.get_Parameter(BuiltInParameter.RBS_END_OFFSET_PARAM).AsDouble();
            double diameter = pipe.get_Parameter(BuiltInParameter.RBS_PIPE_OUTER_DIAMETER).AsDouble();

            try
            {
                // Set parameters
                pipe.LookupParameter(startBoven).Set(startOffset > eindeOffset ? startOffset + diameter / 2 : eindeOffset + diameter / 2);
                pipe.LookupParameter(eindeBoven).Set(eindeOffset > startOffset ? startOffset + diameter / 2 : eindeOffset + diameter / 2);
                pipe.LookupParameter(startCenter).Set(startOffset > eindeOffset ? startOffset : eindeOffset);
                pipe.LookupParameter(eindeCenter).Set(eindeOffset > startOffset ? startOffset : eindeOffset);
                pipe.LookupParameter(startOnder).Set(startOffset > eindeOffset ? startOffset - diameter / 2 : eindeOffset - diameter / 2);
                pipe.LookupParameter(eindeOnder).Set(eindeOffset > startOffset ? startOffset - diameter / 2 : eindeOffset - diameter / 2);
            }
            catch (NullReferenceException ex)
            {
                TaskDialog td = new TaskDialog("ProjectParameters Fout");
                td.MainInstruction = "Niet alle project parameters zijn toegevoegd voor de pipes.\n\nZie details voor meer info.";
                td.ExpandedContent = "De volgende projectparamters moeten toegevoegd zijn in het project voor alle pipes.\n" +
                    "COD_bovenkant_buis_start,\n" +
                    "COD_bovenkant_buis_einde,\n" +
                    "COD_center_buis_start,\n" +
                    "COD_center_buis_einde,\n" +
                    "COD_onderkant_buis_start,\n" +
                    "COD_onderkant_buis_einde";
                td.Show();
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Fout", ex.Message);
            }
        }

        /// <summary>
        /// Set the parameters for a tee fitting.
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="fi"></param>
        public static void SetCodraftParametersTee(List<object> parameters, FamilyInstance fi, JObject file)
        {
            fi.LookupParameter((string)file["parameters"]["tee"]["property_1"]["revit"]).Set((double)parameters[0]);
            fi.LookupParameter((string)file["parameters"]["tee"]["property_2"]["revit"]).Set((double)parameters[1]);
            fi.LookupParameter((string)file["parameters"]["tee"]["property_3"]["revit"]).Set((double)parameters[2]);
            fi.LookupParameter((string)file["parameters"]["tee"]["property_4"]["revit"]).Set((double)parameters[3]);
            fi.LookupParameter((string)file["parameters"]["tee"]["property_5"]["revit"]).Set((double)parameters[4]);
            fi.LookupParameter((string)file["parameters"]["tee"]["property_6"]["revit"]).Set((double)parameters[5]);
            fi.LookupParameter((string)file["parameters"]["tee"]["property_7"]["revit"]).Set((double)parameters[6]);
            fi.LookupParameter((string)file["parameters"]["tee"]["property_8"]["revit"]).Set((double)parameters[7]);
            fi.LookupParameter((string)file["parameters"]["tee"]["property_9"]["revit"]).Set((double)parameters[8]);
            fi.LookupParameter((string)file["parameters"]["tee"]["property_10"]["revit"]).Set((double)parameters[9]);
            fi.LookupParameter((string)file["parameters"]["tee"]["property_11"]["revit"]).Set((double)parameters[10]);
            fi.LookupParameter((string)file["parameters"]["tee"]["property_12"]["revit"]).Set((double)parameters[11]);
            fi.LookupParameter((string)file["parameters"]["tee"]["property_13"]["revit"]).Set((double)parameters[12]);
            fi.LookupParameter((string)file["parameters"]["tee"]["property_14"]["revit"]).Set((double)parameters[13]);
            fi.LookupParameter((string)file["parameters"]["tee"]["property_15"]["revit"]).Set((double)parameters[14]);
            fi.LookupParameter((string)file["parameters"]["tee"]["property_16"]["revit"]).Set((double)parameters[15]);
            fi.LookupParameter((string)file["parameters"]["tee"]["property_17"]["revit"]).Set((double)parameters[16]);
            fi.LookupParameter((string)file["parameters"]["tee"]["property_18"]["revit"]).Set((double)parameters[17]);
            fi.LookupParameter((string)file["parameters"]["tee"]["property_19"]["revit"]).Set((string)parameters[18]);
            fi.LookupParameter((string)file["parameters"]["tee"]["property_20"]["revit"]).Set((string)parameters[19]);
            fi.LookupParameter((string)file["parameters"]["tee"]["property_21"]["revit"]).Set((string)parameters[20]);
            fi.LookupParameter((string)file["parameters"]["tee"]["property_22"]["revit"]).Set((string)parameters[21]);
            fi.LookupParameter((string)file["parameters"]["tee"]["property_23"]["revit"]).Set((string)parameters[22]);
            fi.LookupParameter((string)file["parameters"]["tee"]["property_24"]["revit"]).Set((string)parameters[23]);
        }

        /// <summary>
        /// Set the parameters for a transition fitting.
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="fi"></param>
        /// <param name="switchNds"></param>
        /// <param name="excentrischOrConcentrisch">"1" is excentrisch "0" is concentrisch</param>
        public static void SetCodraftParametersTransition(List<object> parameters, FamilyInstance fi, bool switchNds, int excentrischOrConcentrisch, JObject file)
        {
            if (!switchNds)
            {
                fi.LookupParameter((string)file["parameters"]["transistion"]["property_1"]["revit"]).Set((double)parameters[0]);
                fi.LookupParameter((string)file["parameters"]["transistion"]["property_2"]["revit"]).Set((double)parameters[1]);
                fi.LookupParameter((string)file["parameters"]["transistion"]["property_4"]["revit"]).Set((double)parameters[3]);
                fi.LookupParameter((string)file["parameters"]["transistion"]["property_5"]["revit"]).Set((double)parameters[4]);
                fi.LookupParameter((string)file["parameters"]["transistion"]["property_6"]["revit"]).Set((double)parameters[5]);
                fi.LookupParameter((string)file["parameters"]["transistion"]["property_7"]["revit"]).Set((double)parameters[6]);
                fi.LookupParameter((string)file["parameters"]["transistion"]["property_8"]["revit"]).Set((double)parameters[7]);
                fi.LookupParameter((string)file["parameters"]["transistion"]["property_9"]["revit"]).Set((double)parameters[8]);
                fi.LookupParameter((string)file["parameters"]["transistion"]["property_10"]["revit"]).Set((double)parameters[9]);
                fi.LookupParameter((string)file["parameters"]["transistion"]["property_11"]["revit"]).Set((double)parameters[10]);
            }
            else
            {
                fi.LookupParameter((string)file["parameters"]["transistion"]["property_1"]["revit"]).Set((double)parameters[1]);
                fi.LookupParameter((string)file["parameters"]["transistion"]["property_2"]["revit"]).Set((double)parameters[0]);
                fi.LookupParameter((string)file["parameters"]["transistion"]["property_4"]["revit"]).Set((double)parameters[4]);
                fi.LookupParameter((string)file["parameters"]["transistion"]["property_5"]["revit"]).Set((double)parameters[3]);
                fi.LookupParameter((string)file["parameters"]["transistion"]["property_6"]["revit"]).Set((double)parameters[6]);
                fi.LookupParameter((string)file["parameters"]["transistion"]["property_7"]["revit"]).Set((double)parameters[5]);
                fi.LookupParameter((string)file["parameters"]["transistion"]["property_8"]["revit"]).Set((double)parameters[8]);
                fi.LookupParameter((string)file["parameters"]["transistion"]["property_9"]["revit"]).Set((double)parameters[7]);
                fi.LookupParameter((string)file["parameters"]["transistion"]["property_10"]["revit"]).Set((double)parameters[10]);
                fi.LookupParameter((string)file["parameters"]["transistion"]["property_11"]["revit"]).Set((double)parameters[9]);
            }

            if (excentrischOrConcentrisch == 1)
                fi.LookupParameter((string)file["parameters"]["transistion"]["property_22"]["revit"]).Set(excentrischOrConcentrisch);
            else
                fi.LookupParameter((string)file["parameters"]["transistion"]["property_22"]["revit"]).Set(0);

            fi.LookupParameter((string)file["parameters"]["transistion"]["property_3"]["revit"]).Set((double)parameters[2]);
            fi.LookupParameter((string)file["parameters"]["transistion"]["property_12"]["revit"]).Set((string)parameters[11]);
            fi.LookupParameter((string)file["parameters"]["transistion"]["property_13"]["revit"]).Set((string)parameters[12]);
            fi.LookupParameter((string)file["parameters"]["transistion"]["property_14"]["revit"]).Set((string)parameters[13]);
            fi.LookupParameter((string)file["parameters"]["transistion"]["property_15"]["revit"]).Set((string)parameters[14]);
            fi.LookupParameter((string)file["parameters"]["transistion"]["property_16"]["revit"]).Set((string)parameters[15]);
            fi.LookupParameter((string)file["parameters"]["transistion"]["property_17"]["revit"]).Set((string)parameters[16]);
            fi.LookupParameter((string)file["parameters"]["transistion"]["property_21"]["revit"]).Set(1);
        }

        /// <summary>
        /// Set the systemtype of the pipeelement.
        /// </summary>
        /// <param name="pipe">A pipe element from the document.</param>
        /// <param name="doc">A revit document.</param>
        public static void SetPipeType(Pipe pipe, Document doc)
        {
            // Neem de huidige systemType van de pipe.
            string systemName = pipe.get_Parameter(BuiltInParameter.RBS_PIPING_SYSTEM_TYPE_PARAM).AsValueString();

            // Haal de juiste pipeType uit het document op basis van de systemName.
            PipeType pipeType = new FilteredElementCollector(doc)
                .OfClass(typeof(PipeType))
                .Where(x => x.Name.Split('%').First() == systemName)
                .Cast<PipeType>()
                .First();

            // Set de pipeType.
            pipe.PipeType = pipeType;
        }

        /// <summary>
        /// Set the paramters for an elbow element
        /// </summary>
        /// <param name="parameters">List with all parameters for an elbow.</param>
        /// <param name="fi">An elbow element from the document.</param>
        public static void SetCodraftParametersElbow(List<object> parameters, FamilyInstance fi, JObject file)
        {
            fi.LookupParameter((string)file["parameters"]["elbow"]["property_1"]["revit"]).Set((double)parameters[0]);
            fi.LookupParameter((string)file["parameters"]["elbow"]["property_2"]["revit"]).Set((double)parameters[1]);
            fi.LookupParameter((string)file["parameters"]["elbow"]["property_3"]["revit"]).Set((double)parameters[2]);
            fi.LookupParameter((string)file["parameters"]["elbow"]["property_4"]["revit"]).Set((double)parameters[3]);
            fi.LookupParameter((string)file["parameters"]["elbow"]["property_5"]["revit"]).Set((double)parameters[4]);
            fi.LookupParameter((string)file["parameters"]["elbow"]["property_6"]["revit"]).Set((double)parameters[5]);
            fi.LookupParameter((string)file["parameters"]["elbow"]["property_7"]["revit"]).Set((double)parameters[6]);
            fi.LookupParameter((string)file["parameters"]["elbow"]["property_8"]["revit"]).Set((double)parameters[7]);
            fi.LookupParameter((string)file["parameters"]["elbow"]["property_9"]["revit"]).Set((double)parameters[8]);
            fi.LookupParameter((string)file["parameters"]["elbow"]["property_10"]["revit"]).Set((double)parameters[9]);
            fi.LookupParameter((string)file["parameters"]["elbow"]["property_17"]["revit"]).Set((double)parameters[10] / radsToDegrees);
            fi.LookupParameter((string)file["parameters"]["elbow"]["property_11"]["revit"]).Set((string)parameters[11]);
            fi.LookupParameter((string)file["parameters"]["elbow"]["property_12"]["revit"]).Set((string)parameters[12]);
            fi.LookupParameter((string)file["parameters"]["elbow"]["property_13"]["revit"]).Set((string)parameters[13]);
            fi.LookupParameter((string)file["parameters"]["elbow"]["property_14"]["revit"]).Set((string)parameters[14]);
            fi.LookupParameter((string)file["parameters"]["elbow"]["property_15"]["revit"]).Set((string)parameters[15]);
            fi.LookupParameter((string)file["parameters"]["elbow"]["property_16"]["revit"]).Set((string)parameters[16]);
            fi.LookupParameter("COD_Isolatie").Set(1);
        }

        /// <summary>
        /// Set the parameter for a tap element
        /// </summary>
        /// <param name="parameters">List with all parameters for a tap</param>
        /// <param name="fi">A tap element from the document.</param>
        public static void SetCodraftParametersTap(List<object> parameters, FamilyInstance fi, JObject file)
        {
            fi.LookupParameter((string)file["parameters"]["tap"]["property_1"]["revit"]).Set((double)parameters[0]);
            fi.LookupParameter((string)file["parameters"]["tap"]["property_2"]["revit"]).Set((double)parameters[1]);
            fi.LookupParameter((string)file["parameters"]["tap"]["property_3"]["revit"]).Set((double)parameters[2]);
            fi.LookupParameter((string)file["parameters"]["tap"]["property_4"]["revit"]).Set((string)parameters[3]);
            fi.LookupParameter((string)file["parameters"]["tap"]["property_5"]["revit"]).Set((string)parameters[4]);
            fi.LookupParameter((string)file["parameters"]["tap"]["property_6"]["revit"]).Set((string)parameters[5]);
            fi.LookupParameter((string)file["parameters"]["tap"]["property_7"]["revit"]).Set((string)parameters[6]);
            fi.LookupParameter((string)file["parameters"]["tap"]["property_8"]["revit"]).Set((string)parameters[7]);
            fi.LookupParameter((string)file["parameters"]["tap"]["property_9"]["revit"]).Set((string)parameters[8]);
            fi.LookupParameter((string)file["parameters"]["tap"]["property_11"]["revit"]).Set(1);
        }

        /// <summary>
        /// Set the parameters for an elbow fitting that does not exist in the database.
        /// </summary>
        /// <param name="fi">Elbow fitting from the document.</param>
        public static void ElbowDoesNotExist(FamilyInstance fi, JObject file)
        {
            fi.LookupParameter((string)file["parameters"]["elbow"]["property_11"]["revit"]).Set("BESTAAT NIET!");
            fi.LookupParameter((string)file["parameters"]["elbow"]["property_12"]["revit"]).Set("BESTAAT NIET!");
            fi.LookupParameter((string)file["parameters"]["elbow"]["property_13"]["revit"]).Set("BESTAAT NIET!");
            fi.LookupParameter((string)file["parameters"]["elbow"]["property_14"]["revit"]).Set("BESTAAT NIET!");
            fi.LookupParameter((string)file["parameters"]["elbow"]["property_15"]["revit"]).Set("BESTAAT NIET!");
            fi.LookupParameter((string)file["parameters"]["elbow"]["property_16"]["revit"]).Set("nee");

            fi.LookupParameter((string)file["parameters"]["elbow"]["property_1"]["revit"]).Set(15 / feetToMm);
            fi.LookupParameter((string)file["parameters"]["elbow"]["property_2"]["revit"]).Set(15 / feetToMm);
            fi.LookupParameter((string)file["parameters"]["elbow"]["property_3"]["revit"]).Set(30 / feetToMm);
            fi.LookupParameter((string)file["parameters"]["elbow"]["property_4"]["revit"]).Set(0);
            fi.LookupParameter((string)file["parameters"]["elbow"]["property_5"]["revit"]).Set(0);
            fi.LookupParameter((string)file["parameters"]["elbow"]["property_6"]["revit"]).Set(0);
            fi.LookupParameter((string)file["parameters"]["elbow"]["property_7"]["revit"]).Set(0);
            fi.LookupParameter((string)file["parameters"]["elbow"]["property_8"]["revit"]).Set(0);
            fi.LookupParameter((string)file["parameters"]["elbow"]["property_9"]["revit"]).Set(0);
        }

        /// <summary>
        /// Set the parameter for a tapfitting that does not exist in the database.
        /// </summary>
        /// <param name="fi">Tap fitting from the document</param>
        public static void TapDoesNotExist(FamilyInstance fi, JObject file)
        {
            fi.LookupParameter((string)file["parameters"]["tap"]["property_4"]["revit"]).Set("BESTAAT NIET!");
            fi.LookupParameter((string)file["parameters"]["tap"]["property_5"]["revit"]).Set("BESTAAT NIET!");
            fi.LookupParameter((string)file["parameters"]["tap"]["property_6"]["revit"]).Set("BESTAAT NIET!");
            fi.LookupParameter((string)file["parameters"]["tap"]["property_7"]["revit"]).Set("BESTAAT NIET!");
            fi.LookupParameter((string)file["parameters"]["tap"]["property_8"]["revit"]).Set("BESTAAT NIET!");
            fi.LookupParameter((string)file["parameters"]["tap"]["property_9"]["revit"]).Set("nee");

            fi.LookupParameter((string)file["parameters"]["tap"]["property_1"]["revit"]).Set(15 / feetToMm);
            fi.LookupParameter((string)file["parameters"]["tap"]["property_2"]["revit"]).Set(15 / feetToMm);
            fi.LookupParameter((string)file["parameters"]["tap"]["property_3"]["revit"]).Set(15 / feetToMm);
        }

        /// <summary>
        /// Set the parameters for a tee fitting that does not exist in the database.
        /// </summary>
        /// <param name="fi">Tee fitting from the document</param>
        public static void TeeDoesNotExist(FamilyInstance fi, JObject file)
        {
            fi.LookupParameter((string)file["parameters"]["tee"]["property_19"]["revit"]).Set("BESTAAT NIET!");
            fi.LookupParameter((string)file["parameters"]["tee"]["property_20"]["revit"]).Set("BESTAAT NIET!");
            fi.LookupParameter((string)file["parameters"]["tee"]["property_21"]["revit"]).Set("BESTAAT NIET!");
            fi.LookupParameter((string)file["parameters"]["tee"]["property_22"]["revit"]).Set("BESTAAT NIET!");
            fi.LookupParameter((string)file["parameters"]["tee"]["property_23"]["revit"]).Set("BESTAAT NIET!");
            fi.LookupParameter((string)file["parameters"]["tee"]["property_24"]["revit"]).Set("nee");

            fi.LookupParameter((string)file["parameters"]["tee"]["property_1"]["revit"]).Set(15 / feetToMm);
            fi.LookupParameter((string)file["parameters"]["tee"]["property_2"]["revit"]).Set(15 / feetToMm);
            fi.LookupParameter((string)file["parameters"]["tee"]["property_3"]["revit"]).Set(15 / feetToMm);
            fi.LookupParameter((string)file["parameters"]["tee"]["property_4"]["revit"]).Set(15 / feetToMm);
            fi.LookupParameter((string)file["parameters"]["tee"]["property_5"]["revit"]).Set(15 / feetToMm);
            fi.LookupParameter((string)file["parameters"]["tee"]["property_6"]["revit"]).Set(15 / feetToMm);
            fi.LookupParameter((string)file["parameters"]["tee"]["property_7"]["revit"]).Set(0);
            fi.LookupParameter((string)file["parameters"]["tee"]["property_8"]["revit"]).Set(0);
            fi.LookupParameter((string)file["parameters"]["tee"]["property_9"]["revit"]).Set(0);
            fi.LookupParameter((string)file["parameters"]["tee"]["property_10"]["revit"]).Set(0);
            fi.LookupParameter((string)file["parameters"]["tee"]["property_11"]["revit"]).Set(0);
            fi.LookupParameter((string)file["parameters"]["tee"]["property_12"]["revit"]).Set(0);
            fi.LookupParameter((string)file["parameters"]["tee"]["property_13"]["revit"]).Set(0);
            fi.LookupParameter((string)file["parameters"]["tee"]["property_14"]["revit"]).Set(0);
            fi.LookupParameter((string)file["parameters"]["tee"]["property_15"]["revit"]).Set(0);
        }

        /// <summary>
        /// Set the parameters for a transition fitting that does not exist in the database.
        /// </summary>
        /// <param name="fi">Transition fitting from the document</param>
        public static void TransitionDoesNotExist(FamilyInstance fi, JObject file)
        {
            fi.LookupParameter((string)file["parameters"]["transistion"]["property_12"]["revit"]).Set("BESTAAT NIET!");
            fi.LookupParameter((string)file["parameters"]["transistion"]["property_13"]["revit"]).Set("BESTAAT NIET!");
            fi.LookupParameter((string)file["parameters"]["transistion"]["property_14"]["revit"]).Set("BESTAAT NIET!");
            fi.LookupParameter((string)file["parameters"]["transistion"]["property_15"]["revit"]).Set("BESTAAT NIET!");
            fi.LookupParameter((string)file["parameters"]["transistion"]["property_16"]["revit"]).Set("BESTAAT NIET!");
            fi.LookupParameter((string)file["parameters"]["transistion"]["property_17"]["revit"]).Set("nee");

            fi.LookupParameter((string)file["parameters"]["transistion"]["property_1"]["revit"]).Set(15 / feetToMm);
            fi.LookupParameter((string)file["parameters"]["transistion"]["property_2"]["revit"]).Set(15 / feetToMm);
            fi.LookupParameter((string)file["parameters"]["transistion"]["property_3"]["revit"]).Set(15 / feetToMm);
            fi.LookupParameter((string)file["parameters"]["transistion"]["property_4"]["revit"]).Set(0);
            fi.LookupParameter((string)file["parameters"]["transistion"]["property_5"]["revit"]).Set(0);
            fi.LookupParameter((string)file["parameters"]["transistion"]["property_6"]["revit"]).Set(0);
            fi.LookupParameter((string)file["parameters"]["transistion"]["property_7"]["revit"]).Set(0);
            fi.LookupParameter((string)file["parameters"]["transistion"]["property_8"]["revit"]).Set(0);
            fi.LookupParameter((string)file["parameters"]["transistion"]["property_9"]["revit"]).Set(0);
        }

        /// <summary>
        /// Applies the insulation for the current pipe
        /// </summary>
        /// <param name="elemId">Id of the pipe element</param>
        /// <param name="lookupValues">The values from the access database</param>
        /// <param name="doc">the current revit document</param>
        public static void ApplyInsulation(ElementId elemId, List<object> lookupValues, Document doc)
        {
            string insulType = (string)lookupValues[0];
            double insulThickness = (double)lookupValues[1];
            List<ElementId> id = InsulationLiningBase.GetInsulationIds(doc, elemId).ToList();

            if (id.Count > 0)
                doc.Delete(id.First());

            try
            {
                PipeInsulationType insulationType = (PipeInsulationType)new FilteredElementCollector(doc)
                        .OfCategory(BuiltInCategory.OST_PipeInsulations)
                        .Where(x => x.Name == insulType)
                        .First();

                PipeInsulation.Create(doc, elemId, insulationType.Id, insulThickness);
            }
            catch (Exception)
            {
                TaskDialog.Show("Isolatie Error", $"De isolatietype van de database is niet in Revit gevonden. Het type {insulType} moet aangemaakt worden in de PipeInsulation family!");
            }

            
        }

        /// <summary>
        /// Checks if there's allready insulation applied.
        /// </summary>
        /// <param name="pipe">the current pipe element</param>
        /// <param name="lookupValues">the insulation values from the access database</param>
        /// <returns>true if insulation is allready applied, otherwise false.</returns>
        public static bool IsPipeInsulationApplied(Pipe pipe, List<object> lookupValues)
        {
            string insulName = pipe.get_Parameter(BuiltInParameter.RBS_REFERENCE_INSULATION_TYPE).AsString();
            double insulThickness = Math.Round(pipe.get_Parameter(BuiltInParameter.RBS_REFERENCE_INSULATION_THICKNESS).AsDouble() * feetToMm, 2);

            if ((string)lookupValues[0] == insulName && Math.Round((double)lookupValues[1] * feetToMm, 2) == insulThickness)
                return true;

            return false;
        }

        /// <summary>
        /// Checks if there's allready insulation applied.
        /// </summary>
        /// <param name="fi">the current fitting element</param>
        /// <param name="lookupValues">the insulation values from the access database</param>
        /// <returns>true if insulation is allready applied, otherwise false.</returns>
        public static bool IsFittingInsulationApplied(FamilyInstance fi, List<object> lookupValues)
        {
            string insulName = fi.get_Parameter(BuiltInParameter.RBS_REFERENCE_INSULATION_TYPE).AsString();
            double insulThickness = Math.Round(fi.get_Parameter(BuiltInParameter.RBS_REFERENCE_INSULATION_THICKNESS).AsDouble() * feetToMm, 2);

            if ((string)lookupValues[0] == insulName && Math.Round((double)lookupValues[1] * feetToMm, 2) == insulThickness)
                return true;

            return false;
        }

        /// <summary>
        /// Compares the paramaters form both list with eachother.The parameters that need to be compared in both list must be in the same order.
        /// </summary>
        /// <param name="revitParameters"></param>
        /// <param name="databaseParameters"></param>
        /// <returns>true if all the compared parameters are true, otherwise false.</returns>
        public static bool CompareParameters(List<object> revitParameters, List<object> databaseParameters)
        {
            for (int i = 0; i < revitParameters.Count; i++)
            {
                object revitItem = revitParameters[i];
                object databaseItem = databaseParameters[i];

                if (revitItem is double paramDouble)
                {
                    if (paramDouble != (double)databaseItem)
                        return false;

                    continue;
                }

                if (revitItem is int paramInt)
                {
                    if (paramInt != (int)databaseItem)
                        return false;

                    continue;
                }

                if (revitItem is string paramString)
                {
                    if (paramString != (string)databaseItem)
                        return false;

                    continue;
                }
            }

            return true;
        }

        public static void SetCodraftParamtersStraightValve(List<object> parameters, FamilyInstance pipeAccessory, JObject file)
        {
            pipeAccessory.LookupParameter((string)file["parameters"]["straightValve"]["property_1"]["revit"]).Set((double)parameters[0]);
            pipeAccessory.LookupParameter((string)file["parameters"]["straightValve"]["property_2"]["revit"]).Set((double)parameters[1]);
            //pipeAccessory.LookupParameter((string)file["parameters"]["straightValve"]["property_3"]["revit"]).Set((double)parameters[2]);
            //pipeAccessory.LookupParameter((string)file["parameters"]["straightValve"]["property_4"]["revit"]).Set((double)parameters[3]);
            pipeAccessory.LookupParameter((string)file["parameters"]["straightValve"]["property_5"]["revit"]).Set((double)parameters[4]);
            pipeAccessory.LookupParameter((string)file["parameters"]["straightValve"]["property_6"]["revit"]).Set((double)parameters[5]);
            pipeAccessory.LookupParameter((string)file["parameters"]["straightValve"]["property_7"]["revit"]).Set((double)parameters[6]);
            pipeAccessory.LookupParameter((string)file["parameters"]["straightValve"]["property_8"]["revit"]).Set((double)parameters[7]);
            pipeAccessory.LookupParameter((string)file["parameters"]["straightValve"]["property_9"]["revit"]).Set((double)parameters[8]);
            pipeAccessory.LookupParameter((string)file["parameters"]["straightValve"]["property_10"]["revit"]).Set((double)parameters[9]);
            pipeAccessory.LookupParameter((string)file["parameters"]["straightValve"]["property_11"]["revit"]).Set((double)parameters[10]);
            pipeAccessory.LookupParameter((string)file["parameters"]["straightValve"]["property_12"]["revit"]).Set((double)parameters[11]);
            pipeAccessory.LookupParameter((string)file["parameters"]["straightValve"]["property_13"]["revit"]).Set((int)parameters[12]);
            pipeAccessory.LookupParameter((string)file["parameters"]["straightValve"]["property_14"]["revit"]).Set((int)parameters[13]);
            pipeAccessory.LookupParameter((string)file["parameters"]["straightValve"]["property_15"]["revit"]).Set((double)parameters[14]);
            pipeAccessory.LookupParameter((string)file["parameters"]["straightValve"]["property_16"]["revit"]).Set((double)parameters[15]);
            pipeAccessory.LookupParameter((string)file["parameters"]["straightValve"]["property_17"]["revit"]).Set((double)parameters[16]);
            pipeAccessory.LookupParameter((string)file["parameters"]["straightValve"]["property_18"]["revit"]).Set((double)parameters[17]);
            pipeAccessory.LookupParameter((string)file["parameters"]["straightValve"]["property_19"]["revit"]).Set((string)parameters[18]);
            pipeAccessory.LookupParameter((string)file["parameters"]["straightValve"]["property_20"]["revit"]).Set((string)parameters[19]);
            pipeAccessory.LookupParameter((string)file["parameters"]["straightValve"]["property_21"]["revit"]).Set((string)parameters[20]);
            pipeAccessory.LookupParameter((string)file["parameters"]["straightValve"]["property_22"]["revit"]).Set((string)parameters[21]);
            pipeAccessory.LookupParameter((string)file["parameters"]["straightValve"]["property_23"]["revit"]).Set((string)parameters[22]);
            pipeAccessory.LookupParameter((string)file["parameters"]["straightValve"]["property_24"]["revit"]).Set((string)parameters[23]);
        }

        public static void SetCodraftParametersBalanceValve(List<object> parameters, FamilyInstance pipeAccessory, JObject file)
        {
            pipeAccessory.LookupParameter((string)file["parameters"]["balanceValve"]["property_1"]["revit"]).Set((double)parameters[0]);
            pipeAccessory.LookupParameter((string)file["parameters"]["balanceValve"]["property_2"]["revit"]).Set((double)parameters[1]);
            pipeAccessory.LookupParameter((string)file["parameters"]["balanceValve"]["property_3"]["revit"]).Set((int)parameters[2]);
            pipeAccessory.LookupParameter((string)file["parameters"]["balanceValve"]["property_4"]["revit"]).Set((int)parameters[3]);
            pipeAccessory.LookupParameter((string)file["parameters"]["balanceValve"]["property_5"]["revit"]).Set((double)parameters[4]);
            pipeAccessory.LookupParameter((string)file["parameters"]["balanceValve"]["property_6"]["revit"]).Set((double)parameters[5]);
            pipeAccessory.LookupParameter((string)file["parameters"]["balanceValve"]["property_7"]["revit"]).Set((double)parameters[6]);
            pipeAccessory.LookupParameter((string)file["parameters"]["balanceValve"]["property_8"]["revit"]).Set((double)parameters[7]);
            pipeAccessory.LookupParameter((string)file["parameters"]["balanceValve"]["property_9"]["revit"]).Set((string)parameters[8]);
            pipeAccessory.LookupParameter((string)file["parameters"]["balanceValve"]["property_10"]["revit"]).Set((string)parameters[9]);
            pipeAccessory.LookupParameter((string)file["parameters"]["balanceValve"]["property_11"]["revit"]).Set((string)parameters[10]);
            pipeAccessory.LookupParameter((string)file["parameters"]["balanceValve"]["property_12"]["revit"]).Set((string)parameters[11]);
            pipeAccessory.LookupParameter((string)file["parameters"]["balanceValve"]["property_13"]["revit"]).Set((string)parameters[12]);
            pipeAccessory.LookupParameter((string)file["parameters"]["balanceValve"]["property_14"]["revit"]).Set((string)parameters[13]);
        }

        public static void SetCodraftParametersStrainer(List<object> parameters, FamilyInstance pipeAccessory, JObject file)
        {
            pipeAccessory.LookupParameter((string)file["parameters"]["strainer"]["property_1"]["revit"]).Set((double)parameters[0]);
            pipeAccessory.LookupParameter((string)file["parameters"]["strainer"]["property_2"]["revit"]).Set((double)parameters[1]);
            pipeAccessory.LookupParameter((string)file["parameters"]["strainer"]["property_3"]["revit"]).Set((double)parameters[2]);
            pipeAccessory.LookupParameter((string)file["parameters"]["strainer"]["property_4"]["revit"]).Set((double)parameters[3]);
            pipeAccessory.LookupParameter((string)file["parameters"]["strainer"]["property_5"]["revit"]).Set((int)parameters[4]);
            pipeAccessory.LookupParameter((string)file["parameters"]["strainer"]["property_6"]["revit"]).Set((int)parameters[5]);
            pipeAccessory.LookupParameter((string)file["parameters"]["strainer"]["property_7"]["revit"]).Set((double)parameters[6]);
            pipeAccessory.LookupParameter((string)file["parameters"]["strainer"]["property_8"]["revit"]).Set((double)parameters[7]);
            pipeAccessory.LookupParameter((string)file["parameters"]["strainer"]["property_9"]["revit"]).Set((double)parameters[8]);
            pipeAccessory.LookupParameter((string)file["parameters"]["strainer"]["property_10"]["revit"]).Set((double)parameters[9]);
            pipeAccessory.LookupParameter((string)file["parameters"]["strainer"]["property_11"]["revit"]).Set((string)parameters[10]);
            pipeAccessory.LookupParameter((string)file["parameters"]["strainer"]["property_12"]["revit"]).Set((string)parameters[11]);
            pipeAccessory.LookupParameter((string)file["parameters"]["strainer"]["property_13"]["revit"]).Set((string)parameters[12]);
            pipeAccessory.LookupParameter((string)file["parameters"]["strainer"]["property_14"]["revit"]).Set((string)parameters[13]);
            pipeAccessory.LookupParameter((string)file["parameters"]["strainer"]["property_15"]["revit"]).Set((string)parameters[14]);
            pipeAccessory.LookupParameter((string)file["parameters"]["strainer"]["property_16"]["revit"]).Set((string)parameters[15]);
        }

        public static void SetCodraftParametersThreeWayGlobeValve(List<object> parameters, FamilyInstance pipeAccessory, JObject file)
        {
            pipeAccessory.LookupParameter((string)file["parameters"]["threewayGlobeValve"]["property_1"]["revit"]).Set((double)parameters[0]);
            pipeAccessory.LookupParameter((string)file["parameters"]["threewayGlobeValve"]["property_2"]["revit"]).Set((double)parameters[1]);
            pipeAccessory.LookupParameter((string)file["parameters"]["threewayGlobeValve"]["property_3"]["revit"]).Set((double)parameters[2]);
            pipeAccessory.LookupParameter((string)file["parameters"]["threewayGlobeValve"]["property_4"]["revit"]).Set((int)parameters[3]);
            pipeAccessory.LookupParameter((string)file["parameters"]["threewayGlobeValve"]["property_5"]["revit"]).Set((int)parameters[4]);
            pipeAccessory.LookupParameter((string)file["parameters"]["threewayGlobeValve"]["property_6"]["revit"]).Set((int)parameters[5]);
            pipeAccessory.LookupParameter((string)file["parameters"]["threewayGlobeValve"]["property_7"]["revit"]).Set((double)parameters[6]);
            pipeAccessory.LookupParameter((string)file["parameters"]["threewayGlobeValve"]["property_8"]["revit"]).Set((double)parameters[7]);
            pipeAccessory.LookupParameter((string)file["parameters"]["threewayGlobeValve"]["property_9"]["revit"]).Set((double)parameters[8]);
            pipeAccessory.LookupParameter((string)file["parameters"]["threewayGlobeValve"]["property_10"]["revit"]).Set((double)parameters[9]);
            pipeAccessory.LookupParameter((string)file["parameters"]["threewayGlobeValve"]["property_11"]["revit"]).Set((double)parameters[10]);
            pipeAccessory.LookupParameter((string)file["parameters"]["threewayGlobeValve"]["property_12"]["revit"]).Set((double)parameters[11]);
            pipeAccessory.LookupParameter((string)file["parameters"]["threewayGlobeValve"]["property_13"]["revit"]).Set((double)parameters[12]);
            pipeAccessory.LookupParameter((string)file["parameters"]["threewayGlobeValve"]["property_14"]["revit"]).Set((double)parameters[13]);
            pipeAccessory.LookupParameter((string)file["parameters"]["threewayGlobeValve"]["property_15"]["revit"]).Set((double)parameters[14]);
            pipeAccessory.LookupParameter((string)file["parameters"]["threewayGlobeValve"]["property_16"]["revit"]).Set((double)parameters[15]);
            pipeAccessory.LookupParameter((string)file["parameters"]["threewayGlobeValve"]["property_17"]["revit"]).Set((double)parameters[16]);
            pipeAccessory.LookupParameter((string)file["parameters"]["threewayGlobeValve"]["property_18"]["revit"]).Set((double)parameters[17]);
            pipeAccessory.LookupParameter((string)file["parameters"]["threewayGlobeValve"]["property_19"]["revit"]).Set((string)parameters[18]);
            pipeAccessory.LookupParameter((string)file["parameters"]["threewayGlobeValve"]["property_20"]["revit"]).Set((string)parameters[19]);
            pipeAccessory.LookupParameter((string)file["parameters"]["threewayGlobeValve"]["property_21"]["revit"]).Set((string)parameters[20]);
            pipeAccessory.LookupParameter((string)file["parameters"]["threewayGlobeValve"]["property_22"]["revit"]).Set((string)parameters[21]);
            pipeAccessory.LookupParameter((string)file["parameters"]["threewayGlobeValve"]["property_23"]["revit"]).Set((string)parameters[22]);
            pipeAccessory.LookupParameter((string)file["parameters"]["threewayGlobeValve"]["property_24"]["revit"]).Set((string)parameters[23]);
        }

        public static void SetCodraftParametersButterflyValve(List<object> parameters, FamilyInstance pipeAccessory, JObject file)
        {
            pipeAccessory.LookupParameter((string)file["parameters"]["butterflyValve"]["property_1"]["revit"]).Set((double)parameters[0]);
            pipeAccessory.LookupParameter((string)file["parameters"]["butterflyValve"]["property_2"]["revit"]).Set((double)parameters[1]);
            pipeAccessory.LookupParameter((string)file["parameters"]["butterflyValve"]["property_3"]["revit"]).Set((double)parameters[2]);
            pipeAccessory.LookupParameter((string)file["parameters"]["butterflyValve"]["property_4"]["revit"]).Set((double)parameters[3]);
            pipeAccessory.LookupParameter((string)file["parameters"]["butterflyValve"]["property_5"]["revit"]).Set((double)parameters[4]);
            pipeAccessory.LookupParameter((string)file["parameters"]["butterflyValve"]["property_6"]["revit"]).Set((double)parameters[5]);
            pipeAccessory.LookupParameter((string)file["parameters"]["butterflyValve"]["property_7"]["revit"]).Set((double)parameters[6]);
            pipeAccessory.LookupParameter((string)file["parameters"]["butterflyValve"]["property_8"]["revit"]).Set((double)parameters[7]);
            pipeAccessory.LookupParameter((string)file["parameters"]["butterflyValve"]["property_9"]["revit"]).Set((double)parameters[8]);
            pipeAccessory.LookupParameter((string)file["parameters"]["butterflyValve"]["property_10"]["revit"]).Set((double)parameters[9]);
            pipeAccessory.LookupParameter((string)file["parameters"]["butterflyValve"]["property_11"]["revit"]).Set((double)parameters[10]);
            pipeAccessory.LookupParameter((string)file["parameters"]["butterflyValve"]["property_12"]["revit"]).Set((double)parameters[11]);
            pipeAccessory.LookupParameter((string)file["parameters"]["butterflyValve"]["property_13"]["revit"]).Set((string)parameters[12]);
            pipeAccessory.LookupParameter((string)file["parameters"]["butterflyValve"]["property_14"]["revit"]).Set((string)parameters[13]);
            pipeAccessory.LookupParameter((string)file["parameters"]["butterflyValve"]["property_15"]["revit"]).Set((string)parameters[14]);
            pipeAccessory.LookupParameter((string)file["parameters"]["butterflyValve"]["property_16"]["revit"]).Set((string)parameters[15]);
            pipeAccessory.LookupParameter((string)file["parameters"]["butterflyValve"]["property_17"]["revit"]).Set((string)parameters[16]);
            pipeAccessory.LookupParameter((string)file["parameters"]["butterflyValve"]["property_18"]["revit"]).Set((string)parameters[17]);
        }
    }
}
