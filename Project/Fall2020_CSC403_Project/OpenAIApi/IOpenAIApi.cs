using Fall2020_CSC403_Project.OpenAIApi;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Fall2020_CSC403_Project
{
    [Headers("Content-Type: application/json", "Authorization: Bearer ")]
    public interface IOpenAIApi
    {
        [Post("/v1/chat/completions")]
        Task<ApiResponse<IOpenAIApi>> GetChatCompletion(
            IList<ChatMessage> messages,
            string model = "gpt-3.5-turbo");
    }
}
