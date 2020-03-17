namespace StoneFruit
{
    public interface IOutputSetup
    {
        /// <summary>
        /// Add a secondary output to receive a copy of output text. This text will not include color and
        /// will not participate with interactive prompts.
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        IOutputSetup Add(IOutput output);
    }
}