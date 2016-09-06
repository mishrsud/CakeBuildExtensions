using System;
using System.IO;
using System.Text;
using Cake.Core;
using Cake.Core.Annotations;

namespace BuildVersionExtension
{
    public static class BuildVersionIncrementor
    {
        private static int incParamNum = 0;
        private static string fileName = "";
        private static string versionStr = null;
        private static bool isVB = false;

        [CakeMethodAlias]
        public static void IncrementVersionNumber(this ICakeContext context, string assemblyInfoPath, int incrementAtIndex)
        {
            fileName = assemblyInfoPath;
            incParamNum = incrementAtIndex;

            if (Path.GetExtension(fileName).ToLower() == ".vb")
                isVB = true;

            StreamReader reader = new StreamReader(fileName);
            StreamWriter writer = new StreamWriter(fileName + ".out");
            String line;

            while ((line = reader.ReadLine()) != null)
            {
                line = ProcessLine(line);
                writer.WriteLine(line);
            }
            reader.Close();
            writer.Close();

            File.Delete(fileName);
            File.Move(fileName + ".out", fileName);
        }

        private static string ProcessLine(string line)
        {
            if (isVB)
            {
                line = ProcessLinePart(line, "<Assembly: AssemblyVersion(\"");
                line = ProcessLinePart(line, "<Assembly: AssemblyFileVersion(\"");
            }
            else
            {
                line = ProcessLinePart(line, "[assembly: AssemblyVersion(\"");
                line = ProcessLinePart(line, "[assembly: AssemblyFileVersion(\"");
            }
            return line;
        }

        private static string ProcessLinePart(string line, string part)
        {
            int spos = line.IndexOf(part);
            if (spos >= 0)
            {
                spos += part.Length;
                int epos = line.IndexOf('"', spos);
                string oldVersion = line.Substring(spos, epos - spos);
                string newVersion = "";
                bool performChange = false;

                if (incParamNum > 0)
                {
                    string[] nums = oldVersion.Split('.');
                    if (nums.Length >= incParamNum && nums[incParamNum - 1] != "*")
                    {
                        Int64 val = Int64.Parse(nums[incParamNum - 1]);
                        val++;
                        nums[incParamNum - 1] = val.ToString();
                        newVersion = nums[0];
                        for (int i = 1; i < nums.Length; i++)
                        {
                            newVersion += "." + nums[i];
                        }
                        performChange = true;
                    }

                }
                else if (versionStr != null)
                {
                    newVersion = versionStr;
                    performChange = true;
                }

                if (performChange)
                {
                    StringBuilder str = new StringBuilder(line);
                    str.Remove(spos, epos - spos);
                    str.Insert(spos, newVersion);
                    line = str.ToString();
                }
            }
            return line;
        }
    }
}
