using System;
using System.IO;
using System.Text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

/*
 *    command-line utility
 *    reading text from pdf-files and saving text to txt-files
 *    
 *    this programm use NuGet library iTextSharp ver.5.5.13
 *    
 *    usage:
 *    pdftxt.exe [pdf SORCE] [txt TARGET]
 *    return code:
 *              0 - success
 *              1 - if no params or no source folder or file
 *    
 *    created: 20190129  exinte.com.ua
 *    updated: 
 *             20190221
 *             20190211
 *             
 */

namespace pdftxt
{
	class Program
	{
		static int Main(string[] args)
		{

			// -- print usage
			if ((args.Length == 0) || (((args.Length == 1) && (args[0] == "--help"))))
			{
				Console.WriteLine("pdftxt version 1.0.0.0");
				Console.WriteLine("--help usage:");
				Console.WriteLine("pdftxt.exe [pdf source file or folder] [txt target folder]");
				Console.WriteLine("press <ENTER> for continue...");
				Console.ReadLine();
				return 1;
			}

			// -- 1. SOURCE (args[0]):  file or folder
			//
			String s_file = args[0];
			FileInfo s_info;
			DirectoryInfo sd_info;
			String s_find = "*.pdf";

			// if SOURCE file OR folder ?
			if (File.Exists(s_file))
			{
				// -- SOURCE is file
				s_info = new FileInfo(s_file);
				sd_info = new DirectoryInfo(s_info.DirectoryName);
				s_find = s_info.Name;
				Console.WriteLine("Source type is FILE = " + s_info.FullName);
			}
			else
			{   // -- if SOURCE is FOLDER ?
				if (Directory.Exists(s_file))
				{
					sd_info = new DirectoryInfo(s_file);
					Console.WriteLine("Source type is FOLDER = " + sd_info.FullName);
				}
				else
				{
					Console.WriteLine(args[0]);
					Console.WriteLine("Source file or folder does not exist!");
					return 1;
				}
			}
			Console.WriteLine("----------------");

			// -- 2. TARGET (args[1]):  folder for txt-files
            //
			String t_folder = "";
			DirectoryInfo st_info = new DirectoryInfo(".");

			// -- is param for TARGET folder ?
			if (args.Length > 1)
			{
				t_folder = args[1];
			}

			// -- if EXISTS param for TARGET folder 
			// test relative TARGET path
			if (t_folder.Length > 0)
			{
				// have target folder diskname ?
				// or target folder is subfolder ?
				if (t_folder.LastIndexOf(":") < 0)
				{
					t_folder = st_info.FullName + "\\" + t_folder;
				}
			}

			// param TARGET folder="" => set TARGET folder = SORCE folder
			else
			{
				t_folder = sd_info.FullName;
			}

			// -- test EXISTS TARGET folder
			String ti = "";
			DirectoryInfo td_info = new DirectoryInfo(t_folder);
			if (!td_info.Exists)
			{
				// create TARGET folder
				td_info.Create();
			}

			// take file(s) in source folder
			if (sd_info.Exists)
			{
				// count files
				int n_files = sd_info.GetFiles(s_find).Length;

				Console.WriteLine("Source folder: " + sd_info.FullName);
				Console.WriteLine("Target folder: " + td_info.FullName);
				Console.WriteLine("PDF files: " + n_files);

				Console.WriteLine("----------------");
				int i = 0;

				Console.WriteLine("Start: " + DateTime.Now);

				// appling files in source folder
				foreach (var fi in sd_info.GetFiles(s_find))
				{
					i++;
					ti = td_info.FullName + "\\" + fi.Name + ".txt";
					Console.WriteLine(i + ") " + fi.FullName + " --> " + ti + " --> " + (Math.Round((decimal)(100 * i / n_files), 0)).ToString() + "%");
					using (StreamWriter sw = new StreamWriter(ti, false, Encoding.UTF8))
					{
						sw.Write(ExtractTextFromPdf(fi.FullName));
					}
				}
				Console.WriteLine("The End: " + DateTime.Now);
				return 0;
			}

			return 0;
		}

		//====================================================
		public static string ExtractTextFromPdf(string path)
		{
			using (PdfReader reader = new PdfReader(path))
			{
				StringBuilder text = new StringBuilder();

				for (int i = 1; i <= reader.NumberOfPages; i++)
				{
					text.Append(PdfTextExtractor.GetTextFromPage(reader, i));
				}

				return text.ToString();
			}
		}

	}
}
