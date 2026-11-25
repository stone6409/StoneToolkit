using StoneToolkit.OpenAI.OpenAiDtos;

namespace StoneToolkit.OpenAI.Files
{
    internal interface IFileInfoWebResultConverter
    {
        FileInfo ConvertFromFileInfoWebResult(FileInfoWebResult webResult);
    }
}