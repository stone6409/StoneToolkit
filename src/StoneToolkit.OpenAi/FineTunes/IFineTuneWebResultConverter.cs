using StoneToolkit.OpenAI.OpenAiDtos;

namespace StoneToolkit.OpenAI.FineTunes
{
    internal interface IFineTuneWebResultConverter
    {
        FineTune ConvertFromFineTuneWebResult(FineTuneWebResult webResult);
    }
}