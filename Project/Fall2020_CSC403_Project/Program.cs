﻿using Fall2020_CSC403_Project.Properties;
using Newtonsoft.Json;
using Refit;
using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Fall2020_CSC403_Project
{
    static class Program
    {
        private static readonly RefitSettings GlobalRefitSettings = new RefitSettings()
        {
            ContentSerializer = new NewtonsoftJsonContentSerializer(new JsonSerializerSettings())
        };

        /// <summary>
        /// The main entry point for the application.
        /// </summary> 
        [STAThread]
        static void Main()
        {
            // Setup the OpenAi rest service
            IOpenAIApi openAiApi = RestService
                .For<IOpenAIApi>(
                    "https://api.openai.com/",
                    GlobalRefitSettings);

            Settings.Default.AppDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Settings.Default.SavesDirectory = Path.Combine(Settings.Default.AppDirectory, "..", "..", "Saves");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            using (MainMenu mainMenuForm = new MainMenu(openAiApi))
            {
                mainMenuForm.WindowState = FormWindowState.Maximized;

                if (mainMenuForm.ShowDialog() == DialogResult.OK)
                {
                }
            }

        }
    }
}
