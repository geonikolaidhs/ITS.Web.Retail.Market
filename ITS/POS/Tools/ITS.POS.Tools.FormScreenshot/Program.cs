using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using System.Diagnostics;

namespace TakeFormScreenshot
{
    public class Program
    {
        static List<Assembly> LoadedAssemblies;

        static string GetFullMessage(Exception ex)
        {
            if (ex.InnerException == null)
            {
                return ex.Message + Environment.NewLine + "\t" + ex.StackTrace;
            }
            else
            {
                return ex.Message + GetFullMessage(ex.InnerException);
            }
        }

        [STAThread]
        static void Main(string[] args)
        {
            string directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            using (StreamWriter logger = new StreamWriter(directory + "\\log.txt", true))
            {
                logger.AutoFlush = true;
                try
                {
                    LoadedAssemblies = new List<Assembly>();
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Assembly asm = null;
                    try
                    {
                        asm = Assembly.LoadFile(args[0]);
                    }
                    catch (ReflectionTypeLoadException ex)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine(ex.Message);
                        foreach (Exception exSub in ex.LoaderExceptions)
                        {
                            sb.AppendLine(exSub.Message);
                            FileNotFoundException exFileNotFound = exSub as FileNotFoundException;
                            if (exFileNotFound != null)
                            {
                                if (!string.IsNullOrEmpty(exFileNotFound.FusionLog))
                                {
                                    sb.AppendLine("Fusion Log:");
                                    sb.AppendLine(exFileNotFound.FusionLog);
                                }
                            }
                            sb.AppendLine();
                        }
                        string errorMessage = sb.ToString();
                        logger.WriteLine(errorMessage);
                        Console.WriteLine(errorMessage);
                        Environment.Exit(-1);
                    }
                    catch (System.Exception ex)
                    {
                        logger.WriteLine(DateTime.Now + " - " + GetFullMessage(ex));
                        Console.WriteLine(ex.Message);
                        Environment.Exit(-1);
                    }

                    //string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    //foreach (string filename in Directory.GetFiles(path, "*.dll"))
                    //{
                    //    LoadedAssemblies.Add(Assembly.LoadFile(filename));
                    //}
                    try
                    {
                        Type tp = asm.GetTypes().Where(g => g.BaseType.Name.Contains("frmMainBase") || g.BaseType.Name.Contains("frmSupportingBase")).First();
                        if (tp == null)
                        {
                            Environment.Exit(-2);
                        }

                        object ob = null;

                        ob = Activator.CreateInstance(tp);


                        Form frm = ob as Form;
                        Bitmap bmp = new Bitmap(frm.Width, frm.Height);
                        Application.DoEvents();

                        frm.Visible = true;
                        Application.DoEvents();
                        frm.DrawToBitmap(bmp, Rectangle.FromLTRB(0, 0, 2000, 2000));
                        bmp.Save(args[1]);
                        logger.WriteLine(DateTime.Now + " - Success");
                        Environment.Exit(0);
                    }
                    catch (ReflectionTypeLoadException ex)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine(ex.Message);
                        foreach (Exception exSub in ex.LoaderExceptions)
                        {
                            sb.AppendLine(exSub.Message);
                            FileNotFoundException exFileNotFound = exSub as FileNotFoundException;
                            if (exFileNotFound != null)
                            {
                                if (!string.IsNullOrEmpty(exFileNotFound.FusionLog))
                                {
                                    sb.AppendLine("Fusion Log:");
                                    sb.AppendLine(exFileNotFound.FusionLog);
                                }
                            }
                            sb.AppendLine();
                        }
                        string errorMessage = sb.ToString();
                        logger.WriteLine(DateTime.Now + " - " + errorMessage);
                        Console.WriteLine(errorMessage);
                        Environment.Exit(-1);
                    }
                    catch (System.Exception ex)
                    {
                        logger.WriteLine(DateTime.Now + " - " + GetFullMessage(ex));
                        Console.WriteLine(ex.Message);
                        Environment.Exit(-3);
                    }
                }
                catch (System.Exception ex)
                {
                    logger.WriteLine(DateTime.Now + " - " + GetFullMessage(ex));
                    Console.WriteLine(ex.Message);
                    Environment.Exit(-4);
                }
            }



        }

        //static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        //{
        //    Debugger.Break();

        //    foreach (Assembly asmb in LoadedAssemblies)
        //    {
        //        if (asmb.FullName == args.Name)
        //        {
        //            return asmb;
        //        }
        //    }
        //    return null;
        //}
    }
}

