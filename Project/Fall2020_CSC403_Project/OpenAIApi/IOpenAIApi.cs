using Fall2020_CSC403_Project.OpenAIApi;
using Refit;
using System.Threading.Tasks;

namespace Fall2020_CSC403_Project
{
    [Headers("Content-Type: application/json", "Authorization: Bearer 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")]
    /// <summary>
    /// A REST interface for working with the Open AI API
    /// </summary>
    public interface IOpenAIApi
    {
        /// <summary>
        /// Use Open AI's Chat Completion API to generate dialogue with the player
        /// </summary>
        /// <param name="query">A conversational history up to a point in a dialogue</param>
        /// <returns>An AI generated response message for the last message sent which continues the dialogue</returns>
        [Post("/v1/chat/completions")]
        Task<ChatCompletionResponse> GetChatCompletion(ChatCompletionQuery query);
    }
}
