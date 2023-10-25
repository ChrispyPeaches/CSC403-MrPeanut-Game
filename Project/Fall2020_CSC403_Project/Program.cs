using Newtonsoft.Json;
using Refit;
using System;
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

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            using (MainMenu mainMenuForm = new MainMenu(openAiApi))
            {
                if (mainMenuForm.ShowDialog() == DialogResult.OK)
                {
                    // FrmLevel will be started from MainMenu form
                }
            }
        }
    }
}
